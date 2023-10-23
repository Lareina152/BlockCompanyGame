using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;


namespace ConfigurationBasis
{
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [PreviewComposite("@$value.contentPreviewing")]
    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class ValuePreviewBase<T> : BaseConfigClass
    {
        protected virtual bool valueAlwaysPreviewed => false;

        protected bool willPreviewValue = false;

        protected virtual bool showPreviewValueBelow => willPreviewValue;

        [HideLabel]
        [DisplayAsString, PropertyOrder(2001)]
        [ShowIf(nameof(showPreviewValueBelow))]
        public string contentPreviewing = "";

        [NonSerialized]
        public Action onContentPreviewingChangedAction;

        #region GUI

        [Button("刷新预览值")]
        [PropertyOrder(2000)]
        [ShowIf(nameof(showPreviewValueBelow))]
        protected virtual void PreviewValue()
        {
            onContentPreviewingChangedAction?.Invoke();
        }

        protected virtual string ValueToPreview(T value)
        {
            return value.ToString();
        }

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            willPreviewValue = valueAlwaysPreviewed;
            if (willPreviewValue == false)
            {
                contentPreviewing = "";
            }
            else
            {
                PreviewValue();
            }
        }

        #endregion
    }
}

