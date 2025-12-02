using CatalogoToolsWeb.Data;
using CatalogoToolsWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Data;

namespace CatalogoToolsWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        // Muestra todos los proveedores de forma asíncrona
        public async Task<IActionResult> Admin()
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

            return RedirectToAction(nameof(Admin));
        }

        // POST: subir múltiples imágenes y guardarlas en tbl_Productos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(int id, List<IFormFile> imageFiles)
        {
            var proveedor = await _db.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            if (imageFiles == null || imageFiles.Count == 0)
            {
                TempData["UploadImagesError"] = "Seleccione al menos una imagen.";
                return RedirectToAction(nameof(Admin));
            }

            var errores = new List<string>();

            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var sqlConn = (SqlConnection)conn;
            using var transaction = sqlConn.BeginTransaction();
            try
            {
                using var cmd = sqlConn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = "INSERT INTO dbo.tbl_Productos (nIdProveedor, nNombre, iImage) VALUES (@nIdProveedor, @nNombre, @iImage);";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@nIdProveedor", SqlDbType.Int) { Value = id });
                var pNombre = new SqlParameter("@nNombre", SqlDbType.VarChar, 150);
                cmd.Parameters.Add(pNombre);
                var pImage = new SqlParameter("@iImage", SqlDbType.VarBinary, -1);
                cmd.Parameters.Add(pImage);

                foreach (var file in imageFiles)
                {
                    if (file == null || file.Length == 0)
                    {
                        errores.Add("Archivo vacío.");
                        continue;
                    }

                    if (file.Length > 2_000_000)
                    {
                        errores.Add($"'{file.FileName}' supera 2 MB.");
                        continue;
                    }

                    if (!file.ContentType.StartsWith("image/"))
                    {
                        errores.Add($"'{file.FileName}' no es una imagen válida.");
                        continue;
                    }

                    byte[] data;
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        data = ms.ToArray();
                    }

                    var fileName = Path.GetFileName(file.FileName) ?? "imagen";
                    if (fileName.Length > 150) fileName = fileName.Substring(0, 150);

                    pNombre.Value = fileName;
                    pImage.Value = data;

                    await cmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); } catch { }
                TempData["UploadImagesError"] = "Error al insertar imágenes: " + ex.Message;
                return RedirectToAction(nameof(Admin));
            }

            if (errores.Count > 0)
            {
                TempData["UploadImagesError"] = string.Join(" | ", errores);
            }
            else
            {
                TempData["UploadImagesSuccess"] = "Imágenes subidas correctamente.";
            }

            return RedirectToAction(nameof(Admin));
        }

        // GET: formulario para crear proveedor
        public IActionResult Create()
        {
            return View();
        }

        // POST: insertar proveedor llamando al SP dbo.Insert_tbl_Proveedores
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombre, IFormFile? logoFile, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ModelState.AddModelError("nombre", "El nombre es requerido.");
                var lista = await _db.Proveedores.ToListAsync();
                return View("Admin", lista);
            }

            byte[]? logo = null;
            if (logoFile != null && logoFile.Length > 0)
            {
                if (logoFile.Length > 2_000_000)
                {
                    ModelState.AddModelError("logoFile", "El archivo es demasiado grande (máx. 2 MB).");
                    var lista = await _db.Proveedores.ToListAsync();
                    return View("Admin", lista);
                }

                if (!logoFile.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("logoFile", "El archivo debe ser una imagen.");
                    var lista = await _db.Proveedores.ToListAsync();
                    return View("Admin", lista);
                }

                using var ms = new MemoryStream();
                await logoFile.CopyToAsync(ms);
                logo = ms.ToArray();
            }

            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var sqlConn = (SqlConnection)conn;
            using var cmd = sqlConn.CreateCommand();
            cmd.CommandText = "dbo.Insert_tbl_Proveedores";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@nNombre", SqlDbType.VarChar, 150) { Value = nombre });
            cmd.Parameters.Add(new SqlParameter("@iLogo", SqlDbType.VarBinary, -1) { Value = (object?)logo ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@bActivo", SqlDbType.Bit) { Value = activo });

            var pNewId = new SqlParameter("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pNewId);

            await cmd.ExecuteNonQueryAsync();

            // opcional: obtener el id insertado
            // var newId = pNewId.Value == DBNull.Value ? 0 : Convert.ToInt32(pNewId.Value);

            return RedirectToAction(nameof(Admin));
        }

        // POST: cambiar estado activo/desactivo llamando al SP dbo.Set_tbl_ProveedorActivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var proveedor = await _db.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            var nuevoEstado = !proveedor.Activo;

            using var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            var sqlConn = (SqlConnection)conn;
            using var cmd = sqlConn.CreateCommand();
            cmd.CommandText = "dbo.Set_tbl_ProveedorActivo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            cmd.Parameters.Add(new SqlParameter("@bActivo", SqlDbType.Bit) { Value = nuevoEstado });

            await cmd.ExecuteNonQueryAsync();

            return RedirectToAction(nameof(Admin));
        }

        public IActionResult Catalog()
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
