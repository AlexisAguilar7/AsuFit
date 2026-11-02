using AsuFit.Entidades;
using AsuFit.Negocio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AsuFit.Presentacion
{
    public partial class frmGestionPermisos : Form
    {
        private Usuario usuarioActual;
        private Rol rolActivo;

        public frmGestionPermisos(Rol rolSeleccionado, Usuario userLogueado)
        {
            InitializeComponent();
            usuarioActual = userLogueado;
            rolActivo = rolSeleccionado;
            this.Load += new EventHandler(frmGestionPermisos_Load);
        }

        private void frmGestionPermisos_Load(object sender, EventArgs e)
        {
            ConfigurarTemaOscuro();

            // Inyectar el nombre del rol en la etiqueta
            if (this.Controls.Find("lblNombreRol", true).Length > 0 && this.Controls.Find("lblNombreRol", true)[0] is Label lbl)
            {
                lbl.Text = rolActivo.NombreRol;
            }

            CargarPermisosActivos();

            tvwPermisos.ExpandAll();
            tvwPermisos.SelectedNode = null;

            // Forzamos al sistema a poner la atención visual en el botón de Cancelar
            Control[] btn = this.Controls.Find("btnCancelar", true);
            if (btn.Length > 0) btn[0].Select();
        }

        #region EVENTOS DEL ÁRBOL Y BOTONES
        private void CargarPermisosActivos()
        {
            RolNegocio negocio = new RolNegocio();
            List<string> permisos = negocio.ObtenerPermisosPorRol(rolActivo.IdRol);

            if (this.Controls.Find("tvwPermisos", true).Length > 0 && this.Controls.Find("tvwPermisos", true)[0] is TreeView tvw)
            {
                // Remover el evento temporalmente para que al marcar los padres por código no se dispare el chequeo en cascada
                tvw.AfterCheck -= tvwPermisos_AfterCheck;
                MarcarNodos(tvw.Nodes, permisos);
                tvw.AfterCheck += tvwPermisos_AfterCheck;
            }
        }

        private void MarcarNodos(TreeNodeCollection nodos, List<string> permisos)
        {
            foreach (TreeNode nodo in nodos)
            {
                if (permisos.Contains(nodo.Text))
                {
                    nodo.Checked = true;
                }
                if (nodo.Nodes.Count > 0)
                {
                    MarcarNodos(nodo.Nodes, permisos);
                }
            }
        }

        private void tvwPermisos_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode hijo in e.Node.Nodes)
                    {
                        hijo.Checked = e.Node.Checked;
                    }
                }
                else if (e.Node.Parent != null && e.Node.Checked)
                {
                    e.Node.Parent.Checked = true;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            List<string> modulosSeleccionados = new List<string>();

            if (this.Controls.Find("tvwPermisos", true).Length > 0 && this.Controls.Find("tvwPermisos", true)[0] is TreeView tvw)
            {
                // Desconectar evento temporalmente
                tvw.AfterCheck -= tvwPermisos_AfterCheck;

                // Recorrer el árbol completo buscando las hojas marcadas
                ExtraerNodosMarcados(tvw.Nodes, modulosSeleccionados);

                tvw.AfterCheck += tvwPermisos_AfterCheck;
            }

            RolNegocio negocio = new RolNegocio();
            string mensaje;

            if (negocio.ActualizarPermisos(rolActivo.IdRol, modulosSeleccionados, out mensaje))
            {
                AsuFit.Datos.GestorAuditoria.Registrar(usuarioActual.NombreCompleto, "Permisos", "Actualización", $"Se actualizaron los permisos del rol '{rolActivo.NombreRol}'.");
                MensajeAsuFit.Mostrar("Permisos actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // SOLUCIÓN: Avisarle al formulario padre que el proceso terminó con éxito
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MensajeAsuFit.Mostrar(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtraerNodosMarcados(TreeNodeCollection nodos, List<string> lista)
        {
            foreach (TreeNode nodo in nodos)
            {
                if (nodo.Checked && nodo.Nodes.Count == 0) // Solo módulos finales
                {
                    lista.Add(nodo.Text);
                }

                if (nodo.Nodes.Count > 0) // Si tiene hijos, entrar a revisarlos
                {
                    ExtraerNodosMarcados(nodo.Nodes, lista);
                }
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
                    }
                }
                else if (c is Label lbl) lbl.ForeColor = Color.White;
                else if (c is Button btn)
                {
                    btn.BackColor = Color.FromArgb(35, 39, 47);
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Cursor = Cursors.Hand;
                }
                else if (c is TreeView tvw)
                {
                    tvw.BackColor = Color.FromArgb(35, 39, 47);
                    tvw.ForeColor = Color.White;
                    tvw.LineColor = Color.White;
                    tvw.BorderStyle = BorderStyle.None;
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