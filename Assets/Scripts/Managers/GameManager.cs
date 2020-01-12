using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Controls In game HUD and general game progress
/// Contains game References and settings
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("Sprite References")]
    public Sprite Sprite_BulletExplodeRed;
    public Sprite Sprite_BulletExplodeWhite;
    public Sprite Sprite_BulletExplodeLittle;

    [Header("HUD References")]
    public Text scoreText;
    public Text hiScoreText;
    public Text remainingLifeText;
    public Text mainMessageText;

    public Transform PowerUpsParent;

    public Transform fastMovementTimeT { get; private set; }
    public Transform doubleBarrelTimeT { get; private set; }
    public Transform fastShootTimeT { get; private set; }
    [Header("Power Up Scriptable Objects")]
    public List<PowerUpData> powerUpDatas;

    [Header("General References")]
    public GameObject [] playerShipPrefabs;
    public PlayerController pController;
    public Transform shipStartingPoint;


    [Header("Settings")]

    public int score = 0;
    public int hiScore = 0;
    public int remainingLife = 3;
    public int maxWaveCountForFinishingGame=2;
    private int currentInvaderWave = 0;

    public float waitTimeForMessage = 3f;

    public List<MessageInfo> messageInfos;

    public bool isGamePaused { get; private set; } = false;

    int selectedShip = 0;

    public float xEdge { get; private set; } = 6.7f;

    Coroutine currentCoroutine;

    void Awake()
    {
        InitHUD();

        //Entry Message
        ShowMessage(messageInfos,0,2);

        LoadPlayerShip();
        xEdge = Camera.main.orthographicSize * Camera.main.aspect - (pController.GetComponent<BoxCollider2D>().bounds.size.x / 2f);

    }

    private void LoadPlayerShip()
    {
        selectedShip = PlayerPrefs.GetInt("selectedShip", 0);
        pController = Instantiate(playerShipPrefabs[selectedShip], shipStartingPoint.position, Quaternion.identity).GetComponent<PlayerController>();
    }

    void InitHUD()
    {
        hiScore = PlayerPrefs.GetInt("hiScore", 0);
        hiScoreText.text = HelperFs.ScoreFormat(hiScore);
        remainingLifeText.text = remainingLife.ToString();

        fastMovementTimeT = PowerUpsParent.GetChild(0);
        doubleBarrelTimeT = PowerUpsParent.GetChild(1);
        fastShootTimeT = PowerUpsParent.GetChild(2);
    }

    void EnableMessage(bool active)
    {
        mainMessageText.transform.parent.gameObject.SetActive(active);
    }

    void ShowMessage(List<MessageInfo> infos, int startIndex, int stopIndex, System.Action callback = null)
    {
        currentCoroutine = StartCoroutine(ShowMessageCoroutine(infos,startIndex,stopIndex, callback));
    }

    IEnumerator ShowMessageCoroutine(List<MessageInfo> infos, int startIndex ,int stopIndex,System.Action callback)
    {
        isGamePaused = true;
        EnableMessage(true) ;

        for (int i = startIndex ; i < stopIndex; i++ ) {
            mainMessageText.fontSize = (infos[i].fontSize==0)?32:infos[i].fontSize;
            mainMessageText.text = infos[i].message.Replace("\\n", "\n");
            yield return new WaitForSeconds(infos[i].waitTime);
        }

        EnableMessage(false);

        callback?.Invoke();

        isGamePaused = false;

        currentCoroutine = null;
    }

    internal void AddScore(int add)
    {
        score += add;
        if(score > hiScore) {
            SetHiScore(score);
        }

        //Change HUD
        scoreText.text = HelperFs.ScoreFormat(score);
    }

    void SetHiScore(int newHiScore)
    {
        //Change HUD
        hiScore = newHiScore;
        hiScoreText.text = HelperFs.ScoreFormat(hiScore);

        //Change Player prefs
        PlayerPrefs.SetInt("hiScore", hiScore);
    }

    public void KilledUfo()
    {
        IncrementLife();
        AddScore(100);
    }

    internal void DefeatedWave()
    {
        currentInvaderWave++;
        if(maxWaveCountForFinishingGame == currentInvaderWave) {
            WaveDefeatedWait(true);
        }
        else {
            WaveDefeatedWait(false);
            InvaderManager.Instance.ReloadInvaders();

        }
    }

    private void GameOver()
    {
        ShipDeadWait(true);
    }

    public void ShipExloded()
    {

        DecrementLife();

        if (remainingLife == 0) {
            GameOver();
        }
        else {
            ShipDeadWait(false);
        }
    }

    void IncrementLife()
    {
        remainingLife++;
        remainingLifeText.text = remainingLife.ToString();
    }

    void DecrementLife()
    {
        remainingLife--;
        remainingLifeText.text = remainingLife.ToString();
    }

    void ShipDeadWait(bool isGameOver)
    {
        if (isGameOver) {
            ShowMessage(messageInfos, 3, 5,()=> { SceneManager.LoadScene(0); });
        }
        else {
            ShowMessage(messageInfos, 2, 3, () => { pController.ReloadShip(); });
        }
    }

    void WaveDefeatedWait(bool isLastWave)
    {
        if (isLastWave) {
            ShowMessage(messageInfos,6,8, () => { SceneManager.LoadScene(0); });
        }
        else {
            ShowMessage(messageInfos, 5, 6);

        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
    }

    public void UnpauseGame()
    {
        // Dont let others override GamaManager pause
        if(currentCoroutine == null) {
            isGamePaused = false;
        }
    }

    [System.Serializable]
    public struct MessageInfo
    {
        public string name;
        public string message;
        public float waitTime;
        public int fontSize;
    }
}
