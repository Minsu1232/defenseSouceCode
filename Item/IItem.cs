/// <summary>
/// �������� ȿ���� �����ϴ� �޼��带 ����
/// </summary>
public interface IItem
{
    void ApplyEffect(Monster target, ItemManager manager); // ���Ϳ� ȿ�� ����
    void ApplyEffect(CharacterInfo character, ItemManager manager); // ĳ���Ϳ� ȿ�� ����
}