using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CauDuong.IfcOperations;

public readonly struct IfcOperationsReportRow
{
    public int ExpressId { get; }
    public string Name { get; }
    public string Category { get; }
    public IfcOperationalStatus Status { get; }
    public string SourceFile { get; }

    public IfcOperationsReportRow(
        int expressId,
        string name,
        string category,
        IfcOperationalStatus status,
        string sourceFile)
    {
        ExpressId = expressId;
        Name = name ?? string.Empty;
        Category = category ?? string.Empty;
        Status = status;
        SourceFile = sourceFile ?? string.Empty;
    }
}

public static class IfcOperationsReport
{
    private const float PageWidth = 595f;
    private const float PageHeight = 842f;
    private const float Margin = 42f;
    private const float RowHeight = 34f;
    private const int FirstPageRows = 15;
    private const int FollowingPageRows = 19;

    public static byte[] BuildPdf(
        string projectName,
        DateTime generatedAt,
        int modelCount,
        IReadOnlyList<IfcOperationsReportRow> rows)
    {
        var safeRows = rows ?? Array.Empty<IfcOperationsReportRow>();
        var pages = BuildPageStreams(projectName, generatedAt, modelCount, safeRows);
        return BuildDocument(pages);
    }

    private static List<string> BuildPageStreams(
        string projectName,
        DateTime generatedAt,
        int modelCount,
        IReadOnlyList<IfcOperationsReportRow> rows)
    {
        var pages = new List<string>();
        var rowIndex = 0;
        var pageIndex = 0;

        do
        {
            var firstPage = pageIndex == 0;
            var capacity = firstPage ? FirstPageRows : FollowingPageRows;
            var take = Math.Min(capacity, rows.Count - rowIndex);
            var content = new StringBuilder(8192);
            DrawPageHeader(content, projectName, generatedAt, modelCount, rows, pageIndex + 1);

            var tableTop = firstPage ? 662f : 738f;
            DrawTableHeader(content, tableTop);
            for (var offset = 0; offset < take; offset++)
            {
                DrawTableRow(
                    content,
                    tableTop - 30f - RowHeight * (offset + 1),
                    rowIndex + offset + 1,
                    rows[rowIndex + offset]);
            }

            pages.Add(content.ToString());
            rowIndex += take;
            pageIndex++;
        }
        while (rowIndex < rows.Count || pages.Count == 0);

        return pages;
    }

    private static void DrawPageHeader(
        StringBuilder content,
        string projectName,
        DateTime generatedAt,
        int modelCount,
        IReadOnlyList<IfcOperationsReportRow> rows,
        int pageNumber)
    {
        DrawText(content, 8f, Margin, 815f, generatedAt.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture));
        DrawText(content, 8f, 430f, 815f, $"BIM-GIS InfraOps | Trang {pageNumber}");
        DrawText(content, 18f, Margin, 778f, "BAO CAO VAN HANH & BAO TRI HA TANG GIAO THONG", true);
        DrawText(content, 15f, Margin, 754f, ToAscii(projectName), true);

        if (pageNumber != 1)
        {
            return;
        }

        DrawText(content, 9f, Margin, 728f, $"Ngay lap: {generatedAt:dd/MM/yyyy HH:mm:ss} | Mo hinh IFC: {modelCount:N0} | Tong cau kien: {rows.Count:N0}");
        var statusX = Margin;
        foreach (IfcOperationalStatus status in Enum.GetValues(typeof(IfcOperationalStatus)))
        {
            var count = rows.Count(row => row.Status == status);
            SetFillColor(content, StatusColor(status));
            content.AppendFormat(CultureInfo.InvariantCulture, "{0:F1} 684 10 10 re f\n", statusX);
            SetFillColor(content, "0 0 0");
            DrawText(content, 8f, statusX + 14f, 685f, $"{StatusLabel(status)}: {count:N0}");
            statusX += 130f;
        }
    }

    private static void DrawTableHeader(StringBuilder content, float top)
    {
        SetFillColor(content, "0.91 0.94 0.98");
        content.AppendFormat(CultureInfo.InvariantCulture, "{0:F1} {1:F1} {2:F1} 30 re f\n", Margin, top - 30f, PageWidth - Margin * 2f);
        SetStrokeColor(content, "0.72 0.77 0.84");
        DrawTableGrid(content, top - 30f, 30f);
        SetFillColor(content, "0 0 0");
        DrawText(content, 9f, 49f, top - 19f, "STT", true);
        DrawText(content, 9f, 84f, top - 19f, "MA ID", true);
        DrawText(content, 9f, 150f, top - 19f, "TEN CAU KIEN", true);
        DrawText(content, 9f, 365f, top - 19f, "LOP HA TANG", true);
        DrawText(content, 9f, 491f, top - 19f, "TRANG THAI", true);
    }

    private static void DrawTableRow(StringBuilder content, float bottom, int ordinal, IfcOperationsReportRow row)
    {
        SetStrokeColor(content, "0.82 0.85 0.89");
        DrawTableGrid(content, bottom, RowHeight);
        DrawText(content, 8f, 52f, bottom + 13f, ordinal.ToString(CultureInfo.InvariantCulture));
        DrawText(content, 8f, 82f, bottom + 13f, $"#{row.ExpressId}");

        var nameLines = Wrap(ToAscii(row.Name), 34, 2);
        for (var index = 0; index < nameLines.Count; index++)
        {
            DrawText(content, 7.5f, 142f, bottom + 20f - index * 10f, nameLines[index]);
        }

        var categoryLines = Wrap(ToAscii(row.Category), 20, 2);
        for (var index = 0; index < categoryLines.Count; index++)
        {
            DrawText(content, 7.5f, 354f, bottom + 20f - index * 10f, categoryLines[index]);
        }

        SetFillColor(content, StatusColor(row.Status));
        content.AppendFormat(CultureInfo.InvariantCulture, "482 {0:F1} 7 7 re f\n", bottom + 13f);
        SetFillColor(content, "0 0 0");
        DrawText(content, 7.5f, 493f, bottom + 13f, StatusLabel(row.Status));
    }

    private static void DrawTableGrid(StringBuilder content, float bottom, float height)
    {
        var columns = new[] { Margin, 75f, 135f, 347f, 475f, PageWidth - Margin };
        content.AppendFormat(CultureInfo.InvariantCulture, "{0:F1} {1:F1} {2:F1} {3:F1} re S\n", Margin, bottom, PageWidth - Margin * 2f, height);
        for (var index = 1; index < columns.Length - 1; index++)
        {
            content.AppendFormat(CultureInfo.InvariantCulture, "{0:F1} {1:F1} m {0:F1} {2:F1} l S\n", columns[index], bottom, bottom + height);
        }
    }

    private static void DrawText(StringBuilder content, float size, float x, float y, string value, bool bold = false)
    {
        content.Append("BT\n");
        content.AppendFormat(CultureInfo.InvariantCulture, "/{0} {1:F1} Tf\n", bold ? "F2" : "F1", size);
        content.AppendFormat(CultureInfo.InvariantCulture, "{0:F1} {1:F1} Td\n", x, y);
        content.Append('(').Append(EscapePdfText(ToAscii(value))).Append(") Tj\nET\n");
    }

    private static void SetFillColor(StringBuilder content, string rgb)
    {
        content.Append(rgb).Append(" rg\n");
    }

    private static void SetStrokeColor(StringBuilder content, string rgb)
    {
        content.Append(rgb).Append(" RG\n");
    }

    private static string StatusColor(IfcOperationalStatus status)
    {
        return status switch
        {
            IfcOperationalStatus.Operational => "0.10 0.68 0.43",
            IfcOperationalStatus.Warning => "0.91 0.63 0.10",
            IfcOperationalStatus.Critical => "0.83 0.16 0.24",
            IfcOperationalStatus.Repairing => "0.13 0.43 0.86",
            _ => "0.45 0.49 0.56"
        };
    }

    private static string StatusLabel(IfcOperationalStatus status)
    {
        return status switch
        {
            IfcOperationalStatus.Operational => "OPERATIONAL",
            IfcOperationalStatus.Warning => "WARNING",
            IfcOperationalStatus.Critical => "CRITICAL",
            IfcOperationalStatus.Repairing => "REPAIRING",
            _ => status.ToString().ToUpperInvariant()
        };
    }

    private static List<string> Wrap(string value, int maximumCharacters, int maximumLines)
    {
        var words = (value ?? string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var current = new StringBuilder();
        foreach (var word in words)
        {
            if (current.Length > 0 && current.Length + word.Length + 1 > maximumCharacters)
            {
                lines.Add(current.ToString());
                current.Clear();
                if (lines.Count == maximumLines)
                {
                    break;
                }
            }

            if (current.Length > 0)
            {
                current.Append(' ');
            }

            current.Append(word.Length > maximumCharacters ? word.Substring(0, maximumCharacters) : word);
        }

        if (lines.Count < maximumLines && current.Length > 0)
        {
            lines.Add(current.ToString());
        }

        if (lines.Count == 0)
        {
            lines.Add(string.Empty);
        }

        return lines;
    }

    private static byte[] BuildDocument(IReadOnlyList<string> pageStreams)
    {
        var pageCount = pageStreams.Count;
        var objects = new List<string>(4 + pageCount * 2)
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            string.Empty,
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"
        };

        var pageReferences = new StringBuilder();
        for (var pageIndex = 0; pageIndex < pageCount; pageIndex++)
        {
            var pageObjectId = 5 + pageIndex * 2;
            var streamObjectId = pageObjectId + 1;
            pageReferences.Append(pageObjectId).Append(" 0 R ");
            objects.Add(
                $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth:F0} {PageHeight:F0}] " +
                $"/Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents {streamObjectId} 0 R >>");
            var stream = pageStreams[pageIndex];
            objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}endstream");
        }

        objects[1] = $"<< /Type /Pages /Kids [{pageReferences}] /Count {pageCount} >>";
        using var output = new MemoryStream();
        WriteAscii(output, "%PDF-1.4\n");
        var offsets = new long[objects.Count + 1];
        for (var index = 0; index < objects.Count; index++)
        {
            offsets[index + 1] = output.Position;
            WriteAscii(output, $"{index + 1} 0 obj\n{objects[index]}\nendobj\n");
        }

        var xrefOffset = output.Position;
        WriteAscii(output, $"xref\n0 {objects.Count + 1}\n");
        WriteAscii(output, "0000000000 65535 f \n");
        for (var index = 1; index < offsets.Length; index++)
        {
            WriteAscii(output, $"{offsets[index]:D10} 00000 n \n");
        }

        WriteAscii(output, $"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF");
        return output.ToArray();
    }

    private static void WriteAscii(Stream stream, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static string EscapePdfText(string value)
    {
        return (value ?? string.Empty)
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }

    private static string ToAscii(string value)
    {
        var normalized = (value ?? string.Empty).Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                result.Append(character switch
                {
                    'Đ' => 'D',
                    'đ' => 'd',
                    _ => character <= 127 ? character : '?'
                });
            }
        }

        return result.ToString().Normalize(NormalizationForm.FormC);
    }
}
