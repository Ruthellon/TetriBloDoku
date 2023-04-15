using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject InputName;
    public void StartClassic()
    {
        if (!string.IsNullOrEmpty(TBDGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != TBDGame.Username)
        {
            TBDGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            TBDGame.GameMode = GameModes.Classic;
            PlayerPrefs.SetString("Username", TBDGame.Username);
        }

        SceneManager.LoadScene("TBDGame");
    }

    public void StartRandom()
    {
        if (!string.IsNullOrEmpty(TBDGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != TBDGame.Username)
        {
            TBDGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            TBDGame.GameMode = GameModes.Random;
            PlayerPrefs.SetString("Username", TBDGame.Username);
        }

        SceneManager.LoadScene("TBDGame");
    }

    public void PatchNotes()
    {
        SceneManager.LoadScene("PatchNotes");
    }

    public void HighScores()
    {
        if (!string.IsNullOrEmpty(TBDGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != TBDGame.Username)
        {
            TBDGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            PlayerPrefs.SetString("Username", TBDGame.Username);
        }

        SceneManager.LoadScene("HighScores");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            InputName.GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetString("Username");
            TBDGame.Username = PlayerPrefs.GetString("Username");
        }

        if (PlayerPrefs.HasKey("UserID"))
        {
            TBDGame.UserID = PlayerPrefs.GetString("UserID");
        }
        else
        {
            TBDGame.UserID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("UserID", TBDGame.UserID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
