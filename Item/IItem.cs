/// <summary>
/// 아이템의 효과를 적용하는 메서드를 강제
/// </summary>
public interface IItem
{
    void ApplyEffect(Monster target, ItemManager manager); // 몬스터에 효과 적용
    void ApplyEffect(CharacterInfo character, ItemManager manager); // 캐릭터에 효과 적용
}