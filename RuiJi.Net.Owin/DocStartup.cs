using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Owin
{
    public class DocStartup
    {
        public void Configure(IApplicationBuilder builder)
        {
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            builder.UseDefaultFiles(options);
            builder.UseStaticFiles();
        }
    }
}