using ArgSharp.Attributes;

/*
A collection of classes containing every C# primitive value as params
*/

namespace ArgSharp.Tests
{
    class Empty { }

    class NamedPrimitives
    {
        [NamedArgumentAttribute('b', "byte", required: true)]
        public byte byteProp { get; set; }

        [NamedArgumentAttribute('s', "short")]
        public short shortProp { get; set; }

        [NamedArgumentAttribute('t', "ushort")]
        public ushort ushortProp { get; set; }

        [NamedArgumentAttribute('i', "int")]
        public int intProp { get; set; }

        [NamedArgumentAttribute('j', "uint")]
        public uint uintProp { get; set; }

        [NamedArgumentAttribute('l', "long")]
        public long longProp { get; set; }

        [NamedArgumentAttribute('o', "ulong")]
        public ulong ulongProp { get; set; }

        [NamedArgumentAttribute('c', "char")]
        public char charProp { get; set; }

        [NamedArgumentAttribute('f', "float")]
        public float floatProp { get; set; }

        [NamedArgumentAttribute('d', "double")]
        public double doubleProp { get; set; }

        [NamedArgumentAttribute('m', "decimal")]
        public decimal decimalProp { get; set; }

        [NamedArgumentAttribute('g', "string", false, "")]
        public string stringProp { get; set; }
    }

    class PositionalPrimitives
    {
        [PositionalArgument("byte")]
        public byte byteProp { get; set; }

        [PositionalArgument("short")]
        public short shortProp { get; set; }

        [PositionalArgument("ushort")]
        public ushort ushortProp { get; set; }

        [PositionalArgument("int")]
        public int intProp { get; set; }

        [PositionalArgument("uint")]
        public uint uintProp { get; set; }

        [PositionalArgument("long")]
        public long longProp { get; set; }

        [PositionalArgument("ulong")]
        public ulong ulongProp { get; set; }

        [PositionalArgument("char")]
        public char charProp { get; set; }

        [PositionalArgument("float")]
        public float floatProp { get; set; }

        [PositionalArgument("double")]
        public double doubleProp { get; set; }

        [PositionalArgument("decimal")]
        public decimal decimalProp { get; set; }

        [PositionalArgument("string")]
        public string stringProp { get; set; }
    }

    class FlagBooleans
    {
        [FlagArgument("False1")]
        public bool False1Prop { get; set; }

        [FlagArgument("True1")]
        public bool True1Prop { get; set; }

        [FlagArgument("True2")]
        public bool True2Prop { get; set; }

        [FlagArgument("False2")]
        public bool False2Prop { get; set; }

        [FlagArgument('t')]
        public bool TrueCharProp { get; set; }

        [FlagArgument('f')]
        public bool FalseCharProp { get; set; }

        [FlagArgument('x', "isX", "")]
        public bool xProp { get; set; }

        [FlagArgument('y', "isY", "")]
        public bool yProp { get; set; }
    }

    class DefaultValues
    {
        [NamedArgumentAttribute('d', "defaultVal456")]
        public int Def { get; set; } = 456;
    }

    #region exceptions
    class ReservedHelpFlag
    {
        [FlagArgument("help")] public bool Help { get; set; }
    }

    class ReservedVersionFlag
    {
        [FlagArgument("version")] public bool Version { get; set; }
    }

    class ReservedHelpName
    {
        [NamedArgument('h', "help")] public string Help { get; set; }
    }

    class ReservedVersionName
    {
        [NamedArgument('v', "version")] public string Version { get; set; }
    }

    class WhiteSpaceSubcommand
    {
        [Subcommand("Mock Subcommand")] public object MockSubcommand { get; set; }
    }

    class FlagOnNonBool
    {
        [FlagArgument("NotAFlag")] public float NotABool { get; set; }
    }
    #endregion

    #region Subcommands
    class SubcommandContainer
    {
        [Subcommand("withNamedSub")]
        public SubcommandWithNamed subcomWithName { get; set; }
        [Subcommand("withFlagsSub")]
        public SubcommandWithFlags subcomWithFlags { get; set; }
        [Subcommand("withPositionalsSub")]
        public SubcommandWithPositionals subcomWithPositionals { get; set; }

        [NamedArgument('n', "namedParam")]
        public string namedParam { get; set; }

        [FlagArgument("flagParam")]
        public bool flagParam { get; set; }

        [PositionalArgument("positional")]
        public string positionalParam { get; set; }
    }

    class SubcommandWithNamed
    {
        [NamedArgumentAttribute('f', "float")]
        public float floatProp { get; set; }

        [Subcommand("withFlagsSub")]
        public SubcommandWithFlags subcomWithFlags { get; set; }
    }

    class SubcommandWithFlags
    {
        [FlagArgument("on")]
        public bool shouldBeOn { get; set; }
        [FlagArgument("off")]
        public bool shouldBeOff { get; set; }
    }

    class SubcommandWithPositionals
    {
        [PositionalArgument("first")]
        public string first { get; set; }
        [PositionalArgument("second")]
        public int second { get; set; }
        [PositionalArgument("third")]
        public char third { get; set; }
    }

    class NestedSubcommandContainer
    {
        [Subcommand("nestedUnusedSub")]
        public NamedPrimitives NestedUnused { get; set; }

        [Subcommand("nestedSubChild")]
        public ChildCommand NestedChild { get; set; }
    }

    class ChildCommand
    {
        [Subcommand("nestedUnusedChildSub")]
        public PositionalPrimitives NestedEmpty { get; set; }

        [Subcommand("nestedSubGrandchild")]
        public NestedSubcommandGrandchild NestedGrandChild { get; set; }
    }

    class NestedSubcommandGrandchild
    {
        [FlagArgument("grandchildFlag")]
        public bool GrandchildFlag { get; set; }
    }


    #endregion

    class HelpMessages
    {
        [NamedArgument('a', "ArgumentA", required: true, helpText: "The first required Argument")]
        public string ParamA { get; set; }

        [NamedArgument('b', "ArgumentB", helpText: "The first optional Argument")]
        public string ParamB { get; set; }

        [FlagArgument("flagA", helpText: "This is a flag")]
        public bool FlagA { get; set; }

        [PositionalArgument("posA", helpText: "This is a positional Argument")]
        public string PositionalA { get; set; }

        [Subcommand("doSomething", helpText: "This subcommand does stuff")]
        public HelpMessages Ayudame { get; set; }
    }
}