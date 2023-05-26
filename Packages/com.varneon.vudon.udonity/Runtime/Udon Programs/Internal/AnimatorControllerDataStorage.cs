using UdonSharp;
using UnityEngine;
using Varneon.VUdon.ArrayExtensions;

namespace Varneon.VUdon.Udonity
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AnimatorControllerDataStorage : UdonSharpBehaviour
    {
        [SerializeField]
        internal RuntimeAnimatorController[] runtimeAnimatorControllerLookup;

        [SerializeField]
        internal string[][] parameterNames;

        [SerializeField]
        internal int[][] parameterTypes;

        [SerializeField]
        internal string[][] layerNames;

        [SerializeField]
        internal int[][] layerBlendingModes;

        public bool TryGetControllerIndex(RuntimeAnimatorController controller, out int index)
        {
            if(controller == null) { index = 0; return false; }

            index = runtimeAnimatorControllerLookup.IndexOf(controller);

            return index >= 0;
        }

        public string[] GetControllerParameterNames(int index)
        {
            return parameterNames[index];
        }

        public int[] GetControllerParameterTypes(int index)
        {
            return parameterTypes[index];
        }

        public string[] GetControllerLayerNames(int index)
        {
            return layerNames[index];
        }

        public int[] GetControllerLayerBlendingModes(int index)
        {
            return layerBlendingModes[index];
        }
    }
}
