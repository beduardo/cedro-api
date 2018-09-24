using api.entidades;
using AutoMapper;

namespace api.mapeamentos {
    public static class ExtensoesMappingExpression {
        public static IMappingExpression<TSource, TDest> IgnorePadraoEntidade<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        where TDest : EntidadeBase {
            return expression
                .ForMember(d => d.DataCriacao, opt => opt.Ignore())
                .ForMember(d => d.DataAlteracao, opt => opt.Ignore())
                .ForMember(d => d.Excluido, opt => opt.Ignore());
        }
    }

    public class ConfiguracaoAutoMapper : Profile {
        public ConfiguracaoAutoMapper() {
            RestauranteMapeamentoProfile.Mapear(this);
        }
    }
}