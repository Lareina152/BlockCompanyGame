#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Basis;
using ConfigurationBasis;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "TextureImportTools", menuName = "ToolsConfiguration/TextureImportTools")]
public class TextureImportTools : GeneralSettingBase
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "图片导入设置" },
        { "English", "TextureImportConfigurations" }
    };

    public override bool ignoreGeneralSettingsInGameEditor => true;

    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    public class TextureImportConfigurations
    {
        [LabelText("是否开启")]
        public bool isOn = true;

        [FolderPath]
        [LabelText("图片文件夹（用于过滤）")]
        public string textureFolder = "Assets/Resources/Images";

        [LabelText("忽略精灵导入类别"), FoldoutGroup("精灵")]
        public bool ignoreSpriteImportMode = true;
        [LabelText("精灵导入类别"), HideIf(nameof(ignoreSpriteImportMode)), FoldoutGroup("精灵")]
        public SpriteImportMode spriteImportMode = SpriteImportMode.Single;
        [LabelText("精灵中心点"), FoldoutGroup("精灵")]
        public Vector2Setter spritePivot = new Vector2(0.5f, 0.5f);

        [LabelText("过滤模式")]
        public FilterMode filterMode = FilterMode.Point;
        [LabelText("是否可读/写")]
        public bool isReadable = true;

        [LabelText("图片格式")]
        public TextureImporterFormat textureFormat = TextureImporterFormat.RGBA32;
        [LabelText("是否压缩")]
        public TextureImporterCompression compression = TextureImporterCompression.Uncompressed;
    }

    [ListDrawerSettings(CustomAddFunction = nameof(AddConfiguration))]
    [LabelText("配置")]
    public List<TextureImportConfigurations> configurations = new();

    #region GUI

    private TextureImportConfigurations AddConfiguration() => new TextureImportConfigurations();

    #endregion
}

public class TextureImportToolsPostProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        TextureImporter importer = assetImporter as TextureImporter;

        if (importer == null)
        {
            return;
        }

        if (GameCoreSettingBase.textureImportTools == null)
        {
            return;
        }

        foreach (var configuration in GameCoreSettingBase.textureImportTools.configurations)
        {
            if (configuration.isOn == false)
            {
                continue;
            }

            if (assetPath.StartsWith(configuration.textureFolder))
            {
                if (configuration.ignoreSpriteImportMode == false)
                {
                    importer.spriteImportMode = configuration.spriteImportMode;
                }
                
                importer.filterMode = configuration.filterMode;

                importer.spritePivot = configuration.spritePivot;
                importer.isReadable = configuration.isReadable;

                TextureImporterPlatformSettings settings = new()
                {
                    format = configuration.textureFormat,
                    textureCompression = configuration.compression
                };

                importer.SetPlatformTextureSettings(settings);

                importer.SaveAndReimport();

                break;
            }
        }

        

        //Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
        //if (asset)
        //{
        //    EditorUtility.SetDirty(asset);
        //}
    }
}
#endif