
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
            Assert.ThrowsException<IncompatibleValueException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<NamedPrimitives>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestIncompatiblePositionalValue()
        {
            string[] incompatibleArgs = new string[] { $"{byte.MaxValue + 1}" };
            Assert.ThrowsException<IncompatibleValueException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<PositionalPrimitives>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestRequiredNamedValue()
        {
            string[] incompatibleArgs = new string[] { "--int", "5678" };
            Assert.ThrowsException<RequiredArgumentException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<NamedPrimitives>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestMissingNamedValue()
        {
            string[] incompatibleArgs = new string[] { "--byte", "1", "--long" };
            Assert.ThrowsException<MissingValueException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<NamedPrimitives>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestMissingPositionalValue()
        {
            // Contains all but last arg for stringProp
            string[] incompatibleArgs = new string[] { "1", "2", "3", "5", "8", "13", "21", "3", "55.5", "89.9", "144.4" };
            Assert.ThrowsException<RequiredArgumentException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<NamedPrimitives>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestUnknownNamedValue()
        {
            string[] incompatibleArgs = new string[] { "--IDontExist", "null" };
            Assert.ThrowsException<UnknownArgumentException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<Empty>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestExtraPositionalValue()
        {
            string[] incompatibleArgs = new string[] { "1", "2", "3", "5", "8", "13", "21", "3", "55.5", "89.9", "144.4", "I'm a string", "I'm one too many" };
            Assert.ThrowsException<UnknownArgumentException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<PositionalPrimitives>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestUnknownFlagValue()
        {
            string[] incompatibleArgs = new string[] { "--NotARealFlag" };
            Assert.ThrowsException<UnknownArgumentException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<FlagBooleans>(incompatibleArgs);
            });
        }

        [TestMethod]
        public void TestReservedFlagValue()
        {
            Assert.ThrowsException<InvalidNameException>(() =>
            {
                ReservedHelpFlag rh = new ReservedHelpFlag();
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<ReservedHelpFlag>(new string[] { "--help" });
            });

            Assert.ThrowsException<InvalidNameException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<ReservedVersionName>(new string[] { "--version" });
            });

            Assert.ThrowsException<InvalidNameException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<ReservedHelpFlag>(new string[] { "--help" });
            });

            Assert.ThrowsException<InvalidNameException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<ReservedVersionName>(new string[] { "--version" });
            });
        }

        [TestMethod]
        public void TestSubcommandWhiteSpace()
        {
            Assert.ThrowsException<InvalidNameException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<WhiteSpaceSubcommand>(new string[] { "Mock Subcommand" });
            });
        }

        [TestMethod]
        public void TestFlagOnNonBoolType()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                new ArgSharp.Parser() { ExitAfterPrint = false }.ParseIntoNew<FlagOnNonBool>(new string[] { });
            });
        }
    }
}