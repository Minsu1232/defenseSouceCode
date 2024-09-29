using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPool : MonoBehaviour
{
    public GameObject damageCanvasPrefab;
    public GameObject textPoolPrefab;
    public int poolSize = 6000;
    public int poolIncrement = 2500; // �ʿ��� ������ �߰��� ������ ����
    private Queue<GameObject> pool = new Queue<GameObject>();
    private bool isExpandingPool = false; // Ǯ Ȯ���� ���� ������ Ȯ��

    void Start()
    {
        InitializePool(poolSize);
    }

    // �ʱ� Ǯ ����
    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(damageCanvasPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    public void ReturnDamageCanvas(GameObject obj)
    {
        obj.SetActive(false);

        // �θ� ���迡�� �����ϰ�, Ǯ�� ��ȯ
        obj.transform.SetParent(null); // �θ� ���ְų�, Ư�� �θ� ������Ʈ�� ����
        pool.Enqueue(obj);


    }
    public GameObject GetDamageCanvas()
    {
        // Ǯ�� ���� ������Ʈ�� �ִ��� Ȯ��
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Ǯ���� ������Ʈ�� ������ Ǯ Ȯ���� �����ϰ�, ��� ���ο� ������Ʈ ����
            if (!isExpandingPool)
            {
                
                StartCoroutine(ExpandPoolGradually(poolIncrement, 250)); // �� ���� 250���� ����
            }
            else
            {
                Debug.LogWarning("Pool is already expanding, waiting for new objects.");
            }

            // Ǯ Ȯ���� ���� ���� �� �ӽ÷� �� ������Ʈ�� ��ȯ�ϰų�, ����ϴ� ������ �߰��� �� �ֽ��ϴ�.
            return null; // �ӽ÷� null ��ȯ (���� ������ ���� ���� ����)
        }
    }
    // Ǯ�� ���� ���� �����ϸ� ���������� Ȯ��
    private IEnumerator ExpandPoolGradually(int increment, int batchSize)
    {
        isExpandingPool = true; // Ǯ Ȯ�� �÷��� ����

        for (int i = 0; i < increment; i++)
        {
            GameObject obj = Instantiate(damageCanvasPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);

            // ���� ���� ������Ʈ�� ������ �� �� ������ ���
            if ((i + 1) % batchSize == 0)
            {
                yield return null; // �� ������ ���
            }
        }

        isExpandingPool = false; // Ǯ Ȯ�� �Ϸ� �� �÷��� ����
    }

   

  
}
