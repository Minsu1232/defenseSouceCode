using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 2)]
public class CharacterData : ScriptableObject
{
    public string heroName; // ������ �̸�
    public int level; // ���� ����
    public int upgradeCount; //�÷��̾��� ��ȭ Ƚ��
    public AttackType selectedType; // ���� Ÿ��
    public float attackPower; // ���ݷ�
    public float attackSpeed; // ���� �ӵ�
    public float attackRange; // ���� ����
    public float criticalChance; // ġ��Ÿ Ȯ��
    public HeroGrade heroGrade; // ������ ���
    public GameObject heroPrefab; // ������ ������
    public GameObject attackPrefab;

  
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
   



