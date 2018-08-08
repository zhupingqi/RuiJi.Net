using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RuiJi.Net.Owin
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDirectoryBrowser();
        }

        public void Configure(IApplicationBuilder builder)
        {
            builder.UseDeveloperExceptionPage();
            builder.UseCors();

            builder.UseMvc();

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            builder.UseDefaultFiles(options);
            builder.UseStaticFiles();
            builder.UseDirectoryBrowser("/download");
        }
    }
}