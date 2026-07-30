using CauDuong.IfcOperations;
using NUnit.Framework;
using UnityEngine;

public sealed class IfcInfrastructureClassifierTests
{
    [TestCase("NT_Vạch sơn 3.1a", "IfcBuiltElement", IfcInfrastructureCategory.TrafficSafety)]
    [TestCase("NT-Pole3DBlock", "IfcBuildingElementProxy", IfcInfrastructureCategory.TrafficSafety)]
    [TestCase("K - Pave1", "IfcBuiltElement", IfcInfrastructureCategory.Pavement)]
    [TestCase("Dải phân cách giữa", "IfcBuiltElement", IfcInfrastructureCategory.Barrier)]
    [TestCase("Móng cọc BTXM", "IfcFooting", IfcInfrastructureCategory.Foundation)]
    [TestCase("Mái taluy dương", "IfcGeographicElement", IfcInfrastructureCategory.SlopeAndLandscape)]
    [TestCase("SOLIDS - VNT1 - BV1.RD", "IfcBuiltElement", IfcInfrastructureCategory.RouteInfrastructure)]
    public void Classify_UsesVietnameseAndEnglishKeywords(
        string name,
        string ifcType,
        IfcInfrastructureCategory expected)
    {
        Assert.That(
            IfcInfrastructureClassifier.Classify(name, ifcType),
            Is.EqualTo(expected));
    }

    [Test]
    public void Normalize_RemovesVietnameseDiacriticsAndSeparators()
    {
        Assert.That(
            IfcInfrastructureClassifier.Normalize("MẶT_ĐƯỜNG-BÊ TÔNG"),
            Is.EqualTo("mat duong be tong"));
    }

    [Test]
    public void MeasurementMath_ComputesDistanceHeightAndArea()
    {
        var points = new[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 0f, 4f),
            new Vector3(0f, 0f, 4f)
        };

        Assert.That(
            IfcMeasurementMath.Distance(points[0], points[2]),
            Is.EqualTo(5f).Within(0.0001f));
        Assert.That(
            IfcMeasurementMath.Height(Vector3.zero, new Vector3(1f, 7f, 2f)),
            Is.EqualTo(7f).Within(0.0001f));
        Assert.That(
            IfcMeasurementMath.PolygonArea(points),
            Is.EqualTo(12f).Within(0.0001f));
    }
}
