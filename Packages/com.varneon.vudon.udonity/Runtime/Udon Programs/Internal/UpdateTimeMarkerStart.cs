using System.Diagnostics;
using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.Udonity.Utilities
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-2146483648)]
    public class UpdateTimeMarkerStart : UdonSharpBehaviour
    {
        public long[] Data => data;

        private Stopwatch fixedUpdateStopwatch = new Stopwatch();

        private Stopwatch updateStopwatch = new Stopwatch();

        private Stopwatch lateUpdateStopwatch = new Stopwatch();

        private Stopwatch postLateUpdateStopwatch = new Stopwatch();

        private long[] data = new long[4];

        private void FixedUpdate()
        {
            fixedUpdateStopwatch.Restart();
        }

        private void Update()
        {
            fixedUpdateStopwatch.Reset();

            updateStopwatch.Restart();
        }

        private void LateUpdate()
        {
            lateUpdateStopwatch.Restart();
        }

        public override void PostLateUpdate()
        {
            postLateUpdateStopwatch.Restart();
        }

        internal void OnFixedUpdateEnd()
        {
            fixedUpdateStopwatch.Stop();

            data[0] += fixedUpdateStopwatch.ElapsedTicks;
        }

        internal void OnUpdateEnd()
        {
            updateStopwatch.Stop();

            data[1] = updateStopwatch.ElapsedTicks;
        }

        internal void OnLateUpdateEnd()
        {
            lateUpdateStopwatch.Stop();

            data[2] = lateUpdateStopwatch.ElapsedTicks;
        }

        internal void OnPostLateUpdateEnd()
        {
            postLateUpdateStopwatch.Stop();

            data[3] = postLateUpdateStopwatch.ElapsedTicks;

            data[0] = 0;
        }

        internal void Activate()
        {
            //fixedUpdateStopwatch.Restart();
            //updateStopwatch.Restart();
            //lateUpdateStopwatch.Restart();
            //postLateUpdateStopwatch.Restart();
        }

        internal void Deactivate()
        {
            fixedUpdateStopwatch.Reset();
            updateStopwatch.Reset();
            lateUpdateStopwatch.Reset();
            postLateUpdateStopwatch.Reset();
        }
    }
}
