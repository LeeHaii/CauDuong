using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class IfcOperationsDashboard
{
    private enum DashboardModule
    {
        Home,
        Data,
        Field,
        Report
    }

    private readonly List<IfcModelRegistryRecord> modelRegistry = new();
    private readonly List<FieldInspectionRecord> fieldInspections = new();
    private readonly Dictionary<long, IfcInspectionMarker> inspectionMarkers = new();
    private readonly List<GameObject> geographicMarkerAnchors = new();

    private static readonly List<string> VietnamProvinceChoices = new()
    {
        "Thành phố Hà Nội",
        "Tỉnh Cao Bằng",
        "Tỉnh Tuyên Quang",
        "Tỉnh Điện Biên",
        "Tỉnh Lai Châu",
        "Tỉnh Sơn La",
        "Tỉnh Lào Cai",
        "Tỉnh Thái Nguyên",
        "Tỉnh Lạng Sơn",
        "Tỉnh Quảng Ninh",
        "Tỉnh Bắc Ninh",
        "Tỉnh Phú Thọ",
        "Thành phố Hải Phòng",
        "Tỉnh Hưng Yên",
        "Tỉnh Ninh Bình",
        "Tỉnh Thanh Hóa",
        "Tỉnh Nghệ An",
        "Tỉnh Hà Tĩnh",
        "Tỉnh Quảng Trị",
        "Thành phố Huế",
        "Thành phố Đà Nẵng",
        "Tỉnh Quảng Ngãi",
        "Tỉnh Gia Lai",
        "Tỉnh Khánh Hòa",
        "Tỉnh Đắk Lắk",
        "Tỉnh Lâm Đồng",
        "Tỉnh Đồng Nai",
        "Thành phố Hồ Chí Minh",
        "Tỉnh Tây Ninh",
        "Tỉnh Đồng Tháp",
        "Tỉnh Vĩnh Long",
        "Tỉnh An Giang",
        "Thành phố Cần Thơ",
        "Tỉnh Cà Mau"
    };

    private static readonly List<string> InspectionStatusChoices = new()
    {
        "Chưa xử lý",
        "Đã xử lý"
    };

    private VisualElement homePage;
    private VisualElement dataPage;
    private VisualElement fieldPage;
    private VisualElement reportPage;
    private VisualElement registryList;
    private VisualElement fieldHistoryList;
    private VisualElement inspectionPopup;
    private VisualElement inspectionPopupList;
    private VisualElement modelUploadOverlay;
    private VisualElement inspectionFormOverlay;
    private VisualElement inspectionImagePreview;
    private VisualElement inspectionImagePlaceholder;
    private VisualElement elementInspectionHistory;
    private VisualElement elementInspectionList;
    private Label homeModelCount;
    private Label homeInspectionCount;
    private Label homeElementCount;
    private Label registryCountLabel;
    private Label fieldRecordCountLabel;
    private Label selectedIfcPathLabel;
    private Label modelUploadError;
    private Label inspectionImagePathLabel;
    private Label inspectionFormError;
    private Label elementInspectionTotal;
    private Label elementInspectionOpen;
    private Label elementInspectionResolved;
    private Button inspectionButton;
    private TextField filterProjectInput;
    private TextField filterProvinceInput;
    private TextField filterWardInput;
    private TextField filterFileInput;
    private DropdownField modelProjectInput;
    private DropdownField modelProvinceInput;
    private TextField modelWardInput;
    private TextField modelUnitInput;
    private DropdownField inspectionProjectDropdown;
    private DropdownField inspectionStatusDropdown;
    private TextField inspectionNameInput;
    private TextField inspectionLatitudeInput;
    private TextField inspectionLongitudeInput;
    private TextField inspectionElevationInput;
    private TextField inspectionNoteInput;
    private Texture2D inspectionPreviewTexture;
    private IfcAssetRecord inspectionLinkedRecord;
    private Coroutine inspectionMarkerLinkRoutine;
    private DashboardModule activeModule;
    private bool moduleUiBound;
    private string selectedIfcPath;
    private string selectedInspectionImagePath;

    private bool IsReportModuleActive => activeModule == DashboardModule.Report;

    private void BindModuleUi()
    {
        if (moduleUiBound || root == null)
        {
            return;
        }

        homePage = root.Q<VisualElement>("home-page");
        dataPage = root.Q<VisualElement>("data-page");
        fieldPage = root.Q<VisualElement>("field-page");
        reportPage = root.Q<VisualElement>("report-page");
        registryList = root.Q<VisualElement>("registry-list");
        fieldHistoryList = root.Q<VisualElement>("field-history-list");
        inspectionPopup = root.Q<VisualElement>("inspection-popup");
        inspectionPopupList = root.Q<VisualElement>("inspection-popup-list");
        modelUploadOverlay = root.Q<VisualElement>("model-upload-overlay");
        inspectionFormOverlay = root.Q<VisualElement>("inspection-form-overlay");
        inspectionImagePreview = root.Q<VisualElement>("inspection-image-preview");
        inspectionImagePlaceholder = root.Q<VisualElement>("inspection-image-placeholder");
        elementInspectionHistory = root.Q<VisualElement>("element-inspection-history");
        elementInspectionList = root.Q<VisualElement>("element-inspection-list");
        homeModelCount = root.Q<Label>("home-model-count");
        homeInspectionCount = root.Q<Label>("home-inspection-count");
        homeElementCount = root.Q<Label>("home-element-count");
        registryCountLabel = root.Q<Label>("registry-count");
        fieldRecordCountLabel = root.Q<Label>("field-record-count");
        selectedIfcPathLabel = root.Q<Label>("selected-ifc-path");
        modelUploadError = root.Q<Label>("model-upload-error");
        inspectionImagePathLabel = root.Q<Label>("inspection-image-path");
        inspectionFormError = root.Q<Label>("inspection-form-error");
        elementInspectionTotal = root.Q<Label>("element-inspection-total");
        elementInspectionOpen = root.Q<Label>("element-inspection-open");
        elementInspectionResolved = root.Q<Label>("element-inspection-resolved");
        inspectionButton = root.Q<Button>("inspection-button");
        filterProjectInput = root.Q<TextField>("filter-project-input");
        filterProvinceInput = root.Q<TextField>("filter-province-input");
        filterWardInput = root.Q<TextField>("filter-ward-input");
        filterFileInput = root.Q<TextField>("filter-file-input");
        modelProjectInput = root.Q<DropdownField>("model-project-input");
        modelProvinceInput = root.Q<DropdownField>("model-province-input");
        modelWardInput = root.Q<TextField>("model-ward-input");
        modelUnitInput = root.Q<TextField>("model-unit-input");
        inspectionProjectDropdown = root.Q<DropdownField>("inspection-project-dropdown");
        inspectionStatusDropdown = root.Q<DropdownField>("inspection-status-dropdown");
        inspectionNameInput = root.Q<TextField>("inspection-name-input");
        inspectionLatitudeInput = root.Q<TextField>("inspection-latitude-input");
        inspectionLongitudeInput = root.Q<TextField>("inspection-longitude-input");
        inspectionElevationInput = root.Q<TextField>("inspection-elevation-input");
        inspectionNoteInput = root.Q<TextField>("inspection-note-input");

        if (homePage == null || dataPage == null || fieldPage == null ||
            reportPage == null || registryList == null || fieldHistoryList == null)
        {
            Debug.LogError("The BIM-GIS module UI could not be bound.");
            return;
        }

        root.Q<Button>("home-data-button").clicked +=
            () => ShowModule(DashboardModule.Data);
        root.Q<Button>("home-field-button").clicked += OpenFieldModule;
        root.Q<Button>("home-report-button").clicked +=
            () => ShowModule(DashboardModule.Report);
        root.Q<Button>("data-home-button").clicked +=
            () => ShowModule(DashboardModule.Home);
        root.Q<Button>("field-home-button").clicked +=
            () => ShowModule(DashboardModule.Home);
        root.Q<Button>("report-home-button").clicked +=
            () => ShowModule(DashboardModule.Home);
        root.Q<Button>("open-model-upload-button").clicked += OpenModelUpload;
        root.Q<Button>("close-model-upload-button").clicked += CloseModelUpload;
        root.Q<Button>("cancel-model-upload-button").clicked += CloseModelUpload;
        root.Q<Button>("choose-ifc-file-button").clicked += ChooseIfcForRegistry;
        root.Q<Button>("save-model-upload-button").clicked += SaveModelUpload;
        root.Q<Button>("open-inspection-form-button").clicked +=
            () => OpenInspectionDialog(false);
        root.Q<Button>("close-inspection-form-button").clicked += CloseInspectionDialog;
        root.Q<Button>("cancel-inspection-form-button").clicked += CloseInspectionDialog;
        root.Q<Button>("choose-inspection-image-button").clicked += ChooseInspectionImage;
        root.Q<Button>("use-selected-element-button").clicked += UseSelectedElement;
        root.Q<Button>("submit-inspection-button").clicked += SaveInspection;
        root.Q<Button>("inspection-button").clicked += ToggleInspectionPopup;
        root.Q<Button>("open-field-from-report-button").clicked +=
            () => OpenInspectionDialogFromReport(false);
        filterProjectInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterProvinceInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterWardInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterFileInput.RegisterValueChangedCallback(_ => BuildRegistryList());

        modelProvinceInput.choices = VietnamProvinceChoices;
        modelProvinceInput.SetValueWithoutNotify("Thành phố Hà Nội");
        modelWardInput.SetValueWithoutNotify(string.Empty);
        modelUnitInput.SetValueWithoutNotify("Ban Quản lý Dự án Đầu tư Xây dựng");
        inspectionStatusDropdown.choices = InspectionStatusChoices;
        inspectionStatusDropdown.index = 0;
        modelUploadOverlay.style.display = DisplayStyle.None;
        inspectionFormOverlay.style.display = DisplayStyle.None;
        inspectionPopup.style.display = DisplayStyle.None;
        moduleUiBound = true;

        LoadRegistryFromDatabase();
        LoadInspectionsFromDatabase();
        UpdateInspectionProjectChoices();
        UpdateModelProjectChoices();
        ShowModule(DashboardModule.Home);
        RefreshModuleData();
    }

    private void ShowModule(DashboardModule module)
    {
        activeModule = module;
        if (homePage == null)
        {
            return;
        }

        if (module != DashboardModule.Report &&
            measurementController?.ActiveMode != IfcMeasurementMode.None)
        {
            measurementController.Stop();
        }

        HidePopups();
        homePage.style.display = module == DashboardModule.Home
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        dataPage.style.display = module == DashboardModule.Data
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        fieldPage.style.display = module == DashboardModule.Field
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        reportPage.style.display = module == DashboardModule.Report
            ? DisplayStyle.Flex
            : DisplayStyle.None;

        if (module == DashboardModule.Data)
        {
            LoadRegistryFromDatabase();
            BuildRegistryList();
        }
        else if (module == DashboardModule.Field)
        {
            LoadInspectionsFromDatabase();
            UpdateInspectionProjectChoices();
            BuildFieldHistory();
        }
        else if (module == DashboardModule.Report)
        {
            if (startupLoading)
            {
                SetLoadingVisible(true);
            }

            RefreshDashboard();
            BuildInspectionMarkers();
        }

        UpdateModuleCounts();
    }

    private void OpenFieldModule()
    {
        HidePopups();
        ShowModule(DashboardModule.Field);
    }

    private void OpenInspectionDialogFromReport(bool requireSelectedElement)
    {
        ShowModule(DashboardModule.Field);
        OpenInspectionDialog(requireSelectedElement);
    }

    private void OpenInspectionDialog(bool useSelectedElement)
    {
        ResetInspectionForm();
        UpdateInspectionProjectChoices();
        inspectionFormOverlay.style.display = DisplayStyle.Flex;
        if (useSelectedElement || selectedRecord != null)
        {
            UseSelectedElement();
        }
        else
        {
            PrefillCurrentMapPosition();
        }
    }

    private void CloseInspectionDialog()
    {
        if (inspectionFormOverlay != null)
        {
            inspectionFormOverlay.style.display = DisplayStyle.None;
        }
    }

    private void ResetInspectionForm()
    {
        inspectionLinkedRecord = null;
        inspectionNameInput.SetValueWithoutNotify(string.Empty);
        inspectionNoteInput.SetValueWithoutNotify(string.Empty);
        inspectionLatitudeInput.SetValueWithoutNotify(string.Empty);
        inspectionLongitudeInput.SetValueWithoutNotify(string.Empty);
        inspectionElevationInput.SetValueWithoutNotify("0");
        inspectionStatusDropdown.index = 0;
        inspectionFormError.text = string.Empty;
        selectedInspectionImagePath = string.Empty;
        inspectionImagePathLabel.text = "Chưa chọn ảnh";
        inspectionImagePreview.style.backgroundImage = StyleKeyword.None;
        inspectionImagePlaceholder.style.display = DisplayStyle.Flex;
        if (inspectionPreviewTexture != null)
        {
            Destroy(inspectionPreviewTexture);
            inspectionPreviewTexture = null;
        }
    }

    private IEnumerator LoadRegisteredModels()
    {
        startupLoading = true;
        SetLoadingVisible(true);
        SetLoadingProgress(0.04f, "Đang kiểm tra kho dữ liệu mô hình...");
        yield return null;

        var paths = new List<string>();
        if (operationsDatabase != null && operationsDatabase.IsAvailable)
        {
            SeedDefaultModelRegistry();
            LoadRegistryFromDatabase();
            foreach (var record in modelRegistry.Where(record => record.IsEnabled))
            {
                var path = EnsureRegistryModelAvailable(record);
                if (!string.IsNullOrWhiteSpace(path) &&
                    (loader == null || !loader.LoadedModels.Any(model =>
                        string.Equals(
                            loader.GetModelSourcePath(model),
                            path,
                            StringComparison.OrdinalIgnoreCase))))
                {
                    paths.Add(path);
                }
            }
        }
        else
        {
            var defaultDirectory = GetDefaultIfcDirectory();
            if (Directory.Exists(defaultDirectory))
            {
                paths.AddRange(Directory
                    .GetFiles(defaultDirectory, "*.ifc", SearchOption.TopDirectoryOnly)
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .Where(path => loader == null || !loader.LoadedModels.Any(model =>
                        string.Equals(
                            loader.GetModelSourcePath(model),
                            path,
                            StringComparison.OrdinalIgnoreCase)))
                    .Take(Mathf.Max(1, defaultModelLimit)));
            }
        }

        if (loader == null)
        {
            SetLoadingProgress(1f, "Không tìm thấy bộ nạp IFC.");
            yield return null;
            startupLoading = false;
            startupLoadRoutine = null;
            SetLoadingVisible(false);
            RefreshModuleData();
            yield break;
        }

        var completedCount = 0;
        var failedCount = 0;
        Action<GameObject> completed = _ => completedCount++;
        Action<string> failed = _ => failedCount++;
        loader.LoadCompleted += completed;
        loader.LoadFailed += failed;

        foreach (var path in paths)
        {
            loader.LoadIFC(path);
        }

        var lastSettledCount = -1;
        while (completedCount + failedCount < paths.Count || loader.IsLoading)
        {
            var settledCount = completedCount + failedCount;
            if (settledCount != lastSettledCount)
            {
                lastSettledCount = settledCount;
                SetLoadingProgress(
                    paths.Count == 0
                        ? 0.82f
                        : 0.1f + 0.76f * settledCount / paths.Count,
                    $"Đang nạp mô hình IFC: {settledCount}/{paths.Count}");
            }

            if (settledCount >= paths.Count && loader.IsLoading)
            {
                SetLoadingProgress(
                    0.9f,
                    "Đang tối ưu mesh, proxy khoảng cách và static batching...");
            }

            yield return null;
        }

        loader.LoadCompleted -= completed;
        loader.LoadFailed -= failed;
        SetLoadingProgress(0.95f, "Đang lập chỉ mục cấu kiện và đồng bộ hiện trường...");
        RebuildModelIndex();
        RefreshModuleData();
        if (IsReportModuleActive)
        {
            BuildInspectionMarkers();
        }

        yield return null;
        yield return new WaitForEndOfFrame();
        SetLoadingProgress(
            1f,
            $"Đã sẵn sàng {loader.LoadedModels.Count:N0} mô hình và {records.Count:N0} cấu kiện.");
        yield return null;

        startupLoading = false;
        startupLoadRoutine = null;
        SetLoadingVisible(false);
        SetImportStatus($"Đã nạp {loader.LoadedModels.Count:N0} mô hình IFC từ kho dữ liệu.");
    }

    private string EnsureRegistryModelAvailable(IfcModelRegistryRecord record)
    {
        if (File.Exists(record.IfcPath))
        {
            return record.IfcPath;
        }

        if (!record.HasStoredFile || operationsDatabase == null)
        {
            return string.Empty;
        }

        var directory = Path.Combine(
            Application.persistentDataPath,
            "IfcOperations",
            "Models",
            "Database");
        var fileName = Path.GetFileName(record.StoredFileName);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"model_{record.Id}.ifc";
        }

        var destination = Path.Combine(directory, $"{record.Id}_{fileName}");
        if (!operationsDatabase.RestoreIfcFile(record.Id, destination))
        {
            Debug.LogWarning($"Không thể phục hồi file IFC {fileName} từ SQLite.");
            return string.Empty;
        }

        operationsDatabase.UpdateModelPath(record.Id, destination);
        return destination;
    }

    private IEnumerator LoadRegisteredPath(string path)
    {
        if (loader == null || !File.Exists(path))
        {
            yield break;
        }

        while (loader.IsLoading)
        {
            yield return null;
        }

        var finished = false;
        Action<GameObject> completed = _ => finished = true;
        Action<string> failed = _ => finished = true;
        loader.LoadCompleted += completed;
        loader.LoadFailed += failed;
        loader.LoadIFC(path);
        yield return null;
        while (!finished)
        {
            yield return null;
        }

        loader.LoadCompleted -= completed;
        loader.LoadFailed -= failed;
    }

    private void SeedDefaultModelRegistry()
    {
        var directory = GetDefaultIfcDirectory();
        if (operationsDatabase == null || !Directory.Exists(directory))
        {
            return;
        }

        LoadRegistryFromDatabase();
        foreach (var path in Directory
                     .GetFiles(directory, "*.ifc", SearchOption.TopDirectoryOnly)
                     .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                     .Take(Mathf.Max(1, defaultModelLimit)))
        {
            if (modelRegistry.Any(record => string.Equals(
                    record.IfcPath,
                    path,
                    StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var bundledDuplicates = modelRegistry
                .Where(record =>
                    string.Equals(
                        Path.GetFileName(record.IfcPath),
                        Path.GetFileName(path),
                        StringComparison.OrdinalIgnoreCase) &&
                    IsBundledDefaultPath(record.IfcPath))
                .ToArray();
            foreach (var duplicate in bundledDuplicates)
            {
                operationsDatabase.DeleteModelRegistry(duplicate.Id);
                modelRegistry.Remove(duplicate);
            }

            operationsDatabase.SaveModelRegistry(new IfcModelRegistryRecord(
                0,
                ProjectName,
                "Hà Nội",
                "Chưa cập nhật",
                "Ban Quản lý Dự án Đầu tư Xây dựng",
                path,
                true));
        }

        LoadRegistryFromDatabase();
        foreach (var record in modelRegistry.Where(record =>
                     IsBundledDefaultPath(record.IfcPath) &&
                     File.Exists(record.IfcPath) &&
                     !record.HasStoredFile))
        {
            operationsDatabase.StoreIfcFile(record.Id, record.IfcPath);
        }
    }

    private static bool IsBundledDefaultPath(string path)
    {
        var normalized = (path ?? string.Empty).Replace('\\', '/');
        return normalized.IndexOf(
                   "/Assets/IFC/Default/",
                   StringComparison.OrdinalIgnoreCase) >= 0 ||
               normalized.IndexOf(
                   "/StreamingAssets/IFC/Default/",
                   StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void LoadRegistryFromDatabase()
    {
        if (operationsDatabase != null)
        {
            operationsDatabase.GetModelRegistry(modelRegistry);
        }
    }

    private void LoadInspectionsFromDatabase()
    {
        if (operationsDatabase != null)
        {
            operationsDatabase.GetFieldInspections(fieldInspections);
        }
    }

    private void RefreshModuleData()
    {
        if (!moduleUiBound)
        {
            return;
        }

        LoadRegistryFromDatabase();
        LoadInspectionsFromDatabase();
        BuildRegistryList();
        BuildFieldHistory();
        BuildInspectionPopup();
        UpdateModelProjectChoices();
        UpdateInspectionProjectChoices();
        if (selectedRecord != null)
        {
            BuildElementInspectionHistory(selectedRecord);
        }
        UpdateModuleCounts();
    }

    private void UpdateModuleCounts()
    {
        homeModelCount.text = modelRegistry.Count.ToString("N0");
        homeInspectionCount.text = fieldInspections.Count.ToString("N0");
        homeElementCount.text = records.Count.ToString("N0");
        fieldRecordCountLabel.text = $"{fieldInspections.Count:N0} ghi nhận";
        inspectionButton.text = $"Hiện Trường ({fieldInspections.Count:N0})";
    }

    private void BuildRegistryList()
    {
        if (registryList == null)
        {
            return;
        }

        registryList.Clear();
        var filtered = modelRegistry.Where(MatchesRegistryFilters).ToArray();
        registryCountLabel.text = filtered.Length == modelRegistry.Count
            ? $"{modelRegistry.Count:N0} mô hình trong kho dữ liệu"
            : $"{filtered.Length:N0}/{modelRegistry.Count:N0} mô hình phù hợp";
        if (filtered.Length == 0)
        {
            var empty = new Label("Không có mô hình IFC phù hợp với bộ lọc.");
            empty.AddToClassList("data-empty");
            registryList.Add(empty);
            return;
        }

        for (var index = 0; index < filtered.Length; index++)
        {
            var record = filtered[index];
            var row = new VisualElement();
            row.AddToClassList("data-table-row");
            row.Add(CreateDataLabel((index + 1).ToString(), "data-cell-index"));
            row.Add(CreateDataLabel(record.ProjectName, "data-cell-project"));
            row.Add(CreateDataLabel(record.Province, "data-cell-place"));
            row.Add(CreateDataLabel(record.Ward, "data-cell-place"));
            row.Add(CreateDataLabel(record.ManagingUnit, "data-cell-unit"));

            var fileName = !string.IsNullOrWhiteSpace(record.StoredFileName)
                ? record.StoredFileName
                : Path.GetFileName(record.IfcPath);
            var fileState = File.Exists(record.IfcPath)
                ? record.HasStoredFile
                    ? $"{fileName}\nSQLite • {FormatFileSize(record.StoredFileSize)}"
                    : fileName
                : record.HasStoredFile
                    ? $"{fileName}\nSẵn sàng phục hồi từ SQLite"
                    : $"{fileName}\nThiếu file";
            var fileLabel = CreateDataLabel(
                fileState,
                "data-cell-file");
            fileLabel.tooltip = record.IfcPath;
            row.Add(fileLabel);

            var enabled = new Toggle
            {
                value = record.IsEnabled,
                tooltip = record.IsEnabled
                    ? "Đang dùng trong báo cáo"
                    : "Không dùng trong báo cáo"
            };
            enabled.AddToClassList("registry-toggle");
            enabled.RegisterValueChangedCallback(change =>
                SetRegistryModelEnabled(record, change.newValue));
            var enabledCell = new VisualElement();
            enabledCell.AddToClassList("data-cell");
            enabledCell.AddToClassList("data-cell-state");
            enabledCell.Add(enabled);
            row.Add(enabledCell);

            var actions = new VisualElement();
            actions.AddToClassList("data-cell");
            actions.AddToClassList("data-cell-action");
            var delete = new Button(() => DeleteRegistryModel(record))
            {
                text = "×",
                tooltip = "Xóa mô hình khỏi kho dữ liệu"
            };
            delete.AddToClassList("registry-delete-button");
            actions.Add(delete);
            row.Add(actions);
            registryList.Add(row);
        }
    }

    private static Label CreateDataLabel(string text, string className)
    {
        var label = new Label(string.IsNullOrWhiteSpace(text) ? "-" : text);
        label.AddToClassList("data-cell");
        label.AddToClassList(className);
        return label;
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
        {
            return $"{bytes} B";
        }

        if (bytes < 1024L * 1024L)
        {
            return $"{bytes / 1024d:F1} KB";
        }

        if (bytes < 1024L * 1024L * 1024L)
        {
            return $"{bytes / (1024d * 1024d):F1} MB";
        }

        return $"{bytes / (1024d * 1024d * 1024d):F1} GB";
    }

    private bool MatchesRegistryFilters(IfcModelRegistryRecord record)
    {
        return ContainsFilter(record.ProjectName, filterProjectInput.value) &&
               ContainsFilter(record.Province, filterProvinceInput.value) &&
               ContainsFilter(record.Ward, filterWardInput.value) &&
               ContainsFilter(
                   string.IsNullOrWhiteSpace(record.StoredFileName)
                       ? Path.GetFileName(record.IfcPath)
                       : record.StoredFileName,
                   filterFileInput.value);
    }

    private static bool ContainsFilter(string value, string filter)
    {
        return string.IsNullOrWhiteSpace(filter) ||
               (value ?? string.Empty).IndexOf(
                   filter.Trim(),
                   StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    private void OpenModelUpload()
    {
        UpdateModelProjectChoices();
        selectedIfcPath = string.Empty;
        selectedIfcPathLabel.text = "Chưa chọn file .ifc";
        modelUploadError.text = string.Empty;
        modelUploadOverlay.style.display = DisplayStyle.Flex;
    }

    private void CloseModelUpload()
    {
        if (modelUploadOverlay != null)
        {
            modelUploadOverlay.style.display = DisplayStyle.None;
        }
    }

    private void ChooseIfcForRegistry()
    {
        ResolveDependencies();
        var paths = runtimeLoader?.SelectIfcFiles();
        if (paths == null || paths.Count == 0)
        {
            return;
        }

        selectedIfcPath = paths[0];
        selectedIfcPathLabel.text = Path.GetFileName(selectedIfcPath);
        selectedIfcPathLabel.tooltip = selectedIfcPath;
        modelUploadError.text = string.Empty;
    }

    private void SaveModelUpload()
    {
        var project = modelProjectInput.value?.Trim();
        var province = modelProvinceInput.value?.Trim();
        if (string.IsNullOrWhiteSpace(project) ||
            string.IsNullOrWhiteSpace(province) ||
            string.IsNullOrWhiteSpace(selectedIfcPath) ||
            !File.Exists(selectedIfcPath))
        {
            modelUploadError.text = "Vui lòng nhập dự án, tỉnh/thành phố và chọn file IFC hợp lệ.";
            return;
        }

        try
        {
            var managedPath = PersistIfcModel(selectedIfcPath);
            var record = new IfcModelRegistryRecord(
                0,
                project,
                province,
                modelWardInput.value?.Trim(),
                modelUnitInput.value?.Trim(),
                managedPath,
                true);
            if (operationsDatabase == null || !operationsDatabase.SaveModelRegistry(record))
            {
                modelUploadError.text = "Không thể lưu mô hình vào cơ sở dữ liệu SQLite.";
                return;
            }

            LoadRegistryFromDatabase();
            var storedRecord = modelRegistry.FirstOrDefault(candidate =>
                string.Equals(
                    candidate.IfcPath,
                    managedPath,
                    StringComparison.OrdinalIgnoreCase));
            if (storedRecord.Id <= 0 ||
                !operationsDatabase.StoreIfcFile(storedRecord.Id, managedPath))
            {
                if (storedRecord.Id > 0)
                {
                    operationsDatabase.DeleteModelRegistry(storedRecord.Id);
                }

                modelUploadError.text =
                    "Không thể lưu nội dung file IFC vào cơ sở dữ liệu SQLite.";
                return;
            }

            CloseModelUpload();
            LoadRegistryFromDatabase();
            BuildRegistryList();
            UpdateModelProjectChoices();
            UpdateInspectionProjectChoices();
            UpdateModuleCounts();
            StartCoroutine(LoadRegisteredPath(managedPath));
            SetImportStatus($"Đã thêm {Path.GetFileName(managedPath)} vào kho dữ liệu.");
        }
        catch (Exception exception)
        {
            modelUploadError.text = $"Không thể lưu file IFC: {exception.Message}";
        }
    }

    private static string PersistIfcModel(string sourcePath)
    {
        var directory = Path.Combine(
            Application.persistentDataPath,
            "IfcOperations",
            "Models");
        Directory.CreateDirectory(directory);
        if (string.Equals(
                Path.GetDirectoryName(Path.GetFullPath(sourcePath)),
                Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase))
        {
            return sourcePath;
        }

        var fileName =
            $"{Path.GetFileNameWithoutExtension(sourcePath)}_{DateTime.Now:yyyyMMdd_HHmmssfff}.ifc";
        var destination = Path.Combine(directory, fileName);
        File.Copy(sourcePath, destination, false);
        return destination;
    }

    private void SetRegistryModelEnabled(IfcModelRegistryRecord record, bool enabled)
    {
        if (operationsDatabase == null ||
            !operationsDatabase.SetModelEnabled(record.Id, enabled))
        {
            SetImportStatus("Không thể cập nhật trạng thái mô hình trong SQLite.");
            return;
        }

        if (enabled)
        {
            var availablePath = EnsureRegistryModelAvailable(record);
            if (string.IsNullOrWhiteSpace(availablePath))
            {
                SetImportStatus($"Không thể lấy file IFC từ ổ đĩa hoặc SQLite: {record.IfcPath}");
            }
            else if (!IsModelLoaded(availablePath))
            {
                StartCoroutine(LoadRegisteredPath(availablePath));
            }
        }
        else
        {
            RemoveLoadedModel(record.IfcPath);
        }

        LoadRegistryFromDatabase();
        BuildRegistryList();
        UpdateModuleCounts();
    }

    private void DeleteRegistryModel(IfcModelRegistryRecord record)
    {
        RemoveLoadedModel(record.IfcPath);
        if (operationsDatabase == null ||
            !operationsDatabase.DeleteModelRegistry(record.Id))
        {
            SetImportStatus("Không thể xóa mô hình khỏi cơ sở dữ liệu.");
            return;
        }

        LoadRegistryFromDatabase();
        BuildRegistryList();
        UpdateModelProjectChoices();
        UpdateInspectionProjectChoices();
        UpdateModuleCounts();
        SetImportStatus($"Đã xóa {Path.GetFileName(record.IfcPath)} khỏi kho dữ liệu.");
    }

    private bool IsModelLoaded(string path)
    {
        return loader != null && loader.LoadedModels.Any(model =>
            string.Equals(
                loader.GetModelSourcePath(model),
                path,
                StringComparison.OrdinalIgnoreCase));
    }

    private void RemoveLoadedModel(string path)
    {
        if (loader == null)
        {
            return;
        }

        var model = loader.LoadedModels.FirstOrDefault(candidate =>
            string.Equals(
                loader.GetModelSourcePath(candidate),
                path,
                StringComparison.OrdinalIgnoreCase));
        if (model != null)
        {
            loader.RemoveModel(model);
        }
    }

    private void UpdateInspectionProjectChoices()
    {
        if (inspectionProjectDropdown == null)
        {
            return;
        }

        var current = inspectionProjectDropdown.value;
        var choices = modelRegistry
            .Select(record => record.ProjectName)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(value => value)
            .ToList();
        if (choices.Count == 0)
        {
            choices.Add(ProjectName);
        }

        inspectionProjectDropdown.choices = choices;
        inspectionProjectDropdown.SetValueWithoutNotify(
            choices.Contains(current) ? current : choices[0]);
    }

    private void UpdateModelProjectChoices()
    {
        if (modelProjectInput == null)
        {
            return;
        }

        var current = modelProjectInput.value;
        var choices = modelRegistry
            .Select(record => record.ProjectName)
            .Append(ProjectName)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(value => value)
            .ToList();
        modelProjectInput.choices = choices;
        modelProjectInput.SetValueWithoutNotify(
            choices.Contains(current) ? current : choices[0]);
    }

    private void ChooseInspectionImage()
    {
        ResolveDependencies();
        var path = runtimeLoader?.SelectImageFile();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return;
        }

        selectedInspectionImagePath = path;
        inspectionImagePathLabel.text = Path.GetFileName(path);
        inspectionImagePathLabel.tooltip = path;
        inspectionFormError.text = string.Empty;
        ShowInspectionPreview(path);
    }

    private void ShowInspectionPreview(string path)
    {
        if (inspectionPreviewTexture != null)
        {
            Destroy(inspectionPreviewTexture);
        }

        inspectionPreviewTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
        {
            name = "Field inspection preview"
        };
        if (!inspectionPreviewTexture.LoadImage(File.ReadAllBytes(path)))
        {
            Destroy(inspectionPreviewTexture);
            inspectionPreviewTexture = null;
            inspectionFormError.text = "Không thể đọc ảnh hiện trường đã chọn.";
            return;
        }

        inspectionImagePreview.style.backgroundImage =
            new StyleBackground(inspectionPreviewTexture);
        inspectionImagePlaceholder.style.display = DisplayStyle.None;
    }

    private void UseSelectedElement()
    {
        if (selectedRecord == null)
        {
            inspectionFormError.text =
                "Chưa có cấu kiện được chọn trong không gian báo cáo.";
            return;
        }

        inspectionLinkedRecord = selectedRecord;
        inspectionNameInput.SetValueWithoutNotify(selectedRecord.Name);
        inspectionLatitudeInput.SetValueWithoutNotify(
            selectedRecord.Latitude.ToString("F8", CultureInfo.InvariantCulture));
        inspectionLongitudeInput.SetValueWithoutNotify(
            selectedRecord.Longitude.ToString("F8", CultureInfo.InvariantCulture));
        inspectionElevationInput.SetValueWithoutNotify(
            selectedRecord.Elevation.ToString("F2", CultureInfo.InvariantCulture));
        inspectionFormError.text = string.Empty;

        var matchingProject = modelRegistry.FirstOrDefault(record =>
            Path.GetFileNameWithoutExtension(record.IfcPath).IndexOf(
                Path.GetFileNameWithoutExtension(selectedRecord.SourceFile),
                StringComparison.CurrentCultureIgnoreCase) >= 0);
        if (!string.IsNullOrWhiteSpace(matchingProject.ProjectName) &&
            inspectionProjectDropdown.choices.Contains(matchingProject.ProjectName))
        {
            inspectionProjectDropdown.SetValueWithoutNotify(matchingProject.ProjectName);
        }
    }

    private void PrefillCurrentMapPosition()
    {
        if (arcGisMapLoader == null ||
            Math.Abs(arcGisMapLoader.LastLatitude) < double.Epsilon ||
            Math.Abs(arcGisMapLoader.LastLongitude) < double.Epsilon)
        {
            return;
        }

        inspectionLatitudeInput.SetValueWithoutNotify(
            arcGisMapLoader.LastLatitude.ToString("F8", CultureInfo.InvariantCulture));
        inspectionLongitudeInput.SetValueWithoutNotify(
            arcGisMapLoader.LastLongitude.ToString("F8", CultureInfo.InvariantCulture));
        inspectionElevationInput.SetValueWithoutNotify(
            arcGisMapLoader.LastElevation.ToString("F2", CultureInfo.InvariantCulture));
    }

    private void SaveInspection()
    {
        var name = inspectionNameInput.value?.Trim();
        if (string.IsNullOrWhiteSpace(name) ||
            !TryParseCoordinate(inspectionLatitudeInput.value, out var latitude) ||
            !TryParseCoordinate(inspectionLongitudeInput.value, out var longitude) ||
            !TryParseCoordinate(inspectionElevationInput.value, out var elevation) ||
            latitude is < -90d or > 90d || longitude is < -180d or > 180d)
        {
            inspectionFormError.text =
                "Vui lòng nhập tên, vĩ độ, kinh độ và cao độ hợp lệ.";
            return;
        }

        try
        {
            var imagePath = string.IsNullOrWhiteSpace(selectedInspectionImagePath)
                ? string.Empty
                : PersistInspectionImage(selectedInspectionImagePath);
            var record = new FieldInspectionRecord(
                0,
                inspectionProjectDropdown.value,
                inspectionLinkedRecord?.SourceFile ?? string.Empty,
                inspectionLinkedRecord != null
                    ? GetElementKey(inspectionLinkedRecord.Metadata)
                    : string.Empty,
                name,
                latitude,
                longitude,
                elevation,
                imagePath,
                inspectionNoteInput.value?.Trim(),
                string.Empty,
                inspectionStatusDropdown.index == 1);
            if (operationsDatabase == null ||
                !operationsDatabase.SaveFieldInspection(record, out _))
            {
                inspectionFormError.text =
                    "Không thể lưu ghi nhận hiện trường vào SQLite.";
                return;
            }

            CloseInspectionDialog();
            ResetInspectionForm();
            LoadInspectionsFromDatabase();
            BuildFieldHistory();
            BuildInspectionPopup();
            BuildInspectionMarkers();
            if (selectedRecord != null)
            {
                BuildElementInspectionHistory(selectedRecord);
            }
            UpdateModuleCounts();
            SetImportStatus("Đã lưu ghi nhận hiện trường và đồng bộ điểm lên báo cáo.");
        }
        catch (Exception exception)
        {
            inspectionFormError.text = $"Không thể lưu ghi nhận: {exception.Message}";
        }
    }

    private static bool TryParseCoordinate(string value, out double result)
    {
        return double.TryParse(
                   value,
                   NumberStyles.Float,
                   CultureInfo.InvariantCulture,
                   out result) ||
               double.TryParse(
                   value,
                   NumberStyles.Float,
                   CultureInfo.CurrentCulture,
                   out result);
    }

    private static string PersistInspectionImage(string sourcePath)
    {
        var directory = Path.Combine(
            Application.persistentDataPath,
            "IfcOperations",
            "InspectionImages");
        Directory.CreateDirectory(directory);
        var extension = Path.GetExtension(sourcePath);
        var destination = Path.Combine(
            directory,
            $"inspection_{DateTime.Now:yyyyMMdd_HHmmssfff}{extension}");
        File.Copy(sourcePath, destination, false);
        return destination;
    }

    private void BuildFieldHistory()
    {
        if (fieldHistoryList == null)
        {
            return;
        }

        fieldHistoryList.Clear();
        if (fieldInspections.Count == 0)
        {
            var empty = new Label("Chưa có ghi nhận hiện trường.");
            empty.AddToClassList("field-empty");
            fieldHistoryList.Add(empty);
            return;
        }

        for (var index = 0; index < fieldInspections.Count; index++)
        {
            var inspection = fieldInspections[index];
            var row = new VisualElement();
            row.AddToClassList("field-table-row");
            row.Add(CreateFieldTableLabel((index + 1).ToString(), "field-table-index"));

            var status = new Button(() =>
                SetInspectionResolved(inspection, !inspection.IsResolved))
            {
                text = inspection.IsResolved ? "Đã xử lý" : "Chưa xử lý",
                tooltip = inspection.IsResolved
                    ? "Đánh dấu lại là chưa xử lý"
                    : "Đánh dấu sự cố đã xử lý"
            };
            status.AddToClassList("field-table-cell");
            status.AddToClassList("field-table-status");
            status.AddToClassList(inspection.IsResolved
                ? "field-status-resolved"
                : "field-status-open");
            row.Add(status);
            row.Add(CreateFieldTableLabel(inspection.ElementName, "field-table-name"));
            row.Add(CreateFieldTableLabel(inspection.ProjectName, "field-table-project"));

            var elementText = string.IsNullOrWhiteSpace(inspection.ElementKey)
                ? "Chưa liên kết"
                : $"{Path.GetFileNameWithoutExtension(inspection.SourceFile)}\n#{inspection.ElementKey}";
            row.Add(CreateFieldTableLabel(elementText, "field-table-element"));
            row.Add(CreateFieldTableLabel(
                FormatInspectionTime(inspection.CreatedAt),
                "field-table-time"));
            row.Add(CreateFieldTableLabel(
                $"{inspection.Latitude:F6}, {inspection.Longitude:F6}\n{inspection.Elevation:F1} m",
                "field-table-coordinate"));

            fieldHistoryList.Add(row);
        }
    }

    private static Label CreateFieldTableLabel(string text, string className)
    {
        var label = new Label(string.IsNullOrWhiteSpace(text) ? "-" : text);
        label.AddToClassList("field-table-cell");
        label.AddToClassList(className);
        return label;
    }

    private void SetInspectionResolved(FieldInspectionRecord inspection, bool resolved)
    {
        if (operationsDatabase == null ||
            !operationsDatabase.SetFieldInspectionResolved(inspection.Id, resolved))
        {
            SetImportStatus("Không thể cập nhật tình trạng xử lý trong SQLite.");
            return;
        }

        LoadInspectionsFromDatabase();
        BuildFieldHistory();
        BuildInspectionPopup();
        BuildInspectionMarkers();
        if (selectedRecord != null)
        {
            BuildElementInspectionHistory(selectedRecord);
        }

        UpdateModuleCounts();
        SetImportStatus(resolved
            ? "Đã đánh dấu ghi nhận là đã xử lý."
            : "Đã chuyển ghi nhận về trạng thái chưa xử lý.");
    }

    private static string FormatInspectionTime(string value)
    {
        return DateTime.TryParse(value, out var timestamp)
            ? timestamp.ToString("dd/MM/yyyy HH:mm")
            : value;
    }

    private void BuildInspectionPopup()
    {
        if (inspectionPopupList == null)
        {
            return;
        }

        inspectionPopupList.Clear();
        if (fieldInspections.Count == 0)
        {
            var empty = new Label("Chưa có điểm kiểm tra hiện trường.");
            empty.AddToClassList("category-empty");
            inspectionPopupList.Add(empty);
            return;
        }

        foreach (var inspection in fieldInspections)
        {
            var row = new VisualElement();
            row.AddToClassList("inspection-popup-row");
            var mark = new Label(inspection.IsResolved ? "✓" : "!");
            mark.AddToClassList("inspection-pin-mark");
            if (inspection.IsResolved)
            {
                mark.AddToClassList("inspection-pin-resolved");
            }
            var copy = new VisualElement();
            copy.AddToClassList("inspection-popup-copy");
            var name = new Label(inspection.ElementName);
            name.AddToClassList("inspection-popup-name");
            var meta = new Label(
                $"{(inspection.IsResolved ? "Đã xử lý" : "Chưa xử lý")}  |  " +
                $"{inspection.Latitude:F5}, {inspection.Longitude:F5}");
            meta.AddToClassList("inspection-popup-meta");
            var command = new Label("MỞ");
            command.AddToClassList("inspection-popup-command");
            copy.Add(name);
            copy.Add(meta);
            row.Add(mark);
            row.Add(copy);
            row.Add(command);
            row.RegisterCallback<ClickEvent>(_ => FocusInspection(inspection));
            inspectionPopupList.Add(row);
        }
    }

    private void ToggleInspectionPopup()
    {
        BuildInspectionPopup();
        TogglePopup(inspectionPopup);
    }

    private void OpenInspectionInReport(FieldInspectionRecord inspection)
    {
        ShowModule(DashboardModule.Report);
        root.schedule.Execute(() => FocusInspection(inspection));
    }

    private void FocusInspection(FieldInspectionRecord inspection)
    {
        HidePopups();
        var record = FindAssetForInspection(inspection);
        if (record != null)
        {
            SelectRecord(record, true);
            SetImportStatus($"Đã mở cấu kiện cho ghi nhận: {inspection.ElementName}");
            return;
        }

        if (inspectionMarkers.TryGetValue(inspection.Id, out var marker) &&
            marker != null && orbitCamera != null)
        {
            orbitCamera.pivotPoint = marker.transform.position;
            orbitCamera.distance = Mathf.Clamp(
                65f,
                orbitCamera.minDistance,
                orbitCamera.maxDistance);
            SetImportStatus($"Đã chuyển đến điểm hiện trường: {inspection.ElementName}");
        }
    }

    private IfcAssetRecord FindAssetForInspection(FieldInspectionRecord inspection)
    {
        if (string.IsNullOrWhiteSpace(inspection.ElementKey))
        {
            return null;
        }

        return records.FirstOrDefault(record =>
            SourceFileMatches(record.SourceFile, inspection.SourceFile) &&
            ElementKeyMatches(record.Metadata, inspection.ElementKey));
    }

    private static bool SourceFileMatches(string recordSource, string inspectionSource)
    {
        if (string.IsNullOrWhiteSpace(inspectionSource))
        {
            return true;
        }

        if (string.Equals(recordSource, inspectionSource, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return string.Equals(
            Path.GetFileNameWithoutExtension(recordSource),
            Path.GetFileNameWithoutExtension(inspectionSource),
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool ElementKeyMatches(IfcElementMetadata metadata, string elementKey)
    {
        if (metadata == null || string.IsNullOrWhiteSpace(elementKey))
        {
            return false;
        }

        var normalized = elementKey.Trim().TrimStart('#');
        return string.Equals(
                   GetElementKey(metadata).TrimStart('#'),
                   normalized,
                   StringComparison.OrdinalIgnoreCase) ||
               string.Equals(
                   metadata.GlobalId,
                   normalized,
                   StringComparison.OrdinalIgnoreCase) ||
               string.Equals(
                   metadata.EntityLabel.ToString(CultureInfo.InvariantCulture),
                   normalized,
                   StringComparison.OrdinalIgnoreCase);
    }

    private void BuildInspectionMarkers()
    {
        DestroyInspectionMarkers();
        if (fieldInspections.Count == 0 || viewingCamera == null)
        {
            return;
        }

        foreach (var inspection in fieldInspections)
        {
            var asset = FindAssetForInspection(inspection);
            IfcInspectionMarker marker;
            if (asset != null)
            {
                var assetBounds = RefreshLiveBounds(asset);
                var markerPosition = assetBounds.center +
                                     Vector3.up * Mathf.Max(2f, assetBounds.extents.y + 2f);
                marker = IfcInspectionMarker.Create(
                    asset.Metadata.transform,
                    markerPosition,
                    inspection.Id,
                    asset.Metadata,
                    viewingCamera,
                    inspection.ElementName,
                    inspection.IsResolved);
            }
            else
            {
                ResolveDependencies();
                var anchor = arcGisMapLoader?.CreateGeographicAnchor(
                    $"Inspection {inspection.Id} ArcGIS Anchor",
                    inspection.Latitude,
                    inspection.Longitude,
                    inspection.Elevation);
                if (anchor == null)
                {
                    continue;
                }

                geographicMarkerAnchors.Add(anchor.gameObject);
                marker = IfcInspectionMarker.Create(
                    anchor,
                    anchor.position + Vector3.up * 3f,
                    inspection.Id,
                    null,
                    viewingCamera,
                    inspection.ElementName,
                    inspection.IsResolved);
            }

            inspectionMarkers[inspection.Id] = marker;
        }

        if (inspectionMarkerLinkRoutine != null)
        {
            StopCoroutine(inspectionMarkerLinkRoutine);
        }

        inspectionMarkerLinkRoutine = StartCoroutine(LinkUnassignedInspectionMarkers());
    }

    private IEnumerator LinkUnassignedInspectionMarkers()
    {
        var databaseUpdated = false;
        for (var attempt = 0; attempt < 8; attempt++)
        {
            if (attempt == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.25f);
            }

            var pendingCount = 0;
            foreach (var pair in inspectionMarkers)
            {
                var marker = pair.Value;
                if (marker == null)
                {
                    continue;
                }

                var inspection = fieldInspections.FirstOrDefault(record =>
                    record.Id == pair.Key);
                if (inspection.Id == 0)
                {
                    continue;
                }

                IfcAssetRecord record = null;
                if (marker.LinkedElement != null)
                {
                    recordsByMetadata.TryGetValue(marker.LinkedElement, out record);
                }
                else
                {
                    pendingCount++;
                    record = FindNearestAssetForMarker(marker.transform.position, inspection);
                }

                if (record == null)
                {
                    continue;
                }

                marker.AssignLinkedElement(record.Metadata);
                if ((!SourceFileMatches(record.SourceFile, inspection.SourceFile) ||
                     !ElementKeyMatches(record.Metadata, inspection.ElementKey)) &&
                    operationsDatabase != null &&
                    operationsDatabase.UpdateFieldInspectionLink(
                        inspection.Id,
                        record.SourceFile,
                        GetElementKey(record.Metadata)))
                {
                    databaseUpdated = true;
                }

                pendingCount--;
            }

            if (pendingCount == 0)
            {
                break;
            }
        }

        inspectionMarkerLinkRoutine = null;
        if (!databaseUpdated)
        {
            yield break;
        }

        LoadInspectionsFromDatabase();
        BuildInspectionPopup();
        BuildFieldHistory();
        if (selectedRecord != null)
        {
            BuildElementInspectionHistory(selectedRecord);
        }
    }

    private IfcAssetRecord FindNearestAssetForMarker(
        Vector3 markerPosition,
        FieldInspectionRecord inspection)
    {
        IfcAssetRecord nearest = null;
        var nearestScore = 250f * 250f;
        foreach (var record in records)
        {
            if (record == null || record.Metadata == null)
            {
                continue;
            }

            var bounds = RefreshLiveBounds(record);
            var closest = bounds.ClosestPoint(markerPosition);
            var horizontal = new Vector2(
                markerPosition.x - closest.x,
                markerPosition.z - closest.z);
            var vertical = markerPosition.y >= bounds.max.y
                ? markerPosition.y - bounds.max.y
                : markerPosition.y < bounds.min.y
                    ? bounds.min.y - markerPosition.y
                    : 0f;
            var score = horizontal.sqrMagnitude + vertical * vertical;
            if (NamesLikelyMatch(record.Name, inspection.ElementName))
            {
                score *= 0.2f;
            }

            score += bounds.size.sqrMagnitude * 0.00000001f;
            if (score >= nearestScore)
            {
                continue;
            }

            nearest = record;
            nearestScore = score;
        }

        return nearest;
    }

    private static bool NamesLikelyMatch(string recordName, string inspectionName)
    {
        if (string.IsNullOrWhiteSpace(recordName) ||
            string.IsNullOrWhiteSpace(inspectionName))
        {
            return false;
        }

        return recordName.IndexOf(
                   inspectionName,
                   StringComparison.CurrentCultureIgnoreCase) >= 0 ||
               inspectionName.IndexOf(
                   recordName,
                   StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    private void BuildElementInspectionHistory(IfcAssetRecord record)
    {
        if (elementInspectionList == null || record == null)
        {
            return;
        }

        var history = fieldInspections.Where(inspection =>
                SourceFileMatches(record.SourceFile, inspection.SourceFile) &&
                ElementKeyMatches(record.Metadata, inspection.ElementKey))
            .OrderByDescending(inspection => inspection.CreatedAt)
            .ToArray();

        elementInspectionTotal.text = history.Length.ToString("N0");
        elementInspectionOpen.text = history.Count(item => !item.IsResolved).ToString("N0");
        elementInspectionResolved.text = history.Count(item => item.IsResolved).ToString("N0");
        elementInspectionList.Clear();
        if (history.Length == 0)
        {
            var empty = new Label("Cấu kiện chưa có lịch sử kiểm tra kỹ thuật.");
            empty.AddToClassList("element-inspection-empty");
            elementInspectionList.Add(empty);
            return;
        }

        foreach (var inspection in history)
        {
            var item = new VisualElement();
            item.AddToClassList("element-inspection-item");

            var heading = new VisualElement();
            heading.AddToClassList("element-inspection-item-heading");
            var time = new Label(FormatInspectionTime(inspection.CreatedAt));
            time.AddToClassList("element-inspection-time");
            var status = new Button(() =>
                SetInspectionResolved(inspection, !inspection.IsResolved))
            {
                text = inspection.IsResolved ? "Đã xử lý" : "Chưa xử lý"
            };
            status.AddToClassList("element-inspection-status");
            status.AddToClassList(inspection.IsResolved
                ? "element-inspection-status-resolved"
                : "element-inspection-status-open");
            heading.Add(time);
            heading.Add(status);
            item.Add(heading);

            var title = new Label(inspection.ElementName);
            title.AddToClassList("element-inspection-item-title");
            item.Add(title);
            if (!string.IsNullOrWhiteSpace(inspection.Note))
            {
                var note = new Label(inspection.Note);
                note.AddToClassList("element-inspection-note");
                item.Add(note);
            }

            var location = new Label(
                $"{inspection.Latitude:F6}, {inspection.Longitude:F6} • {inspection.Elevation:F1} m");
            location.AddToClassList("element-inspection-location");
            item.Add(location);
            if (!string.IsNullOrWhiteSpace(inspection.ImagePath) &&
                File.Exists(inspection.ImagePath))
            {
                var imageButton = new Button(() =>
                    Application.OpenURL(new Uri(inspection.ImagePath).AbsoluteUri))
                {
                    text = "Mở ảnh hiện trường"
                };
                imageButton.AddToClassList("element-inspection-image-button");
                item.Add(imageButton);
            }

            elementInspectionList.Add(item);
        }
    }

    private void DestroyInspectionMarkers()
    {
        if (inspectionMarkerLinkRoutine != null)
        {
            StopCoroutine(inspectionMarkerLinkRoutine);
            inspectionMarkerLinkRoutine = null;
        }

        foreach (var marker in inspectionMarkers.Values)
        {
            if (marker != null)
            {
                Destroy(marker.gameObject);
            }
        }

        foreach (var anchor in geographicMarkerAnchors)
        {
            if (anchor != null)
            {
                Destroy(anchor);
            }
        }

        inspectionMarkers.Clear();
        geographicMarkerAnchors.Clear();
    }

    private bool TrySelectInspectionMarker(Vector2 pointerPosition)
    {
        if (viewingCamera == null)
        {
            return false;
        }

        var hits = Physics.RaycastAll(
            viewingCamera.ScreenPointToRay(pointerPosition),
            Mathf.Infinity,
            ~0,
            QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
        foreach (var hit in hits)
        {
            var marker = hit.collider.GetComponentInParent<IfcInspectionMarker>();
            if (marker == null)
            {
                continue;
            }

            var inspection = fieldInspections.FirstOrDefault(record =>
                record.Id == marker.InspectionId);
            if (inspection.Id == 0)
            {
                return false;
            }

            if (marker.LinkedElement != null &&
                recordsByMetadata.TryGetValue(marker.LinkedElement, out var linkedRecord))
            {
                SelectRecord(linkedRecord, true);
                SetImportStatus($"Đã mở và làm nổi bật cấu kiện gắn với kiểm tra: {inspection.ElementName}");
                return true;
            }

            FocusInspection(inspection);
            return true;
        }

        return false;
    }

    private void ReleaseModuleResources()
    {
        DestroyInspectionMarkers();
        if (inspectionPreviewTexture != null)
        {
            Destroy(inspectionPreviewTexture);
            inspectionPreviewTexture = null;
        }
    }
}
