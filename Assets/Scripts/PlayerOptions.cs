using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOptions : MonoBehaviour
{
    [SerializeField]
    private InputField inputName;
    [SerializeField]
    private Dropdown dropGender;
    [SerializeField]
    private Dropdown dropTop;
    [SerializeField]
    private Dropdown dropBottom;
    [SerializeField]
    private Dropdown dropShoe;

    public static string playerNamePrefKey = "Nickname";
    public static string playerGenderPrefKey = "Gender";
    public static string playerTopPrefKey = "TopColor";
    public static string playerBottomPrefKey = "BottomColor";
    public static string playerShoePrefKey = "ShoesColor";

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(playerNamePrefKey))
        {
            inputName.text = PlayerPrefs.GetString(playerNamePrefKey);
        }
        if (PlayerPrefs.HasKey(playerGenderPrefKey))
        {
            dropGender.value = dropGender.options.FindIndex(x => x.text == PlayerPrefs.GetString(playerGenderPrefKey));
        }
        if (PlayerPrefs.HasKey(playerTopPrefKey))
        {
            dropTop.value = dropTop.options.FindIndex(x => x.text == PlayerPrefs.GetString(playerTopPrefKey));
        }
        if (PlayerPrefs.HasKey(playerBottomPrefKey))
        {
            dropBottom.value = dropBottom.options.FindIndex(x => x.text == PlayerPrefs.GetString(playerBottomPrefKey));
        }
        if (PlayerPrefs.HasKey(playerShoePrefKey))
        {
            dropShoe.value = dropShoe.options.FindIndex(x => x.text == PlayerPrefs.GetString(playerShoePrefKey));
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Public Methods
    public void SetName()
    {
        if (!string.IsNullOrWhiteSpace(inputName.text))
        {
            PhotonNetwork.NickName = inputName.text;
            PlayerPrefs.SetString(playerNamePrefKey, inputName.text);
        }
    }

    public void SetGender()
    {
        if (!string.IsNullOrWhiteSpace(dropGender.captionText.text))
        {
            PlayerPrefs.SetString(playerGenderPrefKey, dropGender.captionText.text);
        }
    }

    public void SetTop()
    {
        if (!string.IsNullOrWhiteSpace(dropTop.captionText.text))
        {
            PlayerPrefs.SetString(playerTopPrefKey, dropTop.captionText.text);
        }
    }

    public void SetBottom()
    {
        if (!string.IsNullOrWhiteSpace(dropBottom.captionText.text))
        {
            PlayerPrefs.SetString(playerBottomPrefKey, dropBottom.captionText.text);
        }
    }

    public void SetShoes()
    {
        if (!string.IsNullOrWhiteSpace(dropShoe.captionText.text))
        {
            PlayerPrefs.SetString(playerShoePrefKey, dropShoe.captionText.text);
        }
    }

    #endregion
}
