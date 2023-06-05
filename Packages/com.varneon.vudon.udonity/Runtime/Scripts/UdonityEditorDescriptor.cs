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

        [SerializeField, HideInInspector]
        private RectTransform canvasRectTransform;

        private void OnDrawGizmos()
        {
            Vector2 canvasSize = canvasRectTransform.sizeDelta;

            Color originalColor = Gizmos.color;

            Gizmos.color = Color.clear;

            Matrix4x4 originalMatrix = Gizmos.matrix;

            Gizmos.matrix = canvasRectTransform.localToWorldMatrix;

            Gizmos.DrawCube(Vector3.one, new Vector3(canvasSize.x, canvasSize.y, 0f));

            Gizmos.matrix = originalMatrix;

            Gizmos.color = originalColor;
        }
    }
}
