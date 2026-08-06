using System;
using System.Collections.Generic;
using System.IO;
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
            "is_enabled, created_at, updated_at, ifc_file_name, " +
            "CASE WHEN ifc_file_data IS NOT NULL AND length(ifc_file_data) > 0 " +
            "THEN 1 ELSE 0 END, COALESCE(length(ifc_file_data), 0) " +
            "FROM ifc_model_registry " +
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
                    ReadColumnText(statement, 8),
                    ReadColumnText(statement, 9),
                    sqlite3_column_int(statement, 10) != 0,
                    sqlite3_column_int64(statement, 11)));
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
            "created_at, updated_at, ifc_file_name) " +
            "VALUES (?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8, ?9) " +
            "ON CONFLICT(ifc_path) DO UPDATE SET " +
            "project_name = excluded.project_name, province = excluded.province, " +
            "ward = excluded.ward, managing_unit = excluded.managing_unit, " +
            "is_enabled = excluded.is_enabled, updated_at = excluded.updated_at, " +
            "ifc_file_name = CASE WHEN excluded.ifc_file_name = '' " +
            "THEN ifc_model_registry.ifc_file_name ELSE excluded.ifc_file_name END;";
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
            BindText(statement, 9, record.StoredFileName);
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

    public bool UpdateModelPath(long id, string ifcPath)
    {
        if (!IsAvailable || id <= 0 || string.IsNullOrWhiteSpace(ifcPath))
        {
            return false;
        }

        const string sql =
            "UPDATE ifc_model_registry SET ifc_path = ?1, updated_at = ?2 " +
            "WHERE id = ?3;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, ifcPath);
            BindText(statement, 2, DateTime.Now.ToString("o"));
            sqlite3_bind_int64(statement, 3, id);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool StoreIfcFile(long id, string sourcePath)
    {
        if (!IsAvailable || id <= 0 || !File.Exists(sourcePath))
        {
            return false;
        }

        var fileInfo = new FileInfo(sourcePath);
        if (fileInfo.Length <= 0 || fileInfo.Length > int.MaxValue ||
            !Execute("BEGIN IMMEDIATE TRANSACTION;"))
        {
            return false;
        }

        var success = false;
        IntPtr blob = IntPtr.Zero;
        try
        {
            const string sql =
                "UPDATE ifc_model_registry SET ifc_file_name = ?1, " +
                "ifc_file_data = zeroblob(?2), updated_at = ?3 WHERE id = ?4;";
            var statement = Prepare(sql);
            if (statement == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                BindText(statement, 1, Path.GetFileName(sourcePath));
                sqlite3_bind_int64(statement, 2, fileInfo.Length);
                BindText(statement, 3, DateTime.Now.ToString("o"));
                sqlite3_bind_int64(statement, 4, id);
                if (sqlite3_step(statement) != SqliteDone)
                {
                    return false;
                }
            }
            finally
            {
                sqlite3_finalize(statement);
            }

            if (!OpenRegistryBlob(id, true, out blob))
            {
                return false;
            }

            const int bufferSize = 1024 * 1024;
            var buffer = new byte[bufferSize];
            var nativeBuffer = Marshal.AllocHGlobal(bufferSize);
            try
            {
                using var stream = File.OpenRead(sourcePath);
                var offset = 0;
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Marshal.Copy(buffer, 0, nativeBuffer, read);
                    if (sqlite3_blob_write(blob, nativeBuffer, read, offset) != SqliteOk)
                    {
                        return false;
                    }

                    offset += read;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(nativeBuffer);
            }

            sqlite3_blob_close(blob);
            blob = IntPtr.Zero;
            success = Execute("COMMIT;");
            return success;
        }
        finally
        {
            if (blob != IntPtr.Zero)
            {
                sqlite3_blob_close(blob);
            }

            if (!success)
            {
                Execute("ROLLBACK;");
            }
        }
    }

    public bool RestoreIfcFile(long id, string destinationPath)
    {
        if (!IsAvailable || id <= 0 || string.IsNullOrWhiteSpace(destinationPath) ||
            !OpenRegistryBlob(id, false, out var blob))
        {
            return false;
        }

        var partialPath = destinationPath + ".partial";
        try
        {
            var length = sqlite3_blob_bytes(blob);
            if (length <= 0)
            {
                return false;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath) ?? string.Empty);
            if (File.Exists(partialPath))
            {
                File.Delete(partialPath);
            }

            const int bufferSize = 1024 * 1024;
            var buffer = new byte[bufferSize];
            var nativeBuffer = Marshal.AllocHGlobal(bufferSize);
            try
            {
                using var stream = File.Create(partialPath);
                for (var offset = 0; offset < length;)
                {
                    var count = Math.Min(bufferSize, length - offset);
                    if (sqlite3_blob_read(blob, nativeBuffer, count, offset) != SqliteOk)
                    {
                        return false;
                    }

                    Marshal.Copy(nativeBuffer, buffer, 0, count);
                    stream.Write(buffer, 0, count);
                    offset += count;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(nativeBuffer);
            }

            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            File.Move(partialPath, destinationPath);
            return true;
        }
        finally
        {
            sqlite3_blob_close(blob);
            if (File.Exists(partialPath))
            {
                File.Delete(partialPath);
            }
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
            "latitude, longitude, elevation, image_path, note, created_at, " +
            "is_resolved, resolved_at " +
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
                    ReadColumnText(statement, 10),
                    sqlite3_column_int(statement, 11) != 0,
                    ReadColumnText(statement, 12)));
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
            "longitude, elevation, image_path, note, created_at, is_resolved, resolved_at) " +
            "VALUES (?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8, ?9, ?10, ?11, ?12);";
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
            sqlite3_bind_int(statement, 11, record.IsResolved ? 1 : 0);
            BindText(statement, 12, record.IsResolved
                ? string.IsNullOrWhiteSpace(record.ResolvedAt)
                    ? DateTime.Now.ToString("o")
                    : record.ResolvedAt
                : string.Empty);
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

    public bool SetFieldInspectionResolved(long inspectionId, bool resolved)
    {
        if (!IsAvailable || inspectionId <= 0)
        {
            return false;
        }

        const string sql =
            "UPDATE field_inspections SET is_resolved = ?1, resolved_at = ?2 " +
            "WHERE id = ?3;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            sqlite3_bind_int(statement, 1, resolved ? 1 : 0);
            BindText(statement, 2, resolved ? DateTime.Now.ToString("o") : string.Empty);
            sqlite3_bind_int64(statement, 3, inspectionId);
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
            "created_at TEXT NOT NULL, updated_at TEXT NOT NULL, " +
            "ifc_file_name TEXT NOT NULL DEFAULT '', ifc_file_data BLOB);");
        Execute(
            "CREATE TABLE IF NOT EXISTS field_inspections (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT, project_name TEXT NOT NULL, " +
            "source_file TEXT NOT NULL, element_key TEXT NOT NULL, " +
            "element_name TEXT NOT NULL, latitude REAL NOT NULL, longitude REAL NOT NULL, " +
            "elevation REAL NOT NULL, image_path TEXT NOT NULL, note TEXT NOT NULL, " +
            "created_at TEXT NOT NULL, is_resolved INTEGER NOT NULL DEFAULT 0, " +
            "resolved_at TEXT NOT NULL DEFAULT '');");
        EnsureColumn(
            "ifc_model_registry",
            "ifc_file_name",
            "TEXT NOT NULL DEFAULT ''");
        EnsureColumn("ifc_model_registry", "ifc_file_data", "BLOB");
        EnsureColumn(
            "field_inspections",
            "is_resolved",
            "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(
            "field_inspections",
            "resolved_at",
            "TEXT NOT NULL DEFAULT ''");
        Execute(
            "CREATE INDEX IF NOT EXISTS idx_ifc_registry_filters ON " +
            "ifc_model_registry(project_name, province, ward);");
        Execute(
            "CREATE INDEX IF NOT EXISTS idx_field_inspection_element ON " +
            "field_inspections(source_file, element_key);");
    }

    private void EnsureColumn(string table, string column, string declaration)
    {
        var statement = Prepare($"PRAGMA table_info({table});");
        if (statement == IntPtr.Zero)
        {
            return;
        }

        var exists = false;
        try
        {
            while (sqlite3_step(statement) == SqliteRow)
            {
                if (string.Equals(
                        ReadColumnText(statement, 1),
                        column,
                        StringComparison.OrdinalIgnoreCase))
                {
                    exists = true;
                    break;
                }
            }
        }
        finally
        {
            sqlite3_finalize(statement);
        }

        if (!exists)
        {
            Execute($"ALTER TABLE {table} ADD COLUMN {column} {declaration};");
        }
    }

    private bool OpenRegistryBlob(long id, bool writable, out IntPtr blob)
    {
        blob = IntPtr.Zero;
        var databaseName = AllocateUtf8("main");
        var tableName = AllocateUtf8("ifc_model_registry");
        var columnName = AllocateUtf8("ifc_file_data");
        try
        {
            return sqlite3_blob_open(
                       database,
                       databaseName,
                       tableName,
                       columnName,
                       id,
                       writable ? 1 : 0,
                       out blob) == SqliteOk;
        }
        finally
        {
            Marshal.FreeHGlobal(databaseName);
            Marshal.FreeHGlobal(tableName);
            Marshal.FreeHGlobal(columnName);
        }
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

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_blob_open(
        IntPtr database,
        IntPtr databaseName,
        IntPtr tableName,
        IntPtr columnName,
        long rowId,
        int flags,
        out IntPtr blob);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_blob_close(IntPtr blob);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_blob_bytes(IntPtr blob);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_blob_read(
        IntPtr blob,
        IntPtr buffer,
        int byteCount,
        int offset);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_blob_write(
        IntPtr blob,
        IntPtr buffer,
        int byteCount,
        int offset);
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
    public string StoredFileName { get; }
    public bool HasStoredFile { get; }
    public long StoredFileSize { get; }

    public IfcModelRegistryRecord(
        long id,
        string projectName,
        string province,
        string ward,
        string managingUnit,
        string ifcPath,
        bool isEnabled,
        string createdAt = "",
        string updatedAt = "",
        string storedFileName = "",
        bool hasStoredFile = false,
        long storedFileSize = 0)
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
        StoredFileName = storedFileName ?? string.Empty;
        HasStoredFile = hasStoredFile;
        StoredFileSize = storedFileSize;
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
    public bool IsResolved { get; }
    public string ResolvedAt { get; }

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
        string createdAt = "",
        bool isResolved = false,
        string resolvedAt = "")
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
        IsResolved = isResolved;
        ResolvedAt = resolvedAt ?? string.Empty;
    }
}
