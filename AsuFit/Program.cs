using AsuFit.Presentacion;
using System;
using System.IO;
using System.Windows.Forms;

namespace AsuFit
{
    internal static class Program
    {
        #region 1. PUNTO DE ENTRADA Y CONFIGURACIÓN INICIAL
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InstalarEntornoLocal();

            Application.Run(new frmLogin());
        }
        #endregion

        #region 2. DESPLIEGUE AUTOMÁTICO DE RECURSOS Y ENTORNOS
        // Verifica la existencia de recursos locales adyacentes al ejecutable y los despliega en la raíz del sistema (C:\) solo si no existen previamente.
        private static void InstalarEntornoLocal()
        {
            string rutaOrigen = Path.Combine(Application.StartupPath, "AsuFit");
            string rutaDestino = @"C:\AsuFit";

            // Condición de seguridad: Ejecuta la copia únicamente si existe el origen y el destino está vacío (primera ejecución).
            if (Directory.Exists(rutaOrigen) && !Directory.Exists(rutaDestino))
            {
                try
                {
                    CopiarDirectorioRecursivo(rutaOrigen, rutaDestino);
                }
                catch (Exception)
                {
                    // La excepción se silencia intencionalmente para evitar la interrupción del arranque por políticas de permisos del SO.
                }
            }
        }

        // Realiza una copia profunda de subdirectorios y archivos, estructurando el árbol de directorios de forma iterativa.
        private static void CopiarDirectorioRecursivo(string origen, string destino)
        {
            if (!Directory.Exists(destino))
            {
                Directory.CreateDirectory(destino);
            }

            foreach (string dirPath in Directory.GetDirectories(origen, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(origen, destino));
            }

            foreach (string newPath in Directory.GetFiles(origen, "*.*", SearchOption.AllDirectories))
            {
                string destFile = newPath.Replace(origen, destino);

                if (!File.Exists(destFile))
                {
                    File.Copy(newPath, destFile);
                }
            }
        }
        #endregion
    }
}