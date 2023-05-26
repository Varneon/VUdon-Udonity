using UnityEngine;

namespace Varneon.VUdon.Udonity
{
    /// <summary>
    /// Descriptor for Udonity Editor
    /// </summary>
    [AddComponentMenu("")]
    public class UdonityEditorDescriptor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Root transforms that will be inspected in the hierarchy")]
        internal Transform[] hierarchyRoots;
    }
}
