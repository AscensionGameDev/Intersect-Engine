using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Intersect.Extensions
{

    [TestFixture]
    public class GameObjectInfoAttributeTests
    {

        [Test]
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
