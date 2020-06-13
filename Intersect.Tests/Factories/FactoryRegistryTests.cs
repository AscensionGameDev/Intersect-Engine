using System;
using System.Diagnostics;
using System.Linq;

using NUnit.Framework;

using Assert = NUnit.Framework.Assert;

namespace Intersect.Factories
{
    [TestFixture]
    public class FactoryRegistryTests
    {
        private class IntFactory : IFactory<int>
        {
            /// <inheritdoc />
            public int Create(params object[] args) =>
                args.Select((arg, index) => arg is int argInt ? argInt : index).FirstOrDefault();
        }

        [TearDown]
        public void TearDown()
        {
            var type = typeof(FactoryRegistry<int>);
            var propertyFactory = type.GetProperty(nameof(FactoryRegistry<int>.Factory));

            Debug.Assert(propertyFactory != null, nameof(propertyFactory) + " != null");
            propertyFactory.SetValue(null, null);
        }

        [Test]
        public void CreateTest()
        {
            var factory = new IntFactory();
            FactoryRegistry<int>.RegisterFactory(factory);
            Assert.AreEqual(0, factory.Create());
            Assert.AreEqual(1234, factory.Create(1234));
        }

        [Test]
        public void CreateTest_NoRegisteredFactory()
        {
            Assert.Throws<InvalidOperationException>(

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => FactoryRegistry<int>.Create(), "No factory registered for type: System.Int32"
            );
        }

        [Test]
        public void TryCreateTest()
        {
            var factory = new IntFactory();
            FactoryRegistry<int>.RegisterFactory(factory);
            Assert.IsTrue(FactoryRegistry<int>.TryCreate(out var created, 1234));
            Assert.AreEqual(1234, created);
        }

        [Test]
        public void TryCreateTest_NoRegisteredFactory()
        {
            Assert.IsFalse(FactoryRegistry<int>.TryCreate(out var created));
            Assert.AreEqual(default(int), created);
        }

        [Test]
        public void RegisterFactoryTest()
        {
            Assert.IsNull(FactoryRegistry<int>.Factory);

            var factory = new IntFactory();
            Assert.IsTrue(FactoryRegistry<int>.RegisterFactory(factory));
            Assert.AreEqual(factory, FactoryRegistry<int>.Factory);
            Assert.IsNotNull(FactoryRegistry<int>.Factory);

            var factory2 = new IntFactory();
            Assert.AreNotEqual(factory, factory2);
            Assert.IsFalse(FactoryRegistry<int>.RegisterFactory(factory2));
            Assert.AreEqual(factory, FactoryRegistry<int>.Factory);

            Assert.IsTrue(FactoryRegistry<int>.RegisterFactory(factory2, true));
            Assert.AreEqual(factory2, FactoryRegistry<int>.Factory);
        }
    }
}
