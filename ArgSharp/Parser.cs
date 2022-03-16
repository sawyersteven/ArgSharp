using System;
using System.Collections.Generic;
using System.Reflection;
using ArgSharp.Attributes;
using ArgSharp.Exceptions;

namespace ArgSharp
{
    internal class Command
    {
        public readonly object Obj;
        public readonly string Name;
        public List<ArgBase> Attributes = new List<ArgBase>();
        public Command Subcommand { get; set; }

        public Command(object obj, string name)
        {
            Obj = obj;
            Name = name;
        }
    }

    public class Parser
    {
        public bool ExitAfterPrint { get; set; } = true;
        public bool PrintHelpOnError { get; set; } = true;

        private List<string> inputArgs;
        private Command RootCommand { get; set; }
        private object rootObj;

        public void ParseInto(string[] args, object obj)
        {
            rootObj = obj;
            ParseIntoRootObj(args);
        }

        public T ParseIntoNew<T>(string[] args)
        {
            rootObj = Activator.CreateInstance<T>();
            ParseIntoRootObj(args);
            return (T)rootObj;
        }

        private void ParseIntoRootObj(string[] args)
        {
            this.inputArgs = new List<string>(args);
            RootCommand = new Command(rootObj, System.AppDomain.CurrentDomain.FriendlyName);

            ParseAttributes(RootCommand);
            if (inputArgs.Contains("--help"))
            {
                Print.Usage(RootCommand);
                if (ExitAfterPrint) Environment.Exit(0);
            }
            else if (inputArgs.Contains("--version"))
            {
                Print.Version();
                if (ExitAfterPrint) Environment.Exit(0);
            }
            else
            {
                try
                {
                    AssignValues(RootCommand);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (PrintHelpOnError) Print.Usage(RootCommand);
                    if (ExitAfterPrint) Environment.Exit(1);
                    throw;
                }
            }
            GC.Collect();
        }

        // Recursively assign values from argument attributes into properties
        // This will consume and remove values in inputArgs and node.Attributes
        private void AssignValues(Command node)
        {
            // // Consume flags and names
            foreach (ArgBase a in node.Attributes)
            {
                if (a is FlagArgumentAttribute)
                {
                    var attr = a as FlagArgumentAttribute;
                    attr.Prop.SetValue(node.Obj, ConsumeFlag(attr));
                    continue;
                }
                if (a is NamedArgumentAttribute)
                {
                    var attr = a as NamedArgumentAttribute;
                    string value = ConsumeName(attr);
                    if (value != null)
                    {
                        ConvertAndSet(node.Obj, attr.Prop, value);
                    }
                }
            }

            // Recurse through subcommands so only last subcommand gets positionals
            if (node.Subcommand != null)
            {
                inputArgs.Remove(node.Subcommand.Name);
                AssignValues(node.Subcommand);
            }

            // By the time we get to positionals all the known flags and names
            // will have been removed from inputArgs list.
            foreach (string s in inputArgs)
            {
                if (s.StartsWith('-'))
                {
                    throw new UnknownArgumentException(s);
                }
            }

            foreach (ArgBase a in node.Attributes)
            {
                if (!(a is PositionalArgumentAttribute)) continue;
                var attr = a as PositionalArgumentAttribute;
                ConvertAndSet(node.Obj, attr.Prop, ConsumePositional(attr));
            }

            if (inputArgs.Count > 0)
            {
                throw new UnknownArgumentException(inputArgs[0]);
            }
        }


        // Returns bool indicating if flag is present in inputArgs. Removes string from list
        private bool ConsumeFlag(FlagArgumentAttribute flagArg) => inputArgs.Remove(flagArg.Name);

        // Finds arg matching argument name, returning the following string
        // as the value. Will throw exception if arg has no value or if arg 
        // is required and is not present. Removes key and val from inputArgs
        private string ConsumeName(NamedArgumentAttribute namedArg)
        {
            for (int i = 0; i < inputArgs.Count; i++)
            {
                if (inputArgs[i] == namedArg.ShortName || inputArgs[i] == namedArg.LongName)
                {
                    if (i == inputArgs.Count - 1 || inputArgs[i + 1].StartsWith('-'))
                    {
                        throw new MissingValueException(namedArg.LongName);
                    }
                    string val = inputArgs[i + 1];
                    inputArgs.RemoveAt(i + 1);
                    inputArgs.RemoveAt(i);
                    return val;
                }
            }
            if (namedArg.Required)
            {
                throw new RequiredArgumentException(namedArg.LongName);
            }
            return null;
        }

        private string ConsumePositional(PositionalArgumentAttribute posArg)
        {
            if (inputArgs.Count == 0) throw new MissingValueException(posArg.Name);
            string v = inputArgs[0];
            inputArgs.RemoveAt(0);
            return v;
        }

        // Attempts to find key and convert type to value's type
        // Returns bool indicating if the key was found. Will throw exception
        // if arg cannot be converted into value's type
        public static bool TryGetName<TO>(string[] args, char shortName, string longName, out TO value)
        {
            string s = "-" + shortName;
            string l = "--" + longName;
            value = default(TO);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == s || args[i] == l)
                {
                    if (args.Length > i + 1 && !args[i + 1].StartsWith('-'))
                    {
                        string v = args[i + 1];
                        value = (TO)Convert.ChangeType(v, typeof(TO));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        // Collects attributes and props from node's obj into node.Attributes
        private void ParseAttributes(Command node, int depth = 0)
        {
            Type objType = node.Obj.GetType();

            // Gather attrs and props. This calls the ctor on every attr and
            // will allow the attr's validation to throw an exception now.
            List<ArgBase> attrs = new List<ArgBase>();
            var props = objType.GetProperties();
            foreach (var prop in objType.GetProperties())
            {
                ArgBase attr = prop.GetCustomAttribute<ArgBase>();
                if (attr == null) continue;
                attr.Prop = prop;
                attrs.Add(attr);
            }

            // Parent nodes get flags and names first
            foreach (var attr in attrs)
            {
                if (attr is NamedArgumentAttribute || attr is FlagArgumentAttribute)
                {
                    node.Attributes.Add(attr);
                }
            }

            if (depth >= inputArgs.Count) return;

            // Find subcommands and recursively parse into nodes
            foreach (var a in attrs)
            {
                if (!(a is SubcommandAttribute)) continue;
                var attr = a as SubcommandAttribute;
                node.Attributes.Add(attr);

                if (attr.Name != inputArgs[depth]) continue;

                object subCommandObj = Activator.CreateInstance(attr.Prop.PropertyType);
                attr.Prop.SetValue(node.Obj, subCommandObj);
                Command n = new Command(subCommandObj, attr.Name);
                node.Subcommand = n;
                ParseAttributes(n, depth + 1);
                return; // Since this is not the final node in the chain it doesn't get positionals
            }

            foreach (var attr in attrs)
            {
                if (attr is PositionalArgumentAttribute) node.Attributes.Add(attr);
            }
        }

        // Sets property value, attempting to convert the value type.
        public static void ConvertAndSet(object obj, PropertyInfo prop, string value)
        {
            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(obj, value);
                return;
            }
            try
            {
                prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
            }
            catch (Exception)
            {
                throw new IncompatibleValueException(value, prop.PropertyType.ToString());
            }
        }
    }
}