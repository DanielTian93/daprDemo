using Actors;
using daprA.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace daprA
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

            services.AddGrpc();
            services.AddControllers().AddDapr();
            services.AddActors(options =>
            {
                options.Actors.RegisterActor<DemoActor>();
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "daprA", Version = "v1" });
            });
            RedisHelper.Initialization(csredis: new CSRedis.CSRedisClient("192.168.2.228:6379,password=,prefix=actor_,poolsize=2"));

            #region ����
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.WithOrigins("*").AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "daprA v1"));

            app.UseHttpsRedirection();

            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

            app.UseRouting();
            app.UseCors("any");

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                //��������
                endpoints.MapSubscribeHandler();
                //grpc
                endpoints.MapGrpcService<GrpcService>();
                endpoints.MapControllers();
                //Actors
                endpoints.MapActorsHandlers();
            });
        }
    }
}
