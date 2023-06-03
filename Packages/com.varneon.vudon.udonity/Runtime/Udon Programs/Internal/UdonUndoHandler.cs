using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Enums;

namespace Varneon.VUdon.Udonity
{
    public static class UdonUndoHandler
    {
        internal static void PerformUndo(object[] record)
        {
            object objectToUndo = record[0];

            if (objectToUndo == null) { return; }

            ApplyUndoValues(objectToUndo, (int)record[1], record[3]);
        }

        internal static void PerformRedo(object[] record)
        {
            object objectToRedo = record[0];

            if (objectToRedo == null) { return; }

            ApplyUndoValues(objectToRedo, (int)record[1], record[4]);
        }

        private static void ApplyUndoValues(object objectToUndo, int undoType, object values)
        {
            Type objectType = objectToUndo.GetType();

            if (objectType.Equals(typeof(GameObject)))
            {
                ApplyUndoValues((GameObject)objectToUndo, (GameObjectUndoType)undoType, values);
            }
            else if (objectType.Equals(typeof(Transform)) || objectType.Equals(typeof(RectTransform)))
            {
                ApplyUndoValues((Transform)objectToUndo, (TransformUndoType)undoType, values);
            }
        }

        private static void ApplyUndoValues(GameObject gameObjectToUndo, GameObjectUndoType undoType, object values)
        {
            switch (undoType)
            {
                case GameObjectUndoType.Active:
                    gameObjectToUndo.SetActive((bool)values);
                    break;
                case GameObjectUndoType.Name:
                    gameObjectToUndo.name = (string)values;
                    break;
                case GameObjectUndoType.Layer:
                    gameObjectToUndo.layer = (int)values;
                    break;
            }
        }

        private static void ApplyUndoValues(Transform transformToUndo, TransformUndoType undoType, object values)
        {
            switch (undoType)
            {
                case TransformUndoType.LocalPosition:
                    transformToUndo.localPosition = (Vector3)values;
                    break;
                case TransformUndoType.LocalRotation:
                    transformToUndo.localRotation = (Quaternion)values;
                    break;
                case TransformUndoType.LocalScale:
                    transformToUndo.localScale = (Vector3)values;
                    break;
            }
        }

        internal static object GetRecordedValues(GameObject gameObjectToUndo, GameObjectUndoType undoType)
        {
            switch (undoType)
            {
                case GameObjectUndoType.Active:
                    return gameObjectToUndo.activeSelf;
                case GameObjectUndoType.Name:
                    return gameObjectToUndo.name;
                case GameObjectUndoType.Layer:
                    return gameObjectToUndo.layer;
                default:
                    return null;
            }
        }

        internal static object GetRecordedValues(Transform transformToUndo, TransformUndoType undoType)
        {
            switch (undoType)
            {
                case TransformUndoType.LocalPosition:
                    return transformToUndo.localPosition;
                case TransformUndoType.LocalRotation:
                    return transformToUndo.localRotation;
                case TransformUndoType.LocalScale:
                    return transformToUndo.localScale;
                default:
                    return null;
            }
        }
    }
}
