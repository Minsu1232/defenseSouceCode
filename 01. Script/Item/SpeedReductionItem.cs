public class SpeedReductionItem : Item
{
    public override void ApplyEffect(Monster target, ItemManager manager)
    {
        target.ApplyPassiveEffect(itemData.speedReductionPercentage);
    }

    public override void ApplyEffect(CharacterInfo character, ItemManager manager)
    {
        // ĳ���Ϳ��� ����� ȿ���� ������ ����Ӵϴ�.
    }
}