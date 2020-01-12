using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basic bottom aliens
/// Strong middle aliens
/// Alpha top aliens
/// Ufo Mystery alien
/// </summary>
public enum InvaderType { Basic = 0, Strong = 1, Alpha = 2, Ufo = 3 }
/// <summary>
/// Spawn, Move and fire from invaders
/// </summary>
public class InvaderManager : Singleton<InvaderManager>
{

    #region Public Variables
    [Header("Prefab References")]
    public GameObject invaderPrefab;
    public GameObject [] invaderBulletPrefabs;
    public GameObject powerUpPrefab;
    public GameObject ufoPrefab;

    [Header("Animator References")]
    public AnimatorOverrideController basicInvaderAC;
    public AnimatorOverrideController strongInvaderAC;
    public AnimatorOverrideController alphaInvaderAC;

    [Header("Settings")]
    public int xSize = 11;
    public int ySize = 5;

    public float xOffset = -2.5f;
    public float yOffset = 2.5f;

    public float xCellSize = 0.25f;
    public float yCellSize = 0.5f;

    public float worldMinY;

    public Transform ufoSpawnPoint;

    #endregion

    float lastTimeMoved = 0f;
    float direction = 1;

    [Header("Attack Styles"),Tooltip("Create custom attack styles")]
    [SerializeField] InvaderAttackStyle[] attackStyles;
    InvaderAttackStyle currentAttackStyle;
    int currentAttackStyleIndex = 0;

    public int TotalInvaderCount => xSize * ySize;

    int currentInvaderCount;

    Rect border;

    float worldXMax => GameManager.Instance.xEdge;
    float worldXMin => -GameManager.Instance.xEdge;

    bool[,] aliveInvaders;
    // Will be filled with 1,2,3,4,... picking random column will be easier
    List<int> aliveColumns;

    bool invadersSpawningFinished = false;

    void Start()
    {

        StartCoroutine(LoadInvadersSlowly());
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGamePaused)
            return;

        float elapsedTime = Time.time - lastTimeMoved;
        if(elapsedTime > currentAttackStyle.moveWaitTime) {
            Move();
        }

        FireBullet();

        UfoSpawner();

        // Test Ufo
        if (Input.GetKeyDown(KeyCode.Q)) {
            SpawnUfo();
        }
    }

    #region Ufo Spawner
    float ufoSpawnPeriod = 12f;
    float ufoSpwanChance = 0.32f;
    float ufoSpeed = 5f;
    float lastTimeUfoSpawned = 0f;
    float ufoDestroyTime = 3.2f;

    void UfoSpawner()
    {
        float elapsedTime = Time.time - lastTimeUfoSpawned;
        if(elapsedTime > ufoSpawnPeriod) {
            lastTimeUfoSpawned = Time.time;
            var rand = UnityEngine.Random.value;
            if(rand < ufoSpwanChance) {
                SpawnUfo();
            }
        }

    }

    void SpawnUfo()
    {
        float dir = UnityEngine.Random.value;
        if(dir > 0.5f) {
            GameObject ufo = Instantiate(ufoPrefab, ufoSpawnPoint.position - Vector3.right * 8f, Quaternion.identity);
            Rigidbody2D rb = ufo.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.velocity = Vector2.right * ufoSpeed;
            Destroy(ufo, ufoDestroyTime);
            StartCoroutine(HelperFs.DoAfter(()=> { AudioManager.Instance.ufoSpawn.Stop(); }, ufoDestroyTime));
        }
        else {
            GameObject ufo = Instantiate(ufoPrefab, ufoSpawnPoint.position + Vector3.right * 8f, Quaternion.identity);
            Rigidbody2D rb = ufo.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.velocity = Vector2.right * -ufoSpeed;
            Destroy(ufo, ufoDestroyTime);
            StartCoroutine(HelperFs.DoAfter(() => { AudioManager.Instance.ufoSpawn.Stop(); }, ufoDestroyTime));
        }
    }
    #endregion

    // Invaders Fire Algorithm
    // Use unity inspector to create custom attack styles
    // if current invader count more than half
    // %60 chance of fire every in every 2 seconds
    // else if current invader count more than quarter
    // %80 chance of fire every in every 1 seconds
    // else 
    // %80 chance of fire every in every 0.5 seconds
    // pick random column 

    float lastTimeFired = 0f;

    void FireBullet()
    {
        float elapsedTime = Time.time - lastTimeFired;

        if (elapsedTime > currentAttackStyle.fireWaitTime) {

            lastTimeFired = Time.time;

            float rand = UnityEngine.Random.value;
            //Fire
            if (rand < currentAttackStyle.fireChance) {

                // Pick an alive column and shoot  
                int pickedX = PickRandomAliveColumn();
                int pickedY = 0;
                // Get the lowest invader position
                for(int y=0; y < ySize; y++) {
                    if (aliveInvaders[pickedX, y]) {
                        pickedY = y;
                        break;
                    }
                }

                Vector3 bulletSpawnPos = CalculateInvaderPositionFromIndex(pickedX, pickedY) + Vector3.down * yCellSize * 0.5f;
                Instantiate(SelectRandomBullet(), bulletSpawnPos, Quaternion.identity);
            }

        }
    }

    GameObject SelectRandomBullet()
    {
        return invaderBulletPrefabs[UnityEngine.Random.Range(0, invaderBulletPrefabs.Length)];
    }

    Vector3 CalculateInvaderPositionFromIndex(int x,int y)
    {
        return transform.position + new Vector3(xOffset+x*xCellSize,yOffset+yOffset+y*yCellSize);
    }

    int PickRandomAliveColumn()
    {
        return aliveColumns[UnityEngine.Random.Range(0, aliveColumns.Count)];
    }

    public void InvederExploded(int xIndex, int yIndex)
    {
        aliveInvaders[xIndex, yIndex] = false;

        // Check if column is empty
        bool empty = true;
        for(int y= 0;y < ySize; y++) {
            if(aliveInvaders[xIndex, y]) {
                empty = false;
                break;
            }
        }
        // if so remove from aliveColumnsList;
        if (empty) {
            aliveColumns.Remove(xIndex);
        }

        currentInvaderCount--;//Double Shooting explodes 2 invaders 2 times fix this!!!!!!!!!!!!!

        if(currentInvaderCount == 0) {
            GameManager.Instance.DefeatedWave();
        }

        // Determine current attack style
        float currentPercantage = (float)currentInvaderCount / (float)TotalInvaderCount;

        if (currentPercantage < currentAttackStyle.invaderPercentage) {
            if(currentAttackStyleIndex < attackStyles.Length-1) {
                currentAttackStyle = attackStyles[++currentAttackStyleIndex];

                Debug.Log("Current Attack style: " + currentAttackStyle.name);
                Debug.Log("Current Attack index: " + currentAttackStyleIndex);
            }
           
        }

    }

    void Move()
    {
        lastTimeMoved = Time.time;

        bool flagChangedDirection = false;
        if(direction > 0) {
            //Moving Right
            //control border x max to world x max
            if(border.xMax > worldXMax) {
                Debug.Log(border.xMax);
                direction *= -1;
                flagChangedDirection = true;
            }

        }
        else {
            //Moving left
            //control border x min to world x min
            if (border.xMin < worldXMin) {
                direction *= -1;
                flagChangedDirection = true;
            }
        }

        if(flagChangedDirection && border.yMin > worldMinY) {
            transform.position += Vector3.down * yCellSize * 0.5f;
        }

        transform.position += Vector3.right * xCellSize * 0.25f * direction;
        CalculateBorderRect();

    }

    void CreateInvader(float x,float y,InvaderType type,int xIndex, int yIndex)
    {
        GameObject inv = Instantiate(invaderPrefab, new Vector3(x, y, 0f), Quaternion.identity, transform);

        InvaderBehaviour invB = inv.GetComponent<InvaderBehaviour>();
        invB.Init(type, xIndex, yIndex);

    }

    public void ReloadInvaders()
    {
        StartCoroutine(LoadInvadersSlowly());
    }

    IEnumerator LoadInvadersSlowly()
    {
        invadersSpawningFinished = false;
        GameManager.Instance.PauseGame();

        WaitForSeconds sec = new WaitForSeconds(0.05f);
        aliveInvaders = new bool[xSize,ySize];
        aliveColumns = new List<int>(xSize);
        for(int i = 0;i < xSize; i++) {
            //aliveColumns[i] = i;
            aliveColumns.Add(i);
        }

        currentAttackStyle = attackStyles[0];

        float yPos = yOffset + transform.position.y;
        for (int y = 0; y < ySize; y++) {
            float xPos = xOffset + transform.position.x;

            InvaderType type = SelectType(y);

            for (int x = 0; x < xSize; x++) {
                CreateInvader(xPos, yPos, type, x, y);
                yield return sec;
                aliveInvaders[x, y] = true;

                xPos = xPos + xCellSize;
            }
            yPos = yPos + yCellSize;
        }
        currentInvaderCount = TotalInvaderCount;
        invadersSpawningFinished = true;
        GameManager.Instance.UnpauseGame();

    }

    InvaderType SelectType(int y)
    {
        InvaderType type;
        if (y < 2) {
            type = InvaderType.Basic;
        }
        else if (y < 4) {
            type = InvaderType.Strong;
        }
        else {
            type = InvaderType.Alpha;
        }
        return type;
    }

    void CalculateBorderRect()
    {
        float width = xCellSize * xSize + xCellSize / 2f;
        float height = yCellSize * ySize;

        float xx = transform.position.x - xCellSize / 2f;
        float yy = transform.position.y - yCellSize / 2f;

        border = new Rect(xx, yy, width, height);
    }

    private void OnDrawGizmosSelected()
    {

        float yPos = transform.position.y;
        for (int y = 0; y < ySize; y++) {
            float xPos = transform.position.x;
            for (int x = 0; x < xSize; x++) {
                Gizmos.DrawSphere(new Vector3(xPos, yPos, 0f), 0.1f);
                xPos = xPos + xCellSize;
            }
            yPos = yPos + yCellSize;
        }


        if (!Application.isPlaying) {
            CalculateBorderRect();
        }

        HelperFs.GizmosDrawRect(border);

        if (Application.isPlaying) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(worldXMin, 0, 0f), 0.1f);
            Gizmos.DrawSphere(new Vector3(worldXMax, 0, 0f), 0.1f);
        }
    }


    [System.Serializable]
    public struct InvaderAttackStyle
    {
        public string name;// for debug
        // if invader count below this percantage attack style will change
        [Range(0,1),Tooltip("Invaders change attack style when remaining invader percantage is below this percentage.")]
        public float invaderPercentage;
        public float fireWaitTime;
        [Range(0, 1)]
        public float fireChance;
        public float moveWaitTime;
    }
}

