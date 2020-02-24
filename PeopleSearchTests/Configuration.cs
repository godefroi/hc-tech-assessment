using Xunit;

// We disable test parallelization because we've hard-coded the database
// location, and running all the tests in parallel means everything is
// racing, and causes intermittent failures. A better soloution would be
// to have each test create its own database, but this is a sample...
[assembly: CollectionBehavior(DisableTestParallelization = true)]
