public class AttackSpeedIncreaseItem : Item
{
    public override void ApplyEffect(Monster target, ItemManager manager)
    {
        // ���Ϳ��� ����� ȿ���� ������ ����Ӵϴ�.
    }

    public override void ApplyEffect(CharacterInfo character, ItemManager manager)
    {
        character.IncreaseStats(0, itemData.attackSpeedIncrease, 0);
    }
}