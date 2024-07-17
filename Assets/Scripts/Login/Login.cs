using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class Login : LoginBase
{
	[SerializeField]
	private	Image				imageID;			// ID 필드 색상 변경
	[SerializeField]
	private	TMP_InputField		inputFieldID;		// ID 필드 텍스트 정보 추출
	[SerializeField]
	private	Image				imagePW;			// PW 필드 색상 변경
	[SerializeField]
	private	TMP_InputField		inputFieldPW;		// PW 필드 텍스트 정보 추출

	[SerializeField]
	private	Button				btnLogin;           // 로그인 버튼(상호작용 가능/불가능)

	[SerializeField]
	private GameObject loginUI;

	[SerializeField]
	private UserInfo userInfo;

	[SerializeField]
	private GameObject loginScenario;
	
	/// <summary>
	/// 회원가입에서 로그인으로 넘어올 때 호출
	/// </summary>
	public void OnClickCloseRegister()
	{
		loginUI.SetActive(true);
		ResetUI(imageID, imagePW);
	}


	/// <summary>
	/// "로그" 버튼을 눌렀을 때 호출
	/// </summary>
	public void OnClickLogin()
	{
		// 매개변수로 입력한 InputField UI의 색상과 Message 내용 초기화
		ResetUI(imageID, imagePW);

		// 필드 값이 비어있는지 체크
		if ( IsFieldDataEmpty(imageID, inputFieldID.text, "ID") )	return;
		if ( IsFieldDataEmpty(imagePW, inputFieldPW.text, "PW") )	return;

		// 로그인 버튼을 연타하지 못하도록 상호작용 비활성화
		btnLogin.interactable = false;

		// 서버에 로그인을 요청하는 동안 화면에 출력하는 내용 업데이트
		// ex) 로그인 관련 텍스트 출력, 톱니바퀴 아이콘 회전 등
		StartCoroutine(nameof(LoginProcess));

		// 뒤끝 서버 로그인 시도
		ResponseToLogin(inputFieldID.text, inputFieldPW.text);
	}

	/// <summary>
	/// 로그인 시도 후 서버로부터 전달받은 Message를 기반으로 로직 처리
	/// </summary>
	private void ResponseToLogin(string ID, string PW)
	{
		// 서버에 로그인 요청(비동기)
		Backend.BMember.CustomLogin(ID, PW, callback =>
		{
			StopCoroutine(nameof(LoginProcess));

			// 로그인 성공
			if ( callback.IsSuccess() )
			{
				PlayerPrefs.SetInt("isLoading", 1);
                SetMessage($"Welcome, {inputFieldID.text}!");

                // ��� ��Ʈ ������ �ҷ�����
                //BackendChartData.LoadAllChart();

				// userDB
                //BackendGameData.Instance.GameDataLoad(); // 데이터를 초기화 합니다
                //userInfo.GetUserInfoFromBackend();

				// Lobby ������ �̵�
				//Utils.LoadScene(SceneNames.Loby);
				loginScenario.SetActive(true);
				Loading.SetLoading();

                // 불러오려는 데이터가 존재하지 않을 경우, 데이터를 새로 추가합니다.
                if (BackendGameData.Instance.UserGameData == null)
                {
                    BackendGameData.Instance.GameDataInsert();
                }
            }
			// �α��� ����
			else
			{
				// 로그인에 실패했을 때는 다시 로그인을 해야하기 때문에 "로그"버튼 상호작용 활성화
				btnLogin.interactable = true;

				string message = string.Empty;

				switch ( int.Parse(callback.GetStatusCode()) )
				{
					case 401:	// 존재하지 않는 아이, 잘못된 비밀번호
						message = callback.GetMessage().Contains("customId") ? "This ID does not exist." : "The password does not exist.";
						break;
					case 403:	// 유저 or 디바이스 차단
						message = callback.GetMessage().Contains("user") ? "This user has been blocked." : "This device is blocked.";
						break;
					case 410:	// 탈퇴 진행중
						message = "This user is in the process of withdrawing.";
						break;
					default:
						message = callback.GetMessage();
						break;
				}

				// StatusCode 401에서 "잘못된 비밀번호 입니다."일 때
				if ( message.Contains("password") )
				{
					GuideForIncorrectlyEnteredData(imagePW, message);
				}
				else
				{
					GuideForIncorrectlyEnteredData(imageID, message);
				}
			}
		});
	}
	
	private IEnumerator LoginProcess()
	{
		float time = 0;

		while ( true )
		{
			time += Time.deltaTime;

			SetMessage($"You are logging in... {time:F1}");

			yield return null;
		}
	}
}


/* // �α��� - ����
var bro = Backend.BMember.CustomLogin(ID, PW);
		
		// �α��� ����
		if ( bro.IsSuccess() )
		{
			Debug.Log($"�α��� ���� : {bro.GetStatusCode()}");
		}
		// �α��� ����
		else
		{
			string message = string.Empty;

			switch ( int.Parse(bro.GetStatusCode()) )
			{
				case 401:	// �������� �ʴ� ���̵�, �߸��� ��й�ȣ
					message = bro.GetMessage().Contains("customId") ? "�������� �ʴ� ���̵��Դϴ�." : "�߸��� ��й�ȣ �Դϴ�.";
					break;
				case 403:	// ���� or ����̽� ����
					message = bro.GetMessage().Contains("user") ? "���ܴ��� �����Դϴ�." : "���ܴ��� ����̽��Դϴ�.";
					break;
				case 410:	// Ż�� ������
					message = "Ż�� �������� �����Դϴ�.";
					break;
				default:
					message = bro.GetMessage();
					break;
			}

			// StatusCode 401���� "�߸��� ��й�ȣ �Դϴ�." �� ��
			if ( message.Contains("��й�ȣ") )
			{
				GuideForIncorrectlyEnteredData(imagePW, message);
			}
			else
			{
				GuideForIncorrectlyEnteredData(imageID, message);
			}
		}
*/