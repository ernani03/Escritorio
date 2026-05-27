using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Escritorio
{
    public partial class Pedidos : Form
    {
        private DataTable dtProductos = new DataTable("DetalleProductos");
        private BdHelper bdHelper = new BdHelper();
        public Pedidos()
        {
            InitializeComponent();
        }

        private void Pedidos_Load(object sender, EventArgs e)
        {
            SeteoInicial();
            SetDtProducto();
            ConfigurarYMostrarGrid(dtProductos);
        }

        public void SetDtProducto()
        {
            dtProductos.Columns.Add("IDProducto", typeof(int));
            dtProductos.Columns.Add("Codigo", typeof(string));
            dtProductos.Columns.Add("Descripcion", typeof(string));
            dtProductos.Columns.Add("Cantidad", typeof(int));
            dtProductos.Columns.Add("Total", typeof(decimal));
        }

        private void ConfigurarYMostrarGrid(DataTable dt)
        {
            dgvDetalle.DataSource = dt;

            if (dgvDetalle.Columns.Count > 0)
            {
                dgvDetalle.Columns["Codigo"].HeaderText = "Código";
                dgvDetalle.Columns["Descripcion"].HeaderText = "Descripción";
                dgvDetalle.Columns["Cantidad"].HeaderText = "Cant.";
                dgvDetalle.Columns["Total"].HeaderText = "Total General";

                if (dgvDetalle.Columns.Contains("IDProducto"))
                {
                    dgvDetalle.Columns["IDProducto"].Visible = false;
                }

                dgvDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvDetalle.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvDetalle.Columns["Total"].DefaultCellStyle.Format = "N0";
            }
        }

        private void AgregarProductoALista(int id, string codigo, string descripcion, int cantidad, decimal precioUnitario)
        {
            decimal totalFila = cantidad * precioUnitario;

            DataRow nuevaFila = dtProductos.NewRow();

            nuevaFila["IDProducto"] = id;
            nuevaFila["Codigo"] = codigo;
            nuevaFila["Descripcion"] = descripcion;
            nuevaFila["Cantidad"] = cantidad;
            nuevaFila["Total"] = totalFila;

            dtProductos.Rows.Add(nuevaFila);

            LimpiarClientes();
            LimpiarProductos();
        }

        public bool EsCliente(string documento)
        {
            return true;
        }

        public void SeteoInicial()
        {
            lblCantidad.Text = string.Empty;
            txtCantidad.Text = "1";
            lblCliente.Text = string.Empty;
            lblProducto.Text = string.Empty;
            lblPrecio.Text = string.Empty;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtCliente.Text))
            {
                var ds = bdHelper.GetClientePorDocumento(txtCliente.Text);
                if (ds != null && ds.Tables.Contains("Clientes") && ds.Tables["Clientes"].Rows.Count > 0)
                {
                    txtCliente.Text = ds.Tables["Clientes"].Rows[0].Field<string>("Documento");
                    txtCliente.Tag = ds.Tables["Clientes"].Rows[0].Field<int>("ID");
                    lblCliente.Text = ds.Tables["Clientes"].Rows[0].Field<string>("Nombre");
                }
                else
                {
                    LimpiarClientes();
                }
            }
        }

        private void textBox3_Validated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtProducto.Text))
            {
                var ds = bdHelper.GetProductoPorCodigo(txtProducto.Text);
                if (ds != null && ds.Tables.Contains("Productos") && ds.Tables["Productos"].Rows.Count > 0)
                {
                    txtProducto.Text = ds.Tables["Productos"].Rows[0].Field<string>("Codigo");
                    txtProducto.Tag = ds.Tables["Productos"].Rows[0].Field<int>("ID");
                    lblProducto.Text = ds.Tables["Productos"].Rows[0].Field<string>("Descripcion");
                    lblPrecio.Text = ds.Tables["Productos"].Rows[0].Field<int>("Precio").ToString();
                }
                else
                {
                    MessageBox.Show("Favor cargar cliente valido",
                                "Error de Sistema",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    LimpiarProductos();
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LimpiarProductos()
        {
            txtProducto.Text = string.Empty;
            txtProducto.Tag = string.Empty;
            lblProducto.Text = string.Empty;
            lblPrecio.Text = string.Empty;
            txtCantidad.Text = "1";
        }
        private void LimpiarClientes()
        {
            txtCliente.Text = string.Empty;
            txtCliente.Tag = string.Empty;
            lblCliente.Text = string.Empty;
        }

        private void btnCargarProducto_Click(object sender, EventArgs e)
        {
            AgregarDetalle();
        }

        public void AgregarDetalle()
        {
            try
            {
                if (string.IsNullOrEmpty(txtCliente.Text))
                {
                    MessageBox.Show("Favor cargar cliente valido", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarClientes();
                    return;
                }
                if (string.IsNullOrEmpty(txtProducto.Text))
                {
                    MessageBox.Show("Favor cargar cliente valido", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarProductos();
                    return;
                }

                AgregarProductoALista(Convert.ToInt32(txtProducto.Tag), txtProducto.Text, lblProducto.Text, Convert.ToInt32(txtCantidad.Text), Convert.ToDecimal(lblPrecio.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al agregar el detalle. " + ex.Message, "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblPrecio_Click(object sender, EventArgs e)
        {

        }
    }
}
