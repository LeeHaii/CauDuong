using System;

public static class Vn2000CoordinateConverter
{
    public static bool TryConvertToWgs84(
        double easting,
        double northing,
        double centralMeridianDegrees,
        double scaleFactor,
        out double latitude,
        out double longitude)
    {
        latitude = 0d;
        longitude = 0d;
        if (!double.IsFinite(easting) ||
            !double.IsFinite(northing) ||
            !double.IsFinite(centralMeridianDegrees) ||
            !double.IsFinite(scaleFactor) ||
            easting is < 100_000d or > 900_000d ||
            northing is < 0d or > 3_000_000d ||
            centralMeridianDegrees is < 100d or > 110d ||
            scaleFactor is < 0.99d or > 1.01d)
        {
            return false;
        }

        InverseTransverseMercator(
            easting,
            northing,
            centralMeridianDegrees,
            scaleFactor,
            500_000d,
            0d,
            out latitude,
            out longitude);
        TransformVn2000ToWgs84(ref latitude, ref longitude);

        return double.IsFinite(latitude) &&
               double.IsFinite(longitude) &&
               latitude is >= 8d and <= 24d &&
               longitude is >= 102d and <= 110d;
    }

    private static void InverseTransverseMercator(
        double easting,
        double northing,
        double centralMeridianDegrees,
        double scaleFactor,
        double falseEasting,
        double falseNorthing,
        out double latitudeDegrees,
        out double longitudeDegrees)
    {
        const double semiMajorAxis = 6_378_137d;
        const double inverseFlattening = 298.257_223_563d;
        var flattening = 1d / inverseFlattening;
        var eccentricitySquared = flattening * (2d - flattening);
        var secondEccentricitySquared =
            eccentricitySquared / (1d - eccentricitySquared);
        var eccentricityFourth = eccentricitySquared * eccentricitySquared;
        var eccentricitySixth = eccentricityFourth * eccentricitySquared;

        var meridionalArc = (northing - falseNorthing) / scaleFactor;
        var mu = meridionalArc /
                 (semiMajorAxis *
                  (1d -
                   eccentricitySquared / 4d -
                   3d * eccentricityFourth / 64d -
                   5d * eccentricitySixth / 256d));
        var e1 =
            (1d - Math.Sqrt(1d - eccentricitySquared)) /
            (1d + Math.Sqrt(1d - eccentricitySquared));
        var e1Squared = e1 * e1;
        var e1Cubed = e1Squared * e1;
        var e1Fourth = e1Squared * e1Squared;
        var footprintLatitude =
            mu +
            (3d * e1 / 2d - 27d * e1Cubed / 32d) * Math.Sin(2d * mu) +
            (21d * e1Squared / 16d - 55d * e1Fourth / 32d) * Math.Sin(4d * mu) +
            151d * e1Cubed / 96d * Math.Sin(6d * mu) +
            1_097d * e1Fourth / 512d * Math.Sin(8d * mu);

        var sine = Math.Sin(footprintLatitude);
        var cosine = Math.Cos(footprintLatitude);
        var tangent = Math.Tan(footprintLatitude);
        var tangentSquared = tangent * tangent;
        var c1 = secondEccentricitySquared * cosine * cosine;
        var n1 = semiMajorAxis /
                 Math.Sqrt(1d - eccentricitySquared * sine * sine);
        var r1 =
            semiMajorAxis * (1d - eccentricitySquared) /
            Math.Pow(1d - eccentricitySquared * sine * sine, 1.5d);
        var d = (easting - falseEasting) / (n1 * scaleFactor);
        var d2 = d * d;
        var d3 = d2 * d;
        var d4 = d2 * d2;
        var d5 = d4 * d;
        var d6 = d3 * d3;

        var latitude =
            footprintLatitude -
            n1 * tangent / r1 *
            (d2 / 2d -
             (5d + 3d * tangentSquared + 10d * c1 -
              4d * c1 * c1 - 9d * secondEccentricitySquared) *
             d4 / 24d +
             (61d + 90d * tangentSquared +
              298d * c1 + 45d * tangentSquared * tangentSquared -
              252d * secondEccentricitySquared -
              3d * c1 * c1) *
             d6 / 720d);
        var longitude =
            centralMeridianDegrees * Math.PI / 180d +
            (d -
             (1d + 2d * tangentSquared + c1) * d3 / 6d +
             (5d - 2d * c1 + 28d * tangentSquared -
              3d * c1 * c1 +
              8d * secondEccentricitySquared +
              24d * tangentSquared * tangentSquared) *
             d5 / 120d) /
            cosine;

        latitudeDegrees = latitude * 180d / Math.PI;
        longitudeDegrees = longitude * 180d / Math.PI;
    }

    private static void TransformVn2000ToWgs84(
        ref double latitudeDegrees,
        ref double longitudeDegrees)
    {
        const double semiMajorAxis = 6_378_137d;
        const double inverseFlattening = 298.257_223_563d;
        const double translationX = -191.904_414_29d;
        const double translationY = -39.303_182_79d;
        const double translationZ = -111.450_328_35d;
        const double rotationXArcSeconds = -0.009_288_36d;
        const double rotationYArcSeconds = 0.019_754_79d;
        const double rotationZArcSeconds = -0.004_273_72d;
        const double scalePartsPerMillion = 0.252_906_278d;

        GeodeticToEcef(
            latitudeDegrees,
            longitudeDegrees,
            0d,
            semiMajorAxis,
            inverseFlattening,
            out var sourceX,
            out var sourceY,
            out var sourceZ);

        var arcSecondsToRadians = Math.PI / (180d * 3_600d);
        var rotationX = rotationXArcSeconds * arcSecondsToRadians;
        var rotationY = rotationYArcSeconds * arcSecondsToRadians;
        var rotationZ = rotationZArcSeconds * arcSecondsToRadians;
        var scale = 1d + scalePartsPerMillion * 1e-6d;

        var targetX =
            translationX +
            scale * (sourceX + rotationZ * sourceY - rotationY * sourceZ);
        var targetY =
            translationY +
            scale * (-rotationZ * sourceX + sourceY + rotationX * sourceZ);
        var targetZ =
            translationZ +
            scale * (rotationY * sourceX - rotationX * sourceY + sourceZ);

        EcefToGeodetic(
            targetX,
            targetY,
            targetZ,
            semiMajorAxis,
            inverseFlattening,
            out latitudeDegrees,
            out longitudeDegrees);
    }

    private static void GeodeticToEcef(
        double latitudeDegrees,
        double longitudeDegrees,
        double height,
        double semiMajorAxis,
        double inverseFlattening,
        out double x,
        out double y,
        out double z)
    {
        var flattening = 1d / inverseFlattening;
        var eccentricitySquared = flattening * (2d - flattening);
        var latitude = latitudeDegrees * Math.PI / 180d;
        var longitude = longitudeDegrees * Math.PI / 180d;
        var sineLatitude = Math.Sin(latitude);
        var primeVerticalRadius =
            semiMajorAxis /
            Math.Sqrt(1d - eccentricitySquared * sineLatitude * sineLatitude);

        x = (primeVerticalRadius + height) *
            Math.Cos(latitude) *
            Math.Cos(longitude);
        y = (primeVerticalRadius + height) *
            Math.Cos(latitude) *
            Math.Sin(longitude);
        z = (primeVerticalRadius * (1d - eccentricitySquared) + height) *
            sineLatitude;
    }

    private static void EcefToGeodetic(
        double x,
        double y,
        double z,
        double semiMajorAxis,
        double inverseFlattening,
        out double latitudeDegrees,
        out double longitudeDegrees)
    {
        var flattening = 1d / inverseFlattening;
        var eccentricitySquared = flattening * (2d - flattening);
        var longitude = Math.Atan2(y, x);
        var horizontal = Math.Sqrt(x * x + y * y);
        var latitude = Math.Atan2(
            z,
            horizontal * (1d - eccentricitySquared));

        for (var iteration = 0; iteration < 8; iteration++)
        {
            var sine = Math.Sin(latitude);
            var primeVerticalRadius =
                semiMajorAxis /
                Math.Sqrt(1d - eccentricitySquared * sine * sine);
            var height = horizontal / Math.Cos(latitude) - primeVerticalRadius;
            latitude = Math.Atan2(
                z,
                horizontal *
                (1d -
                 eccentricitySquared *
                 primeVerticalRadius /
                 (primeVerticalRadius + height)));
        }

        latitudeDegrees = latitude * 180d / Math.PI;
        longitudeDegrees = longitude * 180d / Math.PI;
    }
}
