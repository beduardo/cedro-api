using api.mapeamentos;
using AutoMapper;
using Xunit;

namespace test_api.mapeamentos
{
    public class TestesAutomapper
    {
        [Fact(DisplayName = "Mapeamentos gerais")]
        public void Mapeamentos()
        {
            MapperConfiguration automapper_configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ConfiguracaoAutoMapper>();
            });

            automapper_configuration.AssertConfigurationIsValid();
        }
    }
}