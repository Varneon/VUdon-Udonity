#pragma warning disable IDE0044 // Making serialized fields readonly hides them from the inspector

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.Logger.Abstract;
using VRC.SDKBase;

namespace Varneon.VUdon.Udonity.Windows
{
    /// <summary>
    /// In-game console window for debugging UdonBehaviours
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [SelectionBase]
    [DefaultExecutionOrder(-2146483648)]
    public class ConsoleWindow : UdonLogger
    {
        #region Variables

        #region Serialized
        /// <summary>
        /// Should the timestamps be displayed on log entries by default
        /// </summary>
        [Header("Settings")]
        [SerializeField]
        [Tooltip("Should the timestamps be displayed on log entries by default")]
        private bool showTimestamps = false;

        /// <summary>
        /// How many log entries are ensured to always be visible in the console
        /// </summary>
        [SerializeField]
        [Tooltip("How many log entries are ensured to always be visible in the console")]
        private int minLogEntries = 10;

        /// <summary>
        /// How many log entries can the console display simultaneously by default
        /// </summary>
        [SerializeField]
        [Tooltip("How many log entries can the console display simultaneously")]
        private int maxLogEntries = 100;

        /// <summary>
        /// How many entries should be incremented/decremented from MaxLogEntries when buttons on the UI are pressed
        /// </summary>
        [SerializeField]
        [Tooltip("How many entries should be incremented/decremented from MaxLogEntries when buttons on the UI are pressed")]
        private int maxLogEntriesStep = 50;

        /// <summary>
        /// Should the log entries be sent to the default logs as well
        /// </summary>
        [SerializeField]
        [Tooltip("Should the log entries be sent to the default logs as well")]
        private bool proxyEntriesToLogs;

        /// <summary>
        /// Format of the timestamp
        /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings"/>
        /// </summary>
        [Header("Advanced")]
        [SerializeField]
        [Tooltip("DateTime string format for log timestamps.\n\nYear: yyyy\nMonth: MM\nDay: dd\nHours: HH\nMinutes: mm\nSeconds: ss")]
        private string timestampFormat = "yyyy.MM.dd HH:mm:ss";

        [SerializeField]
        [Tooltip("Prefix for default messages from UdonConsole")]
        private string systemPrefix = "[<color=#0CC>UdonConsole</color>]:";

        [SerializeField]
        [Tooltip("Message prefix for player joining the instance")]
        private string playerJoinPrefix = "[<color=#0C0>JOIN</color>]:";

        [SerializeField]
        [Tooltip("Message prefix for player leaving the instance")]
        private string playerLeavePrefix = "[<color=#C00>LEAVE</color>]:";

        [Header("References")]
        [SerializeField]
        private RectTransform logWindow;

        [SerializeField]
        private GameObject logItem;

        [SerializeField]
        private Toggle logToggle, warningToggle, errorToggle, timestampsToggle;

        [SerializeField]
        private InputField maxLogEntriesField;

        [SerializeField]
        private Slider panelSeparatorSlider;

        [SerializeField]
        private RectTransform topPanel, bottomPanel;

        [SerializeField]
        private TextMeshProUGUI messagePreviewText, editorFooterLogPreviewText;
        #endregion

        #region Hidden
        [SerializeField, HideInInspector]
        private Scrollbar scrollbar;

        [SerializeField, HideInInspector]
        private RectTransform canvasRoot;
        #endregion

        #region Private
        private Button selectedMessage;

        private bool typeFilterDirty, timestampToggleDirty;
        #endregion

        #region Constants
        private const string LOG_PREFIX = "[<color=#00FFFF>Udonity</color>]:";

        private const string WHITESPACE = " ";

        private const int ENTRIES_HARDCAP = 1000;

        private const int LOG_TYPE_SPRITE_TAG_LENGTH = 11;
        #endregion

        #endregion

        #region Private Methods
        private void Start()
        {
            Log($"{LOG_PREFIX} Thank you for using Udonity Pre-Alpha!");
            LogWarning($"{LOG_PREFIX} Udonity is still in pre-alpha. Features are missing and central UdonBehaviours may crash unexpectedly.");
        }

        /// <summary>
        /// Reloads all entries and applies filters
        /// </summary>
        private void ReloadLogs()
        {
            int entryOverflow = GetCurrentLogEntryCount() - maxLogEntries;

            if (entryOverflow > 0)
            {
                for (int i = 0; i < entryOverflow; i++)
                {
                    Destroy(logWindow.GetChild(i).gameObject);
                }
            }

            if (selectedMessage)
            {
                LogType type = (LogType)int.Parse(selectedMessage.name[0].ToString());

                if (!IsLogTypeEnabled(type))
                {
                    selectedMessage.interactable = true;

                    selectedMessage = null;

                    messagePreviewText.text = string.Empty;
                }
            }

            for (int i = 0; i < GetCurrentLogEntryCount(); i++)
            {
                GameObject item = logWindow.GetChild(i).gameObject;

                string[] info = item.name.Split(new char[] { ' ' }, 2);

                LogType type = (LogType)int.Parse(info[0]);

                if (timestampToggleDirty)
                {
                    string timestamp = info[1];

                    TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();

                    string textContent = text.text;

                    bool hasTimestamp = !showTimestamps;

                    if (showTimestamps && !hasTimestamp) { text.text = textContent.Insert(LOG_TYPE_SPRITE_TAG_LENGTH, string.Concat(timestamp, WHITESPACE)); }
                    else if (!showTimestamps && hasTimestamp) { text.text = string.Concat(GetLogTypePrefix(type), text.text.Substring(LOG_TYPE_SPRITE_TAG_LENGTH + timestamp.Length)); }
                }

                if (typeFilterDirty) { SetLogEntryActive(item, type); }
            }

            timestampToggleDirty = false;

            typeFilterDirty = false;
        }

        /// <summary>
        /// Sets the log entry active based on current filter states
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="type"></param>
        private void SetLogEntryActive(GameObject logEntry, LogType type)
        {
            logEntry.SetActive(IsLogTypeEnabled(type));
        }

        private bool IsLogTypeEnabled(LogType type)
        {
            return (type == LogType.Log && !logToggle.isOn) ||
                (type == LogType.Warning && !warningToggle.isOn) ||
                ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) && !errorToggle.isOn);
        }

        /// <summary>
        /// Gets the current timestamp formatted as defined by timestampFormat
        /// </summary>
        /// <returns>Formatted timestamp string</returns>
        private string GetTimestamp()
        {
            return DateTime.UtcNow.ToLocalTime().ToString(timestampFormat);
        }

        /// <summary>
        /// Write line to the console
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="text"></param>
        private void WriteLine(LogType logType, string message)
        {
            TextMeshProUGUI textComponent;

            Transform newEntry;

            string timestamp = GetTimestamp();

            if (GetCurrentLogEntryCount() < maxLogEntries)
            {
                newEntry = Instantiate(logItem, logWindow, false).transform;
            }
            else
            {
                newEntry = logWindow.GetChild(0);
                newEntry.SetAsLastSibling();

                Button entryButton = newEntry.GetComponent<Button>();

                if (!entryButton.interactable)
                {
                    selectedMessage = null;

                    entryButton.interactable = true;

                    messagePreviewText.text = string.Empty;
                }
            }

            GameObject newEntryGO = newEntry.gameObject;

            string metadata = string.Join(WHITESPACE, new string[] { ((int)logType).ToString(), timestamp });

            newEntryGO.name = metadata;
            textComponent = newEntry.GetComponentInChildren<TextMeshProUGUI>();

            textComponent.text = message;

            editorFooterLogPreviewText.text = string.Concat(GetLogTypePrefix(logType), " ", GetLogTypeColorTag(logType), showTimestamps ? message.Substring(LOG_TYPE_SPRITE_TAG_LENGTH + metadata.Length - 1) : message.Substring(LOG_TYPE_SPRITE_TAG_LENGTH));

            SetLogEntryActive(newEntryGO, logType);

            LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRoot);

            SendCustomEventDelayedFrames(nameof(ScrollToBottom), 3);
        }

        /// <summary>
        /// Get the number of entries in the console
        /// </summary>
        /// <returns>Log entry count</returns>
        private int GetCurrentLogEntryCount()
        {
            return logWindow.childCount;
        }

        private string BuildLogStringOutput(LogType logType, object message)
        {
            return showTimestamps ?
                string.Join(" ", GetLogTypePrefix(logType), GetTimestamp(), MessageObjectToString(message)) :
                string.Join(" ", GetLogTypePrefix(logType), MessageObjectToString(message));
        }

        private string BuildLogStringOutput(LogType logType, object message, UnityEngine.Object context)
        {
            return showTimestamps ?
                string.Join(" ", GetLogTypePrefix(logType), GetTimestamp(), MessageObjectToString(message), ContextObjectToString(context)) :
                string.Join(" ", GetLogTypePrefix(logType), MessageObjectToString(message), ContextObjectToString(context));
        }

        private string GetLogTypeColorTag(LogType logType)
        {
            switch (logType)
            {
                case LogType.Log:
                    return "<color=#BABABA>";
                case LogType.Warning:
                    return "<color=#c49502>";
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    return "<color=#af0e0f>";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Selects a message
        /// </summary>
        public void SelectMessage()
        {
            if (selectedMessage != null)
            {
                selectedMessage.interactable = true;
            }

            Button[] messageButtons = logWindow.GetComponentsInChildren<Button>();

            for (int i = 0; i < messageButtons.Length; i++)
            {
                if (messageButtons[i].interactable) { continue; }

                selectedMessage = messageButtons[i];

                string message = selectedMessage.GetComponentInChildren<TextMeshProUGUI>().text;

                messagePreviewText.text = showTimestamps ? message.Substring(LOG_TYPE_SPRITE_TAG_LENGTH + selectedMessage.name.Length - 1) : message.Substring(LOG_TYPE_SPRITE_TAG_LENGTH);
            }
        }

        /// <summary>
        /// Resizes the window panels based on separator slider
        /// </summary>
        public void ResizeWindowPanels()
        {
            float position = Mathf.Clamp(panelSeparatorSlider.value, 0.1f, 0.9f);

            panelSeparatorSlider.SetValueWithoutNotify(position);

            topPanel.anchorMin = new Vector2(0f, position);
            bottomPanel.anchorMax = new Vector2(1f, position);
        }

        /// <summary>
        /// Scrolls to the bottom of the window
        /// </summary>
        public void ScrollToBottom()
        {
            scrollbar.value = 0f;
        }
        #endregion

        #region Logger Overrides
        protected override void Log(LogType logType, object message)
        {
            WriteLine(logType, BuildLogStringOutput(logType, message));
        }

        protected override void Log(LogType logType, object message, UnityEngine.Object context)
        {
            WriteLine(logType, BuildLogStringOutput(logType, message, context));
        }

        protected override void LogFormat(LogType logType, string format, params object[] args)
        {
            WriteLine(logType, BuildLogStringOutput(logType, string.Format(format, args)));
        }

        protected override void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            WriteLine(logType, BuildLogStringOutput(logType, string.Format(format, args), context));
        }

        public override void ClearLogs()
        {
            for (int i = 0; i < GetCurrentLogEntryCount(); i++)
            {
                Destroy(logWindow.GetChild(i).gameObject);
            }
        }

        public override void ClearLogs(LogType logType)
        {
            editorFooterLogPreviewText.text = string.Empty;

            string logTypePrefix = ((int)logType).ToString();

            for (int i = 0; i < GetCurrentLogEntryCount(); i++)
            {
                Transform element = logWindow.GetChild(i);

                if (element.name.StartsWith(logTypePrefix))
                {
                    Destroy(element.gameObject);
                }
            }
        }

        /// <summary>
        /// Get the log entry prefix for provided log type
        /// </summary>
        /// <param name="logType">Type of message e.g. warn or error etc</param>
        /// <returns>Default log entry prefix of LogType</returns>
        protected override string GetLogTypePrefix(LogType logType)
        {
            switch (logType)
            {
                case LogType.Log:
                    return "<sprite=0>";
                case LogType.Warning:
                    return "<sprite=1>";
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    return "<sprite=2>";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Player Events
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            Log(string.Join(WHITESPACE, new string[] { systemPrefix, playerJoinPrefix, player.displayName }));
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!VRC.SDKBase.Utilities.IsValid(player)) { return; }

            Log(string.Join(WHITESPACE, new string[] { systemPrefix, playerLeavePrefix, player.displayName }));
        }
        #endregion

        #region Filter Toggles
        /// <summary>
        /// Toggles Log entry type filtering
        /// </summary>
        public void ToggleFilterLog()
        {
            typeFilterDirty = true;

            ReloadLogs();
        }

        /// <summary>
        /// Toggles Warning entry type filtering
        /// </summary>
        public void ToggleFilterWarning()
        {
            typeFilterDirty = true;

            ReloadLogs();
        }

        /// <summary>
        /// Toggles Error entry type filtering
        /// </summary>
        public void ToggleFilterError()
        {
            typeFilterDirty = true;

            ReloadLogs();
        }

        /// <summary>
        /// Toggles timestamp display on log entries
        /// </summary>
        public void ToggleTimestamps()
        {
            timestampToggleDirty = true;

            showTimestamps = !timestampsToggle.isOn;

            ReloadLogs();
        }
        #endregion

        #region Log Entry Limit
        /// <summary>
        /// Applies the max log entry value from MaxLogEntriesField
        /// </summary>
        public void ApplyMaxLogEntries()
        {
            int.TryParse(maxLogEntriesField.text, out maxLogEntries);

            SetMaxLogEntries(maxLogEntries);
        }

        /// <summary>
        /// Decreases the maximum amount of log entries by 10
        /// </summary>
        public void DecreaseMaxEntries()
        {
            SetMaxLogEntries(maxLogEntries - maxLogEntriesStep);
        }

        /// <summary>
        /// Increases the maximum amount of log entries by 10
        /// </summary>
        public void IncreaseMaxEntries()
        {
            SetMaxLogEntries(maxLogEntries + maxLogEntriesStep);
        }

        /// <summary>
        /// Changes the maximum number of log entries based on the provided number
        /// </summary>
        /// <param name="maxEntries"></param>
        private void SetMaxLogEntries(int maxEntries)
        {
            maxLogEntries = Mathf.Clamp(maxEntries, minLogEntries, ENTRIES_HARDCAP);

            maxLogEntriesField.text = maxLogEntries.ToString();

            ReloadLogs();
        }
        #endregion

        #region Initialization

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        internal void InitializeOnBuild()
        {
            timestampsToggle.SetIsOnWithoutNotify(!showTimestamps);
            scrollbar = GetComponentInChildren<Scrollbar>(true);
            canvasRoot = GetComponentInChildren<VerticalLayoutGroup>(true).GetComponent<RectTransform>();
        }
#endif

        #endregion
    }
}
