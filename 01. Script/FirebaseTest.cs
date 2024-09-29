using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;


public class FirebaseTest : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;

    // 유저의 이메일과 비밀번호를 설정합니다.
    private string userEmail = "ok050520@gmail.com"; // 여기에 실제 이메일을 입력하세요.
    private string userPassword = "!!@@asdasd67"; // 여기에 실제 비밀번호를 입력하세요.

    private void Start()
    {
        // Firebase Database 레퍼런스 초기화
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        
        WriteTestData();
    }



    private void WriteTestData()
    {
        // 로그인 완료 후에 데이터를 쓰기
        databaseReference.Child("testPath2").Child("exampleData").SetValueAsync("Hello Firebase with Email").ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("데이터 쓰기 성공: testPath2/exampleData");
            }
            else
            {
                Debug.LogError("데이터 쓰기 실패: " + task.Exception);
            }
        });
    }

}
