<#
.SYNOPSIS
Creates a new person in the API.

.PARAMETER ApiUri
The URI at which the PeopleSearch API is operating. If this parameter is not
supplied, the environment variable PEOPLESEARCH_URI will be used. If the
parameter is not supplied and the environment variable is not set, an error
will be returned.

.PARAMETER Person
An object representing the person to be added.

.PARAMETER FirstName
The first name of the new person

.PARAMETER LastName
The last name of the new person

.PARAMETER Address
The address of the new person

.PARAMETER City
The city of the new person

.PARAMETER State
The state of the new person

.PARAMETER PostalCode
The postal code of the new person

.OUTPUTS
An object or set of objects, each representing a person returned from the API.
#>
function New-PSApiPerson
{
    [CmdletBinding()]
    Param(
		[Parameter(Position=0)]
		[Uri]$ApiUri = $env:PEOPLESEARCH_URI,

		[Parameter(Position=1, Mandatory=$true, ParameterSetName='obj')]
		$Person,

		[Parameter(Position=1, ParameterSetName='props')]
		[string]$FirstName,

		[Parameter(Position=2, ParameterSetName='props')]
		[string]$LastName,

		[Parameter(Position=3, ParameterSetName='props')]
		[string]$Address,

		[Parameter(Position=4, ParameterSetName='props')]
		[string]$City,

		[Parameter(Position=5, ParameterSetName='props')]
		[string]$State,

		[Parameter(Position=6, ParameterSetName='props')]
		[string]$PostalCode
	)

	Process {
		if( [string]::IsNullOrWhiteSpace($ApiUri) ) {
			throw 'No API URI was provided.'
		}

		if( $PSCmdlet.ParameterSetName -eq 'props' ) {
			if( [string]::IsNullOrWhiteSpace($FirstName) -and [string]::IsNullOrWhiteSpace($LastName) ) {
				throw 'Either a first name or a last name must be provided.'
			}

			$Person = [PSCustomObject]@{
				firstName  = $FirstName
				lastName   = $LastName
				address    = $Address
				city       = $City
				state      = $State
				postalCode = $PostalCode
			}
		} else {
			if( [string]::IsNullOrWhiteSpace($Person.firstName) -and [string]::IsNullOrWhiteSpace($person.LastName) ) {
				throw 'Either a first name or a last name must be provided.'
			}
		}

		$uri = new-object UriBuilder $ApiUri

		$uri.Path = '/person'

		invoke-restmethod -Uri $uri.Uri -Method 'Post' -Body (convertto-json $Person) -ContentType 'application/json'
	}
}
