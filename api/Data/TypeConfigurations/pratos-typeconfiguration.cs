using api.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.data.typeconfigurations {
    public class PratosTypeConfiguration : IEntityTypeConfiguration<Prato>
    {
        public void Configure(EntityTypeBuilder<Prato> builder)
        {
            builder.HasQueryFilter(b => !b.Excluido);
        }
    }
}