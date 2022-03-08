using ArgSharp.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
Tests for basic functionality of parsing string params into their property type
and matching names/positions to properties.
*/

namespace ArgSharp.Tests
{
    [TestClass]
    public class BasicParseTests
    {
        [TestMethod]
        public void TestShortName()
        {
            string[] shortNameArgs = new string[] {
            "-b", "1",
            "-s", "2",
            "-t", "3",
            "-i", "5",
            "-j", "8",
            "-l", "13",
            "-o", "21",
            "-c", "3",
            "-f", "55.5",
            "-d", "89.9",
            "-m", "144.4",
            "-g", "I'm a string"
            };

            var np = new ArgSharp.Parser().ParseIntoNew<NamedPrimitives>(shortNameArgs);
            Assert.AreEqual<byte>(1, np.byteProp);
            Assert.AreEqual<short>(2, np.shortProp);
            Assert.AreEqual<ushort>(3, np.ushortProp);
            Assert.AreEqual<int>(5, np.intProp);
            Assert.AreEqual<uint>(8, np.uintProp);
            Assert.AreEqual<long>(13, np.longProp);
            Assert.AreEqual<ulong>(21, np.ulongProp);
            Assert.AreEqual<char>('3', np.charProp);
            Assert.AreEqual<float>(55.5f, np.floatProp);
            Assert.AreEqual<double>(89.9d, np.doubleProp);
            Assert.AreEqual<decimal>(144.4m, np.decimalProp);
            Assert.AreEqual<string>("I'm a string", np.stringProp);
        }

        [TestMethod]
        public void TestLongName()
        {
            string[] longNameArgs = new string[] {
            "--byte", "1",
            "--short", "2",
            "--ushort", "3",
            "--int", "5",
            "--uint", "8",
            "--long", "13",
            "--ulong", "21",
            "--char", "3",
            "--float", "55.5",
            "--double", "89.9",
            "--decimal", "144.4",
            "--string", "I'm a string"
            };


            var np = new ArgSharp.Parser().ParseIntoNew<NamedPrimitives>(longNameArgs);
            Assert.AreEqual<byte>(1, np.byteProp);
            Assert.AreEqual<short>(2, np.shortProp);
            Assert.AreEqual<ushort>(3, np.ushortProp);
            Assert.AreEqual<int>(5, np.intProp);
            Assert.AreEqual<uint>(8, np.uintProp);
            Assert.AreEqual<long>(13, np.longProp);
            Assert.AreEqual<ulong>(21, np.ulongProp);
            Assert.AreEqual<char>('3', np.charProp);
            Assert.AreEqual<float>(55.5f, np.floatProp);
            Assert.AreEqual<double>(89.9d, np.doubleProp);
            Assert.AreEqual<decimal>(144.4m, np.decimalProp);
            Assert.AreEqual<string>("I'm a string", np.stringProp);
        }

        [TestMethod]
        public void TestFlag()
        {
            string[] flagArgs = new string[] { "--True1", "--True2" };
            var f = new ArgSharp.Parser().ParseIntoNew<FlagBooleans>(flagArgs);
            Assert.AreEqual(false, f.False1Prop);
            Assert.AreEqual(true, f.True1Prop);
            Assert.AreEqual(true, f.True2Prop);
            Assert.AreEqual(false, f.False2Prop);
        }

        [TestMethod]
        public void TestFlagsIntoExisting()
        {
            string[] flagArgs = new string[] { "--True1", "--True2" };
            var f = new FlagBooleans();
            new ArgSharp.Parser().ParseInto(flagArgs, f);
            Assert.AreEqual(false, f.False1Prop);
            Assert.AreEqual(true, f.True1Prop);
            Assert.AreEqual(true, f.True2Prop);
            Assert.AreEqual(false, f.False2Prop);
        }

        [TestMethod]
        public void TestPositionals()
        {
            string[] positionalArgs = new string[] { "1", "2", "3", "5", "8", "13", "21", "3", "55.5", "89.9", "144.4", "I'm a string" };
            var pp = new ArgSharp.Parser().ParseIntoNew<PositionalPrimitives>(positionalArgs);
            Assert.AreEqual<byte>(1, pp.byteProp);
            Assert.AreEqual<short>(2, pp.shortProp);
            Assert.AreEqual<ushort>(3, pp.ushortProp);
            Assert.AreEqual<int>(5, pp.intProp);
            Assert.AreEqual<uint>(8, pp.uintProp);
            Assert.AreEqual<long>(13, pp.longProp);
            Assert.AreEqual<ulong>(21, pp.ulongProp);
            Assert.AreEqual<char>('3', pp.charProp);
            Assert.AreEqual<float>(55.5f, pp.floatProp);
            Assert.AreEqual<double>(89.9d, pp.doubleProp);
            Assert.AreEqual<decimal>(144.4m, pp.decimalProp);
            Assert.AreEqual<string>("I'm a string", pp.stringProp);
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            string[] noArgs = new string[] { };
            var dv = new ArgSharp.Parser().ParseIntoNew<DefaultValues>(noArgs);
            Assert.AreEqual(456, dv.Def);
        }

        [TestMethod]
        public void TestTryGetNamed()
        {
            string[] args = new string[] { "--byte", "4" };
            byte value;
            bool found = ArgSharp.Parser.TryGetName(args, 'b', "byte", out value);
            Assert.IsTrue(found);
            Assert.AreEqual<byte>(4, value);
            found = ArgSharp.Parser.TryGetName(args, 'x', "notavalue", out value);
            Assert.IsFalse(found);
            Assert.AreEqual<byte>(0, value);
        }
    }
}