using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AttackDamage))]
public class AttackDamageEditor : Editor
{

	override public void OnInspectorGUI()
	{
		var attackDamage = target as AttackDamage;
		
		attackDamage.knockBackFactor = EditorGUILayout.IntField("Knockback Factor", attackDamage.knockBackFactor);        

        GUILayout.Space(10);

        attackDamage.targetToAttack = (TargetsToAttack)EditorGUILayout.EnumPopup(new GUIContent("Target to Attack", "Collider to be affected by the attack"), attackDamage.targetToAttack);
		
        if(attackDamage.targetToAttack == TargetsToAttack.Player)
        {
            attackDamage.enemy = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Enemy", "GameObject of the enemy landing the attack"), attackDamage.enemy, typeof(GameObject), false);
        }		
	}
}