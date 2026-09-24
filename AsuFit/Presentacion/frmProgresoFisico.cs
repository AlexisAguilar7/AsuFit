using AsuFit.Entidades;
using AsuFit.Negocio;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace AsuFit.Presentacion
{
    public partial class frmProgresoFisico : Form
    {
        #region 1. VARIABLES Y CONSTRUCTOR
        private Socio _socioActual; // Cambiamos los datos sueltos por el objeto completo
        private DataTable _dtHistorial;
        private HistorialFisicoNegocio _negocioHistorial = new HistorialFisicoNegocio();
        private string _rutaFotoOrigen = "";
        private const string _carpetaFotosSocios = @"C:\AsuFit\Fotos\Socios\";

        // Inicializa el formulario recibiendo el objeto Socio con sus datos estáticos.
        public frmProgresoFisico(Socio socio)
        {
            InitializeComponent();
            _socioActual = socio;
            this.Load += FrmProgresoFisico_Load;
        }
        #endregion

        #region 2. INICIALIZACIÓN Y CARGA DE DATOS
        // Orquesta la configuración visual inicial y la carga de datos del socio al abrir el formulario.
        private void FrmProgresoFisico_Load(object sender, EventArgs e)
        {
            ConfigurarFormatoVisual();
            CargarDatosSocio();
            ActualizarPanelGrafico();
            AsignarEventosCalculo();
        }

        // Establece las propiedades de inicio de los controles de entrada y diseño del gráfico.
        private void ConfigurarFormatoVisual()
        {
            dtpFechaEvaluacion.Value = DateTime.Now;
            if (txtFechaVisual != null) txtFechaVisual.Text = dtpFechaEvaluacion.Value.ToString("dd\\/MM\\/yyyy");

            chartEvolucion.Series.Clear();
        }

        // Recupera los datos personales del socio desde la memoria y su última métrica corporal desde SQL.
        private void CargarDatosSocio()
        {
            // 1. Cargar datos personales directamente desde el objeto en memoria (0 delay)
            lblNombreSocio.Text = $"{_socioActual.Nombre} {_socioActual.Apellido}".Trim();
            lblCedula.Text = "Cédula: " + _socioActual.Cedula;
            lblTelefono.Text = "Teléfono: " + _socioActual.Telefono;
            lblPlanActual.Text = "Plan actual: " + _socioActual.NombrePlan;

            lblEstadoSocio.Text = _socioActual.Estado;
            lblEstadoSocio.ForeColor = _socioActual.Estado == "Activo" ? Color.LimeGreen : Color.LightCoral;

            if (_socioActual.FechaVencimiento.HasValue)
                lblVencimiento.Text = "Vencimiento: " + _socioActual.FechaVencimiento.Value.ToString("dd'/'MM'/'yyyy");
            else
                lblVencimiento.Text = "Vencimiento: Sin plan activo";

            // 2. Cargar la última medición física para los recuadros superiores
            _dtHistorial = _negocioHistorial.ObtenerHistorialPorSocio(_socioActual.IdSocio);
            if (_dtHistorial != null && _dtHistorial.Rows.Count > 0)
            {
                DataRow ultimaMedicion = _dtHistorial.Rows[0];
                lblAlturaActual.Text = ultimaMedicion["Altura"].ToString() + " cm";
                lblPesoActual.Text = ultimaMedicion["Peso"].ToString() + " kg";
                lblImcActual.Text = ultimaMedicion["IMC"].ToString();

                if (ultimaMedicion["Grasa"] != DBNull.Value)
                    lblGrasaActual.Text = ultimaMedicion["Grasa"].ToString() + " %";
                else
                    lblGrasaActual.Text = "N/A";
            }
            else
            {
                lblAlturaActual.Text = "0 cm";
                lblPesoActual.Text = "0 kg";
                lblGrasaActual.Text = "0 %";
                lblImcActual.Text = "0";
            }

            // 3. Cargar la foto de perfil del socio (si existe)
            string rutaFotoPerfil = _carpetaFotosSocios + _socioActual.Cedula + ".jpg";
            if (File.Exists(rutaFotoPerfil))
            {
                using (Image imgTemp = Image.FromFile(rutaFotoPerfil))
                {
                    pbFotoSocio.Image = new Bitmap(imgTemp);
                }
            }
            else
            {
                pbFotoSocio.Image = null; // Muestra vacío si no tiene foto
            }
        }

        // Convierte puntos y comas al separador decimal nativo del sistema operativo
        private decimal ParsearDecimalSeguro(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0;

            string separadorSistema = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string textoLimpio = texto.Replace(".", separadorSistema).Replace(",", separadorSistema);

            decimal.TryParse(textoLimpio, out decimal resultado);
            return resultado;
        }

        // Suscribe los controles de entrada a la validación en tiempo real.
        private void AsignarEventosCalculo()
        {
            txtPeso.TextChanged += TextBoxes_TextChanged;
            txtAltura.TextChanged += TextBoxes_TextChanged;
            dtpFechaEvaluacion.ValueChanged += DtpFechaEvaluacion_ValueChanged;
        }
        #endregion

        #region 3. EVENTOS DE ENTRADA Y CÁLCULOS DINÁMICOS
        // Sincroniza la fecha seleccionada con el cuadro de texto visual oscuro.
        private void DtpFechaEvaluacion_ValueChanged(object sender, EventArgs e)
        {
            if (txtFechaVisual != null)
                txtFechaVisual.Text = dtpFechaEvaluacion.Value.ToString("dd\\/MM\\/yyyy");
        }

        // Dispara el cálculo automático del IMC cada vez que cambian los valores de peso o altura.
        private void TextBoxes_TextChanged(object sender, EventArgs e)
        {
            CalcularImcEnTiempoReal();
        }

        // Procesa la entrada del usuario, unifica unidades de medida y actualiza el campo IMC.
        private void CalcularImcEnTiempoReal()
        {
            if (decimal.TryParse(txtPeso.Text, out decimal peso) && peso > 0 &&
                decimal.TryParse(txtAltura.Text, out decimal altura) && altura > 0)
            {
                decimal alturaMetros = altura > 3 ? (altura / 100) : altura;
                decimal imcCalculado = peso / (alturaMetros * alturaMetros);
                txtIMC.Text = Math.Round(imcCalculado, 2).ToString();
            }
            else
            {
                txtIMC.Clear();
            }
        }
        #endregion

        #region 4. ACCIONES PRINCIPALES
        // Recolecta los datos ingresados, instancia la entidad y delega la persistencia a la capa de Negocio.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            decimal peso = ParsearDecimalSeguro(txtPeso.Text);
            decimal altura = ParsearDecimalSeguro(txtAltura.Text);
            decimal grasa = ParsearDecimalSeguro(txtGrasa.Text);
            decimal imc = ParsearDecimalSeguro(txtIMC.Text);

            HistorialFisico nuevoRegistro = new HistorialFisico
            {
                IdSocio = _socioActual.IdSocio,
                Peso = peso,
                Altura = altura,
                Grasa = grasa,
                IMC = imc,
                FechaRegistro = dtpFechaEvaluacion.Value
            };

            bool exito = _negocioHistorial.RegistrarEvaluacion(nuevoRegistro, out string mensaje);

            if (exito)
            {
                MessageBox.Show("Evaluación guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
                ActualizarPanelGrafico();
                CargarDatosSocio();
            }
            else
            {
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Permite al usuario seleccionar una imagen de perfil y la asocia físicamente a la cédula del socio.
        private void btnSubirFoto_Click(object sender, EventArgs e)
        {
            if (_socioActual == null) return;

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Imágenes|*.jpg;*.jpeg;*.png";
            openFile.Title = "Seleccionar Foto de Perfil";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                _rutaFotoOrigen = openFile.FileName;
                try
                {
                    // Mostrar la previsualización en pantalla
                    using (Image imgTemp = Image.FromFile(_rutaFotoOrigen))
                    {
                        pbFotoSocio.Image = new Bitmap(imgTemp);
                    }

                    // Crear el directorio maestro si por algún motivo fue borrado del sistema
                    if (!Directory.Exists(_carpetaFotosSocios)) Directory.CreateDirectory(_carpetaFotosSocios);

                    // Guardado físico definitivo en el disco duro (Sobrescribe si ya existía una)
                    string rutaDestino = _carpetaFotosSocios + _socioActual.Cedula + ".jpg";
                    if (File.Exists(rutaDestino))
                    {
                        File.Delete(rutaDestino);
                    }
                    File.Copy(_rutaFotoOrigen, rutaDestino);

                    MessageBox.Show("Foto de perfil actualizada y guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (OutOfMemoryException)
                {
                    MessageBox.Show("El formato de esta imagen no es compatible o el archivo está dañado.", "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _rutaFotoOrigen = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar la foto: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Restablece los campos de entrada a su estado predeterminado.
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            txtPeso.Clear();
            txtAltura.Clear();
            txtGrasa.Clear();
            txtIMC.Clear();
            dtpFechaEvaluacion.Value = DateTime.Now;
            txtPeso.Focus();
        }

        // Cierra el formulario actual y retorna a la ventana anterior.
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 5. GESTIÓN DEL GRÁFICO
        // Consulta el histórico actualizado y renderiza por defecto la métrica de Peso.
        private void ActualizarPanelGrafico()
        {
            _dtHistorial = _negocioHistorial.ObtenerHistorialPorSocio(_socioActual.IdSocio);
            if (_dtHistorial != null && _dtHistorial.Rows.Count > 0)
            {
                ResaltarBotonFiltro(btnFiltroPeso);
                DibujarGrafico("Peso");
            }
            else
            {
                chartEvolucion.Series.Clear();
            }
        }

        // Genera la representación visual de barras con escalas, formato de unidades y diseño limpio.
        private void DibujarGrafico(string metrica)
        {
            chartEvolucion.Series.Clear();
            Series serie = new Series(metrica)
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Color = Color.FromArgb(0, 229, 255)
            };

            for (int i = _dtHistorial.Rows.Count - 1; i >= 0; i--)
            {
                DataRow fila = _dtHistorial.Rows[i];
                DateTime fecha = Convert.ToDateTime(fila["FechaRegistro"]);

                if (fila[metrica] == DBNull.Value) continue;

                decimal valor = Convert.ToDecimal(fila[metrica]);
                string etiquetaFecha = fecha.ToString("dd'/'MM\nyyyy");

                if (metrica == "Altura" && valor < 3) valor = valor * 100;

                // Agrega el punto a la serie y captura su índice posicional.
                int indexPunto = serie.Points.AddXY(etiquetaFecha, valor);

                // Aplica diferenciación de color basándose en el índice de ordenamiento de SQL (0 = Registro más reciente).
                if (i == 0)
                {
                    serie.Points[indexPunto].Color = Color.FromArgb(0, 229, 255);
                }
                else
                {
                    serie.Points[indexPunto].Color = Color.FromArgb(30, 80, 95);
                }
            }

            chartEvolucion.Series.Add(serie);

            // Configuraciones de ejes (ChartArea)
            ChartArea area = chartEvolucion.ChartAreas[0];
            area.AxisX.Interval = 1;
            area.AxisY.IsStartedFromZero = false;

            // Elimina las líneas verticales de la cuadrícula para un diseño más limpio
            area.AxisX.MajorGrid.Enabled = false;

            // Escalas dinámicas y sufijos de unidades basados en la métrica
            switch (metrica)
            {
                case "Peso":
                    area.AxisY.Interval = 5;
                    area.AxisY.Minimum = 35;
                    area.AxisY.Maximum = 120;
                    area.AxisY.LabelStyle.Format = "0 'kg'";
                    break;
                case "Altura":
                    area.AxisY.Interval = 10;
                    area.AxisY.Minimum = 130;
                    area.AxisY.Maximum = 230;
                    area.AxisY.LabelStyle.Format = "0 'cm'";
                    break;
                case "Grasa":
                    area.AxisY.Interval = 5;
                    area.AxisY.Minimum = 0;
                    area.AxisY.Maximum = 40;
                    area.AxisY.LabelStyle.Format = "0 '%'";
                    break;
                case "IMC":
                    area.AxisY.Interval = 5;
                    area.AxisY.Minimum = 15;
                    area.AxisY.Maximum = 50;
                    area.AxisY.LabelStyle.Format = "0"; // Sin sufijo
                    break;
            }
        }

        // Cambia el color del botón activo para indicar la métrica seleccionada en el gráfico.
        private void ResaltarBotonFiltro(Button botonActivo)
        {
            Color colorNormal = Color.FromArgb(0, 229, 255); // Cyan claro (Normal)
            Color colorActivo = Color.FromArgb(0, 130, 150); // Cyan oscuro (Presionado/Activo)

            // 1. Restablecer todos a su color normal
            btnFiltroPeso.BackColor = colorNormal;
            btnFiltroAltura.BackColor = colorNormal;
            btnFiltroGrasa.BackColor = colorNormal;
            btnFiltroIMC.BackColor = colorNormal;

            // 2. Resaltar únicamente el que fue clickeado
            botonActivo.BackColor = colorActivo;
        }

        // Eventos de los filtros visuales
        private void btnFiltroPeso_Click(object sender, EventArgs e)
        {
            ResaltarBotonFiltro(btnFiltroPeso);
            DibujarGrafico("Peso");
        }

        private void btnFiltroAltura_Click(object sender, EventArgs e)
        {
            ResaltarBotonFiltro(btnFiltroAltura);
            DibujarGrafico("Altura");
        }

        private void btnFiltroGrasa_Click(object sender, EventArgs e)
        {
            ResaltarBotonFiltro(btnFiltroGrasa);
            DibujarGrafico("Grasa");
        }

        private void btnFiltroIMC_Click(object sender, EventArgs e)
        {
            ResaltarBotonFiltro(btnFiltroIMC);
            DibujarGrafico("IMC");
        }
        #endregion

        #region 6. NAVEGACIÓN
        // Despliega la vista detallada del historial en un formulario modal.
        private void btnVerHistorial_Click(object sender, EventArgs e)
        {
            using (frmHistorialFisicoDetalle frmDetalle = new frmHistorialFisicoDetalle(_socioActual.IdSocio)) // <-- Cambio aquí
            {
                frmDetalle.ShowDialog();
            }
        }
        #endregion
    }
}