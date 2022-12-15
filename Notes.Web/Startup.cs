using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notes.BLL.AutoMapperProfiles;
using Notes.BLL.Services.AccountInfoManagers;
using Notes.BLL.Services.CurrentUserAccessor;
using Notes.BLL.Services.MarkdownRendererService;
using Notes.BLL.Services.NoteManagers;
using Notes.DAL.Models;
using Notes.DAL.Repositories;
using Notes.DAL.Repositories.Interfaces;
using Notes.Web.AutoMapperProfiles;
using Notes.Web.Services;

namespace Notes.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(connection);
            });

            services.AddIdentity<UserEntry, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();

            services.AddAutoMapper(
                typeof(NoteMappingProfile),
                typeof(NoteViewModelMappingProfile),
                typeof(AccountInfoViewModelMappingProfile),
                typeof(TagViewModelMappingProfile),
                typeof(SearchFilterMappingProfile)
                );

            services.AddHttpContextAccessor();
            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IMarkdownRenderer, MarkdownSharpAdapter>();
            services.AddTransient<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddTransient<INoteManager, NoteManager>();
            services.AddTransient<IAccountInfoManager, AccountInfoManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
