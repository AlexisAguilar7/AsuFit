using System;

namespace AsuFit.Entidades
{
    // Modelo de datos que representa un registro en la tabla HistorialFisico.
    public class HistorialFisico
    {
        #region 1. PROPIEDADES OBLIGATORIAS
        // Representa el identificador único del registro de historial físico.
        public int IdRegistro { get; set; }

        // Representa el identificador del socio asociado al registro.
        public int IdSocio { get; set; }

        // Representa el peso del socio en kilogramos.
        public decimal Peso { get; set; }

        // Representa la altura del socio (puede recibirse en cm o metros).
        public decimal Altura { get; set; }
        #endregion

        #region 2. PROPIEDADES OPCIONALES Y CALCULADAS
        // Representa el Índice de Masa Corporal (IMC) calculado por el sistema.
        public decimal IMC { get; set; }

        // Representa el porcentaje de grasa corporal estimado (opcional).
        public decimal Grasa { get; set; }

        // Representa la fecha y hora en que se tomó el registro en el sistema.
        public DateTime FechaRegistro { get; set; }
        #endregion
    }
}