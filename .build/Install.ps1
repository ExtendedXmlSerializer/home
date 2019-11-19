. $PSScriptRoot\Common.ps1

git submodule -q update --init

if($release)
{
    git checkout $env:APPVEYOR_REPO_BRANCH -q
    choco install docfx -y
}

Exec { & dotnet restore $env:BUILD_SOLUTION }