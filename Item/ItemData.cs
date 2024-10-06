using UnityEngine;
/// <summary>
/// 아이템 데이터 스크립터블오브젝트
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]


public class ItemData : ScriptableObject
{
    public string itemName; // 아이템 이름
    public string itemExplain; // 아이템 설명
    public Sprite itemIcon; // 아이템 아이콘
    public float speedReductionPercentage; // 속도 감소 퍼센트 (해당 아이템에 필요 시)
    public float attackPowerIncreasePercentage; // 공격력 증가 퍼센트
    public float attackSpeedIncrease; // 공격 속도 증가 (해당 아이템에 필요 시)
    public float duration; // 효과 지속 시간
}
