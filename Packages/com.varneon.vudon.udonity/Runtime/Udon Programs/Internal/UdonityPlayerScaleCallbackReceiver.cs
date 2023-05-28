using UdonSharp;
using UnityEngine;
using Varneon.VUdon.PlayerScaleUtility.Abstract;

namespace Varneon.VUdon.Udonity.Utilities
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonityPlayerScaleCallbackReceiver : PlayerScaleCallbackReceiver
    {
        [SerializeField]
        private Pointer pointer;

        [SerializeField]
        private Logger.Abstract.UdonLogger logger;

        [SerializeField]
        private MeshRenderer handlePlanesRenderer;

        [SerializeField, HideInInspector]
        private Material handlePlanesMaterial;

        private MaterialPropertyBlock handlePlanesMaterialPropertyBlock;

        private void Start()
        {
            handlePlanesMaterialPropertyBlock = new MaterialPropertyBlock();
        }

        public override void OnPlayerScaleChanged(float newPlayerScale)
        {
            handlePlanesMaterialPropertyBlock.SetFloat("_CameraScale", newPlayerScale);

            handlePlanesRenderer.SetPropertyBlock(handlePlanesMaterialPropertyBlock);

            pointer.SetPlayerScale(newPlayerScale);

            logger.Log($"[<color=#09C>PlayerScaleUtility</color>]: Avatar changed! New camera scale: <color=#6CC>{newPlayerScale}</color>");
        }
    }
}
