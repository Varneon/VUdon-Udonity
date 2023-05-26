using TMPro;
using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectField : Abstract.Field
    {
        [SerializeField]
        private TextMeshProUGUI valueLabel;

        public Object Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private Object _value;

        //[SerializeField, HideInInspector]
        private System.Type fieldType = typeof(Object);

        private bool lockForDialogEditing;

        [SerializeField, HideInInspector]
        private Udonity udonity;

        public void SetValueWithoutNotify(Object value)
        {
            if (lockForDialogEditing) { return; }

            //Debug.Log(valueLabel);
            //Debug.Log(fieldType);
            //Debug.Log(value);

            _value = value;

            valueLabel.text = _value == null ? $"None ({fieldType.Name})" : _value.ToString();
        }

        public void SetFieldType(System.Type type)
        {
            fieldType = type;
        }

        internal void ApplyObjectDialogValue(Object value)
        {
            _value = value;

            Value = value;

            valueLabel.text = _value == null ? $"None ({fieldType.Name})" : _value.ToString();
        }

        public void OpenObjectDialog()
        {
            GetComponentInParent<Udonity>().ObjectDialog.OpenWithType(this, fieldType);

            lockForDialogEditing = true;
        }

        public void PingObject()
        {
            GetComponentInParent<Udonity>().SelectAssetInProjectWindow(_value);
        }

        public void OnEndDrag()
        {
            GameObject draggedObject = udonity.DragAndDropObject;

            if(draggedObject == null) { udonity.Logger.LogWarning("Attempted to drag null object!"); return; }

            udonity.Logger.Log($"OnEndDrag: {draggedObject}");

            if (fieldType == typeof(GameObject))
            {
                Value = draggedObject;
            }
            else
            {
                Component component = draggedObject.GetComponent(fieldType);

                if(component == null) { udonity.Logger.LogWarning($"Component of type '{fieldType.Name}' doesnt exist on the dragged object!"); return; }

                Value = component;
            }
        }

        internal void EndObjectDialogSelection()
        {
            lockForDialogEditing = false;
        }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            udonity = GetComponentInParent<Udonity>();
        }
#endif
    }
}
