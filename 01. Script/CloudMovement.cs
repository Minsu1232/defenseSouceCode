using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 2f; // 구름의 이동 속도
    public float startX = -10f; // 구름이 다시 나타나는 시작 위치
    public float endX = 10f; // 구름이 화면을 떠나는 끝 위치

    void Update()
    {
        // 구름을 오른쪽으로 이동
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // 구름이 끝 위치에 도달하면 다시 시작 위치로 이동
        if (transform.position.x > endX)
        {
            Vector2 newPosition = new Vector2(startX, transform.position.y);
            transform.position = newPosition;
        }
    }
}
