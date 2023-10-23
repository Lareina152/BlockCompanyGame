using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;
using Newtonsoft.Json;

namespace Basis.GameItem
{
    public class
        VisualGameItemBundle<TPrefab, TGeneralSetting, TInstance> : SimpleGameItemBundle<TPrefab, TGeneralSetting,
            TInstance>
        where TPrefab : VisualGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItemPrefab, new()
        where TInstance : VisualGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItem, new()
        where TGeneralSetting : VisualGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItemGeneralSetting

    {
        private static Dictionary<Type, List<PropertyConfig>> propertyConfigsDict = new();

        public new abstract class GameItem : SimpleGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItem,
            ITracingTooltipProvider
        {
            #region Tooltip

            public bool TryGetTooltipBindGlobalEvent(out GlobalEventConfig globalEvent)
            {
                globalEvent = generalSetting.tooltipBindGlobalEvent;

                return globalEvent != null;
            }

            public string GetTooltipID()
            {
                return generalSetting.tooltipID;
            }

            public virtual string GetTooltipTitle()
            {
                return origin.nameFormat.GetText(name);
            }

            public virtual IEnumerable<(string attributeName, Func<string> attributeValueGetter, bool isStatic)>
                GetTooltipProperties()
            {
                foreach (var tooltipPropertyConfig in origin.GetTooltipPropertyConfigs())
                {
                    yield return (tooltipPropertyConfig.propertyConfig.name,
                        () => tooltipPropertyConfig.propertyConfig.GetValueString(this),
                        tooltipPropertyConfig.isStatic);
                }
            }

            public virtual string GetTooltipDescription()
            {
                if (origin.hasDescription)
                {
                    return origin.descriptionFormat.GetText(origin.description);
                }

                return string.Empty;
            }

            #endregion

            #region Property

            [ShowInInspector] private Dictionary<string, PropertyOfGameItem> properties;

            private void GenerateProperties()
            {
                properties = new();

                var propertyConfigs = GetPropertyConfigs();

                foreach (var propertyConfig in propertyConfigs)
                {
                    var property = propertyConfig.CreateInstance();

                    property.target = this;

                    properties.Add(property.id, property);
                }
            }

            [Button("获得属性")]
            public IEnumerable<PropertyOfGameItem> GetAllProperties()
            {
                if (properties == null)
                {
                    GenerateProperties();
                }

                return properties.Values;
            }

            public PropertyOfGameItem GetProperty(string propertyID)
            {
                if (properties == null)
                {
                    GenerateProperties();
                }

                if (properties.TryGetValue(propertyID, out var property))
                {
                    return property;
                }

                return null;
            }

            private IReadOnlyList<PropertyConfig> GetPropertyConfigs()
            {
                if (propertyConfigsDict.TryGetValue(GetType(), out var propertyConfigs))
                {
                    return propertyConfigs;
                }

                propertyConfigs = PropertyGeneralSetting.GetPropertyConfigs(GetType());

                propertyConfigsDict[GetType()] = propertyConfigs;

                return propertyConfigs;
            }

            #endregion
        }

        public new abstract class
            GameItemPrefab : SimpleGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItemPrefab
        {
            [LabelText("名称格式覆盖", SdfIconType.Bootstrap), FoldoutGroup(BASIC_SETTING_CATEGORY)] [JsonProperty]
            public TextTagFormat nameFormat = new();

            [LabelText("是否有描述"), FoldoutGroup(BASIC_SETTING_CATEGORY)] [JsonProperty]
            public bool hasDescription = false;

            [LabelText("描述", SdfIconType.BlockquoteLeft), FoldoutGroup(BASIC_SETTING_CATEGORY)]
            [Indent]
            [ShowIf(nameof(hasDescription))]
            [JsonProperty]
            public StringTranslation description = "无描述";

            [LabelText("描述格式覆盖", SdfIconType.Bootstrap), FoldoutGroup(BASIC_SETTING_CATEGORY)]
            [Indent]
            [ShowIf(nameof(hasDescription))]
            [JsonProperty]
            public TextTagFormat descriptionFormat = new();

            [LabelText("自定义提示框显示的属性"), FoldoutGroup(BASIC_SETTING_CATEGORY)] [SerializeField, JsonProperty]
            private bool customTooltipProperties = false;

            [LabelText("自定义模式"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
            [Indent]
            [ShowIf(nameof(customTooltipProperties))]
            [SerializeField, JsonProperty]
            private CustomTooltipPropertiesMode customTooltipPropertiesMode;

            [LabelText("提示框显示的属性"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
            [Indent]
            [ShowIf(nameof(customTooltipProperties))]
            [ListDrawerSettings(CustomAddFunction = nameof(AddTooltipPropertyConfigsGUI))]
            [SerializeField, JsonProperty]
            private List<TooltipPropertyConfig> tooltipPropertyConfigs = new();

            [ShowInInspector] [HideInEditorMode]
            private List<TooltipPropertyConfigRuntime> tooltipPropertyConfigsRuntime;

            #region GUI

            protected override void OnInspectorInit()
            {
                base.OnInspectorInit();

                nameFormat ??= new();
                description ??= "无描述";
                descriptionFormat ??= new();

                tooltipPropertyConfigs ??= new();

                foreach (var tooltipPropertyConfig in tooltipPropertyConfigs)
                {
                    tooltipPropertyConfig.filterType = actualInstanceType;
                }
            }

            private TooltipPropertyConfig AddTooltipPropertyConfigsGUI()
            {
                return new TooltipPropertyConfig()
                {
                    filterType = actualInstanceType
                };
            }

            #endregion

            public override void CheckSettings()
            {
                base.CheckSettings();

                if (customTooltipProperties)
                {
                    foreach (var tooltipPropertyConfig in tooltipPropertyConfigs)
                    {
                        Note.note.AssertIsNotNullOrEmpty(tooltipPropertyConfig.propertyID,
                            $"{nameof(tooltipPropertyConfig)}.{nameof(tooltipPropertyConfig.propertyID)}");
                    }

                    if (tooltipPropertyConfigs.ContainsSame(config => config.propertyID))
                    {
                        Note.note.Warning($"{id}的提示框属性设置有重复的属性ID");
                    }
                }
            }

            protected override void OnInit()
            {
                base.OnInit();

                if (customTooltipProperties)
                {
                    tooltipPropertyConfigsRuntime = new();

                    if (customTooltipPropertiesMode == CustomTooltipPropertiesMode.Incremental)
                    {
                        var defaultConfigs = GetGeneralSetting().GetDefaultTooltipPropertyConfigsConfigs();

                        if (defaultConfigs == null)
                        {
                            Note.note.Error($"无法获取{GetGeneralSetting()}的默认提示框属性设置");
                        }
                        else
                        {
                            tooltipPropertyConfigsRuntime.AddRange(defaultConfigs);
                        }
                    }

                    foreach (var config in tooltipPropertyConfigs)
                    {
                        tooltipPropertyConfigsRuntime.Add(config.ConvertToRuntime());
                    }
                }
            }

            public IReadOnlyList<TooltipPropertyConfigRuntime> GetTooltipPropertyConfigs()
            {
                if (customTooltipProperties)
                {
                    return tooltipPropertyConfigsRuntime;
                }

                return GetGeneralSetting().GetDefaultTooltipPropertyConfigsConfigs();
            }
        }

        public new abstract class
            GameItemGeneralSetting : SimpleGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItemGeneralSetting
        {
            public const string TOOLTIP_SETTING_CATEGORY = "提示框设置";

            public const string PROPERTY_SETTING_CATEGORY = "属性设置";

            [LabelText("提示框"), TitleGroup(TOOLTIP_SETTING_CATEGORY)]
            [ValueDropdown(nameof(GetTooltipIDList))]
            [StringIsNotNullOrEmpty]
            [JsonProperty]
            public string tooltipID;

            [LabelText("提示框绑定的全局事件"), TitleGroup(TOOLTIP_SETTING_CATEGORY)]
            [ValueDropdown(nameof(GetGlobalEventNameList))]
            [JsonProperty, SerializeField]
            private string tooltipBindGlobalEventID;

            public GlobalEventConfig tooltipBindGlobalEvent { get; private set; }

            [LabelText("默认提示框显示的属性"), TitleGroup(TOOLTIP_SETTING_CATEGORY)] [SerializeField, JsonProperty]
            private List<TooltipPropertyConfig> defaultTooltipPropertyConfigs = new();

            [TitleGroup(TOOLTIP_SETTING_CATEGORY)] [ShowInInspector] [HideInEditorMode]
            private List<TooltipPropertyConfigRuntime> defaultTooltipPropertyConfigsRuntime;

            [LabelText("属性类别"), TitleGroup(PROPERTY_SETTING_CATEGORY)] [ValueDropdown(nameof(GetPropertyTypeList))]
            public string propertyTypeID;

            [LabelText("所有类对应的属性设置"), TitleGroup(PROPERTY_SETTING_CATEGORY)]
            [ShowInInspector]
            private static Dictionary<Type, List<PropertyConfig>> propertyConfigsDict =>
                VisualGameItemBundle<TPrefab, TGeneralSetting, TInstance>.propertyConfigsDict;

            #region GUI

            private IEnumerable GetTooltipIDList()
            {
                return GameCoreSettingBase.uiPanelGeneralSetting.GetPrefabNameList(typeof(TracingTooltipPreset));
            }

            private IEnumerable GetGlobalEventNameList()
            {
                return GameCoreSettingBase.globalEventSystemGeneralSetting.GetPrefabNameList();
            }

            private IEnumerable GetPropertyTypeList()
            {
                return GameCoreSettingBase.propertyGeneralSetting.GetAllTypeNameList();
            }

            #endregion

            public override void CheckSettings()
            {
                base.CheckSettings();

                foreach (var tooltipPropertyConfig in defaultTooltipPropertyConfigs)
                {
                    Note.note.AssertIsNotNullOrEmpty(tooltipPropertyConfig.propertyID,
                        $"{nameof(tooltipPropertyConfig)}.{nameof(tooltipPropertyConfig.propertyID)}");
                }

                if (defaultTooltipPropertyConfigs.ContainsSame(config => config.propertyID))
                {
                    Note.note.Warning("默认的提示框属性设置有重复的属性ID");
                }
            }

            protected override void OnPreInit()
            {
                base.OnPreInit();

                defaultTooltipPropertyConfigsRuntime = new();

                foreach (var propertyConfig in defaultTooltipPropertyConfigs)
                {
                    defaultTooltipPropertyConfigsRuntime.Add(propertyConfig.ConvertToRuntime());
                }
            }

            protected override void OnPostInit()
            {
                base.OnPostInit();

                if (tooltipBindGlobalEventID.IsNullOrEmpty() == false)
                {
                    tooltipBindGlobalEvent =
                        GameCoreSettingBase.globalEventSystemGeneralSetting.GetPrefabStrictly(tooltipBindGlobalEventID);
                }
            }

            public IReadOnlyList<TooltipPropertyConfigRuntime> GetDefaultTooltipPropertyConfigsConfigs()
            {
                return defaultTooltipPropertyConfigsRuntime;
            }
        }

        #region Tooltip

        public enum CustomTooltipPropertiesMode
        {
            [LabelText("覆盖")] Override,
            [LabelText("增量")] Incremental
        }

        public class TooltipPropertyConfig : BaseConfigClass
        {
            [HideInInspector] public Type filterType;

            [LabelText("属性")] [ValueDropdown(nameof(GetPropertyNameList))] [StringIsNotNullOrEmpty]
            public string propertyID;

            [LabelText("是否静态")] public bool isStatic = true;

            #region GUI

            protected override void OnInspectorInit()
            {
                base.OnInspectorInit();

                filterType ??= typeof(TInstance);
            }

            private IEnumerable GetPropertyNameList()
            {
                return GameCoreSettingBase.propertyGeneralSetting.GetPropertyNameList(filterType);
            }

            #endregion

            public TooltipPropertyConfigRuntime ConvertToRuntime()
            {
                var configRuntime = new TooltipPropertyConfigRuntime();

                var propertyConfig = GameCoreSettingBase.propertyGeneralSetting.GetPrefabStrictly(propertyID);

                if (typeof(TInstance).IsAssignableFrom(propertyConfig.targetType) == false)
                {
                    Note.note.Error($"属性{propertyID}的目标类型{propertyConfig.targetType}与{typeof(TInstance)}不匹配");
                }

                configRuntime.propertyConfig = propertyConfig;
                configRuntime.isStatic = isStatic;

                return configRuntime;
            }
        }

        public struct TooltipPropertyConfigRuntime
        {
            public PropertyConfig propertyConfig;
            public bool isStatic;
        }

        #endregion
    }
}
