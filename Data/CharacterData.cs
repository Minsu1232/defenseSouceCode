using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 유닛들의 데이터 base류는 CSV에서 로드
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 2)]
public class CharacterData : ScriptableObject
{
    public int id; // 고유 id
    public string heroName; // 영웅의 이름
    public int level; // 현재 레벨
    public int upgradeCount; //플레이어의 강화 횟수
    public AttackType selectedType; // 영웅 타입
    public float baseAttackPower; //CSV할당 변수 (기본 공격력)
    public float baseAttackSpeed; //CSV할당 변수 (기본 공격속도)
    public float baseAttackRange; //CSV할당 변수 (기본 공격범위)
    public float baseCriticalChance;
    public float attackPower; // 공격력
    public float attackSpeed; // 공격 속도
    public float attackRange; // 공격 범위
    public float criticalChance; // 치명타 확률
    public HeroGrade heroGrade; // 영웅의 등급
    public GameObject heroPrefab; // 영웅의 프리팹
    public GameObject attackPrefab; //CSV할당 변수 (기본 공격 프리팹)

  
    public enum AttackType
    {
        Warrior,
        Magic,
        Archer
    }
    // ScriptableObject를 복제하는 메서드
    public CharacterData Clone()
    {
        return Instantiate(this);
    }
}
   



