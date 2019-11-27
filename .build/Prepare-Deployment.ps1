choco install docfx -y
Install-Module -Name PowerShellForGitHub

$env:APPVEYOR_REPO_BRANCH = "master";
git checkout $env:APPVEYOR_REPO_BRANCH -q

git submodule update --rebase --remote

Set-AppveyorBuildVariable "DEPLOY_RELEASE_ENABLED" $true