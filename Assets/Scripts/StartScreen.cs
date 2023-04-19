using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject InputName;
    public void StartClassic()
    {
        if (!string.IsNullOrEmpty(TBDGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != TBDGame.Username)
        {
            TBDGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            PlayerPrefs.SetString("Username", TBDGame.Username);
        }

        TBDGame.GameMode = GameModes.Classic;
        SceneManager.LoadScene("TBDGame");
    }

    public void StartClassicPlus()
    {
        if (!string.IsNullOrEmpty(TBDGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != TBDGame.Username)
        {
            TBDGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            PlayerPrefs.SetString("Username", TBDGame.Username);
        }

        TBDGame.GameMode = GameModes.Twist;
        SceneManager.LoadScene("TBDGame");
    }

    public void StartRandom()
    {
        if (!string.IsNullOrEmpty(TBDGame.Username) || InputName.GetComponent<TMPro.TMP_InputField>().text != TBDGame.Username)
        {
            TBDGame.Username = InputName.GetComponent<TMPro.TMP_InputField>().text;
            PlayerPrefs.SetString("Username", TBDGame.Username);
        }

        TBDGame.GameMode = GameModes.Random;
        SceneManager.LoadScene("TBDGame");
    }

    public void PatchNotes()
    {
        SceneManager.LoadScene("PatchNotes");
    }

    public void SupportDeveloper()
    {
        SceneManager.LoadScene("Paypal");
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

        TBDGame.UserID = SystemInfo.deviceUniqueIdentifier;

        SubmitTopScore(GameModes.Classic);
        SubmitTopScore(GameModes.Random);
        SubmitTopScore(GameModes.Twist);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SubmitTopScore(GameModes gameMode)
    {
        if (PlayerPrefs.HasKey("Highscores" + gameMode.ToString()))
        {
            Highscores.HighScoreList list = JsonUtility.FromJson<Highscores.HighScoreList>(PlayerPrefs.GetString("Highscores" + gameMode.ToString()));

            if (list != null)
            {
                int highScoreIndex = 0;
                int highScore = 0;

                for (int i = 0; i < list.highscores.Length; i++)
                {
                    if (list.highscores[i].score > highScore)
                    {
                        highScore = list.highscores[i].score;
                        highScoreIndex = i;
                    }
                }

                StartCoroutine(PostRequest("http://api.angryelfgames.com/AngryElf/InputHighScore", list.highscores[highScoreIndex], gameMode));
            }
        }
    }

    IEnumerator PostRequest(string uri, Highscores.HighScoreResponse highScore, GameModes gameMode)
    {
        TBDGame.PostData postData = new TBDGame.PostData();
        postData.Username = highScore.username;
        postData.Score = highScore.score;
        postData.GameMode = (int)gameMode;
        postData.UserID = TBDGame.UserID;
        postData.Secret = "06ec43b7-923f-4deb-b8b0-6f0c1b85cee7";

        string json = JsonUtility.ToJson(postData);

        using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, json))
        {
            webRequest.method = "POST";
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
        }
    }
}
