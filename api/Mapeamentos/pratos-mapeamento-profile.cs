using api.entidades;
using api.models;
using AutoMapper;

namespace api.mapeamentos
{
    public static class PratoMapeamentoProfile
    {
        public static void Mapear(Profile perfil)
        {
            perfil.CreateMap<Prato, PratoPersistenciaModel>();
            perfil.CreateMap<PratoPersistenciaModel, Prato>()
                .ForMember(d => d.Restaurante, opt => opt.Ignore())
                .IgnorePadraoEntidade();

        }
    }
}