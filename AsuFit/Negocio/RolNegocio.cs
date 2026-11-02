using AsuFit.Datos;
using AsuFit.Entidades;
using System;
using System.Data;

namespace AsuFit.Negocio
{
    public class RolNegocio
    {
        private RolDatos objDatos = new RolDatos();

        public DataTable ListarRoles(int estadoFiltro)
        {
            try
            {
                return objDatos.ListarRoles(estadoFiltro);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CambiarEstado(int idRol, int nuevoEstado, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                bool resultado = objDatos.CambiarEstado(idRol, nuevoEstado);
                if (!resultado)
                {
                    mensaje = "No se pudo actualizar el estado del rol.";
                }
                return resultado;
            }
            catch (Exception ex)
            {
                mensaje = "Error de negocio: " + ex.Message;
                return false;
            }
        }

        public int GuardarRol(Rol obj, bool esEdicion, out string mensaje)
        {
            mensaje = string.Empty;
            if (string.IsNullOrWhiteSpace(obj.NombreRol) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                mensaje = "Todos los campos son obligatorios.";
                return 0;
            }

            int idGenerado = objDatos.GuardarRol(obj, esEdicion);
            if (idGenerado == 0) mensaje = "Error al intentar guardar el rol en la base de datos.";

            return idGenerado;
        }

        // Consulta permisos usando el nombre del rol (Para el Dashboard)
        public System.Collections.Generic.List<string> ObtenerPermisosPorNombreRol(string nombreRol)
        {
            return objDatos.ObtenerPermisosPorNombreRol(nombreRol);
        }

        public System.Collections.Generic.List<string> ObtenerPermisosPorRol(int idRol)
        {
            return objDatos.ObtenerPermisosPorRol(idRol);
        }

        public bool ActualizarPermisos(int idRol, System.Collections.Generic.List<string> modulosSeleccionados, out string mensaje)
        {
            mensaje = string.Empty;
            bool resultado = objDatos.ActualizarPermisos(idRol, modulosSeleccionados);
            if (!resultado) mensaje = "No se pudieron actualizar los permisos del rol.";
            return resultado;
        }
    }
}