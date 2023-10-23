using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
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

#if UNITY_EDITOR
    public class MinValueDrawerForNumberChooser<TChooser, TNumber, TRange> : OdinAttributeDrawer<MinValueAttribute, TChooser>
        where TChooser : NumberChooser<TNumber, TRange>
        where TNumber : struct, IComparable, IFormattable, IConvertible, IEquatable<TNumber>
        where TRange : KCubeShape<TNumber>, new()
    {
        private static readonly bool IsNumber = GenericNumberUtility.IsNumber(typeof(TNumber));
        private ValueResolver<double> minValueGetter;

        public override bool CanDrawTypeFilter(System.Type type) =>
            type.IsDerivedFrom(typeof(NumberChooser<TNumber, TRange>), true);

        protected override void Initialize() =>
            minValueGetter = ValueResolver.Get(Property, Attribute.Expression, Attribute.MinValue);

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (minValueGetter.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(minValueGetter.ErrorMessage);
                CallNextDrawer(label);
            }
            else
            {
                var smartValue = ValueEntry.SmartValue.GetMinValue();
                double min = minValueGetter.GetValue();

                ValueEntry.SmartValue.SetMinValue(GenericNumberUtility.ConvertNumber<TNumber>(min));

                EditorGUI.BeginChangeCheck();

                CallNextDrawer(label);

                if (!EditorGUI.EndChangeCheck())
                {
                    return;
                }

                //ValueEntry.SmartValue = GenericNumberUtility.Clamp<T>(smartValue, min, double.MaxValue);
            }
        }
    }

    public class MaxValueDrawerForNumberChooser<TChooser, TNumber, TRange> : OdinAttributeDrawer<MaxValueAttribute, TChooser>
        where TChooser : NumberChooser<TNumber, TRange>
        where TNumber : struct, IComparable, IFormattable, IConvertible, IEquatable<TNumber>
        where TRange : KCubeShape<TNumber>, new()
    {
        private static readonly bool IsNumber = GenericNumberUtility.IsNumber(typeof(TNumber));
        private ValueResolver<double> maxValueGetter;

        public override bool CanDrawTypeFilter(System.Type type) =>
            type.IsDerivedFrom(typeof(NumberChooser<TNumber, TRange>), true);

        protected override void Initialize() =>
            maxValueGetter = ValueResolver.Get(Property, Attribute.Expression, Attribute.MaxValue);

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (maxValueGetter.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(maxValueGetter.ErrorMessage);
                CallNextDrawer(label);
            }
            else
            {
                var smartValue = ValueEntry.SmartValue.GetMaxValue();
                double max = maxValueGetter.GetValue();

                ValueEntry.SmartValue.SetMaxValue(GenericNumberUtility.ConvertNumber<TNumber>(max));

                EditorGUI.BeginChangeCheck();

                CallNextDrawer(label);

                if (!EditorGUI.EndChangeCheck())
                {
                    return;
                }

                //ValueEntry.SmartValue = GenericNumberUtility.Clamp<T>(smartValue, min, double.MaxValue);
            }
        }
    }

#endif

    #endregion


    public abstract class NumberChooser<TNumber, TRange> : NumberOrVectorChooser<TNumber, TRange> 
        where TNumber : struct, IComparable, IFormattable, IConvertible, IEquatable<TNumber>
        where TRange : KCubeShape<TNumber>, new()
    {
        protected override bool valueAlwaysPreviewed => true;

        #region GUI



        #endregion

        #region JSON

        

        #endregion

        

    }
}
