using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 유닛들의 최상위 클래스 (현재 클래스는 너무 복잡하게 되었다 생각합니다. 간소화 필요..)
/// </summary>
public abstract class CharacterInfo : MonoBehaviour
{

    public CharacterData characterData; // 캐릭터 정보
    public LayerMask enemyLayer; // 공격할 대상 레이어
    public ParticleSystem activeSkillEffect; // 스킬 발동 파티클 시스템
    public float baseAttackPower; // 기본 공격력 (강화되지 않은 원래 공격력)
    public float baseAttackSpeed; // 기본 공격속도 
    public float baseAttackCritical; // 기본 크리티컬확률 
    public float totalDamageDealt; // 유닛이 가한 총 데미지
    public Transform arrowTransform;
    protected Animator animator;
    protected bool isAttacking = false; // 공격 제어 변수
    protected bool isRolling = false; // 구르기 제어 변수
    public string Name => characterData.heroName;

    protected GameObject closetTarget;
    protected float damage;
    protected bool isCritical;
    public int Level // 레벨 프로퍼티 (강화 관리)
    {
        get => characterData.level;
        set
        {
            characterData.level = value;
            UpdateStats(); // 레벨이 변경되면 스탯을 업데이트
        }
    }

    // 캐릭터의 정보를 반환하는 읽기용 프로퍼티
    public float AttackPower => characterData.attackPower;
    public float AttackSpeed => characterData.attackSpeed;
    public float AttackRange => characterData.attackRange;
    public float CriticalChance => characterData.criticalChance;
    public CharacterData.AttackType AttackType => characterData.selectedType;

    public List<Skill> skills; // 스킬 목록
    public Image mpBar; // UI에서 할당된 MP 바 이미지
    private SpriteRenderer spriteRenderer;
    public GameObject attackprefab => characterData.attackPrefab;
    public GameObject target;
    private bool isBuffed = false; // 현재 버프 상태를 나타내는 변수
    public abstract void PassiveEffect();

    protected abstract void UnlockNewSkill();


    private float attackCooldown = 0f; // 공격 쿨다운 타이머

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = AttackSpeed; // 애니메이션 재생 속도를 초기화합니다.    



        ApplyUpgradeCount(1.03f, 1.05f, 1.02f);
        // 레벨이 1보다 크면 스탯 업데이트
        if (Level > 1)
        {
            UpdateStats();
        }

        animator.speed = AttackSpeed; // 애니메이션 재생 속도 초기화
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// CSV 초기값에 따른 강화 배율을 위해
    /// </summary>
    public void ApplyUpgradeCount(float attackMultiplier, float speedMultiplier, float criticalMultiplier)
    {
        // 강화 카운트(Upgrade Count)에 따른 스탯 계산 강화 카운트는 Firebase에서 로드됨
        int upgradeCount = characterData.upgradeCount;

        // 베이스 값을 기준으로 강화된 스탯 할당 (characterData에서 값 참조)
        characterData.attackPower = characterData.baseAttackPower * Mathf.Pow(attackMultiplier, upgradeCount);
        characterData.attackSpeed = characterData.baseAttackSpeed * Mathf.Pow(speedMultiplier, upgradeCount);
        characterData.criticalChance = characterData.baseCriticalChance * Mathf.Pow(criticalMultiplier, upgradeCount);
    }
    private void UpdateStats()
    {
        // 레벨에 따라 스탯 업데이트 (레벨이 1보다 높을 때만 호출됨)
        characterData.attackPower = characterData.attackPower * Mathf.Pow(1.05f, Level - 1);
        characterData.attackSpeed = characterData.attackSpeed * Mathf.Pow(1.05f, Level - 1);
        characterData.criticalChance = characterData.criticalChance * Mathf.Pow(1.05f, Level - 1);

        Debug.Log($"{characterData.heroName}'s stats have been updated for level {Level}.");
    }

    public void LevelUp() // 프로퍼티 호출
    {
        Level++; // 레벨 증가
        Debug.Log($"{characterData.heroName} has leveled up to level {Level}!");
    }
    public void ApplyItemAttackPowerIncrease(float percentage)
    {
        float increase = baseAttackPower * (percentage / 100f);
        IncreaseStats(increase, 0, 0);
    }
    public void IncreaseStats(float attack, float speed, float critical) // 스탯 증가용 매서드
    {
        characterData.attackPower += attack;
        characterData.attackSpeed += speed;
        characterData.criticalChance += critical;

        // 애니메이션 재생 속도를 업데이트합니다.
        animator.speed = AttackSpeed;
    }
    protected virtual void Update()
    {
        attackCooldown -= Time.deltaTime; // 공격 쿨다운 감소
        if (attackCooldown <= 0f && !isAttacking && !IsRolling() && !IsAnimationPlaying("Attack")) // 쿨다운이 0 이하이고 공격 중이 아니고 구르기 애니메이션이 실행 중이 아니고 공격 애니메이션이 실행 중이 아니면 공격 실행
        {
            StartCoroutine(PerformAttack());
            attackCooldown = 1f / AttackSpeed; // 쿨다운 초기화 (공격 속도에 따라 결정)
        }
        RegenerateMana(); // 매 프레임마다 마나 회복
        TryUseManaSkill(target); // 마나 스킬 사용 시도
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }
    public void SpawnClone(GameObject clonePrefab, float duration, float powerMultiplier, GameObject target)
    {
        if (clonePrefab != null)
        {
            Debug.Log("분신소환");
            GameObject clone = Instantiate(clonePrefab, target.transform.position, Quaternion.identity);
            CharacterInfo cloneInfo = clone.GetComponent<CharacterInfo>();

            if (cloneInfo != null)
            {
                // 분신의 능력치를 원본의 능력치에 기반하여 계산
                cloneInfo.characterData = Instantiate(characterData); // 원본의 데이터를 복사하여 사용 > 기존 원본의 데이터값을 건드리지 않기 위해
                cloneInfo.characterData.attackPower = characterData.attackPower * powerMultiplier;
                cloneInfo.characterData.attackSpeed = characterData.attackSpeed * powerMultiplier;
                cloneInfo.characterData.criticalChance = characterData.criticalChance * powerMultiplier;

                // 분신의 목표 설정
                cloneInfo.target = target;

                // 일정 시간 후 분신 제거
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
                manaSkill.RegenerateMana(); // 마나 회복 처리
            }
        }
        if (mpBar != null)
        {
            UpdateManaBar(); // MP 바 업데이트
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
                    mpBar.fillAmount = manaSkill.currentMana / manaSkill.maxMana; // 현재 마나에 따른 fillAmount 업데이트
                    break; // 첫 번째 ManaSkill만 업데이트하도록 처리 (여러 마나 스킬이 있는 경우 추가 로직 필요)
                }
            }
        }
    }
    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        BasicAttack();
        yield return new WaitUntil(() => !IsAnimationPlaying("Attack")); // 공격 애니메이션이 끝날 때까지 대기
        animator.SetTrigger("Idle"); // 공격이 끝난 후 Idle 애니메이션 실행
        isAttacking = false;
    }

    // 공통 기본 공격 메서드
    protected virtual void BasicAttack()
    {
        // AttackRange 내의 모든 적을 감지하여 배열로 반환
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange, enemyLayer);
        // 감지된 적이 하나라도 있는지 확인
        if (hits.Length > 0)
        {
            closetTarget = null; // 가장 가까운 적을 저장할 변수
            float closestDistance = Mathf.Infinity; // 가장 가까운 거리 초기값을 무한대로 설정

            // 감지된 모든 적을 순회하면서 가장 가까운 적을 찾음
            foreach (var hit in hits)
            {
                Monster enemy = hit.GetComponent<Monster>();
                if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // 적이 존재하고 살아있는지 확인
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position); // 현재 적과의 거리 계산
                    if (distance < closestDistance)
                    {
                        closestDistance = distance; // 가장 가까운 거리 업데이트
                        closetTarget = hit.gameObject; // 가장 가까운 적 업데이트
                    }
                }
                else if (enemy.isDie)
                {
                    Array.Clear(hits, 0, hits.Length);
                }
            }
            // 가장 가까운 적이 있을 경우 공격 실행
            if (closetTarget != null)
            {
                target = closetTarget;
                damage = AttackPower; // 기본 공격력 설정
                animator.speed = AttackSpeed; // 애니메이션 속도를 공격 속도에 맞춤
                animator.SetTrigger("Attack"); // 공격 애니메이션 실행 (SetBool 대신 SetTrigger 사용)
                isCritical = IsCriticalHit();
                if (isCritical)
                {
                    damage *= 2; // 크리티컬 히트 시 데미지 2배                    
                    Debug.Log("Critical hit!");
                }
                foreach (Skill Skill in skills)
                {
                    if(Skill is ManaSkill manaSkill)
                    {
                        if (manaSkill is ByenightManaSkill byenightManaSkill)
                        {
                            byenightManaSkill.GainManaOnAttack(); // 마나를 획득
                        }
                    }                    
                }
                DealDamage(closetTarget, damage); // 적에게 데미지 적용               
                TryUseSkill(closetTarget); // 스킬 발동 확률 계산 및 스킬 사용
            }
        }

    }

    // 스킬 발동 확률 계산 및 스킬 사용
    protected virtual void TryUseSkill(GameObject target)
    {
        foreach (Skill skill in skills)
        {
            if (UnityEngine.Random.value <= skill.skillProbability)
            {
                PlayActiveSkillEffect();
                skill.ActivateSkill(this, target); // 스킬 발동
                break; // 하나의 스킬만 발동
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
                        manaSkill.currentMana = 0; // 마나를 초기화
                        UpdateManaBar(); // 마나 사용 후 MP 바 업데이트
                        Debug.Log("마나 공격 발동");
                    }
                }
            }
        }
    }

    // 활성화된 스킬 이펙트 재생
    protected void PlayActiveSkillEffect()
    {
        if (activeSkillEffect != null)
        {
            Debug.Log("Playing skill effect.");
            activeSkillEffect.transform.position = transform.position; // 이펙트 위치를 캐릭터 위치로 설정
            activeSkillEffect.Play();
        }
        else
        {
            Debug.LogWarning("activeSkillEffect is not assigned!");
        }
    }

    // 크리티컬 히트 여부 결정
    public bool IsCriticalHit()
    {
        return UnityEngine.Random.value <= CriticalChance;
    }

    // 데미지 처리 로직
    protected virtual void DealDamage(GameObject target, float damage)
    {
        Monster enemy = target.GetComponent<Monster>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, characterData.selectedType);
            // 방어력을 고려하여 최종 데미지를 계산
            float finalDamage = damage * enemy.GetDamageMultiplier(enemy.defense);

            // 자신이 가한 데미지를 누적
            totalDamageDealt += finalDamage;
            Debug.Log($"{Name} dealt {damage} {AttackType} damage to {target.name}");
        }
    }

    // 공격 애니메이션이 실행 중인지 확인
    private bool IsAnimationPlaying(string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1.0f;
    }

    // Rolling 애니메이션이 실행 중인지 확인
    private bool IsRolling()
    {
        return IsAnimationPlaying("Rolling");
    }

    // Gizmos를 사용하여 공격 범위를 원으로 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
    // 버프를 적용하는 메서드
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

        // 버프 효과 적용 이건 인게임 강화도 포함됨
        characterData.attackPower *= 1.5f; // 공격력 50% 증가
        characterData.attackSpeed *= 0.7f; // 공격 속도 30% 증가
        characterData.attackRange += 1.5f; // 공격 범위 1.5 증가
        Debug.Log($"{characterData.name} has been buffed!");

        yield return new WaitForSeconds(duration);

        // 버프 효과 해제
        characterData.attackPower /= 1.5f; // 공격력 원상 복귀
        characterData.attackSpeed /= 0.7f; // 공격 속도 원상 복귀
        characterData.attackRange -= 1.5f; // 공격 범위 원상 복귀
        isBuffed = false;
        Debug.Log($"{characterData.name}'s buff has ended.");
    }
    public bool IsBuffed()
    {
        return isBuffed;
    }
    // 스킬 인터페이스
    public interface ISkill
    {
        void ActivateSkill();
    }

    // 패시브 인터페이스
    public interface IPassive
    {
        void ActivatePassive();
    }

}