public class AttackPowerIncreaseItem : Item
{
    public override void ApplyEffect(Monster target, ItemManager manager)
    {
        // 몬스터에게 적용될 효과가 없으면 비워둡니다.
    }

    public override void ApplyEffect(CharacterInfo character, ItemManager manager)
    {

        // 기본 공격력의 20% 증가를 계산
        float attackPowerIncrease = character.baseAttackPower * (itemData.attackPowerIncreasePercentage / 100f);

        // IncreaseStats 메서드를 통해 공격력 증가 적용
        character.IncreaseStats(attackPowerIncrease, 0, 0);
    }
}