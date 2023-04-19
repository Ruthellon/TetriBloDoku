using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paypal : MonoBehaviour
{
    public GameObject PatchNotesText;

    public void Home()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void GoToAAG()
    {
        Application.OpenURL("https://www.angryelfgames.com");
    }

    public void GoToPaypal()
    {
        Application.OpenURL("https://www.paypal.com/paypalme/AngryElfGames");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
