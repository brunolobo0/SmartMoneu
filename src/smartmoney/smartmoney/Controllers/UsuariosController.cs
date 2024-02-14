using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smartmoney.Models;
using smartmoney.Models.ViewModels;
using smartmoney.Services;

namespace smartmoney.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public UsuariosController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Senha")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var dados = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
                if (dados == null)
                {
                    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    TempData["MensagemSucesso"] = "Conta criada com sucesso. Você já pode fazer login";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ViewBag.Message = "Já existe uma conta com este e-mail";
                    return View();
                }
            }

            return View(usuario);
        }

        // GET: Usuarios/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            var dados = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);

            if (dados == null)
            {
                ViewBag.Message = "Usuário e/ou senha inválidos.";
                return View();
            }

            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(usuario.Senha, dados.Senha);

            if (senhaCorreta)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dados.Nome),
                    new Claim(ClaimTypes.NameIdentifier, dados.Id.ToString())
                };

                var usuarioIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(usuarioIdentity);

                var props = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.ToLocalTime().AddHours(8),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(principal, props);

                return Redirect("/");
            }
            else
            {
                ViewBag.Message = "Usuário e/ou senha inválidos.";
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Usuarios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Id == int.Parse(authenticatedUserId));

            if (usuario == null)
            {
                return NotFound();
            }

            UsuarioEditInfo usuarioInfo = new UsuarioEditInfo
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
            };

            return View(usuarioInfo);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email")] UsuarioEditInfo usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Id == int.Parse(authenticatedUserId));

            if (existingUsuario == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingUsuario.Nome = usuario.Nome;
                    existingUsuario.Email = usuario.Email;

                    _context.Entry(existingUsuario).State = EntityState.Modified;
                    TempData["MensagemSucesso"] = "Dados alterados com sucesso.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(usuario);
        }

        // GET: Usuarios/EditPassword/5
        [Authorize]
        public async Task<IActionResult> EditPassword(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Id == int.Parse(authenticatedUserId));

            if (usuario == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST: Usuarios/EditPassword/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(int id, [Bind("Id,Senha,ConfirmarSenha")] UsuarioEditPassword usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Id == int.Parse(authenticatedUserId));

            if (existingUsuario == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (usuario.Senha == usuario.ConfirmarSenha)
                    {
                        existingUsuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                        _context.Entry(existingUsuario).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        TempData["MensagemSucesso"] = "Senha alterada com sucesso.";
                    }
                    else
                    {
                        ViewBag.Message = "As senhas devem ser iguais.";
                        return View();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Id == int.Parse(authenticatedUserId));

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'AppDbContext.Usuarios'  is null.");
            }

            string authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.Id == int.Parse(authenticatedUserId));

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            await HttpContext.SignOutAsync();
            TempData["MensagemSucesso"] = "Conta excluída com sucesso.";
            return RedirectToAction("Login", "Usuarios");
        }

        //GET: EsqueciSenha
        public IActionResult EsqueciSenha()
        {
            return View();
        }

        // POST: EsqueciSenha
        [HttpPost]
        public async Task<IActionResult> EsqueciSenha([Bind("Email")] UsuarioEsqueciSenha dados)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dados.Email);

                if (usuario != null)
                {
                    var guid = Guid.NewGuid();
                    var token = guid.ToString();

                    usuario.TokenRedefinirSenha = token;
                    _context.Entry(usuario).State = EntityState.Modified;

                    var urlConfirmacao = Url.Action(nameof(RedefinirSenha), "Usuarios", new { token, user = usuario.Id }, Request.Scheme);
                    var mensagem = new StringBuilder();
                    mensagem.Append($"<p>Olá, {usuario.Nome}.</p>");
                    mensagem.Append("<p>Houve uma solicitação de redefinição de senha para seu usuário em nosso site. Se não foi você que fez a solicitação, ignore essa mensagem. Caso tenha sido você, clique no link abaixo para criar sua nova senha:</p>");
                    mensagem.Append($"<p><a href='{urlConfirmacao}'>Redefinir Senha</a></p>");
                    mensagem.Append("<p>Atenciosamente,<br>Equipe de Suporte</p>");
                    await _emailService.SendEmailAsync(usuario.Email,
                        "Redefinição de Senha", "", mensagem.ToString());

                    await _context.SaveChangesAsync();

                    return View(nameof(EmailRedefinicaoEnviado));
                }
                else
                {
                    ViewBag.Message = $"Usuário/e-mail <b>{dados.Email}</b> não encontrado.";
                    return View();
                }
            }
            else
            {
                return View(dados);
            }
        }

        public IActionResult EmailRedefinicaoEnviado()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RedefinirSenha(string token, string user)
        {
            var usuario = await _context.Usuarios.FindAsync(int.Parse(user));

            if (usuario != null && usuario.TokenRedefinirSenha == token)
            {
                var modelo = new UsuarioRedefinirSenha();
                modelo.Token = token;
                modelo.Id = int.Parse(user);

                return View(modelo);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RedefinirSenha([Bind("Id,Token,Senha,ConfirmarSenha")] UsuarioRedefinirSenha dados)
        {
            var usuario = await _context.Usuarios.FindAsync(dados.Id);

            if (ModelState.IsValid)
            {
                try
                {
                    if (usuario != null && dados.Token == usuario.TokenRedefinirSenha)
                    {
                        if (dados.Senha == dados.ConfirmarSenha)
                        {
                            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(dados.Senha);
                            _context.Entry(usuario).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            TempData["MensagemSucesso"] = "Senha redefinida com sucesso. Agora você já pode fazer login com a nova senha.";
                            return RedirectToAction("Login", "Usuarios");
                        }
                        else
                        {
                            ViewBag.Message = "As senhas devem ser iguais.";
                            return View(dados);
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(dados);
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
