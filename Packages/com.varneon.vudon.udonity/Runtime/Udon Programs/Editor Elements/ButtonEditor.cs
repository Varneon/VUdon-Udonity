using System;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Editors
{
    public class ButtonEditor : SelectableEditor
    {
        public override Type InspectedType => typeof(UnityEngine.UI.Button);

        private UnityEngine.UI.Button target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (UnityEngine.UI.Button)component;

            selectableTarget = target;

            if (Expanded) { UpdateFields(); }
        }

        public void UpdateFields()
        {
            UpdateSelectableFields();

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }

        public void OnClick()
        {
            target.OnSubmit(null);
        }

        protected override void OnInitializedOnBuild()
        {
            RegisterSelectableFieldValueChangedCallbacks();
        }
    }
}
