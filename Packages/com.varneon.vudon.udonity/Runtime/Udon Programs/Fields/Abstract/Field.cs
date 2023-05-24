using System;
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

namespace Varneon.VUdon.Udonity.Fields.Abstract
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class Field : UdonSharpBehaviour
    {
        public bool Interactive;

        [SerializeField, HideInInspector]
        private UdonSharpBehaviour valueChangedCallbackReceiver;

        [SerializeField, HideInInspector]
        private string valueChangedCallbackMethod;

        /// <summary>
        /// ID for identifying the field
        /// </summary>
        [SerializeField, HideInInspector]
        private int fieldId = -1;

        public void RegisterValueChangedCallback(UdonSharpBehaviour callbackReceiver, string methodName)
        {
            valueChangedCallbackReceiver = callbackReceiver;

            valueChangedCallbackMethod = methodName;
        }

        public void RegisterValueChangedCallback(UdonSharpBehaviour callbackReceiver, string methodName, int id)
        {
            fieldId = id;

            RegisterValueChangedCallback(callbackReceiver, methodName);
        }

        public void OnValueChanged()
        {
            if (valueChangedCallbackReceiver != null)
            {
                if(fieldId >= 0)
                {
                    valueChangedCallbackReceiver.SetProgramVariable("activeFieldId", fieldId);
                }

                valueChangedCallbackReceiver.SendCustomEvent(valueChangedCallbackMethod);
            }
        }
    }
}
