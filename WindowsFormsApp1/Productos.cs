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
    public partial class Productos : Form
    {
        private BdHelper bdHelper = new BdHelper();
        public Productos()
        {
            InitializeComponent();
            CargarGridProductos();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Productos_Load(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        
        private bool ValidarPrecio()
        {
            if (string.IsNullOrEmpty(txtPrecio.Text))
            {
                MessageBox.Show("Favor asigne un precio para el producto." ,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            int precioResultado;
            if (!int.TryParse(txtPrecio.Text.Trim(), out precioResultado))
            {
                MessageBox.Show("El precio debe ser un número entero válido (sin puntos, comas ni letras).",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                txtPrecio.Focus();
                return false;
            }

            if (precioResultado <= 0)
            {
                MessageBox.Show("El precio debe ser mayor a cero.",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                txtPrecio.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarCodigo()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                MessageBox.Show("Favor asigne un codigo para el producto.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidarDescripcion()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                MessageBox.Show("Favor asigne un codigo para el producto.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidarUnidadMedida()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                MessageBox.Show("Favor asigne un codigo para el producto.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if(ValidarCodigo() && ValidarDescripcion() && ValidarPrecio() && ValidarUnidadMedida())
            {
                try
                {
                    if(txtCodigo.Tag is null || string.IsNullOrEmpty(txtCodigo.Tag.ToString()))
                    {
                        var aux = bdHelper.InsertProducto(txtCodigo.Text, txtDescripcion.Text, txtUnidadMedida.Text, txtPrecio.Text);
                        if(aux > 0)
                        {
                            Limpiar();
                            CargarGridProductos();
                        }
                    }
                    else
                    {
                        var aux = bdHelper.updateProducto(Convert.ToInt32(txtCodigo.Tag), txtCodigo.Text, txtDescripcion.Text, txtUnidadMedida.Text, txtPrecio.Text);
                        if(!aux)
                        {
                            MessageBox.Show("Ocurrio un error al guardar el producto: ",
                           "Error",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                        }
                        else
                        {
                            Limpiar();
                            CargarGridProductos();
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ocurrio un error al guardar el producto: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }

        }

        private void Limpiar()
        {
            txtCodigo.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtUnidadMedida.Clear();
        }

        private void CargarGridProductos()
        {
            try
            {
                DataSet ds = bdHelper.GetProductos();

                if (ds != null && ds.Tables.Contains("Productos") && ds.Tables["Productos"].Rows.Count > 0)
                {
                    DgvProductos.DataSource = ds.Tables["Productos"];
                }
                else
                {
                    DgvProductos.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de productos: " + ex.Message,
                                "Error de Sistema",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = DgvProductos.Rows[e.RowIndex];

                txtCodigo.Tag = fila.Cells["ID"].Value?.ToString();
                txtCodigo.Text = fila.Cells["Codigo"].Value?.ToString();
                txtDescripcion.Text = fila.Cells["Descripcion"].Value?.ToString();
                txtUnidadMedida.Text = fila.Cells["UnidadMedida"].Value?.ToString();
                txtPrecio.Text = fila.Cells["Precio"].Value?.ToString();
            }
        }

        private void Eliminar()
        {
            if (txtCodigo.Tag is null || string.IsNullOrEmpty(txtCodigo.Tag.ToString()))
            {
                MessageBox.Show("Por favor, seleccione un producto de la lista para eliminar.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este producto? Esta acción no se puede deshacer.",
                                                     "Confirmar Eliminación",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(txtCodigo.Tag);

                    bool exito = bdHelper.EliminarProductoSP(id);

                    if (exito)
                    {
                        MessageBox.Show("Producto eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Limpiar();
                        CargarGridProductos();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el producto o ya fue eliminado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
