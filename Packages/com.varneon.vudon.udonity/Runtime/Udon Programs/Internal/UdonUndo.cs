using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Enums;

namespace Varneon.VUdon.Udonity
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonUndo : UdonSharpBehaviour
    {
        [SerializeField, Range(8, 128)]
        private int bufferSize = 32;

        /// <summary>
        /// <para>0: (Object) Target</para>
        /// <para>1: (Enum) Target undo type</para>
        /// <para>2: (String) Name</para>
        /// <para>3+: (object[]) Recorded values</para>
        /// </summary>
        private object[][] undoBuffer;

        /// <summary>
        /// Rolling start index of the undo buffer
        /// </summary>
        private int bufferStart;

        /// <summary>
        /// Rolling end index of the undo buffer
        /// </summary>
        private int bufferEnd;

        /// <summary>
        /// Current index of the undo buffer
        /// </summary>
        private int bufferIndex;

        private void Start()
        {
            undoBuffer = new object[bufferSize][];
        }

        [PublicAPI]
        public object BeginRecordObject(Transform transformToUndo, TransformUndoType type)
        {
            return UdonUndoHandler.GetRecordedValues(transformToUndo, type);
        }

        [PublicAPI]
        public void EndRecordObject(Transform transformToUndo, TransformUndoType type, string name, object startValue)
        {
            AddRecord(transformToUndo, (int)type, name, startValue, UdonUndoHandler.GetRecordedValues(transformToUndo, type));
        }

        [PublicAPI]
        public object BeginRecordObject(GameObject gameObjectToUndo, GameObjectUndoType type)
        {
            return UdonUndoHandler.GetRecordedValues(gameObjectToUndo, type);
        }

        [PublicAPI]
        public void EndRecordObject(GameObject gameObjectToUndo, GameObjectUndoType type, string name, object startValue)
        {
            AddRecord(gameObjectToUndo, (int)type, name, startValue, UdonUndoHandler.GetRecordedValues(gameObjectToUndo, type));
        }

        private void AddRecord(Object objectToUndo, int type, string name, object startValue, object endValue)
        {
            if (bufferIndex < bufferEnd) { bufferEnd = bufferIndex; }

            if (++bufferEnd >= bufferStart + bufferSize) { bufferStart++; }

            undoBuffer[bufferIndex++ % bufferSize] = new object[] { objectToUndo, type, name, startValue, endValue };
        }

        [PublicAPI]
        public void PerformUndo()
        {
            if(bufferIndex == bufferStart) { Debug.LogWarning("Reached buffer's start!"); return; }

            object[] record = undoBuffer[--bufferIndex % bufferSize];

            UdonUndoHandler.PerformUndo(record);
        }

        [PublicAPI]
        public void PerformRedo()
        {
            if(bufferIndex == bufferEnd) { Debug.LogWarning("Reached buffer's end!"); return; }

            object[] record = undoBuffer[bufferIndex++ % bufferSize];

            UdonUndoHandler.PerformRedo(record);
        }
    }
}
