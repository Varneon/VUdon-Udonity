using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using VRC.SDKBase;
using VRC.Udon;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class UdonBehaviourEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private Toggle disableInteractiveField;

        [SerializeField]
        private TextField interactionTextField;

        public override Type InspectedType => typeof(UdonBehaviour);

        private UdonBehaviour target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (UdonBehaviour)component;

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            disableInteractiveField.RegisterValueChangedCallback(this, nameof(OnDisableInteractiveChanged));

            interactionTextField.RegisterValueChangedCallback(this, nameof(OnInteractionTextChanged));

            UpdateFields();
        }

        public void UpdateFields()
        {
            //NOT EXPOSED:
            //UdonBehaviour.SyncIsContinuous
            //UdonBehaviour.SyncIsManual
            //UdonBehaviour.SyncMethod
            //UdonBehaviour.programSource

            disableInteractiveField.SetValueWithoutNotify(target.DisableInteractive);

            interactionTextField.SetValueWithoutNotify(target.InteractionText);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        protected override void OnToggleEnabled(bool enabled)
        {
            target.enabled = enabled;
        }

        public void OnDisableInteractiveChanged()
        {
            target.DisableInteractive = disableInteractiveField.Value;
        }

        public void OnInteractionTextChanged()
        {
            target.InteractionText = interactionTextField.Value;
        }

        public void OnInteract()
        {
            target.SendCustomEvent("_interact");
        }

        public void OnTakeOwnership()
        {
            Networking.SetOwner(Networking.LocalPlayer, target.gameObject);
        }

        private void OpenInUdonMonitor()
        {
            containingInspector.udonMonitorWindow.OpenUdonBehaviour(target);
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }

        protected override void OnContextDropdownActionInvoked(int index)
        {
            base.OnContextDropdownActionInvoked(index);

            switch (index)
            {
                case 1:
                    OpenInUdonMonitor();
                    break;
            }
        }
    }
}
