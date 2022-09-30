using System;
using System.Reflection;
using ArgSharp.Exceptions;

namespace ArgSharp.Attributes
{
    internal static class Reserved
    {
        public static readonly string[] Names = new string[] { "help", "version" };
    }

    // Creates a subcommand argument eg `app.exe dosomething`
    // A subcommand that contains positional arguments will consume all
    // positional arguments and will throw an exception if more positional
    // arguments than neccesary for the subcommand are passed
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class SubcommandAttribute : ArgBase
    {
        public string Name { get; set; }
        public SubcommandAttribute(string name, string helpText = "")
        {
            if (name.Contains(" "))
            {
                throw new InvalidNameException($"Subcommand names may not contain whitespace [{Name}]");
            }
            Name = name;
            HelpText = helpText;
        }

        public override (string, string, string) Usage() => (Name, "", HelpText);
    }


    // Creates a simple flag argument eg `app.exe --verbose`
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class FlagArgumentAttribute : ArgBase
    {
        private PropertyInfo _Prop;
        public override PropertyInfo Prop
        {
            get => _Prop;
            set
            {
                if (value.PropertyType != typeof(bool))
                {
                    throw new InvalidOperationException($"Flags cannot be applied to {value.PropertyType} properties");
                }
                _Prop = value;
            }
        }
#nullable enable
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
#nullable restore

        public FlagArgumentAttribute(string longName, string helpText = "")
        {
            if (Array.IndexOf(Reserved.Names, longName) != -1)
            {
                throw new InvalidNameException($"Arguments cannot use reserved names [{string.Join(',', Reserved.Names)}]");
            }
            ShortName = null;
            LongName = "--" + longName;
            HelpText = helpText;
        }

        public FlagArgumentAttribute(char shortName, string helpText = "")
        {
            ShortName = "-" + shortName;
            LongName = null;
            HelpText = helpText;
        }

        public FlagArgumentAttribute(char shortName, string longName, string helpText = "")
        {
            if (Array.IndexOf(Reserved.Names, longName) != -1)
            {
                throw new InvalidNameException($"Arguments cannot use reserved names [{string.Join(',', Reserved.Names)}]");
            }
            ShortName = "-" + shortName;
            LongName = "--" + longName;
            HelpText = helpText;
        }

        public override (string, string, string) Usage()
        {
            if (ShortName == null)
            {
                return ($"{LongName}", "", HelpText);
            }
            else if (LongName == null)
            {
                return ($"{ShortName}", "", HelpText);
            }
            else
            {
                return ($"{ShortName}|{LongName}", "", HelpText);
            }
        }
    }

    // Creates a named argument eg `app.exe --outfile /tmp/out.txt`
    // Named argument require both a short and long name
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class NamedArgumentAttribute : ArgBase
    {
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public bool Required { get; set; }

        public NamedArgumentAttribute(char shortName, string longName, bool required = false, string helpText = "")
        {
            if (Array.IndexOf(Reserved.Names, longName) != -1)
            {
                throw new InvalidNameException($"Arguments cannot use reserved names [{string.Join(',', Reserved.Names)}]");
            }
            ShortName = "-" + shortName;
            LongName = "--" + longName;
            Required = required;
            HelpText = helpText;
        }

        public override (string, string, string) Usage() => ($"{ShortName}|{LongName}", Prop.PropertyType.ToString().Split('.')[^1], HelpText);
    }

    // Creates a positional argument eg `app.exe user_data.csv`
    // Positional argument are parsed in order of their delcaration and
    // must be passed at the end of the argument list.
    // Note: All positional argument are required.
    //
    // for example:
    //   [PositionalArgument()]
    //   public string DatabaseFile { get; set; }
    //
    //   [PositionalArgument()]
    //   public string InputData { get; set; }
    //
    // `app.exe --config conf.txt --verbose databasefilevalue inputdatavalue`
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class PositionalArgumentAttribute : ArgBase
    {
        public string Name { get; set; }
        public PositionalArgumentAttribute(string name, string helpText = "")
        {
            Name = name;
            HelpText = helpText;
        }

        public override (string, string, string) Usage() => (Name, Prop.PropertyType.ToString().Split('.')[^1], HelpText);
    }
}

public abstract class ArgBase : Attribute
{
    public string HelpText { get; set; }
    public virtual PropertyInfo Prop { get; set; }
    //                 name,   type, helptext
    public abstract (string, string, string) Usage();
}