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
    public class TransacoesController : Controller
    {
        private readonly AppDbContext _context;

        public TransacoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Transacoes
        public async Task<IActionResult> Index(string? data, int? tipo, int? categoria)
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transacoes = _context.Transacoes
            .Include(t => t.Carteira)
            .Include(t => t.Categoria)
            .ThenInclude(c => c.Usuario)
            .Where(t => t.Carteira.UsuarioId == int.Parse(authenticatedUserId))
            .Where(t => t.Categoria.UsuarioId == int.Parse(authenticatedUserId))
            .ToList();

            var filtro = transacoes.AsQueryable();

            if (data != null)
            {
                filtro = filtro.Where(t => t.Data.Date == DateTime.Parse(data)); ;
            }

            if (tipo != null)
            {
                TipoTransacao filtroTipo = TipoTransacao.Receita;
                if(tipo == 1)
                {
                    filtroTipo = TipoTransacao.Despesa;
                }
                filtro = filtro.Where(t => t.Tipo == filtroTipo);
            }

            if (categoria != null)
            {
                filtro = filtro.Where(t => t.CategoriaId == categoria);
            }

            var despesas = transacoes
                .Where(t => t.Tipo == TipoTransacao.Despesa)
                .Sum(t => t.Valor);

            var receitas = transacoes
                .Where(t => t.Tipo == TipoTransacao.Receita)
                .Sum(t => t.Valor);

            var categorias = _context.Categorias
                .Where(t => t.UsuarioId == int.Parse(authenticatedUserId))
                .ToList();


            ViewBag.receita = receitas;
            ViewBag.despesa = despesas;
            ViewBag.categoria = categorias;

            return View(filtro);
        }

        // GET: Transacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transacoes == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transacao = await _context.Transacoes
            .Include(t => t.Carteira)
            .ThenInclude(c => c.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id && t.Carteira.UsuarioId == int.Parse(authenticatedUserId));

            if (transacao == null)
            {
                return NotFound();
            }

            return View(transacao);
        }

        // GET: Transacoes/Create
        public IActionResult Create(int? errorCategoria, int? errorCarteira)
        {
            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carteirasDoUsuario = _context.Carteiras.Where(carteira => carteira.UsuarioId == int.Parse(authenticatedUserId)).ToList();
            var categoriasDoUsuario = _context.Categorias.Where(categoria => categoria.UsuarioId == int.Parse(authenticatedUserId)).ToList();

            ViewData["CarteiraId"] = new SelectList(carteirasDoUsuario, "Id", "Titulo");
            ViewData["CategoriaId"] = new SelectList(categoriasDoUsuario, "Id", "Titulo");

            if (carteirasDoUsuario.Count() <= 0)
            {
                ViewBag.errorCarteira = 1;
            }

            if (categoriasDoUsuario.Count() <= 0)
            {
                ViewBag.errorCategoria = 1;
            }

            return View();
        }

        // POST: Transacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Valor,Data,Descricao,Tipo,CarteiraId,CategoriaId")] Transacao transacao)
        {
            if (ModelState.IsValid)
            {
                string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var carteira = await _context.Carteiras.FirstOrDefaultAsync(c => c.Id == transacao.CarteiraId && c.UsuarioId == int.Parse(authenticatedUserId));
                var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == transacao.CategoriaId && c.UsuarioId == int.Parse(authenticatedUserId));

                if (carteira == null || categoria == null)
                {
                    return NotFound();
                }

                _context.Add(transacao);
                await _context.SaveChangesAsync();

                if (carteira is not null)
                {
                    if (transacao.Tipo == TipoTransacao.Receita)
                    {
                        carteira.Saldo += transacao.Valor;
                    }
                    else
                    {
                        carteira.Saldo -= transacao.Valor;
                    }
                    _context.Update(carteira);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarteiraId"] = new SelectList(_context.Carteiras, "Id", "Titulo", transacao.CarteiraId);
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacao.CategoriaId);
            return View(transacao);
        }

        // GET: Transacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transacoes == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transacao = await _context.Transacoes.FirstOrDefaultAsync(t => t.Id == id && t.Carteira.UsuarioId == int.Parse(authenticatedUserId));

            if (transacao == null)
            {
                return NotFound();
            }

            var carteirasDoUsuario = _context.Carteiras.Where(carteira => carteira.UsuarioId == int.Parse(authenticatedUserId)).ToList();
            var categoriasDoUsuario = _context.Categorias.Where(categoria => categoria.UsuarioId == int.Parse(authenticatedUserId)).ToList();

            ViewData["CarteiraId"] = new SelectList(carteirasDoUsuario, "Id", "Titulo", transacao.CarteiraId);
            ViewData["CategoriaId"] = new SelectList(categoriasDoUsuario, "Id", "Titulo", transacao.CategoriaId);
            return View(transacao);
        }

        // POST: Transacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Valor,Data,Descricao,Tipo,CarteiraId,CategoriaId")] Transacao transacao)
        {
            if (id != transacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var carteira = await _context.Carteiras.FindAsync(transacao.CarteiraId);

                    if (carteira?.UsuarioId != int.Parse(authenticatedUserId))
                    {
                        return NotFound();
                    }

                    var transacaoAntiga = await _context.Transacoes.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

                    _context.Update(transacao);
                    await _context.SaveChangesAsync();

                    if (carteira is not null)
                    {
                        if (transacaoAntiga.Valor < transacao.Valor)
                        {
                            if (transacao.Tipo == TipoTransacao.Receita)
                            {
                                carteira.Saldo += (transacao.Valor - transacaoAntiga.Valor);
                            }
                            else
                            {
                                carteira.Saldo -= (transacao.Valor - transacaoAntiga.Valor);
                            }
                        }
                        else
                        {
                            if (transacao.Tipo == TipoTransacao.Receita)
                            {
                                carteira.Saldo -= (transacaoAntiga.Valor - transacao.Valor);
                            }
                            else
                            {
                                carteira.Saldo += (transacaoAntiga.Valor - transacao.Valor);
                            }
                                
                        }
                        _context.Update(carteira);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransacaoExists(transacao.Id))
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
            ViewData["CarteiraId"] = new SelectList(_context.Carteiras, "Id", "Titulo", transacao.CarteiraId);
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Titulo", transacao.CategoriaId);
            return View(transacao);
        }

        // GET: Transacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transacoes == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transacao = await _context.Transacoes
            .Include(t => t.Carteira)
            .ThenInclude(c => c.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id && t.Carteira.UsuarioId == int.Parse(authenticatedUserId));

            if (transacao == null)
            {
                return NotFound();
            }

            return View(transacao);
        }

        // POST: Transacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transacoes == null)
            {
                return Problem("Entity set 'AppDbContext.Transacoes'  is null.");
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transacao = await _context.Transacoes
            .Include(t => t.Carteira)
            .ThenInclude(c => c.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id && t.Carteira.UsuarioId == int.Parse(authenticatedUserId));

            if (transacao != null)
            {
                var carteira = await _context.Carteiras.FindAsync(transacao.CarteiraId);
                if (carteira is not null)
                {
                    if (transacao.Tipo == TipoTransacao.Receita)
                    {
                        carteira.Saldo -= transacao.Valor;
                    }
                    else
                    {
                        carteira.Saldo += transacao.Valor;
                    }
                    _context.Update(carteira);
                    await _context.SaveChangesAsync();
                }
                _context.Transacoes.Remove(transacao);
            } else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransacaoExists(int id)
        {
            return (_context.Transacoes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
