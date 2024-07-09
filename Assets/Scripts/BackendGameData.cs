using UnityEngine;
using BackEnd;

public class BackendGameData
{
    [System.Serializable]
    public class GameDataLoadEvent : UnityEngine.Events.UnityEvent { }
    public GameDataLoadEvent onGameDataLoadEvent = new GameDataLoadEvent();

    private static BackendGameData instance = null;
    public static BackendGameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BackendGameData();
            }
            return instance;
        }
    }

    private UserGameData userGameData = new UserGameData();
    public UserGameData UserGameData => userGameData;

    private string gameDataRowInDate = string.Empty;

    /// <summary>
    /// �ڳ� �ܼ� ���̺��� ���ο� ���� ���� �߰�
    /// </summary>
    public void GameDataInsert()
    {
        userGameData.Reset();

        Param param = new Param()
        {
            { "heart",      userGameData.heart },
            { "equipHead",  userGameData.equipHead }
        };

        Backend.GameData.Insert("USER_DATA", param, callback =>
        {
            if (callback.IsSuccess())
            {
                // ���� ������ ������
                gameDataRowInDate = callback.GetInDate();

                Debug.Log($"���� ���� ������ ���Կ� �����߽��ϴ�: {callback}");
            }
            else
            {
                Debug.LogError($"���� ���� ������ ���Կ� �����߽��ϴ�: {callback}");
            }
        });
    }


    /// <summary>
    /// �ڳ� �ܼ� ���̺����� ���� ������ �ҷ��� �� ȣ��
    /// </summary>
    public void GameDataLoad()
    {
        Backend.GameData.GetMyData("USER_DATA", new Where(), callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log($"���� ���� �ҷ����� ����: {callback}");
                Debug.Log("랄랼랄");

                // JSON �Ľ�
                try
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)
                    {
                        Debug.Log("�����Ͱ� �������� �ʽ��ϴ�.");
                    }
                    else
                    {
                        // �ҷ��� ���� ������ ������
                        gameDataRowInDate = gameDataJson[0]["inDate"].ToString();
                        // ���� ������ ������ ������ ����
                        userGameData.heart = int.Parse(gameDataJson[0]["heart"].ToString());
                        userGameData.equipHead = int.Parse(gameDataJson[0]["equipHead"].ToString());

                        onGameDataLoadEvent?.Invoke();
                    }
                }
                catch (System.Exception e)
                {
                    userGameData.Reset();
                    Debug.LogError(e);
                }
            }
            else
            {
                Debug.LogError($"���� ���� �ҷ����� ����: {callback}");
            }
        });
    }

    public void IncreaseHeart()
    {
        // heart 추가하기

        Debug.Log("heart를 1 증가시킵니다.");
        userGameData.heart += 1;
    }
    public void DecreaseHeart()
    {
        // heart 추가하기

        Debug.Log("heart를 1 증가시킵니다.");
        userGameData.heart -= 1;
    }

    public void UpdateGameMoney()
    {
        // Step 3. 게임 재화 수정하기

        if (userGameData == null)
        {
            Debug.LogError("데이터가 존재하지 않습니다. Initialize 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param();
        param.Add("heart", userGameData.heart);

        BackendReturnObject bro = null;

        Debug.Log("나의 데이터를 수정합니다");
        bro = Backend.GameData.Update("GAME_MONEY", new Where(), param);

        if (bro.IsSuccess())
        {
            Debug.Log("데이터 수정에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("데이터 수정에 실패했습니다. : " + bro);
        }
    }
}
