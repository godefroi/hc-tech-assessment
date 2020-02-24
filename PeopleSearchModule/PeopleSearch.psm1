if( $PSVersionTable.PSEdition -eq 'Desktop' ) {
	add-type -AssemblyName 'System.Web'
}

# just dot-source all the .ps1 files
get-childitem $psscriptroot -filter '*.ps1' | foreach { . $_.fullname }
