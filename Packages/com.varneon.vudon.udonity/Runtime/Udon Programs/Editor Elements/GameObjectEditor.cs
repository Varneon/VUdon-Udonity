using UdonSharp;
using UnityEditor;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using Varneon.VUdon.Udonity.Windows.Hierarchy;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class GameObjectEditor : UdonSharpBehaviour
    {
        [SerializeField]
        private TextField nameField;

        [SerializeField]
        private EnumField layerField;

        [SerializeField]
        private Toggle activeToggle;

        [SerializeField]
        private Toggle staticToggle;

        private GameObject target;

        private HierarchyElement linkedElement;

        internal void Initialize(HierarchyElement hierarchyElement)
        {
            gameObject.SetActive(true);

            linkedElement = hierarchyElement;

            target = hierarchyElement.Target;

            nameField.SetValueWithoutNotify(target.name);

            activeToggle.SetValueWithoutNotify(target.activeSelf);

            layerField.SetValueWithoutNotify(target.layer);

            staticToggle.SetValueWithoutNotify(target.isStatic);

            //EnableTargetHighlight();
        }

        //private void OnDestroy()
        //{
        //    SetTargetHiglightedState(false);
        //}

        //private void SetTargetHiglightedState(bool highlighted)
        //{
        //    foreach(Renderer renderer in target.GetComponentsInChildren<Renderer>(true))
        //    {
        //        InputManager.EnableObjectHighlight(renderer.gameObject, highlighted);
        //    }
        //}

        //public void EnableTargetHighlight()
        //{
        //    SetTargetHiglightedState(true);

        //    SendCustomEventDelayedSeconds(nameof(EnableTargetHighlight), 1f);
        //}

        public void OnNameChanged()
        {
            string name = nameField.Value;

            target.name = name;

            linkedElement.SetName(name);
        }

        public void OnActiveChanged()
        {
            linkedElement.SetActive(activeToggle.Value);
        }

        public void OnLayerChanged()
        {
            target.layer = layerField.Value;
        }

        public void ToggleActive()
        {
            linkedElement.SetActive(activeToggle.Value);
        }

        internal void SetActive(bool active)
        {
            activeToggle.SetValueWithoutNotify(active);
        }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            if (nameField) { nameField.RegisterValueChangedCallback(this, nameof(OnNameChanged)); }

            if (activeToggle) { activeToggle.RegisterValueChangedCallback(this, nameof(OnActiveChanged)); }

            if (layerField) { layerField.RegisterValueChangedCallback(this, nameof(OnLayerChanged)); }
        }
#endif
    }
}
