using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro를 사용하기 위해 추가

[CreateAssetMenu(fileName = "UIReferences", menuName = "ScriptableObjects/UIReferences", order = 1)]
public class UIReferences : ScriptableObject
{
    public Transform[] waypoints;
    public GameObject challengePanel;
    public Button challengeButton;
    public Image experienceBar;
    public TextMeshProUGUI levelText;  // TextMeshProUGUI 타입으로 설정
}