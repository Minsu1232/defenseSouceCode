using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Archer : CharacterInfo
{
    private SkillLoader loader;


    protected override void UnlockNewSkill()
    {
        // ���ο� ��ų ��� ���� ���� ����
    }

    public override void PassiveEffect()
    {
        // �нú� ȿ�� ����
    }
    protected override void BasicAttack()
    {
        base.BasicAttack();       

        // ȭ���� ���� ��ġ�� ĳ������ ���� ��ġ�� ����
        Vector2 arrowSpawnPosition = arrowTransform.transform.position; // ĳ������ ��ġ �������� ȭ���� ����
        arrowSpawnPosition.y += 0.1f; // ĳ���ͺ��� ��¦ ������ ����

        GameObject arrow = Instantiate(attackprefab, arrowSpawnPosition, Quaternion.identity); // ȭ�� ����
        Arrow arrowScript = arrow.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            // Ÿ���� �������� ������� ȭ���� ������ ��ġ���� �����Ǹ�, Ÿ���� ���� �߻��
            arrowScript.Initialize(closetTarget, damage, this); // ȭ���� Ÿ���� ���ϵ��� �ʱ�ȭ
        }


    }

    //public override void LevelUp()
    //{
    //    characterData.level++;
    //    if (characterData.level == 3)
    //    {
    //        UnlockNewSkill();
    //    }
    //}
    protected override void Start()
    {
        base.Start();

        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����
                         // ��ų �ʱ�ȭ       
    }
    protected override void Update()
    {
        base.Update();
    }
}
