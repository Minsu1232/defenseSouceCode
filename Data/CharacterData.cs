using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���ֵ��� ������ base���� CSV���� �ε�
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 2)]
public class CharacterData : ScriptableObject
{
    public int id; // ���� id
    public string heroName; // ������ �̸�
    public int level; // ���� ����
    public int upgradeCount; //�÷��̾��� ��ȭ Ƚ��
    public AttackType selectedType; // ���� Ÿ��
    public float baseAttackPower; //CSV�Ҵ� ���� (�⺻ ���ݷ�)
    public float baseAttackSpeed; //CSV�Ҵ� ���� (�⺻ ���ݼӵ�)
    public float baseAttackRange; //CSV�Ҵ� ���� (�⺻ ���ݹ���)
    public float baseCriticalChance;
    public float attackPower; // ���ݷ�
    public float attackSpeed; // ���� �ӵ�
    public float attackRange; // ���� ����
    public float criticalChance; // ġ��Ÿ Ȯ��
    public HeroGrade heroGrade; // ������ ���
    public GameObject heroPrefab; // ������ ������
    public GameObject attackPrefab; //CSV�Ҵ� ���� (�⺻ ���� ������)

  
    public enum AttackType
    {
        Warrior,
        Magic,
        Archer
    }
    // ScriptableObject�� �����ϴ� �޼���
    public CharacterData Clone()
    {
        return Instantiate(this);
    }
}
   



