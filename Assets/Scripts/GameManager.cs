using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Private fields

        [Tooltip("The Inputs for the question and answers in order (question -> good answer -> wrong answers)")]
        [SerializeField]
        private InputField[] writingInputs;

        [Tooltip("The GameObject for the writing zone")]
        [SerializeField]
        private GameObject writingZone;

        [Tooltip("The GameObject for the question zone")]
        [SerializeField]
        private GameObject questionZone;


        #endregion

        #region Public Fields

        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the player male")]
        public GameObject playerPrefabMale;

        [Tooltip("The prefab to use for representing the player female")]
        public GameObject playerPrefabFemale;

        #endregion

        #region MonoBehaviour CallBacks

        // Start is called before the first frame update
        private void Awake()
        {
            Instance = this;
        }


        void Start()
        {
            Instance = this;
            CameraCtrl.LockMouse(true);
            if (playerPrefabMale == null || playerPrefabFemale == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    // Get the property with the availables numbers
                    string tmp = PhotonNetwork.CurrentRoom.CustomProperties["free"] as string;
                    // Transform it to a char list for easier manipulation
                    List<char> freeNumbers = tmp.ToCharArray().ToList();
                    // Choose a random available position
                    // TODO
                    int r = int.Parse(freeNumbers[Random.Range(0, freeNumbers.Count())].ToString());
                    // Remove the spot
                    freeNumbers.Remove(char.Parse(r.ToString()));
                    // Update the list
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "free", new string(freeNumbers.ToArray()) } });
                    // Give the number to the player
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "assignedNumber", r } });
                    // Get options
                    ExitGames.Client.Photon.Hashtable prefs = GetPlayersPrefs();
                    // Instanciate the avatar for the player
                    GameObject prefab;
                    if ((prefs[PlayerOptions.playerGenderPrefKey] as string).Equals("Female"))
                        prefab = PhotonNetwork.Instantiate(this.playerPrefabFemale.name, Vector3.zero, Quaternion.identity, 0);
                    else
                        prefab = PhotonNetwork.Instantiate(this.playerPrefabMale.name, Vector3.zero, Quaternion.identity, 0);
                    // Call an RPC method
                    PhotonView.Get(prefab).RPC("DoEnable", RpcTarget.AllBuffered, r, prefs);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }


        #endregion

        #region Private methods

        private ExitGames.Client.Photon.Hashtable GetPlayersPrefs()
        {
            return new ExitGames.Client.Photon.Hashtable {
                { PlayerOptions.playerNamePrefKey, PlayerPrefs.GetString(PlayerOptions.playerNamePrefKey)},
                { PlayerOptions.playerGenderPrefKey, PlayerPrefs.GetString(PlayerOptions.playerGenderPrefKey,"Male")},
                { PlayerOptions.playerTopPrefKey, PlayerPrefs.GetString(PlayerOptions.playerTopPrefKey,"black")},
                { PlayerOptions.playerBottomPrefKey, PlayerPrefs.GetString(PlayerOptions.playerBottomPrefKey,"black")},
                { PlayerOptions.playerShoePrefKey, PlayerPrefs.GetString(PlayerOptions.playerShoePrefKey,"black")}
            };
        }

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Get the assigned number for that player
                int i = (int)otherPlayer.CustomProperties["assignedNumber"];
                // Get the list of availables numbers
                string tmp = PhotonNetwork.CurrentRoom.CustomProperties["free"] as string;
                List<char> freeNumbers = tmp.ToCharArray().ToList();
                // Add the newly available number
                freeNumbers.Add(char.Parse(i.ToString()));
                // Update the corresponding property
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "free", new string(freeNumbers.ToArray()) } });
            }
            base.OnPlayerLeftRoom(otherPlayer);
        }

        #endregion

        #region Public methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void SubmitQuestion()
        {
            foreach (InputField inputField in writingInputs)
            {
                if (string.IsNullOrWhiteSpace(inputField.text))
                {
                    return;
                }
            }
            PhotonView.Get(questionZone).RPC("SetQuestion", RpcTarget.All, writingInputs[0].text, writingInputs.Skip(1).Select(x => x.text).ToArray());
            clearText();
            PlayerManager.LocalPlayerInstance.GetComponentInChildren<CameraCtrl>().LookBook(false);
        }

        public void ShowQuestionZone(bool state)
        {
            writingZone.SetActive(state);
        }

        private void clearText()
        {
            foreach (InputField inputField in writingInputs)
            {
                inputField.text = "";
            }
        }
        #endregion
    }
}
