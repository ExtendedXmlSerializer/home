choco install docfx -y
Install-Module -Name PowerShellForGitHub

$branch = @{ $true = "master"; $false = $env:APPVEYOR_REPO_BRANCH; }[$env:APPVEYOR_FORCED_BUILD]
git checkout $branch -q

git submodule update --rebase --remote