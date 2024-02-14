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
    public class CarteirasController : Controller
    {
        private readonly AppDbContext _context;

        public CarteirasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Carteiras
        public async Task<IActionResult> Index()
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appDbContext = _context.Carteiras
            .Where(c => c.UsuarioId == int.Parse(authenticatedUserId))
            .Include(c => c.Usuario);

            return View(await appDbContext.ToListAsync());
        }

        // GET: Carteiras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carteiras == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var carteira = await _context.Carteiras
                .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (carteira == null)
            {
                return NotFound();
            }

            return View(carteira);
        }

        // GET: Carteiras/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Carteiras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Saldo,UsuarioId")] Carteira carteira)
        {
            if (ModelState.IsValid)
            {
                string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                carteira.UsuarioId = int.Parse(authenticatedUserId);

                if (carteira.Saldo is null)
                {
                    carteira.Saldo = 0;
                }
                _context.Add(carteira);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", carteira.UsuarioId);
            return View(carteira);
        }

        // GET: Carteiras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carteiras == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var carteira = await _context.Carteiras
                .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (carteira == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", carteira.UsuarioId);
            return View(carteira);
        }

        // POST: Carteiras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Saldo,UsuarioId")] Carteira carteira)
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != carteira.Id || carteira.UsuarioId != int.Parse(authenticatedUserId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carteira);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarteiraExists(carteira.Id))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Email", carteira.UsuarioId);
            return View(carteira);
        }

        // GET: Carteiras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carteiras == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var carteira = await _context.Carteiras
                .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (carteira == null)
            {
                return NotFound();
            }

            return View(carteira);
        }

        // POST: Carteiras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carteiras == null)
            {
                return Problem("Entity set 'AppDbContext.Carteiras'  is null.");
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var carteira = await _context.Carteiras
                .Where(c => c.Id == id && c.UsuarioId == int.Parse(authenticatedUserId))
                .FirstOrDefaultAsync();

            if (carteira != null)
            {
                _context.Carteiras.Remove(carteira);
            } else
            {
                return NotFound();
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarteiraExists(int id)
        {
          return (_context.Carteiras?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
