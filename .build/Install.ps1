git submodule update --rebase --remote
# git submodule -q update --init

if($documentation)
{
    git checkout $env:APPVEYOR_REPO_BRANCH -q
    choco install docfx -y
}