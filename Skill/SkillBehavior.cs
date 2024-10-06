using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 스킬이 목표를 향해 이동하고, 적과 충돌 시 다양한 효과를 적용하는 로직을 처리
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
    private bool isSpecialSkill; // 스킬이 특수한 경우인지 여부
    private float duration;
    private float speed; // 스킬 이동 속도
    private int monsterLayer; // Monster 레이어 인덱스
    private float damageInterval = 1f; // 피해를 주는 주기

    // MythWarriorSkill 관련 변수
    private GameObject swordPrefab;
    private GameObject barrierPrefab;
    private Transform[] dropPoints;
    private List<Transform> availablePoints;
    private List<Transform> usedPoints;
    private bool isBarrierActive = false;

    

    Animator animator;

    //태초
    public Action<Vector3> OnSwordDestroyed; // 검이 파괴될 때 호출되는 콜백
    // 타격 파티클을 연결할 수 있는 퍼블릭 변수
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
        this.monsterLayer = LayerMask.NameToLayer("Monster"); // Monster 레이어 인덱스를 가져옴

        // 스킬 이동 로직 구현
        StartCoroutine(MoveSkill());
    }

    private IEnumerator MoveSkill()
    {
        // 스킬이 특정 범위 내로 이동
        while (true)
        {
            if (target == null) // 타겟이 사라지면 스킬도 사라짐
            {
                Destroy(gameObject);
                yield break;
            }
            // 타겟의 현재 위치를 목표 지점으로 설정
            targetPosition = target.transform.position;

            // 스킬 이동
            float step = speed * Time.deltaTime; // 이동 속도 설정
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step); // 타겟을 향해 이동
            if(caster.characterData.heroName != "흑마법사" && caster.characterData.heroName != "영리치" && caster.characterData.heroName != "요리사" && caster.characterData.heroName != "주먹쥐고일어서") // 회전하면 안되는 류
            {
                // 회전: 타겟의 위치를 향하도록 스킬을 회전시킴
                UpdateRotation();
            }
          

            // 스킬이 타겟에 도달했는지 확인
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
        // 타겟을 향한 방향을 계산
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 타겟을 바라보도록 회전 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //// 충돌한 객체의 레이어가 Monster 레이어인지 확인
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

            // 지나친 적에게 피해를 줌
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

        // 스킬이 타겟에 도달한 후 5초간 지속
        yield return new WaitForSeconds(duration);

        // 지속 시간이 끝난 후 스킬 제거
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
                // 단일 타겟에만 적용
                Monster enemy = target.GetComponent<Monster>();
                if (enemy != null)
                {
                    ApplyDamageAndEffects(enemy, finalDamage, isCritical);

                    SpawnHitParticle(enemy.transform.position);
                }
            }
            else
            {
                // 범위가 있는 경우, 범위 내 모든 적에게 적용
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

            // 폭발 후 스킬 삭제
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

        // duration 동안 매초마다 실행
        for (int i = 0; i < duration; i++)
        {
            // 범위가 있는 경우, 범위 내 모든 적에게 적용
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
            foreach (Collider2D hit in hits)
            {
                Monster enemy = hit.GetComponent<Monster>();
                if (enemy != null)
                {
                    if(caster.characterData.heroName == "주먹쥐고일어서")
                    {
                        ApplyDamageAndEffects(enemy, finalDamage, isCritical);
                        // 몬스터 방어력을 고려한 실제 데미지 계산
                        float actualDamage = finalDamage * enemy.GetDamageMultiplier(enemy.defense);

                        // 실제 들어간 데미지를 누적
                        caster.totalDamageDealt += actualDamage;

                    }
                    else
                    {
                        // 이부분을 1초에 한번씩 재생
                        SpawnHitParticle(hit.transform.position);
                        ApplyDamageAndEffects(enemy, finalDamage, isCritical);
                        // 몬스터 방어력을 고려한 실제 데미지 계산
                        float actualDamage = finalDamage * enemy.GetDamageMultiplier(enemy.defense);

                        // 실제 들어간 데미지를 누적
                        caster.totalDamageDealt += actualDamage;
                    }
                   
                }
            }

            // 1초 대기
            yield return new WaitForSeconds(1f);
        }

        // 오브젝트 파괴 전에 콜백 호출 (위치 정보 전달)
        OnSwordDestroyed?.Invoke(transform.position);

        // 루프가 끝난 후 오브젝트를 즉시 파괴
        Destroy(gameObject);

    }
    private void ApplyDamageAndEffects(Monster enemy, float finalDamage, bool isCritical)
    {
        enemy.TakeDamage(finalDamage,caster.AttackType);

        // 몬스터의 방어력에 따른 실제 데미지 계산
        float actualDamage = finalDamage * enemy.GetDamageMultiplier(enemy.defense);

        // 최종 계산된 실제 데미지를 누적
        caster.totalDamageDealt += actualDamage;        

        if (hasSlowEffect) // 이속 감소
        {
            enemy.speed -= slowAmount;
            if (enemy.speed < 10)
            {
                enemy.speed = 10;
            }
           
        }

        if (hasDefenseReduction) // 방어력감소
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
        // 타겟 위치를 시각적으로 표시
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
    /// 태초마법사
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="swordPrefab"></param>
    /// <param name="barrierPrefab"></param>
    /// <param name="dropPoints"></param>

}
