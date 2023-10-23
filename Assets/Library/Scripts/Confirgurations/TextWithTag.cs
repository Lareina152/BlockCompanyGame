using System;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;


namespace ConfigurationBasis
{
    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    public class TextWithTag : ValuePreviewBase<string>
    {
        protected override bool valueAlwaysPreviewed => true;
        protected override bool showPreviewValueBelow => false;

        [HideLabel]
        [OnValueChanged(nameof(PreviewValue), true)]
        public StringTranslation text = new();

        [HideLabel]
        [OnValueChanged(nameof(PreviewValue), true)]
        public TextTagFormat tagFormat = new();

        #region GUI

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            tagFormat ??= new();
        }

        protected override void PreviewValue()
        {
            base.PreviewValue();

            contentPreviewing = text;
        }

        #endregion

        [Obsolete]
        public string GetText(float percent)
        {
            return tagFormat.GetText(text);
        }

        public string GetText()
        {
            return tagFormat.GetText(text);
        }

        public bool IsEmpty()
        {
            return text.IsEmpty();
        }

        #region Conversion

        public static implicit operator TextWithTag(string newText)
        {
            return new()
            {
                text = newText
            };
        }

        #endregion
    }
}
