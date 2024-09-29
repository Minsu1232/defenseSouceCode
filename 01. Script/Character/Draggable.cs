using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 originalPosition; // 원래 위치를 저장할 변수
    private Vector3 offset;
    private HeroManager heroManager;

    void Start()
    {
        heroManager = FindObjectOfType<HeroManager>();
    }

    void OnMouseDown()
    {
        isDragging = true;
        originalPosition = transform.position; // 드래그 시작 시 원래 위치 저장
        offset = originalPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(mousePosition.x, mousePosition.y, originalPosition.z);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        // 새로운 위치를 HeroManager에게 전달
        heroManager.UpdateHeroPosition(gameObject, transform.position, originalPosition);
    }
}
