using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.SceneManagement;

public class Highscores : MonoBehaviour
{
    public GameObject Names;
    public GameObject Scores;
    public GameObject GameModeButton;

    TMPro.TextMeshProUGUI names;
    TMPro.TextMeshProUGUI scores;

    private bool justMe = true;
    private int gameMode = TBDGame.GameMode;

    public void StartScreen()
    {
        SceneManager.LoadScene("StartScreen");

        if (gameMode == 2)
        {
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Classic";
        }
        else
        {
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Random";
        }
    }

    public void ChangeGameMode()
    {
        if (gameMode == 1)
        {
            gameMode = 2;
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Classic";
        }
        else
        {
            gameMode = 1;
            GameModeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Random";
        }
        if (justMe)
            GetJustMe();
        else
            GetAll();
    }

    public void GetAll()
    {
        justMe = false;
        StartCoroutine(GetRequest("http://api.angryelfgames.com/angryelf/gethighscores"));
    }

    public void GetJustMe()
    {
        justMe = true;
        StartCoroutine(GetRequest("http://api.angryelfgames.com/angryelf/gethighscores/" + TBDGame.UserID));
    }

    // Start is called before the first frame update
    void Start()
    {
        names = Names.GetComponent<TMPro.TextMeshProUGUI>();
        scores = Scores.GetComponent<TMPro.TextMeshProUGUI>();

        names.text = string.Empty;
        scores.text = string.Empty;

        StartCoroutine(GetRequest("http://api.angryelfgames.com/angryelf/gethighscores/" + TBDGame.UserID));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                RootObject response = JsonUtility.FromJson<RootObject>("{\"highscores\":" + webRequest.downloadHandler.text + "}");
                
                if (response != null)
                {
                    List<string> responseNames = response.highscores.Where(y => y.gameMode == gameMode).Select(x => x.username).ToList();
                    names.text = String.Join(Environment.NewLine, responseNames);

                    List<int> responseScores = response.highscores.Where(y => y.gameMode == gameMode).Select(x => x.score).ToList();
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
    public class RootObject
    {
        public HighScoreResponse[] highscores;
    }
}
