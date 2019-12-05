using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using subir_archivos.Models;

namespace subir_archivos.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly subir_archivosContext _context;

        public UsuariosController(subir_archivosContext context, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuario.ToListAsync());
            
        }
        public IList<Usuario> GetAll()
        {
            return _context.Usuario.FromSql("Select * from Usuario").ToList();
        }
        public Usuario GetById(int UsuarioID)
        {
            //var usuario = _context.Usuario.FromSql("Select * from Usuario where UsuarioID={UsuarioID}",UsuarioID).FirstOrDefault();
            //return usuario;


            var idParameter = new SqlParameter("@UsuarioID",UsuarioID );
            var usuario = _context.Usuario.FromSql("Select * from Usuario where UsuarioID=@UsuarioID", idParameter).FirstOrDefault();
            return usuario;
        }
        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UsuarioID,Nombre,Apellido,Telefono")] Usuario usuario)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(usuario);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    var commandText = "Insert Into [Uusario](Nombre)values(@nombre)";
        //    var parameter = new SqlParameter("@nombre", usuario.Nombre);
        //    _context.Database.ExecuteSqlCommand(commandText, parameter);
        //    return View(usuario);
        //}

        public async Task<IActionResult> Create(UsuarioVM usuarioVM)
        {
            if (usuarioVM.File != null)
            {
                //subir archivos a wwwroot
                var fileName = Path.GetFileName(usuarioVM.File.FileName);
                //juzga si es un archivo PDF
                string ext = Path.GetExtension(usuarioVM.File.FileName);
                if (ext.ToLower() != ".pdf")
                {
                    return View();
                }
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await usuarioVM.File.CopyToAsync(fileSteam);
                }
                //su lógica para guardar filePath en la base de datos, por ejemplo
                Usuario usuario = new Usuario();
                usuario.Nombre = usuarioVM.Nombre;
                usuario.FilePath = filePath;

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();
            }
            else
            {

            }
            return View();
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioID,Nombre,Apellido,Telefono")] Usuario usuario)
        {
            if (id != usuario.UsuarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.UsuarioID == id);
        }

        public IActionResult DownloadFile(string filePath)
        {

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = "mozilla12-pdf.pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            //For preview pdf and the download it use below code
            // var stream = new FileStream(filePath, FileMode.Open);
            //return new FileStreamResult(stream, "application/pdf");
        }
    }
}
