using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorGeneralSetting", menuName = "GameConfiguration/ColorGeneralSetting")]
public class ColorGeneralSetting : GeneralSettingBase
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "颜色" },
        { "English", "Color" }
    };

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.Pen;

    public override StringTranslation folderPath => coreCategory;

    [Serializable]
    public class ColorItem : BaseConfigClass
    {
        [LabelText("@name")]
        [JsonProperty]
        public Color color = new(0, 0, 0, 1);
        [HideLabel]
        [JsonProperty]
        public StringTranslation name = new();

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            name ??= new();
        }
    }

    [LabelText("颜色名称字典")]
    [Searchable]
    [JsonProperty]
    public List<ColorItem> colorNameDictionary = new();

    public string GetColorName(Color color)
    {
        var nearestColor = color.MinDistanceOne(colorNameDictionary, item => item.color);

        return nearestColor.name;
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        colorNameDictionary ??= new();
    }
}
