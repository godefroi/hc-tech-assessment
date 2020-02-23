# hc-tech-assessment
This repository holds my submission for the Health Catalyst tech assessment. It consists of a Visual Studio solution with two projects, as well as a PowerShell module.

## PeopleSearch
The PeopleSearch project is an ASP.NET Core 3.1 web api project with a single controller. This controller provides several routes that allow a called to search for, retrieve, create, and update people in the database. The database is a SQLite database with a single table, `People`, that contains sample data for the purpose of testing the application. Each API call to the service is delayed by 65ms using an `ActionFilter` in order to simulate the average amount of latency on AT&T's network according to the BroadbandNow 2020 survey. This delay is configurable in the `appsettings.json` file, and if the configuration value is not present, the application defaults to no delay.


### Sample Data
The first time the PeopleSearch application starts, the database is created and seeded with data. The sample data comes from various sources, and is designed to be somewhat realistic. Some of this data is retrieved from the internet at startup time (in order to demonstrate the techniques), and some is stored as a compressed resource in the application assembly (for the same reason). __Note that occiasionally, there have been issues retrieving some of the online data (i.e., 503 results from HTTP calls). This has never persisted, and a second try moments later generally succeeds.__
  * First and Last names (both male and female) come from the 1990 census surname frequency list. An effort is made to reproduce the frequency of names found in the census data, and males and females are represented in equal numbers.
  * Street addresses are automatically generated using a list of streets existing in San Francisco. As there are an extremely large number of streets in the US, this list was deemed to be sufficiently large without being overwhelming.
  * Cities, states, and zip codes are generated in a geographically-accurate way using a freely-available database retrieved from zip-codes.com. This database was retrieved as a CSV file, and then modified to reduce its size using the following PowerShell commands:
      ```powershell
      import-csv zip-codes-database-FREE.csv |
          foreach { [pscustomobject]@{Code = $_.ZipCode; City = $_.City; State = $_.State} } |
          convertto-csv -notypeinformation |
          out-file zip-codes-limited.csv
      ```
      Once the unneeded data was removed rom the file, it was compressed into a binary blob using the following PowerShell commands:
      ```powershell
      $fs = new-object system.io.filestream ("zip_code_data.bin", [system.io.filemode]::create)
      $ds = new-object system.io.compression.deflatestream ($fs, [system.io.compression.compressionlevel]::optimal)
      $buf = [system.io.file]::readallbytes('zip-codes-limited.csv')
      $ds.write($buf, 0, $buf.length)
      $ds.close()
      ```
      The file `zip_code_data.bin` was then embedded into the assembly as a resource.


### Database
The database is implemented using Entity Framework Core code-first, with the SQLite backend. This backend was chosen in order to ease the burden on anyone wishing to build, run, and test the application. The SQLite engine is quite performant, and has many powerful features, but it is not usually suitable for a high-traffic mission-critical application requiring high levels of concurrency and online backups. There is a single table in the database, `People`, which contains a fully-denormalized row for each person. Again, this model was chosen for simplicity, and should not be taken as a recommendation for a design in a production system.


### Interacting with the API
The API consists of four methods, all in the `PersonController.cs` file. Two of these methods are used to retrieve data from the database, one is used to create new entries, and one is used to update existing entries.

#### Searching the API for persons
A `GET` request to the controller at `http://localhost:5000/person` can be used to search for matching people in the database. Three query parameters are provided to control what is returned:
  * **find**: This parameter should contain a search string that is tested against first and last names. This parameter supports an asterisk (`*`) as a wildcard character. If this parameter is not provided, no results will be returned.
  * **skip**: This parameter specifies how many of the matching results to skip before returning any. This parameter, along with the **take** parameter, can be used to implement paging in a versatile way. The default number of results to skip is 0.
  * **take**: This parameter specifies the maximum number of results to return. This parameter, along with the **skip** parameter, can be used to implement paging in a versatile way. The default number of results to return is 10.

#### Retrieving a specific person by ID
A `GET` request to the controller at `http://localhost:5000/person/{id}` can be used to return a specific person from the database. The `{id}` part of the URI should be replaced with the ID of the person who is to be retrieved.

#### Adding a new person to the database
A `POST` request to the controller at `http://localhost:5000/person` can be used to add a new person to the database. The body of the request should contain a JSON-encoded object such as this:
```json
{
  "personId": 0,
  "firstName": "SHAUN",
  "lastName": "NEALY",
  "address": "9191 SERRANO DR",
  "city": "ANYWHERE",
  "state": "CA",
  "postalCode": "94953"
}
```
Note that when creating a person, the `personId` MUST be zero. An existing person cannot be updated with this method, and a person CANNOT be created with an arbitrary `personId`. Note that this method is NOT idempotent (as implied by the `POST`), and submitting the same information multiple times will result in multiple duplicate entries.

#### Updating an existing person in the database
A `PUT` request to the controller at `http://localhost:5000/person/{id}` can be used to update an existing entry in the database. The `{id}` section of the URI path must be replaced with the ID of the person which is to be updated, and the body of the request should be a JSON-enconded object such as this:
```json
{
  "personId": 0,
  "firstName": "SHAUN",
  "lastName": "NEALY",
  "address": "9191 SERRANO DR",
  "city": "ANYWHERE",
  "state": "CA",
  "postalCode": "94953"
}
```
Note that the `personId` in the request body and the `{id}` provided on the URI must match; if they do not, an error will be returned. Also, the entry to be updated must exist; an entry cannot be created this way.




## PeopleSearchModule
This PowerShell module is provided as a simple way to interact with the service. The module can be loaded into the current PowerShell session using the following command:
```powershell
import-module -name .\PeopleSearchModule\PeopleSearch.psm1
```
Once the module is loaded, general help can be accessed using this command:
```powershell
help about_peoplesearch
```
A list of cmdlets provided by the module can be retrieved using this command:
```powershell
get-command -module peoplesearch
```
Help for individual cmdlets is available as well:
```powershell
help Get-PSApiPerson
```




## PeopleSearchTests
A test suite is provided that tests various components of the application. This suite is implemented using xUnit, and all provided tests currently pass.
