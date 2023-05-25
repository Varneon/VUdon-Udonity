using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using VRC.SDK3.Components;

namespace Varneon.VUdon.Udonity.Editors
{
    public class VRCAvatarPedestalEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private TextField blueprintIdField;

        [SerializeField]
        private Toggle changeAvatarsOnUseField;

        [SerializeField]
        private ObjectField placementField;

        [SerializeField]
        private FloatField scaleField;

        public override Type InspectedType => typeof(VRCAvatarPedestal);

        private VRCAvatarPedestal target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (VRCAvatarPedestal)component;

            placementField.SetFieldType(typeof(Transform));

            placementField.SetValueWithoutNotify(target.Placement);

            UpdateFields();
        }

        public void UpdateFields()
        {
            blueprintIdField.SetValueWithoutNotify(target.blueprintId);

            changeAvatarsOnUseField.SetValueWithoutNotify(target.ChangeAvatarsOnUse);

            scaleField.SetValueWithoutNotify(target.scale);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnBlueprintIdChanged()
        {
            target.blueprintId = blueprintIdField.Value;
        }

        public void OnChangeAvatarsOnUseChanged()
        {
            target.ChangeAvatarsOnUse = changeAvatarsOnUseField.Value;
        }

        public void OnScaleChanged()
        {
            target.scale = scaleField.Value;
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
            if (blueprintIdField) { blueprintIdField.RegisterValueChangedCallback(this, nameof(OnBlueprintIdChanged)); }

            if (changeAvatarsOnUseField) { changeAvatarsOnUseField.RegisterValueChangedCallback(this, nameof(OnChangeAvatarsOnUseChanged)); }

            if (scaleField) { scaleField.RegisterValueChangedCallback(this, nameof(OnScaleChanged)); }
        }
    }
}
