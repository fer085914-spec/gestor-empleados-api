using MiWebAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace MiWebAPI.Data
{
    public class LibroData
    {
        private readonly string conexion;

        public LibroData(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("CadenaSQL")!;
        }

        // Listar todos los libros activos
        public async Task<List<Libro>> Lista()
        {
            List<Libro> lista = new List<Libro>();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.LIBRO_CONSULTARTODOS", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Libro
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            ISBN = reader["ISBN"].ToString() ?? "",
                            Titulo = reader["TITULO"].ToString() ?? "",
                            Autor = reader["AUTOR"].ToString() ?? "",
                            Editorial = reader["EDITORIAL"].ToString() ?? "",
                            AnioPublicacion = Convert.ToInt32(reader["ANIOPUBLICACION"]),
                            Activo = Convert.ToBoolean(reader["ACTIVO"]),
                            FechaRegistro = Convert.ToDateTime(reader["FECHA_REGISTRO"]),
                            FechaActualizacion = reader["FECHA_ACTUALIZACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHA_ACTUALIZACION"]) : (DateTime?)null
                        });
                    }
                }
            }
            return lista;
        }

        // Obtener un libro por ID
        public async Task<Libro?> Obtener(int id)
        {
            Libro? objeto = null;

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.LIBRO_CONSULTARPORID", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        objeto = new Libro
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            ISBN = reader["ISBN"].ToString() ?? "",
                            Titulo = reader["TITULO"].ToString() ?? "",
                            Autor = reader["AUTOR"].ToString() ?? "",
                            Editorial = reader["EDITORIAL"].ToString() ?? "",
                            AnioPublicacion = Convert.ToInt32(reader["ANIOPUBLICACION"]),
                            Activo = Convert.ToBoolean(reader["ACTIVO"]),
                            FechaRegistro = Convert.ToDateTime(reader["FECHA_REGISTRO"]),
                            FechaActualizacion = reader["FECHA_ACTUALIZACION"] != DBNull.Value ? Convert.ToDateTime(reader["FECHA_ACTUALIZACION"]) : (DateTime?)null
                        };
                    }
                }
            }
            return objeto;
        }

        // Crear / Registrar un libro
        public async Task<bool> Crear(Libro objeto)
        {
            bool respuesta = false;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.LIBRO_INSERTAR", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@isbn", objeto.ISBN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@titulo", objeto.Titulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@autor", objeto.Autor ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@editorial", objeto.Editorial ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@anio_publicacion", objeto.AnioPublicacion);

                try
                {
                    await con.OpenAsync();
                    int filas = await cmd.ExecuteNonQueryAsync();
                    // Con esto evitamos fallos si SQL Server tiene SET NOCOUNT ON
                    respuesta = filas != 0;
                }
                catch
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }

        // Editar / Actualizar un libro
        public async Task<bool> Editar(Libro objeto)
        {
            bool respuesta = false;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.LIBRO_ACTUALIZAR", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", objeto.Id);
                cmd.Parameters.AddWithValue("@isbn", objeto.ISBN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@titulo", objeto.Titulo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@autor", objeto.Autor ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@editorial", objeto.Editorial ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@anio_publicacion", objeto.AnioPublicacion);

                try
                {
                    await con.OpenAsync();
                    int filas = await cmd.ExecuteNonQueryAsync();
                    respuesta = filas != 0;
                }
                catch
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }

        // Eliminar (Baja lógica: cambia Activo a 0)
        public async Task<bool> Eliminar(int id)
        {
            bool respuesta = false;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.LIBRO_ELIMINAR", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    await con.OpenAsync();
                    int filas = await cmd.ExecuteNonQueryAsync();
                    respuesta = filas != 0;
                }
                catch
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }
    }
}