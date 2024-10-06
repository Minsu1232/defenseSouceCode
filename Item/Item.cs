using System.Collections;
using UnityEngine;
/// <summary>
/// 아이템에 직접 상속할 추상 클래스
/// </summary>
public abstract class Item : MonoBehaviour,IItem
{
    public ItemData itemData; // 스크립터블 오브젝트 참조

    public abstract void ApplyEffect(Monster target, ItemManager manager);
    public abstract void ApplyEffect(CharacterInfo character, ItemManager manager);

    protected void StartItemCoroutine(ItemManager manager, IEnumerator coroutine)
    {
        manager.StartCoroutineFromItem(coroutine);

    }
}