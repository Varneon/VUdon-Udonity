using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.UdonityLink;
using Varneon.VUdon.Udonity.Windows;
using Varneon.VUdon.Udonity.Windows.Animator;
using Varneon.VUdon.Udonity.Windows.Hierarchy;
using Varneon.VUdon.Udonity.Windows.Inspector;
using Varneon.VUdon.Udonity.Windows.Project;
using Varneon.VUdon.Udonity.Windows.Scene;
using VRC.Udon.Common;

namespace Varneon.VUdon.Udonity
{
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
    }
}
