using UnityEngine;
using UnityEngine.UI;

namespace Varneon.VUdon.Udonity
{
    /// <summary>
    /// Component for describing an Image component that uses a built-in Unity Editor icon
    /// </summary>
    [AddComponentMenu("")]
    [RequireComponent(typeof(Image))]
    public class BuiltinEditorIconImage : MonoBehaviour
    {
        public string IconName => iconName;

        [SerializeField]
        private string iconName = string.Empty;
    }
}
