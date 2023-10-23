using System;
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
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Vector3Setter : ValuePreviewBase<Vector3>, IValueSetter<Vector3>, INumberOrVectorValueSetter<Vector3>
    {
        protected override bool valueAlwaysPreviewed => true;

        protected override bool showPreviewValueBelow => false;

        [LabelText("仅作为二维向量")]
        [ToggleButtons("是", "否")]
        [OnValueChanged(nameof(PreviewValue))]
        [JsonProperty]
        public bool asVector2D = false;

        [LabelText("向量所在平面")]
        [EnumToggleButtons]
        [ShowIf(nameof(asVector2D))]
        [JsonProperty]
        public PlaneType planeType = PlaneType.XY;

        [HideLabel]
        [HideIf(nameof(asVector2D))]
        [JsonProperty]
        public Vector3SetterCore setter3D = new();

        [HideLabel]
        [ShowIf(nameof(asVector2D))]
        [JsonProperty]
        public Vector2Setter setter2D = new();

        #region GUI

        protected override void PreviewValue()
        {
            if (asVector2D)
            {
                contentPreviewing = setter2D.contentPreviewing;
            }
            else
            {
                contentPreviewing = setter3D.contentPreviewing;
            }
        }

        protected override void OnInspectorInit()
        {
            setter3D ??= new();
            setter2D ??= new();

            setter3D.onContentPreviewingChangedAction = PreviewValue;
            setter2D.onContentPreviewingChangedAction = PreviewValue;

            base.OnInspectorInit();
        }

        #endregion

        #region JSON Serialization

        public bool ShouldSerializeplaneType()
        {
            return asVector2D;
        }

        public bool ShouldSerializesetter3D()
        {
            return asVector2D == false;
        }

        public bool ShouldSerializesetter2D()
        {
            return asVector2D;
        }

        #endregion

        public Vector3 GetValue()
        {
            return asVector2D ? setter2D.GetValue().InsertAs(0, planeType) : setter3D.GetValue();
        }

        public Vector3 GetInterpolatedValue(float t)
        {
            return asVector2D ? setter2D.GetInterpolatedValue(t).InsertAs(0, planeType) : 
                setter3D.GetInterpolatedValue(t);
        }

        public static implicit operator Vector3(Vector3Setter setter)
        {
            return setter.GetValue();
        }

        public static implicit operator Vector3Setter(Vector3 fixedVector)
        {
            return new()
            {
                asVector2D = false,
                planeType = PlaneType.XY,
                setter3D = fixedVector
            };
        }

        public static implicit operator Vector3Setter(Vector2 fixedVector)
        {
            return new()
            {
                asVector2D = true,
                planeType = PlaneType.XY,
                setter2D = fixedVector
            };
        }

        public static implicit operator Vector3Setter(float num)
        {
            return new()
            {
                asVector2D = false,
                setter3D = num
            };
        }
    }

    [Serializable]
    public class Vector3SetterCore : NumberOrVectorChooser<Vector3, CubeFloat>
    {
        protected override string valueName => "向量";

        protected override bool valueAlwaysPreviewed => true;

        [LabelText("小数点后显示几位")]
        [MinValue(0)]
        [OnValueChanged("PreviewValue")]
        public int decimalPlaces = 1;

        #region GUI

        protected override string ValueToPreview(Vector3 value)
        {
            return value.ToString(decimalPlaces);
        }

        #endregion

        public static implicit operator Vector3SetterCore(Vector3 fixedVector)
        {
            return new()
            {
                isRandomValue = false,
                value = fixedVector
            };
        }

        public static implicit operator Vector3SetterCore(float num)
        {
            return new()
            {
                isRandomValue = false,
                value = new Vector3(num, num, num)
            };
        }
    }
}

