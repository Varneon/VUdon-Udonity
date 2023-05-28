using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectDialog : Abstract.ApplicationWindow
    {
        [SerializeField]
        private GameObject listElement;

        [SerializeField]
        private RectTransform listRoot;

        [SerializeField]
        private TextMeshProUGUI headerTitle;

        private ObjectField activeObjectField;

        public void OpenWithType(ObjectField field, Type type)
        {
            headerTitle.text = string.Format("Select {0}", type.Name);

            activeObjectField = field;

            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (activeObjectField) { activeObjectField.EndObjectDialogSelection(); }

            gameObject.SetActive(false);
        }
    }
}
