using System.Reflection;
using Intersect.Framework;
using Intersect.Localization;
using Intersect.Models.Annotations;
using NUnit.Framework;

namespace Intersect.Models.Interop;

[TestFixture]
public class ObjectInteropModelBuilderTests
{
    [Test]
    public void TestConstructor()
    {
    }

    [TestCaseSource(nameof(TestScanHierarchySources))]
    public void TestBuild(Type type, List<Type> expectedHierarchy)
    {
        var objectInteropModel = new ObjectInteropModel.Builder(type)
            .IncludeProperties()
            .IncludeReadOnly()
            .SetPropertyBindingFlags(BindingFlags.Public)
            .Build();
        Assert.That(objectInteropModel, Is.EquivalentTo(expectedHierarchy), "Hierarchy mismatch");
    }

    [TestCaseSource(nameof(TestScanHierarchySources))]
    public void TestScanHierarchy(Type type, List<Type> expectedHierarchy)
    {
        var actualHierarchy = ObjectInteropModel.Builder.ScanHierarchy(type);
        Assert.That(actualHierarchy, Is.EqualTo(expectedHierarchy).AsCollection, "Hierarchy mismatch");
    }

    private static readonly object[] TestScanHierarchySources =
    {
        new object[] { typeof(Grandparent), new List<Type> { typeof(Grandparent) } },
        new object[] { typeof(Parent), new List<Type> { typeof(Grandparent), typeof(Parent) } },
        new object[] { typeof(Child), new List<Type> { typeof(Grandparent), typeof(Parent), typeof(Child) } },
    };

    private class TestNamespace : LocaleNamespace
    {
        public readonly LocalizedString GroupGeneral = @"General";
        public readonly LocalizedString GroupStats = @"Stats";

        public readonly LocalizedString HintDescription = @"The user-visible description of the object";
        public readonly LocalizedString HintName = @"The user-visible name of the object";
        public readonly LocalizedString LabelDescription = @"Description";
        public readonly LocalizedString LabelName = @"Name";
        public readonly LocalizedString TooltipId = @"The generated ID of the object";
    }

    private class TestNamespace2 : LocaleNamespace
    {
        public readonly LocalizedString TooltipDescription = @"The tooltip of the description";
    }

    public class Grandparent
    {
        [Group(typeof(TestNamespace), nameof(TestNamespace.GroupGeneral))]
        [Input(ReadOnly = true)]
        [Tooltip(typeof(TestNamespace), nameof(TestNamespace.TooltipId))]
        public Id<Grandparent> Id { get; set; }

        [Group(typeof(TestNamespace), nameof(TestNamespace.GroupGeneral))]
        [Input(
            HintName = nameof(TestNamespace.HintName),
            HintNamespace = typeof(TestNamespace)
        )]
        [Label(typeof(TestNamespace), nameof(TestNamespace.LabelName))]
        public string Name { get; set; }

        [Ignored] public string IgnoredStringProperty { get; set; }

        public int GrouplessIntegerProperty { get; set; }
    }

    public class Parent : Grandparent
    {
        [Group(typeof(TestNamespace), nameof(TestNamespace.GroupStats))]
        [InputRangeIntegral(0, 100)]
        public int Health { get; set; }

        [InputRangeFloat(-6.28, 6.28)]
        public float Angle { get; set; }
    }

    private class Child : Parent
    {
        [Group(typeof(TestNamespace), nameof(TestNamespace.GroupGeneral))]
        [Input(HintName = nameof(TestNamespace.HintDescription))]
        [Label(typeof(TestNamespace), nameof(TestNamespace.LabelDescription))]
        [Tooltip(typeof(TestNamespace2), nameof(TestNamespace2.TooltipDescription))]
        public string Description { get; set; }
    }
}
