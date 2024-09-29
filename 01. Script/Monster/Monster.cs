using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public MonsterStats stats; // ���͵��� ����
    public List<Transform> waypoints; // ���͵��� �̵� ���
    public float speed = 5f; // �̵� �� ���ǵ�
    public float currentHealth; // ���� ü��
    public float maxHealth;
    public float defense; // ������ ����
    public Image hpBar;
    public TextMeshProUGUI damage;
    public DamageTextManager damageTextManager;

    private SpriteRenderer spriteRenderer;
    private int currentWaypointIndex = 0; // �ε��� üũ ����
    private Animator animator;
    private float damageMultiplier = 1f; // �⺻ ������ ����
    public bool isDie = false; // ���� üũ ����
    private float currentSpeedReduction = 0f; // ���� ����� �ӵ� ���� �ۼ�Ʈ

    private void Awake()
    {   
        MonsterSpawnManager.Instance.RegisterMonster(this); // ���� ���� �Ŵ����� ���
    }

    private void OnDestroy()
    {
        if (MonsterSpawnManager.Instance != null)
        {
            MonsterSpawnManager.Instance.UnregisterMonster(this); // ���� ���� �Ŵ������� ��� ����
        }
    }

    void Start()
    {
        currentHealth = stats.health; // ����ü�� �ʱ�ȭ
        maxHealth = stats.health;
        defense = stats.defense;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UpdateHpBar(); // �ʱ� ü�� �� ������Ʈ
    }

    void Update()
    {
        MoveAlongWaypoints();
    }

    void MoveAlongWaypoints() // ��������Ʈ �ż���
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
        damageMultiplier += increasePercentage / 100f; // ��: 20% ���� -> 1.2��
        yield return new WaitForSeconds(duration);
        damageMultiplier -= increasePercentage / 100f; // ���� ���·� ����
    }
    public void TakeDamage(float damage, CharacterData.AttackType attackType)
    {
        // ������ ����� ������ ���� ���
        float getDamageMultiplier = GetDamageMultiplier(defense);
        float trueDamage = damage * getDamageMultiplier * damageMultiplier;

        currentHealth -= trueDamage;

        // ������ ���� ����
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
                return Color.green;  // �ʷϻ�
            case CharacterData.AttackType.Magic:
                return Color.yellow;  // �����
            case CharacterData.AttackType.Warrior:
                return Color.red;  // ������
            default:
                return Color.white;  // �⺻���� ���
        }
    }
    public void IncreaseHealthByPercentage(float percentage)
    {
        float increaseAmount = maxHealth * (percentage / 100f);
        maxHealth += increaseAmount;
        currentHealth += increaseAmount;
        UpdateHpBar(); // ü�� �� ������Ʈ
    }
    void UpdateHpBar() // ü�� �� ������Ʈ
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currentHealth / stats.health;
        }
    }
    //void ShowDamage(int damageAmount) // ������ ǥ��
    //{
    //    if (damage != null)
    //    {
    //        Vector2 position = damage.transform.position;
    //        // ������ �ؽ�Ʈ�� �����Ͽ� ���ο� ��ü ����
    //        TextMeshProUGUI damageInstance = Instantiate(damage, position, Quaternion.identity);
    //        damageInstance.text = damageAmount.ToString(); // ������ �ؽ�Ʈ ����
    //        StartCoroutine(ShowDamageCoroutine(damageInstance)); // ������ ǥ�� �ڷ�ƾ ����
    //    }
    //}

    //IEnumerator ShowDamageCoroutine(TextMeshProUGUI damageInstance) // ������ �ؽ�Ʈ �Ͻ������� ǥ��
    //{
    //    damageInstance.gameObject.SetActive(true); // ������ �ؽ�Ʈ Ȱ��ȭ
    //    yield return new WaitForSeconds(1f); // 1�� ���
    //    Destroy(damageInstance.gameObject); // 1�� �� ������ �ؽ�Ʈ ����
    //}
    //void ShowDamage(int damageAmount) // ������ ǥ��
    //{
    //    if (damage != null)
    //    {
    //        damage.text = damageAmount.ToString();
    //        StartCoroutine(ShowDamageCoroutine());
    //    }
    //}

    //IEnumerator ShowDamageCoroutine() // ������ �ؽ�Ʈ �Ͻ������� ǥ��
    //{
    //    damage.gameObject.SetActive(true);
    //    yield return new WaitForSeconds(1f);
    //    damage.gameObject.SetActive(false);
    //}

    public float GetDamageMultiplier(float defense) // ���¿� ���� ������ �� ����
    {
        // ������ �������� ������ �������� ���������� ����
        return 1f / (1f + defense / 100f);
    }

    void Die() // ���
    {
        MonsterSpawnManager.Instance.currentMonsterCount--; // ���� ��ü�� ����
        MonsterSpawnManager.Instance.currenMonsterCountText.text = $"{MonsterSpawnManager.Instance.currentMonsterCount}/100";
        AchievementsManager.Instance.totalMonstersDefeated++; // �� ���� óġ �� ����
        waypoints.Clear(); // ��������Ʈ�� ����� �̵� ����
        if (!isDie)
        {
            gameObject.layer = 0; //���̾ �ٲ� ������ ���ͷ� ��� x
            isDie = true;
            animator.SetTrigger("Die");

            // ���� óġ ���� ������Ʈ
            AchievementsManager.Instance.CheckAchievement("���� óġ",AchievementsManager.Instance.totalMonstersDefeated);

            if (gameObject.tag == "Boss")
            {
                MoneyManager.Instance.AddMoney(100);
                MoneyManager.Instance.AddBossCoins(1);
                DamageTextManager.Instance.ShowGetMoneyText(this.transform, 100);
                if (GuideManager.Instance.isFirstTime)
                {
                    Debug.Log("���̵� �Ѿ");
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
        int addMoney = Mathf.CeilToInt(currentRound / 10f); // ���� ���带 10���� ���� �� �ø�

        return addMoney; // �ʿ��� �ݾ��� ����
    }
    IEnumerator DieDelay() // ��ü ���� ������ �ڷ�ƾ
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    // �нú� ȿ���� �����ϴ� �޼���
    public void ApplyPassiveEffect(float speedReductionPercentage)
    {
        // ���� �ӵ� ���Ҹ� �����ϰ� �� �ӵ� ���� ����
        speed += speed * (currentSpeedReduction / 100f);
        currentSpeedReduction = speedReductionPercentage;
        speed -= speed * (currentSpeedReduction / 100f);
    }
}
