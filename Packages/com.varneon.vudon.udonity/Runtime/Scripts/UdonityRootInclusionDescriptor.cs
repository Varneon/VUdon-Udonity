using UnityEngine;

namespace Varneon.VUdon.Udonity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("VUdon/Udonity/Udonity Root Inclusion Descriptor")]
    public class UdonityRootInclusionDescriptor : MonoBehaviour { }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [UnityEditor.CustomEditor(typeof(UdonityRootInclusionDescriptor))]
    public class UdonityRootInclusionDescriptorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("This object hierarchy root will be included in Udonity for inspection on build.", UnityEditor.MessageType.Info);
        }
    }
#endif
}
