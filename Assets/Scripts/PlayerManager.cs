using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("Camera for the current player")]
        [SerializeField]
        private Camera myCamera;

        [Tooltip("All the tops instances")]
        [SerializeField]
        private GameObject[] tops;

        [Tooltip("All the bottoms instances")]
        [SerializeField]
        private GameObject[] bottoms;

        [Tooltip("All the shoes instances")]
        [SerializeField]
        private GameObject[] shoes;

        // Start is called before the first frame update
        void Start()
        {
            if (photonView.IsMine)
            {
                myCamera.gameObject.SetActive(true);
                LocalPlayerInstance = gameObject;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Enable the game object by giving it the right parent for good positioning
        /// </summary>
        /// <param name="parent">id of the parent</param>
        [PunRPC]
        public void DoEnable(int parent, ExitGames.Client.Photon.Hashtable prefs)
        {
            // Look for the parrent GameObject
            GameObject parentGO = GameObject.Find(string.Format("Student {0}", parent));
            // update the transforma values
            Transform prefab = transform;
            prefab.parent = parentGO.transform;
            prefab.localPosition = Vector3.zero;
            prefab.localRotation = Quaternion.identity;
            Customize(prefs);
        }

        private void Customize(ExitGames.Client.Photon.Hashtable properties)
        {
            foreach (GameObject top in tops.Where(x => !x.name.Contains(properties[PlayerOptions.playerTopPrefKey] as string)))
            {
                top.SetActive(false);
            }

            foreach (GameObject bottom in bottoms.Where(x => !x.name.Contains(properties[PlayerOptions.playerBottomPrefKey] as string)))
            {
                bottom.SetActive(false);
            }

            foreach (GameObject shoe in shoes.Where(x => !x.name.Contains(properties[PlayerOptions.playerShoePrefKey] as string)))
            {
                shoe.SetActive(false);
            }
        }
    }
}
