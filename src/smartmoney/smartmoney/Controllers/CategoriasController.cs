using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smartmoney.Models;

namespace smartmoney.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appDbContext = _context.Categorias
            .Where(c => c.UsuarioId == int.Parse(authenticatedUserId))
            .Include(c => c.Usuario);

            return View(await appDbContext.ToListAsync());
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var categoria = await _context.Categorias
                .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,UsuarioId")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                categoria.UsuarioId = int.Parse(authenticatedUserId);

                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", categoria.UsuarioId);
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var categoria = await _context.Categorias
                .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", categoria.UsuarioId);
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,UsuarioId")] Categoria categoria)
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != categoria.Id || categoria.UsuarioId != int.Parse(authenticatedUserId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", categoria.UsuarioId);
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id, int? error)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var categoria = await _context.Categorias
                .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound();
            }

            if (error != null)
            {
                ViewBag.error = 1;
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categorias == null)
            {
                return Problem("Entity set 'AppDbContext.Categorias'  is null.");
            }

            try
            {
                string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var categoria = await _context.Categorias
                    .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                    .FirstOrDefaultAsync();

                if (categoria != null)
                {
                    _context.Categorias.Remove(categoria);
                }
                else
                {
                    return NotFound();
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction("Delete", new Dictionary<string, object>
                {
                    { "id", id },
                    { "error", 1 }
                });
            }
        }

        private bool CategoriaExists(int id)
        {
            return (_context.Categorias?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
