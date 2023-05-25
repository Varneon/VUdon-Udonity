using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    public abstract class SelectableEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        protected Toggle interactableField;

        [SerializeField]
        protected EnumField transitionField;

        [SerializeField]
        protected ObjectField targetGraphicField;

        [SerializeField]
        protected EnumField navigationField;

        public override Type InspectedType => typeof(UnityEngine.UI.Selectable);

        protected UnityEngine.UI.Selectable selectableTarget;

        protected void UpdateSelectableFields()
        {
            interactableField.SetValueWithoutNotify(selectableTarget.interactable);

            transitionField.SetValueWithoutNotify((int)selectableTarget.transition);

            targetGraphicField.SetValueWithoutNotify(selectableTarget.targetGraphic);

            navigationField.SetValueWithoutNotify((int)selectableTarget.navigation.mode);
        }

        protected void RegisterSelectableFieldValueChangedCallbacks()
        {
            if (interactableField) { interactableField.RegisterValueChangedCallback(this, nameof(OnInteractableChanged)); }

            if (transitionField) { transitionField.RegisterValueChangedCallback(this, nameof(OnTransitionChanged)); }

            if (targetGraphicField)
            {
                targetGraphicField.SetFieldType(typeof(UnityEngine.UI.Graphic));

                targetGraphicField.RegisterValueChangedCallback(this, nameof(OnTargetGraphicChanged));
            }

            if (navigationField) { navigationField.RegisterValueChangedCallback(this, nameof(OnNavigationChanged)); }
        }

        public void OnInteractableChanged()
        {
            selectableTarget.interactable = interactableField.Value;
        }

        public void OnTransitionChanged()
        {
            selectableTarget.transition = (UnityEngine.UI.Selectable.Transition)transitionField.Value;
        }

        public void OnTargetGraphicChanged()
        {
            selectableTarget.targetGraphic = (UnityEngine.UI.Graphic)targetGraphicField.Value;
        }

        public void OnNavigationChanged()
        {
            UnityEngine.UI.Navigation navigation = selectableTarget.navigation;

            navigation.mode = (UnityEngine.UI.Navigation.Mode)navigationField.Value;

            selectableTarget.navigation = navigation;
        }

        public void OnSelect()
        {
            selectableTarget.OnSelect(null);
        }

        public void OnDeselect()
        {
            selectableTarget.OnDeselect(null);
        }
    }
}
