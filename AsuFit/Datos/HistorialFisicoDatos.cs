using AsuFit.Entidades;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AsuFit.Datos
{
    // Gestiona las transacciones directas con la base de datos para el historial físico.
    public class HistorialFisicoDatos
    {
        #region 1. LECTURA DE DATOS
        // Ejecuta una consulta para obtener el historial filtrado por socio ordenado por fecha.
        public DataTable ObtenerHistorialPorSocio(int idSocio)
        {
            DataTable dtResultado = new DataTable("HistorialFisico");
            using (SqlConnection sqlCon = Conexion.ObtenerConexion())
            {
                try
                {
                    string query = "SELECT IdRegistro, IdSocio, Peso, Altura, IMC, Grasa, FechaRegistro " +
                                   "FROM HistorialFisico WHERE IdSocio = @IdSocio ORDER BY FechaRegistro DESC";
                    SqlCommand comando = new SqlCommand(query, sqlCon);
                    comando.Parameters.AddWithValue("@IdSocio", idSocio);

                    SqlDataAdapter sqlDat = new SqlDataAdapter(comando);
                    sqlDat.Fill(dtResultado);
                }
                catch (Exception ex)
                {
                    dtResultado = null;
                    throw new Exception("Error al obtener el historial físico: " + ex.Message);
                }
            }
            return dtResultado;
        }
        #endregion

        #region 2. ESCRITURA DE DATOS
        // Inserta un nuevo registro de evaluación física en la tabla correspondiente manejando nulos.
        public bool RegistrarEvaluacion(HistorialFisico objHistorial)
        {
            bool respuesta = false;
            using (SqlConnection sqlCon = Conexion.ObtenerConexion())
            {
                try
                {
                    string query = "INSERT INTO HistorialFisico (IdSocio, Peso, Altura, IMC, Grasa, FechaRegistro) " +
                                   "VALUES (@IdSocio, @Peso, @Altura, @IMC, @Grasa, @FechaRegistro)";
                    SqlCommand comando = new SqlCommand(query, sqlCon);
                    comando.Parameters.AddWithValue("@IdSocio", objHistorial.IdSocio);
                    comando.Parameters.AddWithValue("@Peso", objHistorial.Peso);
                    comando.Parameters.AddWithValue("@Altura", objHistorial.Altura);
                    comando.Parameters.AddWithValue("@IMC", objHistorial.IMC);

                    if (objHistorial.Grasa > 0)
                        comando.Parameters.AddWithValue("@Grasa", objHistorial.Grasa);
                    else
                        comando.Parameters.AddWithValue("@Grasa", DBNull.Value);

                    comando.Parameters.AddWithValue("@FechaRegistro", objHistorial.FechaRegistro);

                    sqlCon.Open();
                    respuesta = comando.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al insertar el historial físico: " + ex.Message);
                }
            }
            return respuesta;
        }
        #endregion
    }
}