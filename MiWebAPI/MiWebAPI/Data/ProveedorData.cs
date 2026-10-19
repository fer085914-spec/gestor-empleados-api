using MiWebAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace MiWebAPI.Data
{
    public class ProveedorData
    {
        private readonly string conexion;

        public ProveedorData(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("CadenaSQL")!;
        }

        public async Task<List<Proveedor>> Lista()
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_SELECCIONA", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Proveedor
                        {
                            Id = Convert.ToInt32(reader["ID"]), // Usar mayúsculas/minúsculas de la BD
                            Nombre = reader["NOMBRE"].ToString(),
                            ApellidoPaterno = reader["APELLIDO_PATERNO"].ToString(),       // <-- Corregido con _
                            ApellidoMaterno = reader["APELLIDO_MATERNO"].ToString(),       // <-- Corregido con _
                            RFC = reader["RFC"].ToString(),
                            Correo = reader["CORREO"].ToString(),
                            Credito = Convert.ToDecimal(reader["CREDITO"]),
                            NombreBanco = reader["NOMBRE_BANCO"].ToString(),              // <-- Corregido con _
                            FechaUltimaModificacion = reader["FECHA_ULTIMA_MODIFICACION"].ToString() // Añadido por si lo necesitas
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<bool> Crear(Proveedor objeto)
        {
            bool respuesta = true;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_AGREGA", con);
                cmd.Parameters.AddWithValue("@nombre", objeto.Nombre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@apellidoPaterno", objeto.ApellidoPaterno ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@apellidoMaterno", objeto.ApellidoMaterno ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@rfc", objeto.RFC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@correo", objeto.Correo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@credito", objeto.Credito);
                cmd.Parameters.AddWithValue("@nombreBanco", objeto.NombreBanco ?? (object)DBNull.Value);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await con.OpenAsync();
                    respuesta = await cmd.ExecuteNonQueryAsync() > 0;
                }
                catch (Exception ex)
                {
                    // Tip extra: Puedes imprimir o loguear 'ex.Message' aquí para depurar si falla SQL
                    respuesta = false;
                }
            }
            return respuesta;
        }
    }
}