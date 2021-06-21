using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AdvertisementPrediction.Repositories;
using AdvertisementPrediction.Commands;
using AdvertisementPrediction.Model;
using RestSharp;

namespace AdvertisementPrediction
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public CosmosClient GetCosmosClient()
        {
            CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(
                accountEndpoint: Configuration["CosmosDb:Endpoint"],
                authKeyOrResourceToken: Configuration["CosmosDb:Key"])
                .WithConnectionModeGateway();
            CosmosClient client = cosmosClientBuilder.Build();
            return client;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAdvertisementRepository, AdvertisementRepository>();
            services.AddSingleton<CosmosClient>(x => GetCosmosClient());

            services.AddScoped<IPredictionRepository, PredictionRepository>();
            services.AddDbContext<AdvertisementPredictionContext>();
            services.AddScoped<AddPredictionCommand>();
            services.AddScoped<UpdatePredictionCommand>();
            services.AddScoped<DeletePredictionCommand>();

            services.AddSingleton<IExampleRepository, ExampleRepository>();
            services.AddSingleton<AddExampleCommand>();
            services.AddSingleton<UpdateExampleCommand>();
            services.AddSingleton<DeleteExampleCommand>();

            services.AddSingleton<IRestClient>( x => new RestClient($"http://127.0.0.1:80/")  );
            services.AddControllers().AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin() 
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
