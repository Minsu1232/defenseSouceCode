using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro�� ����ϱ� ���� �߰�

[CreateAssetMenu(fileName = "UIReferences", menuName = "ScriptableObjects/UIReferences", order = 1)]
public class UIReferences : ScriptableObject
{
    public Transform[] waypoints;
    public GameObject challengePanel;
    public Button challengeButton;
    public Image experienceBar;
    public TextMeshProUGUI levelText;  // TextMeshProUGUI Ÿ������ ����
}