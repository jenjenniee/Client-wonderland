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
    /// 뒤끝 콘솔 테이블에 새로운 유저 정보 추가
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
                // 게임 정보의 고유값
                gameDataRowInDate = callback.GetInDate();

                Debug.Log($"게임 정보 데이터 삽입에 성공했습니다: {callback}");
            }
            else
            {
                Debug.LogError($"게임 정보 데이터 삽입에 실패했습니다: {callback}");
            }
        });
    }


    /// <summary>
    /// 뒤끝 콘솔 테이블에서 유저 정보를 불러올 때 호출
    /// </summary>
    public void GameDataLoad()
    {
        Backend.GameData.GetMyData("USER_DATA", new Where(), callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log($"게임 정보 불러오기 성공: {callback}");

                // JSON 파싱
                try
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)
                    {
                        Debug.Log("데이터가 존재하지 않습니다.");
                    }
                    else
                    {
                        // 불러온 게임 정보의 고유값
                        gameDataRowInDate = gameDataJson[0]["inDate"].ToString();
                        // 게임 정보를 각각의 변수에 저장
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
                Debug.LogError($"게임 정보 불러오기 실패: {callback}");
            }
        });
    }
}
