<#
.SYNOPSIS
Updates an existing person in the API.

.PARAMETER ApiUri
The URI at which the PeopleSearch API is operating. If this parameter is not
supplied, the environment variable PEOPLESEARCH_URI will be used. If the
parameter is not supplied and the environment variable is not set, an error
will be returned.

.PARAMETER Person
An object representing the updated person. The personId property must be set
on the object.

.PARAMETER PersonId
The ID of the person to be updated

.PARAMETER FirstName
The new first name for the person

.PARAMETER LastName
The new last name for the person

.PARAMETER Address
The new address for the person

.PARAMETER City
The new city for the person

.PARAMETER State
The new state for the person

.PARAMETER PostalCode
The new postal code for the person

.OUTPUTS
An object or set of objects, each representing a person returned from the API.
#>
function Set-PSApiPerson
{
    [CmdletBinding()]
    Param(
		[Parameter(Position=0)]
		[Uri]$ApiUri = $env:PEOPLESEARCH_URI,

		[Parameter(Position=1, Mandatory=$true, ParameterSetName='obj')]
		$Person,

		[Parameter(Position=1, ParameterSetName='byid')]
		[int]$PersonId,

		[Parameter(Position=2, ParameterSetName='byid')]
		[string]$FirstName,

		[Parameter(Position=3, ParameterSetName='byid')]
		[string]$LastName,

		[Parameter(Position=4, ParameterSetName='byid')]
		[string]$Address,

		[Parameter(Position=5, ParameterSetName='byid')]
		[string]$City,

		[Parameter(Position=6, ParameterSetName='byid')]
		[string]$State,

		[Parameter(Position=7, ParameterSetName='byid')]
		[string]$PostalCode
	)

	Process {
		if( [string]::IsNullOrWhiteSpace($ApiUri) ) {
			throw 'No API URI was provided.'
		}

		if( $PSCmdlet.ParameterSetName -eq 'byid' ) {
			if( $PSBoundParameters.ContainsKey('FirstName') -and $PSBoundParameters.ContainsKey('LastName') -and [string]::IsNullOrWhiteSpace($FirstName) -and [string]::IsNullOrWhiteSpace($LastName) ) {
				throw 'Either a first name or a last name must be provided.'
			}

			$Person = get-psapiperson -apiuri $ApiUri -personid $PersonId

			if( $PSBoundParameters.ContainsKey('FirstName') ) {
				$Person.firstName = $FirstName
			}

			if( $PSBoundParameters.ContainsKey('LastName') ) {
				$Person.lastName = $LastName
			}

			if( $PSBoundParameters.ContainsKey('Address') ) {
				$Person.address = $Address
			}

			if( $PSBoundParameters.ContainsKey('City') ) {
				$Person.city = $City
			}

			if( $PSBoundParameters.ContainsKey('State') ) {
				$Person.state = $State
			}

			if( $PSBoundParameters.ContainsKey('PostalCode') ) {
				$Person.postalCode = $PostalCode
			}
		} else {
			if( [string]::IsNullOrWhiteSpace($Person.firstName) -and [string]::IsNullOrWhiteSpace($person.LastName) ) {
				throw 'Either a first name or a last name must be provided.'
			}
		}

		$uri = new-object UriBuilder $ApiUri

		$uri.Path = "/person/$($Person.PersonId)"

		invoke-restmethod -Uri $uri.Uri -Method 'Put' -Body (convertto-json $Person) -ContentType 'application/json'
	}
}
