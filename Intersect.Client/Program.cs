using System.Reflection;
using Intersect.Framework.Reflection;

var executingAssembly = Assembly.GetExecutingAssembly();
Console.WriteLine(
    $"Starting {executingAssembly.GetMetadataName()} in {Environment.CurrentDirectory}...\n\t{string.Join(' ', args)}"
);

Intersect.Client.Core.Program.Main(Assembly.GetExecutingAssembly(), args);