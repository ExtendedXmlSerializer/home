. .\Common.ps1

Get-Variable -Name release

if($release)
{
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

    # $key = ("-----BEGIN RSA PRIVATE KEY-----`n" +
    #        $env:DOCUMENTATION_KEY.Replace(' ', "`n") +
    #        "`n-----END RSA PRIVATE KEY-----`n")
    # Set-Content $Home\.ssh\id_rsa $key
    git clone https://github.com/ExtendedXmlSerializer/Documentation.git -b gh-pages .wwwroot -q
    Copy-Item .wwwroot/.git content/.wwwroot -recurse
    CD content/.wwwroot
    git config credential.helper store
    Add-Content "$HOME\.git-credentials" "https://$($env:DOCUMENTATION_TOKEN):x-oauth-basic@github.com`n"
    git config user.email $env:DOCUMENTATION_EMAIL
    git config user.name $env:DOCUMENTATION_USER
    git config core.safecrlf false
    git add -A 2>&1
    git commit -m "AppVeyor Continuous Deployment Documentation Update v$($env:APPVEYOR_BUILD_VERSION)" -q
    git push origin gh-pages -q
}