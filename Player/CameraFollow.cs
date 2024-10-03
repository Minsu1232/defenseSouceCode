using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    public Transform player; // 플레이어의 Transform을 참조
    public Vector3 offset; // 카메라와 플레이어 사이의 거리 오프셋
    public float panSpeed = 0.5f; // 카메라 이동 속도
    public Vector2 panLimitX = new Vector2(-20f, 20f); // X축 카메라 이동 제한
    private Vector2 nowPos, prePos; // 터치 포지션 저장 변수
    private Vector3 movePos; // 이동량 계산 변수
    public float fixedCameraSize = 25f; // 카메라 사이즈
    private Camera cam; // 카메라 참조

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
            cam = Camera.main;
            cam.orthographicSize = fixedCameraSize; // 카메라 사이즈 고정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (cam.orthographicSize != fixedCameraSize)
        {
            cam.orthographicSize = fixedCameraSize; // 카메라 사이즈 고정 유지
        }

        if (player != null && CardDrawer.Instance.isDrawingCard)
        {
            // 카드 뽑는 동안 플레이어에게 카메라 집중
            transform.position = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);
        }
        else
        {
            // 터치가 있을 때 카메라를 이동시킴
            HandleCameraMovement();
        }
    }

    void HandleCameraMovement()
    {
        if (Input.touchCount == 1) // 터치가 하나일 때
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 터치 시작 위치를 저장
                prePos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // 현재 터치 위치를 저장
                nowPos = touch.position;

                // 터치 위치의 차이로 이동량 계산 (X축만 이동, Y축은 고정)
                movePos = (Vector3)(prePos - nowPos) * Time.deltaTime * panSpeed;
                movePos.y = 0; // Y축 이동 차단 (좌우로만 이동)

                // 카메라를 이동
                cam.transform.Translate(movePos);

                // 카메라 이동 제한 (X축만)
                Vector3 pos = cam.transform.position;
                float halfCameraWidth = cam.orthographicSize * cam.aspect;
                pos.x = Mathf.Clamp(pos.x, panLimitX.x + halfCameraWidth, panLimitX.y - halfCameraWidth);

                // 적용된 위치로 카메라 이동
                cam.transform.position = pos;

                // 이전 터치 위치 갱신
                prePos = nowPos;
            }
        }
    }
    //void HandleMouseMovement()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // 마우스 클릭 시작 지점 저장
    //        dragOrigin = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
    //    }

    //    if (Input.GetMouseButton(0))
    //    {
    //        // 드래그 중일 때 차이 계산
    //        Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
    //        difference.y = 0; // Y축 이동 차단 (좌우로만 이동)
    //        transform.position += difference; // 그만큼 카메라 이동
    //    }
    //}
}

