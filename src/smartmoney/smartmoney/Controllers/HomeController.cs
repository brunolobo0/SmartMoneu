using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartmoney.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace smartmoney.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.saldo = await GetSaldoCarteiras(userId);
            ViewBag.receita = await GetValores(TipoTransacao.Receita, userId);
            ViewBag.despesa = await GetValores(TipoTransacao.Despesa, userId);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<decimal> GetValores(TipoTransacao tipo, int userId)
        {
            return (from t in _context.Transacoes
                    join c in _context.Carteiras
                        on t.CarteiraId equals c.Id
                    where t.Tipo == tipo &&
                    c.UsuarioId == userId
                    select t.Valor).Sum();
        }
        private async Task<decimal?> GetSaldoCarteiras(int userId)
        {
            return _context.Carteiras
                 .Where(carteira => carteira.UsuarioId == userId)
                 .Sum(carteira => carteira.Saldo);
        }
    }
}