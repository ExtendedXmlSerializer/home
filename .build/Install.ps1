git submodule -q update --init

if($release)
{
    git checkout $env:APPVEYOR_REPO_BRANCH -q
    choco install docfx -y
}