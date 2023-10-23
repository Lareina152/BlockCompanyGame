using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UGUIPanelPreset : UIPanelPreset
{
    protected const string UGUI_PANEL_CATEGORY = "UGUI面板设置";

    public override Type controllerType => typeof(UGUIPanelController);

    [LabelText("UI预制体"), FoldoutGroup(UGUI_PANEL_CATEGORY)]
    [AssetsOnly]
    [Required]
    public GameObject prefab;

    public override void CheckSettings()
    {
        base.CheckSettings();

        Note.note.AssertIsNotNull(prefab, nameof(prefab));
    }

    protected IEnumerable GetPrefabChildrenNames()
    {
        return prefab.transform.GetAllChildrenNames(true);
    }

    protected IEnumerable GetPrefabChildrenNamesOfTextMeshProUGUI()
    {
        return prefab.transform.GetAllChildrenNames<TextMeshProUGUI>(true);
    }
}
