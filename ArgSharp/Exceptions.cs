namespace ArgSharp.Exceptions
{
    // For when there are args passed that have no matching name or flag, or
    // exceed the length of positional args required
    public class UnknownArgumentException : System.Exception
    {
        public UnknownArgumentException(string arg) : base($"Unknown argument: {arg}") { }
    }

    // For when a args name collides with reserved names (eg --help) or 
    // is identical to another named arg
    public class InvalidNameException : System.Exception
    {
        public InvalidNameException(string message) : base(message) { }
    }

    // For when a named attribute has no subsequent value or if a positional
    // value cannot be found
    public class MissingValueException : System.Exception
    {
        public MissingValueException(string argName) : base($"Missing value for argument: {argName}") { }
    }

    // For when a passed value cannot be coerced into the property type
    public class IncompatibleValueException : System.Exception
    {
        public IncompatibleValueException(string value, string expectedType) : base($"{value} does not match the expected type {expectedType}") { }
    }

    // For when a required value is not included in the arg list
    public class RequiredArgumentException : System.Exception
    {
        public RequiredArgumentException(string argName) : base($"Missing required argument: {argName}") { }
    }
}