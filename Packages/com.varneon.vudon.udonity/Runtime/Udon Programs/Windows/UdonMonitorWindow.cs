using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Windows.Abstract;
using VRC.Udon;
using Varneon.VUdon.ArrayExtensions;
using Varneon.VUdon.Logger.Abstract;
using Varneon.VUdon.Udonity.Fields.Abstract;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Windows.UdonMonitor
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonMonitorWindow : EditorWindow
    {
        public override Vector2 MinSize => new Vector2(600f, 300f);

        [SerializeField, HideInInspector]
        internal UdonProgramDataStorage programDataStorage;

        [SerializeField, HideInInspector]
        internal Udonity udonity;

        [SerializeField]
        private TextMeshProUGUI headerInfoText;

        [SerializeField]
        private RectTransform symbolContainer;

        [SerializeField]
        private RectTransform symbolFieldContainer;

        [SerializeField]
        private RectTransform entryPointContainer;

        [SerializeField]
        private GameObject symbolListItem;

        [SerializeField]
        private GameObject entryPointListItem;

        [SerializeField]
        private TextField symbolNameField;

        [SerializeField]
        private TextField symbolTypeField;

        [SerializeField]
        private UdonLogger logger;

        private int symbolCount;

        private UdonBehaviour target;

        private const string LOG_PREFIX = "[<color=#ABCDEF>UdonMonitor</color>]: ";

        private readonly Type[] typesAvailableForEdit = new Type[]
        {
            typeof(bool),
            typeof(int),
            typeof(float),
            typeof(string),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Color)
        };

        private string selectedSymbolName;

        private Field symbolField;

        internal void OpenUdonBehaviour(UdonBehaviour udonBehaviour)
        {
            Clear();

            if(!programDataStorage.TryGetProgramIndex(udonBehaviour, out int programIndex, out long id, out string name))
            {
                LogWarning("Couldn't load UdonBehaviour data! UdonBehaviour's program source likely wasn't UdonSharpProgramAsset.");

                return;
            }

            target = udonBehaviour;

            headerInfoText.text = $"<color=#ABC>{udonBehaviour.name}</color>: [{name} <color=#888>({id})</color>]";

            string[] symbols = programDataStorage.GetProgramSymbols(programIndex);

            Type[] symbolTypes = programDataStorage.GetProgramSymbolTypes(programIndex);

            string[] entryPoints = programDataStorage.GetProgramEntryPoints(programIndex);

            symbolCount = symbols.Length;

            for(int i = 0; i < symbolCount; i++)
            {
                GameObject newSymbolListElement = Instantiate(symbolListItem, symbolContainer, false);

                string symbolName = symbols[i];

                Type symbolType = symbolTypes[i];

                newSymbolListElement.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=#abc>({symbolType.Name})</color> {symbols[i]}";

                newSymbolListElement.GetComponent<UdonMonitorSymbolElement>().Initialize(symbolName, symbolType);
            }

            for(int i = 0; i < entryPoints.Length; i++)
            {
                GameObject newEntryPointListElement = Instantiate(entryPointListItem, entryPointContainer, false);

                string entryPoint = entryPoints[i];

                newEntryPointListElement.GetComponentsInChildren<TextMeshProUGUI>().LastOrDefault().text = entryPoint;

                newEntryPointListElement.GetComponent<UdonMonitorEntryPointElement>().methodName = entryPoint;
            }
        }

        internal void InvokeMethod(string methodName)
        {
            target.SendCustomEvent(methodName);
        }

        internal void SelectSymbol(string symbolName, Type symbolType)
        {
            ClearSymbolEditField();

            int typeIndex = typesAvailableForEdit.IndexOf(symbolType);

            if(typeIndex < 0)
            {
                LogWarning(string.Concat("Editing of symbols of type <color=red>'", symbolType.FullName,"'</color> is not supported at this time."));
            }

            GameObject newFieldObject;

            bool isReadonlyField = false;

            string symbolTypeName = symbolType.Name;

            switch (symbolTypeName)
            {
                case "Boolean":
                    newFieldObject = Instantiate(udonity.ToggleFieldPrefab.gameObject, symbolFieldContainer);
                    break;
                case "Int32":
                case "Single":
                    newFieldObject = Instantiate(udonity.FloatFieldPrefab.gameObject, symbolFieldContainer);
                    break;
                case "String":
                    newFieldObject = Instantiate(udonity.TextFieldPrefab.gameObject, symbolFieldContainer);
                    break;
                case "Vector2":
                    newFieldObject = Instantiate(udonity.Vector2FieldPrefab.gameObject, symbolFieldContainer);
                    break;
                case "Vector3":
                    newFieldObject = Instantiate(udonity.Vector3FieldPrefab.gameObject, symbolFieldContainer);
                    break;
                case "Color":
                    newFieldObject = Instantiate(udonity.ColorFieldPrefab.gameObject, symbolFieldContainer);
                    break;
                default:
                    newFieldObject = Instantiate(udonity.TextFieldPrefab.gameObject, symbolFieldContainer);
                    isReadonlyField = true;
                    break;
            }

            selectedSymbolName = symbolName;

            symbolNameField.SetValueWithoutNotify(symbolName);

            symbolTypeField.SetValueWithoutNotify(symbolTypeName);

            symbolField = newFieldObject.GetComponent<Field>();

            symbolField.SetLabelText(isReadonlyField ? "Value (Read-Only)" : "Value");

            if (isReadonlyField)
            {
                object symbolValue = target.GetProgramVariable(symbolName);

                ((TextField)symbolField).SetValueWithoutNotify(symbolValue == null ? "null" : symbolValue.ToString());

                symbolField.Interactive = false;
            }
            else
            {
                symbolField.RegisterValueChangedCallback(this, nameof(OnSymbolValueChanged));

                symbolField.TrySetAbstractValueWithoutNotify(target.GetProgramVariable(symbolName));
            }
        }

        public void OnSymbolValueChanged()
        {
            target.SetProgramVariable(selectedSymbolName, symbolField.AbstractValue);
        }

        private void ClearSymbolEditField()
        {
            symbolNameField.SetValueWithoutNotify(string.Empty);

            symbolTypeField.SetValueWithoutNotify(string.Empty);

            if (symbolField)
            {
                Destroy(symbolField.gameObject);
            }
        }

        private void Clear()
        {
            ClearSymbolEditField();

            headerInfoText.text = string.Empty;

            for(int i = 0; i < symbolContainer.childCount; i++)
            {
                Destroy(symbolContainer.GetChild(i).gameObject);
            }

            for(int i = 0; i < entryPointContainer.childCount; i++)
            {
                Destroy(entryPointContainer.GetChild(i).gameObject);
            }
        }

        private void LogWarning(string message)
        {
            if (logger)
            {
                logger.LogWarning(string.Concat(LOG_PREFIX, message));
            }
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            symbolNameField.Interactive = false;
            symbolTypeField.Interactive = false;
        }
#endif
    }
}
