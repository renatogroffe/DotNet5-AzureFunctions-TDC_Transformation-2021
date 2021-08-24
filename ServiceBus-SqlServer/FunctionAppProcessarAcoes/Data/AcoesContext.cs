using Microsoft.EntityFrameworkCore;
using FunctionAppProcessarAcoes.Models;

namespace FunctionAppProcessarAcoes.Data
{
    public class AcoesContext : DbContext
    {
        public DbSet<Acao> Acoes { get; set; }

        public AcoesContext(DbContextOptions<AcoesContext> options) :
            base(options)
        {
        }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acao>()
                .HasKey(a => a.Id);
        }
    }
}