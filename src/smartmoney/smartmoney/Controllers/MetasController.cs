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
    public class MetasController : Controller
    {
        private readonly AppDbContext _context;

        public MetasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Metas
        public async Task<IActionResult> Index()
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appDbContext = _context.Metas
            .Where(m => m.UsuarioId == int.Parse(authenticatedUserId))
            .Include(m => m.Usuario);

            var metas = await appDbContext.ToListAsync();
            return View(metas);
        }

        // GET: Metas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Metas == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var meta = await _context.Metas
                .Where(m => m.Id == id && m.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (meta == null)
            {
                return NotFound();
            }

            return View(meta);
        }

        // GET: Metas/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Metas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,DataInicial,DataFinal,ValorMeta,Valor,UsuarioId")] Meta meta)
        {
            if (ModelState.IsValid)
            {
                string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                meta.UsuarioId = int.Parse(authenticatedUserId);

                _context.Add(meta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", meta.UsuarioId);
            return View(meta);
        }

        // GET: Metas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Metas == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var meta = await _context.Metas
                .Where(m => m.Id == id && m.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (meta == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", meta.UsuarioId);
            return View(meta);
        }

        // POST: Metas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,DataInicial,DataFinal,ValorMeta,Valor,UsuarioId")] Meta meta)
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != meta.Id || meta.UsuarioId != int.Parse(authenticatedUserId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MetaExists(meta.Id))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", meta.UsuarioId);
            return View(meta);
        }

        // GET: Metas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Metas == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var meta = await _context.Metas
                .Where(m => m.Id == id && m.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (meta == null)
            {
                return NotFound();
            }

            return View(meta);
        }

        // POST: Metas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Metas == null)
            {
                return Problem("Entity set 'AppDbContext.Metas'  is null.");
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var meta = await _context.Metas
                .Where(m => m.Id == id && m.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (meta != null)
            {
                _context.Metas.Remove(meta);
            }
            else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MetaExists(int id)
        {
            return (_context.Metas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
