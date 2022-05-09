using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class BoardCamera : MonoBehaviour
    {
        public static Camera from;
        public static BoardCamera instance;
        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            SetActiveState(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SetActiveState(false);
                from.gameObject.SetActive(true);
                CameraCtrl.LockMouse(true);
            }
        }

        public void SetActiveState(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
