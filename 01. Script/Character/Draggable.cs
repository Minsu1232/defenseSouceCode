using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 originalPosition; // ���� ��ġ�� ������ ����
    private Vector3 offset;
    private HeroManager heroManager;

    void Start()
    {
        heroManager = FindObjectOfType<HeroManager>();
    }

    void OnMouseDown()
    {
        isDragging = true;
        originalPosition = transform.position; // �巡�� ���� �� ���� ��ġ ����
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
        // ���ο� ��ġ�� HeroManager���� ����
        heroManager.UpdateHeroPosition(gameObject, transform.position, originalPosition);
    }
}
