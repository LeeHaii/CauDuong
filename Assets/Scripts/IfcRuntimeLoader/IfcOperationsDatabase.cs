using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using CauDuong.IfcOperations;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class IfcOperationsDatabase : MonoBehaviour
{
    private const int SqliteOk = 0;
    private const int SqliteRow = 100;
    private const int SqliteDone = 101;
    private const int SqliteOpenReadWrite = 0x00000002;
    private const int SqliteOpenCreate = 0x00000004;
    private static readonly IntPtr SqliteTransient = new(-1);

    private IntPtr database;

    public bool IsAvailable => database != IntPtr.Zero;
    public string DatabasePath { get; private set; }

    private void Awake()
    {
        Open();
    }

    private void OnEnable()
    {
        Open();
    }

    private void OnDestroy()
    {
        Close();
    }

    public bool TryLoad(
        string sourceFile,
        string elementKey,
        out IfcOperationsSnapshot snapshot)
    {
        snapshot = default;
        if (!IsAvailable)
        {
            return false;
        }

        const string sql =
            "SELECT display_name, category, status, operations_global_id, " +
            "maintenance_note, updated_at FROM ifc_operations " +
            "WHERE source_file = ?1 AND element_key = ?2 LIMIT 1;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            if (sqlite3_step(statement) != SqliteRow)
            {
                return false;
            }

            snapshot = new IfcOperationsSnapshot(
                ReadColumnText(statement, 0),
                (IfcInfrastructureCategory)sqlite3_column_int(statement, 1),
                (IfcOperationalStatus)sqlite3_column_int(statement, 2),
                ReadColumnText(statement, 3),
                ReadColumnText(statement, 4),
                ReadColumnText(statement, 5));
            return true;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool Save(
        string sourceFile,
        string elementKey,
        string displayName,
        IfcOperationsState state)
    {
        if (!IsAvailable || state == null)
        {
            return false;
        }

        const string sql =
            "INSERT OR REPLACE INTO ifc_operations " +
            "(source_file, element_key, display_name, category, status, " +
            "operations_global_id, maintenance_note, updated_at) " +
            "VALUES (?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8);";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            BindText(statement, 3, displayName);
            sqlite3_bind_int(statement, 4, (int)state.Category);
            sqlite3_bind_int(statement, 5, (int)state.Status);
            BindText(statement, 6, state.OperationsGlobalId);
            BindText(statement, 7, state.MaintenanceNote);
            BindText(statement, 8, state.UpdatedAt);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool TryLoadCustomProperties(
        string sourceFile,
        string elementKey,
        IDictionary<string, string> destination)
    {
        if (!IsAvailable || destination == null)
        {
            return false;
        }

        destination.Clear();
        const string sql =
            "SELECT property_key, property_value FROM ifc_custom_properties " +
            "WHERE source_file = ?1 AND element_key = ?2 ORDER BY property_key COLLATE NOCASE;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            while (sqlite3_step(statement) == SqliteRow)
            {
                destination[ReadColumnText(statement, 0)] = ReadColumnText(statement, 1);
            }

            return true;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool SaveCustomProperty(
        string sourceFile,
        string elementKey,
        string propertyKey,
        string propertyValue,
        string previousKey = null)
    {
        if (!IsAvailable || string.IsNullOrWhiteSpace(propertyKey) ||
            !Execute("BEGIN IMMEDIATE TRANSACTION;"))
        {
            return false;
        }

        var success = false;
        try
        {
            if (!string.IsNullOrWhiteSpace(previousKey) &&
                !string.Equals(previousKey, propertyKey, StringComparison.OrdinalIgnoreCase) &&
                !DeleteCustomPropertyInternal(sourceFile, elementKey, previousKey))
            {
                return false;
            }

            if (!UpsertCustomPropertyInternal(
                    sourceFile,
                    elementKey,
                    propertyKey.Trim(),
                    propertyValue ?? string.Empty))
            {
                return false;
            }

            success = Execute("COMMIT;");
            return success;
        }
        finally
        {
            if (!success)
            {
                Execute("ROLLBACK;");
            }
        }
    }

    public bool DeleteCustomProperty(
        string sourceFile,
        string elementKey,
        string propertyKey)
    {
        return IsAvailable &&
               !string.IsNullOrWhiteSpace(propertyKey) &&
               DeleteCustomPropertyInternal(sourceFile, elementKey, propertyKey);
    }

    public bool TryLoadPropertyOverrides(
        string sourceFile,
        string elementKey,
        IDictionary<string, string> values,
        ISet<string> deletedProperties)
    {
        if (!IsAvailable || values == null || deletedProperties == null)
        {
            return false;
        }

        values.Clear();
        deletedProperties.Clear();
        const string sql =
            "SELECT property_key, property_value, is_deleted " +
            "FROM ifc_property_overrides " +
            "WHERE source_file = ?1 AND element_key = ?2;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            while (sqlite3_step(statement) == SqliteRow)
            {
                var propertyKey = ReadColumnText(statement, 0);
                if (sqlite3_column_int(statement, 2) != 0)
                {
                    deletedProperties.Add(propertyKey);
                }
                else
                {
                    values[propertyKey] = ReadColumnText(statement, 1);
                }
            }

            return true;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    public bool SavePropertyOverride(
        string sourceFile,
        string elementKey,
        string propertyKey,
        string propertyValue)
    {
        return UpsertPropertyOverride(
            sourceFile,
            elementKey,
            propertyKey,
            propertyValue,
            false);
    }

    public bool DeleteElementProperty(
        string sourceFile,
        string elementKey,
        string propertyKey)
    {
        return UpsertPropertyOverride(
            sourceFile,
            elementKey,
            propertyKey,
            string.Empty,
            true);
    }

    public bool ResetPropertyOverride(
        string sourceFile,
        string elementKey,
        string propertyKey)
    {
        if (!IsAvailable || string.IsNullOrWhiteSpace(propertyKey))
        {
            return false;
        }

        const string sql =
            "DELETE FROM ifc_property_overrides " +
            "WHERE source_file = ?1 AND element_key = ?2 AND property_key = ?3;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            BindText(statement, 3, propertyKey);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    private void Open()
    {
        if (database != IntPtr.Zero)
        {
            return;
        }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        try
        {
            var directory = Path.Combine(Application.persistentDataPath, "IfcOperations");
            Directory.CreateDirectory(directory);
            DatabasePath = Path.Combine(directory, "ifc_operations.db");
            var pathPointer = AllocateUtf8(DatabasePath);
            try
            {
                var result = sqlite3_open_v2(
                    pathPointer,
                    out database,
                    SqliteOpenReadWrite | SqliteOpenCreate,
                    IntPtr.Zero);
                if (result != SqliteOk)
                {
                    Debug.LogWarning($"Could not open IFC operations database: {GetError()}");
                    Close();
                    return;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pathPointer);
            }

            Execute(
                "CREATE TABLE IF NOT EXISTS ifc_operations (" +
                "source_file TEXT NOT NULL, element_key TEXT NOT NULL, " +
                "display_name TEXT NOT NULL, category INTEGER NOT NULL, " +
                "status INTEGER NOT NULL, operations_global_id TEXT NOT NULL, " +
                "maintenance_note TEXT NOT NULL, updated_at TEXT NOT NULL, " +
                "PRIMARY KEY (source_file, element_key));");
            Execute(
                "CREATE TABLE IF NOT EXISTS ifc_custom_properties (" +
                "source_file TEXT NOT NULL, element_key TEXT NOT NULL, " +
                "property_key TEXT NOT NULL COLLATE NOCASE, " +
                "property_value TEXT NOT NULL, updated_at TEXT NOT NULL, " +
                "PRIMARY KEY (source_file, element_key, property_key));");
            Execute(
                "CREATE TABLE IF NOT EXISTS ifc_property_overrides (" +
                "source_file TEXT NOT NULL, element_key TEXT NOT NULL, " +
                "property_key TEXT NOT NULL COLLATE NOCASE, " +
                "property_value TEXT NOT NULL, is_deleted INTEGER NOT NULL, " +
                "updated_at TEXT NOT NULL, " +
                "PRIMARY KEY (source_file, element_key, property_key));");
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"SQLite persistence is unavailable: {exception.Message}");
            Close();
        }
#else
        Debug.LogWarning("SQLite operations persistence is currently enabled for Windows builds.");
#endif
    }

    private void Close()
    {
        if (database == IntPtr.Zero)
        {
            return;
        }

        sqlite3_close_v2(database);
        database = IntPtr.Zero;
    }

    private bool Execute(string sql)
    {
        var sqlPointer = AllocateUtf8(sql);
        try
        {
            var result = sqlite3_exec(
                database,
                sqlPointer,
                IntPtr.Zero,
                IntPtr.Zero,
                out var errorPointer);
            if (result == SqliteOk)
            {
                return true;
            }

            var error = ReadUtf8(errorPointer);
            if (errorPointer != IntPtr.Zero)
            {
                sqlite3_free(errorPointer);
            }

            Debug.LogWarning($"IFC operations database error: {error}");
            return false;
        }
        finally
        {
            Marshal.FreeHGlobal(sqlPointer);
        }
    }

    private bool UpsertCustomPropertyInternal(
        string sourceFile,
        string elementKey,
        string propertyKey,
        string propertyValue)
    {
        const string sql =
            "INSERT OR REPLACE INTO ifc_custom_properties " +
            "(source_file, element_key, property_key, property_value, updated_at) " +
            "VALUES (?1, ?2, ?3, ?4, ?5);";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            BindText(statement, 3, propertyKey);
            BindText(statement, 4, propertyValue);
            BindText(statement, 5, DateTime.Now.ToString("o"));
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    private bool DeleteCustomPropertyInternal(
        string sourceFile,
        string elementKey,
        string propertyKey)
    {
        const string sql =
            "DELETE FROM ifc_custom_properties " +
            "WHERE source_file = ?1 AND element_key = ?2 AND property_key = ?3;";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            BindText(statement, 3, propertyKey);
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    private bool UpsertPropertyOverride(
        string sourceFile,
        string elementKey,
        string propertyKey,
        string propertyValue,
        bool isDeleted)
    {
        if (!IsAvailable || string.IsNullOrWhiteSpace(propertyKey))
        {
            return false;
        }

        const string sql =
            "INSERT OR REPLACE INTO ifc_property_overrides " +
            "(source_file, element_key, property_key, property_value, is_deleted, updated_at) " +
            "VALUES (?1, ?2, ?3, ?4, ?5, ?6);";
        var statement = Prepare(sql);
        if (statement == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            BindText(statement, 1, sourceFile);
            BindText(statement, 2, elementKey);
            BindText(statement, 3, propertyKey.Trim());
            BindText(statement, 4, propertyValue ?? string.Empty);
            sqlite3_bind_int(statement, 5, isDeleted ? 1 : 0);
            BindText(statement, 6, DateTime.Now.ToString("o"));
            return sqlite3_step(statement) == SqliteDone;
        }
        finally
        {
            sqlite3_finalize(statement);
        }
    }

    private IntPtr Prepare(string sql)
    {
        var sqlPointer = AllocateUtf8(sql);
        try
        {
            var result = sqlite3_prepare_v2(
                database,
                sqlPointer,
                -1,
                out var statement,
                IntPtr.Zero);
            if (result == SqliteOk)
            {
                return statement;
            }

            Debug.LogWarning($"IFC operations database error: {GetError()}");
            return IntPtr.Zero;
        }
        finally
        {
            Marshal.FreeHGlobal(sqlPointer);
        }
    }

    private static void BindText(IntPtr statement, int index, string value)
    {
        var pointer = AllocateUtf8(value ?? string.Empty);
        try
        {
            sqlite3_bind_text(statement, index, pointer, -1, SqliteTransient);
        }
        finally
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

    private string GetError()
    {
        return database == IntPtr.Zero
            ? "Unknown SQLite error"
            : ReadUtf8(sqlite3_errmsg(database));
    }

    private static IntPtr AllocateUtf8(string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes((value ?? string.Empty) + '\0');
        var pointer = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, pointer, bytes.Length);
        return pointer;
    }

    private static string ReadColumnText(IntPtr statement, int index)
    {
        return ReadUtf8(sqlite3_column_text(statement, index));
    }

    private static string ReadUtf8(IntPtr pointer)
    {
        if (pointer == IntPtr.Zero)
        {
            return string.Empty;
        }

        var length = 0;
        while (Marshal.ReadByte(pointer, length) != 0)
        {
            length++;
        }

        var bytes = new byte[length];
        Marshal.Copy(pointer, bytes, 0, length);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_open_v2(
        IntPtr filename,
        out IntPtr database,
        int flags,
        IntPtr virtualFileSystem);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_close_v2(IntPtr database);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_exec(
        IntPtr database,
        IntPtr sql,
        IntPtr callback,
        IntPtr callbackArgument,
        out IntPtr errorMessage);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_prepare_v2(
        IntPtr database,
        IntPtr sql,
        int byteCount,
        out IntPtr statement,
        IntPtr tail);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_step(IntPtr statement);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_finalize(IntPtr statement);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_bind_text(
        IntPtr statement,
        int index,
        IntPtr value,
        int byteCount,
        IntPtr destructor);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_bind_int(IntPtr statement, int index, int value);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sqlite3_column_text(IntPtr statement, int index);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern int sqlite3_column_int(IntPtr statement, int index);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sqlite3_errmsg(IntPtr database);

    [DllImport("winsqlite3", CallingConvention = CallingConvention.Cdecl)]
    private static extern void sqlite3_free(IntPtr memory);
}

public readonly struct IfcOperationsSnapshot
{
    public string DisplayName { get; }
    public IfcInfrastructureCategory Category { get; }
    public IfcOperationalStatus Status { get; }
    public string OperationsGlobalId { get; }
    public string MaintenanceNote { get; }
    public string UpdatedAt { get; }

    public IfcOperationsSnapshot(
        string displayName,
        IfcInfrastructureCategory category,
        IfcOperationalStatus status,
        string operationsGlobalId,
        string maintenanceNote,
        string updatedAt)
    {
        DisplayName = displayName;
        Category = category;
        Status = status;
        OperationsGlobalId = operationsGlobalId;
        MaintenanceNote = maintenanceNote;
        UpdatedAt = updatedAt;
    }
}
