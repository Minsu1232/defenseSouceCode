using UnityEngine;
using DG.Tweening; // DoTween ���ӽ����̽� �߰�

public class Meteor : MonoBehaviour
{
    public Vector3 startPosition;  // ���� ��ġ (ī�޶� ���� ��)
    public Vector3 targetPosition; // ��ǥ ��ġ (��� ������ ��ġ)
    public float travelTime = 2f;  // �̵� �ð� (�� �� ���� �̵�����)
    public float damage = 100000f; // ��� ���Ϳ��� �� ���ط�
    public float impactRadius = 15f; // �浹 �� ���Ϳ��� ���ظ� �� ����
    public LayerMask monsterLayer; // ���� ���̾� ����ũ
    public GameObject explosionEffect; // �浹�� ���� ����Ʈ
    public GameObject hitParticle; // ������ ����Ʈ
    

    void Start()
    {
        // ��� �ʱ� ��ġ�� ����
        transform.position = startPosition;

        // ��� ȸ���� ���� (z������ 45�� ȸ��)
        Vector3 euler = transform.eulerAngles;
        euler.z += 45f;
        transform.eulerAngles = euler;

        // DoTween�� ����Ͽ� ��� ��ǥ ��ġ�� �̵���Ŵ
        transform.DOMove(targetPosition, travelTime).OnComplete(OnReachTarget);
    }

    // ��ǥ ��ġ�� �������� �� ȣ��Ǵ� �޼���
    private void OnReachTarget()
    {
        Debug.Log("Meteor has reached the target position!");

        // ���� ���� ���͸� ��� Ž��
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, impactRadius, monsterLayer);

        foreach (var hit in hits)
        {
            // ���� ��ü�� ã��
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null)
            {
                // ���Ϳ��� ���ظ� ����
                enemy.TakeDamage(damage,CharacterData.AttackType.Warrior);
                Vector2 vector = hit.transform.position; //��Ʈ��ƼŬ ��ġ����
                //vector.x += 3f;
                Instantiate(hitParticle,vector,Quaternion.identity);
                Vector2 vector2 = targetPosition;
                vector2.y += 0.9f;
               Instantiate(explosionEffect,vector2,Quaternion.identity);
                Debug.Log($"Meteor hit {enemy.name} and dealt {damage} damage.");
            }
        }

        // �ʿ� ��, �߰����� ó�� (���� ȿ��, �ı� ��)
        Destroy(gameObject); // ��� �ı�
    }

    // ������ �ð������� ǥ���ϱ� ���� Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
