using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConfigurationBasis
{

    [JsonObject(MemberSerialization.OptIn)]
    [InfoBox("存在颜色的Alpha值过小", InfoMessageType.Warning, @"@$value.ContainsColorWithLowAlpha()")]
    [Serializable]
    public sealed class ColorSetter : NumberOrVectorChooser<Color, ColorRange>
    {
        protected override bool valueAlwaysPreviewed => true;

        protected override bool showPreviewValueBelow => false;

        protected override string valueName => "颜色";

        private float alphaThreshold => 0.2f;

        #region Lerp

        [LabelText("颜色带")]
        [ProgressBar("@this.GetMinLerpPoint()", "@this.GetMaxLerpPoint()", ColorGetter = "ColorBarColorGetter")]
        [ShowIf(@"@isRandomValue == false && fixedType == ""Lerp""")]
        public float colorBar;

        #endregion


        [LabelText("颜色显示格式")]
        [EnumToggleButtons]
        [OnValueChanged(nameof(PreviewValue))]
        public ColorStringFormat colorStringFormat;

        #region GUI

        #region Lerp

        private Color ColorBarColorGetter(float value)
        {
            return GetInterpolatedValue(value);
        }

        #endregion

        #region SetAlpha

        [Button("所有Alpha值置1")]
        [ShowIf(nameof(RequireAlphaSetTo1))]
        private void SetAlphaTo1()
        {
            if (isRandomValue)
            {
                switch (randomType)
                {
                    case "Weighted Select":
                        valueProbabilities.Examine(item => item.value.ChangeAlpha(1f));
                        break;

                    case "Circular Select":
                        circularItems.Examine(item => item.value.ChangeAlpha(1f));
                        break;

                    case "Range":
                        rangeValue.minPos.a = 1f;
                        rangeValue.maxPos.a = 1f;
                        break;
                }
            }
            else
            {
                switch (fixedType)
                {
                    case "Single Value":
                        value.a = 1;
                        break;

                    case "Lerp":
                        lerpPoints.Examine(point => point.value.a = 1);
                        defaultValue.a = 1;
                        break;
                }
            }
            
        }

        private bool RequireAlphaSetTo1()
        {
            if (isRandomValue)
            {
                return randomType switch
                {
                    "Weighted Select" => valueProbabilities.Any(item => item.value.a < 1 - alphaThreshold),
                    "Circular Select" => circularItems.Any(item => item.value.a < 1 - alphaThreshold),
                    "Range" => rangeValue.minPos.a < 1 - alphaThreshold || rangeValue.maxPos.a < 1 - alphaThreshold,
                    _ => throw new ArgumentException()
                };
            }

            return fixedType switch
            {
                "Single Value" => value.a < 1 - alphaThreshold,
                "Lerp" => lerpPoints.Any(point => point.value.a < 1 - alphaThreshold) ||
                          defaultValue.a < 1 - alphaThreshold,
                _ => throw new ArgumentException()
            };
        }

        private bool ContainsColorWithLowAlpha()
        {
            if (isRandomValue)
            {
                return randomType switch
                {
                    "Weighted Select" => valueProbabilities.Any(item => item.value.a < alphaThreshold),
                    "Circular Select" => circularItems.Any(item => item.value.a < alphaThreshold),
                    "Range" => rangeValue.maxPos.a < alphaThreshold || rangeValue.minPos.a < alphaThreshold,
                    _ => throw new ArgumentException()
                };
            }

            return fixedType switch
            {
                "Single Value" => value.a < alphaThreshold,
                "Lerp" => lerpPoints.Any(point => point.value.a < alphaThreshold) ||
                          defaultValue.a < alphaThreshold,
                _ => throw new ArgumentException()
            };
        }

        #endregion

        #region SetRandomColor

        [Button(@"@""随机"" + valueName")]
        private void SetRandomColorGUI()
        {
            if (isRandomValue)
            {
                switch (randomType)
                {
                    case "Weighted Select":
                        valueProbabilities.Examine(item => item.value = GenerateRandomColorGUI());
                        break;

                    case "Circular Select":
                        circularItems.Examine(item => item.value = GenerateRandomColorGUI());
                        break;

                    case "Range":
                        rangeValue.minPos = GenerateRandomColorGUI();
                        rangeValue.maxPos = GenerateRandomColorGUI();
                        break;

                    default:
                        throw new ArgumentException();
                }
            }
            else
            {
                switch (fixedType)
                {
                    case "Single Value":
                        value = GenerateRandomColorGUI();
                        break;

                    case "Lerp":
                        defaultValue = GenerateRandomColorGUI();
                        lerpPoints.Examine(point => point.value = GenerateRandomColorGUI());
                        break;

                    default:
                        throw new ArgumentException();
                }
            }

            PreviewValue();
        }

        private Color GenerateRandomColorGUI()
        {
            return Color.black.RandomRange(Color.white);
        }

        #endregion

        protected override string ValueToPreview(Color value)
        {
            return value.ToString(colorStringFormat);
        }

        #endregion
    }
}

