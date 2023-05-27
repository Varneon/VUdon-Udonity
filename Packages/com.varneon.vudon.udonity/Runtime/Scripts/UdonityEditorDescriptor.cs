using System.Collections.Generic;
using UnityEngine;

namespace Varneon.VUdon.Udonity
{
    /// <summary>
    /// Descriptor for Udonity Editor
    /// </summary>
    [AddComponentMenu("")]
    public class UdonityEditorDescriptor : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        internal List<Transform> hierarchyRoots;
    }
}
