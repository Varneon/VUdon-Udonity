using TMPro;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;
using Varneon.VUdon.Udonity.Fields.Abstract;
using Varneon.VUdon.Udonity.UIElements;

namespace Varneon.VUdon.Udonity.Editors
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MaterialEditor : UdonSharpBehaviour
    {
        public bool Expanded => expanded;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private TextMeshProUGUI shaderLabel;

        [SerializeField]
        private TextField shaderKeywordsField;

        [SerializeField]
        private Foldout inspectorFoldout;

        [SerializeField]
        private ColorField colorField;

        [SerializeField]
        private Vector3Field vector3Field;

        [SerializeField]
        private FloatField floatField;

        [SerializeField]
        private FloatField rangedFloatField;

        [SerializeField]
        private ObjectField textureField;

        [SerializeField, HideInInspector]
        private bool initialized;

        [SerializeField, HideInInspector]
        private string[] properties;

        [SerializeField, HideInInspector]
        private int[] propertyTypes;

        [SerializeField, HideInInspector]
        private Field[] fields;

        [SerializeField, HideInInspector]
        private int propertyCount;

        private bool expanded = true;

        private int activeFieldId;

        private Windows.Inspector.Inspector containingInspector;

        private Material target;

        internal void GenerateFields(Shader shader, UdonAssetDatabase.UdonAssetDatabase assetDatabase = null)
        {
            string[] shaderData = assetDatabase == null ? null : assetDatabase.GetShaderData(shader);

            if (shaderData == null) { return; }

            propertyCount = shaderData.Length;

            properties = new string[propertyCount];

            propertyTypes = new int[propertyCount];

            fields = new Field[propertyCount];

            for (int i = 0; i < propertyCount; i++)
            {
                string line = shaderData[i];

                string propertyName = line.Substring(2);

                properties[i] = propertyName;

                int propertyType = int.Parse(line[0].ToString());

                propertyTypes[i] = propertyType;

                switch (propertyType)
                {
                    case 0:
                        ColorField newColorField = Instantiate(colorField.gameObject, inspectorFoldout.transform, false).GetComponent<ColorField>();

                        newColorField.GetComponentInChildren<TextMeshProUGUI>().text = propertyName;

                        fields[i] = newColorField;

                        newColorField.RegisterValueChangedCallback(this, nameof(OnFieldChanged), i);

                        //if (target) { newColorField.SetValueWithoutNotify(target.GetColor(propertyName)); }
                        break;
                    case 1:
                        break;
                    case 2:
                        FloatField newFloatField = Instantiate(floatField.gameObject, inspectorFoldout.transform, false).GetComponent<FloatField>();

                        newFloatField.GetComponentInChildren<TextMeshProUGUI>().text = propertyName;

                        fields[i] = newFloatField;

                        newFloatField.RegisterValueChangedCallback(this, nameof(OnFieldChanged), i);

                        //if (target) { newFloatField.SetValueWithoutNotify(target.GetFloat(propertyName)); }
                        break;
                    case 3:
                        FloatField newRangedFloatField = Instantiate(rangedFloatField.gameObject, inspectorFoldout.transform, false).GetComponent<FloatField>();

                        newRangedFloatField.GetComponentInChildren<TextMeshProUGUI>().text = propertyName;

                        fields[i] = newRangedFloatField;

                        newRangedFloatField.RegisterValueChangedCallback(this, nameof(OnFieldChanged), i);

                        //if (target) { newRangedFloatField.SetValueWithoutNotify(target.GetFloat(propertyName)); }
                        break;
                    case 4:
                        ObjectField newTextureField = Instantiate(textureField.gameObject, inspectorFoldout.transform, false).GetComponent<ObjectField>();

                        newTextureField.GetComponentInChildren<TextMeshProUGUI>().text = propertyName;

                        newTextureField.SetFieldType(typeof(Texture2D));

                        fields[i] = newTextureField;

                        newTextureField.RegisterValueChangedCallback(this, nameof(OnFieldChanged), i);

                        //if (target) { newTextureField.SetValueWithoutNotify(target.GetTexture(propertyName)); }
                        break;
                }
            }

            initialized = true;
        }

        public void UpdateFields()
        {
            for (int i = 0; i < propertyCount; i++)
            {
                string property = properties[i];

                switch (propertyTypes[i])
                {
                    case 0:
                        ((ColorField)fields[i]).SetValueWithoutNotify(target.GetColor(property));
                        break;
                    case 1:
                        break;
                    case 2:
                        ((FloatField)fields[i]).SetValueWithoutNotify(target.GetFloat(property));
                        break;
                    case 3:
                        ((FloatField)fields[i]).SetValueWithoutNotify(target.GetFloat(property));
                        break;
                    case 4:
                        ((ObjectField)fields[i]).SetValueWithoutNotify(target.GetTexture(property));
                        break;
                }
            }
        }

        public void OnFieldChanged()
        {
            string property = properties[activeFieldId];

            switch (propertyTypes[activeFieldId])
            {
                case 0:
                    target.SetColor(property, ((ColorField)fields[activeFieldId]).Value);
                    ((ColorField)fields[activeFieldId]).SetValueWithoutNotify(target.GetColor(property));
                    break;
                case 1:
                    break;
                case 2:
                    target.SetFloat(property, ((FloatField)fields[activeFieldId]).Value);
                    ((FloatField)fields[activeFieldId]).SetValueWithoutNotify(target.GetFloat(property));
                    break;
                case 3:
                    target.SetFloat(property, ((FloatField)fields[activeFieldId]).Value);
                    ((FloatField)fields[activeFieldId]).SetValueWithoutNotify(target.GetFloat(property));
                    break;
                case 4:
                    target.SetTexture(property, (Texture)((ObjectField)fields[activeFieldId]).Value);
                    ((ObjectField)fields[activeFieldId]).SetValueWithoutNotify(target.GetTexture(property));
                    break;
            }
        }

        internal void Initialize(Material material, Windows.Inspector.Inspector inspector = null, UdonAssetDatabase.UdonAssetDatabase assetDatabase = null, bool collapsed = false)
        {
            gameObject.SetActive(true);

            if (collapsed) { SetEditorExpandedStateWithoutNotify(false); }

            target = material;

            nameText.text = target.name;

            shaderLabel.text = target.shader.name;

            containingInspector = inspector;

            shaderKeywordsField.SetValueWithoutNotify(string.Join(",", material.shaderKeywords));

            if (!initialized) { GenerateFields(material.shader, assetDatabase); }

            UpdateFields();
        }

        internal void SetEditorExpandedStateWithoutNotify(bool value)
        {
            expanded = value;

            if (inspectorFoldout) { inspectorFoldout.SetExpandedStateWithoutNotify(value); }
        }

        public void OnExpandedStateChanged()
        {
            expanded = inspectorFoldout.Expanded;
        }

#if !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            if (inspectorFoldout) { inspectorFoldout.RegisterValueChangedCallback(this, nameof(OnExpandedStateChanged)); }
        }
#endif
    }
}
