﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using api.servicos.persistencia;
using AutoMapper;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddEntityFrameworkNpgsql()
            .AddDbContext<data.ContextoBdAplicacao>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Mapeamentos AutoMapper
            MapperConfiguration automapper_configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<mapeamentos.ConfiguracaoAutoMapper>();
            });
            services.AddSingleton<IMapper>(sp => automapper_configuration.CreateMapper());

            //Serviços Persistência
            services.AddScoped<IServicoPersistenciaRestaurante, ServicoPersistenciaRestaurante>();
            services.AddScoped<IServicoPersistenciaPrato, ServicoPersistenciaPrato>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(bld =>
            {
                bld.AllowAnyOrigin();
                bld.AllowAnyHeader();
                bld.AllowAnyMethod();
                bld.AllowCredentials();
            });

            app.UseMvc();
        }
    }
}
