using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intersect.Extensions
{
    [TestClass]
    public class GameObjectInfoAttributeTests
    {
        [TestMethod]
        public void GameObjectInfoAttributeTest()
        {
            const string table = "FakeTable";
            var type = typeof(GameObjectInfoAttributeTests);

            var gameObjectInfo = new GameObjectInfoAttribute(type, table);

            Assert.AreEqual(type, gameObjectInfo.Type);
            Assert.AreEqual(table, gameObjectInfo.Table);
        }
    }
}