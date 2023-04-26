using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject InputName;
    public GameObject UpdatedText;
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

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
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
        if (!PlayerPrefs.HasKey("ShapesUpdate") || (System.DateTime.Now - System.Convert.ToDateTime(PlayerPrefs.GetString("ShapesUpdate"))).TotalHours >= 6)
            StartCoroutine(GetRequest("http://api.angryelfgames.com/angryelf/ShapesList"));

        if (PlayerPrefs.HasKey("Username"))
        {
            InputName.GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetString("Username");
            TBDGame.Username = PlayerPrefs.GetString("Username");
        }

        if (PlayerPrefs.HasKey("SettingsMusic"))
        {
            TBDGame.MusicOn = PlayerPrefs.GetInt("SettingsMusic") == 1;
        }

        if (PlayerPrefs.HasKey("SettingsSoundFX"))
        {
            TBDGame.SoundFXOn = PlayerPrefs.GetInt("SettingsSoundFX") == 1;
        }

        if (PlayerPrefs.HasKey("SettingsVisualAid"))
        {
            TBDGame.VisualAidOn = PlayerPrefs.GetInt("SettingsVisualAid") == 1;
        }

        TBDGame.UserID = SystemInfo.deviceUniqueIdentifier;

        SubmitTopScore(GameModes.Classic);
        SubmitTopScore(GameModes.Random);
        SubmitTopScore(GameModes.Twist);

        var base64EncodedBytes = System.Convert.FromBase64String(PlayerPrefs.GetString("ShapesList"));
        string json = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        List<Shape> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Shape>>(json);

        if (list != null && list.Count > 0)
        {
            Shapes.ShapesList = list;
            UpdatedText.GetComponent<TMPro.TextMeshProUGUI>().text = "Shapes loaded locally";
        }
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

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                List<Shape> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Shape>>(webRequest.downloadHandler.text);

                if (list != null && list.Count > 0)
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(webRequest.downloadHandler.text);
                    PlayerPrefs.SetString("ShapesList", System.Convert.ToBase64String(plainTextBytes));
                    PlayerPrefs.SetString("ShapesUpdate", System.DateTime.Now.ToString());

                    Shapes.ShapesList = list;

                    UpdatedText.GetComponent<TMPro.TextMeshProUGUI>().text = "Shapes loaded from Server";
                }
            }
        }
    }
}
