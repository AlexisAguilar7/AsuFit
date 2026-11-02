using AsuFit.Entidades;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AsuFit.Presentacion
{
    public partial class frmGestionRoles : Form
    {
        private Usuario usuarioActual;
        private int idRolSeleccionado = 0;

        public frmGestionRoles(Usuario userLogueado)
        {
            InitializeComponent();
            usuarioActual = userLogueado;
            this.Load += new EventHandler(frmGestionRoles_Load);
        }

        private void frmGestionRoles_Load(object sender, EventArgs e)
        {
            ConfigurarTemaOscuro();
            CargarGrilla();
        }

        #region MÉTODOS DE LA GRILLA Y CHECKBOX
        private void CargarGrilla()
        {
            try
            {
                AsuFit.Negocio.RolNegocio negocio = new AsuFit.Negocio.RolNegocio();
                int estadoFiltro = chkMostrarInactivos.Checked ? 0 : 1;

                if (this.Controls.Find("dgvRoles", true)[0] is DataGridView dgv)
                {
                    var datos = negocio.ListarRoles(estadoFiltro);
                    dgv.DataSource = datos;
                    dgv.ClearSelection();
                    dgv.CurrentCell = null;

                    Control[] lbls = this.Controls.Find("lblTotal", true);
                    if (lbls.Length > 0 && lbls[0] is Label lbl)
                    {
                        lbl.Text = "Roles registrados: " + datos.Rows.Count;
                    }
                }
                idRolSeleccionado = 0;
            }
            catch (Exception ex)
            {
                MensajeAsuFit.Mostrar("Error al cargar la lista de roles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvRoles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (this.Controls.Find("dgvRoles", true)[0] is DataGridView dgv)
                {
                    idRolSeleccionado = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["colIdRol"].Value);
                }
            }
        }

        private void dgvRoles_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (this.Controls.Find("dgvRoles", true)[0] is DataGridView dgv)
            {
                dgv.ClearSelection();
                dgv.CurrentCell = null;
            }
        }

        private void chkMostrarInactivos_CheckedChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        private void frmGestionRoles_Click(object sender, EventArgs e)
        {
            if (this.Controls.Find("dgvRoles", true).Length > 0 && this.Controls.Find("dgvRoles", true)[0] is DataGridView dgv)
            {
                dgv.ClearSelection();
                dgv.CurrentCell = null;
            }
            idRolSeleccionado = 0;
        }
        #endregion

        #region EVENTOS DE BOTONES Y NAVEGACIÓN UI
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            frmRegistrarRol ventanaRegistro = new frmRegistrarRol(usuarioActual);
            PrepararFormularioModal(ventanaRegistro);

            DialogResult resultado = ventanaRegistro.ShowDialog();

            if (resultado == DialogResult.Yes) // Si se creó un rol nuevo...
            {
                int idNuevoRol = Convert.ToInt32(ventanaRegistro.Tag);

                Rol rolParaPermisos = new Rol
                {
                    IdRol = idNuevoRol,
                    // No tenemos el nombre aquí, pero no importa, la BD ya lo tiene. Puedes pasar "" o extraerlo si lo guardas.
                    NombreRol = "Rol Nuevo"
                };

                frmGestionPermisos ventanaPermisos = new frmGestionPermisos(rolParaPermisos, usuarioActual);
                PrepararFormularioModal(ventanaPermisos); // Esto aplica la escala perfectamente

                DialogResult resultadoPermisos = ventanaPermisos.ShowDialog();

                if (resultadoPermisos != DialogResult.OK)
                {
                    MensajeAsuFit.Mostrar("El rol fue creado, pero no se le asignaron permisos. Puede asignarlos más tarde.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            CargarGrilla();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idRolSeleccionado > 0)
            {
                DataGridViewRow fila = ((DataGridView)this.Controls.Find("dgvRoles", true)[0]).CurrentRow;
                Rol rolParaEditar = new Rol
                {
                    IdRol = idRolSeleccionado,
                    NombreRol = fila.Cells["colNombreRol"].Value.ToString(),
                    Descripcion = fila.Cells["colDescripcion"].Value.ToString()
                };

                frmRegistrarRol ventanaRegistro = new frmRegistrarRol(rolParaEditar, usuarioActual);
                PrepararFormularioModal(ventanaRegistro);
                ventanaRegistro.ShowDialog();
                CargarGrilla();
            }
            else
            {
                MensajeAsuFit.Mostrar("Por favor, seleccioná un rol de la tabla.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEstado_Click(object sender, EventArgs e)
        {
            if (idRolSeleccionado > 0)
            {
                int nuevoEstado = chkMostrarInactivos.Checked ? 1 : 0;
                string estadoAccion = nuevoEstado == 1 ? "Activar" : "Inactivar";
                string estadoFinal = nuevoEstado == 1 ? "Activo" : "Inactivo"; // Para el log

                // Obtenemos el nombre del rol directamente de la grilla
                DataGridViewRow fila = ((DataGridView)this.Controls.Find("dgvRoles", true)[0]).CurrentRow;
                string nombreRol = fila.Cells["colNombreRol"].Value.ToString();

                DialogResult resultado = MensajeAsuFit.Mostrar($"¿Desea {estadoAccion} este rol?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    AsuFit.Negocio.RolNegocio negocio = new AsuFit.Negocio.RolNegocio();
                    string mensaje;

                    if (negocio.CambiarEstado(idRolSeleccionado, nuevoEstado, out mensaje))
                    {
                        // REEMPLAZA LA LÍNEA DE AUDITORÍA POR ESTA:
                        AsuFit.Datos.GestorAuditoria.Registrar(usuarioActual.NombreCompleto, "Roles", "Cambio de Estado", $"Se cambió el estado del rol '{nombreRol}' a {estadoFinal}.");

                        MensajeAsuFit.Mostrar("Estado actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarGrilla();
                    }
                    else
                    {
                        MensajeAsuFit.Mostrar(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MensajeAsuFit.Mostrar("Por favor, seleccioná un rol de la tabla.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPermisos_Click(object sender, EventArgs e)
        {
            if (idRolSeleccionado > 0)
            {
                DataGridViewRow fila = ((DataGridView)this.Controls.Find("dgvRoles", true)[0]).CurrentRow;
                Rol rolSeleccionado = new Rol
                {
                    IdRol = idRolSeleccionado,
                    NombreRol = fila.Cells["colNombreRol"].Value.ToString()
                };

                frmGestionPermisos ventanaPermisos = new frmGestionPermisos(rolSeleccionado, usuarioActual);
                PrepararFormularioModal(ventanaPermisos);
                ventanaPermisos.ShowDialog();
            }
            else
            {
                MensajeAsuFit.Mostrar("Por favor, seleccioná un rol para gestionar sus permisos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region ESTILOS VISUALES Y ESCALADO (UI)
        private void ConfigurarTemaOscuro()
        {
            this.BackColor = Color.FromArgb(25, 28, 35);

            foreach (Control c in this.Controls)
            {
                if (c is Panel || c is GroupBox)
                {
                    c.BackColor = Color.FromArgb(35, 39, 47);
                    c.ForeColor = Color.White;

                    foreach (Control subC in c.Controls)
                    {
                        if (subC is Label lbl) lbl.ForeColor = Color.White;
                        if (subC is CheckBox chk) chk.ForeColor = Color.White;
                    }
                }
                else if (c is Label lbl) lbl.ForeColor = Color.White;
                else if (c is CheckBox chk) chk.ForeColor = Color.White;
                else if (c is Button btn)
                {
                    btn.BackColor = Color.FromArgb(0, 229, 255);
                    btn.ForeColor = Color.Black;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Cursor = Cursors.Hand;
                }
            }

            if (this.Controls.Find("dgvRoles", true).Length > 0 && this.Controls.Find("dgvRoles", true)[0] is DataGridView dgv)
            {
                ConfigurarTemaOscuroGrilla(dgv);
            }
        }

        private void ConfigurarTemaOscuroGrilla(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.FromArgb(25, 28, 35);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(50, 55, 65);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 39, 47);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(35, 39, 47);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

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

            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void PrepararFormularioModal(Form frm)
        {
            float escalaActual = Properties.Settings.Default.EscalaInterfaz;
            frm.Scale(new SizeF(escalaActual, escalaActual));

            frm.StartPosition = FormStartPosition.Manual;
            if (this.Parent != null)
            {
                Point posicionPanelAbsoluta = this.Parent.PointToScreen(Point.Empty);
                int x = posicionPanelAbsoluta.X + (this.Parent.Width - frm.Width) / 2;
                int y = posicionPanelAbsoluta.Y + (this.Parent.Height - frm.Height) / 2;
                frm.Location = new Point(x > 0 ? x : 0, y > 0 ? y : 0);
            }
            else
            {
                frm.StartPosition = FormStartPosition.CenterParent;
            }
        }
        #endregion
    }
}