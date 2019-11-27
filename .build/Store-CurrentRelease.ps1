$parts = $env:APPVEYOR_REPO_NAME -Split '/'
$view = Get-GitHubRelease -OwnerName $parts[0] -RepositoryName $parts[1] -AccessToken $env:API_GITHUB_TOKEN | Where-Object {$_.tag_name -eq $env:APPVEYOR_REPO_TAG_NAME -and $_.assets.Length -eq 0} | Select-Object url, name, prerelease, body
		   
$enabled = !($view -eq $null)
Write-Host "Deploy GitHub Release: $enabled"        

if ($enabled)
{
	Set-AppveyorBuildVariable "DEPLOY_RELEASE_URL" $view.url
	Set-AppveyorBuildVariable "DEPLOY_RELEASE_NAME" $view.name
	Set-AppveyorBuildVariable "DEPLOY_RELEASE_IS_PRERELEASE" $view.prerelease
	Set-AppveyorBuildVariable "DEPLOY_RELEASE_DESCRIPTION" $view.body                
}