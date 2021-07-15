$version="0.7.0.124-beta";
for ($i = 0; $i -le $args.length; ++$i)
{
    if ("-Version".Equals($args[$i]))
    {
        if ($args.length -gt $i + 1)
        {
            $version = $args[++$i];
        }
    }
}

Write-Host "Building version " $version
nuget pack .\Intersect.Client.Framework\Intersect.Client.Framework.nuspec -OutputDirectory .\build\release\ -Version $version
nuget pack .\Intersect.Server.Framework\Intersect.Server.Framework.nuspec -OutputDirectory .\build\release\ -Version $version
