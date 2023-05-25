using TMPro;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using Varneon.VUdon.Udonity.Utilities;

namespace Varneon.VUdon.Udonity.Windows
{
    public class Profiler : Abstract.EditorWindow
    {
        public override Vector2 MinSize => new Vector2(600f, 300f);

        [SerializeField]
        private TextMeshProUGUI udonTimeGraphLabel, frameTimeGraphLabel;

        [SerializeField]
        private FloatField udonTimeGraphRangeField;

        [SerializeField]
        private FloatField frameTimeGraphRangeField;

        [SerializeField]
        private UpdateTimeMarkerStart timeMarkerStart;

        [SerializeField]
        private CustomRenderTexture graphCustomRenderTexture;

        private bool scanning;

        private float udonExecutionTimeGraphRange = 1f;

        private float frameTimeGraphRange = 10f;

        private void Start()
        {
            udonTimeGraphRangeField.SetValueWithoutNotify(udonExecutionTimeGraphRange);

            frameTimeGraphRangeField.SetValueWithoutNotify(frameTimeGraphRange);

            udonTimeGraphRangeField.RegisterValueChangedCallback(this, nameof(OnUdonTimeGraphRangeChanged));

            frameTimeGraphRangeField.RegisterValueChangedCallback(this, nameof(OnFrameTimeGraphRangeChanged));
        }

        public void Scan()
        {
            if (!scanning) { return; }

            long[] data = timeMarkerStart.Data;

            float fixedUpdateTime = (float)data[0] / 1000f;
            float updateTime = (float)data[1] / 1000f;
            float lateUpdateTime = (float)data[2] / 1000f;
            float postLateUpdateTime = (float)data[3] / 1000f;

            Vector4 udonUpdateTimes = new Vector4(
                fixedUpdateTime / udonExecutionTimeGraphRange,
                updateTime / udonExecutionTimeGraphRange,
                lateUpdateTime / udonExecutionTimeGraphRange,
                postLateUpdateTime / udonExecutionTimeGraphRange);

            Vector4 frameTime = new Vector4(Time.deltaTime * 1000f / frameTimeGraphRange, (fixedUpdateTime + updateTime + lateUpdateTime + postLateUpdateTime) / frameTimeGraphRange, 0f, 0f);

            graphCustomRenderTexture.material.SetVectorArray("_Vectors", new Vector4[] { udonUpdateTimes, frameTime });

            SendCustomEventDelayedFrames(nameof(Scan), 0);
        }

        protected override void OnWindowActiveStateChanged(bool active)
        {
            scanning = active;

            Scan();

            if (active)
            {
                timeMarkerStart.Activate();
            }
            else
            {
                timeMarkerStart.Deactivate();
            }
        }

        public void OnUdonTimeGraphRangeChanged()
        {
            udonExecutionTimeGraphRange = udonTimeGraphRangeField.Value;

            udonTimeGraphLabel.text = $"Udon Update Execution Times <color=#888>(0 - {udonExecutionTimeGraphRange} ms)</color>";
        }

        public void OnFrameTimeGraphRangeChanged()
        {
            frameTimeGraphRange = frameTimeGraphRangeField.Value;

            frameTimeGraphLabel.text = $"Frame Timing <color=#888>(0 - {frameTimeGraphRange} ms)</color>";
        }
    }
}
