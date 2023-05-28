using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Udonity.Enums;
using Varneon.VUdon.Udonity.Utilities;
using Varneon.VUdon.Udonity.Windows;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace Varneon.VUdon.Udonity.UdonityLink
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UdonityLinkClient : UdonSharpBehaviour
    {
        [SerializeField]
        internal UdonAssetDatabase.UdonAssetDatabase udonAssetDatabase;

        [SerializeField]
        private LightingWindow lightingWindow;

        [SerializeField]
        private Logger.Abstract.UdonLogger logger;

        private bool linked;

        private IUdonEventReceiver eventReceiver;

        private UdonityLinkRequestType currentRequestType = UdonityLinkRequestType.None;

        private const string LOG_PREFIX = "[<color=#CBA>UdonityLink</color>]: ";

        private readonly VRCUrl
            ConnectURL = new VRCUrl("http://localhost:57777/udonity/connect"),
            DisconnectURL = new VRCUrl("http://localhost:57777/udonity/disconnect"),
            PullSceneURL = new VRCUrl("http://localhost:57777/udonity/scene/pull"),
            PushSceneURL = new VRCUrl("http://localhost:57777/udonity/scene/push");

        private void Start()
        {
            eventReceiver = (IUdonEventReceiver)this;
        }

        public void Connect()
        {
            SendRequest(UdonityLinkRequestType.Connect);
        }

        public void Disconnect()
        {
            SendRequest(UdonityLinkRequestType.Disconnect);
        }

        public void PullScene()
        {
            SendRequest(UdonityLinkRequestType.PullScene);
        }
        
        public void PushScene()
        {
            SendRequest(UdonityLinkRequestType.PushScene);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            switch (currentRequestType)
            {
                case UdonityLinkRequestType.Connect:
                    linked = true;
                    Log("Connected to remote!");
                    break;
                case UdonityLinkRequestType.Disconnect:
                    linked = false;
                    Log("Disconnected from remote!");
                    break;
                case UdonityLinkRequestType.PushScene:
                    Log("Pushed scene changes to remote!");
                    break;
                case UdonityLinkRequestType.PullScene:
                    string response = result.Result;

                    Log($"Pulled scene from remote! Size: {response.Length} bytes");

                    LoadScene(response);
                    break;
            }

            currentRequestType = UdonityLinkRequestType.None;
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            LogError("Unable to send request to remote! Please ensure the UdonityLink host is running.");

            LogError($"Request error code: {result.ErrorCode}");

            currentRequestType = UdonityLinkRequestType.None;
        }

        private void SendRequest(UdonityLinkRequestType requestType)
        {
            if(currentRequestType != UdonityLinkRequestType.None)
            {
                LogPendingRequestWarning();

                return;
            }

            if(!linked && requestType != UdonityLinkRequestType.Connect)
            {
                LogNotConnectedWarning();

                return;
            }

            currentRequestType = requestType;

            VRCUrl url;

            switch (requestType)
            {
                case UdonityLinkRequestType.Connect: url = ConnectURL; break;
                case UdonityLinkRequestType.Disconnect: url = DisconnectURL; break;
                case UdonityLinkRequestType.PullScene: url = PullSceneURL; break;
                case UdonityLinkRequestType.PushScene: url = PushSceneURL; break;
                default: LogError("Invalid request type!"); return;
            }

            Log(string.Format("Sending request to remote: <color=#888>{0}</color>", url));

            VRCStringDownloader.LoadUrl(url, eventReceiver);
        }

        private void LoadScene(string sceneData)
        {
            // TODO: Implement new JSON based scene loading
            //SceneLoader.LoadScene(sceneData, udonAssetDatabase);

            lightingWindow.Refresh();
        }

        private void LogPendingRequestWarning()
        {
            LogWarning("A request is pending, please wait until the previous request is complete.");
        }

        private void LogNotConnectedWarning()
        {
            LogWarning("Remote connection is not active! Navigate to <b>UdonityLink > Connect</b> to connect to the UdonityLink host.");
        }

        private string FormatLog(string message)
        {
            return string.Concat(LOG_PREFIX, message);
        }

        private void Log(string message)
        {
            logger.Log(FormatLog(message));
        }

        private void LogWarning(string message)
        {
            logger.LogWarning(FormatLog(message));
        }

        private void LogError(string message)
        {
            logger.LogError(FormatLog(message));
        }
    }
}
