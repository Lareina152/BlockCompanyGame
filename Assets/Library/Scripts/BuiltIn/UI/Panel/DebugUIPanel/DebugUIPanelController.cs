using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugUIPanelController : UIToolkitPanelController
{
    [Serializable]
    public struct DebugEntryInfo
    {
        public VisualElement group;
        public Label title;
        public Label content;

        [ShowInInspector]
        private bool display => group.style.display.value == DisplayStyle.Flex;
    }

    private DebugUIPanelPreset debugUIPanelPreset { get; set; }

    [ShowInInspector]
    private VisualElement leftContainer;
    [ShowInInspector]
    private VisualElement rightContainer;

    [ShowInInspector]
    private VisualTreeAsset debugEntryPrototype;

    [ShowInInspector]
    private List<(DebugEntry debugEntry, DebugEntryInfo info)> debugEntryInfos = new();

    private float updateInterval = 0;

    private float currentTime = 0;

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > updateInterval)
        {
            currentTime = 0;

            foreach (var (debugEntry, info) in debugEntryInfos)
            {
                if (debugEntry.ShouldDisplay())
                {
                    info.group.style.display = DisplayStyle.Flex;
                    info.content.text = debugEntry.OnHandleContentUpdate();
                }
                else
                {
                    info.group.style.display = DisplayStyle.None;
                }
            }
        }
    }

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);

        debugUIPanelPreset = preset as DebugUIPanelPreset;

        Note.note.AssertIsNotNull(debugUIPanelPreset, nameof(debugUIPanelPreset));

        updateInterval = GameCoreSettingBase.debugUIPanelGeneralSetting.updateInterval;
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        leftContainer = rootVisualElement.Q(debugUIPanelPreset.leftContainerVisualElementName);
        rightContainer = rootVisualElement.Q(debugUIPanelPreset.rightContainerVisualElementName);

        if (leftContainer == null)
        {
            Note.note.Warning($"{nameof(debugUIPanelPreset.leftContainerVisualElementName)}无效，找不到指定的VisualElement");
        }
        if (rightContainer == null)
        {
            Note.note.Warning($"{nameof(debugUIPanelPreset.rightContainerVisualElementName)}无效，找不到指定的VisualElement");
        }

        debugEntryPrototype = debugUIPanelPreset.debugEntryVisualTree;

        debugEntryInfos.Clear();

        foreach (var debugEntry in DebugEntry.GetAllPrefabs())
        {
            AddEntry(debugEntry);
        }
    }

    private void AddEntry(DebugEntry debugEntry)
    {
        var container = debugEntry.position switch
        {
            DebugEntryPosition.Left => leftContainer,
            DebugEntryPosition.Right => rightContainer,
            _ => throw new ArgumentOutOfRangeException()
        };

        var debugEntryVisualElement = debugEntryPrototype.CloneTree().contentContainer;

        var titleVisualElement = 
            debugEntryVisualElement.Query<Label>(debugUIPanelPreset.entryTitleVisualElementName).First();
        var contentVisualElement =
            debugEntryVisualElement.Query<Label>(debugUIPanelPreset.entryContentVisualElementName).First();

        if (titleVisualElement == null)
        {
            Note.note.Error($"{nameof(debugUIPanelPreset.entryTitleVisualElementName)}无效，找不到指定的VisualElement");
            return;
        }

        if (contentVisualElement == null)
        {
            Note.note.Error($"{nameof(debugUIPanelPreset.entryContentVisualElementName)}无效，找不到指定的VisualElement");
            return;
        }

        titleVisualElement.text = debugEntry.name + ":";
        contentVisualElement.text = "";

        debugEntry.titleFormat.Set(titleVisualElement);
        debugEntry.contentFormat.Set(contentVisualElement);

        AddVisualElement(container, debugEntryVisualElement);

        debugEntryInfos.Add((debugEntry, new DebugEntryInfo()
        {
            group = debugEntryVisualElement,
            title = titleVisualElement,
            content = contentVisualElement,
        }));
    }

    #region Test

    [Button(nameof(AddEntry))]
    private void AddEntry(
        [ValueDropdown("@GameSetting.debugUIPanelGeneralSetting.GetPrefabNameList()")]
        string debugEntryID)
    {
        AddEntry(DebugEntry.GetPrefab(debugEntryID));
    }

    #endregion
}
