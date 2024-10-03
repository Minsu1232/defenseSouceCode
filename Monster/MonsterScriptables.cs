using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStats", menuName = "ScriptableObjects/MonsterStats", order = 1)]
public class MonsterStats : ScriptableObject
{
    public float health;
    public float defense;
}
