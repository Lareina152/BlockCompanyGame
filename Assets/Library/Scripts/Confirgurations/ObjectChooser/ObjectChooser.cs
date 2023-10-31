using Basis;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using NamedValue = Sirenix.OdinInspector.Editor.ActionResolvers.NamedValue;

#endif


namespace ConfigurationBasis
{
    #region Attribute

    [Conditional("UNITY_EDITOR")]
    public class ObjectChooserDrawerSettingsAttribute : Attribute
    {
        public string AfterChange;
    }

#if UNITY_EDITOR

    public class ObjectChooserDrawerSettingsAttributeDrawer : OdinAttributeDrawer<ObjectChooserDrawerSettingsAttribute>
    {
        private static readonly NamedValue[] ActionArgs = new NamedValue[1]
        {
            new NamedValue("info", typeof(CollectionChangeInfo))
        };

        private ActionResolver onAfter;

        private ICollectionResolver resolver;

        protected override void Initialize()
        {
            base.Initialize();

            foreach (var propertyChild in Property.Children)
            {
                if (propertyChild.Name == "valueProbabilities")
                {
                    resolver = (ICollectionResolver)propertyChild.ChildResolver;
                }
            }

            if (Attribute.AfterChange != null || Attribute.AfterChange != "")
            {
                onAfter = ActionResolver.Get(Property, Attribute.AfterChange, ActionArgs);
                if (!onAfter.HasError && resolver != null)
                {
                    resolver.OnAfterChange += OnAfterChange;
                }
            }

            //if ((onAfter == null || !onAfter.HasError))
            //{
            //    SkipWhenDrawing = true;
            //}
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (!string.IsNullOrEmpty(Attribute.AfterChange))
            {
                ActionResolver.DrawErrors(onAfter);
            }

            CallNextDrawer(label);
        }

        private void OnAfterChange(CollectionChangeInfo info)
        {
            onAfter.Context.NamedValues.Set("info", info);
            onAfter.DoAction(info.SelectionIndex);
        }

        public void Dispose()
        {
            if (onAfter != null)
            {
                resolver.OnAfterChange -= OnAfterChange;
            }
        }
    }

#endif

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class DisableObjectChooserTypeAttribute : Attribute
    {
        public string TypeID;

        public DisableObjectChooserTypeAttribute(string typeID)
        {
            TypeID = typeID;
        }
    }

#if UNITY_EDITOR

    public class DisableObjectChooserTypeUniversalDrawer<TChooser> : 
        OdinAttributeDrawer<DisableObjectChooserTypeAttribute, TChooser>
    {
        private ValueResolver<string> typeIDGetter;

        public override bool CanDrawTypeFilter(Type type) => 
            type.HasFieldByReturnType(typeof(ObjectChooser<>));

        protected override void Initialize()
        {
            typeIDGetter = ValueResolver.Get(Property, Attribute.TypeID, Attribute.TypeID);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (typeIDGetter.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(typeIDGetter.ErrorMessage);
                CallNextDrawer(label);
                return;
            }

            string typeID = typeIDGetter.GetValue();

            var previewer = ValueEntry.SmartValue;

            foreach (var fieldInfo in typeof(TChooser).GetFieldsByReturnType(typeof(ObjectChooser<>)))
            {
                var chooser = fieldInfo.GetValue(previewer);

                var randomTypes = chooser.GetMethodValueByName<IEnumerable<string>>("GetRandomTypes");
                var fixedTypes = chooser.GetMethodValueByName<IEnumerable<string>>("GetFixedTypes");

                if (randomTypes.Contains(typeID) == false && fixedTypes.Contains(typeID) == false)
                {
                    SirenixEditorGUI.ErrorMessageBox($"Attribute Error:不存在此种类{typeID}");
                    CallNextDrawer(label);
                    return;
                }

                var isRandomValue = chooser.GetFieldValueByName<bool>("isRandomValue");
                var randomType = chooser.GetFieldValueByName<string>("randomType");
                var fixedType = chooser.GetFieldValueByName<string>("fixedType");

                if (isRandomValue)
                {
                    if (randomType == typeID)
                    {
                        SirenixEditorGUI.ErrorMessageBox($"不允许使用此随机种类{typeID}");
                    }
                }
                else
                {
                    if (fixedType == typeID)
                    {
                        SirenixEditorGUI.ErrorMessageBox($"不允许使用此固定种类{typeID}");
                    }
                }
            }

            CallNextDrawer(label);
        }
    }

    public class DisableObjectChooserTypeDrawer<TChooser, T> : OdinAttributeDrawer<DisableObjectChooserTypeAttribute, TChooser>
        where TChooser : ObjectChooser<T>
    {
        private ValueResolver<string> typeIDGetter;

        public override bool CanDrawTypeFilter(Type type) =>
            type.IsDerivedFrom(typeof(ObjectChooser<T>), true);

        protected override void Initialize()
        {
            typeIDGetter = ValueResolver.Get(Property, Attribute.TypeID, Attribute.TypeID);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (typeIDGetter.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(typeIDGetter.ErrorMessage);
                CallNextDrawer(label);
                return;
            }

            string typeID = typeIDGetter.GetValue();

            var chooser = ValueEntry.SmartValue;

            if (chooser.GetRandomTypes().Contains(typeID) == false && chooser.GetFixedTypes().Contains(typeID) == false)
            {
                SirenixEditorGUI.ErrorMessageBox($"Attribute Error:不存在此种类{typeID}");
                CallNextDrawer(label);
                return;
            }

            if (chooser.isRandomValue)
            {
                if (chooser.randomType == typeID)
                {
                    SirenixEditorGUI.ErrorMessageBox($"不允许使用此随机种类{typeID}");
                }
            }
            else
            {
                if (chooser.fixedType == typeID)
                {
                    SirenixEditorGUI.ErrorMessageBox($"不允许使用此固定种类{typeID}");
                }
            }

            CallNextDrawer(label);
        }
    }


#endif

    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("UNITY_EDITOR")]
    public class RequiredObjectChooserAttribute : Attribute
    {
        public string ErrorMessage;

        public RequiredObjectChooserAttribute()
        {
            ErrorMessage = null;
        }

        public RequiredObjectChooserAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }

#if UNITY_EDITOR

    public class RequiredAttributeDrawer<TChooser, T> : OdinAttributeDrawer<RequiredObjectChooserAttribute, TChooser>
        where TChooser : ObjectChooser<T>
    {
        private ValueResolver<string> errorMessageGetter;

        public override bool CanDrawTypeFilter(Type type) =>
            type.IsDerivedFrom(typeof(ObjectChooser<T>), true, false, true)
            && typeof(T).IsClass;

        protected override void Initialize()
        {
            if (Attribute.ErrorMessage != null)
            {
                errorMessageGetter = ValueResolver.GetForString(Property, Attribute.ErrorMessage);
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (errorMessageGetter is { HasError: true })
            {
                SirenixEditorGUI.ErrorMessageBox(errorMessageGetter.ErrorMessage);
                CallNextDrawer(label);
                return;
            }

            if (IsValid() == false)
            {
                var name = label == null ? Property.Name : label.text;

                var errorMessage = errorMessageGetter == null
                    ? $"{name} is Required"
                    : errorMessageGetter.ErrorMessage;

                SirenixEditorGUI.ErrorMessageBox(errorMessage);
            }

            CallNextDrawer(label);
        }

        private bool IsValid()
        {
            var chooser = ValueEntry.SmartValue;

            if (chooser == null)
            {
                return false;
            }

            var currentValues = chooser.GetCurrentAllValues().ToArray();

            if (currentValues.Length == 0)
            {
                return false;
            }

            foreach (var value in currentValues)
            {
                if (value is string stringValue)
                {
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        return false;
                    }
                }
                else
                {
                    if (value == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

#endif

    #endregion

    public interface IValueSetter<out T>
    {
        public T GetValue();
    }

    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [RequiredObjectChooser]
    public class ObjectChooser<T> : ValuePreviewBase<T>, IValueSetter<T>, ICloneable
    {
        //[JsonObject(MemberSerialization.OptIn)]
        [Serializable]
        public class ProbabilityItem : ICloneable
        {
            [HideLabel]
            [OnValueChanged("@onValueChangedAction()")]
            public T value;

            [LabelText("占比"), LabelWidth(30)]
            [HorizontalGroup]
            [OnValueChanged("@onValueChangedAction()")]
            public int ratio;

            [LabelText("概率"), LabelWidth(30)]
            [SuffixLabel("%", Overlay = true)]
            [HorizontalGroup]
            [DisplayAsString]
            [JsonIgnore]
            public float probability;

            [HideLabel]
            [HorizontalGroup]
            [DisplayAsString]
            [GUIColor("@Color.yellow")]
            [JsonIgnore]
            public string tag;

            [HideInInspector]
            [JsonIgnore]
            public Action onValueChangedAction;

            public object Clone()
            {
                return new ProbabilityItem()
                {
                    value = value,
                    ratio = ratio,
                };
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        [Serializable]
        public class CircularItem : ICloneable
        {
            [HideInInspector]
            public int index;

            [LabelText(@"@""第"" + index.ToString() + ""个""")]
            [JsonProperty]
            public T value;

            [LabelText("循环次数")]
            [MinValue(1)]
            [JsonProperty]
            public int times;

            public object Clone()
            {
                return new CircularItem()
                {
                    value = value,
                    times = times,
                };
            }
        }

        public const string SINGLE_VALUE = "Single Value";
        public const string WEIGHTED_SELECT = "Weighted Select";
        public const string CIRCULAR_SELECT = "Circular Select";

        protected override bool showPreviewValueBelow => willPreviewValue && isRandomValue;

        protected virtual IEnumerable<ValueDropdownItem<string>> allRandomTypes => new ValueDropdownList<string>()
        {
            { "权值选择", WEIGHTED_SELECT },
            { "循环选择", CIRCULAR_SELECT }
        };

        protected virtual IEnumerable<ValueDropdownItem<string>> allFixedTypes => new ValueDropdownList<string>()
        {
            { $"单个{valueName}", SINGLE_VALUE }
        };

        protected virtual string valueName => "";

        protected virtual bool displayRandomTypeChooser => allRandomTypes.Count() > 1 && isRandomValue == true;

        protected virtual bool displayFixedTypeChooser => allFixedTypes.Count() > 1 && isRandomValue == false;

        [HideLabel, PropertyOrder(-999)]
        [ToggleButtons(@"@""随机"" + valueName", @"@""固定"" + valueName")]
        [OnValueChanged(nameof(OnValueGenerateTypeChanged))]
        [JsonProperty(Order = -101)]
        public bool isRandomValue = false;

        #region Fixed

        [LabelText(@"@""固定"" + valueName + ""种类"""), PropertyOrder(-888)]
        [OnValueChanged(nameof(OnValueGenerateTypeChanged))]
        [ValueDropdown(nameof(allFixedTypes))]
        [ShowIf(nameof(displayFixedTypeChooser))]
        [JsonProperty(Order = -100)]
        public string fixedType;

        [HideLabel]
        [OnValueChanged(nameof(PreviewValue))]
        [ShowIf(@"@isRandomValue == false && fixedType == SINGLE_VALUE")]
        [JsonProperty(Order = -98)]
        public T value;

        #endregion

        #region Random

        [LabelText(@"@""随机"" + valueName + ""种类"""), PropertyOrder(-888)]
        [OnValueChanged(nameof(OnValueGenerateTypeChanged))]
        [ValueDropdown(nameof(allRandomTypes))]
        [ShowIf(nameof(displayRandomTypeChooser))]
        [JsonProperty(Order = -99)]
        public string randomType;

        [HideReferenceObjectPicker]
        [LabelText(@"@""按权值随机选择其中的一个"" + valueName")]
        [OnValueChanged(nameof(PreviewValue), true)]
        [ListDrawerSettings(CustomAddFunction = nameof(AddProbabilityItemGUI), NumberOfItemsPerPage = 6)]
        [InfoBox("请至少添加一个", InfoMessageType.Warning, @"@valueProbabilities.Count <= 0")]
        [ShowIf(@"@isRandomValue && randomType == WEIGHTED_SELECT")]
        [InfoBox("包含重复元素", InfoMessageType.Warning, nameof(ValueProbabilitiesContainsSameValue))]
        [InfoBox("占比可以化简", InfoMessageType.Warning, "@!ValueProbabilitiesRatiosAreCoprime()")]
        [InfoBox("占比不能全为0", InfoMessageType.Warning, nameof(ValueProbabilitiesRatiosAreAllZero))]
        [JsonProperty]
        public List<ProbabilityItem> valueProbabilities = new();

        [LabelText("从第几个开始循环")]
        [PropertyTooltip("从0开始计数")]
        [OnValueChanged(nameof(PreviewValue))]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        [InfoBox("不能超过循环体的总数", InfoMessageType.Warning, @"@startCircularIndex >= circularItems.Count")]
        [MinValue(0)]
        [JsonProperty]
        public int startCircularIndex = 0;

        [LabelText("乒乓循环")]
        [PropertyTooltip("循环到底后，从后往前遍历")]
        [OnValueChanged(nameof(PreviewValue))]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT && circularItems != null && circularItems.Count >= 2")]
        [JsonProperty]
        public bool pingPong = false;

        [LabelText("循环体")]
        [HideReferenceObjectPicker]
        [InfoBox("请至少添加一个", InfoMessageType.Warning,
            @"@circularItems != null && circularItems.Count <= 0")]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        [OnValueChanged(nameof(PreviewValue), true)]
        [ListDrawerSettings(CustomAddFunction = nameof(CircularItemsAddGUI), ShowFoldout = true)]
        [JsonProperty]
        public List<CircularItem> circularItems = new();

        [LabelText("当前循环的序号"), FoldoutGroup("信息")]
        [NonSerialized]
        [ShowInInspector, DisplayAsString, HideInEditorMode]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        protected int currentCircularIndex = 0;

        [LabelText("当前循环的序号下的次数"), FoldoutGroup("信息")]
        [NonSerialized]
        [ShowInInspector, DisplayAsString, HideInEditorMode]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        protected int currentCircularTimes = 1;

        [LabelText("当前循环方向"), FoldoutGroup("信息")]
        [NonSerialized]
        [ShowInInspector, DisplayAsString, HideInEditorMode]
        [ShowIf(@"@isRandomValue && randomType == ""Circular Select"" && pingPong")]
        protected bool loopForward = true;

        #endregion

        #region GUI

        #region Weighted Select

        protected virtual ProbabilityItem AddProbabilityItemGUI()
        {
            return new()
            {
                ratio = 1
            };
        }

        private bool ValueProbabilitiesRatiosAreAllZero()
        {
            return valueProbabilities.All(item => item.ratio <= 0);
        }

        private bool ValueProbabilitiesContainsSameValue()
        {
            return valueProbabilities.ContainsSame(item => item.value);
        }

        [Button("合并相同的项")]
        [ShowIf(@"@isRandomValue && randomType == WEIGHTED_SELECT && ValueProbabilitiesContainsSameValue()")]
        private void ValueProbabilitiesMergeDuplicates()
        {
            valueProbabilities = (List<ProbabilityItem>)valueProbabilities.MergeDuplicates(item => item.value,
                (itemA, itemB) =>
                {
                    itemA.ratio += itemB.ratio;
                    return itemA;
                });

            OnValueProbabilitiesChanged();
        }

        private bool ValueProbabilitiesRatiosAreCoprime()
        {
            return valueProbabilities.Select(item => item.ratio).AreCoprime();
        }

        [Button("化简占比")]
        [ShowIf(@"@isRandomValue && randomType == WEIGHTED_SELECT && !ValueProbabilitiesRatiosAreCoprime()")]
        private void ValueProbabilitiesRatiosToCoprime()
        {
            if (ValueProbabilitiesRatiosAreAllZero())
            {
                valueProbabilities.Examine(item => item.ratio = 1);
                return;
            }
            int gcd = valueProbabilities.Select(item => item.ratio).GCD();
            if (gcd > 1)
            {
                valueProbabilities.Examine(item => item.ratio /= gcd);
            }
        }

        protected void OnValueProbabilitiesChanged()
        {

            if (valueProbabilities == null || valueProbabilities.Count == 0)
            {
                return;
            }

            valueProbabilities.RemoveAllNull();

            foreach (var item in valueProbabilities)
            {
                item.ratio = item.ratio.Clamp(valueProbabilities.Count == 1 ? 1 : 0, 9999);
                item.probability = 0;
                item.tag = "";
                item.onValueChangedAction = OnValueProbabilitiesChanged;
            }

            var indicesOfMaxRatio = valueProbabilities.GetIndicesOfMaxValues(item => item.ratio);
            var indicesOfMinRatio = valueProbabilities.GetIndicesOfMinValues(item => item.ratio);
            var totalRatio = valueProbabilities.Sum(item => item.ratio);

            if (totalRatio == 0)
            {
                return;
            }

            for (var index = 0; index < valueProbabilities.Count; index++)
            {
                var item = valueProbabilities[index];
                item.probability = 100f * item.ratio / totalRatio;

                bool isMax = indicesOfMaxRatio.Contains(index);
                bool isMin = indicesOfMinRatio.Contains(index);

                if (isMax && isMin)
                {
                    if (indicesOfMaxRatio.Count > 1)
                    {
                        item.tag += "一样";
                    }
                }
                else if (isMax)
                {
                    if (indicesOfMaxRatio.Count > 1)
                    {
                        item.tag += "同时";
                    }
                    item.tag += "最大";
                }
                else if (isMin)
                {
                    if (indicesOfMinRatio.Count > 1)
                    {
                        item.tag += "同时";
                    }
                    item.tag += "最小";
                }
            }
        }

        #endregion

        #region Circular Select

        private CircularItem CircularItemsAddGUI()
        {
            CircularItem item = new()
            {
                index = circularItems.Count,
                times = 1,
                value = default,
            };

            return item;
        }

        [Button("上移")]
        [ButtonGroup("CircularActions")]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        private void ShiftUpCircularItems()
        {
            circularItems.Rotate(-1);
            PreviewValue();
        }

        [Button("下移")]
        [ButtonGroup("CircularActions")]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        private void ShiftDownCircularItems()
        {
            circularItems.Rotate(1);
            PreviewValue();
        }

        [Button("打乱")]
        [ButtonGroup("CircularActions")]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        private void ShuffleCircularItems()
        {
            circularItems.Shuffle();
            PreviewValue();
        }

        [Button("重置循环次数")]
        [ButtonGroup("CircularActions")]
        [ShowIf(@"@isRandomValue && randomType == CIRCULAR_SELECT")]
        private void ResetCircularItemsTimes()
        {
            foreach (var item in circularItems)
            {
                item.times = 1;
            }

            PreviewValue();
        }

        #endregion

        #region Preview

        protected override void PreviewValue()
        {
            if (willPreviewValue == true)
            {
                if (isRandomValue == true)
                {
                    PreviewRandomValue();
                }
                else
                {
                    PreviewFixedValue();
                }
            }
            else
            {
                contentPreviewing = "";
            }

            base.PreviewValue();
        }

        protected virtual void PreviewFixedValue()
        {
            contentPreviewing = ValueToPreview(value);
        }


        protected virtual void PreviewRandomValue()
        {
            switch (randomType)
            {
                case WEIGHTED_SELECT:
                    OnValueProbabilitiesChanged();

                    if (valueProbabilities.Count == 0)
                    {
                        return;
                    }

                    if (valueProbabilities.Count == 1)
                    {
                        contentPreviewing = $"{ValueToPreview(valueProbabilities[0].value)}";
                    }
                    else
                    {
                        contentPreviewing =
                            ", ".Join(valueProbabilities.
                                Select(item => $"{ValueToPreview(item.value)}:{item.probability.ToString(1)}%"));
                    }
                    
                    break;
                case CIRCULAR_SELECT:
                    contentPreviewing = ", ".Join(circularItems.Select(item =>
                    {
                        if (item.times > 1)
                        {
                            return $"{ValueToPreview(item.value)}:{item.times}次";
                        }

                        return ValueToPreview(item.value);
                    }));

                    if (pingPong == true)
                    {
                        contentPreviewing += "  乒乓循环";
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        protected virtual void OnValueGenerateTypeChanged()
        {
            OnInspectorInit();
        }

        protected override void OnInspectorInit()
        {
            if (randomType.IsNullOrEmptyAfterTrim() || GetRandomTypes().Contains(randomType) == false)
            {
                randomType = GetDefaultRandomType();
            }

            if (fixedType.IsNullOrEmptyAfterTrim() || GetFixedTypes().Contains(fixedType) == false)
            {
                fixedType = GetDefaultFixedType();
            }

            valueProbabilities ??= new();
            circularItems ??= new();

            if (typeof(T).IsDerivedFrom(typeof(BaseConfigClass), false))
            {
                value ??= (T)typeof(T).CreateInstance();

                foreach (var item in valueProbabilities)
                {
                    item.value ??= (T)typeof(T).CreateInstance();
                }

                foreach (var item in circularItems)
                {
                    item.value ??= (T)typeof(T).CreateInstance();
                }
            }

            foreach (var item in valueProbabilities)
            {
                item.onValueChangedAction = OnValueProbabilitiesChanged;
            }

            PreviewValue();

            base.OnInspectorInit();
        }

        #endregion

        #region JSON

        public bool ShouldSerializerandomType()
        {
            return isRandomValue == true;
        }

        public bool ShouldSerializefixedType()
        {
            return isRandomValue == false;
        }

        public bool ShouldSerializevalue()
        {
            return isRandomValue == false && fixedType == "Single Value";
        }

        public bool ShouldSerializevalueProbabilities()
        {
            return isRandomValue == true && randomType == "Weighted Select";
        }

        public bool ShouldSerializecircularItems()
        {
            return isRandomValue == true && randomType == "Circular Select";
        }

        public bool ShouldSerializestartCircularIndex()
        {
            return isRandomValue == true && randomType == "Circular Select";
        }

        public bool ShouldSerializepingPong()
        {
            return isRandomValue == true && randomType == "Circular Select";
        }

        #endregion

        #region Fixed & Random Types

        public string GetDefaultRandomType()
        {
            return allRandomTypes.First().Value;
        }

        public string GetDefaultFixedType()
        {
            return allFixedTypes.First().Value;
        }

        public IEnumerable<string> GetRandomTypes()
        {
            return allRandomTypes.Select(item => item.Value);
        }

        public IEnumerable<string> GetFixedTypes()
        {
            return allFixedTypes.Select(item => item.Value);
        }

        #endregion

        public void Reset()
        {
            currentCircularIndex = 0;
            currentCircularTimes = 1;

            if (startCircularIndex < 0)
            {
                startCircularIndex = 0;
            }
            else if (startCircularIndex >= circularItems.Count)
            {
                startCircularIndex %= circularItems.Count;
            }

            if (circularItems.Count < 2)
            {
                pingPong = false;
            }
        }

        protected int GetCircularCount()
        {
            return circularItems.Sum(item => item.times);
        }

        protected IEnumerable<T> GetCircularItems()
        {
            foreach (var circularItem in circularItems)
            {
                for (int i = 0; i < circularItem.times; i++)
                {
                    yield return circularItem.value;
                }
            }
        }

        #region Get Value

        public virtual T GetFixedValue()
        {
            return value;
        }

        public virtual T GetRandomValue()
        {
            switch (randomType)
            {
                case WEIGHTED_SELECT:

                    if (valueProbabilities == null || valueProbabilities.Count == 0)
                    {
                        return value;
                    }


                    List<T> values = new();
                    List<float> probabilities = new();

                    foreach (var probabilityItem in valueProbabilities)
                    {
                        values.Add(probabilityItem.value);
                        probabilities.Add(probabilityItem.ratio);
                    }

                    return RandomFunc.Choose(values, probabilities);

                case CIRCULAR_SELECT:

                    if (circularItems.Count == 0)
                    {
                        return default;
                    }

                    var item = circularItems[currentCircularIndex];

                    if (pingPong == false)
                    {
                        currentCircularTimes++;
                        if (currentCircularTimes > item.times)
                        {
                            currentCircularTimes = 1;
                            currentCircularIndex++;

                            if (currentCircularIndex >= circularItems.Count)
                            {
                                //startCircularIndex = startCircularIndex.Clamp(0, circularItems.Count - 1);
                                currentCircularIndex = startCircularIndex;
                            }
                        }
                    }
                    else
                    {
                        currentCircularTimes++;
                        if (currentCircularTimes > item.times)
                        {
                            currentCircularTimes = 1;

                            if (loopForward)
                            {
                                currentCircularIndex++;

                                if (currentCircularIndex >= circularItems.Count)
                                {
                                    currentCircularIndex = circularItems.Count - 2;
                                    loopForward = false;
                                }
                            }
                            else
                            {
                                if (currentCircularIndex <= startCircularIndex)
                                {
                                    currentCircularIndex++;
                                    loopForward = true;
                                }
                                else
                                {
                                    currentCircularIndex--;
                                }
                            }
                            
                        }
                    }
                    

                    return item.value;

                default:
                    Note.note.Warning($"randomType的值错误:{randomType}");
                    return value;

            }
            
        }

        public T GetValue()
        {
            return isRandomValue == false ? GetFixedValue() : GetRandomValue();
        }

        #endregion

        #region Get Current Values

        protected virtual IEnumerable<T> GetCurrentFixedValues()
        {
            yield return fixedType switch
            {
                SINGLE_VALUE => value,
                _ => throw new ArgumentException()
            };
        }

        protected virtual IEnumerable<T> GetCurrentRandomValues()
        {
            return randomType switch
            {
                WEIGHTED_SELECT => valueProbabilities.Select(item => item.value),
                CIRCULAR_SELECT => circularItems.Select(item => item.value),
                _ => throw new ArgumentException()
            };
        }

        public IEnumerable<T> GetCurrentAllValues()
        {
            if (isRandomValue)
            {
                return GetCurrentRandomValues();
            }

            return GetCurrentFixedValues();
        }

        #endregion

        #region Conversion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(ObjectChooser<T> chooser)
        {
            if (chooser == null)
            {
                Note.note.Warning("chooser is Null");
                return default;
            }

            return chooser.GetValue();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ObjectChooser<T>(T value)
        {
            return new()
            {
                isRandomValue = false,
                value = value
            };
        }

        #endregion

        public virtual object Clone()
        {
            var clone = (ObjectChooser<T>)GetType().CreateInstance();
            clone.isRandomValue = isRandomValue;
            clone.fixedType = fixedType;
            clone.randomType = randomType;
            clone.value = value.Clone();
            clone.valueProbabilities = valueProbabilities.Clone();
            clone.startCircularIndex = startCircularIndex;
            clone.pingPong = pingPong;
            clone.circularItems = circularItems.Clone();
            clone.currentCircularIndex = 0;
            clone.currentCircularTimes = 1;
            clone.loopForward = true;
            return clone;
        }

        public override string ToString()
        {
            PreviewValue();
            return contentPreviewing;
        }
    }
}


