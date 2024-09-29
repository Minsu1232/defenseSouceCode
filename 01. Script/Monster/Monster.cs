using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public MonsterStats stats; // 몬스터들의 스탯
    public List<Transform> waypoints; // 몬스터들의 이동 경로
    public float speed = 5f; // 이동 시 스피드
    public float currentHealth; // 현재 체력
    public float maxHealth;
    public float defense; // 몬스터의 방어력
    public Image hpBar;
    public TextMeshProUGUI damage;
    public DamageTextManager damageTextManager;

    private SpriteRenderer spriteRenderer;
    private int currentWaypointIndex = 0; // 인덱스 체크 변수
    private Animator animator;
    private float damageMultiplier = 1f; // 기본 데미지 배율
    public bool isDie = false; // 죽음 체크 변수
    private float currentSpeedReduction = 0f; // 현재 적용된 속도 감소 퍼센트

    private void Awake()
    {   
        MonsterSpawnManager.Instance.RegisterMonster(this); // 몬스터 스폰 매니저에 등록
    }

    private void OnDestroy()
    {
        if (MonsterSpawnManager.Instance != null)
        {
            MonsterSpawnManager.Instance.UnregisterMonster(this); // 몬스터 스폰 매니저에서 등록 해제
        }
    }

    void Start()
    {
        currentHealth = stats.health; // 현재체력 초기화
        maxHealth = stats.health;
        defense = stats.defense;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UpdateHpBar(); // 초기 체력 바 업데이트
    }

    void Update()
    {
        MoveAlongWaypoints();
    }

    void MoveAlongWaypoints() // 웨이포인트 매서드
    {
        if (waypoints.Count == 0) return;
        if (waypoints != null)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 direction = targetWaypoint.position - transform.position;
            transform.Translate(direction.normalized * speed * Time.deltaTime / 60, Space.World);

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                if (currentWaypointIndex == 3)
                {
                    spriteRenderer.flipX = false;
                }
                else if (currentWaypointIndex == 1)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
    }
    public void ApplyDamageIncrease(float increasePercentage, float duration)
    {
        StartCoroutine(DamageIncreaseCoroutine(increasePercentage, duration));
    }

    private IEnumerator DamageIncreaseCoroutine(float increasePercentage, float duration)
    {
        damageMultiplier += increasePercentage / 100f; // 예: 20% 증가 -> 1.2배
        yield return new WaitForSeconds(duration);
        damageMultiplier -= increasePercentage / 100f; // 원래 상태로 복구
    }
    public void TakeDamage(float damage, CharacterData.AttackType attackType)
    {
        // 방어력을 고려한 데미지 배율 계산
        float getDamageMultiplier = GetDamageMultiplier(defense);
        float trueDamage = damage * getDamageMultiplier * damageMultiplier;

        currentHealth -= trueDamage;

        // 데미지 색상 결정
        Color damageColor = GetDamageColor(attackType);

      DamageTextManager.Instance.ShowDamageTextColor(this.transform, Mathf.FloorToInt(trueDamage), damageColor);
        UpdateHpBar();

    if (currentHealth <= 0 && !isDie)
    {
        Die();
    }

    }
    private Color GetDamageColor(CharacterData.AttackType attackType)
    {
        switch (attackType)
        {
            case CharacterData.AttackType.Archer:
                return Color.green;  // 초록색
            case CharacterData.AttackType.Magic:
                return Color.yellow;  // 노란색
            case CharacterData.AttackType.Warrior:
                return Color.red;  // 빨간색
            default:
                return Color.white;  // 기본값은 흰색
        }
    }
    public void IncreaseHealthByPercentage(float percentage)
    {
        float increaseAmount = maxHealth * (percentage / 100f);
        maxHealth += increaseAmount;
        currentHealth += increaseAmount;
        UpdateHpBar(); // 체력 바 업데이트
    }
    void UpdateHpBar() // 체력 바 업데이트
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currentHealth / stats.health;
        }
    }
    //void ShowDamage(int damageAmount) // 데미지 표시
    //{
    //    if (damage != null)
    //    {
    //        Vector2 position = damage.transform.position;
    //        // 데미지 텍스트를 복제하여 새로운 객체 생성
    //        TextMeshProUGUI damageInstance = Instantiate(damage, position, Quaternion.identity);
    //        damageInstance.text = damageAmount.ToString(); // 데미지 텍스트 설정
    //        StartCoroutine(ShowDamageCoroutine(damageInstance)); // 데미지 표시 코루틴 시작
    //    }
    //}

    //IEnumerator ShowDamageCoroutine(TextMeshProUGUI damageInstance) // 데미지 텍스트 일시적으로 표시
    //{
    //    damageInstance.gameObject.SetActive(true); // 데미지 텍스트 활성화
    //    yield return new WaitForSeconds(1f); // 1초 대기
    //    Destroy(damageInstance.gameObject); // 1초 후 데미지 텍스트 제거
    //}
    //void ShowDamage(int damageAmount) // 데미지 표시
    //{
    //    if (damage != null)
    //    {
    //        damage.text = damageAmount.ToString();
    //        StartCoroutine(ShowDamageCoroutine());
    //    }
    //}

    //IEnumerator ShowDamageCoroutine() // 데미지 텍스트 일시적으로 표시
    //{
    //    damage.gameObject.SetActive(true);
    //    yield return new WaitForSeconds(1f);
    //    damage.gameObject.SetActive(false);
    //}

    public float GetDamageMultiplier(float defense) // 방어력에 따른 데미지 값 계산용
    {
        // 방어력이 높을수록 데미지 감소율이 높아지도록 조정
        return 1f / (1f + defense / 100f);
    }

    void Die() // 사망
    {
        MonsterSpawnManager.Instance.currentMonsterCount--; // 현재 개체수 감소
        MonsterSpawnManager.Instance.currenMonsterCountText.text = $"{MonsterSpawnManager.Instance.currentMonsterCount}/100";
        AchievementsManager.Instance.totalMonstersDefeated++; // 총 몬스터 처치 수 증가
        waypoints.Clear(); // 웨이포인트를 지우며 이동 멈춤
        if (!isDie)
        {
            gameObject.layer = 0; //레이어를 바꿔 영웅이 몬스터로 취급 x
            isDie = true;
            animator.SetTrigger("Die");

            // 몬스터 처치 업적 업데이트
            AchievementsManager.Instance.CheckAchievement("몬스터 처치",AchievementsManager.Instance.totalMonstersDefeated);

            if (gameObject.tag == "Boss")
            {
                MoneyManager.Instance.AddMoney(100);
                MoneyManager.Instance.AddBossCoins(1);
                DamageTextManager.Instance.ShowGetMoneyText(this.transform, 100);
                if (GuideManager.Instance.isFirstTime)
                {
                    Debug.Log("가이드 넘어감");
                    GuideManager.Instance.ShowNextStep();
                }
            }
            else
            {
                MoneyManager.Instance.AddMoney(DieMoeny());
                DamageTextManager.Instance.ShowGetMoneyText(this.transform, DieMoeny());
            }
           
           

            StartCoroutine(DieDelay());
        }
       
    }
    int DieMoeny()
    {
        int currentRound = MonsterSpawnManager.Instance.currentRound;
        int addMoney = Mathf.CeilToInt(currentRound / 10f); // 현재 라운드를 10으로 나눈 후 올림

        return addMoney; // 필요한 금액을 리턴
    }
    IEnumerator DieDelay() // 객체 삭제 딜레이 코루틴
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    // 패시브 효과를 적용하는 메서드
    public void ApplyPassiveEffect(float speedReductionPercentage)
    {
        // 기존 속도 감소를 제거하고 새 속도 감소 적용
        speed += speed * (currentSpeedReduction / 100f);
        currentSpeedReduction = speedReductionPercentage;
        speed -= speed * (currentSpeedReduction / 100f);
    }
}
