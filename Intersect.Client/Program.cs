using System.Reflection;
using Intersect.Framework.Reflection;

Console.WriteLine($"Starting {Assembly.GetExecutingAssembly().GetMetadataName()} in {Environment.CurrentDirectory}...");

Intersect.Client.Core.Program.Main(Assembly.GetExecutingAssembly(), args);