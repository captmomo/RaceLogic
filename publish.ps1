#1.0.6
$packages = @("maxbl4.RaceLogic")

function Main()
{
    $version = GetNextVersion

    dotnet test
    if (-not $?) { exit $? }

    foreach ($p in $packages){
        Pack $p $version
    }

    UpdateVersion $version
}

function Pack($name, $version)
{    
    dotnet pack -c Release /p:Version=$version .\$name\$name.csproj
    if (-not $?) { exit; }
    nuget push -Source NugetLocal .\$name\bin\Release\$name.$version.nupkg
    if (-not $?) { exit; }
    nuget push .\$name\bin\Release\$name.$version.nupkg
    if (-not $?) { exit; }
}

function GetNextVersion()
{
    $lines = Get-Content $MyInvocation.ScriptName
    $version = [System.Version]::Parse($lines[0].Substring(1))
    return "$($version.Major).$($version.Minor).$($version.Build + 1)"
}

function UpdateVersion($version)
{
    $lines = Get-Content $MyInvocation.ScriptName
    $lines[0] = "#$version"
    $lines > $MyInvocation.ScriptName
}

Main
