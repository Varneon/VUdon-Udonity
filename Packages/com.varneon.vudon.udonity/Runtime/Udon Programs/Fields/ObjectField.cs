﻿using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Varneon.VUdon.Udonity.Fields
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectField : Abstract.Field
    {
        public override object AbstractValue => _value;

        public override Type FieldType => typeof(Object);

        [SerializeField]
        private TextMeshProUGUI valueLabel;

        public Object Value { get => _value; set { SetValueWithoutNotify(value); OnValueChanged(); } }

        private Object _value;

        //[SerializeField, HideInInspector]
        private System.Type fieldType = typeof(Object);

        public string FieldTypeNameOverride;

        private bool lockForDialogEditing;

        [SerializeField, HideInInspector]
        private Udonity udonity;

        public void SetValueWithoutNotify(Object value)
        {
            if (lockForDialogEditing) { return; }

            //Debug.Log(valueLabel);
            //Debug.Log(fieldType);
            //Debug.Log(value);

            if(_value != value)
            {
                _value = value;

                UpdateValueLabel();
            }
        }

        private void UpdateValueLabel()
        {
            bool hasCustomTypeName = string.IsNullOrWhiteSpace(FieldTypeNameOverride);

            if (_value == null)
            {
                valueLabel.text = $"None ({(hasCustomTypeName ? fieldType.Name : FieldTypeNameOverride)})";
            }
            else
            {
                valueLabel.text = $"{_value.name} ({(hasCustomTypeName ? (fieldType.Equals(typeof(Object)) ? _value.GetType().Name : fieldType.Name) : FieldTypeNameOverride)})";
            }
        }

        public void SetFieldType(System.Type type)
        {
            fieldType = type ?? typeof(Object);

            UpdateValueLabel();
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
            if(_value == null) { return; }

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

        public override bool TrySetAbstractValueWithoutNotify(object value)
        {
            if (value != null && !value.GetType().Equals(FieldType)) { return false; }

            SetValueWithoutNotify((Object)value);

            return true;
        }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            udonity = GetComponentInParent<Udonity>();
        }
#endif
    }
}
