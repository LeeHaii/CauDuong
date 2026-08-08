using System;
using CauDuong.IfcOperations;
using NUnit.Framework;
using UnityEngine;

public sealed class IfcOperationsStateTests
{
    [TestCase(IfcOperationalStatus.Operational, false, true)]
    [TestCase(IfcOperationalStatus.Operational, true, true)]
    [TestCase(IfcOperationalStatus.Warning, false, true)]
    [TestCase(IfcOperationalStatus.Warning, true, false)]
    [TestCase(IfcOperationalStatus.Critical, true, false)]
    [TestCase(IfcOperationalStatus.Repairing, true, false)]
    public void CalloutPolicy_OperationalAlwaysRemainsUnresolved(
        IfcOperationalStatus status,
        bool hasUserUpdate,
        bool expectedUnresolved)
    {
        Assert.That(
            IfcInspectionCalloutPolicy.ShouldShowUnresolved(status, hasUserUpdate),
            Is.EqualTo(expectedUnresolved));
    }

    [Test]
    public void HasUserUpdate_DistinguishesDefaultStatusFromExplicitSave()
    {
        var gameObject = new GameObject("Operations state test");
        try
        {
            var state = gameObject.AddComponent<IfcOperationsState>();
            state.Initialize(IfcInfrastructureCategory.Pavement, 42, 0);

            Assert.That(state.Status, Is.EqualTo(IfcOperationalStatus.Operational));
            Assert.That(state.HasUserUpdate, Is.False);

            state.UpdateOperations(
                IfcOperationalStatus.Operational,
                string.Empty,
                new DateTime(2026, 8, 8, 10, 30, 0));

            Assert.That(state.Status, Is.EqualTo(IfcOperationalStatus.Operational));
            Assert.That(state.HasUserUpdate, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(gameObject);
        }
    }
}
