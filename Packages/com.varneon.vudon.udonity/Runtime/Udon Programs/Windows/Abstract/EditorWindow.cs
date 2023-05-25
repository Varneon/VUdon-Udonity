using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Windows.Abstract
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class EditorWindow : UdonSharpBehaviour
    {
        public abstract Vector2 MinSize { get; }

        public void SetWindowActive(bool active)
        {
            OnWindowActiveStateChanged(active);

            gameObject.SetActive(active);
        }

        protected virtual void OnWindowActiveStateChanged(bool active) { }
    }
}
