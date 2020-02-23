using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PeopleSearch
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This method is called by the runtime; marking static is not possible.")]
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<Models.PeopleSearchContext>();

			services.AddControllers();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This method is called by the runtime; marking static is not possible.")]
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if( env.IsDevelopment() ) {
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}
	}
}
