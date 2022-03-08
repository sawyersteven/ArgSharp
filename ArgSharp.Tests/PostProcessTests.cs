using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
Tests for postprocessing functions
*/

namespace ArgSharp.Tests
{
    [TestClass]
    public class PostProcessTests
    {

        [TestMethod]
        public void TestPostProcessNumbers()
        {
            string[] positionalArgs = new string[] { "1", "2", "3", "5", "8", "13", "21", "3", "55.5", "89.9", "144.4", "I'm a string" };

            var pp = new ArgSharp.Parser<PositionalPrimitives>().Parse(positionalArgs, (parsed) =>
            {
                parsed.byteProp--;
                parsed.shortProp++;
                parsed.ushortProp--;
                parsed.intProp++;
                parsed.uintProp--;
                parsed.longProp++;
                parsed.ulongProp *= 2;
                parsed.charProp--;
                parsed.floatProp++;
                parsed.doubleProp--;
                parsed.decimalProp++;
            });

            Assert.AreEqual<byte>(1 - 1, pp.byteProp);
            Assert.AreEqual<short>(2 + 1, pp.shortProp);
            Assert.AreEqual<ushort>(3 - 1, pp.ushortProp);
            Assert.AreEqual<int>(5 + 1, pp.intProp);
            Assert.AreEqual<uint>(8 - 1, pp.uintProp);
            Assert.AreEqual<long>(13 + 1, pp.longProp);
            Assert.AreEqual<ulong>(21 * 2, pp.ulongProp);
            Assert.AreEqual<char>((char)('3' - 1), pp.charProp);
            Assert.AreEqual<float>(55.5f + 1, pp.floatProp);
            Assert.AreEqual<double>(89.9d - 1, pp.doubleProp);
            Assert.AreEqual<decimal>(144.4m + 1, pp.decimalProp);
            Assert.AreEqual<string>("I'm a string", pp.stringProp);
        }
    }
}