choco install docfx -y
Install-Module -Name PowerShellForGitHub

$env:APPVEYOR_REPO_BRANCH = @{ $true = "master"; $false = $env:APPVEYOR_REPO_BRANCH; }[[bool]$env:APPVEYOR_FORCED_BUILD]
git checkout $env:APPVEYOR_REPO_BRANCH -q

git submodule update --rebase --remote 2>&1
