using System;
using UnityEngine;

public readonly struct OsmTileCoordinate
{
    public OsmTileCoordinate(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }
    public double Y { get; }
}

public readonly struct OsmTileKey : IEquatable<OsmTileKey>
{
    public OsmTileKey(int zoom, int x, int y)
    {
        Zoom = zoom;
        X = x;
        Y = y;
    }

    public int Zoom { get; }
    public int X { get; }
    public int Y { get; }

    public OsmTileKey GetChild(int index)
    {
        if (index is < 0 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return new OsmTileKey(
            Zoom + 1,
            X * 2 + index % 2,
            Y * 2 + index / 2);
    }

    public bool Equals(OsmTileKey other)
    {
        return Zoom == other.Zoom && X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        return obj is OsmTileKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Zoom, X, Y);
    }

    public override string ToString()
    {
        return $"{Zoom}/{X}/{Y}";
    }
}

public static class OsmTileMath
{
    public const double EarthCircumferenceMetres = 40075016.68557849d;
    public const double MaximumMercatorLatitude = 85.05112878d;

    public static OsmTileCoordinate LatLonToTile(
        double latitude,
        double longitude,
        int zoom)
    {
        ValidateZoom(zoom);
        var tileCount = Math.Pow(2d, zoom);
        var clampedLatitude = Math.Clamp(
            latitude,
            -MaximumMercatorLatitude,
            MaximumMercatorLatitude);
        var latitudeRadians = clampedLatitude * Math.PI / 180d;
        return new OsmTileCoordinate(
            (longitude + 180d) / 360d * tileCount,
            (1d - Math.Asinh(Math.Tan(latitudeRadians)) / Math.PI) *
            0.5d *
            tileCount);
    }

    public static void TileToLatLon(
        double tileX,
        double tileY,
        int zoom,
        out double latitude,
        out double longitude)
    {
        ValidateZoom(zoom);
        var tileCount = Math.Pow(2d, zoom);
        longitude = tileX / tileCount * 360d - 180d;
        var mercatorY = Math.PI * (1d - 2d * tileY / tileCount);
        latitude = Math.Atan(Math.Sinh(mercatorY)) * 180d / Math.PI;
    }

    public static double GroundTileSizeMetres(double latitude, int zoom)
    {
        ValidateZoom(zoom);
        var clampedLatitude = Math.Clamp(
            latitude,
            -MaximumMercatorLatitude,
            MaximumMercatorLatitude);
        return EarthCircumferenceMetres *
               Math.Cos(clampedLatitude * Math.PI / 180d) /
               Math.Pow(2d, zoom);
    }

    public static Bounds GetLocalBounds(
        OsmTileKey key,
        double originTileX,
        double originTileY,
        double originLatitude,
        float verticalSize = 2f)
    {
        var tileSize = GroundTileSizeMetres(originLatitude, key.Zoom);
        var centerX = (key.X + 0.5d - originTileX) * tileSize;
        var centerZ = (originTileY - key.Y - 0.5d) * tileSize;
        return new Bounds(
            new Vector3((float)centerX, 0f, (float)centerZ),
            new Vector3(
                (float)tileSize,
                Mathf.Max(0.01f, verticalSize),
                (float)tileSize));
    }

    public static int WrapTileX(int tileX, int zoom)
    {
        ValidateZoom(zoom);
        var tileCount = 1 << zoom;
        return ((tileX % tileCount) + tileCount) % tileCount;
    }

    public static bool IsValidTileY(int tileY, int zoom)
    {
        ValidateZoom(zoom);
        var tileCount = 1 << zoom;
        return tileY >= 0 && tileY < tileCount;
    }

    private static void ValidateZoom(int zoom)
    {
        if (zoom is < 0 or > 30)
        {
            throw new ArgumentOutOfRangeException(
                nameof(zoom),
                "OSM zoom must be between 0 and 30.");
        }
    }
}
