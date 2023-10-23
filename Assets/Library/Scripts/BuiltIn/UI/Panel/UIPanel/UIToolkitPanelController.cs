using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIToolkitPanelController : UIPanelController
{
    protected UIDocument uiDocument => GetComponent<UIDocument>();

    protected UIToolkitPanelPreset uiToolkitPanelPreset { get; private set; }

    protected VisualElement rootVisualElement;

    #region Init

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);
        
        uiToolkitPanelPreset = preset as UIToolkitPanelPreset;

        Note.note.AssertIsNotNull(uiToolkitPanelPreset, nameof(uiToolkitPanelPreset));

        var uiDocument = this.GetOrAddComponent<UIDocument>();

        uiDocument.panelSettings = uiToolkitPanelPreset.panelSettings;
        uiDocument.visualTreeAsset = uiToolkitPanelPreset.visualTree;

        uiDocument.enabled = true;
    }

    protected override void OnPostInit(UIPanelPreset preset)
    {
        base.OnPostInit(preset);

        InitTooltipBinds();
    }

    #endregion

    protected override void OnOpen()
    {
        base.OnOpen();

        uiDocument.enabled = true;

        rootVisualElement = uiDocument.rootVisualElement;

        rootVisualElement.style.visibility = Visibility.Hidden;

        if (uiToolkitPanelPreset.closeUIButtonName.IsNullOrEmpty() == false)
        {
            var closeButton = rootVisualElement.Q<Button>(uiToolkitPanelPreset.closeUIButtonName);

            closeButton.clicked += Close;
        }

        BindTooltips();
    }

    protected override void OnClose()
    {
        base.OnClose();

        uiDocument.enabled = false;
    }

    protected override void OnPostLayoutChange()
    {
        base.OnPostLayoutChange();

        rootVisualElement.style.visibility = Visibility.Visible;

        foreach (var visualElement in rootVisualElement.Children())
        {
            visualElement.RegisterCallback<MouseEnterEvent>(OnPointerEnter);
            visualElement.RegisterCallback<MouseLeaveEvent>(OnPointerLeave);
        }

        if (uiToolkitPanelPreset.ignoreMouseEvents)
        {
            foreach (var visualElement in rootVisualElement.GetAll<VisualElement>())
            {
                visualElement.pickingMode = PickingMode.Ignore;
            }
        }
    }

    protected override void OnCurrentLanguageChanged(string previousLanguage, string currentLanguage)
    {
        base.OnCurrentLanguageChanged(previousLanguage, currentLanguage);

        var previousLanguageConfig = GameCoreSettingBase.uiPanelGeneralSetting.languageConfigs.GetConfig(previousLanguage);

        if (previousLanguageConfig != null)
        {
            rootVisualElement.styleSheets.Remove(previousLanguageConfig.styleSheet);
        }

        var currentLanguageConfig = GameCoreSettingBase.uiPanelGeneralSetting.languageConfigs.GetConfig(currentLanguage);

        if (currentLanguageConfig != null)
        {
            rootVisualElement.styleSheets.Add(currentLanguageConfig.styleSheet);
        }
    }

    protected void AddVisualElement(VisualElement parent, VisualElement newChild)
    {
        parent.Add(newChild);

        OnNewVisualElementPostProcessing(newChild);
    }

    protected virtual void OnNewVisualElementPostProcessing(VisualElement newVisualElement)
    {
        if (uiToolkitPanelPreset.ignoreMouseEvents)
        {
            foreach (var visualElement in newVisualElement.GetAll<VisualElement>())
            {
                visualElement.pickingMode = PickingMode.Ignore;
            }
        }
    }

    #region Bind Tooltip

    [ShowInInspector]
    private Dictionary<string, string> tooltipBinds = new();

    private void InitTooltipBinds()
    {
        if (uiToolkitPanelPreset.tooltipBindConfigs.IsNullOrEmpty() == false)
        {
            foreach (var tooltipBindConfig in uiToolkitPanelPreset.tooltipBindConfigs)
            {
                if (tooltipBindConfig.targetName.IsNullOrEmpty())
                {
                    continue;
                }

                if (tooltipBindConfig.tooltipName.IsNullOrEmpty())
                {
                    continue;
                }

                tooltipBinds[tooltipBindConfig.targetName] = tooltipBindConfig.tooltipName;
            }
        }

        foreach (var tooltipBindConfig in OnInitTooltipBindConfigs())
        {
            if (tooltipBindConfig.Item1.IsNullOrEmpty())
            {
                continue;
            }

            if (tooltipBindConfig.Item2.IsNullOrEmpty())
            {
                continue;
            }

            tooltipBinds[tooltipBindConfig.Item1] = tooltipBindConfig.Item2;
        }
    }

    private void BindTooltips()
    {
        foreach (var (targetName, tooltipName) in tooltipBinds)
        {
            var target = rootVisualElement.Q(targetName);

            if (target == null)
            {
                Note.note.Warning($"{targetName}不存在");
                continue;
            }

            var tooltip = rootVisualElement.Q(tooltipName);

            if (tooltip == null)
            {
                Note.note.Warning($"{tooltipName}不存在");
                continue;
            }

            tooltip.style.opacity = 0;

            target.RegisterCallback<MouseEnterEvent>(evt =>
            {
                tooltip.DOKill();
                tooltip.style.opacity = 1;
            });

            target.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                tooltip.DOKill();
                tooltip.DOFade(0, uiToolkitPanelPreset.tooltipFadeDuration);
            });
        }
    }

    protected virtual IEnumerable<(string, string)> OnInitTooltipBindConfigs()
    {
        return Enumerable.Empty<(string, string)>();
    }

    #endregion
}
