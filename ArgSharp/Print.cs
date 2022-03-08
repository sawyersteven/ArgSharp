using System;
using System.Collections.Generic;
using System.Reflection;
using ArgSharp.Attributes;

namespace ArgSharp
{
    using HelpInfo = ValueTuple<string, string, string>;
    internal static class Print
    {
        public static void Usage(Command root)
        {
            new UsagePrinter(root);
        }

        private class UsagePrinter
        {
            private const int COL_PADDING = 4;
            private const int INDENT = 2;
            private List<HelpInfo> requiredArgs = new List<HelpInfo>();
            private List<HelpInfo> optionalArgs = new List<HelpInfo>();
            private List<HelpInfo> subCommands = new List<HelpInfo>();
            public UsagePrinter(Command root)
            {

                CollectUsageData(root);
                Version();
                string commandChain = CommandChain(root);
                Console.WriteLine($"Usage: {commandChain} {BuildArgumentString(root)}");
                if (subCommands.Count > 0 && subCommands[^1].Item3 != "") Console.WriteLine(subCommands[^1].Item3);

                if (optionalArgs.Count > 0)
                {
                    Console.WriteLine("Optional Arguments");
                    Console.WriteLine(BuildArgumentTable(optionalArgs));
                }

                if (requiredArgs.Count > 0)
                {
                    Console.WriteLine("Required Arguments");
                    Console.WriteLine(BuildArgumentTable(requiredArgs));
                }

                if (subCommands.Count > 0)
                {
                    Console.WriteLine("Additional Commands");
                    Console.WriteLine(BuildArgumentTable(subCommands));
                    Console.WriteLine($"For more information run: {commandChain} [command] --help");
                }
            }

            // Always obey the chain o' command
            private string CommandChain(Command root)
            {
                List<string> commandChain = new List<string>();
                var current = root;
                while (current != null)
                {
                    commandChain.Add(current.Name);
                    current = current.Subcommand;
                }
                return string.Join(' ', commandChain);
            }

            private void CollectUsageData(Command command)
            {
                foreach (var a in command.Attributes)
                {
                    switch (a)
                    {
                        case FlagArgumentAttribute flagAttr:
                            optionalArgs.Add(flagAttr.Usage());
                            break;
                        case NamedArgumentAttribute namedAttr:
                            if (namedAttr.Required) requiredArgs.Insert(0, namedAttr.Usage());
                            else optionalArgs.Insert(0, namedAttr.Usage());
                            break;
                        case PositionalArgumentAttribute posAttr:
                            if (command.Subcommand == null) requiredArgs.Add(posAttr.Usage());
                            break;
                        case SubcommandAttribute subAttr:
                            if (command.Subcommand == null) subCommands.Add(subAttr.Usage());
                            break;
                    }
                }
                if (command.Subcommand != null) CollectUsageData(command.Subcommand);
            }

            private string BuildArgumentString(Command root)
            {
                List<string> argString = new List<string>();

                foreach (var n in optionalArgs)
                {
                    if (n.Item1.Contains('|'))
                    {
                        string[] parts = n.Item1.Split("|--");
                        argString.Add($"[{parts[0]} {parts[1]}]");
                    }
                    else
                    {
                        argString.Add($"[{n.Item1}]");
                    }
                }

                foreach (var n in requiredArgs)
                {
                    if (n.Item1.Contains('|'))
                    {
                        string[] parts = n.Item1.Split("|--");
                        argString.Add($"{parts[0]} {parts[1]}");
                    }
                    else
                    {
                        argString.Add(n.Item1);
                    }
                }
                return string.Join(' ', argString);
            }

            private string BuildArgumentTable(List<(string, string, string)> helps)
            {
                int nameWidth = 0;
                int typeWidth = 0;
                string indent = new string(' ', INDENT);

                foreach (var help in helps)
                {
                    nameWidth = Math.Max(nameWidth, help.Item1.Length);
                    typeWidth = Math.Max(typeWidth, help.Item2.Length);
                }

                nameWidth += COL_PADDING;
                typeWidth += COL_PADDING;

                string[] lines = new string[helps.Count];
                for (int i = 0; i < helps.Count; i++)
                {
                    lines[i] = $"{indent}{helps[i].Item1.PadRight(nameWidth)}{helps[i].Item2.PadRight(typeWidth)}{helps[i].Item3}";
                }
                return string.Join('\n', lines) + '\n';
            }
        }

        public static void Version()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"{System.AppDomain.CurrentDomain.FriendlyName} ({v.Major}.{v.Minor}.{v.Build})");
        }
    }
}