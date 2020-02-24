using System;

using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PeopleSearch.GraphQL;

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

			services.AddControllers(o => o.Filters.Add<LatencyFilter>());

			services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
			services.AddScoped<PeopleSearchSchema>();
			services.AddGraphQL(o => o.ExposeExceptions = true).AddGraphTypes(ServiceLifetime.Scoped);

			services.AddGrpc(o => o.EnableDetailedErrors = true);

			services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
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
				endpoints.MapGrpcService<gRPC.PeopleSearchService>();
			});

			app.UseGraphQL<PeopleSearchSchema>();
			app.UseGraphQLPlayground(options: new GraphQLPlaygroundOptions());
		}
	}
}
