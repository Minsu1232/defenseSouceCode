using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    public static CardDrawer Instance; 

    public Player player;  // 플레이어 오브젝트 참조
    public GameObject[] cards; // 카드 프리팹 배열 (1부터 6까지의 카드)
    public Transform cardSpawnPoint; // 카드가 나타나는 위치
    public Camera mainCamera; // 메인 카메라 참조
    private GameObject drawnCard; // 드로우된 카드
    public bool isDrawingCard = false; // 카드 드로우 중인지 확인하는 플래그
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
    // 드로우 버튼 클릭 시 호출
    public void OnDrawButtonClick()
    {
        if (!isDrawingCard && !Player.Instance.IsMoving) // 카드 드로우 중이 아니고, 플레이어가 움직이지 않는 경우에만 실행
        {
            StartCoroutine(DrawCard());
        }
    }

    private IEnumerator DrawCard()
    {
        isDrawingCard = true; // 드로우 시작

        // 기존 카드가 존재하면 제거
        if (drawnCard != null)
        {
            Destroy(drawnCard);
        }

        // 카드 스폰 포인트를 캐릭터 머리위 중앙으로 이동
        
        cardSpawnPoint.position = new Vector3(player.transform.position.x, player.transform.position.y +3, 0); // Z값은 필요에 따라 조정

        // 무작위 카드 선택
        int cardIndex = Random.Range(0, cards.Length);
        drawnCard = Instantiate(cards[cardIndex], cardSpawnPoint.position, Quaternion.identity);

        // 카드 드로우 애니메이션
        drawnCard.transform.localScale = Vector3.zero;
        drawnCard.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(0.5f);

        // 선택된 카드의 숫자만큼 플레이어 이동
        int cardValue = cardIndex + 1; // 카드 인덱스에 1을 더해 숫자 결정
         Player.Instance.Move(cardValue);

        // 플레이어가 이동을 마칠 때까지 대기 (예시로 1초 대기)
        while (Player.Instance.IsMoving)
        {
            yield return null; // 플레이어가 이동을 마치는 것을 대기
        }

        // 잠시 후 카드 제거
        yield return new WaitForSeconds(1.5f);
        Destroy(drawnCard);

        isDrawingCard = false; // 드로우 종료
    }
}
