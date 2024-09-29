using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;


public class FirebaseTest : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;

    // ������ �̸��ϰ� ��й�ȣ�� �����մϴ�.
    private string userEmail = "ok050520@gmail.com"; // ���⿡ ���� �̸����� �Է��ϼ���.
    private string userPassword = "!!@@asdasd67"; // ���⿡ ���� ��й�ȣ�� �Է��ϼ���.

    private void Start()
    {
        // Firebase Database ���۷��� �ʱ�ȭ
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        WriteTestData();
    }



    private void WriteTestData()
    {
        // �α��� �Ϸ� �Ŀ� �����͸� ����
        databaseReference.Child("testPath2").Child("exampleData").SetValueAsync("Hello Firebase with Email").ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("������ ���� ����: testPath2/exampleData");
            }
            else
            {
                Debug.LogError("������ ���� ����: " + task.Exception);
            }
        });
    }

}
