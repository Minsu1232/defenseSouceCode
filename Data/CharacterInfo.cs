using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ���ֵ��� �ֻ��� Ŭ���� (���� Ŭ������ �ʹ� �����ϰ� �Ǿ��� �����մϴ�. ����ȭ �ʿ�..)
/// </summary>
public abstract class CharacterInfo : MonoBehaviour
{

    public CharacterData characterData; // ĳ���� ����
    public LayerMask enemyLayer; // ������ ��� ���̾�
    public ParticleSystem activeSkillEffect; // ��ų �ߵ� ��ƼŬ �ý���
    public float baseAttackPower; // �⺻ ���ݷ� (��ȭ���� ���� ���� ���ݷ�)
    public float baseAttackSpeed; // �⺻ ���ݼӵ� 
    public float baseAttackCritical; // �⺻ ũ��Ƽ��Ȯ�� 
    public float totalDamageDealt; // ������ ���� �� ������
    public Transform arrowTransform;
    protected Animator animator;
    protected bool isAttacking = false; // ���� ���� ����
    protected bool isRolling = false; // ������ ���� ����
    public string Name => characterData.heroName;

    protected GameObject closetTarget;
    protected float damage;
    protected bool isCritical;
    public int Level // ���� ������Ƽ (��ȭ ����)
    {
        get => characterData.level;
        set
        {
            characterData.level = value;
            UpdateStats(); // ������ ����Ǹ� ������ ������Ʈ
        }
    }

    // ĳ������ ������ ��ȯ�ϴ� �б�� ������Ƽ
    public float AttackPower => characterData.attackPower;
    public float AttackSpeed => characterData.attackSpeed;
    public float AttackRange => characterData.attackRange;
    public float CriticalChance => characterData.criticalChance;
    public CharacterData.AttackType AttackType => characterData.selectedType;

    public List<Skill> skills; // ��ų ���
    public Image mpBar; // UI���� �Ҵ�� MP �� �̹���
    private SpriteRenderer spriteRenderer;
    public GameObject attackprefab => characterData.attackPrefab;
    public GameObject target;
    private bool isBuffed = false; // ���� ���� ���¸� ��Ÿ���� ����
    public abstract void PassiveEffect();

    protected abstract void UnlockNewSkill();


    private float attackCooldown = 0f; // ���� ��ٿ� Ÿ�̸�

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = AttackSpeed; // �ִϸ��̼� ��� �ӵ��� �ʱ�ȭ�մϴ�.    



        ApplyUpgradeCount(1.03f, 1.05f, 1.02f);
        // ������ 1���� ũ�� ���� ������Ʈ
        if (Level > 1)
        {
            UpdateStats();
        }

        animator.speed = AttackSpeed; // �ִϸ��̼� ��� �ӵ� �ʱ�ȭ
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// CSV �ʱⰪ�� ���� ��ȭ ������ ����
    /// </summary>
    public void ApplyUpgradeCount(float attackMultiplier, float speedMultiplier, float criticalMultiplier)
    {
        // ��ȭ ī��Ʈ(Upgrade Count)�� ���� ���� ��� ��ȭ ī��Ʈ�� Firebase���� �ε��
        int upgradeCount = characterData.upgradeCount;

        // ���̽� ���� �������� ��ȭ�� ���� �Ҵ� (characterData���� �� ����)
        characterData.attackPower = characterData.baseAttackPower * Mathf.Pow(attackMultiplier, upgradeCount);
        characterData.attackSpeed = characterData.baseAttackSpeed * Mathf.Pow(speedMultiplier, upgradeCount);
        characterData.criticalChance = characterData.baseCriticalChance * Mathf.Pow(criticalMultiplier, upgradeCount);
    }
    private void UpdateStats()
    {
        // ������ ���� ���� ������Ʈ (������ 1���� ���� ���� ȣ���)
        characterData.attackPower = characterData.attackPower * Mathf.Pow(1.05f, Level - 1);
        characterData.attackSpeed = characterData.attackSpeed * Mathf.Pow(1.05f, Level - 1);
        characterData.criticalChance = characterData.criticalChance * Mathf.Pow(1.05f, Level - 1);

        Debug.Log($"{characterData.heroName}'s stats have been updated for level {Level}.");
    }

    public void LevelUp() // ������Ƽ ȣ��
    {
        Level++; // ���� ����
        Debug.Log($"{characterData.heroName} has leveled up to level {Level}!");
    }
    public void ApplyItemAttackPowerIncrease(float percentage)
    {
        float increase = baseAttackPower * (percentage / 100f);
        IncreaseStats(increase, 0, 0);
    }
    public void IncreaseStats(float attack, float speed, float critical) // ���� ������ �ż���
    {
        characterData.attackPower += attack;
        characterData.attackSpeed += speed;
        characterData.criticalChance += critical;

        // �ִϸ��̼� ��� �ӵ��� ������Ʈ�մϴ�.
        animator.speed = AttackSpeed;
    }
    protected virtual void Update()
    {
        attackCooldown -= Time.deltaTime; // ���� ��ٿ� ����
        if (attackCooldown <= 0f && !isAttacking && !IsRolling() && !IsAnimationPlaying("Attack")) // ��ٿ��� 0 �����̰� ���� ���� �ƴϰ� ������ �ִϸ��̼��� ���� ���� �ƴϰ� ���� �ִϸ��̼��� ���� ���� �ƴϸ� ���� ����
        {
            StartCoroutine(PerformAttack());
            attackCooldown = 1f / AttackSpeed; // ��ٿ� �ʱ�ȭ (���� �ӵ��� ���� ����)
        }
        RegenerateMana(); // �� �����Ӹ��� ���� ȸ��
        TryUseManaSkill(target); // ���� ��ų ��� �õ�
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }
    public void SpawnClone(GameObject clonePrefab, float duration, float powerMultiplier, GameObject target)
    {
        if (clonePrefab != null)
        {
            Debug.Log("�нż�ȯ");
            GameObject clone = Instantiate(clonePrefab, target.transform.position, Quaternion.identity);
            CharacterInfo cloneInfo = clone.GetComponent<CharacterInfo>();

            if (cloneInfo != null)
            {
                // �н��� �ɷ�ġ�� ������ �ɷ�ġ�� ����Ͽ� ���
                cloneInfo.characterData = Instantiate(characterData); // ������ �����͸� �����Ͽ� ��� > ���� ������ �����Ͱ��� �ǵ帮�� �ʱ� ����
                cloneInfo.characterData.attackPower = characterData.attackPower * powerMultiplier;
                cloneInfo.characterData.attackSpeed = characterData.attackSpeed * powerMultiplier;
                cloneInfo.characterData.criticalChance = characterData.criticalChance * powerMultiplier;

                // �н��� ��ǥ ����
                cloneInfo.target = target;

                // ���� �ð� �� �н� ����
                Destroy(clone, duration);
            }
        }
        else
        {
            Debug.LogError("Clone prefab is not assigned.");
        }
    }
    public void RegenerateMana()
    {
        foreach (var skill in skills)
        {
            if (skill is ManaSkill manaSkill)
            {
                manaSkill.RegenerateMana(); // ���� ȸ�� ó��
            }
        }
        if (mpBar != null)
        {
            UpdateManaBar(); // MP �� ������Ʈ
        }

    }
    private void UpdateManaBar()
    {
        if (mpBar != null)
        {
            foreach (var skill in skills)
            {
                if (skill is ManaSkill manaSkill)
                {
                    mpBar.fillAmount = manaSkill.currentMana / manaSkill.maxMana; // ���� ������ ���� fillAmount ������Ʈ
                    break; // ù ��° ManaSkill�� ������Ʈ�ϵ��� ó�� (���� ���� ��ų�� �ִ� ��� �߰� ���� �ʿ�)
                }
            }
        }
    }
    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        BasicAttack();
        yield return new WaitUntil(() => !IsAnimationPlaying("Attack")); // ���� �ִϸ��̼��� ���� ������ ���
        animator.SetTrigger("Idle"); // ������ ���� �� Idle �ִϸ��̼� ����
        isAttacking = false;
    }

    // ���� �⺻ ���� �޼���
    protected virtual void BasicAttack()
    {
        // AttackRange ���� ��� ���� �����Ͽ� �迭�� ��ȯ
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, enemyLayer);
        // ������ ���� �ϳ��� �ִ��� Ȯ��
        if (hits.Length > 0)
        {
            closetTarget = null; // ���� ����� ���� ������ ����
            float closestDistance = Mathf.Infinity; // ���� ����� �Ÿ� �ʱⰪ�� ���Ѵ�� ����

            // ������ ��� ���� ��ȸ�ϸ鼭 ���� ����� ���� ã��
            foreach (var hit in hits)
            {
                Monster enemy = hit.GetComponent<Monster>();
                if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // ���� �����ϰ� ����ִ��� Ȯ��
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position); // ���� ������ �Ÿ� ���
                    if (distance < closestDistance)
                    {
                        closestDistance = distance; // ���� ����� �Ÿ� ������Ʈ
                        closetTarget = hit.gameObject; // ���� ����� �� ������Ʈ
                    }
                }
                else if (enemy.isDie)
                {
                    Array.Clear(hits, 0, hits.Length);
                }
            }
            // ���� ����� ���� ���� ��� ���� ����
            if (closetTarget != null)
            {
                target = closetTarget;
                damage = AttackPower; // �⺻ ���ݷ� ����
                animator.speed = AttackSpeed; // �ִϸ��̼� �ӵ��� ���� �ӵ��� ����
                animator.SetTrigger("Attack"); // ���� �ִϸ��̼� ���� (SetBool ��� SetTrigger ���)
                isCritical = IsCriticalHit();
                if (isCritical)
                {
                    damage *= 2; // ũ��Ƽ�� ��Ʈ �� ������ 2��                    
                    Debug.Log("Critical hit!");
                }
                foreach (Skill Skill in skills)
                {
                    if(Skill is ManaSkill manaSkill)
                    {
                        if (manaSkill is ByenightManaSkill byenightManaSkill)
                        {
                            byenightManaSkill.GainManaOnAttack(); // ������ ȹ��
                        }
                    }                    
                }
                DealDamage(closetTarget, damage); // ������ ������ ����               
                TryUseSkill(closetTarget); // ��ų �ߵ� Ȯ�� ��� �� ��ų ���
            }
        }

    }

    // ��ų �ߵ� Ȯ�� ��� �� ��ų ���
    protected virtual void TryUseSkill(GameObject target)
    {
        foreach (Skill skill in skills)
        {
            if (UnityEngine.Random.value <= skill.skillProbability)
            {
                PlayActiveSkillEffect();
                skill.ActivateSkill(this, target); // ��ų �ߵ�
                break; // �ϳ��� ��ų�� �ߵ�
            }
        }
    }
    protected virtual void TryUseManaSkill(GameObject target)
    {
        foreach (Skill skill in skills)
        {
            if (skill is ManaSkill manaSkill)
            {
                if (manaSkill.currentMana >= manaSkill.maxMana)
                {
                    if (target != null)
                    {
                        manaSkill.ActivateManaSkill(this, target);
                        manaSkill.currentMana = 0; // ������ �ʱ�ȭ
                        UpdateManaBar(); // ���� ��� �� MP �� ������Ʈ
                        Debug.Log("���� ���� �ߵ�");
                    }
                }
            }
        }
    }

    // Ȱ��ȭ�� ��ų ����Ʈ ���
    protected void PlayActiveSkillEffect()
    {
        if (activeSkillEffect != null)
        {
            Debug.Log("Playing skill effect.");
            activeSkillEffect.transform.position = transform.position; // ����Ʈ ��ġ�� ĳ���� ��ġ�� ����
            activeSkillEffect.Play();
        }
        else
        {
            Debug.LogWarning("activeSkillEffect is not assigned!");
        }
    }

    // ũ��Ƽ�� ��Ʈ ���� ����
    public bool IsCriticalHit()
    {
        return UnityEngine.Random.value <= CriticalChance;
    }

    // ������ ó�� ����
    protected virtual void DealDamage(GameObject target, float damage)
    {
        Monster enemy = target.GetComponent<Monster>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, characterData.selectedType);
            // ������ ����Ͽ� ���� �������� ���
            float finalDamage = damage * enemy.GetDamageMultiplier(enemy.defense);

            // �ڽ��� ���� �������� ����
            totalDamageDealt += finalDamage;
            Debug.Log($"{Name} dealt {damage} {AttackType} damage to {target.name}");
        }
    }

    // ���� �ִϸ��̼��� ���� ������ Ȯ��
    private bool IsAnimationPlaying(string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1.0f;
    }

    // Rolling �ִϸ��̼��� ���� ������ Ȯ��
    private bool IsRolling()
    {
        return IsAnimationPlaying("Rolling");
    }

    // Gizmos�� ����Ͽ� ���� ������ ������ ǥ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
    // ������ �����ϴ� �޼���
    public void ApplyBuff(float duration)
    {
        if (isBuffed)
        {
            Debug.Log($"{gameObject.name} is already buffed.");
            return;
        }

        StartCoroutine(BuffCoroutine(duration));
    }

    private IEnumerator BuffCoroutine(float duration)
    {
        isBuffed = true;

        // ���� ȿ�� ���� �̰� �ΰ��� ��ȭ�� ���Ե�
        characterData.attackPower *= 1.5f; // ���ݷ� 50% ����
        characterData.attackSpeed *= 0.7f; // ���� �ӵ� 30% ����
        characterData.attackRange += 1.5f; // ���� ���� 1.5 ����
        Debug.Log($"{characterData.name} has been buffed!");

        yield return new WaitForSeconds(duration);

        // ���� ȿ�� ����
        characterData.attackPower /= 1.5f; // ���ݷ� ���� ����
        characterData.attackSpeed /= 0.7f; // ���� �ӵ� ���� ����
        characterData.attackRange -= 1.5f; // ���� ���� ���� ����
        isBuffed = false;
        Debug.Log($"{characterData.name}'s buff has ended.");
    }
    public bool IsBuffed()
    {
        return isBuffed;
    }
    // ��ų �������̽�
    public interface ISkill
    {
        void ActivateSkill();
    }

    // �нú� �������̽�
    public interface IPassive
    {
        void ActivatePassive();
    }

}