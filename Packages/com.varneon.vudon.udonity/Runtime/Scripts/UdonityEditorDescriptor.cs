using System.Collections.Generic;
using UnityEngine;

namespace Varneon.VUdon.Udonity
{
    /// <summary>
    /// Descriptor for Udonity Editor
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class UdonityEditorDescriptor : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        internal List<Transform> hierarchyRoots;

        [SerializeField, HideInInspector]
        internal bool hideRootInclusionTip;
    }
}
