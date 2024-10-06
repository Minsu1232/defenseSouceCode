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
/// �α��� ��ũ��Ʈ
/// </summary>
public class FireBaseAuthManager : MonoBehaviour
{
    public TMP_InputField emailInput; // �̸��� �Է� �ʵ�
    public TMP_InputField passwordInput; // ��й�ȣ �Է� �ʵ�
    public TMP_InputField loginemailInput;
    public TMP_InputField loginpasswordInput;
    public TMP_InputField nickName;
    public TextMeshProUGUI errorText; // ���� �޽��� ǥ��
    public TextMeshProUGUI loginerrorText; // ���� �޽��� ǥ��
    public GameObject signinPanel;
    private FirebaseAuth auth;
    private DatabaseReference databaseReference; // Firebase Realtime Database ����

    private void Start()
    {
        StartCoroutine(WaitForFirebaseAndInitialize());
    }

    private IEnumerator WaitForFirebaseAndInitialize()
    {
        // FirebaseManager�� �ʱ�ȭ�� ������ ���
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // �� ������ ���
        }

        // Firebase �ʱ�ȭ �Ϸ� �� FirebaseAuth �� Database ���� ���
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

    // �г����� Firebase Realtime Database�� �����ϴ� �Լ�
    private void SaveNickNameToDatabase(string userId, string nickName)
    {
        databaseReference.Child("users").Child(userId).Child("nickName").SetValueAsync(nickName).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("�г��� ���� ����!");
            }
            else
            {
                Debug.LogError("�г��� ���� ����");
            }
        });
    }

    public void Signup()
    {
        // �Էµ� �̸��ϰ� ��й�ȣ, �г����� ������
        string email = emailInput.text;
        string password = passwordInput.text;
        string nickNametext = nickName.text;

        // ��й�ȣ ���� �˻� (6�ڸ� �̻��̾�� ��)
        if (password.Length < 6)
        {
            errorText.text = "��й�ȣ�� 6�ڸ� �̻��̾�� �մϴ�.";
            return;
        }

        // �̸��ϰ� ��й�ȣ�� Firebase ����� ���� ��û
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            // ��û�� ��ҵǾ��ų� ������ �߻��� ���
            if (task.IsCanceled || task.IsFaulted)
            {
                // Firebase ���ܸ� �����ͼ� �����ڵ带 Ȯ��
                Firebase.FirebaseException firebaseEx = task.Exception?.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;

                    // �̸����� �̹� ��� ���� ���
                    if (errorCode == AuthError.EmailAlreadyInUse)
                    {
                        errorText.text = "�ߺ��� �̸����Դϴ�!";
                    }
                    else
                    {
                        errorText.text = "�̸��� ���·� ����� ������";
                    }
                }
            }
            // ȸ�������� ���������� �Ϸ�� ���
            else if (task.IsCompletedSuccessfully)
            {
                // AuthResult���� ���� ������ FirebaseUser ��ü ������
                AuthResult authResult = task.Result;
                FirebaseUser newUser = authResult.User; // FirebaseUser�� authResult �ȿ� ����.
                errorText.text = "ȸ������ ����!";
                // ����� UID�� ����Ͽ� �г����� �����ͺ��̽��� ����
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
                loginerrorText.text = "�α��� ����";
            }
            else if (task.IsCompletedSuccessfully)
            {
                FirebaseUser user = task.Result.User;
                loginerrorText.text = "�α��� ����!";
                // �α��� ���� �� �ʿ��� �۾��� ���⿡ �߰� (��: ���� ȭ������ ��ȯ)
                // �α��� ���� �� ��Ʈ�� ������ �̵�
                SceneManager.LoadScene("Intro"); // 'IntroScene'�� ��Ʈ�� ���� �̸��Դϴ�.
            }
        });
    }
}
