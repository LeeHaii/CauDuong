using System;
using UnityEngine;

namespace CauDuong.IfcOperations
{
    public enum IfcOperationalStatus
    {
        Operational,
        Warning,
        Critical,
        Repairing
    }

    public static class IfcInspectionCalloutPolicy
    {
        public static bool ShouldShowUnresolved(
            IfcOperationalStatus status,
            bool hasUserUpdate)
        {
            return status == IfcOperationalStatus.Operational || !hasUserUpdate;
        }
    }

    [DisallowMultipleComponent]
    public sealed class IfcOperationsState : MonoBehaviour
    {
        [SerializeField] private IfcInfrastructureCategory category;
        [SerializeField] private IfcOperationalStatus status;
        [SerializeField] private string operationsGlobalId;
        [SerializeField, TextArea(2, 5)] private string maintenanceNote;
        [SerializeField] private string updatedAt;
        [SerializeField] private bool initialized;

        public IfcInfrastructureCategory Category => category;
        public IfcOperationalStatus Status => status;
        public string OperationsGlobalId => operationsGlobalId;
        public string MaintenanceNote => maintenanceNote;
        public string UpdatedAt => updatedAt;
        public bool HasUserUpdate => !string.IsNullOrWhiteSpace(updatedAt);

        public void Initialize(
            IfcInfrastructureCategory infrastructureCategory,
            int expressId,
            int index)
        {
            category = infrastructureCategory;

            if (initialized)
            {
                return;
            }

            status = IfcOperationalStatus.Operational;
            operationsGlobalId = $"VD3-{Math.Max(0, expressId)}-{Math.Max(0, index)}";
            maintenanceNote = string.Empty;
            updatedAt = string.Empty;
            initialized = true;
        }

        public void UpdateOperations(
            IfcOperationalStatus newStatus,
            string note,
            DateTime updatedTime)
        {
            status = newStatus;
            maintenanceNote = note ?? string.Empty;
            updatedAt = updatedTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public void Restore(
            IfcInfrastructureCategory restoredCategory,
            IfcOperationalStatus restoredStatus,
            string restoredOperationsGlobalId,
            string restoredNote,
            string restoredUpdatedAt)
        {
            category = restoredCategory;
            status = restoredStatus;
            operationsGlobalId = restoredOperationsGlobalId ?? string.Empty;
            maintenanceNote = restoredNote ?? string.Empty;
            updatedAt = restoredUpdatedAt ?? string.Empty;
            initialized = true;
        }
    }
}
