using System;
using TMPro;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    public class DefaultComponentEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private TextMeshProUGUI nameText;

        public override Type InspectedType => typeof(Component);

        protected override void OnEditorInitialized(Component component)
        {
            if(component == null) { return; }

            Type type = component.GetType();

            string fullName = type.FullName;

            nameText.text = fullName.Contains(".") ? string.Format("<color=#888>{0}</color>.{1}", fullName.Substring(0, fullName.LastIndexOf('.')), type.Name) : fullName;
        }

        internal void OverrideTypeName(string typeName)
        {
            nameText.text = $"{typeName} <color=#844>{nameText.text}</color>";
        }
    }
}
