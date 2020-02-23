<#
.SYNOPSIS
Retrieve a person or set of persons from the API.

.PARAMETER ApiUri
The URI at which the PeopleSearch API is operating. If this parameter is not
supplied, the environment variable PEOPLESEARCH_URI will be used. If the
parameter is not supplied and the environment variable is not set, an error
will be returned.

.PARAMETER PersonId
The PersonId of the person to retrieve.

.PARAMETER Filter
The filter to be applied to the first and last names of the persons returned.
Wildcard searches are allowed, using the * character.

.PARAMETER Skip
The number of persons to skip in the result set.

.PARAMETER Take
The maximum number of persons to return.

.OUTPUTS
An object or set of objects, each representing a person returned from the API.
#>
function Get-PSApiPerson
{
    [CmdletBinding()]
    Param(
		[Parameter(Position=0, Mandatory=$false)]
		[Uri]$ApiUri = $env:PEOPLESEARCH_URI,

		[Parameter(Position=1, Mandatory=$true, ParameterSetName='byid')]
		[int]$PersonId,

		[Parameter(Position=1, Mandatory=$true, ParameterSetName='filter')]
		[string]$Filter,

		[Parameter(Position=2, Mandatory=$false, ParameterSetName='filter')]
		[int]$Skip,

		[Parameter(Position=3, Mandatory=$false, ParameterSetName='filter')]
		[int]$Take
	)

	Process {
		if( [string]::IsNullOrWhiteSpace($ApiUri) ) {
			throw 'No API URI was provided.'
		}

		$uri = new-object UriBuilder $ApiUri

		if( $PSCmdlet.ParameterSetName -eq 'filter' ) {
			$qry = [system.web.httputility]::ParseQueryString([string]::Empty)

			$qry.Add("find", $Filter)
	
			if( $PSBoundParameters.ContainsKey('Skip') ) {
				$qry.Add('skip', $Skip)
			}
	
			if( $PSBoundParameters.ContainsKey('Take') ) {
				$qry.Add('take', $Take)
			}
	

			$uri.Path  = '/person'
			$uri.Query = $qry.ToString()
	
			invoke-restmethod -Uri $uri.Uri -Method 'Get'	
		} else {
			$uri.Path = "/person/$PersonId"

			invoke-restmethod -Uri $uri.Uri -Method 'Get'	
		}
	}
}
