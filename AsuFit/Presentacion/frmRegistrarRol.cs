using AsuFit.Entidades;
using AsuFit.Negocio;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AsuFit.Presentacion
{
    public partial class frmRegistrarRol : Form
    {
        private Usuario usuarioActual;
        private Rol rolEdicion = null;
        private bool esEdicion = false;

        // Constructor para NUEVO ROL
        public frmRegistrarRol(Usuario userLogueado)
        {
            InitializeComponent();
            usuarioActual = userLogueado;
            this.Load += new EventHandler(frmRegistrarRol_Load);
        }

        // Constructor para EDITAR ROL
        public frmRegistrarRol(Rol rol, Usuario userLogueado)
        {
            InitializeComponent();
            usuarioActual = userLogueado;
            rolEdicion = rol;
            esEdicion = true;
            this.Load += new EventHandler(frmRegistrarRol_Load);
        }

        private void frmRegistrarRol_Load(object sender, EventArgs e)
        {
            ConfigurarTemaOscuro();

            // Si es edición, cargar los datos en los TextBox
            if (esEdicion && rolEdicion != null)
            {
                txtNombre.Text = rolEdicion.NombreRol;
                txtDescripcion.Text = rolEdicion.Descripcion;
            }
        }

        #region EVENTOS DE BOTONES
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MensajeAsuFit.Mostrar("El campo Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
            if (string.IsNullOrEmpty(descripcion))
            {
                MensajeAsuFit.Mostrar("El campo Descripción es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
                return;
            }

            Rol nuevoObj = new Rol
            {
                IdRol = esEdicion ? rolEdicion.IdRol : 0,
                NombreRol = txtNombre.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim()
            };

            RolNegocio negocio = new RolNegocio();
            string mensaje;

            int idResultado = negocio.GuardarRol(nuevoObj, esEdicion, out mensaje);

            if (idResultado > 0)
            {
                if (esEdicion)
                {
                    AsuFit.Datos.GestorAuditoria.Registrar(usuarioActual.NombreCompleto, "Roles", "Edición", $"Se editó el rol: {nuevoObj.NombreRol}");
                    MensajeAsuFit.Mostrar("Rol actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; 
                    this.Close();
                }
                else
                {
                    AsuFit.Datos.GestorAuditoria.Registrar(usuarioActual.NombreCompleto, "Roles", "Registro", $"Se registró un nuevo rol: {nuevoObj.NombreRol}");
                    MensajeAsuFit.Mostrar("Rol registrado correctamente. A continuación, configure sus permisos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Asignamos el ID al objeto (propiedad Tag) para que el padre lo lea y lo abra
                    this.Tag = idResultado;
                    this.DialogResult = DialogResult.Yes; // Yes = "Se guardó y quiero abrir permisos"
                    this.Close();
                }
            }
            else
            {
                MensajeAsuFit.Mostrar(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region ESTILOS VISUALES (UI)
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
                        if (subC is TextBox txt)
                        {
                            txt.BackColor = Color.FromArgb(35, 39, 47);
                            txt.ForeColor = Color.White;
                            txt.BorderStyle = BorderStyle.FixedSingle;
                        }
                    }
                }
                else if (c is Label lbl) lbl.ForeColor = Color.White;
                else if (c is TextBox txt)
                {
                    txt.BackColor = Color.FromArgb(35, 39, 47);
                    txt.ForeColor = Color.White;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is Button btn)
                {
                    btn.BackColor = Color.FromArgb(35, 39, 47);
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Cursor = Cursors.Hand;
                }
            }

            Control[] btnGuardar = this.Controls.Find("btnGuardar", true);
            if (btnGuardar.Length > 0 && btnGuardar[0] is Button btnPrimario)
            {
                btnPrimario.BackColor = Color.FromArgb(0, 229, 255);
                btnPrimario.ForeColor = Color.Black;
            }
        }
        #endregion
    }
}