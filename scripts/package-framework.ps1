$version = "0.7.0.125-beta";
$development = $false;
$copyTo = "";
for ($i = 0; $i -le $args.length; ++$i)
{
    if ("-Version".Equals($args[$i]))
    {
        if ($args.length -gt $i + 1)
        {
            $version = $args[++$i];
        }
    }

    if ("-d".Equals($args[$i]))
    {
        $development = $true;
    }

    if ("-o".Equals($args[$i])) {
        if ($args.length -gt $i + 1)
        {
            $copyTo = $args[++$i];
        }
    }
}

if ($development)
{
    $version = $version + "-development";
}

Write-Host "Building version " $version
nuget pack .\Intersect.Client.Framework\Intersect.Client.Framework.nuspec -OutputDirectory .\build\release\ -Version $version
nuget pack .\Intersect.Server.Framework\Intersect.Server.Framework.nuspec -OutputDirectory .\build\release\ -Version $version

if ($copyTo -ne "")
{
    cp .\build\release\AscensionGameDev.Intersect.Client.Framework.$version.nupkg $copyTo
    cp .\build\release\AscensionGameDev.Intersect.Server.Framework.$version.nupkg $copyTo
}
