using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public sealed partial class IfcOperationsDatabase
{
    public bool GetModelRegistry(ICollection<IfcModelRegistryRecord> destination)
    {
        if (!IsAvailable || destination == null)
        {
            return false;
        }

        destination.Clear();
        const string sql =
            "SELECT id, project_name, province, ward, managing_unit, ifc_path, " +
            "is_enabled, created_at, updated_at FROM ifc_model_registry " +
            "ORDER BY updated_at DESC, id DESC;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            while (sqlite3_step(statement) == SqliteRow)
            {
                destination.Add(new IfcModelRegistryRecord(
                    sqlite3_column_int64(statement, 0),
                    ReadColumnText(statement, 1),
                    ReadColumnText(statement, 2),
                    ReadColumnText(statement, 3),
                    ReadColumnText(statement, 4),
                    ReadColumnText(statement, 5),
                    sqlite3_column_int(statement, 6) != 0,
                    ReadColumnText(statement, 7),
                    ReadColumnText(statement, 8)));
            }

            return true;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool SaveModelRegistry(IfcModelRegistryRecord record)
    {
        if (!IsAvailable || string.IsNullOrWhiteSpace(record.IfcPath))
        {
            return false;
        }

        const string sql =
            "INSERT INTO ifc_model_registry " +
            "(project_name, province, ward, managing_unit, ifc_path, is_enabled, " +
            "created_at, updated_at) VALUES (?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8) " +
            "ON CONFLICT(ifc_path) DO UPDATE SET " +
            "project_name = excluded.project_name, province = excluded.province, " +
            "ward = excluded.ward, managing_unit = excluded.managing_unit, " +
            "is_enabled = excluded.is_enabled, updated_at = excluded.updated_at;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        var now = DateTime.Now.ToString("o");
        try
        {
            BindText(statement, 1, record.ProjectName);
            BindText(statement, 2, record.Province);
            BindText(statement, 3, record.Ward);
            BindText(statement, 4, record.ManagingUnit);
            BindText(statement, 5, record.IfcPath);
            sqlite3_bind_int(statement, 6, record.IsEnabled ? 1 : 0);
            BindText(statement, 7, string.IsNullOrWhiteSpace(record.CreatedAt)
                ? now
                : record.CreatedAt);
            BindText(statement, 8, now);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool SetModelEnabled(long id, bool enabled)
    {
        if (!IsAvailable)
        {
            return false;
        }

        const string sql =
            "UPDATE ifc_model_registry SET is_enabled = ?1, updated_at = ?2 " +
            "WHERE id = ?3;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            sqlite3_bind_int(statement, 1, enabled ? 1 : 0);
            BindText(statement, 2, DateTime.Now.ToString("o"));
            sqlite3_bind_int64(statement, 3, id);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool DeleteModelRegistry(long id)
    {
        if (!IsAvailable)
        {
            return false;
        }

        const string sql = "DELETE FROM ifc_model_registry WHERE id = ?1;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            sqlite3_bind_int64(statement, 1, id);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool GetFieldInspections(ICollection<FieldInspectionRecord> destination)
    {
        if (!IsAvailable || destination == null)
        {
            return false;
        }

        destination.Clear();
        const string sql =
            "SELECT id, project_name, source_file, element_key, element_name, " +
            "latitude, longitude, elevation, image_path, note, created_at " +
            "FROM field_inspections ORDER BY created_at DESC, id DESC;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            while (sqlite3_step(statement) == SqliteRow)
            {
                destination.Add(new FieldInspectionRecord(
                    sqlite3_column_int64(statement, 0),
                    ReadColumnText(statement, 1),
                    ReadColumnText(statement, 2),
                    ReadColumnText(statement, 3),
                    ReadColumnText(statement, 4),
                    sqlite3_column_double(statement, 5),
                    sqlite3_column_double(statement, 6),
                    sqlite3_column_double(statement, 7),
                    ReadColumnText(statement, 8),
                    ReadColumnText(statement, 9),
                    ReadColumnText(statement, 10)));
            }

            return true;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool SaveFieldInspection(
        FieldInspectionRecord record,
        out long inspectionId)
    {
        inspectionId = 0;
        if (!IsAvailable || string.IsNullOrWhiteSpace(record.ElementName) ||
            !double.IsFinite(record.Latitude) ||
            !double.IsFinite(record.Longitude))
        {
            return false;
        }

        const string sql =
            "INSERT INTO field_inspections " +
            "(project_name, source_file, element_key, element_name, latitude, " +
            "longitude, elevation, image_path, note, created_at) " +
            "VALUES (?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8, ?9, ?10);";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, record.ProjectName);
            BindText(statement, 2, record.SourceFile);
            BindText(statement, 3, record.ElementKey);
            BindText(statement, 4, record.ElementName);
            sqlite3_bind_double(statement, 5, record.Latitude);
            sqlite3_bind_double(statement, 6, record.Longitude);
            sqlite3_bind_double(statement, 7, record.Elevation);
            BindText(statement, 8, record.ImagePath);
            BindText(statement, 9, record.Note);
            BindText(statement, 10, string.IsNullOrWhiteSpace(record.CreatedAt)
                ? DateTime.Now.ToString("o")
                : record.CreatedAt);
            if (sqlite3_step(statement) != SqliteDone)
            {
                return false;
            }

            inspectionId = sqlite3_last_insert_rowid(database);
            return true;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool DeleteFieldInspection(long inspectionId)
    {
        if (!IsAvailable || inspectionId <= 0)
        {
            return false;
        }

        const string sql = "DELETE FROM field_inspections WHERE id = ?1;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            sqlite3_bind_int64(statement, 1, inspectionId);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    private void EnsureModuleSchema()
    {
        Execute(
            "CREATE TABLE IF NOT EXISTS ifc_model_registry (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT, project_name TEXT NOT NULL, " +
            "province TEXT NOT NULL, ward TEXT NOT NULL, managing_unit TEXT NOT NULL, " +
            "ifc_path TEXT NOT NULL COLLATE NOCASE UNIQUE, is_enabled INTEGER NOT NULL, " +
            "created_at TEXT NOT NULL, updated_at TEXT NOT NULL);");
        Execute(
            "CREATE TABLE IF NOT EXISTS field_inspections (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT, project_name TEXT NOT NULL, " +
            "source_file TEXT NOT NULL, element_key TEXT NOT NULL, " +
            "element_name TEXT NOT NULL, latitude REAL NOT NULL, longitude REAL NOT NULL, " +
            "elevation REAL NOT NULL, image_path TEXT NOT NULL, note TEXT NOT NULL, " +
            "created_at TEXT NOT NULL);");
        Execute(
            "CREATE INDEX IF NOT EXISTS idx_ifc_registry_filters ON " +
            "ifc_model_registry(project_name, province, ward);");
        Execute(
            "CREATE INDEX IF NOT EXISTS idx_field_inspection_element ON " +
            "field_inspections(source_file, element_key);");
    }

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_bind_int64(IntPtr statement, int index, long value);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_bind_double(IntPtr statement, int index, double value);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern long sqlite3_column_int64(IntPtr statement, int index);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern double sqlite3_column_double(IntPtr statement, int index);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern long sqlite3_last_insert_rowid(IntPtr database);
}

public readonly struct IfcModelRegistryRecord
{
    public long Id { get; }
    public string ProjectName { get; }
    public string Province { get; }
    public string Ward { get; }
    public string ManagingUnit { get; }
    public string IfcPath { get; }
    public bool IsEnabled { get; }
    public string CreatedAt { get; }
    public string UpdatedAt { get; }

    public IfcModelRegistryRecord(
        long id,
        string projectName,
        string province,
        string ward,
        string managingUnit,
        string ifcPath,
        bool isEnabled,
        string createdAt = "",
        string updatedAt = "")
    {
        Id = id;
        ProjectName = projectName ?? string.Empty;
        Province = province ?? string.Empty;
        Ward = ward ?? string.Empty;
        ManagingUnit = managingUnit ?? string.Empty;
        IfcPath = ifcPath ?? string.Empty;
        IsEnabled = isEnabled;
        CreatedAt = createdAt ?? string.Empty;
        UpdatedAt = updatedAt ?? string.Empty;
    }
}

public readonly struct FieldInspectionRecord
{
    public long Id { get; }
    public string ProjectName { get; }
    public string SourceFile { get; }
    public string ElementKey { get; }
    public string ElementName { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public double Elevation { get; }
    public string ImagePath { get; }
    public string Note { get; }
    public string CreatedAt { get; }

    public FieldInspectionRecord(
        long id,
        string projectName,
        string sourceFile,
        string elementKey,
        string elementName,
        double latitude,
        double longitude,
        double elevation,
        string imagePath,
        string note,
        string createdAt = "")
    {
        Id = id;
        ProjectName = projectName ?? string.Empty;
        SourceFile = sourceFile ?? string.Empty;
        ElementKey = elementKey ?? string.Empty;
        ElementName = elementName ?? string.Empty;
        Latitude = latitude;
        Longitude = longitude;
        Elevation = elevation;
        ImagePath = imagePath ?? string.Empty;
        Note = note ?? string.Empty;
        CreatedAt = createdAt ?? string.Empty;
    }
}
