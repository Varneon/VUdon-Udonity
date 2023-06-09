using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Udonity.Fields;
using VRC.Udon;
using Object = UnityEngine.Object;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class UdonSharpBehaviourEditor : Abstract.ComponentEditor
    {
        [Header("UdonSharpBehaviour EditorElement References")]
        [SerializeField]
        private Image scriptIcon;

        [SerializeField]
        private TextMeshProUGUI headerLabel;

        [SerializeField]
        private ObjectField programScriptField;

        [SerializeField]
        private TextMeshProUGUI programScriptFieldLabel;

        public override Type InspectedType => Type.GetType("UdonSharp.UdonSharpBehaviour");

        private UdonBehaviour target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (UdonBehaviour)component;

            enabledToggle.SetIsOnWithoutNotify(target.enabled);

            //UpdateFields();
        }

        internal void InitializeUdonSharpBehaviourEditor(string fullClassName, bool hasStartMethod, Object programScript, Texture classIcon)
        {
            if(classIcon != null)
            {
                scriptIcon.materialForRendering.mainTexture = classIcon;

                scriptIcon.material = scriptIcon.materialForRendering;

                scriptIcon.sprite = null;
            }

            string className = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);

            headerLabel.text = string.Concat(className, " (U# Script)");

            if (!hasStartMethod) { enabledToggle.gameObject.SetActive(false); }

            programScriptField.SetValueWithoutNotify(programScript);

            programScriptFieldLabel.text = className;
        }

        //public void UpdateFields()
        //{
        //    if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        //}

        protected override void OnToggleEnabled(bool enabled)
        {
            target.enabled = enabled;
        }

        private void OpenInUdonMonitor()
        {
            containingInspector.udonMonitorWindow.OpenUdonBehaviour(target);
        }

        //protected override void OnToggleExpanded(bool expanded)
        //{
        //    if (expanded) { UpdateFields(); }
        //}

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
