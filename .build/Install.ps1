. .\Common.ps1

git submodule -q update --init


$release  = $env:APPVEYOR_REPO_TAG -eq "true" -and $env:APPVEYOR_REPO_TAG_NAME;
if($release)
{
    git checkout $env:APPVEYOR_REPO_BRANCH -q
    choco install docfx -y
}

Exec { & dotnet restore $env:BUILD_SOLUTION }