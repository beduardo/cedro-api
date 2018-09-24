using api.entidades;
using api.models;
using AutoMapper;

namespace api.mapeamentos {
    public static class RestauranteMapeamentoProfile {
        public static void Mapear(Profile perfil) {
            perfil.CreateMap<Restaurante, RestaurantePersistenciaModel>();
            perfil.CreateMap<RestaurantePersistenciaModel, Restaurante>()
                .IgnorePadraoEntidade();

        }
    }
}