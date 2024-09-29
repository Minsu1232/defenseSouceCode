using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    public static CardDrawer Instance; 

    public Player player;  // �÷��̾� ������Ʈ ����
    public GameObject[] cards; // ī�� ������ �迭 (1���� 6������ ī��)
    public Transform cardSpawnPoint; // ī�尡 ��Ÿ���� ��ġ
    public Camera mainCamera; // ���� ī�޶� ����
    private GameObject drawnCard; // ��ο�� ī��
    public bool isDrawingCard = false; // ī�� ��ο� ������ Ȯ���ϴ� �÷���
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ��ο� ��ư Ŭ�� �� ȣ��
    public void OnDrawButtonClick()
    {
        if (!isDrawingCard && !Player.Instance.IsMoving) // ī�� ��ο� ���� �ƴϰ�, �÷��̾ �������� �ʴ� ��쿡�� ����
        {
            StartCoroutine(DrawCard());
        }
    }

    private IEnumerator DrawCard()
    {
        isDrawingCard = true; // ��ο� ����

        // ���� ī�尡 �����ϸ� ����
        if (drawnCard != null)
        {
            Destroy(drawnCard);
        }

        // ī�� ���� ����Ʈ�� ĳ���� �Ӹ��� �߾����� �̵�
        
        cardSpawnPoint.position = new Vector3(player.transform.position.x, player.transform.position.y +3, 0); // Z���� �ʿ信 ���� ����

        // ������ ī�� ����
        int cardIndex = Random.Range(0, cards.Length);
        drawnCard = Instantiate(cards[cardIndex], cardSpawnPoint.position, Quaternion.identity);

        // ī�� ��ο� �ִϸ��̼�
        drawnCard.transform.localScale = Vector3.zero;
        drawnCard.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // �ִϸ��̼��� ���� ������ ���
        yield return new WaitForSeconds(0.5f);

        // ���õ� ī���� ���ڸ�ŭ �÷��̾� �̵�
        int cardValue = cardIndex + 1; // ī�� �ε����� 1�� ���� ���� ����
         Player.Instance.Move(cardValue);

        // �÷��̾ �̵��� ��ĥ ������ ��� (���÷� 1�� ���)
        while (Player.Instance.IsMoving)
        {
            yield return null; // �÷��̾ �̵��� ��ġ�� ���� ���
        }

        // ��� �� ī�� ����
        yield return new WaitForSeconds(1.5f);
        Destroy(drawnCard);

        isDrawingCard = false; // ��ο� ����
    }
}
