using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;



public class TracingTooltipController : TracingUIPanelController<TracingTooltipController>
{
    public struct AttributeInfo
    {
        public Label label;
        public string name;
        public Func<string> valueGetter;
    }

    protected static HashSet<TracingTooltipController> openedTooltips = new();

    protected TracingTooltipPreset tracingTooltipPreset { get; private set; }

    [ShowInInspector]
    protected Label title, description;

    [ShowInInspector]
    protected VisualElement attributeContainer;

    [ShowInInspector]
    protected ITracingTooltipProvider tooltipProvider;

    [ShowInInspector]
    private List<AttributeInfo> dynamicAttributeInfos = new();

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);

        tracingTooltipPreset = preset as TracingTooltipPreset;

        Note.note.AssertIsNotNull(tracingTooltipPreset, nameof(tracingTooltipPreset));
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        title = rootVisualElement.Q<Label>(tracingTooltipPreset.titleLabelName);
        description = rootVisualElement.Q<Label>(tracingTooltipPreset.descriptionLabelName);
        attributeContainer = rootVisualElement.Q(tracingTooltipPreset.attributeContainerName);

        Note.note.AssertIsNotNull(title, nameof(title));
        Note.note.AssertIsNotNull(description, nameof(description));
        Note.note.AssertIsNotNull(attributeContainer, nameof(attributeContainer));

        openedTooltips.Add(this);
    }

    protected override void OnClose()
    {
        base.OnClose();

        if (tooltipProvider != null)
        {
            if (tooltipProvider.TryGetTooltipBindGlobalEvent(out var globalEvent))
            {
                globalEvent.OnEnabledStateChangedEvent += OnGlobalEventEnabledStateChanged;
            }

            tooltipProvider = null;
        }

        title = null;
        description = null;

        dynamicAttributeInfos.Clear();

        openedTooltips.Remove(this);
    }

    private void FixedUpdate()
    {
        if (isOpen)
        {
            if (dynamicAttributeInfos.Count > 0)
            {
                foreach (var attributeInfo in dynamicAttributeInfos)
                {
                    attributeInfo.label.text = $"{attributeInfo.name}: {attributeInfo.valueGetter()}";
                }
            }
        }
    }

    private void OnGlobalEventEnabledStateChanged(bool enabled)
    {
        if (enabled == false)
        {
            Close();
        }
    }

    public static void Open(ITracingTooltipProvider tooltipProvider, UIPanelController source)
    {
        if (tooltipProvider == null)
        {
            return;
        }

        var tooltipID = tooltipProvider.GetTooltipID();

        var tooltip = UIPanelManager.GetUniqueUIPanel(tooltipID) as TracingTooltipController;

        Note.note.AssertIsNotNull(tooltip, nameof(tooltip));

        if (tooltip.tooltipProvider == tooltipProvider)
        {
            return;
        }

        if (tooltipProvider.TryGetTooltipBindGlobalEvent(out var globalEvent))
        {
            if (globalEvent.enabled == false)
            {
                return;
            }

            globalEvent.OnEnabledStateChangedEvent += tooltip.OnGlobalEventEnabledStateChanged;
        }

        if (openedTooltips.Count > 0)
        {
            var minPriority = int.MinValue;

            var willClosedTooltips = new List<TracingTooltipController>();

            foreach (var openedTooltip in openedTooltips)
            {
                var openedTooltipPriority = openedTooltip.tracingTooltipPreset.tooltipPriority;

                if (openedTooltipPriority < tooltip.tracingTooltipPreset.tooltipPriority)
                {
                    willClosedTooltips.Add(openedTooltip);
                }
                else if (openedTooltipPriority > minPriority)
                {
                    minPriority = openedTooltip.tracingTooltipPreset.tooltipPriority;
                }
            }

            foreach (var willClosedTooltip in willClosedTooltips)
            {
                ((UIPanelController)willClosedTooltip).Close();
            }

            if (minPriority > tooltip.tracingTooltipPreset.tooltipPriority)
            {
                return;
            }
        }

        tooltip.Open(source);

        tooltip.tooltipProvider = tooltipProvider;

        var title = tooltip.title;
        var description = tooltip.description;
        var attributeContainer = tooltip.attributeContainer;

        attributeContainer.Clear();

        title.text = tooltipProvider.GetTooltipTitle();

        var attributes = tooltipProvider.GetTooltipProperties();

        foreach (var attribute in attributes)
        {
            var attributeLabel = new Label();
            attributeContainer.Add(attributeLabel);

            attributeLabel.text = $"{attribute.attributeName}: {attribute.attributeValueGetter()}";

            if (attribute.isStatic == false)
            {
                tooltip.dynamicAttributeInfos.Add(new AttributeInfo
                {
                    label = attributeLabel,
                    valueGetter = attribute.attributeValueGetter,
                    name = attribute.attributeName
                });
            }
        }

        var descriptionText = tooltipProvider.GetTooltipDescription();
        if (descriptionText.IsNullOrEmpty() == false)
        {
            description.text = descriptionText;
        }
        else
        {
            description.style.display = DisplayStyle.None;
        }
    }

    public static void Close(ITracingTooltipProvider tooltipProvider)
    {
        if (tooltipProvider == null)
        {
            Note.note.Warning($"{nameof(tooltipProvider)} is null");
            return;
        }

        var tooltip = UIPanelManager.GetUniqueUIPanel(tooltipProvider.GetTooltipID()) as TracingTooltipController;

        Note.note.AssertIsNotNull(tooltip, nameof(tooltip));

        if (tooltip.tooltipProvider == tooltipProvider)
        {
            ((UIPanelController)tooltip).Close();
        }
    }
}
