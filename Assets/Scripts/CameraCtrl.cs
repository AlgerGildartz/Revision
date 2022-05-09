using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Inspired from https://forum.unity.com/threads/looking-with-the-mouse.109250/

namespace Com.MyCompany.MyGame
{
    public class CameraCtrl : MonoBehaviour
    {


        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;

        public float minimumX = -360F;
        public float maximumX = 360F;

        public float minimumY = -60F;
        public float maximumY = 60F;

        float rotationY = 0F;

        bool wasMouseDown = false;
        bool isWriting = false;

        readonly Vector3 BOOK_BASE_POS = Vector3.zero;
        readonly Vector3 BOOK_UP_POS = new Vector3(0, 0.5f, -0.6f);
        readonly Vector3 BOOK_BASE_ANGLE = Vector3.up * 180;
        readonly Vector3 BOOK_UP_ANGLE = new Vector3(90, 180, 0);

        public static CameraCtrl instance;

        private GameObject notepad = null;
        private GameObject objectHeld = null;
        private float objectHeldDist = 0.0f;

        void Update()
        {
            if (!Input.GetKey(KeyCode.LeftAlt) && !isWriting)
            {
                if (axes == RotationAxes.MouseXAndY)
                {
                    float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                }
                else if (axes == RotationAxes.MouseX)
                {
                    transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
                }
                else
                {
                    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                    transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
                }
            }
            if (Input.GetKey(KeyCode.Escape) && isWriting)
            {
                LookBook(false);
            }

            // Allows control of the distance
            objectHeldDist += Input.GetAxis("Mouse ScrollWheel") * 2;
        }

        void FixedUpdate()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1.5f, Color.white);
            if (Input.GetMouseButton(0))
            {
                if (!wasMouseDown && !isWriting)
                {
                    wasMouseDown = true;
                    // Try for button
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1 << 5))
                    {
                        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                        hit.transform.GetComponent<Button>().onClick.Invoke();
                    }
                    // Try for projector
                    else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1 << 9))
                    {
                        LockMouse(false);
                        gameObject.SetActive(false);
                        BoardCamera.instance.SetActiveState(true);
                        BoardCamera.from = GetComponent<Camera>();

                        //Debug.Log("Did not Hit");
                    }
                    // Try for book
                    else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1.5f, 1 << 8))
                    {
                        notepad = hit.transform.gameObject;
                        LookBook(true);
                    }
                    else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 3f, 1 << 10))
                    {
                        objectHeld = hit.transform.gameObject;
                        objectHeldDist = hit.distance;
                        objectHeld.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
            }
            else
            {
                wasMouseDown = false;
                if (objectHeld != null)
                {
                    objectHeld.GetComponent<Rigidbody>().useGravity = true;
                    objectHeld = null;
                }
            }

            

            if (objectHeld != null)
                objectHeld.GetComponent<Rigidbody>().MovePosition(GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, objectHeldDist)));
        }

        void Start()
        {
            instance = this;
            GameManager.Instance.ShowQuestionZone(false);
        }

        public void LookBook(bool state)
        {
            if (notepad != null)
            {
                if (state)
                {
                    notepad.GetComponent<Rigidbody>().useGravity = false;
                    isWriting = true;
                    notepad.transform.localPosition = BOOK_UP_POS;
                    notepad.transform.localEulerAngles = BOOK_UP_ANGLE;
                    transform.localEulerAngles = Vector3.zero;
                    rotationY = 0;
                    LockMouse(false);
                    GameManager.Instance.ShowQuestionZone(true);
                }
                else
                {
                    isWriting = false;
                    notepad.transform.localPosition = BOOK_BASE_POS;
                    notepad.transform.localEulerAngles = BOOK_BASE_ANGLE;
                    notepad.GetComponent<Rigidbody>().useGravity = true;
                    LockMouse(true);
                    GameManager.Instance.ShowQuestionZone(false);
                }
            }
        }

        public static void LockMouse(bool state)
        {
            if (state)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }
}