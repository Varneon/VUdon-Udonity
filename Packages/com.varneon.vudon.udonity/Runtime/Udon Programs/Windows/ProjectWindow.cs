using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;

namespace Varneon.VUdon.Udonity.Windows.Project
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ProjectWindow : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(600f, 200f);

        [SerializeField, HideInInspector]
        internal UdonAssetDatabase.UdonAssetDatabase assetDatabase;

        [SerializeField]
        private Logger.Abstract.UdonLogger logger;

        [SerializeField]
        [FormerlySerializedAs("HierarchyContainer")]
        internal RectTransform hierarchyContainer;

        [SerializeField]
        [FormerlySerializedAs("HierarchyElement")]
        internal GameObject hierarchyElement;

        [SerializeField]
        private RectTransform folderContainer;

        [SerializeField]
        private GameObject assetElement;

        [SerializeField]
        private GameObject folderElement;

        [SerializeField]
        private TextMeshProUGUI folderDirectoryLabel;

        [HideInInspector]
        public string folderHierarchies;

        internal void OnFolderSelected(string directory)
        {
            // Apply the directory to the toolbar label on the folder container
            folderDirectoryLabel.text = directory.Replace("/", " <color=#777>></color> ");

            // Clear the contents of the folder container
            ClearFolderContainer();

            // Get list of assets in the directory
            string[] files = assetDatabase.GetFilesInDirectory(directory);

            int directoryCharCount = directory.Length;

            string addedFoldersLookup = "\n";

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];

                int nextSlashIndex = file.IndexOf('/', directoryCharCount + 1);

                string fileDirectory = nextSlashIndex == -1 ? string.Empty : file.Substring(directoryCharCount + 1, nextSlashIndex - directoryCharCount - 1);

                if (string.IsNullOrEmpty(fileDirectory))
                {
                    GameObject newAssetElement = Instantiate(assetElement, folderContainer, false);

                    newAssetElement.GetComponentInChildren<TextMeshProUGUI>().text = file.Substring(directoryCharCount + 1);
                }
                else if (fileDirectory.IndexOf('/') == -1 && addedFoldersLookup.IndexOf(fileDirectory) == -1)
                {
                    GameObject newFolderElement = Instantiate(folderElement, folderContainer, false);

                    newFolderElement.GetComponentInChildren<TextMeshProUGUI>().text = fileDirectory;

                    addedFoldersLookup += $"{fileDirectory}\n";
                }
            }
        }

        /// <summary>
        /// Destroys every object in the folder container
        /// </summary>
        private void ClearFolderContainer()
        {
            for(int i = 0; i < folderContainer.childCount; i++)
            {
                Destroy(folderContainer.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Pings an object in the project window if it exists in the UdonAssetDatabase
        /// </summary>
        /// <param name="asset"></param>
        internal void PingAsset(Object asset)
        {
            string path = assetDatabase.GetAssetPath(asset);

            if (string.IsNullOrWhiteSpace(path)) { return; }

            OnFolderSelected(path.Substring(0, path.LastIndexOf('/')));
        }
    }
}
