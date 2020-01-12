using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp Data")]
public class PowerUpData : ScriptableObject
{
    public new string name;// For Debug
    public byte id;
    public Sprite sprite;
    public float cooldownTime;
    [Range(0, 1)]
    public float spawnChance;

}
