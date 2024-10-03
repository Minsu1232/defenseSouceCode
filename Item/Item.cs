using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemData itemData; // ��ũ���ͺ� ������Ʈ ����

    public abstract void ApplyEffect(Monster target, ItemManager manager);
    public abstract void ApplyEffect(CharacterInfo character, ItemManager manager);

    protected void StartItemCoroutine(ItemManager manager, IEnumerator coroutine)
    {
        manager.StartCoroutineFromItem(coroutine);

    }
}