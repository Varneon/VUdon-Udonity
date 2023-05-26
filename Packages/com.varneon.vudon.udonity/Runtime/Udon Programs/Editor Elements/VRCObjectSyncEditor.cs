using System;
using TMPro;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    public class VRCObjectSyncEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private Toggle allowCollisionOwnershipTransferField;

        [SerializeField]
        private UnityEngine.UI.Button takeOwnershipButton;

        [SerializeField]
        private TextMeshProUGUI takeOwnershipButtonLabel;

        [SerializeField]
        private Toggle gravityField;

        [SerializeField]
        private Toggle kinematicField;

        public override Type InspectedType => typeof(VRCObjectSync);

        private VRCObjectSync target;

        private Rigidbody rb;

        private bool hasRigidbody;

        protected override void OnEditorInitialized(Component component)
        {
            target = (VRCObjectSync)component;

            rb = target.GetComponent<Rigidbody>();

            hasRigidbody = rb;

            if (!hasRigidbody)
            {
                gravityField.gameObject.SetActive(false);

                kinematicField.gameObject.SetActive(false);
            }

            UpdateFields();
        }

        public void UpdateFields()
        {
            allowCollisionOwnershipTransferField.SetValueWithoutNotify(target.AllowCollisionOwnershipTransfer);

            if (hasRigidbody)
            {
                gravityField.SetValueWithoutNotify(rb.useGravity);

                kinematicField.SetValueWithoutNotify(rb.isKinematic);
            }

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnAllowCollisionOwnershipTransferChanged()
        {
            target.AllowCollisionOwnershipTransfer = allowCollisionOwnershipTransferField.Value;
        }

        public void OnTakeOwnership()
        {
            Networking.SetOwner(Networking.LocalPlayer, target.gameObject);
        }

        public void OnKinematicChanged()
        {
            target.SetKinematic(kinematicField.Value);
        }

        public void OnGravityChanged()
        {
            target.SetGravity(gravityField.Value);
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }

        protected override void OnInitializedOnBuild()
        {
            if (allowCollisionOwnershipTransferField) { allowCollisionOwnershipTransferField.RegisterValueChangedCallback(this, nameof(OnAllowCollisionOwnershipTransferChanged)); }

            if (gravityField) { gravityField.RegisterValueChangedCallback(this, nameof(OnGravityChanged)); }

            if (kinematicField) { kinematicField.RegisterValueChangedCallback(this, nameof(OnKinematicChanged)); }
        }
    }
}
