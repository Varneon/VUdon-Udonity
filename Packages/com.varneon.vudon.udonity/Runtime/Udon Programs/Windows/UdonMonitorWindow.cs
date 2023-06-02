using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Windows.Abstract;
using VRC.Udon;
using Varneon.VUdon.ArrayExtensions;
using Varneon.VUdon.Logger.Abstract;

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

        [SerializeField]
        private TextMeshProUGUI headerInfoText;

        [SerializeField]
        private RectTransform symbolContainer;

        [SerializeField]
        private RectTransform entryPointContainer;

        [SerializeField]
        private GameObject symbolListItem;

        [SerializeField]
        private GameObject entryPointListItem;

        [SerializeField]
        private UdonLogger logger;

        private int symbolCount;

        private UdonBehaviour target;

        private const string LOG_PREFIX = "[<color=#ABCDEF>UdonMonitor</color>]: ";

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

                newSymbolListElement.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=#abc>({symbolTypes[i].Name})</color> {symbols[i]}";
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

        private void Clear()
        {
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
    }
}
