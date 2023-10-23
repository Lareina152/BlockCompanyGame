#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Linq;
using Basis;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;

public class GameEditor : OdinMenuEditorWindow
{
    private readonly AuxiliaryTools auxiliaryTools = new();
    private readonly NamingTools namingTools = new();
    private readonly PixelOutlineTools pixelOutlineTools = new();

    [MenuItem("Tools/游戏编辑器")]
    private static void OpenWindow()
    {
        GameCoreSettingBaseFile.CheckGlobal();

        var window = GetWindow<GameEditor>("游戏编辑器");
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        if (GameCoreSettingBase.gameCoreSettingsFileBase == null)
        {
            return new OdinMenuTree(supportsMultiSelect: true);
        }

        OdinMenuTree tree = new(supportsMultiSelect: true)
        {
            { "辅助工具", auxiliaryTools, EditorIcons.HamburgerMenu },
            { "辅助工具/命名工具", namingTools, EditorIcons.FileCabinet },
            { "辅助工具/图片导入工具", GameCoreSettingBase.textureImportTools},
            { "辅助工具/像素描边工具", pixelOutlineTools},
            { "辅助工具/待办清单", GameCoreSettingBase.toDoListTools },
            { "通用设置", GameCoreSettingBase.gameCoreSettingsFileBase, SdfIconType.GearFill },
            { $"通用设置/{GeneralSettingBase.coreCategory}", null, EditorIcons.StarPointer },
            { $"通用设置/{GeneralSettingBase.visualEffectCategory}", null, SdfIconType.Boxes},
            { $"通用设置/{GeneralSettingBase.builtInCategory}", null, SdfIconType.Inboxes},
            { "具体设置", null, SdfIconType.GearFill }
        };

        tree.DefaultMenuStyle.IconSize = 24.00f;
        tree.Config.DrawSearchToolbar = true;

        foreach (var generalSetting in GameCoreSettingBase.GetAllGeneralSettings())
        {
            if (generalSetting == null)
            {
                continue;
            }

            if (generalSetting.ignoreGeneralSettingsInGameEditor)
            {
                continue;
            }

            string folderPath = generalSetting.folderPath;
            folderPath = folderPath.Replace("\\", "/");
            folderPath = folderPath.Trim('/');

            string totalPath;

            if (folderPath.IsNullOrEmptyAfterTrim())
            {
                totalPath = $"通用设置/{generalSetting.fullSettingName}";
            }
            else
            {
                totalPath = $"通用设置/{folderPath}/{generalSetting.fullSettingName}";
            }

            if (generalSetting.hasIcon)
            {
                tree.Add(totalPath, generalSetting, generalSetting.icon);
            }
            else
            {
                tree.Add(totalPath, generalSetting);
            }
        }

        foreach (var generalSetting in GameCoreSettingBase.GetAllGeneralSettings())
        {
            if (generalSetting == null)
            {
                continue;
            }

            var willShowPrefabOnWindowSideName = "willShowPrefabOnWindowSide";

            if (generalSetting.GetType().HasFieldByName(willShowPrefabOnWindowSideName) == false)
            {
                continue;
            }

            var willShowPrefabOnWindowSide = generalSetting.GetFieldValueByName<bool>(willShowPrefabOnWindowSideName);

            if (willShowPrefabOnWindowSide == false)
            {
                continue;
            }

            string folderPath = generalSetting.folderPath;
            folderPath = folderPath.Replace("\\", "/");
            folderPath = folderPath.Trim('/');

            var itemName = generalSetting.GetPropertyValueByName<StringTranslation>("prefabName");
            var itemSuffixName = generalSetting.GetPropertyValueByName<StringTranslation>("prefabSuffixName");

            var prefabs = generalSetting.GetMethodValueByName<IEnumerable>("GetAllPrefabs");

            foreach (var prefab in prefabs)
            {
                string totalPath;

                var prefabName = prefab.GetFieldValueByName<StringTranslation>("name");

                if (folderPath.IsNullOrEmptyAfterTrim())
                {
                    totalPath = $"具体设置/{itemName}{itemSuffixName}/{prefabName}";
                }
                else
                {
                    totalPath = $"具体设置/{folderPath}/{itemName}{itemSuffixName}/{prefabName}";
                }

                tree.Add(totalPath, prefab);
            }
        }

        return tree;
    }

    protected virtual void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }

    protected override void OnBeginDrawEditors()
    {
        if (MenuTree == null)
        {
            return;
        }
        if (MenuTree.Selection == null)
        {
            return;
        }
        var selected = this.MenuTree.Selection.FirstOrDefault();
        var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected is { Value: { } })
            {
                GUILayout.Label(selected.Name);

                if (selected.Value is ScriptableObject scriptableObject)
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("在脚本里打开")))
                    {
                        scriptableObject.GetType().OpenScriptOfType();
                    }

                }



                //if (selected.Value.GetType().IsDerivedFrom(typeof(GamePrefabCoreBundle<>.GameItemGeneralSetting),
                //        true, false, true))
                //{
                //    if (SirenixEditorGUI.ToolbarButton(new GUIContent("改变类型")))
                //    {
                //        selected.Value.InvokeMethod("ChangeType");
                //    }
                //}
            }

        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}


[Serializable]
public class AuxiliaryTools
{
    [PropertySpace(SpaceBefore = 50)]
    [Button("打开EditorIcons概览", ButtonSizes.Large)]
    private void OpenEditorIconsOverview()
    {
        EditorIconsOverview.OpenEditorIconsOverview();
    }

    [Button("重绘GUI", ButtonSizes.Large)]
    private void RequestRepaint()
    {
        GUIHelper.RequestRepaint();
    }

    [Button("打开游戏编辑器脚本", ButtonSizes.Large)]
    private void OpenGameEditorScript()
    {
        typeof(GameEditor).OpenScriptOfType();
    }
}
#endif