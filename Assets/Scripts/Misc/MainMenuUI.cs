using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    public Text hiScoreText;

   

    private void Start()
    {
        CanvasScaler cs = GetComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(Screen.width, Screen.height);

        //"HI-Score : 5430"
        hiScoreText.text = "HI-Score : " + HelperFs.ScoreFormat(GetHighScore());
    }


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public AudioMixer audioMixer;

    public void SetVolume(float value)
    {
        audioMixer.SetFloat("volume",(1-value)*-80f);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    int GetHighScore()
    {
        return PlayerPrefs.GetInt("hiScore", 0);
    }
    
    public void SelectSpaceShip(int selectedShip)
    {
        PlayerPrefs.SetInt("selectedShip", selectedShip);
    }


}
