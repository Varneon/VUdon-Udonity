using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ColorField : Abstract.Field
    {
        public bool HasAlpha => hasAlpha;

        public bool IsHDR => isHDR;

        [SerializeField]
        private Image colorPreview;

        [SerializeField]
        private Image colorAlphaFill;

        [SerializeField, HideInInspector]
        private RectTransform colorAlphaFillRectTransform;

        [SerializeField, HideInInspector]
        private bool hasAlpha;

        [SerializeField]
        private bool isHDR;

        public Color Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private Color _value;

        private bool lockForDialogEditing;

        public void SetValueWithoutNotify(Color value)
        {
            if (lockForDialogEditing) { return; }

            _value = value;

            // Temporary method of detecting HDR colors
            if(_value.maxColorComponent > 1f) { isHDR = true; } // TODO: Scan fields for attributes

            //colorPreview.color = value;
            UpdateColorPreview();
        }

        internal void ApplyColorDialogValue(Color value)
        {
            _value = value;

            Value = value;

            //colorPreview.color = value;
            UpdateColorPreview();
        }

        public void EditColor()
        {
            GetComponentInParent<Udonity>().ColorDialog.OpenWithField(this);

            lockForDialogEditing = true;
        }

        private void UpdateColorPreview()
        {
            Color color = _value;

            color.a = 1f;

            colorPreview.color = color;

            if (hasAlpha)
            {
                colorAlphaFillRectTransform.localScale = new Vector3(_value.a, 1f, 1f);
            }
        }

        internal void EndColorEditing()
        {
            lockForDialogEditing = false;
        }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            if (colorAlphaFill) { colorAlphaFillRectTransform = colorAlphaFill.rectTransform; }

            hasAlpha = colorAlphaFillRectTransform;
        }
#endif
    }
}
