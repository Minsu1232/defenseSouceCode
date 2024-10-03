using System.Collections;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemData itemData; // 스크립터블 오브젝트 참조

    public abstract void ApplyEffect(Monster target, ItemManager manager);
    public abstract void ApplyEffect(CharacterInfo character, ItemManager manager);

    protected void StartItemCoroutine(ItemManager manager, IEnumerator coroutine)
    {
        manager.StartCoroutineFromItem(coroutine);

    }
}