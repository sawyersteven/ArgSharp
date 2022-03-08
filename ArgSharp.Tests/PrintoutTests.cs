using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArgSharp.Tests
{
    [TestClass]
    public class PrintoutTests
    {
        private class StdoutList : TextWriter
        {
            public List<string> Lines = new List<string>();
            public override Encoding Encoding => System.Text.Encoding.Default;

            public override void WriteLine(string value)
            {
                Lines.Add(value);
            }
        }

        [TestMethod]
        public void TestUsage()
        {
            StdoutList outList = new StdoutList();
            Console.SetOut(outList);

            outList.Lines.Clear();
            new ArgSharp.Parser<HelpMessages>() { ExitIfPrintText = false }.Parse(new string[] { "--help" });
            Assert.AreEqual("Usage: testhost [-b ArgumentB] [--flagA] -a ArgumentA posA", outList.Lines[1]);
        }

        [TestMethod]
        public void TestVersion()
        {
            StdoutList outList = new StdoutList();
            Console.SetOut(outList);

            outList.Lines.Clear();
            new ArgSharp.Parser<HelpMessages>() { ExitIfPrintText = false }.Parse(new string[] { "--version" });
            Assert.AreEqual("testhost (1.1.0)", outList.Lines[0]);
        }

        [TestMethod]
        public void TestCategoryOrder()
        {
            StdoutList outList = new StdoutList();
            Console.SetOut(outList);

            outList.Lines.Clear();
            new ArgSharp.Parser<HelpMessages>() { ExitIfPrintText = false }.Parse(new string[] { "--help" });

            int[] indexes = new int[3]; // optional, required, commands
            for (int i = 0; i < outList.Lines.Count; i++)
            {
                string line = outList.Lines[i];
                if (line.StartsWith("Optional")) indexes[0] = i;
                else if (line.StartsWith("Required")) indexes[1] = i;
                else if (line.StartsWith("Additional Commands")) indexes[2] = i;
            }

            int mult = 1;
            foreach (int i in indexes) mult *= i;

            Assert.IsTrue(mult != 0);

            for (int i = 0; i < indexes.Length - 1; i++)
            {
                Assert.IsTrue(indexes[i] < indexes[i + 1]);
            }
        }

        [TestMethod]
        public void TestHelpNestedCommands()
        {
            StdoutList outList = new StdoutList();
            Console.SetOut(outList);
            outList.Lines.Clear();

            new ArgSharp.Parser<NestedSubcommandContainer>() { ExitIfPrintText = false }.Parse(new string[] { "nestedSubChild", "nestedSubGrandchild", "--help" });
            Assert.AreEqual("Usage: testhost nestedSubChild nestedSubGrandchild [--grandchildFlag]", outList.Lines[1]);
        }
    }
}