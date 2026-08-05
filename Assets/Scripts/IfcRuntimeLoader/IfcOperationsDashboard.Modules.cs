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

    private VisualElement homePage;
    private VisualElement dataPage;
    private VisualElement fieldPage;
    private VisualElement reportPage;
    private VisualElement registryList;
    private VisualElement fieldHistoryList;
    private VisualElement inspectionPopup;
    private VisualElement inspectionPopupList;
    private VisualElement modelUploadOverlay;
    private VisualElement inspectionImagePreview;
    private VisualElement inspectionImagePlaceholder;
    private Label homeModelCount;
    private Label homeInspectionCount;
    private Label homeElementCount;
    private Label registryCountLabel;
    private Label fieldRecordCountLabel;
    private Label selectedIfcPathLabel;
    private Label modelUploadError;
    private Label inspectionImagePathLabel;
    private Label inspectionFormError;
    private Button inspectionButton;
    private TextField filterProjectInput;
    private TextField filterProvinceInput;
    private TextField filterWardInput;
    private TextField filterFileInput;
    private TextField modelProjectInput;
    private TextField modelProvinceInput;
    private TextField modelWardInput;
    private TextField modelUnitInput;
    private DropdownField inspectionProjectDropdown;
    private TextField inspectionNameInput;
    private TextField inspectionLatitudeInput;
    private TextField inspectionLongitudeInput;
    private TextField inspectionElevationInput;
    private TextField inspectionNoteInput;
    private Texture2D inspectionPreviewTexture;
    private IfcAssetRecord inspectionLinkedRecord;
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
        inspectionImagePreview = root.Q<VisualElement>("inspection-image-preview");
        inspectionImagePlaceholder = root.Q<VisualElement>("inspection-image-placeholder");
        homeModelCount = root.Q<Label>("home-model-count");
        homeInspectionCount = root.Q<Label>("home-inspection-count");
        homeElementCount = root.Q<Label>("home-element-count");
        registryCountLabel = root.Q<Label>("registry-count");
        fieldRecordCountLabel = root.Q<Label>("field-record-count");
        selectedIfcPathLabel = root.Q<Label>("selected-ifc-path");
        modelUploadError = root.Q<Label>("model-upload-error");
        inspectionImagePathLabel = root.Q<Label>("inspection-image-path");
        inspectionFormError = root.Q<Label>("inspection-form-error");
        inspectionButton = root.Q<Button>("inspection-button");
        filterProjectInput = root.Q<TextField>("filter-project-input");
        filterProvinceInput = root.Q<TextField>("filter-province-input");
        filterWardInput = root.Q<TextField>("filter-ward-input");
        filterFileInput = root.Q<TextField>("filter-file-input");
        modelProjectInput = root.Q<TextField>("model-project-input");
        modelProvinceInput = root.Q<TextField>("model-province-input");
        modelWardInput = root.Q<TextField>("model-ward-input");
        modelUnitInput = root.Q<TextField>("model-unit-input");
        inspectionProjectDropdown = root.Q<DropdownField>("inspection-project-dropdown");
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
        root.Q<Button>("choose-inspection-image-button").clicked += ChooseInspectionImage;
        root.Q<Button>("use-selected-element-button").clicked += UseSelectedElement;
        root.Q<Button>("submit-inspection-button").clicked += SaveInspection;
        root.Q<Button>("inspection-button").clicked += ToggleInspectionPopup;
        root.Q<Button>("open-field-from-report-button").clicked += OpenFieldModule;

        filterProjectInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterProvinceInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterWardInput.RegisterValueChangedCallback(_ => BuildRegistryList());
        filterFileInput.RegisterValueChangedCallback(_ => BuildRegistryList());

        modelProjectInput.SetValueWithoutNotify(ProjectName);
        modelProvinceInput.SetValueWithoutNotify("Hà Nội");
        modelWardInput.SetValueWithoutNotify(string.Empty);
        modelUnitInput.SetValueWithoutNotify("Ban Quản lý Dự án Đầu tư Xây dựng");
        modelUploadOverlay.style.display = DisplayStyle.None;
        inspectionPopup.style.display = DisplayStyle.None;
        moduleUiBound = true;

        LoadRegistryFromDatabase();
        LoadInspectionsFromDatabase();
        UpdateInspectionProjectChoices();
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
            RefreshDashboard();
            BuildInspectionMarkers();
        }

        UpdateModuleCounts();
    }

    private void OpenFieldModule()
    {
        HidePopups();
        ShowModule(DashboardModule.Field);
        if (selectedRecord != null)
        {
            UseSelectedElement();
        }
        else
        {
            PrefillCurrentMapPosition();
        }
    }

    private IEnumerator LoadRegisteredModels()
    {
        if (operationsDatabase == null || !operationsDatabase.IsAvailable)
        {
            yield return LoadDefaultModels();
            yield break;
        }

        SeedDefaultModelRegistry();
        LoadRegistryFromDatabase();
        var paths = modelRegistry
            .Where(record => record.IsEnabled && File.Exists(record.IfcPath))
            .Select(record => record.IfcPath)
            .Where(path => loader == null || !loader.LoadedModels.Any(model =>
                string.Equals(
                    loader.GetModelSourcePath(model),
                    path,
                    StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        if (loader == null || paths.Length == 0)
        {
            startupLoadRoutine = null;
            RefreshModuleData();
            yield break;
        }

        startupLoading = true;
        SetLoadingVisible(activeModule == DashboardModule.Report);
        for (var index = 0; index < paths.Length; index++)
        {
            if (loadingMessage != null)
            {
                loadingMessage.text =
                    $"Đang nạp mô hình {index + 1}/{paths.Length}: " +
                    Path.GetFileNameWithoutExtension(paths[index]);
            }

            yield return LoadRegisteredPath(paths[index]);
        }

        startupLoading = false;
        startupLoadRoutine = null;
        SetLoadingVisible(false);
        RebuildModelIndex();
        RefreshModuleData();
        SetImportStatus($"Đã nạp {loader.LoadedModels.Count:N0} mô hình IFC từ kho dữ liệu.");
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
        UpdateInspectionProjectChoices();
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

            var fileName = Path.GetFileName(record.IfcPath);
            var fileLabel = CreateDataLabel(
                File.Exists(record.IfcPath) ? fileName : $"{fileName}\nThiếu file",
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
            enabled.AddToClassList("data-cell-state");
            enabled.AddToClassList("registry-toggle");
            enabled.RegisterValueChangedCallback(change =>
                SetRegistryModelEnabled(record, change.newValue));
            row.Add(enabled);

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

    private bool MatchesRegistryFilters(IfcModelRegistryRecord record)
    {
        return ContainsFilter(record.ProjectName, filterProjectInput.value) &&
               ContainsFilter(record.Province, filterProvinceInput.value) &&
               ContainsFilter(record.Ward, filterWardInput.value) &&
               ContainsFilter(Path.GetFileName(record.IfcPath), filterFileInput.value);
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

            CloseModelUpload();
            LoadRegistryFromDatabase();
            BuildRegistryList();
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
            if (!File.Exists(record.IfcPath))
            {
                SetImportStatus($"Không tìm thấy file IFC: {record.IfcPath}");
            }
            else if (!IsModelLoaded(record.IfcPath))
            {
                StartCoroutine(LoadRegisteredPath(record.IfcPath));
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
                inspectionNoteInput.value?.Trim());
            if (operationsDatabase == null ||
                !operationsDatabase.SaveFieldInspection(record, out _))
            {
                inspectionFormError.text =
                    "Không thể lưu ghi nhận hiện trường vào SQLite.";
                return;
            }

            inspectionNameInput.SetValueWithoutNotify(string.Empty);
            inspectionNoteInput.SetValueWithoutNotify(string.Empty);
            selectedInspectionImagePath = string.Empty;
            inspectionImagePathLabel.text = "Chưa chọn ảnh";
            inspectionImagePreview.style.backgroundImage = StyleKeyword.None;
            inspectionImagePlaceholder.style.display = DisplayStyle.Flex;
            if (inspectionPreviewTexture != null)
            {
                Destroy(inspectionPreviewTexture);
                inspectionPreviewTexture = null;
            }

            inspectionLinkedRecord = null;
            inspectionFormError.text = string.Empty;
            LoadInspectionsFromDatabase();
            BuildFieldHistory();
            BuildInspectionPopup();
            BuildInspectionMarkers();
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

        foreach (var inspection in fieldInspections)
        {
            var row = new VisualElement();
            row.AddToClassList("field-history-row");
            var copy = new VisualElement();
            copy.AddToClassList("field-history-copy");
            var name = new Label(inspection.ElementName);
            name.AddToClassList("field-history-name");
            var meta = new Label(
                $"{inspection.ProjectName}  |  {FormatInspectionTime(inspection.CreatedAt)}");
            meta.AddToClassList("field-history-meta");
            copy.Add(name);
            copy.Add(meta);
            var coordinate = new Label(
                $"{inspection.Latitude:F6}, {inspection.Longitude:F6}  |  {inspection.Elevation:F1}m");
            coordinate.AddToClassList("field-history-coordinate");
            row.Add(copy);
            row.Add(coordinate);
            row.RegisterCallback<ClickEvent>(_ => OpenInspectionInReport(inspection));
            fieldHistoryList.Add(row);
        }
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
            var mark = new Label("!");
            mark.AddToClassList("inspection-pin-mark");
            var copy = new VisualElement();
            copy.AddToClassList("inspection-popup-copy");
            var name = new Label(inspection.ElementName);
            name.AddToClassList("inspection-popup-name");
            var meta = new Label(
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
            string.Equals(
                record.SourceFile,
                inspection.SourceFile,
                StringComparison.OrdinalIgnoreCase) &&
            string.Equals(
                GetElementKey(record.Metadata),
                inspection.ElementKey,
                StringComparison.OrdinalIgnoreCase));
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
                var markerPosition = asset.Bounds.center +
                                     Vector3.up * Mathf.Max(2f, asset.Bounds.extents.y + 2f);
                marker = IfcInspectionMarker.Create(
                    asset.Metadata.transform,
                    markerPosition,
                    inspection.Id,
                    viewingCamera);
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
                    viewingCamera);
            }

            inspectionMarkers[inspection.Id] = marker;
        }
    }

    private void DestroyInspectionMarkers()
    {
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
