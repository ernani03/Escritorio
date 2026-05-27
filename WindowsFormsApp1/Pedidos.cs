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
        private int total = 0;
        private int? _pedidoIdParaCargar = null;
        public Pedidos()
        {
            InitializeComponent();
        }

        public Pedidos(int pedidoId) : this()
        {
            _pedidoIdParaCargar = pedidoId;
        }

        private void Pedidos_Load(object sender, EventArgs e)
        {
            SeteoInicial();
            SetDtProducto();
            ConfigurarYMostrarGrid(dtProductos);
            lblPedido.Visible = false;
            if (_pedidoIdParaCargar.HasValue)
            {
                CargarDatosDelPedidoExistente(_pedidoIdParaCargar.Value);
            }
        }

        private void CargarDatosDelPedidoExistente(int idPedido)
        {
            try
            {
                lblPedido.Visible = true;
                lblPedido.Tag = idPedido;
                lblPedido.Text = "Pedido N°: " + idPedido.ToString();

                var auxPedido = bdHelper.GetPedidoPorId(idPedido);
                var auxCliente = bdHelper.GetClientePorId(auxPedido.Rows[0].Field<int>("ID"));
                txtCliente.Text = auxCliente.Rows[0].Field<string>("Documento");
                txtCliente.Tag = auxCliente.Rows[0].Field<int>("ID");
                lblCliente.Text = auxCliente.Rows[0].Field<string>("Nombre");

                var dtDetalles = bdHelper.GetPedidoDetallePorId(auxPedido.Rows[0].Field<int>("ID"));

                int total = dtDetalles.AsEnumerable().Sum(row => row.Field<int>("Total"));
                txtTotal.Text = total.ToString();

                dtProductos = dtDetalles.Copy();
                ConfigurarYMostrarGrid(dtProductos);
                btnGuardar.Enabled = false;
                btnCargarProducto.Enabled = false;
                button1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al cargar el pedido: " + ex.Message, "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetDtProducto()
        {
            dtProductos.Columns.Add("IDProducto", typeof(int));
            dtProductos.Columns.Add("Codigo", typeof(string));
            dtProductos.Columns.Add("Descripcion", typeof(string));
            dtProductos.Columns.Add("Cantidad", typeof(int));
            dtProductos.Columns.Add("Precio", typeof(int));
            dtProductos.Columns.Add("Total", typeof(int));
        }

        private void ConfigurarYMostrarGrid(DataTable dt)
        {
            dgvDetalle.DataSource = dt;

            if (dgvDetalle.Columns.Count > 0)
            {
                dgvDetalle.Columns["Codigo"].HeaderText = "Código";
                dgvDetalle.Columns["Descripcion"].HeaderText = "Descripción";
                dgvDetalle.Columns["Cantidad"].HeaderText = "Cant.";
                dgvDetalle.Columns["Precio"].HeaderText = "Precio";
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

        private void AgregarProductoALista(int id, string codigo, string descripcion, int cantidad, int precioUnitario)
        {
            int totalFila = cantidad * precioUnitario;

            DataRow nuevaFila = dtProductos.NewRow();

            nuevaFila["IDProducto"] = id;
            nuevaFila["Codigo"] = codigo;
            nuevaFila["Descripcion"] = descripcion;
            nuevaFila["Cantidad"] = cantidad;
            nuevaFila["Precio"] = precioUnitario;
            nuevaFila["Total"] = totalFila;

            total = total + totalFila;
            txtTotal.Text = total.ToString();

            dtProductos.Rows.Add(nuevaFila);

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
            ValidarCliente();
        }

        public bool ValidarCliente()
        {
            if (!string.IsNullOrEmpty(txtCliente.Text))
            {
                var ds = bdHelper.GetClientePorDocumento(txtCliente.Text);
                if (ds != null && ds.Tables.Contains("Clientes") && ds.Tables["Clientes"].Rows.Count > 0)
                {
                    txtCliente.Text = ds.Tables["Clientes"].Rows[0].Field<string>("Documento");
                    txtCliente.Tag = ds.Tables["Clientes"].Rows[0].Field<int>("ID");
                    lblCliente.Text = ds.Tables["Clientes"].Rows[0].Field<string>("Nombre");
                    return true;
                }
                else
                {
                    MessageBox.Show("Favor cargar Cliente valido", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarClientes();
                }
            }
            return false;
        }

        private void textBox3_Validated(object sender, EventArgs e)
        {
            ValidarProducto();
        }

        private bool ValidarProducto()
        {
            if (!string.IsNullOrEmpty(txtProducto.Text))
            {
                var ds = bdHelper.GetProductoPorCodigo(txtProducto.Text);
                if (ds != null && ds.Tables.Contains("Productos") && ds.Tables["Productos"].Rows.Count > 0)
                {
                    int precio = ds.Tables["Productos"].Rows[0].Field<int>("Precio");
                    txtProducto.Text = ds.Tables["Productos"].Rows[0].Field<string>("Codigo");
                    txtProducto.Tag = ds.Tables["Productos"].Rows[0].Field<int>("ID");
                    lblProducto.Text = ds.Tables["Productos"].Rows[0].Field<string>("Descripcion");
                    lblPrecio.Text = "Precio: " + precio.ToString();
                    return true;
                }
                else
                {
                    MessageBox.Show("Favor cargar Producto valido", "Error de Sistema",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                    LimpiarProductos();
                }
            }

            return false;
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

                AgregarProductoALista(Convert.ToInt32(txtProducto.Tag), txtProducto.Text, lblProducto.Text, Convert.ToInt32(txtCantidad.Text.Trim()), Convert.ToInt32(lblPrecio.Text.Trim().Replace("Precio: ", "")));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al agregar el detalle. " + ex.Message, "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void EliminarDetalle()
        {
            if (dgvDetalle.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione el detalle que desea eliminar.",
                                "Aviso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show("¿Desea quitar este producto de la lista?",
                                             "Confirmar",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                int index = dgvDetalle.CurrentRow.Index;

                //DataTable dt = (DataTable)dgvDetalle.DataSource;
                var restar = dtProductos.Rows[index].Field<int>("Total");
                total = total - restar;
                txtTotal.Text = total.ToString();

                dtProductos.Rows[index].Delete();

                dtProductos.AcceptChanges();

                MessageBox.Show("Producto quitado de la lista.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lblPrecio_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            EliminarDetalle();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarClientes();
            LimpiarProductos();
            btnGuardar.Enabled = true;
            btnCargarProducto.Enabled = true;
            button1.Enabled = true;
            dtProductos.Rows.Clear();
            dgvDetalle.Refresh();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarPedido();
        }

        private void GuardarPedido()
        {
            try
            {
                if (!ValidarCliente())
                {
                    if(string.IsNullOrEmpty( txtCliente.Text))
                        MessageBox.Show("Favor cargar Cliente valido", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
                if(!(dtProductos.Rows.Count > 0))
                {
                    MessageBox.Show("Favor agregar detalle al pedido", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var aux = bdHelper.InsertarPedidoCab((int)txtCliente.Tag, total);
                bdHelper.InsertarPedidoDetalle(aux, dtProductos);

                MessageBox.Show("Se ha insertado el pedido correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el Cliente: " + ex.Message, "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
