using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialCombination", menuName = "ScriptableObjects/SpecialCombination", order = 3)]
public class SpecialCombination : ScriptableObject
{
    public List<CharacterData> requiredHeroes; // Ư�� ���տ� �ʿ��� ���� ����Ʈ
    public CharacterData resultHero;           // ����� ���� ����
}
