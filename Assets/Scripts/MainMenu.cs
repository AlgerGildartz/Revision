using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {

        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 8;

        #endregion

        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        readonly string gameVersion = "1";

        /// <summary>
        /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
        /// Typically this is used for the OnConnectedToMaster() callback.
        /// </summary>
        bool isConnecting;

        #endregion

        #region Public Fields

        // Game objects
        [Tooltip("The Ui GameObject for main menu")]
        [SerializeField]
        private GameObject mainMenuGO;

        [Tooltip("The Ui GameObject to create a room")]
        [SerializeField]
        private GameObject creationGO;

        [Tooltip("The Ui GameObject to join a room")]
        [SerializeField]
        private GameObject joinGO;

        [Tooltip("The Ui GameObject for options")]
        [SerializeField]
        private GameObject optionsGO;

        // Creation objects
        [Tooltip("The Ui InputField to create a room")]
        [SerializeField]
        private UnityEngine.UI.InputField creationRoomName;

        [Tooltip("The Ui Text to show an error in a room creation")]
        [SerializeField]
        private UnityEngine.UI.Text creationError;

        // Join objects
        [Tooltip("The Ui InputField to join a room")]
        [SerializeField]
        private UnityEngine.UI.InputField joinRoomName;

        [Tooltip("The Ui Text to show an error in a room join")]
        [SerializeField]
        private UnityEngine.UI.Text joinError;

        //[Tooltip("The Ui Panel to let the user enter name, connect and play")]
        //[SerializeField]
        //private GameObject controlPanel;
        //[Tooltip("The UI Label to inform the user that the connection is in progress")]
        //[SerializeField]
        //private GameObject progressLabel;


        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            CameraCtrl.LockMouse(false);
            LoadMainMenu();
            Connect();
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (!PhotonNetwork.IsConnected)
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }

        }

        public void CreateRoom()
        {

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                RoomOptions roomOptions = new RoomOptions
                {
                    CustomRoomPropertiesForLobby = new string[] { "free" },
                    CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "free", Enumerable.Range(0, maxPlayersPerRoom).Select(x => x.ToString()).Aggregate("", (acc, x) => acc + x) } },
                    MaxPlayers = maxPlayersPerRoom
                };
                PhotonNetwork.CreateRoom(creationRoomName.text, roomOptions);
            }
            else
            {
                Connect();
                CreateRoom();
            }
        }

        public void JoinRoom()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRoom(joinRoomName.text);
            }
            else
            {
                Connect();
                JoinRoom();
            }
        }

        private void Load(int id)
        {
            mainMenuGO.SetActive(id == 1);
            creationGO.SetActive(id == 2);
            joinGO.SetActive(id == 3);
            optionsGO.SetActive(id == 4);
        }

        public void LoadMainMenu()
        {
            Load(1);
        }

        public void LoadCreation()
        {
            Load(2);
        }

        public void LoadJoin()
        {
            Load(3);
        }

        public void LoadOption()
        {
            Load(4);
        }

        public void Quit()
        {
            Application.Quit();
        }
        #endregion



        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnDisconnected(DisconnectCause cause)
        {
            isConnecting = false;
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                // #Critical
                // Load the Level.
                PhotonNetwork.LoadLevel("GameWorld_Inside");
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            creationError.gameObject.SetActive(true);
            creationError.text = string.Format("{0} : {1}", returnCode, message);
            base.OnCreateRoomFailed(returnCode, message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            joinError.gameObject.SetActive(true);
            joinError.text = string.Format("{0} : {1}", returnCode, message);
            base.OnJoinRoomFailed(returnCode, message);
        }


        #endregion
    }
}
