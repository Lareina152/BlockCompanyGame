using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using UnityEngine;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class UIPanelManager : UniqueMonoBehaviour<UIPanelManager>
{
    protected static UIPanelGeneralSetting setting => GameCoreSettingBase.uiPanelGeneralSetting;

    [ShowInInspector]
    private static Dictionary<string, List<UIPanelController>> allUIPanelControllers = new();

    [ShowInInspector]
    private static Dictionary<string, HashSet<UIPanelController>> allClosedUIPanels = new();

    [ShowInInspector]
    private static Dictionary<string, UIPanelController> allUniquePanelControllers = new();

    protected override void Awake()
    {
        base.Awake();
        
        ProcedureManager.AddOnEnterEvent(GameInitializationProcedure.registeredID, Init);
    }

    public static void Init()
    {
        InitCanvas();

        foreach (var uiPanelPreset in UIPanelPreset.GetAllPrefabs())
        {
            if (uiPanelPreset.isUnique)
            {
                CreatePanel(uiPanelPreset.id);
            }
        }
    }

    [Button("重建面板")]
    public static UIPanelController RecreateUniquePanel(
        [ValueDropdown("@GameSetting.uiPanelGeneralSetting.GetPrefabNameList()")] string presetID)
    {
        if (allUniquePanelControllers.TryGetValue(presetID, out var panelCtrl))
        {
            if (panelCtrl.preset.isUnique == false)
            {
                Note.note.Error($"{panelCtrl.preset}不是Unique的UIPanel");
            }

            var newPanel = CreatePanel(presetID);

            panelCtrl.OnRecreate(newPanel);

            Destroy(panelCtrl.gameObject);

            return newPanel;
        }

        return null;
    }

    [Button("创建面板")]
    public static UIPanelController CreatePanel(
        [ValueDropdown("@GameSetting.uiPanelGeneralSetting.GetPrefabNameList()")]
        string presetID)
    {
        var preset = GameCoreSettingBase.uiPanelGeneralSetting.GetPrefabStrictly(presetID);

        var newUIPanel = new GameObject(preset.name).AddComponent(preset.controllerType) as UIPanelController;

        if (newUIPanel == null)
        {
            Note.note.Error($"添加组件{nameof(UIPanelController)}失败，预设ID：{presetID}");
            return null;
        }

        newUIPanel.transform.SetParent(setting.container);

        if (allUIPanelControllers.ContainsKey(presetID) == false)
        {
            allUIPanelControllers[presetID] = new();
        }

        allUIPanelControllers[presetID].Add(newUIPanel);

        if (preset.isUnique)
        {
            if (allUniquePanelControllers.ContainsKey(presetID))
            {
                Note.note.Warning($"ID为{presetID}的唯一UI面板已经创建，旧的面板将被覆盖");
            }
            allUniquePanelControllers[presetID] = newUIPanel;
        }

        newUIPanel.Init(preset);

        newUIPanel.Close();
        
        if (preset.autoOpenOnCreation)
        {
            newUIPanel.Open();
        }

        if (preset.isUnique == false)
        {
            newUIPanel.OnCloseEvent += () => AddClosedUIPanel(newUIPanel);
            newUIPanel.OnOpenEvent += () => RemoveClosedUIPanel(newUIPanel);
        }

        return newUIPanel;
    }

    [Button("获取或创建面板")]
    public static UIPanelController GetOrCreatePanel(
        [ValueDropdown("@GameSetting.uiPanelGeneralSetting.GetPrefabNameList()")]
        string presetID)
    {
        if (allUniquePanelControllers.TryGetValue(presetID, out var result))
        {
            return result;
        }

        if (allClosedUIPanels.TryGetValue(presetID, out var pool))
        {
            if (pool.Count > 0)
            {
                return pool.First();
            }
            
            return CreatePanel(presetID);
        }

        allClosedUIPanels[presetID] = new();
        return CreatePanel(presetID);
    }

    [Button("获取或创建面板并打开")]
    private static void GetOrCreatePanelAndOpen(
        [ValueDropdown("@GameSetting.uiPanelGeneralSetting.GetPrefabNameList()")]
        string presetID)
    {
        var newPanel = GetOrCreatePanel(presetID);

        newPanel.Open();
    }

    public static T GetOrCreatePanel<T>(string presetID) where T : UIPanelController
    {
        return GetOrCreatePanel(presetID) as T;
    }

    private static void RemoveClosedUIPanel(UIPanelController panelController)
    {
        if (allClosedUIPanels.TryGetValue(panelController.preset.id, out var pool))
        {
            pool.Remove(panelController);
        }
    }

    private static void AddClosedUIPanel(UIPanelController panelController)
    {
        if (panelController.preset.isUnique)
        {
            return;
        }

        var id = panelController.preset.id;

        if (allClosedUIPanels.TryGetValue(id, out var pool))
        {
            pool.Add(panelController);
            return;
        }

        allClosedUIPanels[id] = new HashSet<UIPanelController> { panelController };
    }

    #region Unique Panel

    public static UIPanelController GetUniqueUIPanel(string id)
    {
        if (allUniquePanelControllers.TryGetValue(id, out var result))
        {
            return result;
        }

        Note.note.Warning($"ID为{id}的Unique UI Panel不存在");

        return null;
    }

    public static void OpenUniquePanel(string id)
    {
        var panel = GetUniqueUIPanel(id);

        if (panel != null)
        {
            panel.Open();
        }
    }

    public static void CloseUniquePanel(string id)
    {
        var panel = GetUniqueUIPanel(id);

        if (panel != null)
        {
            panel.Close();
        }
    }

    #endregion

    #region Canvas

    [ShowInInspector]
    private static Transform canvasContainer;

    [ShowInInspector]
    private static Dictionary<int, Canvas> canvasDict = new();

    private static void InitCanvas()
    {
        canvasContainer = GameCoreSettingBase.uiPanelGeneralSetting.container;
    }

    public static Canvas GetCanvas(int sortingOrder)
    {
        if (canvasDict.ContainsKey(sortingOrder) == false)
        {
            var (canvas, canvasScaler, graphicRaycaster) = canvasContainer.CreateCanvas($"Canvas:{sortingOrder}");

            canvas.sortingOrder = sortingOrder;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = setting.defaultReferenceResolution;
            canvasScaler.matchWidthOrHeight = setting.defaultMatch;

            canvasDict[sortingOrder] = canvas;
        }
        
        return canvasDict[sortingOrder];
    }

    #endregion
}
