public class SpeedReductionItem : Item
{
    public override void ApplyEffect(Monster target, ItemManager manager)
    {
        target.ApplyPassiveEffect(itemData.speedReductionPercentage);
    }

    public override void ApplyEffect(CharacterInfo character, ItemManager manager)
    {
        // 캐릭터에게 적용될 효과가 없으면 비워둡니다.
    }
}