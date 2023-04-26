using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public GameObject MusicEnabled;
    public GameObject SoundFXEnabled;
    public GameObject VisualAidEnabled;

    public GameObject ScrollViewText;

    public GameObject UpdateText;

    public Sprite ToggleOn;
    public Sprite ToggleOff;

    public Sprite ToggleSoundOn;
    public Sprite ToggleSoundOff;

    public void Home()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void Return()
    {
        SceneManager.LoadScene("TBDGame");
    }

    public void ReturnSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void DevSettings()
    {
        SceneManager.LoadScene("DevSettings");
    }

    public void ToggleMusic()
    {
        if (TBDGame.MusicOn)
        {
            MusicEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOff;
            TBDGame.MusicOn = false;
        }
        else
        {
            MusicEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOn;
            TBDGame.MusicOn = true;
        }

        PlayerPrefs.SetInt("SettingsMusic", TBDGame.MusicOn ? 1 : 0);
    }

    public void ToggleSoundFX()
    {
        if (TBDGame.SoundFXOn)
        {
            SoundFXEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOff;
            TBDGame.SoundFXOn = false;
        }
        else
        {
            SoundFXEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOn;
            TBDGame.SoundFXOn = true;
        }

        PlayerPrefs.SetInt("SettingsSoundFX", TBDGame.SoundFXOn ? 1 : 0);
    }

    public void ToggleVisualAid()
    {
        if (TBDGame.VisualAidOn)
        {
            VisualAidEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleOff;
            TBDGame.VisualAidOn = false;
        }
        else
        {
            VisualAidEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleOn;
            TBDGame.VisualAidOn = true;
        }

        PlayerPrefs.SetInt("SettingsVisualAid", TBDGame.VisualAidOn ? 1 : 0);
    }

    public void DeleteGameData()
    {
        PlayerPrefs.DeleteKey("ShapesList");
        PlayerPrefs.DeleteKey("ShapesUpdate");
        PlayerPrefs.DeleteKey("Username"); 
        PlayerPrefs.DeleteKey("BoardState" + GameModes.Random.ToString());
        PlayerPrefs.DeleteKey("Score" + GameModes.Random.ToString());
        PlayerPrefs.DeleteKey("Shape1" + GameModes.Random.ToString());
        PlayerPrefs.DeleteKey("Shape2" + GameModes.Random.ToString());
        PlayerPrefs.DeleteKey("Shape3" + GameModes.Random.ToString());
        PlayerPrefs.DeleteKey("BoardState" + GameModes.Classic.ToString());
        PlayerPrefs.DeleteKey("Score" + GameModes.Classic.ToString());
        PlayerPrefs.DeleteKey("Shape1" + GameModes.Classic.ToString());
        PlayerPrefs.DeleteKey("Shape2" + GameModes.Classic.ToString());
        PlayerPrefs.DeleteKey("Shape3" + GameModes.Classic.ToString());
        PlayerPrefs.DeleteKey("BoardState" + GameModes.Twist.ToString());
        PlayerPrefs.DeleteKey("Score" + GameModes.Twist.ToString());
        PlayerPrefs.DeleteKey("Shape1" + GameModes.Twist.ToString());
        PlayerPrefs.DeleteKey("Shape2" + GameModes.Twist.ToString());
        PlayerPrefs.DeleteKey("Shape3" + GameModes.Twist.ToString());

        if (UpdateText != null)
        {
            UpdateText.GetComponent<TMPro.TextMeshProUGUI>().text = "Data Deleted...";
        }
    }

    private string scrollText = string.Empty;
    // Start is called before the first frame update
    void Start()
    {
        if (MusicEnabled != null)
        {
            if (TBDGame.MusicOn)
                MusicEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOn;
            else
                MusicEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOff;
        }

        if (SoundFXEnabled != null)
        {
            if (TBDGame.SoundFXOn)
                SoundFXEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOn;
            else
                SoundFXEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleSoundOff;
        }

        if (VisualAidEnabled != null)
        {
            if (TBDGame.VisualAidOn)
                VisualAidEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleOn;
            else
                VisualAidEnabled.GetComponent<UnityEngine.UI.Image>().sprite = ToggleOff;
        }

        int[] shapes = new int[Shapes.ShapesList.Count];
        for (int i = 0; i < 100000; i++)
        {
            int shape = UnityEngine.Random.Range(0, Shapes.ShapesList.Sum(x => x.Frequency));
            int chosenShape = -1;
            int runningFrequency = 0;
            for (int j = 0; j < Shapes.ShapesList.Count; j++)
            {
                runningFrequency += Shapes.ShapesList[j].Frequency;

                if (runningFrequency >= shape)
                {
                    chosenShape = j;
                    break;
                }
            }

            shapes[chosenShape]++;
        }

        if (ScrollViewText != null)
        {
            scrollText = string.Join(System.Environment.NewLine, Shapes.ShapesList.Select((x, i) => i.ToString() + System.Environment.NewLine
            + "    Frequency: " + x.Frequency + System.Environment.NewLine
            + "    Quadrants: " + x.Quadrants + System.Environment.NewLine
            + "    Theoretical: " + (((float)x.Frequency / (float)Shapes.ShapesList.Sum(y => y.Frequency)) * 100) + "%" + System.Environment.NewLine
            + "    Actual: " + (((float)shapes[i] / (float)shapes.Sum(y => y)) * 100) + "%" + System.Environment.NewLine
            + "    I: " + string.Join(",", x.I) + System.Environment.NewLine
            + "    II: " + (x.II != null ? string.Join(",", x.II) : "") + System.Environment.NewLine
            + "    III: " + (x.III != null ? string.Join(",", x.III) : "") + System.Environment.NewLine
            + "    IV: " + (x.IV != null ? string.Join(",", x.IV) : "")));

            ScrollViewText.GetComponent<TMPro.TextMeshProUGUI>().text = scrollText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
