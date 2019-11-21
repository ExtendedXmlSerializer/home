Install-Module -Name PowerShellForGitHub

if($documentation)
{
    git checkout $env:APPVEYOR_REPO_BRANCH -q
    choco install docfx -y
}

git submodule -q update --init
git submodule update --rebase --remote