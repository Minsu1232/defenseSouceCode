using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Achievements
{
    public string name;             // ���� �̸�
    public int targetCount;         // ��ǥ ��ġ
    public int maxTier;             // �ִ� Ƽ��
    public float rewardMultiplier;  // ���� ����
    public int currentTier;         // ���� Ƽ��
    public bool isCompleted;        // ���� �Ϸ� ����
    public int baseExperience = 800; // �⺻ ����ġ ����
    public int baseMoney = 200;     // �⺻ ��� ����
}