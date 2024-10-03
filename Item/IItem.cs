public interface IItem
{
    void ApplyEffect(Monster target); // 몬스터에 효과 적용
    void ApplyEffect(CharacterInfo character); // 캐릭터에 효과 적용
}