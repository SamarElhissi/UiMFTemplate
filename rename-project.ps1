# Set the name of your new app
$myappname = 'UiMFTemplate';

# Don't change the below code
$toreplace = 'UiMF.Template';

Get-ChildItem -Filter "*$toreplace.*" -Recurse `
    | ? { $_.FullName -notmatch 'node_modules' } `
    | Rename-Item -NewName { 
        $_.name.Replace("$toreplace.", "$myappname.")
    }

$files = Get-ChildItem `
    -Recurse `
    -Include *.cs, *.md, *.txt, *.html, *.js, *.sql, *.csproj, *.sqlproj, *.json, *.gitignore, *.css, *.cshtml, *.sln `
    | ? { $_.FullName -notmatch 'node_modules' }
foreach ($file in $files)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { $_.Replace($toreplace, $myappname) } |
    Set-Content $file.PSPath
}