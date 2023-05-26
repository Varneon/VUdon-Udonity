using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Editors.Abstract;
using VRC.Udon;

namespace Varneon.VUdon.Udonity.Utilities
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CrashWatcher : UdonSharpBehaviour
    {
        [SerializeField]
        private UdonBehaviour[] internalBehaviours;

        [SerializeField, Range(0f, 10f)]
        private float scanInterval = 1f;

        [SerializeField]
        private GameObject crashDialog;

        [SerializeField]
        private GameObject criticalErrorDialog;

        [SerializeField]
        private Windows.Inspector.Inspector inspector;

        private ComponentEditor[] componentEditors = new ComponentEditor[0];

        private void Start()
        {
            ScanBehavioursForCrashes();
        }

        public void ScanBehavioursForCrashes()
        {
            for(int i = 0; i < internalBehaviours.Length; i++)
            {
                UdonBehaviour behaviour = internalBehaviours[i];

                if(behaviour == null) { Debug.LogError($"[CrashWatcher]: Behaviour at index ({i}) is null!"); continue; }

                if (!behaviour.enabled) { crashDialog.SetActive(true); return; }
            }

            foreach (ComponentEditor editor in componentEditors)
            {
                if(editor == null) { continue; }

                if (!editor.enabled)
                {
                    EnableCriticalErrorDialog();

                    inspector.OnEditorCrash(editor);
                }
            }

            SendCustomEventDelayedSeconds(nameof(ScanBehavioursForCrashes), scanInterval);
        }

        internal void SetComponentEditors(ComponentEditor[] editors)
        {
            componentEditors = editors;
        }

        public void EnableCriticalErrorDialog()
        {
            criticalErrorDialog.SetActive(true);

            SendCustomEventDelayedSeconds(nameof(DisableCriticalErrorDialog), 2f);
        }

        public void DisableCriticalErrorDialog()
        {
            criticalErrorDialog.SetActive(false);
        }
    }
}
