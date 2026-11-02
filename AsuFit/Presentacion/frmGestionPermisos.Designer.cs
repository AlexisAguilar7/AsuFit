namespace AsuFit.Presentacion
{
    partial class frmGestionPermisos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Inicio");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Registro de Asistencia");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Registrar Socio");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Gestión de Socios");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Gestión de Planes");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("GESTIÓN DE SOCIOS Y ACCESOS", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Inventario y Ventas");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Gestión de Productos");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Ingreso de Mercadería");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Proveedores");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("COMERCIAL E INVENTARIO", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10});
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Arqueo de Caja");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Registrar Cobro");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Historial de Transacciones");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Gestión de Gastos");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Reportes y Estadísticas");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("CAJA Y FINANZAS", new System.Windows.Forms.TreeNode[] {
            treeNode12,
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Registrar Usuario");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Gestión de Usuarios");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Gestión de Roles");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Auditoría");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("SEGURIDAD Y CONTROL", new System.Windows.Forms.TreeNode[] {
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode21});
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Ajustes del Sistema");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("CONFIGURACIÓN GENERAL", new System.Windows.Forms.TreeNode[] {
            treeNode23});
            this.label1 = new System.Windows.Forms.Label();
            this.lblTituloRol = new System.Windows.Forms.Label();
            this.lblNombreRol = new System.Windows.Forms.Label();
            this.tvwPermisos = new System.Windows.Forms.TreeView();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "GESTIÓN DE PERMISOS";
            // 
            // lblTituloRol
            // 
            this.lblTituloRol.AutoSize = true;
            this.lblTituloRol.Location = new System.Drawing.Point(12, 41);
            this.lblTituloRol.Name = "lblTituloRol";
            this.lblTituloRol.Size = new System.Drawing.Size(94, 13);
            this.lblTituloRol.TabIndex = 1;
            this.lblTituloRol.Text = "Rol Seleccionado:";
            // 
            // lblNombreRol
            // 
            this.lblNombreRol.AutoSize = true;
            this.lblNombreRol.Location = new System.Drawing.Point(112, 41);
            this.lblNombreRol.Name = "lblNombreRol";
            this.lblNombreRol.Size = new System.Drawing.Size(60, 13);
            this.lblNombreRol.TabIndex = 2;
            this.lblNombreRol.Text = "NombreRol";
            // 
            // tvwPermisos
            // 
            this.tvwPermisos.CheckBoxes = true;
            this.tvwPermisos.Location = new System.Drawing.Point(15, 92);
            this.tvwPermisos.Name = "tvwPermisos";
            treeNode1.Name = "Nodo0";
            treeNode1.Text = "Inicio";
            treeNode2.Name = "frmAsistencia";
            treeNode2.Text = "Registro de Asistencia";
            treeNode3.Name = "frmRegistrarSocio";
            treeNode3.Text = "Registrar Socio";
            treeNode4.Name = "frmGestionSocios";
            treeNode4.Text = "Gestión de Socios";
            treeNode5.Name = "frmGestionPlanes";
            treeNode5.Text = "Gestión de Planes";
            treeNode6.Name = "Nodo0";
            treeNode6.Text = "GESTIÓN DE SOCIOS Y ACCESOS";
            treeNode7.Name = "frmPuntoVenta";
            treeNode7.Text = "Inventario y Ventas";
            treeNode8.Name = "frmGestionProductos";
            treeNode8.Text = "Gestión de Productos";
            treeNode9.Name = "frmIngresoMercaderia";
            treeNode9.Text = "Ingreso de Mercadería";
            treeNode10.Name = "frmProveedores";
            treeNode10.Text = "Proveedores";
            treeNode11.Name = "Nodo5";
            treeNode11.Text = "COMERCIAL E INVENTARIO";
            treeNode12.Name = "frmArqueoCaja";
            treeNode12.Text = "Arqueo de Caja";
            treeNode13.Name = "frmRegistrarCobro";
            treeNode13.Text = "Registrar Cobro";
            treeNode14.Name = "frmHistorialTransacciones";
            treeNode14.Text = "Historial de Transacciones";
            treeNode15.Name = "frmGestionGastos";
            treeNode15.Text = "Gestión de Gastos";
            treeNode16.Name = "frmReportes";
            treeNode16.Text = "Reportes y Estadísticas";
            treeNode17.Name = "Nodo10";
            treeNode17.Text = "CAJA Y FINANZAS";
            treeNode18.Name = "frmRegistrarUsuario";
            treeNode18.Text = "Registrar Usuario";
            treeNode19.Name = "frmGestionUsuarios";
            treeNode19.Text = "Gestión de Usuarios";
            treeNode20.Name = "frmGestionRoles";
            treeNode20.Text = "Gestión de Roles";
            treeNode21.Name = "frmAuditoria";
            treeNode21.Text = "Auditoría";
            treeNode22.Name = "Nodo16";
            treeNode22.Text = "SEGURIDAD Y CONTROL";
            treeNode23.Name = "frmConfiguracion";
            treeNode23.Text = "Ajustes del Sistema";
            treeNode24.Name = "Nodo21";
            treeNode24.Text = "CONFIGURACIÓN GENERAL";
            this.tvwPermisos.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode6,
            treeNode11,
            treeNode17,
            treeNode22,
            treeNode24});
            this.tvwPermisos.Size = new System.Drawing.Size(235, 390);
            this.tvwPermisos.TabIndex = 3;
            this.tvwPermisos.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvwPermisos_AfterCheck);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(160, 488);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(91, 23);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "CANCELAR";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Location = new System.Drawing.Point(15, 488);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(91, 23);
            this.btnGuardar.TabIndex = 4;
            this.btnGuardar.Text = "GUARDAR";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(238, 13);
            this.label2.TabIndex = 53;
            this.label2.Text = "Seleccione los módulos y submódulos permitidos:";
            // 
            // frmGestionPermisos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 523);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.tvwPermisos);
            this.Controls.Add(this.lblNombreRol);
            this.Controls.Add(this.lblTituloRol);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGestionPermisos";
            this.Text = "frmGestionPermisos";
            this.Load += new System.EventHandler(this.frmGestionPermisos_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTituloRol;
        private System.Windows.Forms.Label lblNombreRol;
        private System.Windows.Forms.TreeView tvwPermisos;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label label2;
    }
}