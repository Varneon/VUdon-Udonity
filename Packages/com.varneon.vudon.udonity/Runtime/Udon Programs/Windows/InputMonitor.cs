using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.Udon.Common;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    public class InputMonitor : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(600f, 400f);

        [SerializeField]
        private CustomRenderTexture graphCustomRenderTexture;

        [SerializeField]
        private Material graphMaterial;

        [SerializeField]
        private GameObject graphElement;

        [SerializeField]
        private Transform graphContainer;

        private bool scanning;

        private readonly string[] inputAxes = new string[]
        {
            "Horizontal",
            "Vertical",
            "Fire1",
            "Fire2",
            "Fire3",
            "Jump",
            "Shift",
            "Submit",
            "Cancel",
            "Mouse X",
            "Mouse Y",
            "Mouse ScrollWheel",
            "Window Shake X",
            "Window Shake Y",
            "Mouse Horizontal",
            "Mouse Vertical",
            "Mouse Wheel",
            "Oculus_GearVR_LThumbstickX",
            "Oculus_GearVR_LThumbstickY",
            "Oculus_GearVR_RThumbstickX",
            "Oculus_GearVR_RThumbstickY",
            "Oculus_GearVR_DpadX",
            "Oculus_GearVR_DpadY",
            "Oculus_GearVR_LIndexTrigger",
            "Oculus_GearVR_RIndexTrigger",
            "Oculus_CrossPlatform_Button2",
            "Oculus_CrossPlatform_Button4",
            "Oculus_CrossPlatform_PrimaryThumbstick",
            "Oculus_CrossPlatform_SecondaryThumbstick",
            "Oculus_CrossPlatform_PrimaryIndexTrigger",
            "Oculus_CrossPlatform_SecondaryIndexTrigger",
            "Oculus_CrossPlatform_PrimaryHandTrigger",
            "Oculus_CrossPlatform_SecondaryHandTrigger",
            "Oculus_CrossPlatform_PrimaryThumbstickHorizontal",
            "Oculus_CrossPlatform_PrimaryThumbstickVertical",
            "Oculus_CrossPlatform_SecondaryThumbstickHorizontal",
            "Oculus_CrossPlatform_SecondaryThumbstickVertical"
        };

        private string[] inputButtons = new string[]
        {

        };

        private float[] inputMethodValues = new float[8];

        private float[] inputData = new float[256];

        private int graphIndex;

        private void Start()
        {
            for(int i = 0; i < inputAxes.Length; i++)
            {
                AddGraph(inputAxes[i]);
            }

            AddGraph("InputMoveHorizontal()");
            AddGraph("InputMoveVertical()");
            AddGraph("InputLookHorizontal()");
            AddGraph("InputLookVertical()");
            AddGraph("InputJump()");
            AddGraph("InputUse()");
            AddGraph("InputGrab()");
            AddGraph("InputDrop()");
        }

        private void AddGraph(string name)
        {
            GameObject newGraphElement = Instantiate(graphElement, graphContainer, false);

            newGraphElement.GetComponentInChildren<TextMeshProUGUI>().text = name;

            Image image = newGraphElement.transform.GetChild(0).GetChild(0).GetComponent<Image>();

            float offset = (float)graphIndex / 255f;

            Color imageColor = image.color;

            image.color = new Color(imageColor.r, imageColor.g, imageColor.b, offset);

            graphIndex++;
        }

        public void ScanInputs()
        {
            if (!scanning) { return; }

            int index = 0;

            for (int i = 0; i < inputAxes.Length; i++)
            {
                inputData[index] = Input.GetAxisRaw(inputAxes[i]);

                index++;
            }

            for (int i = 0; i < inputMethodValues.Length; i++)
            {
                inputData[index] = inputMethodValues[i];

                index++;
            }

            graphCustomRenderTexture.material.SetFloatArray("_Inputs", inputData);

            SendCustomEventDelayedFrames(nameof(ScanInputs), 0);
        }

        protected override void OnWindowActiveStateChanged(bool active)
        {
            scanning = active;

            ScanInputs();
        }

        public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
        {
            inputMethodValues[0] = value;
        }

        public override void InputMoveVertical(float value, UdonInputEventArgs args)
        {
            inputMethodValues[1] = value;
        }

        public override void InputLookHorizontal(float value, UdonInputEventArgs args)
        {
            inputMethodValues[2] = value;
        }

        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            inputMethodValues[3] = value;
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            inputMethodValues[4] = value ? 1f : 0f;
        }

        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            inputMethodValues[5] = value ? 1f : 0f;
        }

        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            inputMethodValues[6] = value ? 1f : 0f;
        }

        public override void InputDrop(bool value, UdonInputEventArgs args)
        {
            inputMethodValues[7] = value ? 1f : 0f;
        }
    }
}
