using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPanelController : MonoBehaviour
{
    #region Panel On Mouse Hover

    private static UIPanelController _panelOnMouseHover;

    [ShowInInspector]
    protected static UIPanelController panelOnMouseHover
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _panelOnMouseHover;
        private set
        {
            var oldPanelOnMouseHover = _panelOnMouseHover;
            _panelOnMouseHover = value;
            OnPanelOnMouseHoverChanged?.Invoke(oldPanelOnMouseHover, _panelOnMouseHover);
        }
    }

    protected static event Action<UIPanelController, UIPanelController> OnPanelOnMouseHoverChanged;

    #endregion

    #region Panel On Mouse Click

    private static UIPanelController _panelOnMouseClick;

    [ShowInInspector]
    protected static UIPanelController panelOnMouseClick
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _panelOnMouseClick;
        private set
        {
            var oldPanelOnMouseClick = _panelOnMouseClick;
            _panelOnMouseClick = value;
            OnPanelOnMouseClickChanged?.Invoke(oldPanelOnMouseClick, _panelOnMouseClick);
        }
    }

    protected static event Action<UIPanelController, UIPanelController> OnPanelOnMouseClickChanged;

    #endregion

    [ShowInInspector]
    protected bool isDebugging { get; private set; } = false;

    [ShowInInspector]
    public UIPanelPreset preset { get; private set; }

    [ShowInInspector]
    public bool isOpen { get; private set; }

    [ShowInInspector]
    public bool uiEnabled { get; private set; } = false;

    [ShowInInspector]
    public bool mouseOnUI { get; private set; } = false;

    [ShowInInspector]
    protected UIPanelController sourceUIPanel {get; private set; }

    public event Action OnOpenEvent;

    public event Action OnCloseEvent;

    protected event Action OnCrashEvent;

    #region Static Update

    private static void StaticUpdate()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            panelOnMouseClick = panelOnMouseHover;
        }
    }

    #endregion

    #region Init & Destroy

    public void Init(UIPanelPreset preset)
    {
        Note.note.Begin($"加载UI {preset.name}");

        OnPreInit(preset);
        OnInit(preset);
        OnPostInit(preset);

        if (UpdateDelegateManager.HasUpdateDelegate(UpdateType.Update, StaticUpdate) == false)
        {
            UpdateDelegateManager.AddUpdateDelegate(UpdateType.Update, StaticUpdate);
        }

        Note.note.End();
    }

    protected virtual void OnPreInit(UIPanelPreset preset)
    {
        this.preset = preset;
    }

    protected virtual void OnInit(UIPanelPreset preset)
    {
        if (preset.isDebugging)
        {
            isDebugging = true;
        }

        if (preset.enableCloseInputMapping)
        {
            GlobalEventManager.AddEvent(preset.closeInputMappingID, CloseEvent);
        }

        if (preset.enableToggleInputMapping)
        {
            GlobalEventManager.AddEvent(preset.toggleInputMappingID, ToggleEvent);
        }

        foreach (var procedureID in preset.autoOpenOnEnterProcedures)
        {
            ProcedureManager.AddOnEnterEvent(procedureID, Open);
        }

        foreach (var procedureID in preset.autoOpenOnLeaveProcedures)
        {
            ProcedureManager.AddOnLeaveEvent(procedureID, Open);
        }

        foreach (var procedureID in preset.autoCloseOnEnterProcedures)
        {
            ProcedureManager.AddOnEnterEvent(procedureID, Close);
        }

        foreach (var procedureID in preset.autoCloseOnLeaveProcedures)
        {
            ProcedureManager.AddOnLeaveEvent(procedureID, Close);
        }
    }

    protected virtual void OnPostInit(UIPanelPreset preset)
    {

    }

    private void CloseEvent(bool eventState)
    {
        if (eventState && isOpen)
        {
            Close();
        }
    }

    private void ToggleEvent(bool eventState)
    {
        if (eventState)
        {
            Toggle();
        }
    }

    protected virtual void OnDestroy()
    {
        if (preset.enableCloseInputMapping)
        {
            GlobalEventManager.RemoveEvent(preset.closeInputMappingID, CloseEvent);
        }

        if (preset.enableToggleInputMapping)
        {
            GlobalEventManager.RemoveEvent(preset.toggleInputMappingID, ToggleEvent);
        }

        if (isOpen)
        {
            foreach (var inputMappingID in preset.globalEventDisabledListOnOpen)
            {
                GlobalEventManager.EnableEvent(inputMappingID);
            }
        }
    }

    #endregion

    #region Basic Event

    protected virtual void OnCrash()
    {
        GameCoreSettingBase.translationGeneralSetting.OnCurrentLanguageChanged -= OnCurrentLanguageChanged;

        OnCrashEvent?.Invoke();

        if (preset.isUnique)
        {
            UIPanelManager.RecreateUniquePanel(preset.id);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnRecreate(UIPanelController newPanel)
    {
        if (isOpen)
        {
            newPanel.Open();
        }
    }

    protected virtual void OnOpen()
    {
        
    }

    protected virtual void OnClose()
    {

    }

    protected virtual void OnSetEnabled()
    {

    }

    protected virtual void OnLayoutChange()
    {

    }

    protected virtual void OnPostLayoutChange()
    {

    }

    protected virtual void OnCurrentLanguageChanged(string previousLanguage, string currentLanguage)
    {
        
    }

    #endregion

    #region Pointer Event

    protected void OnPointerEnter(MouseEnterEvent mouseEnterEvent)
    {
        mouseOnUI = true;

        panelOnMouseHover = this;

        foreach (var inputMappingID in preset.globalEventDisabledListOnMouseEnter)
        {
            GlobalEventManager.DisableEvent(inputMappingID);
        }

        if (isDebugging)
        {
            Note.note.Warning($"{name}鼠标进入");
        }
    }

    protected void OnPointerLeave(MouseLeaveEvent mouseLeaveEvent)
    {
        mouseOnUI = false;

        if (panelOnMouseHover == this)
        {
            panelOnMouseHover = null;
        }

        foreach (var inputMappingID in preset.globalEventDisabledListOnMouseEnter)
        {
            GlobalEventManager.EnableEvent(inputMappingID);
        }

        if (isDebugging)
        {
            Note.note.Warning($"{name}鼠标离开");
        }
    }

    #endregion

    #region Basic Operation

    [Button("打开")]
    public void Open()
    {
        Open(null);
    }

    public void Open(UIPanelController source)
    {
        if (isDebugging)
        {
            Note.note.Warning($"{name}打开");
        }

        if (isOpen)
        {
            return;
        }
        
        foreach (var inputMappingID in preset.globalEventDisabledListOnOpen)
        {
            GlobalEventManager.DisableEvent(inputMappingID);
        }

        isOpen = true;

        sourceUIPanel = source;

        if (sourceUIPanel != null)
        {
            sourceUIPanel.OnCloseEvent += Close;
        }

        OnOpen();

        OnOpenEvent?.Invoke();

        1.DelayFrameAction(() =>
        {
            if (isOpen)
            {
                OnLayoutChange();

                OnCurrentLanguageChanged(GameCoreSettingBase.currentLanguage, GameCoreSettingBase.currentLanguage);

                GameCoreSettingBase.translationGeneralSetting.OnCurrentLanguageChanged += OnCurrentLanguageChanged;

                OnPostLayoutChange();
            }
        });

        //SetEnabled(false);
    }

    [Button("关闭")]
    public void Close()
    {
        if (isDebugging)
        {
            Note.note.Warning($"{name}关闭");
        }

        GameCoreSettingBase.translationGeneralSetting.OnCurrentLanguageChanged -= OnCurrentLanguageChanged;

        isOpen = false;

        OnClose();

        OnCloseEvent?.Invoke();

        if (sourceUIPanel != null)
        {
            sourceUIPanel.OnCloseEvent -= Close;
        }

        sourceUIPanel = null;

        foreach (var inputMappingID in preset.globalEventDisabledListOnOpen)
        {
            GlobalEventManager.EnableEvent(inputMappingID);
        }
    }

    [Button("切换开关")]
    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void SetEnabled(bool enableState)
    {
        if (isDebugging)
        {
            Note.note.Warning($"{name}激活状态：{enableState}");
        }

        this.uiEnabled = enableState;

        OnSetEnabled();
    }

    [Button(nameof(Crash))]
    public void Crash()
    {
        1.DelayFrameAction(OnCrash);
    }

    #endregion

    #region Visual Element



    #endregion
}
