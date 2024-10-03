using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magician : CharacterInfo
{

    protected override void UnlockNewSkill()
    {
        // ���ο� ��ų ��� ���� ���� ����
    }

    public override void PassiveEffect()
    {
        float speedReductionPercentage = characterData.heroGrade.speedReductionPercentage;
        //MonsterSpawnManager.Instance.ApplyPassiveEffect(speedReductionPercentage);
    }
    protected override void BasicAttack()
    {
        base.BasicAttack();
        target = closetTarget;
        float damage = AttackPower; // �⺻ ���ݷ� ����
        animator.speed = AttackSpeed; // �ִϸ��̼� �ӵ��� ���� �ӵ��� ����
        animator.SetTrigger("Attack"); // ���� �ִϸ��̼� ���� (SetBool ��� SetTrigger ���)
        bool isCrtical = IsCriticalHit();
        if (isCrtical)
        {
            damage *= 2; // ũ��Ƽ�� ��Ʈ �� ������ 2��                    
            Debug.Log("Critical hit!");
        }

        Vector2 arrowSpawnPosition = arrowTransform.transform.position; // ĳ������ ��ġ �������� �ҷ� ����
        arrowSpawnPosition.y += 0.1f; // ĳ���ͺ��� ��¦ ������ ����

        GameObject energyBolt = Instantiate(attackprefab, arrowSpawnPosition, Quaternion.identity);
        EnergyBolt boltScript = energyBolt.GetComponent<EnergyBolt>();
        if (boltScript != null)
        {
            boltScript.Initialize(closetTarget, damage, this);
        }

    }

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
