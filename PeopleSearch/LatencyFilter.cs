using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace PeopleSearch
{
	public class LatencyFilter : IActionFilter, IFilterMetadata
	{
		private readonly int m_delay;

		public LatencyFilter(IConfiguration configuration)
		{
			m_delay = configuration.GetValue<int>("LatencySimulation", 0);
		}

		public void OnActionExecuted(ActionExecutedContext context) { }

		public void OnActionExecuting(ActionExecutingContext context)
		{
			if( m_delay > 0)
				Thread.Sleep(m_delay);
		}
	}
}
