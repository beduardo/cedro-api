using api.entidades;
using Microsoft.EntityFrameworkCore;

namespace api.data {
    public class ContextoBdAplicacao: DbContext {

        public ContextoBdAplicacao(DbContextOptions<ContextoBdAplicacao> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //Aplica as configurações das entidades
            modelBuilder.ApplyConfiguration(new typeconfigurations.RestaurantesTypeConfiguration());
            modelBuilder.ApplyConfiguration(new typeconfigurations.PratosTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Prato> Pratos { get; set; }
    }
}