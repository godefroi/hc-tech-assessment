using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PeopleSearch
{
	public class LatencyFilter : IActionFilter, IFilterMetadata
	{
		public void OnActionExecuted(ActionExecutedContext context) { }

		public void OnActionExecuting(ActionExecutingContext context) => Thread.Sleep(65);
	}
}
