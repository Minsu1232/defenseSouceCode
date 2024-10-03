public class AttackPowerIncreaseItem : Item
{
    public override void ApplyEffect(Monster target, ItemManager manager)
    {
        // ���Ϳ��� ����� ȿ���� ������ ����Ӵϴ�.
    }

    public override void ApplyEffect(CharacterInfo character, ItemManager manager)
    {

        // �⺻ ���ݷ��� 20% ������ ���
        float attackPowerIncrease = character.baseAttackPower * (itemData.attackPowerIncreasePercentage / 100f);

        // IncreaseStats �޼��带 ���� ���ݷ� ���� ����
        character.IncreaseStats(attackPowerIncrease, 0, 0);
    }
}