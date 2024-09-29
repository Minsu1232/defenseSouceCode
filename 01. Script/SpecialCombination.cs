using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialCombination", menuName = "ScriptableObjects/SpecialCombination", order = 3)]
public class SpecialCombination : ScriptableObject
{
    public List<CharacterData> requiredHeroes; // 특수 조합에 필요한 영웅 리스트
    public CharacterData resultHero;           // 결과로 나올 영웅
}
