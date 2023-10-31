using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConfigurationBasis;
using UnityEditor;
using UnityEngine;

namespace Basis
{

    [Serializable]
    [HideReferenceObjectPicker]
    [InfoBox("翻译语言种类未补全", InfoMessageType.Warning, "@!$value.ContainsAllLanguage()")]
    [InfoBox("语言重复", InfoMessageType.Warning, "@$value.ContainsSameLanguage()")]
    [InfoBox("存在空的翻译文本", InfoMessageType.Warning, "@$value.ContainsEmptyTranslation()")]
    [InfoBox("请至少添加一个翻译", InfoMessageType.Error, "@$value != null && $value.allTranslations != null && $value.allTranslations.Count == 0")]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class StringTranslation : ValuePreviewBase<string>, IEnumerable<StringTranslation.LanguageInfo>, ICloneable
    {
        [Serializable]
        [HideReferenceObjectPicker]
        [JsonObject(MemberSerialization.OptIn)]
        public class LanguageInfo : ICloneable
        {
            [GUIColor(0.3f, 0.8f, 0.8f)]
            [ValueDropdown("@GameSetting.translationGeneralSetting.GetPrefabIDList()"), HideLabel]
            [InfoBox("该语言不存在", InfoMessageType.Error,
                VisibleIf = "@!GameSetting.translationGeneralSetting.ContainsPrefabID(languageType)")]
            [JsonProperty]
            public string languageType;
            [HideLabel, MultiLineProperty(3)]
            [InfoBox("该翻译不能为空", InfoMessageType.Warning, @"@translation == null || translation.Trim() == """"")]
            [JsonProperty]
            public string translation;

            public LanguageInfo(string languageType = "", string translation = "")
            {
                this.languageType = languageType;
                this.translation = translation;
            }

            public object Clone()
            {
                return new LanguageInfo(languageType, translation);
            }
        }

        public const string ANY_LANGUAGE = "Any";

        protected override bool valueAlwaysPreviewed => true;
        protected override bool showPreviewValueBelow => false;

        [SerializeField]
        [ListDrawerSettings(
            CustomAddFunction = "AddLanguageInfoButton",
            ShowFoldout = false
            )]
        [LabelText("所有翻译")]
        [OnInspectorInit(nameof(OnInspectorInit))]
        [OnValueChanged(nameof(OnAllTranslationsChanged))]
        [JsonProperty]
        public List<LanguageInfo> allTranslations = new();

        [NonSerialized]
        public string defaultTranslation = "";

        #region GUI

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            if (allTranslations == null)
            {
                allTranslations = new List<LanguageInfo>();

                foreach (string languageType in GameCoreSettingBase.translationGeneralSetting.GetPrefabIDList())
                {
                    if (languageType != ANY_LANGUAGE)
                    {
                        allTranslations.Add(new LanguageInfo(languageType));
                    }
                }
            }
        }

        protected override void PreviewValue()
        {
            base.PreviewValue();

            contentPreviewing = this;
        }

        private void OnAllTranslationsChanged()
        {
            allTranslations.RemoveAllNull();
        }

        private LanguageInfo AddLanguageInfoButton()
        {
            foreach (string languageType in GameCoreSettingBase.translationGeneralSetting.GetPrefabIDList())
            {
                if (languageType != ANY_LANGUAGE && !Contains(languageType))
                {
                    return new LanguageInfo(languageType);
                }
            }

#if UNITY_EDITOR
            EditorUtility.DisplayDialog("提示", "语言种类已齐全", "OK");
#endif

            return null;
        }



        [Button("补全翻译")]
        [ShowIf(@"@ContainsAllLanguage() == false")]
        private void AddMissingLanguageType()
        {
            foreach (var languageType in GameCoreSettingBase.translationGeneralSetting.GetPrefabIDList())
            {
                if (languageType == ANY_LANGUAGE)
                {
                    continue;
                }
                if (Contains(languageType) == false)
                {
                    Add(languageType, "");
                }
            }
        }

        #endregion


        public bool IsEmpty()
        {
            return GetTranslation(GameCoreSettingBase.currentLanguage).IsEmptyAfterTrim();
        }

        /// <summary>
        /// 是否包含指定语言的翻译
        /// </summary>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public bool Contains(string languageType)
        {
            return allTranslations.Any(info => info.languageType == languageType);
        }

        public bool ContainsAllLanguage()
        {
            if (Contains(ANY_LANGUAGE))
            {
                return true;
            }

            return GameCoreSettingBase.translationGeneralSetting.GetPrefabIDList().
                All(languageType => languageType == ANY_LANGUAGE || Contains(languageType));
        }

        public bool ContainsSameLanguage()
        {
            return allTranslations != null && allTranslations.ContainsSame(info => info.languageType);
        }

        public bool ContainsEmptyTranslation()
        {
            return allTranslations != null && allTranslations.Any(info => info.translation.IsNullOrEmptyAfterTrim());
        }

        /// <summary>
        /// 获取指定语言类型的翻译
        /// </summary>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public string GetTranslation(string languageType)
        {
            if (allTranslations == null)
            {
                return "";
            }

            foreach (var info in allTranslations.
                         Where(info => info.languageType == languageType || languageType == ANY_LANGUAGE))
            {
                return info.translation;
            }

            foreach (var info in allTranslations.Where(info => info.languageType == ANY_LANGUAGE))
            {
                return info.translation;
            }

            Note.note.Warning($"没有找到对应语言:{languageType}的翻译");

            return defaultTranslation;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<LanguageInfo> GetEnumerator()
        {
            return allTranslations.GetEnumerator();
        }

        /// <summary>
        /// 移除指定语言的翻译
        /// </summary>
        /// <param name="languageType"></param>
        public void Remove(string languageType)
        {
            allTranslations.Remove(item => item.languageType == languageType);
        }

        /// <summary>
        /// 添加新翻译
        /// </summary>
        /// <param name="languageType"></param>
        /// <param name="translation"></param>
        public void Add(string languageType, string translation)
        {
            Remove(languageType);
            allTranslations.Add(new LanguageInfo(languageType, translation));
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <returns></returns>
        public StringTranslation GetClone()
        {
            return Clone() as StringTranslation;
        }

        #region Conversion

        public static implicit operator StringTranslation(string content)
        {
            return new()
            {
                { ANY_LANGUAGE, content }
            };
        }

        public static implicit operator string(StringTranslation stringTranslation)
        {
            return stringTranslation == null ? "" : stringTranslation.GetTranslation(GameCoreSettingBase.currentLanguage);
        }

        public override string ToString()
        {
            return this;
        }

        public object Clone()
        {
            var newInstance = new StringTranslation
            {
                allTranslations = allTranslations.Clone(),
                defaultTranslation = defaultTranslation
            };
            return newInstance;
        }

        #endregion


    }

    
}

