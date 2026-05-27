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
    public partial class Clientes : Form
    {
        private BdHelper bdHelper = new BdHelper();
        public Clientes()
        {
            InitializeComponent();
            CargarGridClientes();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Productos_Load(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        
        private bool ValidarTipoDoumento()
        {
            if (string.IsNullOrEmpty(CbTipoDocumento.SelectedItem.ToString()))
            {
                MessageBox.Show("Favor asigne un tipo de documento para el cliente.",
                   "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        private bool ValidarDocumento()
        {
            if (string.IsNullOrEmpty(txtDocumento.Text))
            {
                MessageBox.Show("Favor asigne un numero de documento.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidarNombre()
        {
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                MessageBox.Show("Favor asigne un nombre para el cliente.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidarApellido()
        {
            if (string.IsNullOrEmpty(txtApellido.Text))
            {
                MessageBox.Show("Favor asigne un apelido para el cliente.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if(ValidarDocumento() && ValidarNombre() && ValidarTipoDoumento() && ValidarApellido())
            {
                try
                {
                    if(txtDocumento.Tag is null || string.IsNullOrEmpty(txtDocumento.Tag.ToString()))
                    {
                        var aux = bdHelper.InsertCliente(txtNombre.Text, txtApellido.Text, CbTipoDocumento.SelectedItem.ToString(), txtDocumento.Text);
                        if(aux > 0)
                        {
                            Limpiar();
                            CargarGridClientes();
                        }
                    }
                    else
                    {
                        var aux = bdHelper.ActualizarCliente(Convert.ToInt32(txtDocumento.Tag), txtNombre.Text, txtApellido.Text, CbTipoDocumento.SelectedItem.ToString(), txtDocumento.Text);
                        if (!aux)
                        {
                            MessageBox.Show("Ocurrio un error al guardar el Cliente: ",
                           "Error",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                        }
                        else
                        {
                            Limpiar();
                            CargarGridClientes();
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ocurrio un error al guardar el Cliente: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }

        }

        private void Limpiar()
        {
            txtDocumento.Clear();
            txtDocumento.Tag = string.Empty;
            CbTipoDocumento.SelectedIndex = 0;
            txtNombre.Clear();
            txtApellido.Clear();
        }

        private void CargarGridClientes()
        {
            try
            {
                DataSet ds = bdHelper.GetClientes();

                if (ds != null && ds.Tables.Contains("Clientes") && ds.Tables["Clientes"].Rows.Count > 0)
                {
                    DgvClientes.DataSource = ds.Tables["Clientes"];
                }
                else
                {
                    DgvClientes.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de Clientes: " + ex.Message,
                                "Error de Sistema",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void DgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = DgvClientes.Rows[e.RowIndex];

                txtDocumento.Tag = fila.Cells["ID"].Value?.ToString();
                txtDocumento.Text = fila.Cells["Documento"].Value?.ToString();
                CbTipoDocumento.SelectedValue = fila.Cells["TipoDocumento"].Value?.ToString();
                txtApellido.Text = fila.Cells["Apellido"].Value?.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
            }
        }

        private void Eliminar()
        {
            if (txtDocumento.Tag is null || string.IsNullOrEmpty(txtDocumento.Tag.ToString()))
            {
                MessageBox.Show("Por favor, seleccione un Cliente de la lista para eliminar.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este Cliente? Esta acción no se puede deshacer.",
                                                     "Confirmar Eliminación",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtDocumento.Tag);

                    bool exito = bdHelper.EliminarClienteSP(id);

                    if (exito)
                    {
                        MessageBox.Show("Cliente eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Limpiar();
                        CargarGridClientes();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el cliente o ya fue eliminado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al intentar eliminar el registro: " + ex.Message,
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Eliminar();
        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
