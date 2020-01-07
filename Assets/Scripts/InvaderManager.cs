using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderManager : Singleton<InvaderManager>
{
    [Header("Prefab References")]
    public GameObject invaderPrefab;

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

    public float moveWaitTime = 1f;
    public float lastTimeMoved = 0f;
    public float worldMinY;
    float direction = 1;

    public int TotalInvaderCount => xSize * ySize;

    int currentInvaderCount;

    Rect border;

    float worldXMax;
    float worldXMin;

    // Start is called before the first frame update
    void Start()
    {
        worldXMax = Camera.main.orthographicSize * (Camera.main.aspect);
        Debug.Log("worldXMax" + worldXMax);
        worldXMin = -worldXMax;

        LoadInvadersImmidiate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        CalculateBorderRect();


        float elapsedTime = Time.time - lastTimeMoved;
        if(elapsedTime > moveWaitTime) {
            Move();
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

        transform.position += Vector3.right * xCellSize * 0.5f * direction;


    }

    void CreateInvader(float x,float y,InvaderType type)
    {
        GameObject inv = Instantiate(invaderPrefab, new Vector3(x, y, 0f), Quaternion.identity, transform);

        inv.GetComponent<InvaderBehaviour>().invaderType = type;

    }

    void LoadInvadersImmidiate()
    {

        float yPos = yOffset + transform.position.y;
        for (int y = 0; y < ySize; y++) {
            float xPos = xOffset + transform.position.x;

            InvaderType type = SelectType(y);

            for (int x = 0; x < xSize; x++) {
                CreateInvader(xPos, yPos,type);
                xPos = xPos + xCellSize;
            }
            yPos = yPos + yCellSize;
        }
        currentInvaderCount = TotalInvaderCount;
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

    private void OnDrawGizmosSelected()
    {

        float yPos = transform.position.y;
        for (int y = 0; y < ySize; y++) {
            float xPos = transform.position.x;
            for (int x = 0; x < xSize; x++) {
                Gizmos.DrawSphere(new Vector3(xPos,yPos,0f),0.1f);
                xPos = xPos + xCellSize;
            }
            yPos = yPos + yCellSize;
        }
        if (!Application.isPlaying) {
            CalculateBorderRect();
        }

        GizmosDrawRect(border);
    }

    void CalculateBorderRect()
    {
        float width = xCellSize * xSize + xCellSize / 2f;
        float height = yCellSize * ySize;

        float xx = transform.position.x - xCellSize / 2f;
        float yy = transform.position.y - yCellSize / 2f;

        border = new Rect(xx, yy, width, height);
    }

    // Note that this function is only meant to be called from OnGUI() functions.
    public static void GizmosDrawRect(Rect rect )
    {

        Gizmos.color = Color.yellow;


        //Left
        Gizmos.DrawLine(new Vector3(rect.x, rect.y, 0), new Vector3(rect.x, rect.y + rect.height,0f));
        //Right
        Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y, 0), new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));
        //Top
        Gizmos.DrawLine(new Vector3(rect.x , rect.y, 0), new Vector3(rect.x + rect.width, rect.y , 0f));
        //Bot
        Gizmos.DrawLine(new Vector3(rect.x , rect.y + rect.height, 0f), new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));

    }
}


/// <summary>
/// Basic bottom aliens
/// Strong middle aliens
/// Alpha top aliens
/// </summary>
public enum InvaderType { Basic = 0, Strong = 1, Alpha = 2 }
