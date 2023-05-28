using System;
using TMPro;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class TextMeshProUGUIEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private TextField textField;

        [SerializeField]
        private ColorField colorField;

        public override Type InspectedType => typeof(TextMeshProUGUI);

        private TextMeshProUGUI target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (TextMeshProUGUI)component;

            UpdateFields();
        }

        public void UpdateFields()
        {
            textField.SetValueWithoutNotify(target.text);

            colorField.SetValueWithoutNotify(target.color);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnTextChanged()
        {
            target.text = textField.Value;
        }

        public void OnColorChanged()
        {
            target.color = colorField.Value;
        }

        protected override void OnToggleEnabled(bool enabled)
        {
            target.enabled = enabled;
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }

        protected override void OnInitializedOnBuild()
        {
            if (textField) { textField.RegisterValueChangedCallback(this, nameof(OnTextChanged)); }

            if (colorField) { colorField.RegisterValueChangedCallback(this, nameof(OnColorChanged)); }
        }
    }
}
