if($documentation)
{
    git checkout $env:APPVEYOR_REPO_BRANCH -q
    choco install docfx -y
}

# git submodule update --rebase --remote
git submodule -q update --init

$blockRdp = $true; iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/appveyor/ci/master/scripts/enable-rdp.ps1'))