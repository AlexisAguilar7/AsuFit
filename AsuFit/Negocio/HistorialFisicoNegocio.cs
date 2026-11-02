using AsuFit.Datos;
using AsuFit.Entidades;
using System;
using System.Data;

namespace AsuFit.Negocio
{
    // Orquesta las reglas de negocio, cálculos matemáticos y validaciones para el progreso físico.
    public class HistorialFisicoNegocio
    {
        // Instancia de la capa de datos para interactuar con la base de datos.
        private HistorialFisicoDatos objHistorialDatos = new HistorialFisicoDatos();

        #region 1. CONSULTAS Y BÚSQUEDAS
        // Obtiene el historial físico completo de un socio mediante la capa de datos.
        public DataTable ObtenerHistorialPorSocio(int idSocio)
        {
            if (idSocio <= 0) return new DataTable();
            return objHistorialDatos.ObtenerHistorialPorSocio(idSocio);
        }
        #endregion

        #region 2. OPERACIONES Y VALIDACIONES
        // Valida los parámetros obligatorios, asegura la conversión a metros y envía el registro.
        public bool RegistrarEvaluacion(HistorialFisico objHistorial, out string mensaje)
        {
            mensaje = string.Empty;

            if (objHistorial.IdSocio <= 0)
            {
                mensaje = "El identificador del socio no es válido.";
                return false;
            }

            if (objHistorial.Peso <= 0 || objHistorial.Altura <= 0)
            {
                mensaje = "El peso y la altura son campos obligatorios y deben ser mayores a cero.";
                return false;
            }

            // Convertimos la altura a metros incondicionalmente ANTES de guardar
            decimal alturaMetros = objHistorial.Altura > 3 ? (objHistorial.Altura / 100) : objHistorial.Altura;
            objHistorial.Altura = Math.Round(alturaMetros, 2);

            if (objHistorial.IMC <= 0)
            {
                decimal imcCalculado = objHistorial.Peso / (alturaMetros * alturaMetros);
                objHistorial.IMC = Math.Round(imcCalculado, 2);
            }

            bool respuestaBD = objHistorialDatos.RegistrarEvaluacion(objHistorial);
            if (!respuestaBD)
            {
                mensaje = "Ocurrió un error al intentar registrar la evaluación en la base de datos.";
            }

            return respuestaBD;
        }
        #endregion
    }
}