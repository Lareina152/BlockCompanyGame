using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Basis;
using GenericBasis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEditor;

#endif


namespace ConfigurationBasis
{
    #region Attribute

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class InterpolatedRangeAttribute : Attribute
    {
        public double MinT;
        public string MinTExpression;

        public double MaxT;
        public string MaxTExpression;

        public InterpolatedRangeAttribute(double minT, double maxT)
        {
            MinT = minT;
            MaxT = maxT;
        }

        public InterpolatedRangeAttribute(string minTExpression, string maxTExpression)
        {
            MinTExpression = minTExpression;
            MaxTExpression = maxTExpression;
        }
    }

#if UNITY_EDITOR
    public class InterpolatedRangeDrawer<TChooser, TNumber, TRange> : OdinAttributeDrawer<InterpolatedRangeAttribute, TChooser>
        where TChooser : NumberOrVectorChooser<TNumber, TRange>
        where TNumber : struct, IEquatable<TNumber>
        where TRange : KCubeShape<TNumber>, new()
    {
        private static readonly bool IsNumber = GenericNumberUtility.IsNumber(typeof(TNumber));

        private ValueResolver<double> minTGetter;
        private ValueResolver<double> maxTGetter;

        public override bool CanDrawTypeFilter(System.Type type) =>
            type.IsDerivedFrom(typeof(NumberOrVectorChooser<TNumber, TRange>), true);

        protected override void Initialize()
        {
            minTGetter = ValueResolver.Get(Property, Attribute.MinTExpression, Attribute.MinT);
            maxTGetter = ValueResolver.Get(Property, Attribute.MaxTExpression, Attribute.MaxT);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var hasErrors = new ValueResolver<double>[]
                    { minTGetter, maxTGetter}
                .ExamineIf(resolver => resolver.HasError, resolver =>
                {
                    SirenixEditorGUI.ErrorMessageBox(resolver.ErrorMessage);
                    CallNextDrawer(label);
                });

            if (hasErrors.Any())
            {
                return;
            }

            double minT = minTGetter.GetValue();
            double maxT = maxTGetter.GetValue();

            foreach (var lerpPoint in ValueEntry.SmartValue.lerpPoints)
            {
                if (GenericNumberUtility.NumberIsInRange(lerpPoint.t, minT, maxT) == false)
                {
                    lerpPoint.t = GenericNumberUtility.Clamp(lerpPoint.t, minT, maxT);
                }
            }

            ValueEntry.SmartValue.lerpTLimit = true;
            ValueEntry.SmartValue.minLerpT = minT;
            ValueEntry.SmartValue.maxLerpT = maxT;

            EditorGUI.BeginChangeCheck();


            CallNextDrawer(label);

            if (!EditorGUI.EndChangeCheck())
            {
                return;
            }
        }
    }

#endif

    #endregion

    public interface INumberOrVectorValueSetter<out T>
    {
        public T GetInterpolatedValue(float t);
    }


    public abstract class NumberOrVectorChooser<T, TRange> : ObjectChooser<T>, INumberOrVectorValueSetter<T>
        where T : struct, IEquatable<T>
        where TRange : KCubeShape<T>, new()
    {
        [HideDuplicateReferenceBox]
        [HideReferenceObjectPicker]
        [JsonObject(MemberSerialization.OptIn)]
        [Serializable]
        public class LerpPoint : ICloneable
        {
            [LabelText("插值点")]
            [JsonProperty]
            public float t;

            [LabelText("插值系数")]
            [JsonProperty]
            public T value;

            public object Clone()
            {
                return new LerpPoint()
                {
                    t = t,
                    value = value
                };
            }
        }

        public const string RANGE_SELECT = "Range";
        public const string LERP_SELECT = "Lerp";

        protected virtual float minInterpolationPower => 0.1f;

        protected virtual float maxInterpolationPower => 10;

        protected override IEnumerable<ValueDropdownItem<string>> allRandomTypes =>
            base.allRandomTypes.Concat(new ValueDropdownList<string>()
            {
                { "范围", RANGE_SELECT }
            });

        protected override IEnumerable<ValueDropdownItem<string>> allFixedTypes =>
            base.allFixedTypes.Concat(new ValueDropdownList<string>()
            {
                { $"{valueName}插值", LERP_SELECT },
            });

        #region Range

        [HideLabel]
        [ShowIf(@"@isRandomValue && randomType == RANGE_SELECT")]
        [OnValueChanged(nameof(PreviewValue), true)]
        [JsonProperty]
        public TRange rangeValue;

        #endregion

        #region Lerp

        [LabelText(@"@""默认"" + valueName")]
        [ShowIf(@"@isRandomValue == false && fixedType == LERP_SELECT")]
        [OnValueChanged("PreviewValue")]
        [JsonProperty]
        public T defaultValue;

        [LabelText("插值函数的幂次")]
        [ShowIf(@"@isRandomValue == false && fixedType == LERP_SELECT")]
        [MinValue("minInterpolationPower"), MaxValue("maxInterpolationPower")]
        [PropertyTooltip(@"@""用来调整"" + valueName + ""渐变的速度""")]
        [OnValueChanged("PreviewValue")]
        [JsonProperty]
        public float interpolationPower = 1f;

        [LabelText(@"@valueName + ""插值点""")]
        [ShowIf(@"@isRandomValue == false && fixedType == LERP_SELECT")]
        [InfoBox("插值点不能为空", InfoMessageType.Error, @"@lerpPoints.Count <= 0")]
        [InfoBox("插值点至少两个", InfoMessageType.Error, @"@lerpPoints.Count == 1")]
        [InfoBox(@"@""推荐插值系数范围：["" + minLerpT + "", "" + maxLerpT + ""]""", nameof(lerpTLimit))]
        [ListDrawerSettings(CustomAddFunction = "AddLerpPointGUI")]
        [OnCollectionChanged("OnLerpPointsChanged")]
        [JsonProperty]
        public List<LerpPoint> lerpPoints = new();

        [NonSerialized]
        public bool lerpTLimit;
        [NonSerialized]
        public double maxLerpT;
        [NonSerialized]
        public double minLerpT;

        #endregion

        #region GUI

        #region Lerp

        protected virtual float GetMaxLerpPoint()
        {
            if (lerpPoints.Count == 0)
            {
                return 0;
            }
            return lerpPoints.Max(point => point.t);
        }

        protected virtual float GetMinLerpPoint()
        {
            if (lerpPoints.Count == 0)
            {
                return 0;
            }
            return lerpPoints.Min(point => point.t);
        }

        protected virtual LerpPoint AddLerpPointGUI()
        {
            return new LerpPoint();
        }

        [Button("排序")]
        [ShowIf(@"@isRandomValue == false && fixedType == LERP_SELECT && lerpPoints.Count > 1")]
        protected virtual void OnLerpPointsChanged()
        {
            lerpPoints.Sort((a, b) => a.t.CompareTo(b.t));

            PreviewValue();
        }

        #endregion

        protected override void PreviewFixedValue()
        {
            switch (fixedType)
            {
                case LERP_SELECT:
                    if (lerpPoints == null || lerpPoints.Count == 0)
                    {
                        contentPreviewing = ValueToPreview(defaultValue);
                    }
                    else
                    {
                        contentPreviewing = lerpPoints.Select(point => ValueToPreview(point.value)).Join("->");
                    }
                    break;
                default:
                    base.PreviewFixedValue();
                    break;
            }
        }

        protected override void PreviewRandomValue()
        {
            switch (randomType)
            {
                case RANGE_SELECT:
                    contentPreviewing = $"[{ValueToPreview(rangeValue.minPos)}, {ValueToPreview(rangeValue.maxPos)}]";
                    break;
                default:
                    base.PreviewRandomValue();
                    break;
            }
        }

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            rangeValue ??= new();

            lerpPoints ??= new();

            if (interpolationPower.BetweenInclusive(minInterpolationPower, maxInterpolationPower) == false)
            {
                interpolationPower = 1;
            }
        }

        #endregion

        #region JSON

        public bool ShouldSerializedefaultValue()
        {
            return isRandomValue == false && fixedType == LERP_SELECT;
        }

        public bool ShouldSerializeinterpolationPower()
        {
            return isRandomValue == false && fixedType == LERP_SELECT;
        }

        public bool ShouldSerializelerpPoints()
        {
            return isRandomValue == false && fixedType == LERP_SELECT;
        }

        public bool ShouldSerializerangeValue()
        {
            return isRandomValue == true && randomType == RANGE_SELECT;
        }

        #endregion

        #region MaxValue

        public virtual T GetMaxWeightedProbabilityValue()
        {
            return valueProbabilities.MaxOrDefault(item => item.value);
        }

        public virtual T GetMaxCircularValue()
        {
            return circularItems.MaxOrDefault(item => item.value);
        }

        public virtual T GetMaxRangeValue()
        {
            return rangeValue.maxPos;
        }

        protected virtual T GetMaxRandomValue()
        {
            T MaxValue = GetMaxRangeValue();

            if (valueProbabilities.Count > 0)
            {
                MaxValue = GetMaxWeightedProbabilityValue();
            }

            if (circularItems.Count > 0)
            {
                MaxValue = MaxValue.Max(GetMaxCircularValue());
            }

            return MaxValue;
        }

        protected virtual T GetMaxSingleValue()
        {
            return value;
        }

        protected virtual T GetMaxLerpValue()
        {
            T MaxValue = defaultValue;

            if (lerpPoints.Count > 0)
            {
                MaxValue = MaxValue.Max(GenericNumberFunc.Max(lerpPoints, item => item.value));
            }

            return MaxValue;
        }

        protected virtual T GetMaxFixedValue()
        {
            var MaxValue = GetMaxSingleValue();

            MaxValue = MaxValue.Max(GetMaxLerpValue());

            return MaxValue;
        }

        public T GetMaxValue()
        {
            return GetMaxFixedValue().Max(GetMaxRandomValue());
        }

        public virtual void SetMaxRandomValue(T MaxValue)
        {
            rangeValue.maxPos = rangeValue.maxPos.ClampMax(MaxValue);
            rangeValue.maxPos = rangeValue.maxPos.ClampMax(MaxValue);
            valueProbabilities.Examine(item => item.value = item.value.ClampMax(MaxValue));
            circularItems.Examine(item => item.value = item.value.ClampMax(MaxValue));
        }

        protected virtual void SetMaxFixedValue(T MaxValue)
        {
            value = value.ClampMax(MaxValue);
            lerpPoints.Examine(point => point.value = point.value.ClampMax(MaxValue));
            defaultValue = defaultValue.ClampMax(MaxValue);
        }

        public void SetMaxValue(T MaxValue)
        {
            SetMaxRandomValue(MaxValue);
            SetMaxFixedValue(MaxValue);
        }

        #endregion

        #region MinValue

        public virtual T GetMinWeightedProbabilityValue()
        {
            return valueProbabilities.MinOrDefault(item => item.value);
        }

        public virtual T GetMinCircularValue()
        {
            return circularItems.MinOrDefault(item => item.value);
        }

        public virtual T GetMinRangeValue()
        {
            return rangeValue.minPos;
        }

        protected virtual T GetMinRandomValue()
        {
            T minValue = GetMinRangeValue();

            if (valueProbabilities.Count > 0)
            {
                minValue = GetMinWeightedProbabilityValue();
            }

            if (circularItems.Count > 0)
            {
                minValue = minValue.Min(GetMinCircularValue());
            }

            return minValue;
        }

        protected virtual T GetMinSingleValue()
        {
            return value;
        }

        protected virtual T GetMinLerpValue()
        {
            T minValue = defaultValue;

            if (lerpPoints.Count > 0)
            {
                minValue = minValue.Min(GenericNumberFunc.Min(lerpPoints, item => item.value));
            }

            return minValue;
        }

        protected virtual T GetMinFixedValue()
        {
            var minValue = GetMinSingleValue();

            minValue = minValue.Min(GetMinLerpValue());

            return minValue;
        }

        public T GetMinValue()
        {
            return GetMinFixedValue().Min(GetMinRandomValue());
        }

        public virtual void SetMinRandomValue(T minValue)
        {
            rangeValue.maxPos = rangeValue.maxPos.ClampMin(minValue);
            rangeValue.minPos = rangeValue.minPos.ClampMin(minValue);
            valueProbabilities.Examine(item => item.value = item.value.ClampMin(minValue));
            circularItems.Examine(item => item.value = item.value.ClampMin(minValue));
        }

        protected virtual void SetMinFixedValue(T minValue)
        {
            value = value.ClampMin(minValue);
            lerpPoints.Examine(point => point.value = point.value.ClampMin(minValue));
            defaultValue = defaultValue.ClampMin(minValue);
        }

        public void SetMinValue(T minValue)
        {
            SetMinRandomValue(minValue);
            SetMinFixedValue(minValue);
        }

        #endregion

        #region Add

        public void Add(T addend)
        {
            value = value.Add(addend);

            foreach (var item in lerpPoints)
            {
                item.value = item.value.Add(addend);
            }

            foreach (var item in valueProbabilities)
            {
                item.value = item.value.Add(addend);
            }

            foreach (var item in circularItems)
            {
                item.value = item.value.Add(addend);
            }

            rangeValue?.Add(addend);
        }

        #endregion

        public override T GetFixedValue()
        {
            return fixedType == LERP_SELECT ? defaultValue : base.GetFixedValue();
        }

        public virtual T GetInterpolatedValue(float t)
        {
            if (isRandomValue == false && fixedType == LERP_SELECT)
            {
                if (lerpPoints.Count < 2)
                {
                    return defaultValue;
                }

                var (firstPoint, secondPoint) = t.Between(lerpPoints, point => point.t);

                if (firstPoint == null)
                {
                    return secondPoint.value;
                }

                if (secondPoint == null)
                {
                    return firstPoint.value;
                }

                return firstPoint.value.Lerp(secondPoint.value,
                    Mathf.Pow(t.Normalize(firstPoint.t, secondPoint.t), interpolationPower));
            }


            return GetValue();
        }

        public override T GetRandomValue()
        {
            return randomType switch
            {
                RANGE_SELECT => rangeValue.GetRandomPoint(),
                _ => base.GetRandomValue(),
            };
        }

        protected override IEnumerable<T> GetCurrentFixedValues()
        {
            return fixedType switch
            {
                LERP_SELECT => lerpPoints.Select(point => point.value),
                _ => base.GetCurrentFixedValues()
            };
        }

        protected override IEnumerable<T> GetCurrentRandomValues()
        {
            return randomType switch
            {
                RANGE_SELECT => new[] { rangeValue.minPos, rangeValue.maxPos }, 
                _ => base.GetCurrentRandomValues()
            };
        }

        public override object Clone()
        {
            var baseClone = base.Clone();
            var newInstance = (NumberOrVectorChooser<T, TRange>)baseClone;
            newInstance.rangeValue = (TRange)rangeValue?.Clone();
            newInstance.defaultValue = defaultValue;
            newInstance.interpolationPower = interpolationPower;
            newInstance.lerpPoints = lerpPoints?.Clone();
            return newInstance;
        }
    }
}


