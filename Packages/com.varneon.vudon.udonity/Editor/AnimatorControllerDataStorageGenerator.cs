using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Editor
{
    public static class AnimatorControllerDataStorageGenerator
    {
        public static void GenerateAnimatorControllerDataStorage()
        {
            AnimatorControllerDataStorage dataStorage = Resources.FindObjectsOfTypeAll<AnimatorControllerDataStorage>().Where(s => s.gameObject.scene.IsValid()).FirstOrDefault();

            if(dataStorage == null) { return; }

            AnimatorController[] animatorControllers = Resources.FindObjectsOfTypeAll<AnimatorController>();

            List<string[]> parameterNames = new List<string[]>();

            List<int[]> parameterTypes = new List<int[]>();

            List<string[]> layerNames = new List<string[]>();

            List<int[]> layerBlendingModes = new List<int[]>();

            foreach(AnimatorController controller in animatorControllers)
            {
                AnimatorControllerParameter[] parameters = controller.parameters;

                parameterNames.Add(parameters.Select(p => p.name).ToArray());

                parameterTypes.Add(parameters.Select(p => GetParameterTypeIndex(p.type)).ToArray());

                AnimatorControllerLayer[] layers = controller.layers;

                layerNames.Add(layers.Select(l => l.name).ToArray());

                layerBlendingModes.Add(layers.Select(l => (int)l.blendingMode).ToArray());
            }

            dataStorage.runtimeAnimatorControllerLookup = animatorControllers.Select(c => AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AssetDatabase.GetAssetPath(c))).ToArray();

            dataStorage.parameterNames = parameterNames.ToArray();

            dataStorage.parameterTypes = parameterTypes.ToArray();

            dataStorage.layerNames = layerNames.ToArray();

            dataStorage.layerBlendingModes = layerBlendingModes.ToArray();
        }

        private static int GetParameterTypeIndex(AnimatorControllerParameterType type)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Float: return 0;
                case AnimatorControllerParameterType.Int: return 1;
                case AnimatorControllerParameterType.Bool: return 2;
                case AnimatorControllerParameterType.Trigger: return 3;
                default: return -1;
            }
        }
    }
}
