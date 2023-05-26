using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Windows.Project
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ProjectHierarchyElement : UdonSharpBehaviour
    {
        [HideInInspector]
        public string Directory;

        [HideInInspector]
        public ProjectWindow ProjectWindow;

        public void Select()
        {
            ProjectWindow.OnFolderSelected(Directory);
        }
    }
}
