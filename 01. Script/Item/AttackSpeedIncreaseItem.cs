public class AttackSpeedIncreaseItem : Item
{
    public override void ApplyEffect(Monster target, ItemManager manager)
    {
        // 몬스터에게 적용될 효과가 없으면 비워둡니다.
    }

    public override void ApplyEffect(CharacterInfo character, ItemManager manager)
    {
        character.IncreaseStats(0, itemData.attackSpeedIncrease, 0);
    }
}