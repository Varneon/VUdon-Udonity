using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class WindowSubLayoutElement : WindowLayoutElement
    {
        [SerializeField]
        private Vector2 elementMinSize = Vector2.zero;

        private void Start()
        {
            minSize = elementMinSize;
        }
    }
}
