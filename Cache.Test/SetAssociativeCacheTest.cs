using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cache.Test
{
    [TestClass]
    public class SetAssociativeCacheTest
    {
        [TestMethod]
        public void InitializeTest()
        {
            new SetAssociativeCache<string, string>(2, 2);
        }

        [TestMethod]
        public void InitializeTest_InvalidMaxCount()
        {
            Assert.ThrowsException<ArgumentException>(() => 
                new SetAssociativeCache<string, string>(0, 2));
        }

        [TestMethod]
        public void InitializeTest_InvalidSetCount()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new SetAssociativeCache<string, string>(1, -1));
        }

        [TestMethod]
        public void GetTest_NotFound()
        {
            var cache = new SetAssociativeCache<string, string>(2, 2);
            var isSuccess = cache.TryGet("notfound", out _);

            Assert.IsFalse(isSuccess);
        }

        [TestMethod]
        public void GetTest()
        {
            var cache = new SetAssociativeCache<string, string>(2, 2);
            cache.TryAdd("key", "value");
            var isSuccess = cache.TryGet("key", out var value);

            Assert.IsTrue(isSuccess);
            Assert.AreEqual("value", value);
        }
    }
}