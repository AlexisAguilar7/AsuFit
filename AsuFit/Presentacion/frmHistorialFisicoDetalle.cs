using AsuFit.Negocio;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AsuFit.Presentacion
{
    public partial class frmHistorialFisicoDetalle : Form
    {
        #region 1. VARIABLES GLOBALES Y CONSTRUCTOR
        private int _idSocioActual;
        private DataTable _dtHistorialCompleto;
        private HistorialFisicoNegocio _negocioHistorial = new HistorialFisicoNegocio();

        // Inicializa el formulario detallado recibiendo el identificador del socio a consultar.
        public frmHistorialFisicoDetalle(int idSocio)
        {
            InitializeComponent();
            _idSocioActual = idSocio;
            dgvHistorial.AutoGenerateColumns = false;

            this.Load += FrmHistorialFisicoDetalle_Load;
            this.Shown += FrmHistorialFisicoDetalle_Shown;
        }
        #endregion

        #region 2. ESTILOS VISUALES Y COMPORTAMIENTO UI
        // Aplica el estilo visual oscuro estándar del sistema al control DataGridView.
        private void ConfigurarTemaOscuroGrilla(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.FromArgb(25, 28, 35);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(50, 55, 65);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 39, 47);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(35, 39, 47);

            dgv.DefaultCellStyle.BackColor = Color.FromArgb(25, 28, 35);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 229, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowTemplate.Height = 35;
        }

        // Gestiona el comportamiento de la marca de agua con desvanecimiento dinámico estilo AsuFit.
        private void AplicarPlaceholder(TextBox txt, string textoAyuda)
        {
            txt.Tag = textoAyuda;

            if (string.IsNullOrWhiteSpace(txt.Text) || txt.Text == textoAyuda)
            {
                txt.Text = textoAyuda;
                txt.ForeColor = Color.Silver;
            }
            else
            {
                txt.ForeColor = Color.White;
            }

            txt.Enter += delegate
            {
                if (txt.Text == textoAyuda)
                {
                    this.BeginInvoke(new Action(() => txt.SelectionStart = 0));
                }
            };

            txt.MouseDown += delegate
            {
                if (txt.Text == textoAyuda)
                {
                    txt.SelectionStart = 0;
                    txt.SelectionLength = 0;
                }
            };

            txt.MouseMove += delegate
            {
                if (txt.Text == textoAyuda && txt.SelectionLength > 0)
                {
                    txt.SelectionStart = 0;
                    txt.SelectionLength = 0;
                }
            };

            txt.TextChanged += delegate
            {
                if (txt.Text != textoAyuda && txt.ForeColor == Color.Silver)
                {
                    string entradaUsuario = txt.Text.Replace(textoAyuda, "");
                    txt.ForeColor = Color.White;
                    txt.Text = entradaUsuario;
                    txt.SelectionStart = txt.Text.Length;
                }
                else if (string.IsNullOrEmpty(txt.Text))
                {
                    txt.ForeColor = Color.Silver;
                    txt.Text = textoAyuda;
                    txt.SelectionStart = 0;
                }
            };

            txt.KeyDown += delegate (object sender, KeyEventArgs e)
            {
                if (txt.Text == textoAyuda && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right))
                {
                    e.SuppressKeyPress = true;
                }
            };
        }
        #endregion

        #region 3. INICIALIZACIÓN Y CARGA DE DATOS
        // Orquesta la inicialización de estilos visuales y la carga del histórico del socio.
        private void FrmHistorialFisicoDetalle_Load(object sender, EventArgs e)
        {
            ConfigurarTemaOscuroGrilla(dgvHistorial);
            AplicarPlaceholder(txtBuscar, "Buscar por fecha o medidas...");
            CargarHistorial();

            txtBuscar.TextChanged += txtBuscar_TextChanged;
            dgvHistorial.DataBindingComplete += dgvHistorial_DataBindingComplete;
            this.ActiveControl = null;
        }

        // Obtiene el historial completo desde la capa de Negocio y vincula las columnas visuales.
        private void CargarHistorial()
        {
            try
            {
                _dtHistorialCompleto = _negocioHistorial.ObtenerHistorialPorSocio(_idSocioActual);
                dgvHistorial.DataSource = _dtHistorialCompleto;

                if (dgvHistorial.Columns.Contains("colFecha")) dgvHistorial.Columns["colFecha"].DataPropertyName = "FechaRegistro";
                if (dgvHistorial.Columns.Contains("colPeso")) dgvHistorial.Columns["colPeso"].DataPropertyName = "Peso";
                if (dgvHistorial.Columns.Contains("colAltura")) dgvHistorial.Columns["colAltura"].DataPropertyName = "Altura";
                if (dgvHistorial.Columns.Contains("colGrasa")) dgvHistorial.Columns["colGrasa"].DataPropertyName = "Grasa";
                if (dgvHistorial.Columns.Contains("colIMC")) dgvHistorial.Columns["colIMC"].DataPropertyName = "IMC";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 4. EVENTOS Y FILTROS
        // Ejecuta el filtro dinámico en memoria sobre el origen de datos basado en el texto ingresado.
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (_dtHistorialCompleto == null) return;

            string filtro = txtBuscar.Text.Trim();
            string placeholder = txtBuscar.Tag != null ? txtBuscar.Tag.ToString() : "";

            if (string.IsNullOrEmpty(filtro) || filtro == placeholder)
            {
                _dtHistorialCompleto.DefaultView.RowFilter = "";
            }
            else
            {
                _dtHistorialCompleto.DefaultView.RowFilter = string.Format(
                    "Convert(FechaRegistro, 'System.String') LIKE '%{0}%' OR " +
                    "Convert(Peso, 'System.String') LIKE '%{0}%' OR " +
                    "Convert(IMC, 'System.String') LIKE '%{0}%'", filtro);
            }
        }

        // Limpia la selección automática del sistema una vez renderizados los datos.
        private void dgvHistorial_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvHistorial.ClearSelection();
        }

        // Libera la selección automática del sistema y aplica el formato de fecha forzado.
        private void FrmHistorialFisicoDetalle_Shown(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            dgvHistorial.ClearSelection();

            // Aplica el formato estricto con barras literales, idéntico a frmConfiguracion
            if (dgvHistorial.Columns.Contains("colFecha"))
            {
                dgvHistorial.Columns["colFecha"].DefaultCellStyle.Format = "dd\\/MM\\/yyyy";
            }
        }

        // Cierra el formulario modal y retorna a la vista principal.
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}