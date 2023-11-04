using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity
{
    /// <summary>
    /// Component for describing an Image component that uses a built-in Unity Editor icon
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class BuiltinEditorIconImage : MonoBehaviour
    {
#if UNITY_2022_3_OR_NEWER
        public string IconName => iconName.Replace('-', ' ');
#else
        public string IconName => iconName;
#endif

        [SerializeField]
        private string iconName = string.Empty;
    }
}
