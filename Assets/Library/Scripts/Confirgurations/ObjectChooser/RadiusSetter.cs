using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using ConfigurationBasis;
using UnityEngine;

namespace ConfigurationBasis
{
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [JsonObject(MemberSerialization.OptIn)]
    [PreviewComposite("@$value.radius.contentPreviewing")]
    public class RadiusSetter : ValuePreviewBase<float>, IValueSetter<float>
    {
        protected override bool valueAlwaysPreviewed => true;

        protected override bool showPreviewValueBelow => false;

        [LabelText("半径")]
        [JsonProperty]
        public FloatSetter radius = new();

        [LabelText("距离种类")]
        [JsonProperty]
        public EnumSetter<DistanceType> distanceType = DistanceType.Manhattan;

        #region GUI

        protected override void PreviewValue()
        {
            contentPreviewing = radius.contentPreviewing + distanceType.contentPreviewing;
        }

        protected override void OnInspectorInit()
        {
            radius ??= new();
            distanceType ??= new();

            radius.onContentPreviewingChangedAction = PreviewValue;
            distanceType.onContentPreviewingChangedAction = PreviewValue;

            base.OnInspectorInit();
        }

        #endregion

        public void Reset()
        {
            radius.Reset();
        }

        public float GetValue()
        {
            return radius;
        }

        public static implicit operator float(RadiusSetter setter)
        {
            return setter.GetValue();
        }

        public static implicit operator RadiusSetter(float fixedRadius)
        {
            return new()
            {
                radius = fixedRadius,
                distanceType = DistanceType.Manhattan
            };
        }
    }
}

