#if UNITY_EDITOR

using Basis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using NamedValue = Sirenix.OdinInspector.Editor.ActionResolvers.NamedValue;
using UnityEditor.SceneManagement;

namespace ItemBasis
{
    [PreviewComposite]
    [HideReferenceObjectPicker]
    public class CustomPrefabCreator
    {
        [LabelText(@"@displayName.GetTranslation(""中文"") + ""预制体""")]
        [AssetsOnly, InlineEditor(InlineEditorModes.FullEditor)]
        [HideIf("hidePrefabPreview")]
        public GameObject customPrefab;

        [HideInInspector]
        public StringTranslation displayName;
        [HideInInspector]
        public GameObject universalPrefab;
        [HideInInspector]
        public string path = "";
        [HideInInspector]
        public string name = "";
        [HideInInspector]
        public Action<bool, GameObject> afterGenerate = null;

        [HideInInspector]
        public bool hidePrefabPreview = false;



        [ShowInInspector]
        [DisplayAsString]
        [LabelText("名称预览")]
        public string namePreview => $"{name}_{displayName.GetTranslation("English")}_prefab.prefab";

        //public CustomPrefabCreator(GameObject universalPrefab, string path, string name, Action<bool, GameObject> afterGenerate = null)
        //{
        //    this.universalPrefab = universalPrefab;
        //    this.path = path;
        //    this.name = name;
        //    this.afterGenerate = afterGenerate;
        //}

        public GameObject GenerateCustomPrefabObject(GameObject prefab, string path, string name, Action<bool, GameObject> afterGenerate = default)
        {
            string prefabPath = Path.Combine(path, namePreview);

            FileInfo prefabFile = new FileInfo(prefabPath);

            if (prefabFile.Exists)
            {
                int choose = EditorUtility.DisplayDialogComplex("警告", "预制体已存在！", "覆盖原本的预制体", "取消创建", "不覆盖，创建新的预制体");

                //覆盖
                if (choose == 0)
                {

                }
                //不覆盖
                else if (choose == 2)
                {
                    prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                }
                //取消创建
                else
                {
                    return null;
                }
            }

            GameObject tempPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            bool success = false;
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(tempPrefab, prefabPath, out success);

            if (success == true)
            {
                EditorUtility.DisplayDialog("自定义预制体成功", "创建成功！", "OK");
                Note.note.Log($"{name}预制体创建成功！");

                customPrefab = newPrefab;
            }
            else
            {
                EditorUtility.DisplayDialog("自定义预制体失败", "创建失败！", "OK");
                Note.note.Warning($"{name}预制体创建失败!");
            }

            UnityEngine.Object.DestroyImmediate(tempPrefab);

            if (afterGenerate != default)
            {
                afterGenerate(success, newPrefab);
            }

            return newPrefab;
        }

        [Button(@"@""生成自定义"" + displayName +""预制体""", ButtonSizes.Large, Icon = SdfIconType.Box), GUIColor(0.4f, 0.8f, 1)]
        public void GenerateCustomPrefabObject()
        {
            GenerateCustomPrefabObject(universalPrefab, path, name, afterGenerate);
        }

        [Button(@"@""编辑"" + displayName +""预制体""", ButtonSizes.Large, Icon = SdfIconType.Collection), GUIColor(0.4f, 0.8f, 1)]
        [HideIf("@customPrefab == null")]
        public void EditCustomPrefabObject()
        {
            PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(customPrefab));
            if (SceneView.sceneViews.Count > 0)
            {
                ((SceneView)SceneView.sceneViews[0]).Focus();
            }
        }

        [Button(@"@""删除"" + displayName +""预制体""", ButtonSizes.Large, Icon = SdfIconType.DashCircleDotted), GUIColor("@Color.red")]
        [HideIf("@customPrefab == null")]
        public void DeleteCustomPrefabObject()
        {
            if (customPrefab == null)
            {
                return;
            }

            if (EditorUtility.DisplayDialog(
                            $"是否确认删除此{displayName}预制体?",
                            "谨慎选择，不可撤销",
                            "是",
                            "否"
                            )
                )
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(customPrefab));

                customPrefab = null;
            }
        }

    }


    public class CustomPrefabCreatorSettingAttribute : Attribute
    {
        public string DisplayName;
        public string UniversalPrefab;
        public string Path;
        public string Name;
        public string AfterGenerate;
        public bool HidePrefabPreview = false;

        public CustomPrefabCreatorSettingAttribute(string UniversalPrefab, string Path, string Name)
        {
            this.UniversalPrefab = UniversalPrefab;
            this.Path = Path;
            this.Name = Name;
        }
    }

    public class CustomPrefabCreatorSettingAttributeDrawer : OdinAttributeDrawer<CustomPrefabCreatorSettingAttribute>
    {
        private static readonly NamedValue[] ActionArgs = new NamedValue[2]
        {
            new NamedValue("isSuccess", typeof(bool)),
            new NamedValue("newPrefab", typeof(GameObject))
        };

        public ValueResolver<StringTranslation> displayName;
        public ValueResolver<GameObject> universalPrefab;
        public ValueResolver<string> path;
        public ValueResolver<string> name;
        public ActionResolver afterGenerate;

        public bool hidePrefabPreview;

        protected override void Initialize()
        {
            universalPrefab = ValueResolver.Get<GameObject>(Property, Attribute.UniversalPrefab);
            path = ValueResolver.GetForString(Property, Attribute.Path);
            name = ValueResolver.GetForString(Property, Attribute.Name);

            hidePrefabPreview = Attribute.HidePrefabPreview;

            if (Attribute.AfterGenerate != null && Attribute.AfterGenerate != "")
            {
                afterGenerate = ActionResolver.Get(Property, Attribute.AfterGenerate, ActionArgs);
            }

            if (Attribute.DisplayName != null && Attribute.DisplayName != "")
            {
                displayName = ValueResolver.Get<StringTranslation>(Property, Attribute.DisplayName);
            }

        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            ValueResolver.DrawErrors(universalPrefab, path, name, displayName);
            ActionResolver.DrawErrors(afterGenerate);

            ((CustomPrefabCreator)Property.ValueEntry.WeakSmartValue).path = path.GetValue();
            ((CustomPrefabCreator)Property.ValueEntry.WeakSmartValue).name = name.GetValue();
            ((CustomPrefabCreator)Property.ValueEntry.WeakSmartValue).universalPrefab = universalPrefab.GetValue();

            if (displayName != null)
            {
                ((CustomPrefabCreator)Property.ValueEntry.WeakSmartValue).displayName = displayName.GetValue();
            }

            if (afterGenerate != null)
            {
                ((CustomPrefabCreator)Property.ValueEntry.WeakSmartValue).afterGenerate = (isSuccess, newPrefab) =>
                {

                    afterGenerate.Context.NamedValues.Set("isSuccess", isSuccess);
                    afterGenerate.Context.NamedValues.Set("newPrefab", newPrefab);

                    afterGenerate.DoActionForAllSelectionIndices();
                };


            }

            ((CustomPrefabCreator)Property.ValueEntry.WeakSmartValue).hidePrefabPreview = hidePrefabPreview;

            CallNextDrawer(label);
        }
    }
}


#endif
