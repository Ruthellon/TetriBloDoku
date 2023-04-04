using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject InputName;
    public void StartClassic()
    {
        if (!string.IsNullOrEmpty(RandomGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != RandomGame.Username)
        {
            RandomGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            PlayerPrefs.SetString("Username", RandomGame.Username);
        }

        SceneManager.LoadScene("Classic");
    }

    public void StartRandom()
    {
        if (!string.IsNullOrEmpty(RandomGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != RandomGame.Username)
        {
            RandomGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            PlayerPrefs.SetString("Username", RandomGame.Username);
        }

        SceneManager.LoadScene("Random");
    }

    public void HighScores()
    {
        if (!string.IsNullOrEmpty(RandomGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != RandomGame.Username)
        {
            RandomGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            PlayerPrefs.SetString("Username", RandomGame.Username);
        }

        SceneManager.LoadScene("HighScores");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            InputName.GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetString("Username");
            RandomGame.Username = PlayerPrefs.GetString("Username");
        }

        if (PlayerPrefs.HasKey("UserID"))
        {
            RandomGame.UserID = PlayerPrefs.GetString("UserID");
        }
        else
        {
            RandomGame.UserID = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("UserID", RandomGame.UserID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
