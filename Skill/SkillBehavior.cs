using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// ��ų�� ��ǥ�� ���� �̵��ϰ�, ���� �浹 �� �پ��� ȿ���� �����ϴ� ������ ó��
/// </summary>
public class SkillBehavior : MonoBehaviour
{
    private CharacterInfo caster;
    private float damage;
    private float range;
    private Vector3 targetPosition;
    private GameObject target;
    private bool isSingleTarget;
    private bool hasSlowEffect;
    private float slowAmount;
    private bool hasDefenseReduction;
    private float defenseReductionAmount;
    private bool isSpecialSkill; // ��ų�� Ư���� ������� ����
    private float duration;
    private float speed; // ��ų �̵� �ӵ�
    private int monsterLayer; // Monster ���̾� �ε���
    private float damageInterval = 1f; // ���ظ� �ִ� �ֱ�

    // MythWarriorSkill ���� ����
    private GameObject swordPrefab;
    private GameObject barrierPrefab;
    private Transform[] dropPoints;
    private List<Transform> availablePoints;
    private List<Transform> usedPoints;
    private bool isBarrierActive = false;

    

    Animator animator;

    //����
    public Action<Vector3> OnSwordDestroyed; // ���� �ı��� �� ȣ��Ǵ� �ݹ�
    // Ÿ�� ��ƼŬ�� ������ �� �ִ� �ۺ� ����
    public GameObject hitParticlePrefab;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(CharacterInfo caster, float damage, float range, Vector3 targetPosition, GameObject target, bool isSingleTarget = false, bool hasSlowEffect = false, float slowAmount = 0f, bool hasDefenseReduction = false, float defenseReductionAmount = 0f, bool isSpecialSkill = false, float duration = 5f, float speed = 10f)
    {
        this.caster = caster;
        this.damage = damage;
        this.range = range;
        this.targetPosition = targetPosition;
        this.target = target;
        this.isSingleTarget = isSingleTarget;
        this.hasSlowEffect = hasSlowEffect;
        this.slowAmount = slowAmount;
        this.hasDefenseReduction = hasDefenseReduction;
        this.defenseReductionAmount = defenseReductionAmount;
        this.isSpecialSkill = isSpecialSkill;
        this.duration = duration;
        this.speed = speed;
        this.monsterLayer = LayerMask.NameToLayer("Monster"); // Monster ���̾� �ε����� ������

        // ��ų �̵� ���� ����
        StartCoroutine(MoveSkill());
    }

    private IEnumerator MoveSkill()
    {
        // ��ų�� Ư�� ���� ���� �̵�
        while (true)
        {
            if (target == null) // Ÿ���� ������� ��ų�� �����
            {
                Destroy(gameObject);
                yield break;
            }
            // Ÿ���� ���� ��ġ�� ��ǥ �������� ����
            targetPosition = target.transform.position;

            // ��ų �̵�
            float step = speed * Time.deltaTime; // �̵� �ӵ� ����
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step); // Ÿ���� ���� �̵�
            if(caster.characterData.heroName != "�渶����" && caster.characterData.heroName != "����ġ" && caster.characterData.heroName != "�丮��" && caster.characterData.heroName != "�ָ�����Ͼ") // ȸ���ϸ� �ȵǴ� ��
            {
                // ȸ��: Ÿ���� ��ġ�� ���ϵ��� ��ų�� ȸ����Ŵ
                UpdateRotation();
            }
          

            // ��ų�� Ÿ�ٿ� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, targetPosition) < 0.3f)
            {
                Explode();
                yield break;
            }
         

            yield return null;
        }
    }
    private void UpdateRotation()
    {
        // Ÿ���� ���� ������ ���
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Ÿ���� �ٶ󺸵��� ȸ�� ����
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //// �浹�� ��ü�� ���̾ Monster ���̾����� Ȯ��
        //if (!isSpecialSkill&&other.gameObject.layer == monsterLayer)
        //{
        //    Monster enemy = other.GetComponent<Monster>();
        //    if (enemy != null && (isSingleTarget && enemy.gameObject == target))
        //    {
        //        Explode();
        //    }
        //    else if (!isSingleTarget)
        //    {
        //        Explode();
        //    }
        //}
    }
    public IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / duration) * speed);
            elapsedTime += Time.deltaTime;

            // ����ģ ������ ���ظ� ��
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, 1 << monsterLayer);
            foreach (var hit in hits)
            {
                Monster enemy = hit.GetComponent<Monster>();
                if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie)
                {
                    enemy.TakeDamage(damage, caster.AttackType);                 
                    
                }
            }

            yield return null;
        }

        // ��ų�� Ÿ�ٿ� ������ �� 5�ʰ� ����
        yield return new WaitForSeconds(duration);

        // ���� �ð��� ���� �� ��ų ����
        Destroy(gameObject);
    }

    private void Explode()
    {
        bool isCritical = IsCriticalHit();
        float finalDamage = damage;

        if (isCritical)
        {
            finalDamage *= 2;
            Debug.Log("Critical hit with skill!");
        }
        if (!isSpecialSkill)
        {
            if (isSingleTarget)
            {
                // ���� Ÿ�ٿ��� ����
                Monster enemy = target.GetComponent<Monster>();
                if (enemy != null)
                {
                    ApplyDamageAndEffects(enemy, finalDamage, isCritical);

                    SpawnHitParticle(enemy.transform.position);
                }
            }
            else
            {
                // ������ �ִ� ���, ���� �� ��� ������ ����
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
                foreach (Collider2D hit in hits)
                {
                    Monster enemy = hit.GetComponent<Monster>();
                    if (enemy != null)
                    {
                        ApplyDamageAndEffects(enemy, finalDamage, isCritical);

                        SpawnHitParticle(hit.transform.position);
                    }
                }
            }

            // ���� �� ��ų ����
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(DestroyDelay());
        }
       
    }
 
    IEnumerator DestroyDelay()
    {
        bool isCritical = IsCriticalHit();
        float finalDamage = damage;

        // duration ���� ���ʸ��� ����
        for (int i = 0; i < duration; i++)
        {
            // ������ �ִ� ���, ���� �� ��� ������ ����
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
            foreach (Collider2D hit in hits)
            {
                Monster enemy = hit.GetComponent<Monster>();
                if (enemy != null)
                {
                    if(caster.characterData.heroName == "�ָ�����Ͼ")
                    {
                        ApplyDamageAndEffects(enemy, finalDamage, isCritical);
                        // ���� ������ ����� ���� ������ ���
                        float actualDamage = finalDamage * enemy.GetDamageMultiplier(enemy.defense);

                        // ���� �� �������� ����
                        caster.totalDamageDealt += actualDamage;

                    }
                    else
                    {
                        // �̺κ��� 1�ʿ� �ѹ��� ���
                        SpawnHitParticle(hit.transform.position);
                        ApplyDamageAndEffects(enemy, finalDamage, isCritical);
                        // ���� ������ ����� ���� ������ ���
                        float actualDamage = finalDamage * enemy.GetDamageMultiplier(enemy.defense);

                        // ���� �� �������� ����
                        caster.totalDamageDealt += actualDamage;
                    }
                   
                }
            }

            // 1�� ���
            yield return new WaitForSeconds(1f);
        }

        // ������Ʈ �ı� ���� �ݹ� ȣ�� (��ġ ���� ����)
        OnSwordDestroyed?.Invoke(transform.position);

        // ������ ���� �� ������Ʈ�� ��� �ı�
        Destroy(gameObject);

    }
    private void ApplyDamageAndEffects(Monster enemy, float finalDamage, bool isCritical)
    {
        enemy.TakeDamage(finalDamage,caster.AttackType);

        // ������ ���¿� ���� ���� ������ ���
        float actualDamage = finalDamage * enemy.GetDamageMultiplier(enemy.defense);

        // ���� ���� ���� �������� ����
        caster.totalDamageDealt += actualDamage;        

        if (hasSlowEffect) // �̼� ����
        {
            enemy.speed -= slowAmount;
            if (enemy.speed < 10)
            {
                enemy.speed = 10;
            }
           
        }

        if (hasDefenseReduction) // ���°���
        {
            enemy.defense -= enemy.defense * defenseReductionAmount;
            if (enemy.defense < 10)
            {
                enemy.defense = 10;
            }
         
        }
    }

    private bool IsCriticalHit()
    {
        return UnityEngine.Random.value <= caster.CriticalChance;
    }

    private void OnDrawGizmosSelected()
    {
        // Ÿ�� ��ġ�� �ð������� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range > 0 ? range : 0.5f);
    }
    private void SpawnHitParticle(Vector3 hitPosition)
    {
        if (hitParticlePrefab != null)
        {
            Instantiate(hitParticlePrefab, hitPosition, Quaternion.identity);
        }
    }
    /// <summary>
    /// ���ʸ�����
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="swordPrefab"></param>
    /// <param name="barrierPrefab"></param>
    /// <param name="dropPoints"></param>

}
