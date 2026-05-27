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
    public partial class ListadoPedidos : Form
    {
        private BdHelper bdHelper = new BdHelper();
        private DataTable dtTodosLosPedidos;
        public ListadoPedidos()
        {
            InitializeComponent();
        }

        private void CargarGridPedidos()
        {
            try
            {
                DataSet ds = bdHelper.ListarPedidos();

                if (ds != null && ds.Tables.Contains("Pedidos"))
                {
                    dtTodosLosPedidos = ds.Tables["Pedidos"];
                    dgvPedidos.DataSource = dtTodosLosPedidos;

                    dgvPedidos.AllowUserToAddRows = false;
                    dgvPedidos.ReadOnly = true;
                    dgvPedidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    if (dgvPedidos.Columns.Contains("Total"))
                    {
                        dgvPedidos.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvPedidos.Columns["Total"].DefaultCellStyle.Format = "N0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el listado de pedidos: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListadoPedidos_Load(object sender, EventArgs e)
        {
            CargarGridPedidos();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Pedidos ped = new Pedidos();
            ped.ShowDialog();
            CargarGridPedidos();
        }

        private void txtBuscador_TextChanged(object sender, EventArgs e)
        {
            if (dtTodosLosPedidos == null) return;

            if (string.IsNullOrEmpty(txtBuscador.Text.Trim()))
            {
                dtTodosLosPedidos.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                dtTodosLosPedidos.DefaultView.RowFilter = string.Format(
                    "Convert(Pedido, 'System.String') LIKE '%{0}%' OR Cliente LIKE '%{0}%'",
                    txtBuscador.Text.Trim()
                );
            }
        }

        private void dgvPedidos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvPedidos.Rows[e.RowIndex];

                int idPedido = Convert.ToInt32(fila.Cells["Pedido"].Value);

                Pedidos frmPedidos = new Pedidos(idPedido);

                frmPedidos.ShowDialog();
            }
        }
    }
}
