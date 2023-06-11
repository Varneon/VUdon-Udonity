using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Udonity.Enums;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Windows
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ColorDialog : Abstract.ApplicationWindow
    {
        [SerializeField]
        private Image originalColorPreview;

        [SerializeField]
        private Image colorPreview;

        [SerializeField]
        private RectTransform hueRingRoot;

        [SerializeField]
        private RectTransform hueRingPivot;

        [SerializeField]
        private RectTransform quadMarker;

        [SerializeField]
        private EnumField modeField;

        [SerializeField]
        private FloatField intensityField;

        [SerializeField]
        private Slider
            sliderR,
            sliderG,
            sliderB,
            sliderA;

        [SerializeField]
        private InputField
            inputFieldR,
            inputFieldG,
            inputFieldB,
            inputFieldA,
            inputFieldHex;

        [SerializeField]
        private Image
            hueBackground,
            saturationBackground,
            valueBackground;

        [SerializeField]
        private Image
            redBackground,
            greenBackground,
            blueBackground;

        [SerializeField]
        private Image svQuad;

        [SerializeField]
        private GameObject alphaContainer;

        [SerializeField]
        private TextMeshProUGUI windowTitle;

        private ColorDisplayMode activeColorDisplayMode;

        private ColorField activeColorField;

        private Color originalColor;

        private Color currentColor;

        private bool draggingHueRing;

        private bool draggingSVQuad;

        private Vector3 hueRingGrabOffset;

        private Vector3 svQuadGrabOffset;

        private bool isCurrentColorHDR;

        private float intensity;

        private const float MAX_OVEREXPOSED_COLOR_COMPONENT = 191f;

        private void Start()
        {
            modeField.RegisterValueChangedCallback(this, nameof(OnColorDisplayModeChanged));

            modeField.SetValueWithoutNotify((int)activeColorDisplayMode);

            intensityField.RegisterValueChangedCallback(this, nameof(OnIntensityChanged));
        }

        public void OpenWithField(ColorField colorField)
        {
            gameObject.SetActive(true);

            isCurrentColorHDR = colorField.IsHDR;

            windowTitle.text = isCurrentColorHDR ? "HDR Color" : "Color";

            intensityField.gameObject.SetActive(isCurrentColorHDR);

            bool hasAlpha = colorField.HasAlpha;

            alphaContainer.SetActive(hasAlpha && !isCurrentColorHDR);

            inputFieldHex.gameObject.SetActive(!isCurrentColorHDR);

            activeColorField = colorField;

            originalColor = colorField.Value;

            Color originalPreviewColor = originalColor;

            originalPreviewColor.a = 0f;

            originalColorPreview.color = originalPreviewColor;

            SetColor(originalColor);
        }

        public void Close()
        {
            activeColorField.EndColorEditing();

            gameObject.SetActive(false);
        }

        private void SetColor(Color color)
        {
            currentColor = color;

            if (isCurrentColorHDR)
            {
                float scaleFactor = MAX_OVEREXPOSED_COLOR_COMPONENT / currentColor.maxColorComponent;

                float totalIntensity = Mathf.Log(255f / scaleFactor) / Mathf.Log(2f);

                if (totalIntensity > 0f)
                {
                    intensityField.SetValueWithoutNotify(totalIntensity);

                    intensity = totalIntensity;

                    float originalAlpha = currentColor.a;

                    currentColor /= Mathf.Pow(2, totalIntensity);

                    currentColor.a = originalAlpha;
                }
                else
                {
                    intensityField.Value = 0f;
                }
            }

            Color.RGBToHSV(currentColor, out float h, out float s, out float v);

            float a = currentColor.a;

            if (activeColorDisplayMode == ColorDisplayMode.HSV)
            {
                sliderR.SetValueWithoutNotify(h);
                sliderG.SetValueWithoutNotify(s);
                sliderB.SetValueWithoutNotify(v);
                sliderA.SetValueWithoutNotify(a);

                inputFieldR.text = (h * 360f).ToString("F0");
                inputFieldG.text = (s * 100f).ToString("F0");
                inputFieldB.text = (v * 100f).ToString("F0");
                inputFieldA.text = (a * 100f).ToString("F0");
            }
            else
            {
                float r = currentColor.r;
                float g = currentColor.g;
                float b = currentColor.b;

                sliderR.SetValueWithoutNotify(r);
                sliderG.SetValueWithoutNotify(g);
                sliderB.SetValueWithoutNotify(b);
                sliderA.SetValueWithoutNotify(a);

                if (activeColorDisplayMode == ColorDisplayMode.RGB1)
                {
                    inputFieldR.text = r.ToString();
                    inputFieldG.text = g.ToString();
                    inputFieldB.text = b.ToString();
                    inputFieldA.text = a.ToString();
                }
                else
                {
                    inputFieldR.text = (r * 255f).ToString("F0");
                    inputFieldG.text = (g * 255f).ToString("F0");
                    inputFieldB.text = (b * 255f).ToString("F0");
                    inputFieldA.text = (a * 255f).ToString("F0");
                }
            }

            UpdateMarkers();
        }

        public void OnColorDisplayModeChanged()
        {
            activeColorDisplayMode = (ColorDisplayMode)modeField.Value;

            SetColor(currentColor);

            bool isHSV = activeColorDisplayMode == ColorDisplayMode.HSV;

            redBackground.gameObject.SetActive(!isHSV);
            greenBackground.gameObject.SetActive(!isHSV);
            blueBackground.gameObject.SetActive(!isHSV);
            hueBackground.gameObject.SetActive(isHSV);
            saturationBackground.gameObject.SetActive(isHSV);
            valueBackground.gameObject.SetActive(isHSV);
        }

        public void OnIntensityChanged()
        {
            intensity = intensityField.Value;

            ApplyColorToField();
        }

        public void OnRedOrHueSliderUpdated()
        {
            if(activeColorDisplayMode == ColorDisplayMode.HSV)
            {
                float h = sliderR.value;

                currentColor = Color.HSVToRGB(
                    h,
                    sliderG.value,
                    sliderB.value
                    );

                currentColor.a = sliderA.value;

                inputFieldR.text = (h * 360f).ToString("F0");
            }
            else
            {
                float r = sliderR.value;

                currentColor.r = r;

                inputFieldR.text = activeColorDisplayMode == ColorDisplayMode.RGB255 ? (r * 255f).ToString("F0") : r.ToString();
            }

            UpdateMarkers();

            ApplyColorToField();
        }

        public void OnGreenOrSaturationSliderUpdated()
        {
            if (activeColorDisplayMode == ColorDisplayMode.HSV)
            {
                float s = sliderG.value;

                currentColor = Color.HSVToRGB(
                    sliderR.value,
                    s,
                    sliderB.value
                    );

                currentColor.a = sliderA.value;

                inputFieldG.text = (s * 100f).ToString("F0");
            }
            else
            {
                float g = sliderG.value;

                currentColor.g = g;

                inputFieldG.text = activeColorDisplayMode == ColorDisplayMode.RGB255 ? (g * 255f).ToString("F0") : g.ToString();
            }

            UpdateMarkers();

            ApplyColorToField();
        }

        public void OnBlueOrValueSliderUpdated()
        {
            if (activeColorDisplayMode == ColorDisplayMode.HSV)
            {
                float v = sliderB.value;

                currentColor = Color.HSVToRGB(
                    sliderR.value,
                    sliderG.value,
                    v
                    );

                currentColor.a = sliderA.value;

                inputFieldB.text = (v * 100f).ToString("F0");
            }
            else
            {
                float b = sliderB.value;

                currentColor.b = b;

                inputFieldB.text = activeColorDisplayMode == ColorDisplayMode.RGB255 ? (b * 255f).ToString("F0") : b.ToString();
            }

            UpdateMarkers();

            ApplyColorToField();
        }

        public void OnAlphaSliderUpdated()
        {
            float a = sliderA.value;

            currentColor.a = a;

            inputFieldA.text = activeColorDisplayMode == ColorDisplayMode.HSV ? (a * 100f).ToString("F0") : activeColorDisplayMode == ColorDisplayMode.RGB255 ? (a * 255f).ToString("F0") : a.ToString();

            ApplyColorToField();
        }

        public void OnBeginHueRingDrag()
        {
            if (draggingHueRing) { return; }

            if(draggingHueRing = pointer.TryGetPointOnPlane(hueRingRoot, hueRingRoot.forward, out Vector3 pointerPosition))
            {
                hueRingGrabOffset = Vector3.Scale(hueRingRoot.InverseTransformPoint(pointerPosition), new Vector3(1f, 1f, 0f));

                OnHueRingDrag();
            }
        }

        public void OnHueRingDrag()
        {
            if (!draggingHueRing) { return; }

            if (!pointer.TryGetPointOnPlane(hueRingRoot, hueRingRoot.forward, out Vector3 pointerPosition)) { draggingHueRing = false; return; }

            Vector3 delta = Vector3.Scale(hueRingRoot.InverseTransformPoint(pointerPosition), new Vector3(1f, 1f, 0f));

            float hue = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            hueRingPivot.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);

            UpdateHue((hue < 0f ? hue + 360f : hue) / 360f);

            SendCustomEventDelayedFrames(nameof(OnHueRingDrag), 0);
        }

        public void OnEndHueRingDrag()
        {
            if (!draggingHueRing) { return; }

            draggingHueRing = false;
        }

        public void OnBeginSVQuadDrag()
        {
            if (draggingSVQuad) { return; }

            if(draggingSVQuad = pointer.TryGetPointOnPlane(quadMarker, quadMarker.forward, out Vector3 pointerPosition))
            {
                svQuadGrabOffset = Vector3.Scale(quadMarker.InverseTransformPoint(pointerPosition), new Vector3(1f, 1f, 0f));

                OnSVQuadDrag();
            }
        }

        public void OnSVQuadDrag()
        {
            if (!draggingSVQuad) { return; }

            if(!pointer.TryGetPointOnPlane(quadMarker, quadMarker.forward, out Vector3 pointerPosition)) { draggingSVQuad = false; return; }

            Vector3 delta = Vector3.Scale(quadMarker.InverseTransformPoint(pointerPosition), new Vector3(1f, 1f, 0f));

            quadMarker.localPosition += delta - svQuadGrabOffset;

            Vector3 localPos = quadMarker.localPosition;

            localPos = new Vector3(Mathf.Clamp(localPos.x, -54f, 54f), Mathf.Clamp(localPos.y, -54f, 54f), 0f);

            quadMarker.localPosition = localPos;

            UpdateSV((54f + localPos.x) / 108f, (54f + localPos.y) / 108f);

            SendCustomEventDelayedFrames(nameof(OnSVQuadDrag), 0);
        }

        public void OnEndSVQuadDrag()
        {
            if (!draggingSVQuad) { return; }

            draggingSVQuad = false;
        }

        private void UpdateHue(float hue)
        {
            Color.RGBToHSV(currentColor, out float h, out float s, out float v);

            currentColor = Color.HSVToRGB(hue, s, v);

            UpdateMarkers();

            UpdateFields();

            ApplyColorToField();
        }

        private void UpdateSV(float saturation, float value)
        {
            Color.RGBToHSV(currentColor, out float h, out float s, out float v);

            currentColor = Color.HSVToRGB(h, saturation, value);

            UpdateMarkers();

            UpdateFields();

            ApplyColorToField();
        }

        private void UpdateMarkers()
        {
            Color.RGBToHSV(currentColor, out float h, out float s, out float v);

            hueRingPivot.localEulerAngles = new Vector3(0f, 0f, h * 360f);

            quadMarker.anchoredPosition = new Vector2(-54f + s * 108f, -54f + v * 108f);

            Color fullHue = Color.HSVToRGB(h, 1f, 1f);

            svQuad.color = fullHue;

            saturationBackground.color = fullHue;

            inputFieldHex.text = string.Format("{0}{1}{2}", ((byte)(Mathf.Clamp01(currentColor.r) * 255f)).ToString("X2"), ((byte)(Mathf.Clamp01(currentColor.g) * 255f)).ToString("X2"), ((byte)(Mathf.Clamp01(currentColor.b) * 255f)).ToString("X2"));

            Color previewOutputColor = GetOutputColor();

            previewOutputColor.a = 1f;

            colorPreview.color = previewOutputColor;
        }

        private void UpdateFields()
        {
            if (activeColorDisplayMode == ColorDisplayMode.HSV)
            {
                Color.RGBToHSV(currentColor, out float h, out float s, out float v);

                sliderR.SetValueWithoutNotify(h);
                sliderG.SetValueWithoutNotify(s);
                sliderB.SetValueWithoutNotify(v);

                inputFieldR.text = (h * 360f).ToString("F0");
                inputFieldG.text = (s * 100f).ToString("F0");
                inputFieldB.text = (v * 100f).ToString("F0");
            }
            else
            {
                float r = currentColor.r;
                float g = currentColor.g;
                float b = currentColor.b;

                sliderR.SetValueWithoutNotify(r);
                sliderG.SetValueWithoutNotify(g);
                sliderB.SetValueWithoutNotify(b);

                if (activeColorDisplayMode == ColorDisplayMode.RGB1)
                {
                    inputFieldR.text = r.ToString();
                    inputFieldG.text = g.ToString();
                    inputFieldB.text = b.ToString();
                }
                else
                {
                    inputFieldR.text = (r * 255f).ToString("F0");
                    inputFieldG.text = (g * 255f).ToString("F0");
                    inputFieldB.text = (b * 255f).ToString("F0");
                }
            }
        }

        private Color GetOutputColor()
        {
            if (isCurrentColorHDR)
            {
                float originalAlpha = currentColor.a;

                Color hdrColor = currentColor * Mathf.Pow(2f, intensity);

                hdrColor.a = originalAlpha;

                return hdrColor;
            }
            else
            {
                return currentColor;
            }
        }

        private void ApplyColorToField()
        {
            activeColorField.ApplyColorDialogValue(GetOutputColor());
        }
    }
}
