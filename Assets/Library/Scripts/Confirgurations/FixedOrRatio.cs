#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

using System;
using System.Diagnostics;
using Basis;
using GenericBasis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConfigurationBasis
{

    #region MinMax

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class MinMaxFixedRatioValueAttribute : Attribute
    {
        public double MinFixedValue;
        public string MinFixedExpression;

        public double MaxFixedValue;
        public string MaxFixedExpression;

        public double MinRatio;
        public string MinRatioExpression;

        public double MaxRatio;
        public string MaxRatioExpression;

        /// <summary>
        /// Sets minimums for the property in the inspector.
        /// </summary>
        /// <param name="minFixedValue">The minimum value.</param>
        /// <param name="maxFixedValue"></param>
        /// <param name="minRatio">The minimum ratio</param>
        /// <param name="maxRatio"></param>
        public MinMaxFixedRatioValueAttribute(double minFixedValue, double maxFixedValue, double minRatio, double maxRatio)
        {
            MinFixedValue = minFixedValue;
            MaxFixedValue = maxFixedValue;
            MinRatio = minRatio;
            MaxRatio = maxRatio;
        }

        /// <summary>
        /// Sets minimums for the property in the inspector.
        /// </summary>
        /// <param name="minFixedExpression"></param>
        /// <param name="maxFixedExpression"></param>
        /// <param name="minRatioExpression"></param>
        /// <param name="maxRatioExpression"></param>
        public MinMaxFixedRatioValueAttribute(string minFixedExpression, string maxFixedExpression,
            string minRatioExpression, string maxRatioExpression)
        {
            MinFixedExpression = minFixedExpression;
            MaxFixedExpression = maxFixedExpression;
            MinRatioExpression = minRatioExpression;
            MaxRatioExpression = maxRatioExpression;
        }
    }
#if UNITY_EDITOR
    public class MinMaxFixedRatioValueDrawer<TChooser, TNumber> : OdinAttributeDrawer<MinMaxFixedRatioValueAttribute, TChooser>
        where TChooser : FixedOrRatio<TNumber>
        where TNumber : struct, IComparable, IFormattable, IConvertible, IEquatable<TNumber>
    {
        private static readonly bool IsNumber = GenericNumberUtility.IsNumber(typeof(TNumber));

        private ValueResolver<double> minFixedValueGetter;
        private ValueResolver<double> maxFixedValueGetter;
        private ValueResolver<double> minRatioGetter;
        private ValueResolver<double> maxRatioGetter;

        public override bool CanDrawTypeFilter(System.Type type) =>
            type.IsDerivedFrom(typeof(FixedOrRatio<TNumber>), true);

        protected override void Initialize()
        {
            minFixedValueGetter = ValueResolver.Get(Property, Attribute.MinFixedExpression, Attribute.MinFixedValue);
            maxFixedValueGetter = ValueResolver.Get(Property, Attribute.MaxFixedExpression, Attribute.MaxFixedValue);
            minRatioGetter = ValueResolver.Get(Property, Attribute.MinRatioExpression, Attribute.MinRatio);
            maxRatioGetter = ValueResolver.Get(Property, Attribute.MaxRatioExpression, Attribute.MaxRatio);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var hasErrors = new ValueResolver<double>[]
                    { minFixedValueGetter, maxFixedValueGetter, minRatioGetter, maxRatioGetter }
                .ExamineIf(resolver => resolver.HasError, resolver =>
                {
                    SirenixEditorGUI.ErrorMessageBox(resolver.ErrorMessage);
                    CallNextDrawer(label);
                });

            if (hasErrors.Any())
            {
                return;
            }

            var fixedValue = ValueEntry.SmartValue.fixedValue;
            var ratio = ValueEntry.SmartValue.ratio;

            double minFixedValue = minFixedValueGetter.GetValue();
            double maxFixedValue = maxFixedValueGetter.GetValue();
            double minRatio = minRatioGetter.GetValue();
            double maxRatio = maxRatioGetter.GetValue();

            ValueEntry.SmartValue.fixedValue =
                GenericNumberUtility.Clamp(ValueEntry.SmartValue.fixedValue, minFixedValue, maxFixedValue);
            ValueEntry.SmartValue.ratio =
                GenericNumberUtility.Clamp(ValueEntry.SmartValue.ratio, minRatio, maxRatio);

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

    public enum FixedRatioValueType
    {
        [LabelText("固定值")]
        Fixed,
        [LabelText("按比例")]
        Ratio
    }

    [Serializable]
    public class FixedOrRatio<TNumber> : ValuePreviewBase<TNumber>
        where TNumber : struct, IConvertible, IEquatable<TNumber>, IComparable, IFormattable
    {
        protected override bool valueAlwaysPreviewed => true;
        protected override bool showPreviewValueBelow => false;

        [HideLabel, EnumToggleButtons]
        [OnValueChanged(nameof(PreviewValue))]
        public FixedRatioValueType fixedRatioValueType;
        [HideLabel, ShowIf(@"@fixedRatioValueType == FixedRatioValueType.Fixed")]
        [OnValueChanged(nameof(PreviewValue))]
        public TNumber fixedValue;
        [HideLabel, ShowIf("@fixedRatioValueType == FixedRatioValueType.Ratio")]
        [OnValueChanged(nameof(PreviewValue))]
        public float ratio;

        #region GUI

        protected override void PreviewValue()
        {
            base.PreviewValue();

            contentPreviewing = fixedRatioValueType switch
            {
                FixedRatioValueType.Fixed => ValueToPreview(fixedValue),
                FixedRatioValueType.Ratio => $"{ratio}，按比例",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #endregion

        public TNumber GetValue(TNumber totalNumber)
        {
            double _totalNumber = GenericNumberFunc.ConvertNumber<double>(totalNumber);
            return fixedRatioValueType switch
            {
                FixedRatioValueType.Fixed => fixedValue,
                FixedRatioValueType.Ratio => GenericNumberFunc.ConvertNumber<TNumber>(ratio * _totalNumber),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

