using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class Highscores : MonoBehaviour
{
    public GameObject Names;
    public GameObject Scores;
    public GameObject GameModeButton;
    public GameObject UserModeButton;

    TMPro.TextMeshProUGUI names;
    TMPro.TextMeshProUGUI scores;

    private bool justMe = true;
    private GameModes gameMode = TBDGame.GameMode;

    public void StartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void ChangeGameMode()
    {
        if (gameMode == GameModes.Random)
        {
            gameMode = GameModes.Classic;
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Classic";
        }
        else if (gameMode == GameModes.Twist)
        {
            gameMode = GameModes.Random;
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Random";
        }
        else if (gameMode == GameModes.Classic)
        {
            gameMode = GameModes.Twist;
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Twist";
        }
        
        if (justMe)
            GetLocalScores();
        else
            StartCoroutine(GetRequest("http://api.angryelfgames.com/angryelf/gethighscores"));
    }

    public void ChangeUserMode()
    {
        if (justMe)
        {
            justMe = false;
            UserModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Global";
            StartCoroutine(GetRequest("http://api.angryelfgames.com/angryelf/gethighscores"));
        }
        else
        {
            justMe = true;
            UserModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Just Me";
            GetLocalScores();
        }
    }

    public void ClearHighScores()
    {
        if (justMe)
        {
            if (PlayerPrefs.HasKey("Highscores" + gameMode.ToString()))
            {
                PlayerPrefs.DeleteKey("Highscores" + gameMode.ToString());
            }

            GetLocalScores();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        names = Names.GetComponent<TMPro.TextMeshProUGUI>();
        scores = Scores.GetComponent<TMPro.TextMeshProUGUI>();

        names.text = string.Empty;
        scores.text = string.Empty;

        if (gameMode == GameModes.Classic)
        {
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Classic";
        }
        else if (gameMode == GameModes.Random)
        {
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Random";
        }
        else if (gameMode == GameModes.Twist)
        {
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Twist";
        }

        GetLocalScores();

        //StartCoroutine(GetRequest("http://api.angryelfgames.com/angryelf/gethighscores/" + TBDGame.UserID));
    }

    void GetLocalScores()
    {
        if (PlayerPrefs.HasKey("Highscores" + gameMode.ToString()))
        {
            HighScoreList list = JsonUtility.FromJson<HighScoreList>(PlayerPrefs.GetString("Highscores" + gameMode.ToString()));

            if (list != null)
            {
                List<string> responseNames = list.highscores.Where(y => y.gameMode == (int)gameMode).Select(x => x.username).ToList();
                names.text = String.Join(Environment.NewLine, responseNames);

                List<int> responseScores = list.highscores.Where(y => y.gameMode == (int)gameMode).Select(x => x.score).ToList();
                scores.text = String.Join(Environment.NewLine, responseScores);
            }
        }
        else
        {
            names.text = "";
            scores.text = "";
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                HighScoreList response = JsonUtility.FromJson<HighScoreList>("{\"highscores\":" + webRequest.downloadHandler.text + "}");
                
                if (response != null)
                {
                    List<HighScoreResponse> highscores = response.highscores.Where(x => x.gameMode == (int)gameMode).ToList();
                    highscores.RemoveAll(x => 
                        x.username.ToLower() == "ass" ||
                        x.username.ToLower() == "cum" ||
                        x.username.ToLower() == "fag" ||
                        x.username.ToLower() == "gay" ||
                        x.username.ToLower() == "jew" ||
                        x.username.ToLower() == "tit" ||
                        x.username.ToLower() == "cunt" ||
                        x.username.ToLower() == "fuck" ||
                        x.username.ToLower() == "feck" ||
                        x.username.ToLower() == "fick" ||
                        x.username.ToLower() == "fock" ||
                        x.username.ToLower() == "foak" ||
                        x.username.ToLower() == "jism" ||
                        x.username.ToLower() == "gism" ||
                        x.username.ToLower() == "jizz" ||
                        x.username.ToLower() == "shit" ||
                        x.username.ToLower() == "slut" ||
                        x.username.ToLower() == "twat" ||
                        x.username.ToLower() == "tits");

                    List<string> responseNames = highscores.Select(x => x.username.Substring(0,4)).ToList();
                    names.text = String.Join(Environment.NewLine, responseNames);

                    List<int> responseScores = highscores.Select(x => x.score).ToList();
                    scores.text = String.Join(Environment.NewLine, responseScores);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public class HighScoreResponse
    {
        public string username;
        public int score;
        public int gameMode;
        public DateTime dateAchieved;
    }

    [Serializable]
    public class HighScoreList
    {
        public HighScoreResponse[] highscores;
    }
}
