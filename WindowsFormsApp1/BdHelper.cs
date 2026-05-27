using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escritorio
{
    class BdHelper
    {
        private string connectionString = "Data Source=ECOP-ENZOD\\SQLEXPRESS;Initial Catalog=Prueba;user id=sa;password=yagu@123;MultipleActiveResultSets=True;Trusted_Connection=false";

        public BdHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public BdHelper()
        {
        }

        public bool ActualizarCliente(int id, string nombre, string apellido, string tipoDocumento, string documento)
        {
            string spName = "sp_Clientes_Actualizar";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    command.Parameters.Add("@Nombre", SqlDbType.VarChar, 30).Value = (object)nombre ?? DBNull.Value;
                    command.Parameters.Add("@Apellido", SqlDbType.VarChar, 30).Value = (object)apellido ?? DBNull.Value;
                    command.Parameters.Add("@TipoDocumento", SqlDbType.VarChar, 20).Value = (object)tipoDocumento ?? DBNull.Value;
                    command.Parameters.Add("@Documento", SqlDbType.VarChar, 30).Value = (object)documento ?? DBNull.Value;

                    try
                    {
                        connection.Open();

                        int filasAfectadas = command.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Error al actualizar cliente: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public int InsertCliente(string nombre, string apellido, string tipoDocumento, string documento)
        {
            string spName = "sp_Clientes_Insertar";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Nombre", SqlDbType.VarChar, 30).Value = nombre;
                    command.Parameters.Add("@Apellido", SqlDbType.VarChar, 30).Value = apellido;
                    command.Parameters.Add("@TipoDocumento", SqlDbType.VarChar, 20).Value = tipoDocumento;
                    command.Parameters.Add("@Documento", SqlDbType.VarChar, 30).Value = documento;

                    try
                    {
                        connection.Open();

                        int idGenerado = Convert.ToInt32(command.ExecuteScalar());

                        return idGenerado;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al insertar usuario: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public int InsertProducto(string codigo, string descripcion, string unidadMedida, string precio)
        {

            string sp = "sp_Productos_Insertar";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Codigo", SqlDbType.VarChar, 30).Value = codigo;
                    command.Parameters.Add("@Descripcion", SqlDbType.VarChar, 30).Value = descripcion;
                    command.Parameters.Add("@UnidadMedida", SqlDbType.VarChar, 10).Value = unidadMedida;
                    command.Parameters.Add("@Precio", SqlDbType.Int).Value = precio;

                    try
                    {
                        connection.Open();

                        int idGenerado = Convert.ToInt32(command.ExecuteScalar());

                        return idGenerado;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al insertar producto: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public bool updateProducto(int id, string codigo, string descripcion, string unidadMedida, string precio)
        {

            string sp = "sp_Productos_Actualizar";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    command.Parameters.Add("@Codigo", SqlDbType.VarChar, 30).Value = codigo;
                    command.Parameters.Add("@Descripcion", SqlDbType.VarChar, 30).Value = descripcion;
                    command.Parameters.Add("@UnidadMedida", SqlDbType.VarChar, 10).Value = unidadMedida;
                    command.Parameters.Add("@Precio", SqlDbType.Int).Value = precio;

                    try
                    {
                        connection.Open();

                        int filasAfectadas = command.ExecuteNonQuery();

                        return filasAfectadas > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al insertar producto: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public bool EliminarProductoSP(int id)
        {
            string spName = "sp_Productos_Eliminar";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    try
                    {
                        connection.Open();

                        int filasAfectadas = command.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al eliminar producto: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public bool EliminarClienteSP(int id)
        {
            string spName = "sp_Clientes_Eliminar";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@ID", SqlDbType.Int).Value = id;

                    try
                    {
                        connection.Open();

                        int filasAfectadas = command.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al eliminar cliente: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public DataSet GetProductos()
        {

            string query = "SELECT ID, Codigo, Descripcion, Precio, UnidadMedida FROM Productos";

            DataSet dsProductos = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    try
                    {
                        adapter.Fill(dsProductos, "Productos");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al obtener productos: " + ex.Message);
                        throw;
                    }
                }
            }

            return dsProductos;
        }

        public DataSet GetClientes()
        {

            string query = "SELECT ID, Nombre, Apellido, TipoDocumento, Documento FROM Clientes";

            DataSet dsClientes = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    try
                    {
                        adapter.Fill(dsClientes, "Clientes");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al obtener Clientes: " + ex.Message);
                        throw;
                    }
                }
            }

            return dsClientes;
        }

        public DataSet GetClientePorDocumento(string documento)
        {

            string query = "SELECT ID, Nombre, Apellido, TipoDocumento, Documento FROM Clientes WHERE Documento = @Documento";

            DataSet dsClientes = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.SelectCommand.Parameters.Add("@Documento", SqlDbType.VarChar, 30).Value = documento.Trim();
                    try
                    {
                        adapter.Fill(dsClientes, "Clientes");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al obtener Cliente: " + ex.Message);
                        throw;
                    }
                }
            }

            return dsClientes;
        }

        public DataSet GetProductoPorCodigo(string codigo)
        {

            string query = "SELECT ID, Codigo, Descripcion, Precio, UnidadMedida FROM Productos where codigo = @Codigo";

            DataSet dsProductos = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.SelectCommand.Parameters.Add("@Codigo", SqlDbType.VarChar, 10).Value = codigo.Trim();
                    try
                    {
                        adapter.Fill(dsProductos, "Productos");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al obtener productos: " + ex.Message);
                        throw;
                    }
                }
            }

            return dsProductos;
        }

        public int InsertPedido(string codigo, string descripcion, string unidadMedida, string precio)
        {

            string sp = "sp_Clientes_Insertar";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Codigo", SqlDbType.VarChar, 30).Value = codigo;
                    command.Parameters.Add("@Descripcion", SqlDbType.VarChar, 30).Value = descripcion;
                    command.Parameters.Add("@UnidadMedida", SqlDbType.VarChar, 10).Value = unidadMedida;
                    command.Parameters.Add("@Precio", SqlDbType.Int).Value = precio;

                    try
                    {
                        connection.Open();

                        int idGenerado = Convert.ToInt32(command.ExecuteScalar());

                        return idGenerado;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al insertar producto: " + ex.Message);
                        throw;
                    }
                }
            }
        }
    }
}
