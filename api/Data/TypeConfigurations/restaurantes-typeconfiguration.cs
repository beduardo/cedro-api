using api.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.data.typeconfigurations {
    public class RestaurantesTypeConfiguration : IEntityTypeConfiguration<Restaurante>
    {
        public void Configure(EntityTypeBuilder<Restaurante> builder)
        {
            builder.HasQueryFilter(b => !b.Excluido);
        }
    }
}