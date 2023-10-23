using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ConfigurationBasis
{
    [JsonObject(MemberSerialization.OptIn, Description = "Float Setter")]
    [Serializable]
    public class FloatSetter : NumberChooser<float, RangeFloat>
    {
        protected override string valueName => "浮点数";

        protected override bool valueAlwaysPreviewed => true;

        [LabelText("小数点后显示几位")]
        [MinValue(0)]
        [OnValueChanged("PreviewValue")]
        public int decimalPlaces = 1;

        #region GUI

        [Button("添加定步长的范围浮点数", Style = ButtonStyle.Box)]
        [ShowIf(@"@isRandomValue && randomType == ""Circular Select""")]
        private void SetCircularItemsToRange([LabelText("起始值")] float start = 0,
            [LabelText("结束值")] float end = 5, 
            [LabelText("步长"), MinValue(0.01)] float step = 1)
        {
            circularItems.Clear();

            foreach (var point in start.GetSteppedPoints(end, step))
            {
                circularItems.Add(new CircularItem() {times = 1, value = point});
            }

            PreviewValue();
        }

        #endregion


        protected override string ValueToPreview(float value)
        {
            return value.ToString(decimalPlaces);
        }

        public static implicit operator FloatSetter(double fixedFloat)
        {
            return new FloatSetter()
            {
                isRandomValue = false,
                value = fixedFloat.F()
            };
        }
    }
}

