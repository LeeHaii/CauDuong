using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace CauDuong.IfcOperations
{
    public enum IfcInfrastructureCategory
    {
        TrafficSafety,
        Pavement,
        Barrier,
        Foundation,
        SlopeAndLandscape,
        RouteInfrastructure
    }

    public sealed class IfcCategoryDefinition
    {
        public IfcInfrastructureCategory Category { get; }
        public string Symbol { get; }
        public string DisplayName { get; }
        public Color AccentColor { get; }

        public IfcCategoryDefinition(
            IfcInfrastructureCategory category,
            string symbol,
            string displayName,
            Color accentColor)
        {
            Category = category;
            Symbol = symbol;
            DisplayName = displayName;
            AccentColor = accentColor;
        }
    }

    public static class IfcInfrastructureClassifier
    {
        private static readonly IReadOnlyList<IfcCategoryDefinition> definitions =
            new[]
            {
                new IfcCategoryDefinition(
                    IfcInfrastructureCategory.TrafficSafety,
                    "AT",
                    "An Toàn & Tín Hiệu Giao Thông",
                    new Color32(225, 54, 77, 255)),
                new IfcCategoryDefinition(
                    IfcInfrastructureCategory.Pavement,
                    "MD",
                    "Mặt Đường & Thảm Nhựa",
                    new Color32(42, 105, 219, 255)),
                new IfcCategoryDefinition(
                    IfcInfrastructureCategory.Barrier,
                    "HL",
                    "Hộ Lan & Dải Phân Cách",
                    new Color32(116, 90, 214, 255)),
                new IfcCategoryDefinition(
                    IfcInfrastructureCategory.Foundation,
                    "KC",
                    "Kết Cấu Móng & Bê Tông",
                    new Color32(219, 118, 47, 255)),
                new IfcCategoryDefinition(
                    IfcInfrastructureCategory.SlopeAndLandscape,
                    "TL",
                    "Mái Ta Luy & Mảng Xanh",
                    new Color32(31, 151, 102, 255)),
                new IfcCategoryDefinition(
                    IfcInfrastructureCategory.RouteInfrastructure,
                    "HT",
                    "Hạ Tầng Tuyến & Nhánh Giao Thông",
                    new Color32(20, 137, 188, 255))
            };

        private static readonly string[] safetyKeywords =
        {
            "vach", "pole", "nts", "tin hieu", "bien bao"
        };

        private static readonly string[] pavementKeywords =
        {
            "pave", "tham", "mat duong"
        };

        private static readonly string[] barrierKeywords =
        {
            "barrier", "lancan", "lan can", "ho lan", "phan cach"
        };

        private static readonly string[] foundationKeywords =
        {
            "btl", "vxm", "btxm", "mong", "be tong", "coc"
        };

        private static readonly string[] slopeKeywords =
        {
            "taluy", "ta luy", "topo", "ledat", "le dat"
        };

        public static IReadOnlyList<IfcCategoryDefinition> Definitions => definitions;

        public static IfcInfrastructureCategory Classify(string name, string ifcType)
        {
            var searchableText = Normalize($"{name} {ifcType}");

            if (ContainsAny(searchableText, safetyKeywords))
            {
                return IfcInfrastructureCategory.TrafficSafety;
            }

            if (ContainsAny(searchableText, pavementKeywords))
            {
                return IfcInfrastructureCategory.Pavement;
            }

            if (ContainsAny(searchableText, barrierKeywords))
            {
                return IfcInfrastructureCategory.Barrier;
            }

            if (ContainsAny(searchableText, foundationKeywords))
            {
                return IfcInfrastructureCategory.Foundation;
            }

            if (ContainsAny(searchableText, slopeKeywords))
            {
                return IfcInfrastructureCategory.SlopeAndLandscape;
            }

            return IfcInfrastructureCategory.RouteInfrastructure;
        }

        public static IfcCategoryDefinition GetDefinition(
            IfcInfrastructureCategory category)
        {
            foreach (var definition in definitions)
            {
                if (definition.Category == category)
                {
                    return definition;
                }
            }

            return definitions[definitions.Count - 1];
        }

        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var decomposed = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(decomposed.Length);

            foreach (var character in decomposed)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character == 'đ' ? 'd' : character);
                }
            }

            return builder
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace('_', ' ')
                .Replace('-', ' ');
        }

        private static bool ContainsAny(string value, IEnumerable<string> keywords)
        {
            foreach (var keyword in keywords)
            {
                if (value.Contains(keyword, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
