using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(EnemyMovement))]
public class EnemyMovementEditor : Editor
{

	override public void OnInspectorGUI()
	{
		var enemyMovement = target as EnemyMovement;
		
        enemyMovement.wanderAround = GUILayout.Toggle(enemyMovement.wanderAround, new GUIContent("Wander Around", "Allows the enemy to wander around randomly"));

        if(enemyMovement.wanderAround)
        {
            enemyMovement.movementRadius = EditorGUILayout.FloatField(new GUIContent("Wander Distance", "Maximum wandering distance for each movement"), enemyMovement.movementRadius);
            enemyMovement.movementDelay = EditorGUILayout.FloatField(new GUIContent("Wander Delay", "Delay between each movement"), enemyMovement.movementDelay);

            GUILayout.Space(10);
            enemyMovement.limitedArea = GUILayout.Toggle(enemyMovement.limitedArea, new GUIContent("Limited Area", "Limits the enemy's movements to a specif area"));

            if(enemyMovement.limitedArea)
            {
                enemyMovement.movementArea = (Collider2D)EditorGUILayout.ObjectField(new GUIContent("Movement Area", "Area which the enemy is limited to"), enemyMovement.movementArea, typeof(Collider2D), true);
            }
        }
    }
    
}