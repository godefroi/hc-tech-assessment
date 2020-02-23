# just dot-source all the .ps1 files
get-childitem $psscriptroot -filter '*.ps1' | foreach { . $_.fullname }
