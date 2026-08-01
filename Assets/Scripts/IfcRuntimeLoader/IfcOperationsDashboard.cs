using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CauDuong.IfcOperations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[DisallowMultipleComponent]
[RequireComponent(typeof(UIDocument))]
public sealed class IfcOperationsDashboard : MonoBehaviour
{
    private const string ProjectName = "Tuyến Đường Vành Đai 3 - TP. Hà Nội";
    private const float ClickTolerancePixels = 8f;
    private const float MaximumClickDurationSeconds = 0.3f;
    private const float SelectedColorMultiplier = 0.42f;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [Header("IFC Integration")]
    [SerializeField] private XbimIfcLoader loader;
    [SerializeField] private RuntimeIfcLoader runtimeLoader;

    [Header("Startup")]
    [SerializeField] private bool loadDefaultModelsOnStart = true;
    [SerializeField, Min(1)] private int defaultModelLimit = 2;

    [Header("Scene Interaction")]
    [SerializeField] private Camera viewingCamera;
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private bool focusSelection = true;

    private readonly List<IfcAssetRecord> records = new();
    private readonly Dictionary<IfcInfrastructureCategory, bool> categoryVisibility = new();
    private readonly HashSet<IfcInfrastructureCategory> expandedCategories = new();
    private readonly Dictionary<int, Button> statusButtons = new();
    private readonly Dictionary<IfcElementMetadata, VisualElement> assetRows = new();
    private readonly Dictionary<IfcElementMetadata, IfcAssetRecord> recordsByMetadata = new();
    private readonly Dictionary<Transform, IfcAssetRecord> recordsByGeometry = new();

    private UIDocument document;
    private VisualElement root;
    private VisualElement leftPanel;
    private VisualElement detailsPanel;
    private VisualElement categoryList;
    private ScrollView categoryScroll;
    private VisualElement propertyList;
    private Label propertySetCountLabel;
    private VisualElement modelManagerPopup;
    private VisualElement measurementPopup;
    private VisualElement exportPopup;
    private VisualElement modelList;
    private VisualElement statusStrip;
    private VisualElement loadingOverlay;
    private VisualElement loadingSpinner;
    private VisualElement measurementHud;
    private VisualElement customPropertyOverlay;
    private VisualElement analyticsOverlay;
    private VisualElement analyticsStatusChart;
    private VisualElement analyticsStatusLegend;
    private VisualElement analyticsCategoryChart;
    private VisualElement analyticsCategoryLegend;
    private VisualElement analyticsSourceChart;
    private Label totalCountLabel;
    private Button layerCountButton;
    private Button measureButton;
    private Label importStatusLabel;
    private Label loadingMessage;
    private Label measurementHudTitle;
    private Label measurementHudValue;
    private Label detailTypeLabel;
    private Label detailNameLabel;
    private Label globalIdLabel;
    private Label expressIdLabel;
    private Label customPropertyTitle;
    private Label customPropertyError;
    private Label analyticsGeneratedAt;
    private Label analyticsTotalCount;
    private Label analyticsOperationalCount;
    private Label analyticsWarningCount;
    private Label analyticsCriticalCount;
    private Label analyticsRepairingCount;
    private Label analyticsModelCount;
    private VisualElement colorSwatch;
    private TextField displayNameInput;
    private TextField maintenanceNoteInput;
    private DropdownField statusDropdown;
    private TextField customPropertyKeyInput;
    private TextField customPropertyValueInput;
    private Button customPropertySaveButton;
    private IfcAssetRecord selectedRecord;
    private IfcOperationalStatus? activeStatusFilter;
    private IfcMeasurementController measurementController;
    private IfcOperationsDatabase operationsDatabase;
    private MaterialPropertyBlock selectionPropertyBlock;
    private Coroutine toastRoutine;
    private Coroutine startupLoadRoutine;
    private Vector2 scenePointerDownPosition;
    private float scenePointerDownTime;
    private float spinnerAngle;
    private bool pendingSceneClick;
    private bool startupLoading;
    private bool uiBound;
    private string editingCustomPropertyKey;
    private string editingBuiltInPropertyKey;
    private DonutChartElement statusDonutChart;

    private static readonly Color[] StatusChartColors =
    {
        new Color32(24, 173, 115, 255),
        new Color32(229, 161, 26, 255),
        new Color32(213, 42, 67, 255),
        new Color32(34, 110, 220, 255)
    };

    private static readonly List<string> StatusChoices = new()
    {
        "OPERATIONAL - Tốt",
        "WARNING - Cần bảo trì",
        "CRITICAL - Hỏng hóc",
        "REPAIRING - Đang sửa chữa"
    };

    private void Awake()
    {
        Application.runInBackground = true;
        selectionPropertyBlock = new MaterialPropertyBlock();
        document = GetComponent<UIDocument>();
        measurementController = GetComponent<IfcMeasurementController>() ??
                                gameObject.AddComponent<IfcMeasurementController>();
        operationsDatabase = GetComponent<IfcOperationsDatabase>() ??
                             gameObject.AddComponent<IfcOperationsDatabase>();
        ResolveDependencies();

        EnsureCategoryVisibilityDefaults();
    }

    private void OnEnable()
    {
        selectionPropertyBlock ??= new MaterialPropertyBlock();
        document ??= GetComponent<UIDocument>();
        measurementController ??= GetComponent<IfcMeasurementController>();
        operationsDatabase ??= GetComponent<IfcOperationsDatabase>();

        if (uiBound && !HasBoundUi())
        {
            uiBound = false;
            statusButtons.Clear();
            assetRows.Clear();
        }

        EnsureCategoryVisibilityDefaults();
        ResolveDependencies();
        SubscribeToLoader();
        SubscribeToMeasurement();
        BindUi();
    }

    private void EnsureCategoryVisibilityDefaults()
    {
        foreach (var definition in IfcInfrastructureClassifier.Definitions)
        {
            categoryVisibility.TryAdd(definition.Category, true);
        }
    }

    private void Start()
    {
        ResolveDependencies();
        BindUi();

        if (loader != null && loader.LoadedModels.Count > 0)
        {
            RebuildModelIndex();
        }
        else if (loadDefaultModelsOnStart)
        {
            startupLoadRoutine = StartCoroutine(LoadDefaultModels());
        }
    }

    private void OnDisable()
    {
        SetSelectionHighlight(selectedRecord, false);
        UnsubscribeFromLoader();
        UnsubscribeFromMeasurement();
    }

    private void Update()
    {
        HandleSceneSelection();
        AnimateLoadingSpinner();
    }

    private void ResolveDependencies()
    {
        loader ??= FindFirstObjectByType<XbimIfcLoader>();
        runtimeLoader ??= FindFirstObjectByType<RuntimeIfcLoader>();
        if (viewingCamera == null)
        {
            viewingCamera = Camera.main;
        }

        if (viewingCamera == null)
        {
            viewingCamera = FindFirstObjectByType<Camera>();
        }

        if (orbitCamera == null)
        {
            orbitCamera = viewingCamera != null
                ? viewingCamera.GetComponent<OrbitCamera>()
                : FindFirstObjectByType<OrbitCamera>();
        }
    }

    private void SubscribeToLoader()
    {
        if (loader == null)
        {
            return;
        }

        loader.LoadCompleted -= HandleLoadCompleted;
        loader.StatusChanged -= HandleLoaderStatus;
        loader.LoadFailed -= HandleLoaderFailure;
        loader.ModelsChanged -= HandleModelsChanged;
        loader.LoadCompleted += HandleLoadCompleted;
        loader.StatusChanged += HandleLoaderStatus;
        loader.LoadFailed += HandleLoaderFailure;
        loader.ModelsChanged += HandleModelsChanged;
    }

    private void UnsubscribeFromLoader()
    {
        if (loader == null)
        {
            return;
        }

        loader.LoadCompleted -= HandleLoadCompleted;
        loader.StatusChanged -= HandleLoaderStatus;
        loader.LoadFailed -= HandleLoaderFailure;
        loader.ModelsChanged -= HandleModelsChanged;
    }

    private void SubscribeToMeasurement()
    {
        if (measurementController == null)
        {
            return;
        }

        measurementController.StatusChanged -= SetImportStatus;
        measurementController.ModeChanged -= HandleMeasurementModeChanged;
        measurementController.HudChanged -= HandleMeasurementHudChanged;
        measurementController.StatusChanged += SetImportStatus;
        measurementController.ModeChanged += HandleMeasurementModeChanged;
        measurementController.HudChanged += HandleMeasurementHudChanged;
    }

    private void UnsubscribeFromMeasurement()
    {
        if (measurementController == null)
        {
            return;
        }

        measurementController.StatusChanged -= SetImportStatus;
        measurementController.ModeChanged -= HandleMeasurementModeChanged;
        measurementController.HudChanged -= HandleMeasurementHudChanged;
    }

    private void BindUi()
    {
        if (uiBound || document == null)
        {
            return;
        }

        root = document.rootVisualElement;
        if (root == null)
        {
            return;
        }

        leftPanel = root.Q<VisualElement>("left-panel");
        detailsPanel = root.Q<VisualElement>("details-panel");
        categoryList = root.Q<VisualElement>("category-list");
        categoryScroll = root.Q<ScrollView>("category-scroll");
        propertyList = root.Q<VisualElement>("property-list");
        propertySetCountLabel = root.Q<Label>("property-set-count");
        modelManagerPopup = root.Q<VisualElement>("model-manager-popup");
        measurementPopup = root.Q<VisualElement>("measurement-popup");
        exportPopup = root.Q<VisualElement>("export-popup");
        modelList = root.Q<VisualElement>("model-list");
        statusStrip = root.Q<VisualElement>("status-strip");
        loadingOverlay = root.Q<VisualElement>("loading-overlay");
        loadingSpinner = root.Q<VisualElement>("loading-spinner");
        measurementHud = root.Q<VisualElement>("measurement-hud");
        customPropertyOverlay = root.Q<VisualElement>("custom-property-overlay");
        analyticsOverlay = root.Q<VisualElement>("analytics-overlay");
        analyticsStatusChart = root.Q<VisualElement>("analytics-status-chart");
        analyticsStatusLegend = root.Q<VisualElement>("analytics-status-legend");
        analyticsCategoryChart = root.Q<VisualElement>("analytics-category-chart");
        analyticsCategoryLegend = root.Q<VisualElement>("analytics-category-legend");
        analyticsSourceChart = root.Q<VisualElement>("analytics-source-chart");
        totalCountLabel = root.Q<Label>("total-count");
        layerCountButton = root.Q<Button>("layer-button");
        measureButton = root.Q<Button>("measure-button");
        importStatusLabel = root.Q<Label>("import-status");
        loadingMessage = root.Q<Label>("loading-message");
        measurementHudTitle = root.Q<Label>("measurement-hud-title");
        measurementHudValue = root.Q<Label>("measurement-hud-value");
        detailTypeLabel = root.Q<Label>("detail-ifc-type");
        detailNameLabel = root.Q<Label>("detail-name");
        globalIdLabel = root.Q<Label>("detail-global-id");
        expressIdLabel = root.Q<Label>("detail-express-id");
        customPropertyTitle = root.Q<Label>("custom-property-title");
        customPropertyError = root.Q<Label>("custom-property-error");
        analyticsGeneratedAt = root.Q<Label>("analytics-generated-at");
        analyticsTotalCount = root.Q<Label>("analytics-total-count");
        analyticsOperationalCount = root.Q<Label>("analytics-operational-count");
        analyticsWarningCount = root.Q<Label>("analytics-warning-count");
        analyticsCriticalCount = root.Q<Label>("analytics-critical-count");
        analyticsRepairingCount = root.Q<Label>("analytics-repairing-count");
        analyticsModelCount = root.Q<Label>("analytics-model-count");
        colorSwatch = root.Q<VisualElement>("color-swatch");
        displayNameInput = root.Q<TextField>("display-name-input");
        maintenanceNoteInput = root.Q<TextField>("maintenance-note-input");
        statusDropdown = root.Q<DropdownField>("status-dropdown");
        customPropertyKeyInput = root.Q<TextField>("custom-property-key");
        customPropertyValueInput = root.Q<TextField>("custom-property-value");
        customPropertySaveButton = root.Q<Button>("save-custom-property-button");

        root.Q<Button>("browse-ifc-button").clicked += BrowseIfc;
        root.Q<Button>("layer-button").clicked += ToggleModelManager;
        root.Q<Button>("frame-model-button").clicked += FrameModel;
        root.Q<Button>("measure-button").clicked += ToggleMeasurementPopup;
        root.Q<Button>("export-button").clicked += ToggleExportPopup;
        root.Q<Button>("clear-models-button").clicked += ClearModels;
        root.Q<Button>("measure-distance-button").clicked +=
            () => BeginMeasurement(IfcMeasurementMode.Distance);
        root.Q<Button>("measure-height-button").clicked +=
            () => BeginMeasurement(IfcMeasurementMode.Height);
        root.Q<Button>("measure-area-button").clicked +=
            () => BeginMeasurement(IfcMeasurementMode.Area);
        root.Q<Button>("clear-measurements-button").clicked += ClearMeasurements;
        root.Q<Button>("export-csv-button").clicked += ExportReport;
        root.Q<Button>("export-json-button").clicked += ExportJson;
        root.Q<Button>("export-pdf-button").clicked += OpenAnalyticsDashboard;
        root.Q<Button>("close-details-button").clicked += CloseDetails;
        root.Q<Button>("save-operations-button").clicked += SaveOperations;
        root.Q<Button>("add-custom-property-button").clicked += OpenAddCustomProperty;
        root.Q<Button>("cancel-custom-property-button").clicked += CloseCustomPropertyEditor;
        customPropertySaveButton.clicked += SaveCustomProperty;
        root.Q<Button>("close-analytics-button").clicked += CloseAnalyticsDashboard;
        root.Q<Button>("analytics-export-pdf-button").clicked += ExportPdfReport;

        RegisterStatusButton("filter-all", null);
        RegisterStatusButton("filter-operational", IfcOperationalStatus.Operational);
        RegisterStatusButton("filter-warning", IfcOperationalStatus.Warning);
        RegisterStatusButton("filter-critical", IfcOperationalStatus.Critical);
        RegisterStatusButton("filter-repairing", IfcOperationalStatus.Repairing);

        statusDropdown.choices = StatusChoices;
        statusDropdown.index = 0;
        var mapSource = root.Q<DropdownField>("map-source");
        mapSource.choices = new List<string> { "OpenStreetMap" };
        mapSource.index = 0;
        detailsPanel.style.display = DisplayStyle.None;
        modelManagerPopup.style.display = DisplayStyle.None;
        measurementPopup.style.display = DisplayStyle.None;
        exportPopup.style.display = DisplayStyle.None;
        statusStrip.style.display = DisplayStyle.None;
        loadingOverlay.style.display = DisplayStyle.None;
        measurementHud.style.display = DisplayStyle.None;
        customPropertyOverlay.style.display = DisplayStyle.None;
        analyticsOverlay.style.display = DisplayStyle.None;
        statusDonutChart = new DonutChartElement
        {
            pickingMode = PickingMode.Ignore
        };
        statusDonutChart.style.flexGrow = 1f;
        analyticsStatusChart.Add(statusDonutChart);
        root.RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);

        uiBound = true;
        BuildModelList();
        RefreshDashboard();
    }

    private bool HasBoundUi()
    {
        return root != null &&
               totalCountLabel != null &&
               layerCountButton != null &&
               categoryList != null &&
               propertyList != null &&
               customPropertyOverlay != null &&
               analyticsOverlay != null;
    }

    private void RegisterStatusButton(string elementName, IfcOperationalStatus? status)
    {
        var button = root.Q<Button>(elementName);
        statusButtons[StatusFilterKey(status)] = button;
        button.clicked += () => SetStatusFilter(status);
    }

    private void BrowseIfc()
    {
        if (runtimeLoader != null)
        {
            runtimeLoader.BrowseIFC();
        }
        else
        {
            SetImportStatus("Không tìm thấy RuntimeIfcLoader.");
        }
    }

    private void HandleLoadCompleted(GameObject modelRoot)
    {
        RebuildModelIndex();
        if (!startupLoading)
        {
            FrameModel();
        }
    }

    private void HandleLoaderStatus(string message)
    {
        if (startupLoading && loadingMessage != null)
        {
            loadingMessage.text = message;
            return;
        }

        SetImportStatus(message);
    }

    private void HandleLoaderFailure(string message)
    {
        HandleLoaderStatus(message);
    }

    private void HandleModelsChanged()
    {
        RebuildModelIndex();
        BuildModelList();
    }

    private void RebuildModelIndex()
    {
        SetSelectionHighlight(selectedRecord, false);
        records.Clear();
        recordsByMetadata.Clear();
        recordsByGeometry.Clear();
        selectedRecord = null;
        CloseCustomPropertyEditor();
        CloseAnalyticsDashboard();

        if (loader == null || loader.LoadedModels.Count == 0)
        {
            RefreshDashboard();
            return;
        }

        var operationalIndex = 0;
        foreach (var modelRoot in loader.LoadedModels)
        {
            if (modelRoot == null)
            {
                continue;
            }

            var modelContext = ReadModelContext(modelRoot);
            var metadataComponents =
                modelRoot.GetComponentsInChildren<IfcElementMetadata>(true);
            foreach (var metadata in metadataComponents)
            {
                var renderers = CollectOwnedRenderers(metadata.transform);
                if (renderers.Count == 0)
                {
                    continue;
                }

                var category = IfcInfrastructureClassifier.Classify(
                    metadata.gameObject.name,
                    metadata.IfcType);
                var state = metadata.GetComponent<IfcOperationsState>() ??
                            metadata.gameObject.AddComponent<IfcOperationsState>();
                state.Initialize(category, metadata.EntityLabel, operationalIndex);
                var elementKey = GetElementKey(metadata);
                if (operationsDatabase != null &&
                    operationsDatabase.TryLoad(
                        modelContext.SourceFile,
                        elementKey,
                        out var snapshot))
                {
                    if (!string.IsNullOrWhiteSpace(snapshot.DisplayName))
                    {
                        metadata.gameObject.name = snapshot.DisplayName;
                    }

                    state.Restore(
                        snapshot.Category,
                        snapshot.Status,
                        snapshot.OperationsGlobalId,
                        snapshot.MaintenanceNote,
                        snapshot.UpdatedAt);
                }

                var record = BuildRecord(
                    metadata,
                    state,
                    renderers,
                    modelRoot.transform,
                    modelContext);
                records.Add(record);
                recordsByMetadata[metadata] = record;
                foreach (var renderer in renderers)
                {
                    if (renderer != null)
                    {
                        recordsByGeometry[renderer.transform] = record;
                    }
                }

                operationalIndex++;
            }
        }

        ApplyCategoryVisibility();
        RefreshDashboard();
        BuildModelList();
    }

    private static List<Renderer> CollectOwnedRenderers(Transform owner)
    {
        var result = new List<Renderer>();
        CollectOwnedRenderersRecursive(owner, owner, result);
        return result;
    }

    private static void CollectOwnedRenderersRecursive(
        Transform current,
        Transform owner,
        ICollection<Renderer> result)
    {
        for (var index = 0; index < current.childCount; index++)
        {
            var child = current.GetChild(index);
            if (child != owner && child.TryGetComponent<IfcElementMetadata>(out _))
            {
                continue;
            }

            if (child.TryGetComponent<Renderer>(out var renderer))
            {
                result.Add(renderer);
            }

            CollectOwnedRenderersRecursive(child, owner, result);
        }
    }

    private static IfcAssetRecord BuildRecord(
        IfcElementMetadata metadata,
        IfcOperationsState state,
        List<Renderer> renderers,
        Transform modelRoot,
        ModelContext context)
    {
        var bounds = renderers[0].bounds;
        var triangleCount = 0L;
        var vertexCount = 0L;
        var normalCount = 0L;
        var indexCount = 0L;

        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);

            if (!renderer.TryGetComponent<MeshFilter>(out var meshFilter) ||
                meshFilter.sharedMesh == null)
            {
                continue;
            }

            var mesh = meshFilter.sharedMesh;
            vertexCount += mesh.vertexCount;
            if (mesh.HasVertexAttribute(UnityEngine.Rendering.VertexAttribute.Normal))
            {
                normalCount += mesh.vertexCount;
            }

            for (var subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
            {
                var subMeshIndices = (long)mesh.GetIndexCount(subMesh);
                indexCount += subMeshIndices;
                triangleCount += subMeshIndices / 3L;
            }
        }

        var localCenter = modelRoot.InverseTransformPoint(bounds.center);
        var vn2000X = (context.OriginX + localCenter.x) * context.MetresPerUnit;
        var vn2000Y = (context.OriginY + localCenter.z) * context.MetresPerUnit;

        return new IfcAssetRecord
        {
            Metadata = metadata,
            State = state,
            Renderers = renderers,
            Name = metadata.gameObject.name,
            IfcType = string.IsNullOrWhiteSpace(metadata.IfcType)
                ? "IfcProduct"
                : metadata.IfcType,
            IfcGlobalId = metadata.GlobalId,
            SourceFile = context.SourceFile,
            Bounds = bounds,
            Color = ReadRendererColor(renderers),
            TriangleCount = triangleCount,
            VertexCount = vertexCount,
            NormalCount = normalCount,
            IndexCount = indexCount,
            Vn2000X = vn2000X,
            Vn2000Y = vn2000Y
        };
    }

    private static Color ReadRendererColor(IReadOnlyList<Renderer> renderers)
    {
        foreach (var renderer in renderers)
        {
            var material = renderer.sharedMaterial;
            if (material == null)
            {
                continue;
            }

            if (material.HasProperty("_BaseColor"))
            {
                return material.GetColor("_BaseColor");
            }

            if (material.HasProperty("_Color"))
            {
                return material.GetColor("_Color");
            }
        }

        return Color.white;
    }

    private static ModelContext ReadModelContext(GameObject modelRoot)
    {
        var context = new ModelContext
        {
            SourceFile = modelRoot.name,
            MetresPerUnit = 1d
        };

        if (!modelRoot.TryGetComponent<IfcMetadataComponent>(out var metadata))
        {
            return context;
        }

        if (metadata.Properties.TryGetValue("Length Scale (metres/unit)", out var scaleText) &&
            double.TryParse(
                scaleText,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var scale) &&
            scale > 0d)
        {
            context.MetresPerUnit = scale;
        }

        if (metadata.Properties.TryGetValue(
                "Local Origin (IFC coordinates)",
                out var originText))
        {
            var values = originText.Split(',');
            if (values.Length >= 2)
            {
                double.TryParse(
                    values[0].Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out context.OriginX);
                double.TryParse(
                    values[1].Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out context.OriginY);
            }
        }

        return context;
    }

    private void RefreshDashboard()
    {
        if (!uiBound || !HasBoundUi())
        {
            uiBound = false;
            BindUi();
        }

        if (!uiBound || !HasBoundUi())
        {
            return;
        }

        totalCountLabel.text = $"{records.Count:N0} cấu kiện";
        var modelCount = loader?.LoadedModels.Count ?? 0;
        layerCountButton.text = $"Lớp IFC ({modelCount:N0})";
        BuildCategoryList();
        UpdateStatusButtons();

        if (selectedRecord != null)
        {
            ShowDetails(selectedRecord);
        }
    }

    private void BuildCategoryList()
    {
        categoryList.Clear();
        assetRows.Clear();

        foreach (var definition in IfcInfrastructureClassifier.Definitions)
        {
            var categoryRecords = records
                .Where(record => record.State.Category == definition.Category)
                .Where(MatchesStatusFilter)
                .OrderBy(record => record.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
            var fullCategoryCount = records.Count(
                record => record.State.Category == definition.Category);
            categoryList.Add(CreateCategoryCard(
                definition,
                categoryRecords,
                fullCategoryCount));
        }
    }

    private VisualElement CreateCategoryCard(
        IfcCategoryDefinition definition,
        IReadOnlyList<IfcAssetRecord> categoryRecords,
        int fullCategoryCount)
    {
        var card = new VisualElement();
        card.AddToClassList("category-card");

        var header = new VisualElement();
        header.AddToClassList("category-header");

        var toggle = new Toggle
        {
            value = categoryVisibility[definition.Category],
            tooltip = $"Ẩn/hiện nhóm {definition.DisplayName}"
        };
        toggle.AddToClassList("category-toggle");
        toggle.RegisterValueChangedCallback(change =>
        {
            categoryVisibility[definition.Category] = change.newValue;
            ApplyCategoryVisibility();
        });

        var symbol = new Label(definition.Symbol);
        symbol.AddToClassList("category-symbol");
        symbol.style.backgroundColor = new StyleColor(definition.AccentColor);

        var title = new Label(definition.DisplayName);
        title.AddToClassList("category-title");

        var count = new Label($"{fullCategoryCount:N0}");
        count.AddToClassList("category-count");

        header.Add(toggle);
        header.Add(symbol);
        header.Add(title);
        header.Add(count);
        card.Add(header);

        if (categoryRecords.Count == 0)
        {
            var empty = new Label(activeStatusFilter.HasValue
                ? "Không có cấu kiện ở trạng thái này"
                : "Chưa có cấu kiện");
            empty.AddToClassList("category-empty");
            card.Add(empty);
            return card;
        }

        var expanded = expandedCategories.Contains(definition.Category);
        var visibleCount = expanded ? categoryRecords.Count : Math.Min(5, categoryRecords.Count);

        for (var index = 0; index < visibleCount; index++)
        {
            var record = categoryRecords[index];
            var row = new Button(() => SelectRecord(record))
            {
                text = $"• {record.Name}",
                tooltip = $"{record.IfcType} | ExpressID #{record.Metadata.EntityLabel}"
            };
            row.AddToClassList("asset-row");
            assetRows[record.Metadata] = row;

            if (record == selectedRecord)
            {
                row.AddToClassList("asset-row-selected");
            }

            card.Add(row);
        }

        if (categoryRecords.Count > 5)
        {
            var remaining = categoryRecords.Count - 5;
            var expandButton = new Button(() =>
            {
                if (!expandedCategories.Add(definition.Category))
                {
                    expandedCategories.Remove(definition.Category);
                }

                BuildCategoryList();
            })
            {
                text = expanded
                    ? "Thu gọn"
                    : $"+{remaining:N0} cấu kiện khác... Mở rộng"
            };
            expandButton.AddToClassList("category-expand");
            card.Add(expandButton);
        }

        return card;
    }

    private bool MatchesStatusFilter(IfcAssetRecord record)
    {
        return !activeStatusFilter.HasValue ||
               record.State.Status == activeStatusFilter.Value;
    }

    private void SetStatusFilter(IfcOperationalStatus? status)
    {
        activeStatusFilter = status;
        BuildCategoryList();
        UpdateStatusButtons();
    }

    private void UpdateStatusButtons()
    {
        foreach (var pair in statusButtons)
        {
            pair.Value.EnableInClassList(
                "filter-button-active",
                pair.Key == StatusFilterKey(activeStatusFilter));

            if (pair.Key < 0)
            {
                pair.Value.text = $"Tất cả  {records.Count:N0}";
                continue;
            }

            var status = (IfcOperationalStatus)pair.Key;
            var count = records.Count(record => record.State.Status == status);
            pair.Value.text = $"{GetStatusShortLabel(status)}  {count:N0}";
        }
    }

    private void ApplyCategoryVisibility()
    {
        foreach (var record in records)
        {
            var visible = categoryVisibility[record.State.Category];
            foreach (var renderer in record.Renderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = visible;
                }
            }
        }
    }

    private void SelectRecord(IfcAssetRecord record, bool frameSelection = true)
    {
        if (selectedRecord != record)
        {
            SetSelectionHighlight(selectedRecord, false);
        }

        selectedRecord = record;
        SetSelectionHighlight(record, true);
        expandedCategories.Add(record.State.Category);
        if (!MatchesStatusFilter(record))
        {
            activeStatusFilter = null;
            UpdateStatusButtons();
        }

        ShowDetails(record);
        BuildCategoryList();
        if (assetRows.TryGetValue(record.Metadata, out var selectedRow))
        {
            categoryScroll.schedule.Execute(() => categoryScroll.ScrollTo(selectedRow));
        }

        var lodController = record.Metadata.GetComponentInParent<IfcModelLodController>();
        lodController?.Reveal(record.Renderers);

        if (focusSelection && frameSelection)
        {
            FrameBounds(record.Bounds);
        }
    }

    private void ShowDetails(IfcAssetRecord record)
    {
        EnsureElementPropertiesLoaded(record);

        detailsPanel.style.display = DisplayStyle.Flex;
        detailTypeLabel.text = record.IfcType;
        detailNameLabel.text = record.Name;
        globalIdLabel.text = $"GlobalID: {record.State.OperationsGlobalId}";
        expressIdLabel.text = $"ExpressID: #{record.Metadata.EntityLabel}";
        displayNameInput.SetValueWithoutNotify(record.Name);
        maintenanceNoteInput.SetValueWithoutNotify(record.State.MaintenanceNote);
        statusDropdown.index = (int)record.State.Status;
        colorSwatch.style.backgroundColor = new StyleColor(record.Color);

        propertyList.Clear();
        propertySetCountLabel.text = $"PropertySets ({(record.CustomProperties.Count > 0 ? 2 : 1)})";
        AddPropertySetHeading("Pset_VD3InfraOps");
        AddBuiltInPropertyRow(record, "Dự án", ProjectName);
        AddBuiltInPropertyRow(record, "Nguồn File", record.SourceFile);
        AddBuiltInPropertyRow(record, "Mã IFC", record.IfcType);
        AddBuiltInPropertyRow(record, "IFC GlobalId gốc", EmptyFallback(record.IfcGlobalId));
        AddBuiltInPropertyRow(record, "Màu RGBA", FormatColor(record.Color));
        AddBuiltInPropertyRow(record, "Số tam giác 3D", $"{record.TriangleCount:N0} tam giác");
        AddBuiltInPropertyRow(
            record,
            "MeshData",
            $"{record.VertexCount:N0} vertices | {record.NormalCount:N0} normals | " +
            $"{record.IndexCount:N0} indices");
        AddBuiltInPropertyRow(
            record,
            "Kích thước bao (X x Y x Z)",
            $"{record.Bounds.size.x:F2}m x {record.Bounds.size.y:F2}m x " +
            $"{record.Bounds.size.z:F2}m");
        AddBuiltInPropertyRow(
            record,
            "Tọa độ dự án VN2000",
            $"X: {record.Vn2000X:F2}, Y: {record.Vn2000Y:F2}");
        AddBuiltInPropertyRow(
            record,
            "Thời gian cập nhật",
            EmptyFallback(record.State.UpdatedAt, "Chưa cập nhật"));

        if (record.CustomProperties.Count > 0)
        {
            AddPropertySetHeading("ThuộcTínhTùyChỉnh");
            foreach (var property in record.CustomProperties.OrderBy(
                         pair => pair.Key,
                         StringComparer.CurrentCultureIgnoreCase))
            {
                AddPropertyRow(
                    property.Key,
                    property.Value,
                    () => OpenEditCustomProperty(property.Key),
                    () => DeleteCustomProperty(property.Key));
            }
        }
    }

    private void AddBuiltInPropertyRow(
        IfcAssetRecord record,
        string key,
        string sourceValue)
    {
        if (record.DeletedProperties.Contains(key))
        {
            return;
        }

        var displayedValue = record.PropertyOverrides.TryGetValue(key, out var overrideValue)
            ? overrideValue
            : sourceValue;
        AddPropertyRow(
            key,
            displayedValue,
            () => OpenEditBuiltInProperty(key, displayedValue),
            () => DeleteBuiltInProperty(key));
    }

    private void EnsureElementPropertiesLoaded(IfcAssetRecord record)
    {
        EnsureCustomPropertiesLoaded(record);
        if (record == null || record.PropertyOverridesLoaded || operationsDatabase == null)
        {
            return;
        }

        record.PropertyOverridesLoaded = operationsDatabase.TryLoadPropertyOverrides(
            record.SourceFile,
            GetElementKey(record.Metadata),
            record.PropertyOverrides,
            record.DeletedProperties);
    }

    private void EnsureCustomPropertiesLoaded(IfcAssetRecord record)
    {
        if (record == null || record.CustomPropertiesLoaded || operationsDatabase == null)
        {
            return;
        }

        record.CustomPropertiesLoaded = operationsDatabase.TryLoadCustomProperties(
            record.SourceFile,
            GetElementKey(record.Metadata),
            record.CustomProperties);
    }

    private void AddPropertySetHeading(string text)
    {
        var heading = new Label(text);
        heading.AddToClassList("property-set-heading");
        propertyList.Add(heading);
    }

    private void AddPropertyRow(
        string key,
        string value,
        Action edit = null,
        Action delete = null)
    {
        var row = new VisualElement();
        row.AddToClassList("property-row");

        var copy = new VisualElement();
        copy.AddToClassList("property-copy");

        var keyLabel = new Label(key);
        keyLabel.AddToClassList("property-key");

        var valueLabel = new Label(value);
        valueLabel.AddToClassList("property-value");

        copy.Add(keyLabel);
        copy.Add(valueLabel);
        row.Add(copy);

        if (edit != null || delete != null)
        {
            var actions = new VisualElement();
            actions.AddToClassList("property-actions");
            if (edit != null)
            {
                var editButton = new Button(edit)
                {
                    text = "✎",
                    tooltip = $"Sửa thuộc tính {key}"
                };
                editButton.AddToClassList("property-action-button");
                actions.Add(editButton);
            }

            if (delete != null)
            {
                var deleteButton = new Button(delete)
                {
                    text = "×",
                    tooltip = $"Xóa thuộc tính {key}"
                };
                deleteButton.AddToClassList("property-action-button");
                deleteButton.AddToClassList("property-action-delete");
                actions.Add(deleteButton);
            }

            row.Add(actions);
        }

        propertyList.Add(row);
    }

    private void OpenAddCustomProperty()
    {
        if (selectedRecord == null)
        {
            SetImportStatus("Hãy chọn một cấu kiện IFC trước khi thêm thuộc tính.");
            return;
        }

        editingCustomPropertyKey = null;
        editingBuiltInPropertyKey = null;
        customPropertyKeyInput.SetEnabled(true);
        customPropertyTitle.text = "+ Thêm Thuộc Tính IFC";
        customPropertySaveButton.text = "Thêm";
        customPropertyKeyInput.SetValueWithoutNotify(string.Empty);
        customPropertyValueInput.SetValueWithoutNotify(string.Empty);
        customPropertyError.text = string.Empty;
        customPropertyOverlay.style.display = DisplayStyle.Flex;
        customPropertyKeyInput.schedule.Execute(customPropertyKeyInput.Focus);
    }

    private void OpenEditCustomProperty(string propertyKey)
    {
        if (selectedRecord == null ||
            !selectedRecord.CustomProperties.TryGetValue(propertyKey, out var value))
        {
            return;
        }

        editingCustomPropertyKey = propertyKey;
        editingBuiltInPropertyKey = null;
        customPropertyKeyInput.SetEnabled(true);
        customPropertyTitle.text = "Sửa Thuộc Tính IFC";
        customPropertySaveButton.text = "Lưu";
        customPropertyKeyInput.SetValueWithoutNotify(propertyKey);
        customPropertyValueInput.SetValueWithoutNotify(value);
        customPropertyError.text = string.Empty;
        customPropertyOverlay.style.display = DisplayStyle.Flex;
        customPropertyValueInput.schedule.Execute(customPropertyValueInput.Focus);
    }

    private void OpenEditBuiltInProperty(string propertyKey, string propertyValue)
    {
        if (selectedRecord == null)
        {
            return;
        }

        editingCustomPropertyKey = null;
        editingBuiltInPropertyKey = propertyKey;
        customPropertyTitle.text = "Sửa Thuộc Tính IFC";
        customPropertySaveButton.text = "Lưu";
        customPropertyKeyInput.SetValueWithoutNotify(propertyKey);
        customPropertyKeyInput.SetEnabled(false);
        customPropertyValueInput.SetValueWithoutNotify(propertyValue);
        customPropertyError.text = string.Empty;
        customPropertyOverlay.style.display = DisplayStyle.Flex;
        customPropertyValueInput.schedule.Execute(customPropertyValueInput.Focus);
    }

    private void CloseCustomPropertyEditor()
    {
        editingCustomPropertyKey = null;
        editingBuiltInPropertyKey = null;
        customPropertyKeyInput?.SetEnabled(true);
        if (customPropertyOverlay != null)
        {
            customPropertyOverlay.style.display = DisplayStyle.None;
        }
    }

    private void SaveCustomProperty()
    {
        if (selectedRecord == null)
        {
            CloseCustomPropertyEditor();
            return;
        }

        var propertyKey = customPropertyKeyInput.value?.Trim();
        if (string.IsNullOrWhiteSpace(propertyKey))
        {
            customPropertyError.text = "Tên thuộc tính không được để trống.";
            return;
        }

        var propertyValue = customPropertyValueInput.value ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(editingBuiltInPropertyKey))
        {
            var builtInKey = editingBuiltInPropertyKey;
            var persistedOverride = operationsDatabase != null &&
                                      operationsDatabase.SavePropertyOverride(
                                          selectedRecord.SourceFile,
                                          GetElementKey(selectedRecord.Metadata),
                                          builtInKey,
                                          propertyValue);
            if (!persistedOverride)
            {
                customPropertyError.text = "Không thể lưu thuộc tính vào SQLite.";
                return;
            }

            selectedRecord.PropertyOverrides[builtInKey] = propertyValue;
            selectedRecord.DeletedProperties.Remove(builtInKey);
            CloseCustomPropertyEditor();
            ShowDetails(selectedRecord);
            SetImportStatus($"Đã cập nhật thuộc tính {builtInKey}.");
            return;
        }

        if (selectedRecord.CustomProperties.ContainsKey(propertyKey) &&
            !string.Equals(propertyKey, editingCustomPropertyKey, StringComparison.OrdinalIgnoreCase))
        {
            customPropertyError.text = "Tên thuộc tính này đã tồn tại.";
            return;
        }

        var persisted = operationsDatabase != null &&
                        operationsDatabase.SaveCustomProperty(
                            selectedRecord.SourceFile,
                            GetElementKey(selectedRecord.Metadata),
                            propertyKey,
                            propertyValue,
                            editingCustomPropertyKey);
        if (!persisted)
        {
            customPropertyError.text = "Không thể lưu thuộc tính vào SQLite.";
            return;
        }

        if (!string.IsNullOrWhiteSpace(editingCustomPropertyKey) &&
            !string.Equals(propertyKey, editingCustomPropertyKey, StringComparison.OrdinalIgnoreCase))
        {
            selectedRecord.CustomProperties.Remove(editingCustomPropertyKey);
        }

        selectedRecord.CustomProperties[propertyKey] = propertyValue;
        CloseCustomPropertyEditor();
        ShowDetails(selectedRecord);
        SetImportStatus($"Đã lưu thuộc tính {propertyKey} vào SQLite.");
    }

    private void DeleteCustomProperty(string propertyKey)
    {
        if (selectedRecord == null ||
            !selectedRecord.CustomProperties.ContainsKey(propertyKey))
        {
            return;
        }

        var deleted = operationsDatabase != null &&
                      operationsDatabase.DeleteCustomProperty(
                          selectedRecord.SourceFile,
                          GetElementKey(selectedRecord.Metadata),
                          propertyKey);
        if (!deleted)
        {
            SetImportStatus($"Không thể xóa thuộc tính {propertyKey} khỏi SQLite.");
            return;
        }

        selectedRecord.CustomProperties.Remove(propertyKey);
        ShowDetails(selectedRecord);
        SetImportStatus($"Đã xóa thuộc tính {propertyKey}.");
    }

    private void DeleteBuiltInProperty(string propertyKey)
    {
        if (selectedRecord == null)
        {
            return;
        }

        var deleted = operationsDatabase != null &&
                      operationsDatabase.DeleteElementProperty(
                          selectedRecord.SourceFile,
                          GetElementKey(selectedRecord.Metadata),
                          propertyKey);
        if (!deleted)
        {
            SetImportStatus($"Không thể xóa thuộc tính {propertyKey} khỏi SQLite.");
            return;
        }

        selectedRecord.PropertyOverrides.Remove(propertyKey);
        selectedRecord.DeletedProperties.Add(propertyKey);
        ShowDetails(selectedRecord);
        SetImportStatus($"Đã xóa thuộc tính {propertyKey} khỏi cấu kiện.");
    }

    private void SaveOperations()
    {
        if (selectedRecord == null)
        {
            return;
        }

        var newName = displayNameInput.value?.Trim();
        if (!string.IsNullOrWhiteSpace(newName) &&
            !string.Equals(newName, selectedRecord.Name, StringComparison.Ordinal))
        {
            selectedRecord.Name = newName;
            selectedRecord.Metadata.gameObject.name = newName;
            var newCategory = IfcInfrastructureClassifier.Classify(
                newName,
                selectedRecord.IfcType);
            selectedRecord.State.Initialize(
                newCategory,
                selectedRecord.Metadata.EntityLabel,
                ParseOperationsIndex(selectedRecord.State.OperationsGlobalId));
        }

        selectedRecord.State.UpdateOperations(
            (IfcOperationalStatus)Mathf.Clamp(statusDropdown.index, 0, 3),
            maintenanceNoteInput.value,
            DateTime.Now);
        var persisted = operationsDatabase != null &&
                        operationsDatabase.Save(
                            selectedRecord.SourceFile,
                            GetElementKey(selectedRecord.Metadata),
                            selectedRecord.Name,
                            selectedRecord.State);
        SetImportStatus(persisted
            ? $"Đã lưu vận hành cho {selectedRecord.Name} vào SQLite."
            : $"Đã cập nhật vận hành cho {selectedRecord.Name}; chưa thể ghi SQLite.");
        RefreshDashboard();
    }

    private static int ParseOperationsIndex(string operationsGlobalId)
    {
        if (string.IsNullOrWhiteSpace(operationsGlobalId))
        {
            return 0;
        }

        var segments = operationsGlobalId.Split('-');
        return segments.Length > 0 &&
               int.TryParse(segments[^1], out var index)
            ? index
            : 0;
    }

    private void CloseDetails()
    {
        CloseCustomPropertyEditor();
        SetSelectionHighlight(selectedRecord, false);
        selectedRecord = null;
        detailsPanel.style.display = DisplayStyle.None;
        BuildCategoryList();
    }

    private IEnumerator LoadDefaultModels()
    {
        if (loader == null)
        {
            SetImportStatus("Không tìm thấy XbimIfcLoader để tải mô hình mặc định.");
            yield break;
        }

        var defaultDirectory = Path.Combine(Application.dataPath, "IFC", "Default");
        if (!Directory.Exists(defaultDirectory))
        {
            SetImportStatus($"Không tìm thấy thư mục IFC mặc định: {defaultDirectory}");
            yield break;
        }

        var paths = Directory
            .GetFiles(defaultDirectory, "*.ifc", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Where(path => !loader.LoadedModels.Any(
                model => string.Equals(
                    loader.GetModelSourcePath(model),
                    path,
                    StringComparison.OrdinalIgnoreCase)))
            .Take(Mathf.Max(1, defaultModelLimit))
            .ToArray();

        if (paths.Length == 0)
        {
            SetImportStatus("Không có mô hình IFC mặc định mới để tải.");
            yield break;
        }

        startupLoading = true;
        SetLoadingVisible(true);

        var completedCount = 0;
        foreach (var path in paths)
        {
            while (loader.IsLoading)
            {
                yield return null;
            }

            var finished = false;
            Action<GameObject> completed = _ => finished = true;
            Action<string> failed = _ => finished = true;
            loader.LoadCompleted += completed;
            loader.LoadFailed += failed;

            loadingMessage.text =
                $"Đang nạp mô hình {completedCount + 1}/{paths.Length}: " +
                Path.GetFileNameWithoutExtension(path);
            loader.LoadIFC(path);
            yield return null;

            while (!finished)
            {
                yield return null;
            }

            loader.LoadCompleted -= completed;
            loader.LoadFailed -= failed;
            completedCount++;
        }

        startupLoading = false;
        startupLoadRoutine = null;
        SetLoadingVisible(false);
        RebuildModelIndex();
        FrameModel();
        SetImportStatus(
            $"Đã tải {loader.LoadedModels.Count:N0} mô hình IFC mặc định.");
    }

    private void BuildModelList()
    {
        if (!uiBound || modelList == null)
        {
            return;
        }

        modelList.Clear();
        if (loader == null || loader.LoadedModels.Count == 0)
        {
            var empty = new Label("Chưa có file IFC đang mở.");
            empty.AddToClassList("category-empty");
            modelList.Add(empty);
            return;
        }

        foreach (var model in loader.LoadedModels.ToList())
        {
            if (model == null)
            {
                continue;
            }

            var row = new VisualElement();
            row.AddToClassList("model-row");

            var visibility = new Toggle
            {
                value = model.activeSelf,
                tooltip = $"Ẩn/hiện {model.name}"
            };
            visibility.AddToClassList("model-visibility");
            visibility.RegisterValueChangedCallback(change =>
            {
                if (model != null)
                {
                    model.SetActive(change.newValue);
                    SetImportStatus(
                        $"{(change.newValue ? "Đã hiện" : "Đã ẩn")} {model.name}.");
                }
            });

            var name = new Label(model.name)
            {
                tooltip = loader.GetModelSourcePath(model)
            };
            name.AddToClassList("model-name");

            var remove = new Button(() =>
            {
                var modelName = model.name;
                loader.RemoveModel(model);
                SetImportStatus($"Đã xóa mô hình {modelName} khỏi cảnh.");
            })
            {
                text = "×",
                tooltip = $"Xóa {model.name}"
            };
            remove.AddToClassList("model-remove");

            row.Add(visibility);
            row.Add(name);
            row.Add(remove);
            modelList.Add(row);
        }
    }

    private void ToggleModelManager()
    {
        BuildModelList();
        TogglePopup(modelManagerPopup);
    }

    private void ToggleMeasurementPopup()
    {
        if (measurementController != null &&
            measurementController.ActiveMode != IfcMeasurementMode.None)
        {
            measurementController.Stop();
            HidePopups();
            SetImportStatus("Đã thoát chế độ đo 3D.");
            return;
        }

        TogglePopup(measurementPopup);
    }

    private void ToggleExportPopup()
    {
        TogglePopup(exportPopup);
    }

    private void TogglePopup(VisualElement popup)
    {
        if (popup == null)
        {
            return;
        }

        var shouldOpen = popup.resolvedStyle.display == DisplayStyle.None;
        HidePopups();
        popup.style.display = shouldOpen ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void HidePopups()
    {
        modelManagerPopup.style.display = DisplayStyle.None;
        measurementPopup.style.display = DisplayStyle.None;
        exportPopup.style.display = DisplayStyle.None;
    }

    private void ClearModels()
    {
        if (loader == null || loader.IsLoading)
        {
            SetImportStatus("Không thể xóa mô hình trong khi đang nhập IFC.");
            return;
        }

        loader.ClearModels();
        CloseCustomPropertyEditor();
        CloseAnalyticsDashboard();
        HidePopups();
        SetImportStatus("Đã xóa tất cả mô hình IFC khỏi cảnh.");
    }

    private void BeginMeasurement(IfcMeasurementMode mode)
    {
        measurementController?.Begin(mode);
        HidePopups();
    }

    private void ClearMeasurements()
    {
        measurementController?.ClearMeasurements();
        HidePopups();
    }

    private void HandleMeasurementModeChanged(IfcMeasurementMode mode)
    {
        measureButton?.EnableInClassList(
            "toolbar-button-primary",
            mode != IfcMeasurementMode.None);
    }

    private void SetLoadingVisible(bool visible)
    {
        if (loadingOverlay != null)
        {
            loadingOverlay.style.display =
                visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void AnimateLoadingSpinner()
    {
        if (!startupLoading || loadingSpinner == null)
        {
            return;
        }

        spinnerAngle = (spinnerAngle - Time.unscaledDeltaTime * 260f) % 360f;
        loadingSpinner.style.rotate = new Rotate(new Angle(spinnerAngle));
    }

    private void FrameModel()
    {
        if (records.Count == 0)
        {
            return;
        }

        var bounds = records[0].Bounds;
        for (var index = 1; index < records.Count; index++)
        {
            bounds.Encapsulate(records[index].Bounds);
        }

        FrameBounds(bounds);
    }

    private void FrameBounds(Bounds bounds)
    {
        if (orbitCamera == null)
        {
            return;
        }

        orbitCamera.pivotPoint = bounds.center;
        var targetDistance = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 1.35f;
        orbitCamera.distance = Mathf.Clamp(
            Mathf.Max(targetDistance, orbitCamera.minDistance),
            orbitCamera.minDistance,
            orbitCamera.maxDistance);
    }

    private void OpenAnalyticsDashboard()
    {
        if (records.Count == 0)
        {
            SetImportStatus("Chưa có dữ liệu IFC để mở dashboard báo cáo.");
            return;
        }

        HidePopups();
        PopulateAnalyticsDashboard();
        analyticsOverlay.style.display = DisplayStyle.Flex;
    }

    private void CloseAnalyticsDashboard()
    {
        if (analyticsOverlay != null)
        {
            analyticsOverlay.style.display = DisplayStyle.None;
        }
    }

    private void PopulateAnalyticsDashboard()
    {
        var generatedAt = DateTime.Now;
        analyticsGeneratedAt.text =
            $"Cập nhật {generatedAt:dd/MM/yyyy HH:mm:ss} | {(loader?.LoadedModels.Count ?? 0):N0} mô hình IFC";
        analyticsTotalCount.text = records.Count.ToString("N0");

        var statusCounts = Enum
            .GetValues(typeof(IfcOperationalStatus))
            .Cast<IfcOperationalStatus>()
            .Select(status => records.Count(record => record.State.Status == status))
            .ToArray();
        analyticsOperationalCount.text = statusCounts[(int)IfcOperationalStatus.Operational].ToString("N0");
        analyticsWarningCount.text = statusCounts[(int)IfcOperationalStatus.Warning].ToString("N0");
        analyticsCriticalCount.text = statusCounts[(int)IfcOperationalStatus.Critical].ToString("N0");
        analyticsRepairingCount.text = statusCounts[(int)IfcOperationalStatus.Repairing].ToString("N0");
        analyticsModelCount.text = $"{(loader?.LoadedModels.Count ?? 0):N0} mô hình";

        statusDonutChart.SetData(statusCounts, StatusChartColors);
        BuildStatusLegend(statusCounts);
        BuildCategoryStatusChart();
        BuildSourceDistributionChart();
    }

    private void BuildStatusLegend(IReadOnlyList<int> statusCounts)
    {
        analyticsStatusLegend.Clear();
        foreach (IfcOperationalStatus status in Enum.GetValues(typeof(IfcOperationalStatus)))
        {
            var row = new VisualElement();
            row.AddToClassList("analytics-legend-row");

            var swatch = new VisualElement();
            swatch.AddToClassList("analytics-legend-swatch");
            swatch.style.backgroundColor = StatusChartColors[(int)status];

            var label = new Label(GetStatusLongLabel(status));
            label.AddToClassList("analytics-legend-label");
            var value = new Label(statusCounts[(int)status].ToString("N0"));
            value.AddToClassList("analytics-legend-value");

            row.Add(swatch);
            row.Add(label);
            row.Add(value);
            analyticsStatusLegend.Add(row);
        }
    }

    private void BuildCategoryStatusChart()
    {
        analyticsCategoryChart.Clear();
        analyticsCategoryLegend.Clear();
        var counts = new Dictionary<IfcInfrastructureCategory, int[]>();
        var maximum = 1;
        foreach (var definition in IfcInfrastructureClassifier.Definitions)
        {
            var values = new int[StatusChartColors.Length];
            foreach (IfcOperationalStatus status in Enum.GetValues(typeof(IfcOperationalStatus)))
            {
                values[(int)status] = records.Count(record =>
                    record.State.Category == definition.Category &&
                    record.State.Status == status);
                maximum = Math.Max(maximum, values[(int)status]);
            }

            counts[definition.Category] = values;
        }

        foreach (var definition in IfcInfrastructureClassifier.Definitions)
        {
            var group = new VisualElement();
            group.AddToClassList("analytics-column-group");
            var bars = new VisualElement();
            bars.AddToClassList("analytics-column-bars");
            var values = counts[definition.Category];
            foreach (IfcOperationalStatus status in Enum.GetValues(typeof(IfcOperationalStatus)))
            {
                var count = values[(int)status];
                var bar = new VisualElement
                {
                    tooltip = $"{definition.DisplayName} | {GetStatusLongLabel(status)}: {count:N0}"
                };
                bar.AddToClassList("analytics-column-bar");
                bar.style.height = Mathf.Max(3f, count / (float)maximum * 145f);
                bar.style.backgroundColor = StatusChartColors[(int)status];
                bars.Add(bar);
            }

            var total = values.Sum();
            var label = new Label($"{definition.Symbol}\n{total:N0}")
            {
                tooltip = definition.DisplayName
            };
            label.AddToClassList("analytics-column-label");
            group.Add(bars);
            group.Add(label);
            analyticsCategoryChart.Add(group);
        }

        foreach (IfcOperationalStatus status in Enum.GetValues(typeof(IfcOperationalStatus)))
        {
            var item = new VisualElement();
            item.AddToClassList("analytics-inline-legend-item");
            var swatch = new VisualElement();
            swatch.AddToClassList("analytics-legend-swatch");
            swatch.style.backgroundColor = StatusChartColors[(int)status];
            var label = new Label(GetStatusShortLabel(status));
            label.AddToClassList("analytics-legend-label");
            item.Add(swatch);
            item.Add(label);
            analyticsCategoryLegend.Add(item);
        }
    }

    private void BuildSourceDistributionChart()
    {
        analyticsSourceChart.Clear();
        var sources = records
            .GroupBy(record => record.SourceFile, StringComparer.OrdinalIgnoreCase)
            .Select(group => new
            {
                Name = Path.GetFileNameWithoutExtension(group.Key),
                Count = group.Count()
            })
            .OrderByDescending(source => source.Count)
            .ToArray();
        var maximum = Math.Max(1, sources.Select(source => source.Count).DefaultIfEmpty(1).Max());

        foreach (var source in sources)
        {
            var row = new VisualElement();
            row.AddToClassList("analytics-source-row");
            var name = new Label(source.Name)
            {
                tooltip = source.Name
            };
            name.AddToClassList("analytics-source-name");
            var track = new VisualElement();
            track.AddToClassList("analytics-source-track");
            var fill = new VisualElement();
            fill.AddToClassList("analytics-source-fill");
            fill.style.width = new Length(source.Count / (float)maximum * 100f, LengthUnit.Percent);
            track.Add(fill);
            var value = new Label($"{source.Count:N0}");
            value.AddToClassList("analytics-source-value");
            row.Add(name);
            row.Add(track);
            row.Add(value);
            analyticsSourceChart.Add(row);
        }
    }

    private List<IfcOperationsReportRow> BuildReportRows()
    {
        return records
            .OrderBy(record => record.Metadata.EntityLabel)
            .Select(record => new IfcOperationsReportRow(
                record.Metadata.EntityLabel,
                record.Name,
                IfcInfrastructureClassifier.GetDefinition(record.State.Category).DisplayName,
                record.State.Status,
                record.SourceFile))
            .ToList();
    }

    private void ExportReport()
    {
        if (records.Count == 0)
        {
            SetImportStatus("Chưa có dữ liệu IFC để xuất báo cáo.");
            return;
        }

        var builder = new StringBuilder();
        builder.AppendLine(
            "GlobalID,ExpressID,Name,IfcType,Category,Status,Triangles,SizeX,SizeY,SizeZ," +
            "VN2000X,VN2000Y,MaintenanceNote,UpdatedAt");

        foreach (var record in records)
        {
            builder.AppendLine(string.Join(
                ",",
                Csv(record.State.OperationsGlobalId),
                record.Metadata.EntityLabel.ToString(CultureInfo.InvariantCulture),
                Csv(record.Name),
                Csv(record.IfcType),
                Csv(IfcInfrastructureClassifier.GetDefinition(record.State.Category).DisplayName),
                record.State.Status,
                record.TriangleCount.ToString(CultureInfo.InvariantCulture),
                record.Bounds.size.x.ToString("F3", CultureInfo.InvariantCulture),
                record.Bounds.size.y.ToString("F3", CultureInfo.InvariantCulture),
                record.Bounds.size.z.ToString("F3", CultureInfo.InvariantCulture),
                record.Vn2000X.ToString("F3", CultureInfo.InvariantCulture),
                record.Vn2000Y.ToString("F3", CultureInfo.InvariantCulture),
                Csv(record.State.MaintenanceNote),
                Csv(record.State.UpdatedAt)));
        }

        var fileName = $"IFC-Operations-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
        var path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, builder.ToString(), new UTF8Encoding(true));
        SetImportStatus($"Đã xuất báo cáo: {fileName}");
        Debug.Log($"IFC operations report exported to {path}");
        HidePopups();
    }

    private void ExportJson()
    {
        if (records.Count == 0)
        {
            SetImportStatus("Chưa có dữ liệu IFC để xuất JSON.");
            return;
        }

        var builder = new StringBuilder();
        builder.AppendLine("{");
        builder.AppendLine($"  \"project\": \"{Json(ProjectName)}\",");
        builder.AppendLine($"  \"modelCount\": {loader.LoadedModels.Count},");
        builder.AppendLine("  \"elements\": [");

        for (var index = 0; index < records.Count; index++)
        {
            var record = records[index];
            EnsureElementPropertiesLoaded(record);
            builder.AppendLine("    {");
            builder.AppendLine(
                $"      \"globalId\": \"{Json(record.State.OperationsGlobalId)}\",");
            builder.AppendLine(
                $"      \"expressId\": {record.Metadata.EntityLabel},");
            builder.AppendLine($"      \"name\": \"{Json(record.Name)}\",");
            builder.AppendLine($"      \"ifcType\": \"{Json(record.IfcType)}\",");
            builder.AppendLine(
                $"      \"category\": \"{Json(IfcInfrastructureClassifier.GetDefinition(record.State.Category).DisplayName)}\",");
            builder.AppendLine($"      \"status\": \"{record.State.Status}\",");
            builder.AppendLine($"      \"triangles\": {record.TriangleCount},");
            builder.AppendLine(
                $"      \"vn2000\": {{ \"x\": {record.Vn2000X.ToString("R", CultureInfo.InvariantCulture)}, " +
                $"\"y\": {record.Vn2000Y.ToString("R", CultureInfo.InvariantCulture)} }},");
            builder.AppendLine("      \"customProperties\": {");
            var customProperties = record.CustomProperties
                .OrderBy(pair => pair.Key, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            for (var propertyIndex = 0; propertyIndex < customProperties.Length; propertyIndex++)
            {
                var property = customProperties[propertyIndex];
                builder.Append(
                    $"        \"{Json(property.Key)}\": \"{Json(property.Value)}\"");
                builder.AppendLine(
                    propertyIndex + 1 < customProperties.Length ? "," : string.Empty);
            }

            builder.AppendLine("      },");
            builder.AppendLine("      \"propertyOverrides\": {");
            var propertyOverrides = record.PropertyOverrides
                .OrderBy(pair => pair.Key, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            for (var propertyIndex = 0; propertyIndex < propertyOverrides.Length; propertyIndex++)
            {
                var property = propertyOverrides[propertyIndex];
                builder.Append(
                    $"        \"{Json(property.Key)}\": \"{Json(property.Value)}\"");
                builder.AppendLine(
                    propertyIndex + 1 < propertyOverrides.Length ? "," : string.Empty);
            }

            builder.AppendLine("      },");
            builder.AppendLine("      \"deletedProperties\": [");
            var deletedProperties = record.DeletedProperties
                .OrderBy(value => value, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            for (var propertyIndex = 0; propertyIndex < deletedProperties.Length; propertyIndex++)
            {
                builder.Append($"        \"{Json(deletedProperties[propertyIndex])}\"");
                builder.AppendLine(
                    propertyIndex + 1 < deletedProperties.Length ? "," : string.Empty);
            }

            builder.AppendLine("      ]");
            builder.Append("    }");
            builder.AppendLine(index + 1 < records.Count ? "," : string.Empty);
        }

        builder.AppendLine("  ]");
        builder.AppendLine("}");

        var fileName = $"IFC-BIM-Data-{DateTime.Now:yyyyMMdd-HHmmss}.json";
        var path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, builder.ToString(), new UTF8Encoding(false));
        SetImportStatus($"Đã xuất dữ liệu BIM: {fileName}");
        Debug.Log($"IFC BIM JSON exported to {path}");
        HidePopups();
    }

    private void ExportPdfReport()
    {
        if (records.Count == 0)
        {
            SetImportStatus("Chưa có dữ liệu IFC để in báo cáo.");
            return;
        }

        var generatedAt = DateTime.Now;
        var fileName = $"IFC-Operations-{generatedAt:yyyyMMdd-HHmmss}.pdf";
        var path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(
            path,
            IfcOperationsReport.BuildPdf(
                ProjectName,
                generatedAt,
                loader?.LoadedModels.Count ?? 0,
                BuildReportRows()));
        SetImportStatus($"Đã tạo báo cáo PDF: {fileName}");
        if (analyticsGeneratedAt != null)
        {
            analyticsGeneratedAt.text = $"Đã xuất {fileName}";
        }
        Debug.Log($"IFC operations PDF exported to {path}");
    }

    private void HandleSceneSelection()
    {
        ResolveDependencies();
        if (Mouse.current == null ||
            records.Count == 0 ||
            measurementController?.ActiveMode != IfcMeasurementMode.None)
        {
            pendingSceneClick = false;
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            scenePointerDownPosition = Mouse.current.position.ReadValue();
            scenePointerDownTime = Time.unscaledTime;
            pendingSceneClick = !IsPointerOverDashboard(scenePointerDownPosition);
        }

        if (!Mouse.current.leftButton.wasReleasedThisFrame ||
            !pendingSceneClick)
        {
            return;
        }

        pendingSceneClick = false;
        var pointerPosition = Mouse.current.position.ReadValue();
        var heldDuration = Time.unscaledTime - scenePointerDownTime;
        if (heldDuration > MaximumClickDurationSeconds ||
            Vector2.Distance(scenePointerDownPosition, pointerPosition) >
            ClickTolerancePixels)
        {
            return;
        }

        TrySelectSceneElement(pointerPosition);
    }

    private bool TrySelectSceneElement(Vector2 pointerPosition)
    {
        if (IsPointerOverDashboard(pointerPosition))
        {
            return false;
        }

        if (!IfcInteractionRaycaster.TryRaycast(
                viewingCamera,
                pointerPosition,
                out var hit,
                out var metadata))
        {
            return false;
        }

        var record = FindRecordForHit(hit.transform, metadata);
        if (record == null)
        {
            return false;
        }

        SelectRecord(record, false);
        return true;
    }

    private void SetSelectionHighlight(IfcAssetRecord record, bool highlighted)
    {
        if (record == null)
        {
            return;
        }

        foreach (var renderer in record.Renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            var materials = renderer.sharedMaterials;
            for (var materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                if (!highlighted)
                {
                    renderer.SetPropertyBlock(null, materialIndex);
                    continue;
                }

                var material = materials[materialIndex];
                if (material == null)
                {
                    continue;
                }

                var sourceColor = material.HasProperty(BaseColorId)
                    ? material.GetColor(BaseColorId)
                    : material.HasProperty(ColorId)
                        ? material.GetColor(ColorId)
                        : Color.white;
                var selectedColor = new Color(
                    sourceColor.r * SelectedColorMultiplier,
                    sourceColor.g * SelectedColorMultiplier,
                    sourceColor.b * SelectedColorMultiplier,
                    sourceColor.a);

                selectionPropertyBlock.Clear();
                renderer.GetPropertyBlock(selectionPropertyBlock, materialIndex);
                selectionPropertyBlock.SetColor(BaseColorId, selectedColor);
                selectionPropertyBlock.SetColor(ColorId, selectedColor);
                renderer.SetPropertyBlock(selectionPropertyBlock, materialIndex);
            }
        }
    }

    private IfcAssetRecord FindRecordForHit(
        Transform hitTransform,
        IfcElementMetadata metadata)
    {
        if (metadata != null &&
            recordsByMetadata.TryGetValue(metadata, out var metadataRecord))
        {
            return metadataRecord;
        }

        for (var current = hitTransform; current != null; current = current.parent)
        {
            if (recordsByGeometry.TryGetValue(current, out var geometryRecord))
            {
                return geometryRecord;
            }

            if (current.TryGetComponent<IfcElementMetadata>(out var owner) &&
                recordsByMetadata.TryGetValue(owner, out var ownerRecord))
            {
                return ownerRecord;
            }
        }

        return records.FirstOrDefault(record =>
            record.Renderers.Any(renderer =>
                renderer != null &&
                (renderer.transform == hitTransform ||
                 hitTransform.IsChildOf(renderer.transform) ||
                 renderer.transform.IsChildOf(hitTransform))));
    }

    private bool IsPointerOverDashboard(Vector2 screenPosition)
    {
        return IfcUiHitTest.IsPointerOverInteractiveUi(document, screenPosition);
    }

    private void HandleMeasurementHudChanged(string title, string value, bool visible)
    {
        if (measurementHud == null)
        {
            return;
        }

        measurementHudTitle.text = title;
        measurementHudValue.text = value;
        measurementHud.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private static string GetElementKey(IfcElementMetadata metadata)
    {
        return !string.IsNullOrWhiteSpace(metadata.GlobalId)
            ? metadata.GlobalId
            : $"#{metadata.EntityLabel}";
    }

    private void HandleGeometryChanged(GeometryChangedEvent change)
    {
        var width = change.newRect.width;
        root.EnableInClassList("dashboard-compact", width < 1450f);
        root.EnableInClassList("dashboard-narrow", width < 1000f);
    }

    private void SetImportStatus(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if (startupLoading && loadingMessage != null)
        {
            loadingMessage.text = message;
            return;
        }

        if (importStatusLabel == null || statusStrip == null)
        {
            return;
        }

        if (toastRoutine != null)
        {
            StopCoroutine(toastRoutine);
        }

        importStatusLabel.text = message;
        statusStrip.style.display = DisplayStyle.Flex;
        statusStrip.AddToClassList("status-strip-visible");
        toastRoutine = StartCoroutine(HideStatusAfterDelay());
    }

    private IEnumerator HideStatusAfterDelay()
    {
        yield return new WaitForSecondsRealtime(3.2f);
        statusStrip.RemoveFromClassList("status-strip-visible");
        yield return new WaitForSecondsRealtime(0.4f);
        statusStrip.style.display = DisplayStyle.None;
        toastRoutine = null;
    }

    private static string GetStatusShortLabel(IfcOperationalStatus status)
    {
        return status switch
        {
            IfcOperationalStatus.Operational => "Tốt",
            IfcOperationalStatus.Warning => "Bảo trì",
            IfcOperationalStatus.Critical => "Hỏng hóc",
            IfcOperationalStatus.Repairing => "Đang sửa",
            _ => status.ToString()
        };
    }

    private static string GetStatusLongLabel(IfcOperationalStatus status)
    {
        return status switch
        {
            IfcOperationalStatus.Operational => "Tốt / Hoạt động bình thường",
            IfcOperationalStatus.Warning => "Cần bảo trì định kỳ",
            IfcOperationalStatus.Critical => "Sự cố / Hỏng hóc",
            IfcOperationalStatus.Repairing => "Đang sửa chữa",
            _ => status.ToString()
        };
    }

    private static int StatusFilterKey(IfcOperationalStatus? status)
    {
        return status.HasValue ? (int)status.Value : -1;
    }

    private static string FormatColor(Color color)
    {
        return $"R:{color.r:F3} G:{color.g:F3} B:{color.b:F3} A:{color.a:F3}";
    }

    private static string EmptyFallback(string value, string fallback = "Không có")
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static string Csv(string value)
    {
        return $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";
    }

    private static string Json(string value)
    {
        return (value ?? string.Empty)
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
    }

    private static byte[] BuildSimplePdf(IReadOnlyList<string> sourceLines)
    {
        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 14 Tf");
        content.AppendLine("48 795 Td");

        for (var index = 0; index < sourceLines.Count; index++)
        {
            if (index == 1)
            {
                content.AppendLine("/F1 10 Tf");
            }

            var line = EscapePdfText(ToAscii(sourceLines[index]));
            content.AppendLine($"({line}) Tj");
            content.AppendLine($"0 -{(index == 0 ? 22 : 16)} Td");
        }

        content.AppendLine("ET");
        var streamText = content.ToString();
        var objects = new[]
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] " +
            "/Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
            $"<< /Length {Encoding.ASCII.GetByteCount(streamText)} >>\n" +
            $"stream\n{streamText}endstream"
        };

        using var output = new MemoryStream();
        WriteAscii(output, "%PDF-1.4\n");
        var offsets = new long[objects.Length + 1];
        for (var index = 0; index < objects.Length; index++)
        {
            offsets[index + 1] = output.Position;
            WriteAscii(output, $"{index + 1} 0 obj\n{objects[index]}\nendobj\n");
        }

        var xrefOffset = output.Position;
        WriteAscii(output, $"xref\n0 {objects.Length + 1}\n");
        WriteAscii(output, "0000000000 65535 f \n");
        for (var index = 1; index < offsets.Length; index++)
        {
            WriteAscii(output, $"{offsets[index]:D10} 00000 n \n");
        }

        WriteAscii(
            output,
            $"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\n" +
            $"startxref\n{xrefOffset}\n%%EOF");
        return output.ToArray();
    }

    private static void WriteAscii(Stream stream, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static string EscapePdfText(string value)
    {
        return value
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
            if (CharUnicodeInfo.GetUnicodeCategory(character) !=
                UnicodeCategory.NonSpacingMark)
            {
                result.Append(character <= 127 ? character : '?');
            }
        }

        return result.ToString().Normalize(NormalizationForm.FormC);
    }

    private sealed class DonutChartElement : VisualElement
    {
        private int[] values = Array.Empty<int>();
        private Color[] colors = Array.Empty<Color>();

        public DonutChartElement()
        {
            generateVisualContent += DrawChart;
            RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
        }

        public void SetData(IReadOnlyList<int> chartValues, IReadOnlyList<Color> chartColors)
        {
            values = chartValues?.ToArray() ?? Array.Empty<int>();
            colors = chartColors?.ToArray() ?? Array.Empty<Color>();
            MarkDirtyRepaint();
        }

        private void DrawChart(MeshGenerationContext context)
        {
            var total = values.Sum();
            if (total <= 0 || contentRect.width <= 1f || contentRect.height <= 1f)
            {
                return;
            }

            var segmentCounts = new int[values.Length];
            var totalSegments = 0;
            for (var index = 0; index < values.Length; index++)
            {
                if (values[index] <= 0)
                {
                    continue;
                }

                var span = 360f * values[index] / total;
                segmentCounts[index] = Mathf.Max(2, Mathf.CeilToInt(span / 5f));
                totalSegments += segmentCounts[index];
            }

            if (totalSegments == 0)
            {
                return;
            }

            var vertices = new Vertex[totalSegments * 4];
            var indices = new ushort[totalSegments * 6];
            var center = contentRect.center;
            var outerRadius = Mathf.Min(contentRect.width, contentRect.height) * 0.46f;
            var innerRadius = outerRadius * 0.58f;
            var angle = -90f;
            var vertexCursor = 0;
            var indexCursor = 0;

            for (var valueIndex = 0; valueIndex < values.Length; valueIndex++)
            {
                if (values[valueIndex] <= 0)
                {
                    continue;
                }

                var span = 360f * values[valueIndex] / total;
                var segmentAngle = span / segmentCounts[valueIndex];
                var color = valueIndex < colors.Length ? colors[valueIndex] : Color.gray;
                for (var segment = 0; segment < segmentCounts[valueIndex]; segment++)
                {
                    var start = angle + segment * segmentAngle;
                    var end = start + segmentAngle + 0.12f;
                    vertices[vertexCursor] = ChartVertex(PointOnCircle(center, outerRadius, start), color);
                    vertices[vertexCursor + 1] = ChartVertex(PointOnCircle(center, outerRadius, end), color);
                    vertices[vertexCursor + 2] = ChartVertex(PointOnCircle(center, innerRadius, end), color);
                    vertices[vertexCursor + 3] = ChartVertex(PointOnCircle(center, innerRadius, start), color);

                    indices[indexCursor] = (ushort)vertexCursor;
                    indices[indexCursor + 1] = (ushort)(vertexCursor + 1);
                    indices[indexCursor + 2] = (ushort)(vertexCursor + 2);
                    indices[indexCursor + 3] = (ushort)vertexCursor;
                    indices[indexCursor + 4] = (ushort)(vertexCursor + 2);
                    indices[indexCursor + 5] = (ushort)(vertexCursor + 3);
                    vertexCursor += 4;
                    indexCursor += 6;
                }

                angle += span;
            }

            var mesh = context.Allocate(vertices.Length, indices.Length);
            mesh.SetAllVertices(vertices);
            mesh.SetAllIndices(indices);
        }

        private static Vector2 PointOnCircle(Vector2 center, float radius, float angleDegrees)
        {
            var radians = angleDegrees * Mathf.Deg2Rad;
            return center + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
        }

        private static Vertex ChartVertex(Vector2 position, Color color)
        {
            return new Vertex
            {
                position = new Vector3(position.x, position.y, Vertex.nearZ),
                tint = color,
                uv = Vector2.zero
            };
        }
    }

    private sealed class IfcAssetRecord
    {
        public IfcElementMetadata Metadata;
        public IfcOperationsState State;
        public List<Renderer> Renderers;
        public string Name;
        public string IfcType;
        public string IfcGlobalId;
        public string SourceFile;
        public Bounds Bounds;
        public Color Color;
        public long TriangleCount;
        public long VertexCount;
        public long NormalCount;
        public long IndexCount;
        public double Vn2000X;
        public double Vn2000Y;
        public readonly Dictionary<string, string> CustomProperties =
            new(StringComparer.CurrentCultureIgnoreCase);
        public bool CustomPropertiesLoaded;
        public readonly Dictionary<string, string> PropertyOverrides =
            new(StringComparer.CurrentCultureIgnoreCase);
        public readonly HashSet<string> DeletedProperties =
            new(StringComparer.CurrentCultureIgnoreCase);
        public bool PropertyOverridesLoaded;
    }

    private struct ModelContext
    {
        public string SourceFile;
        public double MetresPerUnit;
        public double OriginX;
        public double OriginY;
    }
}
