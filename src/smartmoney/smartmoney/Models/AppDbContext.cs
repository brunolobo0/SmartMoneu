using Microsoft.EntityFrameworkCore;

namespace smartmoney.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Carteira> Carteiras { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Meta> Metas { get; set; }

    }
}
