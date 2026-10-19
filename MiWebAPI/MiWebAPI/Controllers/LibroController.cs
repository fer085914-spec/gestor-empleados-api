using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MiWebAPI.Data;
using MiWebAPI.Models;

namespace MiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly LibroData _libroData;

        public LibroController(LibroData libroData)
        {
            _libroData = libroData;
        }

        // GET: api/Libro (Consultar todos los libros activos)
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Libro> lista = await _libroData.Lista();
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        // GET: api/Libro/1 (Consultar un libro específico por ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            Libro objeto = await _libroData.Obtener(id);
            if (objeto == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Libro no encontrado" });
            }
            return StatusCode(StatusCodes.Status200OK, objeto);
        }

        // POST: api/Libro (Registrar un nuevo libro)
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Libro objeto)
        {
            bool respuesta = await _libroData.Crear(objeto);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
        }

        // PUT: api/Libro (Actualizar los datos de un libro)
        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Libro objeto)
        {
            bool respuesta = await _libroData.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
        }

        // DELETE: api/Libro/1 (Dar de baja lógica un libro)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            bool respuesta = await _libroData.Eliminar(id);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
        }
    }
}