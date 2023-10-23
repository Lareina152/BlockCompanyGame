using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using ConfigurationBasis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConfigurationBasis
{
    public class Vector2Setter : NumberOrVectorChooser<Vector2, RectangleFloat>
    {
        protected override string valueName => "向量";

        protected override bool valueAlwaysPreviewed => true;

        [LabelText("小数点后显示几位")]
        [MinValue(0)]
        [OnValueChanged("PreviewValue")]
        public int decimalPlaces = 1;

        #region GUI

        [Button("X轴对称")]
        [ShowIf(@"@isRandomValue && randomType == WEIGHTED_SELECT")]
        [ButtonGroup("WeightedSelectTools")]
        private void AddXAxisymmetric()
        {
            foreach (var item in valueProbabilities.ToArray())
            {
                var symmetric = item.value.XAxisymmetric();

                if (valueProbabilities.All(item => item.value != symmetric))
                {
                    valueProbabilities.Add(new()
                    {
                        value = symmetric,
                        ratio = 1
                    });
                }
            }

            OnValueProbabilitiesChanged();
        }

        [Button("Y轴对称")]
        [ShowIf(@"@isRandomValue && randomType == WEIGHTED_SELECT")]
        [ButtonGroup("WeightedSelectTools")]
        private void AddYAxisymmetric()
        {
            foreach (var item in valueProbabilities.ToArray())
            {
                var symmetric = item.value.YAxisymmetric();

                if (valueProbabilities.All(item => item.value != symmetric))
                {
                    valueProbabilities.Add(new()
                    {
                        value = symmetric,
                        ratio = 1
                    });
                }
            }

            OnValueProbabilitiesChanged();
        }

        [Button("原点对称")]
        [ShowIf(@"@isRandomValue && randomType == WEIGHTED_SELECT")]
        [ButtonGroup("WeightedSelectTools")]
        private void AddOriginSymmetric()
        {
            foreach (var item in valueProbabilities.ToArray())
            {
                var symmetric = item.value.PointSymmetric();

                if (valueProbabilities.All(item => item.value != symmetric))
                {
                    valueProbabilities.Add(new()
                    {
                        value = symmetric,
                        ratio = 1
                    });
                }
            }

            OnValueProbabilitiesChanged();
        }

        protected override string ValueToPreview(Vector2 value)
        {
            return value.ToString(decimalPlaces);
        }

        #endregion

        public static implicit operator Vector2Setter(Vector2 fixedVector)
        {
            return new()
            {
                isRandomValue = false,
                value = fixedVector
            };
        }
    }
}

