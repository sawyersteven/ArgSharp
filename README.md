# ArgSharp

A small, simple command-line argument parsing library

## Quick Start Guide

Create a class to contain command line arguments:

```cs
public class CMDLineArgs{

    public int LogLevel {get;set;}

    public string InFile {get;set;}

    public string OutFile {get;set;}
}
```

Add attributes to define command-line usage:

```cs
public class CMDLineArgs{

    [NamedArgument('l', "log_level", helpText: "Logging level: 0 ERROR, 1 WARN, 2 INFO"]
    public int LogLevel {get;set;}

    [PositionalArgument("Input File", helpText: "File to compress")]
    public string InFile {get;set;}

    [PositionalArgument("Ouput File", helpText: "Output archive file")]
    public string OutFile {get;set;}
}
```

Parse your arguments:
```cs
public static void Main(string[] args)
{
    var parser = new ArgSharp.Parser<CMDLineArgs>();
    CMDLineArgs parsedCMD = parser.Parse(args);
    Console.WriteLine($"Input File is {parsedCMD.InFile}");
}
```

Build and run your program:
```bash
$ ./my_app --log_level 1 ~/Documents/some_file.txt ~/Documents/some_file.7z
$ Input File is ~/Documents/some_file.txt
```

# Attribute Types

## Named Arguments
Usage:

`[NamedArgument(char shortname, string longname, bool required=false, string help="")]`

Example:
```cs
[NamedArgument('c', "compression", required: true, helpText: "Compression level"]
public int CompressionLevel {get;set;}
```

Help printout:
```
-c|--compression    int    Compression level
```

A `NamedArgument` requires both a `shortname` and `longname`. An attempt will be made to convert the passed string value into the property type. If this conversion fails the program will exit and an error message will be printed. If the argument is `required` and a value is not passed in the command line the program will exit and an error message will be printed.

## Flag Arguments
Usage:

`[FlagArg(string name, string help="")]`

Example:
```cs
[FlagArg("overwrite", helpText: "Overwrite existing files without warning")]
public bool Overwrite {get;set;}
```

Help printout:
```
--overwrite    Overwrite existing files without warning
```

A `FlagArg` operates as a boolean flag and may only be applied to bool properties. The default value of any `FlagArg` will be false and the property will be set to true if the flag is included in the argument list.

## Positional Arguments
Usage:

`[PositionalArgument(string name, string help="")]`

Example:
```cs
[PositionalArgument("input_file", helpText: "File to compress")]
public string InFile {get;set;}
```

Help printout:
```
input_file    File to process
```

A `PositionalArgument` is an un-named argument passed at the end of the argument string. A `PositionalArgument` is always required unless a `Subcommand` is passed. In this case every `PositionalArgument` will be parsed in the subcommand. If a required `PositionalArgument` is not passed the program will exit and an error message will be printed.

## Subcommand
Usage:

`[Subcommand(string name, string help="")]`

Example:
```cs
[Subcommand("unpack", "Unpack files from the provided archive")]
public UnpackSubcommand Unpack {get;set;}
```

Help printout:
```
[unpack]    Unpack files from the provided archive
```

A `Subcommand` is used to created nested groups of arguments. If a `Subcommand` is parsed the parent will not recieve any `PositionalArgumenteter` values and all will be parsed as arguments to the subcommand. Flags and Names are still valid on the parent object. If a parent and a subcommand have identical names for flag or named arguments the parent will consume them first. It is best to avoid this situation if possible.

A typical use case would look similar to this:
```cs

public class CMDLineArgs
{
    [FlagArg("verbose", helpText:"Print all messages to console")]
    public bool Verbose {get;set;}

    [Subcommand("unpack", helpText:"Unpack an archive")]
    public UnpackSubcommand UnpackCommand {get;set;}

    [Subcommand("verify", helpText:"Verify the contents of an archive")]
    public VeryifySubcommand VerifyCommand {get;set;}
}

public class UnpackSubcommand
{
    [PositionalArgument("inputFile", helpText:"Archive file to unpack")]
    public string InputFile{get;set;}

    [NamedArgument('o', "outputDir", helpText:"Directory to unpack into")]
    public string OutputDir{get;set;}
}

public class VerifySubcommand
{
    [PositionalArgument("inputFile", helpText:"Archive file to verify")]
    public string InputFile{get;set;}
}
```

Then handle the arguments:

```cs
public static void Main(string[] args)
{
    var parser = new ArgSharp.Parser<CMDLineArgs>;
    CMDLineArgs parsedCMD = parser.Parse(args);
    if(parsedCMD.UnpackCommand != null)
    {
        Console.WriteLine($"Unpacking {a.InFile}");
    } else if (parsedCMD.VerifyCommand != null){
        Console.WriteLine($"Verifying archive {a.InFile}");
    }
}
```

```
$ ./my_app unpack -o /tmp some_archive.7z
$ Unpacking some_archive.7z
```



# Default Values
Any argument can have a default value assigned to it simply by assigning a value to the property delcaration:

```cs
[NamedArgument('i',"iterations", helpText: "Number of times to process file")]
public int Iterations {get;set;} = 42;
```

A default value *can* be assigned to any argument, but should only be assigned to `NamedArgument` properties.

* A `Subcommand` being null indicates that the user did not select this command and assigning a value to it will complicate handling the subcommand later.
* A `FlagArg` will have the default value `false`; assigning `true` will result in the flag always being true regardless of the user providing that flag.
* A `PositionalArgument` is always required and the default value will never be used.