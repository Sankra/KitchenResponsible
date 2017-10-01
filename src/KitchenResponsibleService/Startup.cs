﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KitchenResponsibleService.Configuration;
using KitchenResponsibleService.Db;
using KitchenResponsibleService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KitchenResponsibleService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // TODO: Add end-to-end tests like: https://github.com/seesharper/Blog.AspNetCoreUnitTesting
            // TODO: Add metrics https://github.com/Recognos/Metrics.NET
            // TODO: Add dashboard https://github.com/grafana/grafana
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMvc();

            services.AddSingleton(ReadBlobStorageConfig());
            services.AddSingleton<IStorage, BlobStorage>();
            services.AddSingleton<KitchenService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseStaticFiles();
        }

		static BlobStorageConfiguration ReadBlobStorageConfig()
		{
			return JsonConvert.DeserializeObject<BlobStorageConfiguration>(File.ReadAllText("config.json"));
		}
    }
}
