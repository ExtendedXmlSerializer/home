<#
.SYNOPSIS
    You can add this to you build script to ensure that psbuild is available before calling
    Invoke-MSBuild. If psbuild is not available locally it will be downloaded automatically.
#>
function EnsurePsbuildInstalled{
    [cmdletbinding()]
    param(
        [string]$psbuildInstallUri = 'https://raw.githubusercontent.com/ligershark/psbuild/master/src/GetPSBuild.ps1'
    )
    process{
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            'Installing psbuild from [{0}]' -f $psbuildInstallUri | Write-Verbose
            (new-object Net.WebClient).DownloadString($psbuildInstallUri) | iex
        }
        else{
            'psbuild already loaded, skipping download' | Write-Verbose
        }

        # make sure it's loaded and throw if not
        if(-not (Get-Command "Invoke-MsBuild" -errorAction SilentlyContinue)){
            throw ('Unable to install/load psbuild from [{0}]' -f $psbuildInstallUri)
        }
    }
}

# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

EnsurePsbuildInstalled

exec { & dotnet restore .\ExtendedXmlSerializer.sln }
exec { & dotnet build .\ExtendedXmlSerializer.sln -c Release }

# TODO: Remove once AppVeyor properly detects .NET Core test assemblies: https://help.appveyor.com/discussions/problems/25375-automatically-discovered-tests-misses-net-core-testing-assembly
exec { & dotnet test .\ExtendedXmlSerializer.sln -c Release -f netcoreapp2.1 }

$release = $env:APPVEYOR_REPO_TAG -eq "true" -and $env:APPVEYOR_REPO_TAG_NAME;

$suffix = @{ $true = ""; $false = "preview" }[$release];
exec { & dotnet pack .\src\ExtendedXmlSerializer\ExtendedXmlSerializer.csproj -c Release -o .\..\..\artifacts --version-suffix=$suffix }

if ($release) {
    $headers = @{
        "Authorization" = "Bearer $env:APPVEYOR_TOKEN"
        "Content-type" = "application/json"
        "Accept" = "application/json"
    }
    $build = @{
        nextBuildNumber = 1
    }
    $json = $build | ConvertTo-Json
    Invoke-RestMethod -Method Put "https://ci.appveyor.com/api/projects/$env:APPVEYOR_ACCOUNT_NAME/$env:APPVEYOR_PROJECT_SLUG/settings/build-number" -Body $json -Headers $headers
}