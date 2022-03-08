using ArgSharp.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
Tests for anything involving a subcommand, inlcuding nested subcommands and
ensuring the parent is able to parse values without te sub consuming them first
*/

namespace ArgSharp.Tests
{
    [TestClass]
    public class SubcommandTests
    {


        [TestMethod]
        public void TestSubCommandWithNamed()
        {
            string[] args = new string[] { "withNamedSub", "--float", "123.456" };
            var scc = new ArgSharp.Parser<SubcommandContainer>().Parse(args);
            Assert.IsNotNull(scc.subcomWithName);
            Assert.AreEqual(123.456f, scc.subcomWithName.floatProp);
        }

        [TestMethod]
        public void TestSubCommandWithFlags()
        {
            string[] args = new string[] { "withFlagsSub", "--on" };
            var scc = new ArgSharp.Parser<SubcommandContainer>().Parse(args);
            Assert.IsNotNull(scc.subcomWithFlags);
            Assert.AreEqual(true, scc.subcomWithFlags.shouldBeOn);
            Assert.AreEqual(false, scc.subcomWithFlags.shouldBeOff);
        }

        [TestMethod]
        public void TestSubCommandWithPositionals()
        {
            string[] args = new string[] { "withPositionalsSub", "first", "2", "3" };
            var scc = new ArgSharp.Parser<SubcommandContainer>().Parse(args);
            Assert.IsNotNull(scc.subcomWithPositionals);
            Assert.AreEqual("first", scc.subcomWithPositionals.first);
            Assert.AreEqual(2, scc.subcomWithPositionals.second);
            Assert.AreEqual('3', scc.subcomWithPositionals.third);
        }

        [TestMethod]
        public void TestSubcommandWithParentArgs()
        {
            string[] args = new string[] { "withNamedSub", "--float", "123.456", "--namedParam", "named param in parent", "--flagParam", };
            var scc = new ArgSharp.Parser<SubcommandContainer>().Parse(args);
            Assert.IsNotNull(scc.subcomWithName);
            Assert.AreEqual(123.456f, scc.subcomWithName.floatProp);
            Assert.AreEqual("named param in parent", scc.namedParam);
            Assert.AreEqual(true, scc.flagParam);
        }

        [TestMethod]
        public void TestNestedSubcommands()
        {
            /*
            subcommandContainer
                withNamedSub subcommandWithNamedParams
                    withFlags subCommandWithFlagParams
                        --on
                    -f 987.654
                containerPositional
            */
            string[] args = new string[] { "withNamedSub", "withFlagsSub", "--on", "-f", "987.654" };
            var scc = new ArgSharp.Parser<SubcommandContainer>().Parse(args);

            Assert.IsNotNull(scc.subcomWithName);
            Assert.IsNull(scc.subcomWithFlags);
            Assert.IsNull(scc.subcomWithPositionals);
            Assert.IsNotNull(scc.subcomWithName.subcomWithFlags);
            Assert.AreEqual(true, scc.subcomWithName.subcomWithFlags.shouldBeOn);
            Assert.AreEqual(987.654f, scc.subcomWithName.floatProp);
        }
    }
}