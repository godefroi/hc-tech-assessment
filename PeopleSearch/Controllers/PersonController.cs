using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PeopleSearch.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PeopleSearch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> m_logger;

        public PersonController(ILogger<PersonController> logger) => m_logger = logger;

        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return null;
        }
    }
}
