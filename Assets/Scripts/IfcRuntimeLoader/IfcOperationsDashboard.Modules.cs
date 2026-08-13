using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CauDuong.IfcOperations;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class IfcOperationsDashboard
{
    private enum DashboardModule
    {
        Home,
        Data,
        Field,
        Report,
        Dashboard
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

    private static readonly List<string> FieldInspectionTypeChoices = new()
    {
        "Tất cả loại cấu kiện",
        "Dầm",
        "Trụ",
        "Mố",
        "Móng cọc",
        "Mặt đường",
        "Ta luy",
        "Hộ lan",
        "Cột đèn",
        "Biển báo",
        "Vạch sơn",
        "Thoát nước",
        "Khác"
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
    private VisualElement inspectionDetailOverlay;
    private VisualElement inspectionDetailImage;
    private VisualElement inspectionDetailImagePlaceholder;
    private VisualElement elementInspectionHistory;
    private VisualElement elementInspectionList;
    private Label homeModelCount;
    private Label homeInspectionCount;
    private Label homeElementCount;
    private Label registryCountLabel;
    private Label fieldRecordCountLabel;
    private Label selectedIfcPathLabel;
    private Label modelUploadError;
    private Label modelUploadTitle;
    private Label inspectionImagePathLabel;
    private Label inspectionFormError;
    private Label inspectionFormTitle;
    private Label elementInspectionTotal;
    private Label elementInspectionOpen;
    private Label elementInspectionResolved;
    private Label inspectionDetailCreatedAt;
    private Label inspectionDetailStatus;
    private Label inspectionDetailName;
    private Label inspectionDetailType;
    private Label inspectionDetailCreator;
    private Label inspectionDetailProject;
    private Label inspectionDetailElement;
    private Label inspectionDetailCoordinate;
    private Label inspectionDetailNote;
    private Button inspectionButton;
    private Button submitInspectionButton;
    private Button chooseIfcFileButton;
    private Button saveModelUploadButton;
    private Button inspectionDetailDeleteButton;
    private TextField filterProjectInput;
    private TextField filterProvinceInput;
    private TextField filterWardInput;
    private TextField filterFileInput;
    private DropdownField modelProjectInput;
    private DropdownField modelProvinceInput;
    private DropdownField homeProjectDropdown;
    private DropdownField fieldProjectFilter;
    private DropdownField fieldTypeFilter;
    private Button fieldPointsToggleButton;
    private TextField modelWardInput;
    private TextField modelUnitInput;
    private DropdownField inspectionProjectDropdown;
    private DropdownField inspectionStatusDropdown;
    private TextField inspectionNameInput;
    private TextField inspectionElementTypeInput;
    private TextField inspectionCreatedByInput;
    private TextField inspectionLatitudeInput;
    private TextField inspectionLongitudeInput;
    private TextField inspectionElevationInput;
    private TextField inspectionNoteInput;
    private Texture2D inspectionPreviewTexture;
    private Texture2D inspectionDetailTexture;
    private IfcAssetRecord inspectionLinkedRecord;
    private Coroutine inspectionMarkerLinkRoutine;
    private DashboardModule activeModule;
    private bool moduleUiBound;
    private string selectedIfcPath;
    private string selectedInspectionImagePath;
    private IfcModelRegistryRecord? editingRegistryModel;
    private FieldInspectionRecord? editingInspection;
    private long displayedInspectionId;
    private FieldInspectionRecord? displayedInspection;
    private bool inspectionDetailDeleteArmed;
    private bool fieldPointsVisible = true;

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
        inspectionDetailOverlay = root.Q<VisualElement>("inspection-detail-overlay");
        inspectionDetailImage = root.Q<VisualElement>("inspection-detail-image");
        inspectionDetailImagePlaceholder =
            root.Q<VisualElement>("inspection-detail-image-placeholder");
        elementInspectionHistory = root.Q<VisualElement>("element-inspection-history");
        elementInspectionList = root.Q<VisualElement>("element-inspection-list");
        homeModelCount = root.Q<Label>("home-model-count");
        homeInspectionCount = root.Q<Label>("home-inspection-count");
        homeElementCount = root.Q<Label>("home-element-count");
        registryCountLabel = root.Q<Label>("registry-count");
        fieldRecordCountLabel = root.Q<Label>("field-record-count");
        selectedIfcPathLabel = root.Q<Label>("selected-ifc-path");
        modelUploadError = root.Q<Label>("model-upload-error");
        modelUploadTitle = root.Q<Label>("model-upload-title");
        inspectionImagePathLabel = root.Q<Label>("inspection-image-path");
        inspectionFormError = root.Q<Label>("inspection-form-error");
        inspectionFormTitle = root.Q<Label>("inspection-form-title");
        elementInspectionTotal = root.Q<Label>("element-inspection-total");
        elementInspectionOpen = root.Q<Label>("element-inspection-open");
        elementInspectionResolved = root.Q<Label>("element-inspection-resolved");
        inspectionDetailCreatedAt = root.Q<Label>("inspection-detail-created-at");
        inspectionDetailStatus = root.Q<Label>("inspection-detail-status");
        inspectionDetailName = root.Q<Label>("inspection-detail-name");
        inspectionDetailType = root.Q<Label>("inspection-detail-type");
        inspectionDetailCreator = root.Q<Label>("inspection-detail-creator");
        inspectionDetailProject = root.Q<Label>("inspection-detail-project");
        inspectionDetailElement = root.Q<Label>("inspection-detail-element");
        inspectionDetailCoordinate = root.Q<Label>("inspection-detail-coordinate");
        inspectionDetailNote = root.Q<Label>("inspection-detail-note");
        inspectionButton = root.Q<Button>("inspection-button");
        submitInspectionButton = root.Q<Button>("submit-inspection-button");
        chooseIfcFileButton = root.Q<Button>("choose-ifc-file-button");
        saveModelUploadButton = root.Q<Button>("save-model-upload-button");
        inspectionDetailDeleteButton = root.Q<Button>("inspection-detail-delete-button");
        filterProjectInput = root.Q<TextField>("filter-project-input");
        filterProvinceInput = root.Q<TextField>("filter-province-input");
        filterWardInput = root.Q<TextField>("filter-ward-input");
        filterFileInput = root.Q<TextField>("filter-file-input");
        modelProjectInput = root.Q<DropdownField>("model-project-input");
        modelProvinceInput = root.Q<DropdownField>("model-province-input");
        homeProjectDropdown = root.Q<DropdownField>("home-project-dropdown");
        fieldProjectFilter = root.Q<DropdownField>("field-project-filter");
        fieldTypeFilter = root.Q<DropdownField>("field-type-filter");
        fieldPointsToggleButton = root.Q<Button>("field-points-toggle-button");
        modelWardInput = root.Q<TextField>("model-ward-input");
        modelUnitInput = root.Q<TextField>("model-unit-input");
        inspectionProjectDropdown = root.Q<DropdownField>("inspection-project-dropdown");
        inspectionStatusDropdown = root.Q<DropdownField>("inspection-status-dropdown");
        inspectionNameInput = root.Q<TextField>("inspection-name-input");
        inspectionElementTypeInput = root.Q<TextField>("inspection-element-type-input");
        inspectionCreatedByInput = root.Q<TextField>("inspection-created-by-input");
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
        root.Q<Button>("home-dashboard-button").clicked +=
            () => ShowModule(DashboardModule.Dashboard);
        root.Q<Button>("data-home-button").clicked +=
            () => ShowModule(DashboardModule.Home);
        root.Q<Button>("field-home-button").clicked +=
            () => ShowModule(DashboardModule.Home);
        root.Q<Button>("report-home-button").clicked +=
            () => ShowModule(DashboardModule.Home);
        root.Q<Button>("open-model-upload-button").clicked += OpenModelUpload;
        root.Q<Button>("close-model-upload-button").clicked += CloseModelUpload;
        root.Q<Button>("cancel-model-upload-button").clicked += CloseModelUpload;
        chooseIfcFileButton.clicked += ChooseIfcForRegistry;
        saveModelUploadButton.clicked += SaveModelUpload;
        root.Q<Button>("open-inspection-form-button").clicked +=
            () => OpenInspectionDialog(false);
        root.Q<Button>("close-inspection-form-button").clicked += CloseInspectionDialog;
        root.Q<Button>("cancel-inspection-form-button").clicked += CloseInspectionDialog;
        root.Q<Button>("choose-inspection-image-button").clicked += ChooseInspectionImage;
        root.Q<Button>("use-selected-element-button").clicked += UseSelectedElement;
        submitInspectionButton.clicked += SaveInspection;
        root.Q<Button>("close-inspection-detail-button").clicked +=
            CloseInspectionDetails;
        root.Q<Button>("inspection-detail-map-button").clicked += OpenDisplayedInspectionOnMap;
        root.Q<Button>("inspection-detail-edit-button").clicked += EditDisplayedInspection;
        inspectionDetailDeleteButton.clicked += DeleteDisplayedInspection;
        root.Q<Button>("inspection-button").clicked += ToggleInspectionPopup;
        root.Q<Button>("open-field-from-report-button").clicked +=
            () => OpenInspectionDialogFromReport(false);
        filterProjectInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterProvinceInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterWardInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterFileInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        fieldProjectFilter.RegisterValueChangedCallback(_ => BuildFieldHistory());
        fieldTypeFilter.RegisterValueChangedCallback(_ => BuildFieldHistory());
        root.Q<Button>("field-reload-button").clicked += ReloadFieldInspections;
        fieldPointsToggleButton.clicked += ToggleFieldInspectionPoints;

        modelProvinceInput.choices = VietnamProvinceChoices;
        modelProvinceInput.SetValueWithoutNotify("Thành phố Hà Nội");
        modelWardInput.SetValueWithoutNotify(string.Empty);
        modelUnitInput.SetValueWithoutNotify("Ban Quản lý Dự án Đầu tư Xây dựng");
        inspectionStatusDropdown.choices = InspectionStatusChoices;
        inspectionStatusDropdown.index = 0;
        inspectionStatusDropdown.SetEnabled(false);
        fieldTypeFilter.choices = FieldInspectionTypeChoices;
        fieldTypeFilter.SetValueWithoutNotify(FieldInspectionTypeChoices[0]);
        UpdateFieldProjectFilterChoices();
        UpdateFieldPointsToggle();
        modelUploadOverlay.style.display = DisplayStyle.None;
        inspectionFormOverlay.style.display = DisplayStyle.None;
        inspectionDetailOverlay.style.display = DisplayStyle.None;
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
        if (module != DashboardModule.Dashboard && analyticsOverlay != null)
        {
            analyticsOverlay.style.display = DisplayStyle.None;
            analyticsOverlay.RemoveFromClassList("analytics-standalone");
        }
        homePage.style.display = module == DashboardModule.Home
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        dataPage.style.display = module == DashboardModule.Data
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        fieldPage.style.display = module == DashboardModule.Field
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        reportPage.style.display = module is DashboardModule.Report or DashboardModule.Dashboard
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
            UpdateFieldProjectFilterChoices();
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
        else if (module == DashboardModule.Dashboard)
        {
            LoadInspectionsFromDatabase();
            RefreshDashboard();
            BuildInspectionMarkers();
            OpenAnalyticsDashboard(true);
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
        editingInspection = null;
        inspectionLinkedRecord = null;
        inspectionNameInput.SetValueWithoutNotify(string.Empty);
        inspectionElementTypeInput.SetValueWithoutNotify(string.Empty);
        inspectionCreatedByInput.SetValueWithoutNotify("Người dùng hiện trường");
        inspectionNoteInput.SetValueWithoutNotify(string.Empty);
        inspectionLatitudeInput.SetValueWithoutNotify(string.Empty);
        inspectionLongitudeInput.SetValueWithoutNotify(string.Empty);
        inspectionElevationInput.SetValueWithoutNotify("0");
        inspectionStatusDropdown.index = 0;
        inspectionFormError.text = string.Empty;
        inspectionFormTitle.text = "Tạo Ghi Nhận Hiện Trường";
        submitInspectionButton.text = "Lưu Ghi Nhận";
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

    private FieldInspectionRecord FindNewestInspectionForElement(IfcAssetRecord record)
    {
        if (record?.Metadata == null)
        {
            return default;
        }

        return fieldInspections.FirstOrDefault(inspection =>
            SourceFileMatches(record.SourceFile, inspection.SourceFile) &&
            ElementKeyMatches(record.Metadata, inspection.ElementKey));
    }

    private int ResolveNewestInspectionsAfterReportExport()
    {
        if (operationsDatabase == null || records.Count == 0 || fieldInspections.Count == 0)
        {
            return 0;
        }

        var resolvedIds = new HashSet<long>();
        foreach (var record in records)
        {
            var newestInspection = FindNewestInspectionForElement(record);
            if (newestInspection.Id <= 0 || newestInspection.IsResolved ||
                !resolvedIds.Add(newestInspection.Id))
            {
                continue;
            }

            if (!operationsDatabase.SetFieldInspectionResolved(newestInspection.Id, true))
            {
                resolvedIds.Remove(newestInspection.Id);
            }
        }

        if (resolvedIds.Count > 0)
        {
            RefreshInspectionViews();
        }

        return resolvedIds.Count;
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
        UpdateFieldProjectFilterChoices();
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
        fieldRecordCountLabel.text = $"{fieldInspections.Count:N0} báo cáo • ✓ Đã đồng bộ";
        inspectionButton.text = $"Điểm Báo Cáo 3D ({fieldInspections.Count:N0})";
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

            var fileName = !string.IsNullOrWhiteSpace(record.StoredFileName)
                ? record.StoredFileName
                : Path.GetFileName(record.IfcPath);
            var fileLabel = CreateDataLabel($"▤  {fileName}", "data-cell-file");
            fileLabel.tooltip = record.IfcPath;
            row.Add(fileLabel);
            row.Add(CreateDataLabel(record.ProjectName, "data-cell-project"));
            row.Add(CreateDataLabel(
                string.Join(", ", new[] { record.Ward, record.Province }
                    .Where(value => !string.IsNullOrWhiteSpace(value))),
                "data-cell-location"));
            row.Add(CreateDataLabel(record.ManagingUnit, "data-cell-unit"));
            row.Add(CreateDataLabel(FormatRegistryDate(record.CreatedAt), "data-cell-date"));
            row.Add(CreateDataLabel(
                record.StoredFileSize > 0
                    ? FormatFileSize(record.StoredFileSize)
                    : File.Exists(record.IfcPath)
                        ? FormatFileSize(new FileInfo(record.IfcPath).Length)
                        : "-",
                "data-cell-size"));
            row.Add(CreateRegistryState(record));
            row.Add(CreateRegistryActions(record));
            registryList.Add(row);
        }
    }

    private VisualElement CreateRegistryState(IfcModelRegistryRecord record)
    {
        var cell = new VisualElement();
        cell.AddToClassList("data-cell");
        cell.AddToClassList("data-cell-state");
        var state = new Button(() => SetRegistryModelEnabled(record, !record.IsEnabled))
        {
            text = record.IsEnabled ? "Đã render" : "Tạm dừng",
            tooltip = record.IsEnabled
                ? "Nhấn để tạm dừng mô hình trong báo cáo"
                : "Nhấn để nạp mô hình vào báo cáo"
        };
        state.AddToClassList("registry-state-pill");
        state.AddToClassList(record.IsEnabled
            ? "registry-state-ready"
            : "registry-state-paused");
        cell.Add(state);
        return cell;
    }

    private VisualElement CreateRegistryActions(IfcModelRegistryRecord record)
    {
        var actions = new VisualElement();
        actions.AddToClassList("data-cell");
        actions.AddToClassList("data-cell-action");
        actions.Add(CreateRegistryActionButton(
            "Xem 3D",
            "registry-view-button",
            () => OpenRegistryModelInReport(record),
            "Mở mô hình trong không gian báo cáo"));
        actions.Add(CreateRegistryActionButton(
            "Sửa",
            "registry-edit-button",
            () => OpenRegistryEditor(record),
            "Cập nhật thông tin mô hình"));
        actions.Add(CreateRegistryActionButton(
            "↓",
            "registry-download-button",
            () => ExportRegistryModel(record),
            "Tải file IFC"));

        var deleteArmed = false;
        Button delete = null;
        delete = CreateRegistryActionButton(
            "×",
            "registry-delete-button",
            () =>
            {
                if (!deleteArmed)
                {
                    deleteArmed = true;
                    delete.text = "?";
                    delete.AddToClassList("registry-delete-confirm");
                    delete.schedule.Execute(() =>
                    {
                        deleteArmed = false;
                        delete.text = "×";
                        delete.RemoveFromClassList("registry-delete-confirm");
                    }).StartingIn(3500);
                    return;
                }

                DeleteRegistryModel(record);
            },
            "Xóa mô hình khỏi kho dữ liệu");
        actions.Add(delete);
        return actions;
    }

    private static Button CreateRegistryActionButton(
        string text,
        string variantClass,
        Action clicked,
        string tooltip)
    {
        var button = new Button(clicked)
        {
            text = text,
            tooltip = tooltip
        };
        button.AddToClassList("registry-action-button");
        button.AddToClassList(variantClass);
        return button;
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

    private static string FormatRegistryDate(string value)
    {
        return DateTime.TryParse(value, out var timestamp)
            ? timestamp.ToString("yyyy-MM-dd")
            : "-";
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
        editingRegistryModel = null;
        UpdateModelProjectChoices();
        selectedIfcPath = string.Empty;
        selectedIfcPathLabel.text = "Chưa chọn file .ifc";
        modelUploadTitle.text = "Thêm Mô Hình IFC";
        saveModelUploadButton.text = "Thêm & Nạp Mô Hình";
        chooseIfcFileButton.SetEnabled(true);
        modelUploadError.text = string.Empty;
        modelUploadOverlay.style.display = DisplayStyle.Flex;
    }

    private void OpenRegistryEditor(IfcModelRegistryRecord record)
    {
        editingRegistryModel = record;
        UpdateModelProjectChoices();
        if (!modelProjectInput.choices.Contains(record.ProjectName))
        {
            modelProjectInput.choices = modelProjectInput.choices
                .Append(record.ProjectName)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        modelProjectInput.SetValueWithoutNotify(record.ProjectName);
        if (!modelProvinceInput.choices.Contains(record.Province))
        {
            modelProvinceInput.choices = modelProvinceInput.choices
                .Append(record.Province)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        modelProvinceInput.SetValueWithoutNotify(record.Province);
        modelWardInput.SetValueWithoutNotify(record.Ward);
        modelUnitInput.SetValueWithoutNotify(record.ManagingUnit);
        selectedIfcPath = record.IfcPath;
        selectedIfcPathLabel.text = string.IsNullOrWhiteSpace(record.StoredFileName)
            ? Path.GetFileName(record.IfcPath)
            : record.StoredFileName;
        selectedIfcPathLabel.tooltip = record.IfcPath;
        modelUploadTitle.text = "Cập Nhật Mô Hình IFC";
        saveModelUploadButton.text = "Lưu Thay Đổi";
        chooseIfcFileButton.SetEnabled(false);
        modelUploadError.text = string.Empty;
        modelUploadOverlay.style.display = DisplayStyle.Flex;
        modelUploadOverlay.BringToFront();
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
            (!editingRegistryModel.HasValue &&
             (string.IsNullOrWhiteSpace(selectedIfcPath) || !File.Exists(selectedIfcPath))))
        {
            modelUploadError.text = "Vui lòng nhập dự án, tỉnh/thành phố và chọn file IFC hợp lệ.";
            return;
        }

        try
        {
            if (editingRegistryModel.HasValue)
            {
                var existing = editingRegistryModel.Value;
                var updated = new IfcModelRegistryRecord(
                    existing.Id,
                    project,
                    province,
                    modelWardInput.value?.Trim(),
                    modelUnitInput.value?.Trim(),
                    existing.IfcPath,
                    existing.IsEnabled,
                    existing.CreatedAt,
                    existing.UpdatedAt,
                    existing.StoredFileName,
                    existing.HasStoredFile,
                    existing.StoredFileSize);
                if (operationsDatabase == null ||
                    !operationsDatabase.SaveModelRegistry(updated))
                {
                    modelUploadError.text =
                        "Không thể cập nhật thông tin mô hình trong SQLite.";
                    return;
                }

                CloseModelUpload();
                editingRegistryModel = null;
                LoadRegistryFromDatabase();
                BuildRegistryList();
                UpdateModelProjectChoices();
                UpdateInspectionProjectChoices();
                UpdateModuleCounts();
                SetImportStatus($"Đã cập nhật thông tin {Path.GetFileName(existing.IfcPath)}.");
                return;
            }

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

        if (homeProjectDropdown != null)
        {
            var homeCurrent = homeProjectDropdown.value;
            var homeChoices = new List<string> { "Tất Cả Dự Án" };
            homeChoices.AddRange(choices);
            homeProjectDropdown.choices = homeChoices;
            homeProjectDropdown.SetValueWithoutNotify(
                homeChoices.Contains(homeCurrent) ? homeCurrent : homeChoices[0]);
        }
    }

    private void ChooseInspectionImage()
    {
        try
        {
            ResolveDependencies();
            var selectedPath = runtimeLoader?.SelectImageFile();
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return;
            }

            var path = Path.GetFullPath(selectedPath.Trim().Trim('"'));
            if (!File.Exists(path))
            {
                inspectionFormError.text = "Không tìm thấy ảnh hiện trường đã chọn.";
                return;
            }

            selectedInspectionImagePath = path;
            inspectionImagePathLabel.text = Path.GetFileName(path);
            inspectionImagePathLabel.tooltip = path;
            inspectionFormError.text = string.Empty;
            ShowInspectionPreview(path);
        }
        catch (Exception exception)
        {
            inspectionFormError.text = $"Không thể mở ảnh đã chọn: {exception.Message}";
            Debug.LogException(exception);
        }
    }

    private void ShowInspectionPreview(string path)
    {
        if (inspectionPreviewTexture != null)
        {
            Destroy(inspectionPreviewTexture);
        }

        try
        {
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
        catch (Exception exception)
        {
            if (inspectionPreviewTexture != null)
            {
                Destroy(inspectionPreviewTexture);
                inspectionPreviewTexture = null;
            }

            inspectionFormError.text = $"Không thể đọc ảnh hiện trường: {exception.Message}";
        }
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
        inspectionElementTypeInput.SetValueWithoutNotify(
            EmptyFallback(selectedRecord.IfcType, "Cấu kiện IFC"));
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
            var existing = editingInspection;
            var hasNewImage = !string.IsNullOrWhiteSpace(selectedInspectionImagePath);
            var imagePath = hasNewImage
                ? PersistInspectionImage(selectedInspectionImagePath)
                : existing?.ImagePath ?? string.Empty;
            var isResolved = existing?.IsResolved ?? false;
            var record = new FieldInspectionRecord(
                existing?.Id ?? 0,
                inspectionProjectDropdown.value,
                inspectionLinkedRecord?.SourceFile ?? existing?.SourceFile ?? string.Empty,
                inspectionLinkedRecord != null
                    ? GetElementKey(inspectionLinkedRecord.Metadata)
                    : existing?.ElementKey ?? string.Empty,
                name,
                inspectionElementTypeInput.value?.Trim(),
                inspectionCreatedByInput.value?.Trim(),
                latitude,
                longitude,
                elevation,
                imagePath,
                inspectionNoteInput.value?.Trim(),
                existing?.CreatedAt ?? string.Empty,
                isResolved,
                isResolved ? existing?.ResolvedAt ?? string.Empty : string.Empty);
            var saved = operationsDatabase != null && (existing.HasValue
                ? operationsDatabase.UpdateFieldInspection(record)
                : operationsDatabase.SaveFieldInspection(record, out _));
            if (!saved)
            {
                if (hasNewImage)
                {
                    DeletePersistedInspectionImage(imagePath);
                }

                inspectionFormError.text =
                    "Không thể lưu ghi nhận hiện trường vào SQLite.";
                return;
            }

            if (hasNewImage && existing.HasValue &&
                !string.Equals(
                    existing.Value.ImagePath,
                    imagePath,
                    StringComparison.OrdinalIgnoreCase))
            {
                DeletePersistedInspectionImage(existing.Value.ImagePath);
            }

            CloseInspectionDialog();
            ResetInspectionForm();
            RefreshInspectionViews();
            SetImportStatus(existing.HasValue
                ? "Đã cập nhật ghi nhận hiện trường và đồng bộ điểm lên báo cáo."
                : "Đã lưu ghi nhận hiện trường và đồng bộ điểm lên báo cáo.");
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

    private static void DeletePersistedInspectionImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        try
        {
            var imageDirectory = Path.GetFullPath(Path.Combine(
                Application.persistentDataPath,
                "IfcOperations",
                "InspectionImages"));
            var fullPath = Path.GetFullPath(imagePath);
            var directoryPrefix = imageDirectory.TrimEnd(
                                      Path.DirectorySeparatorChar,
                                      Path.AltDirectorySeparatorChar) +
                                  Path.DirectorySeparatorChar;
            if (fullPath.StartsWith(directoryPrefix, StringComparison.OrdinalIgnoreCase) &&
                File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not remove field inspection image: {exception.Message}");
        }
    }

    private void RefreshInspectionViews()
    {
        LoadInspectionsFromDatabase();
        BuildFieldHistory();
        BuildInspectionPopup();
        BuildInspectionMarkers();
        if (selectedRecord != null)
        {
            BuildElementInspectionHistory(selectedRecord);
        }

        UpdateModuleCounts();
    }

    private void OpenRegistryModelInReport(IfcModelRegistryRecord record)
    {
        if (!record.IsEnabled)
        {
            SetRegistryModelEnabled(record, true);
        }
        else
        {
            var availablePath = EnsureRegistryModelAvailable(record);
            if (!string.IsNullOrWhiteSpace(availablePath) && !IsModelLoaded(availablePath))
            {
                StartCoroutine(LoadRegisteredPath(availablePath));
            }
        }

        ShowModule(DashboardModule.Report);
        SetImportStatus($"Đang mở mô hình {Path.GetFileName(record.IfcPath)} trong không gian 3D.");
    }

    private void ExportRegistryModel(IfcModelRegistryRecord record)
    {
        var fileName = string.IsNullOrWhiteSpace(record.StoredFileName)
            ? Path.GetFileName(record.IfcPath)
            : record.StoredFileName;
        if (!RuntimeSaveFileDialog.TryGetSavePath(
                "Tải file IFC",
                fileName,
                "ifc",
                "IFC files",
                out var destination))
        {
            return;
        }

        try
        {
            if (File.Exists(record.IfcPath))
            {
                if (!string.Equals(
                        Path.GetFullPath(record.IfcPath),
                        Path.GetFullPath(destination),
                        StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(record.IfcPath, destination, true);
                }
            }
            else if (operationsDatabase == null ||
                     !operationsDatabase.RestoreIfcFile(record.Id, destination))
            {
                SetImportStatus("Không thể tải file IFC từ kho dữ liệu.");
                return;
            }

            SetImportStatus($"Đã tải file IFC: {Path.GetFileName(destination)}");
        }
        catch (Exception exception)
        {
            SetImportStatus($"Không thể tải file IFC: {exception.Message}");
        }
    }

    private void BuildFieldHistory()
    {
        if (fieldHistoryList == null)
        {
            return;
        }

        fieldHistoryList.Clear();
        var filteredInspections = fieldInspections
            .Where(MatchesFieldInspectionFilters)
            .ToArray();
        if (filteredInspections.Length == 0)
        {
            var empty = new Label(fieldInspections.Count == 0
                ? "Chưa có ghi nhận hiện trường."
                : "Không có báo cáo phù hợp với bộ lọc.");
            empty.AddToClassList("field-empty");
            fieldHistoryList.Add(empty);
            return;
        }

        for (var index = 0; index < filteredInspections.Length; index++)
        {
            var inspection = filteredInspections[index];
            var row = new VisualElement();
            row.AddToClassList("field-table-row");
            row.Add(CreateFieldTableLabel((index + 1).ToString(), "field-table-index"));

            var status = new Label
            {
                text = inspection.IsResolved ? "Đã xử lý" : "Chưa xử lý",
                tooltip = inspection.IsResolved
                    ? "Đã được xác nhận khi xuất báo cáo vận hành"
                    : "Sẽ tự động hoàn tất khi xuất báo cáo vận hành"
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
            row.Add(CreateFieldInspectionActions(inspection));

            fieldHistoryList.Add(row);
        }
    }

    private void ReloadFieldInspections()
    {
        LoadInspectionsFromDatabase();
        UpdateInspectionProjectChoices();
        UpdateFieldProjectFilterChoices();
        BuildFieldHistory();
        BuildInspectionPopup();
        BuildInspectionMarkers();
        UpdateModuleCounts();
        SetImportStatus("Đã tải lại dữ liệu báo cáo hiện trường.");
    }

    private void UpdateFieldProjectFilterChoices()
    {
        if (fieldProjectFilter == null)
        {
            return;
        }

        var current = fieldProjectFilter.value;
        var choices = new List<string> { "Tất cả dự án" };
        choices.AddRange(fieldInspections
            .Select(inspection => inspection.ProjectName)
            .Where(project => !string.IsNullOrWhiteSpace(project))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(project => project, StringComparer.CurrentCultureIgnoreCase));
        fieldProjectFilter.choices = choices;
        fieldProjectFilter.SetValueWithoutNotify(
            choices.Contains(current) ? current : choices[0]);
    }

    private bool MatchesFieldInspectionFilters(FieldInspectionRecord inspection)
    {
        var projectFilter = fieldProjectFilter?.value;
        if (!string.IsNullOrWhiteSpace(projectFilter) &&
            !string.Equals(projectFilter, "Tất cả dự án", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(projectFilter, inspection.ProjectName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var typeFilter = fieldTypeFilter?.value;
        return string.IsNullOrWhiteSpace(typeFilter) ||
               string.Equals(typeFilter, FieldInspectionTypeChoices[0], StringComparison.OrdinalIgnoreCase) ||
               MatchesFieldInspectionType(inspection.ElementType, typeFilter);
    }

    private static bool MatchesFieldInspectionType(string elementType, string filter)
    {
        var value = (elementType ?? string.Empty).Trim();
        bool ContainsAny(params string[] terms) =>
            terms.Any(term => value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);

        var isKnownType =
            ContainsAny("Dầm", "IfcBeam", "Beam") ||
            ContainsAny("Trụ", "IfcColumn", "Column", "Pier") ||
            ContainsAny("Mố", "Abutment") ||
            ContainsAny("Móng", "Cọc", "IfcPile", "IfcFooting", "Foundation") ||
            ContainsAny("Mặt đường", "Pavement", "Road", "IfcSlab") ||
            ContainsAny("Ta luy", "Slope") ||
            ContainsAny("Hộ lan", "Guardrail") ||
            ContainsAny("Cột đèn", "Lighting Pole", "Lamp Post") ||
            ContainsAny("Biển báo", "Traffic Sign") ||
            ContainsAny("Vạch sơn", "Road Marking") ||
            ContainsAny("Thoát nước", "Drain", "Culvert");

        return filter switch
        {
            "Dầm" => ContainsAny("Dầm", "IfcBeam", "Beam"),
            "Trụ" => ContainsAny("Trụ", "IfcColumn", "Column", "Pier"),
            "Mố" => ContainsAny("Mố", "Abutment"),
            "Móng cọc" => ContainsAny("Móng", "Cọc", "IfcPile", "IfcFooting", "Foundation"),
            "Mặt đường" => ContainsAny("Mặt đường", "Pavement", "Road", "IfcSlab"),
            "Ta luy" => ContainsAny("Ta luy", "Slope"),
            "Hộ lan" => ContainsAny("Hộ lan", "Guardrail"),
            "Cột đèn" => ContainsAny("Cột đèn", "Lighting Pole", "Lamp Post"),
            "Biển báo" => ContainsAny("Biển báo", "Traffic Sign"),
            "Vạch sơn" => ContainsAny("Vạch sơn", "Road Marking"),
            "Thoát nước" => ContainsAny("Thoát nước", "Drain", "Culvert"),
            "Khác" => !isKnownType,
            _ => true
        };
    }

    private void ToggleFieldInspectionPoints()
    {
        fieldPointsVisible = !fieldPointsVisible;
        ApplyFieldPointsVisibility();
        UpdateFieldPointsToggle();
    }

    private void ApplyFieldPointsVisibility()
    {
        foreach (var marker in inspectionMarkers.Values)
        {
            if (marker != null)
            {
                marker.gameObject.SetActive(fieldPointsVisible);
            }
        }
    }

    private void UpdateFieldPointsToggle()
    {
        if (fieldPointsToggleButton == null)
        {
            return;
        }

        fieldPointsToggleButton.text = fieldPointsVisible
            ? "Point 3D Bản đồ: Đang BẬT"
            : "Point 3D Bản đồ: Đang TẮT";
        fieldPointsToggleButton.EnableInClassList("field-points-toggle-on", fieldPointsVisible);
        fieldPointsToggleButton.EnableInClassList("field-points-toggle-off", !fieldPointsVisible);
    }

    private VisualElement CreateFieldInspectionActions(FieldInspectionRecord inspection)
    {
        var actions = new VisualElement();
        actions.AddToClassList("field-table-cell");
        actions.AddToClassList("field-table-actions");

        var editButton = new Button(() => OpenInspectionEditor(inspection))
        {
            text = "Sửa",
            tooltip = "Cập nhật ghi nhận hiện trường"
        };
        editButton.AddToClassList("field-table-action-button");
        actions.Add(editButton);

        var deleteArmed = false;
        var deleteButton = new Button
        {
            text = "Xóa",
            tooltip = "Xóa ghi nhận hiện trường"
        };
        deleteButton.clicked += () =>
        {
            if (!deleteArmed)
            {
                deleteArmed = true;
                deleteButton.text = "Xác nhận";
                deleteButton.AddToClassList("field-table-delete-confirm");
                deleteButton.schedule.Execute(() =>
                {
                    deleteArmed = false;
                    deleteButton.text = "Xóa";
                    deleteButton.RemoveFromClassList("field-table-delete-confirm");
                }).StartingIn(3500);
                return;
            }

            DeleteInspection(inspection);
        };
        deleteButton.AddToClassList("field-table-action-button");
        deleteButton.AddToClassList("field-table-delete-button");
        actions.Add(deleteButton);
        return actions;
    }

    private void DeleteInspection(FieldInspectionRecord inspection)
    {
        if (operationsDatabase == null ||
            !operationsDatabase.DeleteFieldInspection(inspection.Id))
        {
            SetImportStatus("Không thể xóa ghi nhận hiện trường khỏi SQLite.");
            return;
        }

        DeletePersistedInspectionImage(inspection.ImagePath);
        if (displayedInspectionId == inspection.Id)
        {
            CloseInspectionDetails();
        }

        if (editingInspection?.Id == inspection.Id)
        {
            CloseInspectionDialog();
            ResetInspectionForm();
        }

        RefreshInspectionViews();
        if (activeModule == DashboardModule.Dashboard)
        {
            PopulateAnalyticsDashboard();
        }
        SetImportStatus($"Đã xóa ghi nhận hiện trường: {inspection.ElementName}.");
    }

    private static Label CreateFieldTableLabel(string text, string className)
    {
        var label = new Label(string.IsNullOrWhiteSpace(text) ? "-" : text);
        label.AddToClassList("field-table-cell");
        label.AddToClassList(className);
        return label;
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
            ResolveDependencies();
            var useStoredCoordinate = asset == null ||
                                      InspectionCoordinateMatchesAsset(
                                          inspection,
                                          asset);
            var anchor = useStoredCoordinate
                ? arcGisMapLoader?.CreateGeographicAnchor(
                    $"Inspection {inspection.Id} ArcGIS Anchor",
                    inspection.Latitude,
                    inspection.Longitude,
                    inspection.Elevation)
                : null;
            IfcInspectionMarker marker = null;
            if (anchor != null)
            {
                geographicMarkerAnchors.Add(anchor.gameObject);
                marker = IfcInspectionMarker.Create(
                    anchor,
                    anchor.position,
                    inspection.Id,
                    asset?.Metadata,
                    viewingCamera,
                    inspection.ElementName,
                    inspection.IsResolved);
            }
            else if (asset != null)
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

            if (marker == null)
            {
                continue;
            }

            if (asset != null)
            {
                marker.SetElementStatus(
                    asset.State.Status,
                    asset.State.HasUserUpdate);
            }

            inspectionMarkers[inspection.Id] = marker;
        }

        ApplyFieldPointsVisibility();

        if (inspectionMarkerLinkRoutine != null)
        {
            StopCoroutine(inspectionMarkerLinkRoutine);
        }

        inspectionMarkerLinkRoutine = StartCoroutine(LinkUnassignedInspectionMarkers());
    }

    private bool InspectionCoordinateMatchesAsset(
        FieldInspectionRecord inspection,
        IfcAssetRecord asset)
    {
        if (asset?.Metadata == null || arcGisMapLoader == null)
        {
            return false;
        }

        var mapComponent = FindFirstObjectByType<
            Esri.ArcGISMapsSDK.Components.ArcGISMapComponent>();
        if (mapComponent == null)
        {
            return false;
        }

        var coordinatePosition = mapComponent.GeographicToEngine(
            new Esri.GameEngine.Geometry.ArcGISPoint(
                inspection.Longitude,
                inspection.Latitude,
                inspection.Elevation,
                Esri.GameEngine.Geometry.ArcGISSpatialReference.WGS84()));
        var hits = Physics.RaycastAll(
            coordinatePosition + Vector3.up * 3f,
            Vector3.down,
            6f,
            ~0,
            QueryTriggerInteraction.Ignore);
        foreach (var hit in hits)
        {
            if (hit.collider == null ||
                hit.collider.GetComponentInParent<IfcInspectionMarker>() != null)
            {
                continue;
            }

            var metadata = hit.collider.GetComponentInParent<IfcElementMetadata>();
            if (metadata == asset.Metadata)
            {
                return true;
            }
        }

        var bounds = RefreshLiveBounds(asset);
        var closestPoint = bounds.ClosestPoint(coordinatePosition);
        return Vector3.Distance(coordinatePosition, closestPoint) <= 5f;
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
                marker.SetElementStatus(
                    record.State.Status,
                    record.State.HasUserUpdate);
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
            item.tooltip = "Nhấn để xem đầy đủ ảnh và thông tin kiểm tra";
            item.RegisterCallback<ClickEvent>(_ => OpenInspectionDetails(inspection));

            var heading = new VisualElement();
            heading.AddToClassList("element-inspection-item-heading");
            var time = new Label(FormatInspectionTime(inspection.CreatedAt));
            time.AddToClassList("element-inspection-time");
            var status = new Label
            {
                text = inspection.IsResolved ? "Đã xử lý" : "Chưa xử lý"
            };
            status.tooltip = inspection.IsResolved
                ? "Đã được xác nhận khi xuất báo cáo vận hành"
                : "Sẽ tự động hoàn tất khi xuất báo cáo vận hành";
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

            var detailCommand = new Label(
                string.IsNullOrWhiteSpace(inspection.ImagePath)
                    ? "Xem chi tiết"
                    : "Xem ảnh và chi tiết");
            detailCommand.AddToClassList("element-inspection-image-button");
            item.Add(detailCommand);

            elementInspectionList.Add(item);
        }
    }

    private void OpenInspectionEditor(FieldInspectionRecord inspection)
    {
        ResetInspectionForm();
        UpdateInspectionProjectChoices();
        editingInspection = inspection;
        inspectionLinkedRecord = FindAssetForInspection(inspection);
        inspectionFormTitle.text = "Cập Nhật Ghi Nhận Hiện Trường";
        submitInspectionButton.text = "Lưu Thay Đổi";

        if (!string.IsNullOrWhiteSpace(inspection.ProjectName) &&
            !inspectionProjectDropdown.choices.Contains(inspection.ProjectName))
        {
            var choices = new List<string>(inspectionProjectDropdown.choices)
            {
                inspection.ProjectName
            };
            inspectionProjectDropdown.choices = choices;
        }

        inspectionProjectDropdown.SetValueWithoutNotify(inspection.ProjectName);
        inspectionStatusDropdown.index = inspection.IsResolved ? 1 : 0;
        inspectionNameInput.SetValueWithoutNotify(inspection.ElementName);
        inspectionElementTypeInput.SetValueWithoutNotify(inspection.ElementType);
        inspectionCreatedByInput.SetValueWithoutNotify(inspection.CreatedBy);
        inspectionLatitudeInput.SetValueWithoutNotify(
            inspection.Latitude.ToString("F8", CultureInfo.InvariantCulture));
        inspectionLongitudeInput.SetValueWithoutNotify(
            inspection.Longitude.ToString("F8", CultureInfo.InvariantCulture));
        inspectionElevationInput.SetValueWithoutNotify(
            inspection.Elevation.ToString("F2", CultureInfo.InvariantCulture));
        inspectionNoteInput.SetValueWithoutNotify(inspection.Note);

        if (!string.IsNullOrWhiteSpace(inspection.ImagePath) &&
            File.Exists(inspection.ImagePath))
        {
            inspectionImagePathLabel.text = Path.GetFileName(inspection.ImagePath);
            inspectionImagePathLabel.tooltip = inspection.ImagePath;
            ShowInspectionPreview(inspection.ImagePath);
        }

        inspectionFormOverlay.style.display = DisplayStyle.Flex;
        inspectionFormOverlay.BringToFront();
    }

    private void OpenInspectionDetails(FieldInspectionRecord inspection)
    {
        if (inspectionDetailOverlay == null)
        {
            return;
        }

        displayedInspectionId = inspection.Id;
        displayedInspection = inspection;
        inspectionDetailCreatedAt.text =
            $"Ghi nhận lúc {FormatInspectionTime(inspection.CreatedAt)}";
        inspectionDetailStatus.text = inspection.IsResolved
            ? "Đã xử lý"
            : "Chưa xử lý";
        inspectionDetailStatus.EnableInClassList(
            "inspection-detail-status-resolved",
            inspection.IsResolved);
        inspectionDetailName.text = EmptyFallback(
            inspection.ElementName,
            "Ghi nhận hiện trường");
        var linkedAsset = FindAssetForInspection(inspection);
        inspectionDetailType.text = EmptyFallback(
            inspection.ElementType,
            linkedAsset?.IfcType ?? "Cấu kiện IFC");
        inspectionDetailCreator.text = EmptyFallback(
            inspection.CreatedBy,
            "Người dùng hiện trường");
        inspectionDetailProject.text = EmptyFallback(inspection.ProjectName);

        var sourceName = string.IsNullOrWhiteSpace(inspection.SourceFile)
            ? "Chưa liên kết mô hình"
            : Path.GetFileNameWithoutExtension(inspection.SourceFile);
        inspectionDetailElement.text = string.IsNullOrWhiteSpace(inspection.ElementKey)
            ? sourceName
            : $"{sourceName}  •  #{inspection.ElementKey.TrimStart('#')}";
        inspectionDetailCoordinate.text =
            $"Vĩ độ: {inspection.Latitude:F6}\n" +
            $"Kinh độ: {inspection.Longitude:F6}\n" +
            $"Cao độ: {inspection.Elevation:F1} m";
        inspectionDetailNote.text = EmptyFallback(
            inspection.Note,
            "Không có mô tả hoặc ghi chú.");

        ShowInspectionDetailImage(inspection.ImagePath);
        inspectionDetailOverlay.style.display = DisplayStyle.Flex;
        inspectionDetailOverlay.BringToFront();
    }

    private void CloseInspectionDetails()
    {
        if (inspectionDetailOverlay != null)
        {
            inspectionDetailOverlay.style.display = DisplayStyle.None;
        }

        displayedInspectionId = 0;
        displayedInspection = null;
        inspectionDetailDeleteArmed = false;
        if (inspectionDetailDeleteButton != null)
        {
            inspectionDetailDeleteButton.text = "Xóa";
            inspectionDetailDeleteButton.RemoveFromClassList("inspection-detail-delete-confirm");
        }
        ReleaseInspectionDetailTexture();
    }

    private void OpenDisplayedInspectionOnMap()
    {
        if (!displayedInspection.HasValue)
        {
            return;
        }

        var inspection = displayedInspection.Value;
        CloseInspectionDetails();
        OpenInspectionInReport(inspection);
    }

    private void EditDisplayedInspection()
    {
        if (!displayedInspection.HasValue)
        {
            return;
        }

        var inspection = displayedInspection.Value;
        CloseInspectionDetails();
        ShowModule(DashboardModule.Field);
        OpenInspectionEditor(inspection);
    }

    private void DeleteDisplayedInspection()
    {
        if (!displayedInspection.HasValue)
        {
            return;
        }

        if (!inspectionDetailDeleteArmed)
        {
            inspectionDetailDeleteArmed = true;
            inspectionDetailDeleteButton.text = "Xác nhận xóa";
            inspectionDetailDeleteButton.AddToClassList("inspection-detail-delete-confirm");
            inspectionDetailDeleteButton.schedule.Execute(() =>
            {
                inspectionDetailDeleteArmed = false;
                inspectionDetailDeleteButton.text = "Xóa";
                inspectionDetailDeleteButton.RemoveFromClassList(
                    "inspection-detail-delete-confirm");
            }).StartingIn(3500);
            return;
        }

        DeleteInspection(displayedInspection.Value);
    }

    private void ShowInspectionDetailImage(string imagePath)
    {
        ReleaseInspectionDetailTexture();
        inspectionDetailImage.style.backgroundImage = StyleKeyword.None;
        inspectionDetailImagePlaceholder.style.display = DisplayStyle.Flex;

        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            return;
        }

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
        {
            name = $"Inspection detail {Path.GetFileName(imagePath)}"
        };
        if (!texture.LoadImage(File.ReadAllBytes(imagePath)))
        {
            Destroy(texture);
            return;
        }

        inspectionDetailTexture = texture;
        inspectionDetailImage.style.backgroundImage =
            new StyleBackground(inspectionDetailTexture);
        inspectionDetailImagePlaceholder.style.display = DisplayStyle.None;
    }

    private void ReleaseInspectionDetailTexture()
    {
        if (inspectionDetailTexture == null)
        {
            return;
        }

        Destroy(inspectionDetailTexture);
        inspectionDetailTexture = null;
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
        ReleaseInspectionDetailTexture();
        ReleaseAnalyticsThumbnailTextures();
        if (inspectionPreviewTexture != null)
        {
            Destroy(inspectionPreviewTexture);
            inspectionPreviewTexture = null;
        }
    }
}
