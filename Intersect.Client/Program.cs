using System.Reflection;
using Intersect.Framework.Reflection;

Console.WriteLine($"Starting {Assembly.GetExecutingAssembly().GetMetadataName()}...");

Intersect.Client.Core.Program.Main(Assembly.GetExecutingAssembly(), args);