using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class IfcGeoPositionExtractor : MonoBehaviour
{
    private static readonly Regex NumberPattern = new(
        @"[-+]?(?:\d+(?:[.,]\d+)?|\.\d+)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex HemispherePattern = new(
        @"(?<![A-Z])([NSEW])(?![A-Z])",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly string[] LatitudeKeys =
    {
        "MapConversion/OriginLatitude",
        "OriginLatitude",
        "RefLatitude",
        "ReferenceLatitude",
        "SiteLatitude",
        "Latitude"
    };

    private static readonly string[] LongitudeKeys =
    {
        "MapConversion/OriginLongitude",
        "OriginLongitude",
        "RefLongitude",
        "ReferenceLongitude",
        "SiteLongitude",
        "Longitude"
    };

    private static readonly string[] ElevationKeys =
    {
        "MapConversion/OriginElevation",
        "OriginElevation",
        "RefElevation",
        "ReferenceElevation",
        "SiteElevation",
        "Elevation",
        "Height"
    };

    private static readonly string[] LocalOriginKeys =
    {
        "Local Origin (IFC coordinates)",
        "IFC Local Origin",
        "Local Origin"
    };

    private static readonly string[] LengthScaleKeys =
    {
        "Length Scale (metres/unit)",
        "Metres Per Unit",
        "Meters Per Unit",
        "Length Scale"
    };

    [SerializeField] private XbimIfcLoader loader;
    [SerializeField] private CesiumGeoreference georeference;
    [SerializeField] private bool createGeoreferenceIfMissing = true;
    [SerializeField] private bool attachGlobeAnchor = true;
    [SerializeField] private bool useVn2000LocalOriginFallback = true;
    [SerializeField] private double vn2000CentralMeridianDegrees = 105d;
    [SerializeField] private double vn2000ScaleFactor = 0.9999d;

    public double LastLatitude { get; private set; }
    public double LastLongitude { get; private set; }
    public double LastElevation { get; private set; }

    public event Action<GameObject, double, double, double> GeoPositionApplied;

    private void OnEnable()
    {
        ResolveLoader();
        if (loader != null)
        {
            loader.LoadCompleted += HandleLoadCompleted;
        }
    }

    private void OnDisable()
    {
        if (loader != null)
        {
            loader.LoadCompleted -= HandleLoadCompleted;
        }
    }

    public bool TryApply(GameObject modelRoot)
    {
        if (modelRoot == null)
        {
            Debug.LogWarning("Cannot apply IFC geolocation because the model root is missing.");
            return false;
        }

        if (!TryFindGeoMetadata(
                modelRoot,
                out var geoMetadata,
                out var geoSource,
                out var latitude,
                out var longitude,
                out var elevation))
        {
            Debug.LogWarning(
                "The imported IFC model does not contain a usable IfcSite " +
                "geographic position, supported IfcMapConversion, or valid " +
                "Local Origin (IFC coordinates).");
            return false;
        }

        var targetGeoreference = ResolveGeoreference();
        if (targetGeoreference == null)
        {
            Debug.LogWarning("No CesiumGeoreference is available for the imported IFC model.");
            return false;
        }

        targetGeoreference.SetOriginLongitudeLatitudeHeight(
            longitude,
            latitude,
            elevation);

        PlaceModelAtCoordinates(
            modelRoot,
            targetGeoreference,
            longitude,
            latitude,
            elevation,
            geoMetadata.Properties);

        LastLatitude = latitude;
        LastLongitude = longitude;
        LastElevation = elevation;

        Debug.Log(
            $"Applied IFC geographic position from {geoSource}: " +
            $"latitude {latitude:F8}, longitude {longitude:F8}, " +
            $"elevation {elevation:F3} m.");
        GeoPositionApplied?.Invoke(modelRoot, latitude, longitude, elevation);
        return true;
    }

    public static bool TryReadGeoPosition(
        IReadOnlyDictionary<string, string> properties,
        out double latitude,
        out double longitude,
        out double elevation)
    {
        latitude = 0d;
        longitude = 0d;
        elevation = 0d;

        if (properties == null ||
            !TryGetProperty(properties, LatitudeKeys, out var latitudeText) ||
            !TryGetProperty(properties, LongitudeKeys, out var longitudeText) ||
            !TryParseAngle(latitudeText, out latitude) ||
            !TryParseAngle(longitudeText, out longitude) ||
            latitude is < -90d or > 90d ||
            longitude is < -180d or > 180d)
        {
            return false;
        }

        if (TryGetProperty(properties, ElevationKeys, out var elevationText))
        {
            TryParseNumber(elevationText, out elevation);
        }

        return double.IsFinite(elevation);
    }

    public static bool TryReadLocalOriginGeoPosition(
        IReadOnlyDictionary<string, string> properties,
        out double latitude,
        out double longitude,
        out double elevation)
    {
        latitude = 0d;
        longitude = 0d;
        elevation = 0d;

        if (properties == null ||
            !TryGetProperty(properties, LocalOriginKeys, out var originText))
        {
            return false;
        }

        var matches = NumberPattern.Matches(originText);
        if (matches.Count < 3 ||
            !TryParseNumber(matches[0].Value, out var first) ||
            !TryParseNumber(matches[1].Value, out var second) ||
            !TryParseNumber(matches[2].Value, out var third))
        {
            return false;
        }

        // IFC local origins are X/Y/Z. For geographic values this normally
        // means longitude/latitude/elevation, but accept the common reversed
        // latitude/longitude order when only the second value can be longitude.
        if (Math.Abs(first) <= 90d &&
            Math.Abs(second) > 90d &&
            Math.Abs(second) <= 180d)
        {
            latitude = first;
            longitude = second;
        }
        else
        {
            longitude = first;
            latitude = second;
        }

        if (latitude is < -90d or > 90d ||
            longitude is < -180d or > 180d)
        {
            return false;
        }

        var metresPerUnit = 1d;
        if (TryGetProperty(properties, LengthScaleKeys, out var scaleText) &&
            (!TryParseNumber(scaleText, out metresPerUnit) ||
             metresPerUnit <= 0d))
        {
            return false;
        }

        elevation = third * metresPerUnit;
        return double.IsFinite(latitude) &&
               double.IsFinite(longitude) &&
               double.IsFinite(elevation);
    }

    public static bool TryReadVn2000LocalOriginGeoPosition(
        IReadOnlyDictionary<string, string> properties,
        double centralMeridianDegrees,
        double scaleFactor,
        out double latitude,
        out double longitude,
        out double elevation)
    {
        latitude = 0d;
        longitude = 0d;
        elevation = 0d;
        if (properties == null ||
            !TryGetProperty(properties, LocalOriginKeys, out var originText))
        {
            return false;
        }

        var matches = NumberPattern.Matches(originText);
        if (matches.Count < 3 ||
            !TryParseNumber(matches[0].Value, out var easting) ||
            !TryParseNumber(matches[1].Value, out var northing) ||
            !TryParseNumber(matches[2].Value, out var localElevation))
        {
            return false;
        }

        var metresPerUnit = 1d;
        if (TryGetProperty(properties, LengthScaleKeys, out var scaleText) &&
            (!TryParseNumber(scaleText, out metresPerUnit) ||
             metresPerUnit <= 0d))
        {
            return false;
        }

        easting *= metresPerUnit;
        northing *= metresPerUnit;
        elevation = localElevation * metresPerUnit;
        return Vn2000CoordinateConverter.TryConvertToWgs84(
                   easting,
                   northing,
                   centralMeridianDegrees,
                   scaleFactor,
                   out latitude,
                   out longitude) &&
               double.IsFinite(elevation);
    }

    public static bool TryParseAngle(string value, out double degrees)
    {
        degrees = 0d;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = NormalizeAngleSeparators(value);
        var matches = NumberPattern.Matches(normalized);
        if (matches.Count == 0)
        {
            return false;
        }

        var parts = new List<double>(matches.Count);
        foreach (Match match in matches)
        {
            if (!TryParseNumber(match.Value, out var part))
            {
                return false;
            }

            parts.Add(part);
        }

        if (parts.Count == 1)
        {
            degrees = parts[0];
        }
        else
        {
            var minutes = Math.Abs(parts[1]);
            var seconds = parts.Count > 2 ? Math.Abs(parts[2]) : 0d;
            if (parts.Count > 3)
            {
                seconds += Math.Abs(parts[3]) / 1_000_000d;
            }

            if (minutes >= 60d || seconds >= 60d)
            {
                return false;
            }

            var sign = parts[0] < 0d ||
                       matches[0].Value.TrimStart().StartsWith("-", StringComparison.Ordinal)
                ? -1d
                : 1d;
            degrees = sign * (Math.Abs(parts[0]) + minutes / 60d + seconds / 3600d);
        }

        var hemisphereMatch = HemispherePattern.Match(value);
        if (hemisphereMatch.Success &&
            (hemisphereMatch.Groups[1].Value.Equals("S", StringComparison.OrdinalIgnoreCase) ||
             hemisphereMatch.Groups[1].Value.Equals("W", StringComparison.OrdinalIgnoreCase)))
        {
            degrees = -Math.Abs(degrees);
        }
        else if (hemisphereMatch.Success)
        {
            degrees = Math.Abs(degrees);
        }

        return double.IsFinite(degrees);
    }

    private void HandleLoadCompleted(GameObject modelRoot)
    {
        TryApply(modelRoot);
    }

    private void ResolveLoader()
    {
        if (loader == null)
        {
            loader = GetComponent<XbimIfcLoader>();
        }
    }

    private CesiumGeoreference ResolveGeoreference()
    {
        if (georeference != null)
        {
            return georeference;
        }

        georeference = FindFirstObjectByType<CesiumGeoreference>();
        if (georeference == null && createGeoreferenceIfMissing)
        {
            var georeferenceObject = new GameObject("CesiumGeoreference");
            georeference = georeferenceObject.AddComponent<CesiumGeoreference>();
        }

        return georeference;
    }

    private void PlaceModelAtCoordinates(
        GameObject modelRoot,
        CesiumGeoreference targetGeoreference,
        double longitude,
        double latitude,
        double elevation,
        IReadOnlyDictionary<string, string> properties)
    {
        var modelTransform = modelRoot.transform;
        var mapScale = 1d;
        var mapYawDegrees = 0d;
        TryReadMapTransform(properties, out mapScale, out mapYawDegrees);

        if (!modelTransform.IsChildOf(targetGeoreference.transform))
        {
            var modelScale = modelTransform.localScale;
            modelTransform.SetParent(targetGeoreference.transform, false);
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localScale = modelScale * (float)mapScale;
        }

        modelTransform.localRotation = Quaternion.Euler(
            0f,
            (float)-mapYawDegrees,
            0f);

        if (!attachGlobeAnchor)
        {
            return;
        }

        var anchor = modelRoot.GetComponent<CesiumGlobeAnchor>();
        if (anchor == null)
        {
            anchor = modelRoot.AddComponent<CesiumGlobeAnchor>();
        }

        anchor.longitudeLatitudeHeight = new double3(longitude, latitude, elevation);
    }

    private bool TryFindGeoMetadata(
        GameObject modelRoot,
        out IfcMetadataComponent geoMetadata,
        out string geoSource,
        out double latitude,
        out double longitude,
        out double elevation)
    {
        var metadataComponents =
            modelRoot.GetComponentsInChildren<IfcMetadataComponent>(true);

        foreach (var metadata in metadataComponents)
        {
            if (metadata.IfcType?.Contains(
                    "IfcSite",
                    StringComparison.OrdinalIgnoreCase) != true ||
                !TryReadGeoPosition(
                    metadata.Properties,
                    out latitude,
                    out longitude,
                    out elevation))
            {
                continue;
            }

            geoMetadata = metadata;
            geoSource = "IfcSite";
            return true;
        }

        foreach (var metadata in metadataComponents)
        {
            if (!TryReadGeoPosition(
                    metadata.Properties,
                    out latitude,
                    out longitude,
                    out elevation))
            {
                continue;
            }

            geoMetadata = metadata;
            geoSource = $"{metadata.IfcType} metadata";
            return true;
        }

        foreach (var metadata in metadataComponents)
        {
            if (!TryReadLocalOriginGeoPosition(
                    metadata.Properties,
                    out latitude,
                    out longitude,
                    out elevation))
            {
                continue;
            }

            geoMetadata = metadata;
            geoSource = "Local Origin (IFC coordinates)";
            return true;
        }

        if (useVn2000LocalOriginFallback)
        {
            foreach (var metadata in metadataComponents)
            {
                if (!TryReadVn2000LocalOriginGeoPosition(
                        metadata.Properties,
                        vn2000CentralMeridianDegrees,
                        vn2000ScaleFactor,
                        out latitude,
                        out longitude,
                        out elevation))
                {
                    continue;
                }

                geoMetadata = metadata;
                geoSource =
                    $"VN2000 local origin ({vn2000CentralMeridianDegrees:F2}° central meridian)";
                return true;
            }
        }

        geoMetadata = null;
        geoSource = string.Empty;
        latitude = 0d;
        longitude = 0d;
        elevation = 0d;
        return false;
    }

    private static bool TryReadMapTransform(
        IReadOnlyDictionary<string, string> properties,
        out double scale,
        out double yawDegrees)
    {
        scale = 1d;
        yawDegrees = 0d;
        if (!TryGetProperty(
                properties,
                new[] { "MapConversion/XAxisAbscissa", "XAxisAbscissa" },
                out var abscissaText) ||
            !TryGetProperty(
                properties,
                new[] { "MapConversion/XAxisOrdinate", "XAxisOrdinate" },
                out var ordinateText) ||
            !TryParseNumber(abscissaText, out var abscissa) ||
            !TryParseNumber(ordinateText, out var ordinate))
        {
            return false;
        }

        var length = Math.Sqrt(abscissa * abscissa + ordinate * ordinate);
        if (!double.IsFinite(length) || length <= 1e-12d)
        {
            return false;
        }

        yawDegrees = Math.Atan2(ordinate, abscissa) * 180d / Math.PI;
        if (TryGetProperty(
                properties,
                new[] { "MapConversion/Scale", "MapScale" },
                out var scaleText) &&
            TryParseNumber(scaleText, out var parsedScale) &&
            parsedScale > 0d)
        {
            scale = parsedScale;
        }

        return double.IsFinite(yawDegrees) && double.IsFinite(scale);
    }

    private static bool TryGetProperty(
        IReadOnlyDictionary<string, string> properties,
        IEnumerable<string> candidateKeys,
        out string value)
    {
        foreach (var candidate in candidateKeys)
        {
            var normalizedCandidate = NormalizeKey(candidate);
            foreach (var property in properties)
            {
                var normalizedKey = NormalizeKey(property.Key);
                if ((normalizedKey.Equals(
                         normalizedCandidate,
                         StringComparison.OrdinalIgnoreCase) ||
                     normalizedKey.EndsWith(
                         normalizedCandidate,
                         StringComparison.OrdinalIgnoreCase)) &&
                    !string.IsNullOrWhiteSpace(property.Value))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = string.Empty;
        return false;
    }

    private static bool TryParseNumber(string value, out double number)
    {
        number = 0d;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().Replace(',', '.');
        if (double.TryParse(
                normalized,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out number))
        {
            return double.IsFinite(number);
        }

        var match = NumberPattern.Match(value);
        return match.Success &&
               double.TryParse(
                   match.Value.Replace(',', '.'),
                   NumberStyles.Float,
                   CultureInfo.InvariantCulture,
                   out number) &&
               double.IsFinite(number);
    }

    private static string NormalizeAngleSeparators(string value)
    {
        var commaCount = 0;
        var semicolonCount = 0;
        foreach (var character in value)
        {
            if (character == ',')
            {
                commaCount++;
            }
            else if (character == ';')
            {
                semicolonCount++;
            }
        }

        return commaCount >= 2 || semicolonCount > 0
            ? value.Replace(',', ' ').Replace(';', ' ')
            : value;
    }

    private static string NormalizeKey(string key)
    {
        var normalized = new StringBuilder(key.Length);
        foreach (var character in key)
        {
            if (char.IsLetterOrDigit(character))
            {
                normalized.Append(char.ToUpperInvariant(character));
            }
        }

        return normalized.ToString();
    }
}
