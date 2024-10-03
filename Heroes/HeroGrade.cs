using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HeroGrade", menuName = "ScriptableObjects/HeroGrade", order = 1)]
public class HeroGrade : ScriptableObject
{
    public string gradeName;
    public float summonProbability; // 소환 확률
    public bool isSummonable; // 뽑기로 소환 가능 여부
    public bool isCombinable; // 조합 가능 여부
    public HeroGrade[] combinableFrom; // 조합으로 생성 가능한 등급
    public float speedReductionPercentage; // 속도 감소 퍼센트
}