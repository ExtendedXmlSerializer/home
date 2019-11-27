$key = ("-----BEGIN RSA PRIVATE KEY-----`n" +
		$env:DEPLOY_KEY.Replace(' ', "`n") +
		"`n-----END RSA PRIVATE KEY-----`n")
Set-Content "$Home\.ssh\id_rsa" $key
git clone "git@github.com:ExtendedXmlSerializer/documentation.git" -b gh-pages .wwwroot -q

Copy-Item .wwwroot/.git documentation/.wwwroot -recurse
CD documentation/.wwwroot
git config user.email $env:DEPLOY_EMAIL
git config user.name $env:DEPLOY_USER
git config core.safecrlf false
Exec { git add -A 2>&1 }
Exec { git commit -m "AppVeyor Continuous Deployment Documentation Update v$($env:APPVEYOR_BUILD_VERSION)" -q }
Exec { git push origin gh-pages -q }