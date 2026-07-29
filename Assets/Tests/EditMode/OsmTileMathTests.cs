using System;
using NUnit.Framework;
using UnityEngine;

public sealed class OsmTileMathTests
{
    [Test]
    public void LatLonToTile_MapsPrimeMeridianAndEquatorToWorldCentre()
    {
        var tile = OsmTileMath.LatLonToTile(0d, 0d, 1);

        Assert.That(tile.X, Is.EqualTo(1d).Within(1e-12d));
        Assert.That(tile.Y, Is.EqualTo(1d).Within(1e-12d));
    }

    [Test]
    public void TileToLatLon_RoundTripsProjectedCoordinate()
    {
        const double expectedLatitude = 21.13284948d;
        const double expectedLongitude = 105.89976681d;
        var tile = OsmTileMath.LatLonToTile(
            expectedLatitude,
            expectedLongitude,
            18);

        OsmTileMath.TileToLatLon(
            tile.X,
            tile.Y,
            18,
            out var actualLatitude,
            out var actualLongitude);

        Assert.That(actualLatitude, Is.EqualTo(expectedLatitude).Within(1e-9d));
        Assert.That(actualLongitude, Is.EqualTo(expectedLongitude).Within(1e-9d));
    }

    [Test]
    public void Children_PerfectlySubdivideParentBounds()
    {
        const double latitude = 21.13284948d;
        const double longitude = 105.89976681d;
        const int parentZoom = 12;
        var parentOrigin = OsmTileMath.LatLonToTile(
            latitude,
            longitude,
            parentZoom);
        var childOrigin = OsmTileMath.LatLonToTile(
            latitude,
            longitude,
            parentZoom + 1);
        var parentKey = new OsmTileKey(
            parentZoom,
            (int)Math.Floor(parentOrigin.X),
            (int)Math.Floor(parentOrigin.Y));
        var parentBounds = OsmTileMath.GetLocalBounds(
            parentKey,
            parentOrigin.X,
            parentOrigin.Y,
            latitude);
        var childrenBounds = OsmTileMath.GetLocalBounds(
            parentKey.GetChild(0),
            childOrigin.X,
            childOrigin.Y,
            latitude);

        for (var i = 1; i < 4; i++)
        {
            childrenBounds.Encapsulate(
                OsmTileMath.GetLocalBounds(
                    parentKey.GetChild(i),
                    childOrigin.X,
                    childOrigin.Y,
                    latitude));
        }

        Assert.That(childrenBounds.min.x, Is.EqualTo(parentBounds.min.x).Within(0.01f));
        Assert.That(childrenBounds.max.x, Is.EqualTo(parentBounds.max.x).Within(0.01f));
        Assert.That(childrenBounds.min.z, Is.EqualTo(parentBounds.min.z).Within(0.01f));
        Assert.That(childrenBounds.max.z, Is.EqualTo(parentBounds.max.z).Within(0.01f));
        Assert.That(
            OsmTileMath.GetLocalBounds(
                parentKey.GetChild(0),
                childOrigin.X,
                childOrigin.Y,
                latitude).size.x,
            Is.EqualTo(parentBounds.size.x * 0.5f).Within(0.01f));
    }
}
