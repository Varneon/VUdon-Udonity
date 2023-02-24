using UnityEngine;
using UnityEngine.Rendering;

namespace Varneon.VUdon.Udonity.EnumResolver
{
    public static class UnityEngine_RenderingPath
    {
        public static object[] Items() => new object[]
        {
            RenderingPath.UsePlayerSettings,
            RenderingPath.Forward,
            RenderingPath.DeferredShading,
            RenderingPath.VertexLit,
            RenderingPath.DeferredLighting
        };

        public static int[] Values() => new int[] { 0, 3, 1, 2, 4 };

        public static RenderingPath FromInt(int value) => (RenderingPath)Items()[value];

        public static int ToInt(RenderingPath value) => Values()[(int)value + 1];
    }

    public static class UnityEngine_StereoTargetEyeMask
    {
        public static object[] Items() => new object[]
        {
            StereoTargetEyeMask.Both,
            StereoTargetEyeMask.Left,
            StereoTargetEyeMask.Right,
            StereoTargetEyeMask.None
        };

        public static int[] Values() => new int[] { 3, 1, 2, 0 };

        public static StereoTargetEyeMask FromInt(int value) => (StereoTargetEyeMask)Items()[value];

        public static int ToInt(StereoTargetEyeMask value) => Values()[(int)value];
    }

    public static class UnityEngine_CameraClearFlags
    {
        public static object[] Items() => new object[]
        {
            CameraClearFlags.Skybox,
            CameraClearFlags.SolidColor,
            CameraClearFlags.Depth,
            CameraClearFlags.Nothing
        };

        public static CameraClearFlags FromInt(int value) => (CameraClearFlags)Items()[value];

        public static int ToInt(CameraClearFlags value) => (int)value - 1;
    }

    public static class UnityEngine_Rendering_LightShadowResolution
    {
        public static object[] Items() => new object[]
        {
            LightShadowResolution.FromQualitySettings,
            LightShadowResolution.Low,
            LightShadowResolution.Medium,
            LightShadowResolution.High,
            LightShadowResolution.VeryHigh
        };

        public static LightShadowResolution FromInt(int value) => (LightShadowResolution)Items()[value];

        public static int ToInt(LightShadowResolution value) => (int)value - 1;
    }
}
