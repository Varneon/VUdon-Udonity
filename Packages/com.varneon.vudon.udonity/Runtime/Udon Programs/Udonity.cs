using TMPro;
using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using Varneon.VUdon.Udonity.UdonityLink;
using Varneon.VUdon.Udonity.Visualizers;
using Varneon.VUdon.Udonity.Windows;
using Varneon.VUdon.Udonity.Windows.Animator;
using Varneon.VUdon.Udonity.Windows.Hierarchy;
using Varneon.VUdon.Udonity.Windows.Inspector;
using Varneon.VUdon.Udonity.Windows.Project;
using Varneon.VUdon.Udonity.Windows.Scene;
using VRC.Udon.Common;

namespace Varneon.VUdon.Udonity
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Udonity : UdonSharpBehaviour
    {
        public GameObject DragAndDropObject => draggedObject;

        public ConsoleWindow ConsoleWindow => consoleWindow;

        public Logger.Abstract.UdonLogger Logger => consoleWindow;

        public Hierarchy Hierarchy => hierarchy;

        public Inspector Inspector => inspector;

        public ProjectWindow ProjectWindow => projectWindow;

        public AnimatorWindow AnimatorWindow => animatorWindow;

        public UdonityLinkClient LinkClient => linkClient;

        public ColorDialog ColorDialog => colorDialog;

        public ObjectDialog ObjectDialog => objectDialog;

        public BoxColliderVisualizer BoxColliderVisualizerPrefab => boxColliderVisualizerPrefab;

        public MeshColliderVisualizer MeshColliderVisualizerPrefab => meshColliderVisualizerPrefab;

        public ColorField ColorFieldPrefab => colorFieldPrefab;

        public Vector3Field Vector3FieldPrefab => vector3FieldPrefab;

        public FloatField FloatFieldPrefab => floatFieldPrefab;

        public FloatField RangedFloatFieldPrefab => rangedFloatFieldPrefab;

        public ObjectField ObjectFieldPrefab => objectFieldPrefab;

        internal Transform[] roots = new Transform[0];

        [Header("Windows")]
        [SerializeField]
        private Hierarchy hierarchy;

        [SerializeField]
        private Inspector inspector;

        [SerializeField]
        private ProjectWindow projectWindow;

        [SerializeField]
        private AnimatorWindow animatorWindow;

        [SerializeField]
        private ConsoleWindow consoleWindow;

        [Header("Utilities")]
        [SerializeField]
        private Handle handle;

        [SerializeField]
        private SceneViewCamera sceneViewCamera;

        [SerializeField]
        private ColorDialog colorDialog;

        [SerializeField]
        private ObjectDialog objectDialog;

        [SerializeField]
        private UnityEngine.UI.Toggle recursiveHighlightingToggle;

        [SerializeField]
        private UdonityLinkClient linkClient;

        [Header("Window Tabs")]
        [SerializeField]
        private WindowTab projectWindowTab;

        [SerializeField]
        private WindowTab animatorWindowTab;

        [Header("Dynamic Texts")]
        [SerializeField]
        private TextMeshProUGUI editorTitleText;

        [SerializeField]
        private TextMeshProUGUI sceneHeaderText;

        [SerializeField]
        private TextMeshProUGUI aboutWindowBannerText;

        [SerializeField]
        private TextMeshProUGUI notRespondingTitleText;

        [Header("Runtime Prefabs")]
        [SerializeField]
        private BoxColliderVisualizer boxColliderVisualizerPrefab;

        [SerializeField]
        private MeshColliderVisualizer meshColliderVisualizerPrefab;

        [SerializeField]
        private ColorField colorFieldPrefab;

        [SerializeField]
        private Vector3Field vector3FieldPrefab;

        [SerializeField]
        private FloatField floatFieldPrefab;

        [SerializeField]
        private FloatField rangedFloatFieldPrefab;

        [SerializeField]
        private ObjectField objectFieldPrefab;

        private GameObject draggedObject;

        #region Toolbar
        public void SetToolModeHand() { ResetHandleType(); }

        public void SetToolModeMove() { handle.SetHandleType(Enums.HandleType.Position); }

        public void SetToolModeRotate() { handle.SetHandleType(Enums.HandleType.Rotation); }

        public void SetToolModeScale() { handle.SetHandleType(Enums.HandleType.Scale); }

        public void SetToolModeRect() { ResetHandleType(); }

        public void SetToolModeMRS() { ResetHandleType(); }
        
        public void SetToolModeCustom() { ResetHandleType(); }

        public void SetToolHandleRotationGlobal() { handle.SpaceMode = Space.World; }

        public void SetToolHandleRotationLocal() { handle.SpaceMode = Space.Self; }

        public void SetGridSnappingOn() { handle.SetGridSnappingEnabled(true); }

        public void SetGridSnappingOff() { handle.SetGridSnappingEnabled(false); }

        public void CreateEmpty() { hierarchy.CreateEmpty(); }

        public void CreateEmptyChild() { }

        public void DuplicateObject() { hierarchy.DuplicateObject(); }

        public void DeleteObject() { hierarchy.DeleteObject(); }

        public void ToggleRecursiveHighlighting()
        {
            hierarchy.SetRecursiveHighlightingEnabled(recursiveHighlightingToggle.isOn);
        }

        private void ResetHandleType()
        {
            handle.SetHandleType(Enums.HandleType.None);
        }
        #endregion

        private GameObject GetSelectedHierarchyObject()
        {
            HierarchyElement activeHierarchyElement = hierarchy.ActiveHierarchyElement;

            if (activeHierarchyElement == null) { return null; }

            return activeHierarchyElement.Target;
        }

        public void FrameSelected()
        {
            GameObject target = GetSelectedHierarchyObject();

            if(target == null) { return; }

            sceneViewCamera.FrameTransform(target.transform);
        }

        public void LockViewToSelected()
        {
            GameObject target = GetSelectedHierarchyObject();

            if (target == null) { return; }

            sceneViewCamera.LockViewToSelected(target.transform);
        }

        internal void OnGameObjectSelected(GameObject selectedGameObject)
        {
            sceneViewCamera.ReleaseFollow();
        }

        public void MoveToView()
        {
            GameObject target = GetSelectedHierarchyObject();

            if (target == null) { return; }

            sceneViewCamera.MoveToView(target.transform);
        }

        public void AlignWithView()
        {
            GameObject target = GetSelectedHierarchyObject();

            if (target == null) { return; }

            sceneViewCamera.AlignWithView(target.transform);
        }

        public void SelectAssetInProjectWindow(Object asset)
        {
            projectWindowTab.OnClicked();

            projectWindow.PingAsset(asset);
        }

        public void SelectAnimatorInAnimatorWindow(Animator animator)
        {
            animatorWindowTab.OnClicked();
        }

        public void SetDraggedObject(GameObject go)
        {
            draggedObject = go;
        }

        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            if (!value)
            {
                if (draggedObject) { draggedObject = null; }
            }
        }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild(string sceneName, string version)
        {
            GameObject runtimePrefabRoot = new GameObject("RUNTIME_PREFABS");
            runtimePrefabRoot.SetActive(false);
            Transform runtimePrefabParent = runtimePrefabRoot.transform;
            runtimePrefabParent.SetParent(transform.parent);

            boxColliderVisualizerPrefab = Instantiate(boxColliderVisualizerPrefab, runtimePrefabParent);
            meshColliderVisualizerPrefab = Instantiate(meshColliderVisualizerPrefab, runtimePrefabParent);
            colorFieldPrefab = Instantiate(colorFieldPrefab, runtimePrefabParent);
            vector3FieldPrefab = Instantiate(vector3FieldPrefab, runtimePrefabParent);
            floatFieldPrefab = Instantiate(floatFieldPrefab, runtimePrefabParent);
            rangedFloatFieldPrefab = Instantiate(rangedFloatFieldPrefab, runtimePrefabParent);
            objectFieldPrefab = Instantiate(objectFieldPrefab, runtimePrefabParent);

            editorTitleText.text = editorTitleText.text.Replace("{{scene}}", sceneName).Replace("{{version}}", version);
            sceneHeaderText.text = sceneName;
            aboutWindowBannerText.text = aboutWindowBannerText.text.Replace("{{version}}", version);
            notRespondingTitleText.text = notRespondingTitleText.text.Replace("{{version}}", version);
        }
#endif
    }
}
