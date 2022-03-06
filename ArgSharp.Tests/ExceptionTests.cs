
using System;
using ArgSharp.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
Tests providing malformed, incompatible, or incomplete params to the parser.
Every test in this class should throw some kind of exception.
*/

namespace ArgSharp.Tests
{
    [TestClass]
    public class ExceptionTests
    {
        [TestMethod]
        public void TestIncompatibleNamedValue()
        {
            string[] incompatibleArgs = new string[] { "--byte", $"{byte.MaxValue + 1}" };
            NamedPrimitives np = new NamedPrimitives();
            Assert.ThrowsException<IncompatibleValueException>(() =>
            {
                new ArgSharp.Parser(np).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestIncompatiblePositionalValue()
        {
            string[] incompatibleArgs = new string[] { $"{byte.MaxValue + 1}" };
            PositionalPrimitives pp = new PositionalPrimitives();
            Assert.ThrowsException<IncompatibleValueException>(() =>
            {
                new ArgSharp.Parser(pp).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestRequiredNamedValue()
        {
            string[] incompatibleArgs = new string[] { "--int", "5678" };
            NamedPrimitives np = new NamedPrimitives();
            Assert.ThrowsException<RequiredArgumentException>(() =>
            {
                new ArgSharp.Parser(np).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestMissingNamedValue()
        {
            string[] incompatibleArgs = new string[] { "--byte", "1", "--long" };
            NamedPrimitives np = new NamedPrimitives();
            Assert.ThrowsException<MissingValueException>(() =>
            {
                new ArgSharp.Parser(np).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestMissingPositionalValue()
        {
            // Contains all but last arg for stringProp
            string[] incompatibleArgs = new string[] { "1", "2", "3", "5", "8", "13", "21", "3", "55.5", "89.9", "144.4" };
            NamedPrimitives np = new NamedPrimitives();
            Assert.ThrowsException<RequiredArgumentException>(() =>
            {
                new ArgSharp.Parser(np).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestUnknownNamedValue()
        {
            string[] incompatibleArgs = new string[] { "--IDontExist", "null" };
            Empty mt = new Empty();
            Assert.ThrowsException<UnknownArgumentException>(() =>
            {
                new ArgSharp.Parser(mt).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestExtraPositionalValue()
        {
            string[] incompatibleArgs = new string[] { "1", "2", "3", "5", "8", "13", "21", "3", "55.5", "89.9", "144.4", "I'm a string", "I'm one too many" };
            PositionalPrimitives pp = new PositionalPrimitives();
            Assert.ThrowsException<UnknownArgumentException>(() =>
            {
                new ArgSharp.Parser(pp).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestUnknownFlagValue()
        {
            string[] incompatibleArgs = new string[] { "--NotARealFlag" };
            FlagBooleans fb = new FlagBooleans();
            Assert.ThrowsException<UnknownArgumentException>(() =>
            {
                new ArgSharp.Parser(fb).Parse(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestReservedFlagValue()
        {
            Assert.ThrowsException<InvalidNameException>(() =>
            {
                ReservedHelpFlag rh = new ReservedHelpFlag();
                new ArgSharp.Parser(rh).Parse(new string[] { "--help" });
            });

            Assert.ThrowsException<InvalidNameException>(() =>
            {
                new ArgSharp.Parser(new ReservedVersionFlag()).Parse(new string[] { "--version" });
            });

            Assert.ThrowsException<InvalidNameException>(() =>
            {
                new ArgSharp.Parser(new ReservedHelpName()).Parse(new string[] { "--help" });
            });

            Assert.ThrowsException<InvalidNameException>(() =>
            {
                new ArgSharp.Parser(new ReservedVersionName()).Parse(new string[] { "--version" });
            });
        }

        [TestMethod]
        public void TestSubcommandWhiteSpace()
        {
            Assert.ThrowsException<InvalidNameException>(() =>
            {
                WhiteSpaceSubcommand ws = new WhiteSpaceSubcommand();
                new ArgSharp.Parser(ws).Parse(new string[] { "Mock Subcommand" });
            });
        }

        [TestMethod]
        public void TestFlagOnNonBoolType()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                FlagOnNonBool ws = new FlagOnNonBool();
                new ArgSharp.Parser(ws).Parse(new string[] { });
            });
        }
    }
}