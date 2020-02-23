using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using PeopleSearch.Controllers;
using PeopleSearch.Models;
using Xunit;
using Xunit.Abstractions;

namespace PeopleSearchTests
{
	public class PersonControllerTests
	{
		private ILoggerFactory    m_loggerFactory;
		private ITestOutputHelper m_output;

		public PersonControllerTests(ITestOutputHelper output)
		{
			var serviceProvider = new ServiceCollection().AddLogging().BuildServiceProvider();

			m_loggerFactory = serviceProvider.GetService<ILoggerFactory>();

			m_loggerFactory.AddProvider(new XUnitLoggerProvider(output));
		}

		[Fact]
		public void SearchWithoutCriteriaReturnsNothing()
		{
			// first, clear out the database and add some data real quick
			PeopleSearchContext.Initialize(true, true);

			using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
				ctx.AddRange(Enumerable.Range(0, 100).Select(i => new Person() { FirstName = $"John {i}", LastName = "Smith" }));
				ctx.SaveChanges();

				// then, make sure none of it is returned
				var ctrlr = new PersonController(m_loggerFactory.CreateLogger<PersonController>(), ctx);

				Assert.Empty(ctrlr.Index());
			}
		}

		[Fact]
		public void SearchWithCriteriaReturnsPeople()
		{
			// first, clear out the database and add some data real quick
			PeopleSearchContext.Initialize(true, true);

			using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
				ctx.AddRange(Enumerable.Range(0, 100).Select(i => new Person() { FirstName = $"John {i}", LastName = "Smith" }));
				ctx.AddRange(Enumerable.Range(0, 5).Select(i => new Person() { FirstName = $"John {i}", LastName = "Jones" }));
				ctx.SaveChanges();

				// ensure that the correct number of results are returned
				var ctrlr = new PersonController(m_loggerFactory.CreateLogger<PersonController>(), ctx);

				Assert.Equal(5, ctrlr.Index("Jones", 0, 10).Count());
				Assert.Equal(5, ctrlr.Index("Jones", 0, 20).Count());
			}
		}

		[Fact]
		public void WildcardSearchesWork()
		{
			// first, clear out the database and add some data real quick
			PeopleSearchContext.Initialize(true, true);

			// don't log the inserts, we're not interested in those
			using( var ctx = new PeopleSearchContext() ) {
				ctx.AddRange(Enumerable.Range(0, 100).Select(i => new Person() { FirstName = $"John {i}", LastName = "Smith" }));

				ctx.Add(new Person() { FirstName = "abcyyydef" });
				ctx.Add(new Person() { FirstName = "yyyabcdef" });
				ctx.Add(new Person() { FirstName = "abcdefyyy" });

				ctx.Add(new Person() { LastName = "abcyyydef" });
				ctx.Add(new Person() { LastName = "yyyabcdef" });
				ctx.Add(new Person() { LastName = "abcdefyyy" });

				ctx.SaveChanges();
			}

			using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
				var ctrlr = new PersonController(m_loggerFactory.CreateLogger<PersonController>(), ctx);

				// this one should return all 6, because % is zero or more characters
				Assert.Equal(6, ctrlr.Index("%yyy%", 0, 10).Count());

				// each of these should return two, one each for first name match and last name match
				Assert.Collection(ctrlr.Index("yyy%", 0, 10), p => Assert.True(p.FirstName == "yyyabcdef"), p => Assert.True(p.LastName == "yyyabcdef"));
				Assert.Collection(ctrlr.Index("%yyy", 0, 10), p => Assert.True(p.FirstName == "abcdefyyy"), p => Assert.True(p.LastName == "abcdefyyy"));
			}
		}

		[Fact]
		public void AddingAPersonWorks()
		{
			// first, clear out the database so we don't have any collisions
			PeopleSearchContext.Initialize(true, true);

			using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
				var ctrlr = new PersonController(m_loggerFactory.CreateLogger<PersonController>(), ctx);

				ctrlr.Index(new Person() {
					FirstName = "newtest",
					LastName  = "person",
				});

				Assert.NotNull(ctx.People.Single(p => p.FirstName == "newtest" && p.LastName == "person"));
			}
		}

		[Fact]
		public void AddingAnExistingPersonFails()
		{
			// first, clear out the database so we don't have any collisions
			PeopleSearchContext.Initialize(true, true);

			using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
				var ctrlr = new PersonController(m_loggerFactory.CreateLogger<PersonController>(), ctx);

				Assert.IsType<BadRequestResult>(ctrlr.Index(new Person() {
					PersonId  = 10,
					FirstName = "newtest",
					LastName  = "person",
				}));
			}
		}

		[Fact]
		public void UpdatingAPersonWorks()
		{
			// first, clear out the database so we don't have any collisions
			PeopleSearchContext.Initialize(true, true);

			using( var ctx = new PeopleSearchTestContext(m_loggerFactory) ) {
				// add a person that we can use to update
				ctx.Add(new Person() { FirstName = "test", LastName = "person" });
				ctx.SaveChanges();

				// grab the person from the db
				var person = ctx.People.Single(p => p.FirstName == "test");

				// mutate the person
				person.LastName = "other";

				var ctrlr = new PersonController(m_loggerFactory.CreateLogger<PersonController>(), ctx);

				// use the controller to update the person
				ctrlr.Index(person.PersonId, person);

				// re-retrieve the person from the db
				person = ctx.People.Single(p => p.PersonId == person.PersonId);

				// ensure the last name was updated
				Assert.Equal("other", person.LastName);
			}
		}
	}
}
