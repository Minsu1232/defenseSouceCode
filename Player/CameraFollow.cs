using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    public Transform player; // �÷��̾��� Transform�� ����
    public Vector3 offset; // ī�޶�� �÷��̾� ������ �Ÿ� ������
    public float panSpeed = 0.5f; // ī�޶� �̵� �ӵ�
    public Vector2 panLimitX = new Vector2(-20f, 20f); // X�� ī�޶� �̵� ����
    private Vector2 nowPos, prePos; // ��ġ ������ ���� ����
    private Vector3 movePos; // �̵��� ��� ����
    public float fixedCameraSize = 25f; // ī�޶� ������
    private Camera cam; // ī�޶� ����

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
            cam = Camera.main;
            cam.orthographicSize = fixedCameraSize; // ī�޶� ������ ����
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
            cam.orthographicSize = fixedCameraSize; // ī�޶� ������ ���� ����
        }

        if (player != null && CardDrawer.Instance.isDrawingCard)
        {
            // ī�� �̴� ���� �÷��̾�� ī�޶� ����
            transform.position = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);
        }
        else
        {
            // ��ġ�� ���� �� ī�޶� �̵���Ŵ
            HandleCameraMovement();
        }
    }

    void HandleCameraMovement()
    {
        if (Input.touchCount == 1) // ��ġ�� �ϳ��� ��
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // ��ġ ���� ��ġ�� ����
                prePos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // ���� ��ġ ��ġ�� ����
                nowPos = touch.position;

                // ��ġ ��ġ�� ���̷� �̵��� ��� (X�ุ �̵�, Y���� ����)
                movePos = (Vector3)(prePos - nowPos) * Time.deltaTime * panSpeed;
                movePos.y = 0; // Y�� �̵� ���� (�¿�θ� �̵�)

                // ī�޶� �̵�
                cam.transform.Translate(movePos);

                // ī�޶� �̵� ���� (X�ุ)
                Vector3 pos = cam.transform.position;
                float halfCameraWidth = cam.orthographicSize * cam.aspect;
                pos.x = Mathf.Clamp(pos.x, panLimitX.x + halfCameraWidth, panLimitX.y - halfCameraWidth);

                // ����� ��ġ�� ī�޶� �̵�
                cam.transform.position = pos;

                // ���� ��ġ ��ġ ����
                prePos = nowPos;
            }
        }
    }
    //void HandleMouseMovement()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // ���콺 Ŭ�� ���� ���� ����
    //        dragOrigin = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
    //    }

    //    if (Input.GetMouseButton(0))
    //    {
    //        // �巡�� ���� �� ���� ���
    //        Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
    //        difference.y = 0; // Y�� �̵� ���� (�¿�θ� �̵�)
    //        transform.position += difference; // �׸�ŭ ī�޶� �̵�
    //    }
    //}
}

