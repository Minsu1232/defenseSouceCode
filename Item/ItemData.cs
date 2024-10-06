using UnityEngine;
/// <summary>
/// ������ ������ ��ũ���ͺ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]


public class ItemData : ScriptableObject
{
    public string itemName; // ������ �̸�
    public string itemExplain; // ������ ����
    public Sprite itemIcon; // ������ ������
    public float speedReductionPercentage; // �ӵ� ���� �ۼ�Ʈ (�ش� �����ۿ� �ʿ� ��)
    public float attackPowerIncreasePercentage; // ���ݷ� ���� �ۼ�Ʈ
    public float attackSpeedIncrease; // ���� �ӵ� ���� (�ش� �����ۿ� �ʿ� ��)
    public float duration; // ȿ�� ���� �ð�
}
