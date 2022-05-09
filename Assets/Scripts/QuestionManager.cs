using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class QuestionManager : MonoBehaviour
    {
        #region Private fields
        [Tooltip("Question placeholder")]
        [SerializeField]
        private Text questionGO;

        [Tooltip("The buttons for answers")]
        [SerializeField]
        private Button[] buttonsGO;

        [Tooltip("Timer placeholder")]
        [SerializeField]
        private Text timerGO;

        [Tooltip("Alert placeholder")]
        [SerializeField]
        private Text alertGO;

        private float timer;
        private bool hasQuestion = false;
        private bool isShowingAnswer = false;
        private int goodAnswer;
        private int selected = -1;

        const float TIME_FOR_QUESTION = 30f;
        const float TIME_FOR_GOOD_ANSWER = 5f;

        #endregion

        #region MonoBehaviour CallBacks

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (hasQuestion)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    timerGO.text = timer.ToString("f");
                }
                else
                {
                    hasQuestion = false;
                    ShowGoodAnswer();
                }
            }
            if (isShowingAnswer)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    timerGO.text = timer.ToString("f");
                }
                else
                {
                    isShowingAnswer = false;
                    EndOfQuestion();
                }
            }
        }

        #endregion

        #region Private methods

        private void SetAll(bool value)
        {
            questionGO.gameObject.SetActive(value);
            timerGO.gameObject.SetActive(value);
            foreach (Button b in buttonsGO)
            {
                b.gameObject.SetActive(value);
            }
        }

        private void EndOfQuestion()
        {
            if (selected != -1)
                buttonsGO[selected].GetComponent<Image>().color = Color.white;
            buttonsGO[goodAnswer].GetComponent<Image>().color = Color.white;
            SetAll(false);
            selected = -1;
        }

        private void ShowGoodAnswer()
        {
            if (selected != -1)
                buttonsGO[selected].GetComponent<Image>().color = Color.red;
            buttonsGO[goodAnswer].GetComponent<Image>().color = Color.green;
            timer = TIME_FOR_GOOD_ANSWER;
            isShowingAnswer = true;
        }

        private int CalculateFontSize(int length)
        {
            int def_val = 50;
            if (length <= 25)
                return def_val;
            else
            {
                // 26  ...
                int val = length - 25;
                int sub = (int)Mathf.Ceil((float)val / 5);
                return def_val - sub;
            }
        }

        #endregion

        #region Public methods

        public void SelectAnswer(int i)
        {
            if (selected != -1)
                buttonsGO[selected].GetComponent<Image>().color = Color.white;
            selected = i;
            buttonsGO[selected].GetComponent<Image>().color = Color.yellow;
        }

        [PunRPC]
        public void SetQuestion(string question, string[] answers)
        {
            if (!hasQuestion && !isShowingAnswer)
            {
                SetAll(true);
                questionGO.text = question;
                for (int i = 0; i < buttonsGO.Length; i++)
                {
                    buttonsGO[i].GetComponentInChildren<Text>().fontSize = CalculateFontSize(answers[i].Length);
                    buttonsGO[i].GetComponentInChildren<Text>().text = answers[i];
                }
                goodAnswer = 0;
                timer = TIME_FOR_QUESTION;
                hasQuestion = true;
                CameraCtrl.instance.LookBook(false);
            }
        }

        #endregion
    }
}
