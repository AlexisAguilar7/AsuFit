using AsuFit.Entidades;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AsuFit.Datos
{
    public class RolDatos
    {
        public DataTable ListarRoles(int estadoFiltro)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection oConexion = Conexion.ObtenerConexion())
                {
                    string query = "SELECT IdRol, NombreRol, Descripcion, CASE WHEN Estado = 1 THEN 'Activo' ELSE 'Inactivo' END AS EstadoTexto FROM Roles WHERE Estado = @estado";
                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@estado", estadoFiltro);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar roles en BD: " + ex.Message);
            }
            return dt;
        }

        public bool CambiarEstado(int idRol, int nuevoEstado)
        {
            try
            {
                using (SqlConnection oConexion = Conexion.ObtenerConexion())
                {
                    string query = "UPDATE Roles SET Estado = @estado WHERE IdRol = @id";
                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@id", idRol);

                    oConexion.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Registra o actualiza un rol en la base de datos
        public int GuardarRol(Rol obj, bool esEdicion)
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection oConexion = Conexion.ObtenerConexion())
                {
                    // Se agrega SCOPE_IDENTITY() para capturar el ID recién creado o SELECT @id para edición
                    string query = esEdicion ?
                        "UPDATE Roles SET NombreRol = @nombre, Descripcion = @desc WHERE IdRol = @id; SELECT @id;" :
                        "INSERT INTO Roles (NombreRol, Descripcion, Estado) VALUES (@nombre, @desc, 1); SELECT SCOPE_IDENTITY();";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@nombre", obj.NombreRol);
                    cmd.Parameters.AddWithValue("@desc", obj.Descripcion);
                    if (esEdicion) cmd.Parameters.AddWithValue("@id", obj.IdRol);

                    oConexion.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null ? Convert.ToInt32(resultado) : 0;
                }
            }
            catch (Exception) { return 0; }
        }

        // Consulta permisos usando el nombre del rol (Para el Dashboard)
        public System.Collections.Generic.List<string> ObtenerPermisosPorNombreRol(string nombreRol)
        {
            var listaPermisos = new System.Collections.Generic.List<string>();
            try
            {
                using (System.Data.SqlClient.SqlConnection oConexion = Conexion.ObtenerConexion())
                {
                    // Blindaje SQL: Forzamos LTRIM y RTRIM para ignorar espacios en la base de datos
                    string query = @"SELECT m.NombreVisible 
                                     FROM PermisosRoles pr 
                                     INNER JOIN Modulos m ON pr.IdModulo = m.IdModulo 
                                     INNER JOIN Roles r ON pr.IdRol = r.IdRol
                                     WHERE LTRIM(RTRIM(r.NombreRol)) = LTRIM(RTRIM(@nombreRol))";

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, oConexion);
                    // Blindaje C#: Limpiamos el parámetro antes de enviarlo
                    cmd.Parameters.AddWithValue("@nombreRol", nombreRol.Trim());

                    oConexion.Open();
                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // Blindaje C#: Limpiamos el resultado devuelto por la BD
                            listaPermisos.Add(dr["NombreVisible"].ToString().Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error SQL en permisos: " + ex.Message);
            }
            return listaPermisos;
        }

        // Obtiene la lista de nombres de módulos a los que un rol tiene acceso
        public System.Collections.Generic.List<string> ObtenerPermisosPorRol(int idRol)
        {
            var listaPermisos = new System.Collections.Generic.List<string>();
            try
            {
                using (System.Data.SqlClient.SqlConnection oConexion = Conexion.ObtenerConexion())
                {
                    string query = @"SELECT m.NombreVisible 
                                     FROM PermisosRoles pr 
                                     INNER JOIN Modulos m ON pr.IdModulo = m.IdModulo 
                                     WHERE pr.IdRol = @idRol";
                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@idRol", idRol);

                    oConexion.Open();
                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // Blindaje C#: Limpiamos el resultado devuelto por la BD
                            listaPermisos.Add(dr["NombreVisible"].ToString().Trim());
                        }
                    }
                }
            }
            catch (Exception) { }
            return listaPermisos;
        }

        // Reemplaza los permisos antiguos por los nuevos mediante una transacción
        public bool ActualizarPermisos(int idRol, System.Collections.Generic.List<string> modulosSeleccionados)
        {
            using (System.Data.SqlClient.SqlConnection oConexion = Conexion.ObtenerConexion())
            {
                oConexion.Open();
                using (System.Data.SqlClient.SqlTransaction transaccion = oConexion.BeginTransaction())
                {
                    try
                    {
                        System.Data.SqlClient.SqlCommand cmdLimpiar = new System.Data.SqlClient.SqlCommand("DELETE FROM PermisosRoles WHERE IdRol = @idRol", oConexion, transaccion);
                        cmdLimpiar.Parameters.AddWithValue("@idRol", idRol);
                        cmdLimpiar.ExecuteNonQuery();

                        string queryInsert = @"INSERT INTO PermisosRoles (IdRol, IdModulo) 
                                               VALUES (@idRol, (SELECT IdModulo FROM Modulos WHERE NombreVisible = @nombreModulo))";

                        foreach (string modulo in modulosSeleccionados)
                        {
                            System.Data.SqlClient.SqlCommand cmdInsert = new System.Data.SqlClient.SqlCommand(queryInsert, oConexion, transaccion);
                            cmdInsert.Parameters.AddWithValue("@idRol", idRol);
                            cmdInsert.Parameters.AddWithValue("@nombreModulo", modulo);
                            cmdInsert.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaccion.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}