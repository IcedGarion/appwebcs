using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Upo.Data;

namespace Upo
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /*
             * AddDbContext NON CHIAMA IL COSTRUTTORE di UPOECOMMERCECONTEXT:
             * Aggiunge Context ma non imposta la connessione.
             * ConnectionString impostata nel UpoECommerceContext
             */

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<UpoECommerceContext>(options =>
                options.UseSqlServer(connectionString));
            


            /*
             * AUTORIZZAZIONI settate nelle pagine stesse, controllando i ruoli in SESSION
             * NO AspNetAuthotization
             * 
            services.AddDbContext<IdentityDbContext>(options =>
               options.UseSqlServer(
                   connectionString,
                   optionsBuilder => optionsBuilder.MigrationsAssembly("UpoECommerce")
               )
           );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            */

            //Per iniettare HttpContextAccessor nelle Views
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc();
            //session
            services.AddDistributedMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //session
            app.UseSession();
            //app.UseIdentity();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response
                .WriteAsync("<html>\n" +
                                "<head>\n<title>NOT FOUND</title>\n" +
                                    "<link rel=\"stylesheet\" type=\"text/css\" href=\"./css/StyleSheet.css\">\n" +
                                    "<link href=\"./styles/layout.css?v=1\" rel=\"stylesheet\"/>\n" +
                                "</head>\n" + 
                                "<body>\n<div class=\"container\">\n<div class=\"main\">\n<h1>404 NOT FOUND</h1>\n</div>\n</div>\n</body>\n" +
                            "</html>");
            });
        }
    }
}
