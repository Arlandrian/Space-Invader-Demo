using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Divedes box collider with a grid and uses this grid to create sprite masks for barrier damaging
/// </summary>
public class BarrierBehaviour : MonoBehaviour
{
    public struct PointCollider
    {
        public Vector2 point;
        public bool hit;
    }

    public GameObject spriteMaskPrefab;

    // OPTIMIZATION
    // Can use Disctionary instead of this
    PointCollider[,] pointColliders;

    public int gridXCount = 6;
    public int gridYCount = 5;

    public int TotalPointCount => (gridXCount+1) * (gridYCount+1);
    int hitCount = 0;

    Collider2D bullet;

    private void Start()
    {
        //Initialize Point Colliders
        pointColliders = new PointCollider[gridXCount+1,gridYCount+1];

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        Bounds bounds = col.bounds;

        Vector3 leftBottom = bounds.min;
        Vector3 rightTop = bounds.max;

        float xCellSize = (leftBottom.x - rightTop.x) / gridXCount;
        float yCellSize = (rightTop.y - leftBottom.y) / gridYCount;

        int center = gridXCount / 2;

        //Create grid for point colliders
        float yPos = leftBottom.y;
        for (int y = 0; y <= gridYCount; y++) {
            float xPos = leftBottom.x;
            
            for (int x = 0; x <= gridXCount; x++) {
                pointColliders[x, y].point = new Vector2(xPos,yPos);
                if (y == gridYCount && (x == 0 || x == gridXCount)) {
                    OnHitPoint(x, y);
                }else if(y == 0 & (x == center || x == center-1 || x== center + 1)) {
                    OnHitPoint(x, y);
                }
                else {
                    pointColliders[x, y].hit = false;
                }
                xPos = xPos - xCellSize;
            }
            yPos = yPos + yCellSize;
        }

    }
    
    private void FixedUpdate()
    {
        if(bullet == null) {
            return;
        }

        for (int y = 0; y <= gridYCount; y++) {
            for (int x = 0; x <= gridXCount; x++) {
                if (!pointColliders[x, y].hit && bullet.OverlapPoint(pointColliders[x, y].point)) {
                   
                    OnHitPoint(x, y);

                    Instantiate(spriteMaskPrefab, pointColliders[x, y].point, Quaternion.identity, transform);
                       
                    Destroy(bullet.gameObject);
                    bullet = null;
                        
                    if (hitCount == TotalPointCount) {
                        Destroy(gameObject);
                    }
                    return;
                }
            }
        }

    }

    void OnHitPoint(int x,int y)
    {
        pointColliders[x, y].hit = true;
        hitCount++;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet")) {
            bullet = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == bullet) {
            bullet = null;
        }
    }

    private void OnDrawGizmosSelected()
    {

        if(pointColliders != null) {
            for (int y = 0; y <= gridYCount; y++) {
                for (int x = 0; x <= gridXCount; x++) {
                    if (pointColliders[x, y].hit) {
                        Gizmos.color = Color.red;
                    }
                    else {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawSphere(pointColliders[x, y].point, 0.02f);

                }
            }
            return;
        }

        var col = GetComponent<BoxCollider2D>();
        var bounds = col.bounds;

        Vector3 leftBottom = bounds.min;
        Vector3 rightTop = bounds.max;

        Gizmos.color = Color.red;

        float xCellSize = (leftBottom.x - rightTop.x) / gridXCount;
        float yCellSize = (rightTop.y - leftBottom.y) / gridYCount;

        float yPos = leftBottom.y;
        for(int y = 0; y <= gridYCount; y++) {
            float xPos = leftBottom.x;
            for(int x = 0; x <= gridXCount; x++) {
                Gizmos.DrawSphere(new Vector3(xPos, yPos, 0f), 0.02f);
                xPos = xPos - xCellSize;
            }
            yPos = yPos + yCellSize;
        }
    }

}
