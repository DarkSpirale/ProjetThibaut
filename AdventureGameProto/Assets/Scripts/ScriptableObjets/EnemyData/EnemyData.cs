using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "My Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int id;
    public string enemyName;
    public string description;
    public int health;
    public int armor;
    public int attackPower;
    public int damageOnCollision;
    public int moveSpeed;
    public bool targetsPlayer;
    public float detectionRadius;
}
