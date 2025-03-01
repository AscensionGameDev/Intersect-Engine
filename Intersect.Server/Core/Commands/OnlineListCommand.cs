﻿using Intersect.Network;
using Intersect.Server.Core.CommandParsing;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Core.Commands
{

    internal sealed partial class OnlineListCommand : ServerCommand
    {

        public OnlineListCommand() : base(Strings.Commands.OnlineList)
        {
        }

        protected override void HandleValue(ServerContext context, ParserResult result)
        {
            var bufferWidth = (Console.BufferWidth == 0 ? 100 : Console.BufferWidth) - 1;
            var columnWidths = new[]
            {
                37,
                -Strings.Commandoutput.ListAccount.ToString().Length,
                -Strings.Commandoutput.ListCharacter.ToString().Length,
                37,
            };
            var resizableColumns = columnWidths.Count(columnWidth => columnWidth < 1);
            var availableWidth = columnWidths.Aggregate(
                bufferWidth,
                (remainingWidth, columnWidth) => Math.Max(0, remainingWidth - Math.Max(0, columnWidth))
            ) - resizableColumns * 2;
            var resizableWidth = columnWidths.Aggregate(
                0,
                (totalWidth, columnWidth) => totalWidth + Math.Max(0, -columnWidth)
            );
            var remainingAvailableWidth = availableWidth;
            var resizedColumns = 0;
            for (var column = 0; column < columnWidths.Length; ++column)
            {
                var columnWidth = columnWidths[column];
                if (columnWidths[column] > 0)
                {
                    continue;
                }

                ++resizedColumns;
                var weight = Math.Max(0f, -columnWidth) / Math.Max(1f, resizableWidth);
                columnWidth = (int)(availableWidth * weight);
                remainingAvailableWidth -= columnWidth;
                if (resizedColumns >= resizableColumns)
                {
                    columnWidth += remainingAvailableWidth;
                }
                columnWidths[column] = columnWidth;
            }

            var formatLine = string.Join("| ", columnWidths.Select((width, column) => $"{{{column},{-width}}}"));

            var clients = Client.Instances.ToArray();

            HashSet<IConnection> seenConnections = new();

            if (clients.Length > 0)
            {
                Console.WriteLine(
                    Strings.Commandoutput.ActiveConnections.ToString(context.Network.ConnectionCount)
                );

                Console.WriteLine(
                    formatLine,
                    Strings.Commandoutput.ListId,
                    Strings.Commandoutput.ListAccount,
                    Strings.Commandoutput.ListCharacter,
                    "Connection ID"
                );

                Console.WriteLine(new string('-', bufferWidth));

                foreach (var client in Client.Instances.ToArray())
                {
                    if (client == default)
                    {
                        continue;
                    }

                    seenConnections.Add(client.Connection);

                    var name = client.Entity?.Name ?? string.Empty;
                    Console.WriteLine(formatLine, client.Id, client.Name, name, client.Connection?.Guid ?? default);
                }
            }
            else
            {
                Console.WriteLine(Strings.Commandoutput.NoClientsConnected);
            }

            var strayConnections =
                context.Network.Connections.ToArray().Where(c => !seenConnections.Contains(c)).ToArray();

            // ReSharper disable once InvertIf
            if (strayConnections.Length > 0)
            {
                Console.WriteLine(Strings.Commandoutput.StrayConnections.ToString(strayConnections.Length));

                foreach (var connection in strayConnections)
                {
                    Console.WriteLine($"{connection.Guid} | {connection.Ip}:{connection.Port}");
                }
            }
        }

    }

}
