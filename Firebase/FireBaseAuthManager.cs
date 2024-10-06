using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Firebase.Database;
/// <summary>
/// 로그인 스크립트
/// </summary>
public class FireBaseAuthManager : MonoBehaviour
{
    public TMP_InputField emailInput; // 이메일 입력 필드
    public TMP_InputField passwordInput; // 비밀번호 입력 필드
    public TMP_InputField loginemailInput;
    public TMP_InputField loginpasswordInput;
    public TMP_InputField nickName;
    public TextMeshProUGUI errorText; // 에러 메시지 표시
    public TextMeshProUGUI loginerrorText; // 에러 메시지 표시
    public GameObject signinPanel;
    private FirebaseAuth auth;
    private DatabaseReference databaseReference; // Firebase Realtime Database 참조

    private void Start()
    {
        StartCoroutine(WaitForFirebaseAndInitialize());
    }

    private IEnumerator WaitForFirebaseAndInitialize()
    {
        // FirebaseManager가 초기화될 때까지 대기
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // 한 프레임 대기
        }

        // Firebase 초기화 완료 후 FirebaseAuth 및 Database 참조 얻기
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FireBaseManager.Instance.GetDatabaseReference();
        Debug.Log("Firebase AuthManager Initialized");
    }


    public void OpenSigninPanel()
    {
        signinPanel.SetActive(true);
    }

    public void CloseSigninPanel()
    {
        signinPanel.SetActive(false);
    }

    // 닉네임을 Firebase Realtime Database에 저장하는 함수
    private void SaveNickNameToDatabase(string userId, string nickName)
    {
        databaseReference.Child("users").Child(userId).Child("nickName").SetValueAsync(nickName).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("닉네임 저장 성공!");
            }
            else
            {
                Debug.LogError("닉네임 저장 실패");
            }
        });
    }

    public void Signup()
    {
        // 입력된 이메일과 비밀번호, 닉네임을 가져옴
        string email = emailInput.text;
        string password = passwordInput.text;
        string nickNametext = nickName.text;

        // 비밀번호 길이 검사 (6자리 이상이어야 함)
        if (password.Length < 6)
        {
            errorText.text = "비밀번호는 6자리 이상이어야 합니다.";
            return;
        }

        // 이메일과 비밀번호로 Firebase 사용자 생성 요청
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            // 요청이 취소되었거나 오류가 발생한 경우
            if (task.IsCanceled || task.IsFaulted)
            {
                // Firebase 예외를 가져와서 에러코드를 확인
                Firebase.FirebaseException firebaseEx = task.Exception?.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;

                    // 이메일이 이미 사용 중인 경우
                    if (errorCode == AuthError.EmailAlreadyInUse)
                    {
                        errorText.text = "중복된 이메일입니다!";
                    }
                    else
                    {
                        errorText.text = "이메일 형태로 만들어 보세요";
                    }
                }
            }
            // 회원가입이 성공적으로 완료된 경우
            else if (task.IsCompletedSuccessfully)
            {
                // AuthResult에서 새로 생성된 FirebaseUser 객체 가져옴
                AuthResult authResult = task.Result;
                FirebaseUser newUser = authResult.User; // FirebaseUser는 authResult 안에 있음.
                errorText.text = "회원가입 성공!";
                // 사용자 UID를 사용하여 닉네임을 데이터베이스에 저장
                SaveNickNameToDatabase(newUser.UserId, nickNametext);
            }
        });
    }

    public void Login()
    {
        string email = loginemailInput.text;
        string password = loginpasswordInput.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                loginerrorText.text = "로그인 실패";
            }
            else if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                loginerrorText.text = "로그인 성공!";
                // 로그인 성공 후 필요한 작업을 여기에 추가 (예: 메인 화면으로 전환)
                // 로그인 성공 후 인트로 씬으로 이동
                SceneManager.LoadScene("Intro"); // 'IntroScene'은 인트로 씬의 이름입니다.
            }
        });
    }
}
