using CatalogoToolsWeb.Data;
using CatalogoToolsWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;

namespace CatalogoToolsWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        // Muestra todos los proveedores de forma asíncrona
        public async Task<IActionResult> Index()
        {
            var proveedores = await _db.Proveedores.ToListAsync();
            return View(proveedores);
        }

        // GET: muestra formulario para subir logo
        public async Task<IActionResult> UploadLogo(int id)
        {
            var proveedor = await _db.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: recibe el archivo y lo guarda en la columna iLogo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLogo(int id, IFormFile logoFile)
        {
            var proveedor = await _db.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            if (logoFile == null || logoFile.Length == 0)
            {
                ModelState.AddModelError("logoFile", "Seleccione un archivo de imagen.");
                return View(proveedor);
            }

            // Validaciones básicas
            if (logoFile.Length > 2_000_000) // 2 MB límite
            {
                ModelState.AddModelError("logoFile", "El archivo es demasiado grande (máx. 2 MB).");
                return View(proveedor);
            }

            if (!logoFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("logoFile", "El archivo debe ser una imagen.");
                return View(proveedor);
            }

            // Convertir a byte[] y guardar
            using (var ms = new MemoryStream())
            {
                await logoFile.CopyToAsync(ms);
                proveedor.Logo = ms.ToArray();
            }

            _db.Update(proveedor);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
