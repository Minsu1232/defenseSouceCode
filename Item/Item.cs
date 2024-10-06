using System.Collections;
using UnityEngine;
/// <summary>
/// �����ۿ� ���� ����� �߻� Ŭ����
/// </summary>
public abstract class Item : MonoBehaviour,IItem
{
    public ItemData itemData; // ��ũ���ͺ� ������Ʈ ����

    public abstract void ApplyEffect(Monster target, ItemManager manager);
    public abstract void ApplyEffect(CharacterInfo character, ItemManager manager);

    protected void StartItemCoroutine(ItemManager manager, IEnumerator coroutine)
    {
        manager.StartCoroutineFromItem(coroutine);

    }
}