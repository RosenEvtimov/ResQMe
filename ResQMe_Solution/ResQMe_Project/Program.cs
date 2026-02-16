namespace ResQMe_Project
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using ResQMe.Data;
    using ResQMe.Data.Models.Identity;
    using ResQMe.Services.Core;
    using ResQMe.Services.Core.Interfaces;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            /* Builder DI */
            builder.Services.AddDbContext<ResQMeDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            /* Identity DI */
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ResQMeDbContext>();

            /* Optional setting for unique email - might remove later */
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });

            builder.Services.AddControllersWithViews();

            /* My Services DI */
            builder.Services.AddScoped<IAnimalService, AnimalService>();
            builder.Services.AddScoped<IShelterService, ShelterService>();
            builder.Services.AddScoped<ISpeciesService, SpeciesService>();
            builder.Services.AddScoped<IBreedService, BreedService>();
            builder.Services.AddScoped<IAdoptionRequestService, AdoptionRequestService>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            /* Allows us to use the default Authorization of Identity */
            app.UseAuthentication();

            /* Middleware to assign User role if missing */
            app.Use(async (context, next) =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();

                    var user = await userManager.GetUserAsync(context.User);

                    if (user != null)
                    {
                        var roles = await userManager.GetRolesAsync(user);
                        if (!roles.Any())
                        {
                            await userManager.AddToRoleAsync(user, "User");
                            await signInManager.RefreshSignInAsync(user);
                        }
                    }
                }

                await next();
            });

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            /* Seeding Roles and an Admin Account */
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles = { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                string adminEmail = "admin@admin.com";
                string adminPassword = "Admin123!";

                var admin = await userManager.FindByEmailAsync(adminEmail);

                if (admin == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, adminPassword);
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            app.Run();
        }
    }
}