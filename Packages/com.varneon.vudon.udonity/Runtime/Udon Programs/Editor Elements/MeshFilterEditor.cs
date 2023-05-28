using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class MeshFilterEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private ObjectField meshField;

        public override Type InspectedType => typeof(MeshFilter);

        private MeshFilter target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (MeshFilter)component;

            meshField.SetFieldType(typeof(Mesh));

            UpdateFields();
        }

        public void UpdateFields()
        {
            meshField.SetValueWithoutNotify(target.mesh);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }
    }
}
