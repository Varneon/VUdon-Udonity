using UnityEngine;
using Varneon.VUdon.Udonity.Windows.Scene;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    public class SceneView : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(600f, 400f);

        [SerializeField]
        private SceneViewCamera sceneViewCamera;

        protected override void OnWindowActiveStateChanged(bool active)
        {
            sceneViewCamera.gameObject.SetActive(active);
        }
    }
}
