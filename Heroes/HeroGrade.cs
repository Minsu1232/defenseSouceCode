using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������� �� ��ȯ Ȯ�� ���ϴ� ��ũ���ͺ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = "HeroGrade", menuName = "ScriptableObjects/HeroGrade", order = 1)]
public class HeroGrade : ScriptableObject
{
    public string gradeName;
    public float summonProbability; // ��ȯ Ȯ��
    public bool isSummonable; // �̱�� ��ȯ ���� ����
    public bool isCombinable; // ���� ���� ����
    public HeroGrade[] combinableFrom; // �������� ���� ������ ���
    public float speedReductionPercentage; // �ӵ� ���� �ۼ�Ʈ
}