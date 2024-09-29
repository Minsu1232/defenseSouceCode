using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 2f; // ������ �̵� �ӵ�
    public float startX = -10f; // ������ �ٽ� ��Ÿ���� ���� ��ġ
    public float endX = 10f; // ������ ȭ���� ������ �� ��ġ

    void Update()
    {
        // ������ ���������� �̵�
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // ������ �� ��ġ�� �����ϸ� �ٽ� ���� ��ġ�� �̵�
        if (transform.position.x > endX)
        {
            Vector2 newPosition = new Vector2(startX, transform.position.y);
            transform.position = newPosition;
        }
    }
}
