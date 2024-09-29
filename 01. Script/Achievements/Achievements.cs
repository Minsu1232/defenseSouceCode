using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievements
{
    public string name;             // 업적 이름
    public int targetCount;         // 목표 수치
    public int maxTier;             // 최대 티어
    public float rewardMultiplier;  // 보상 배율
    public int currentTier;         // 현재 티어
    public bool isCompleted;        // 업적 완료 여부
    public int baseExperience = 800; // 기본 경험치 보상
    public int baseMoney = 200;     // 기본 골드 보상
}