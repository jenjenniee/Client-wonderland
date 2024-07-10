using UnityEngine;
using BackEnd;
using TMPro;
using System;
using UnityEngine.Events;
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
            { "equipHead",  userGameData.equipHead },
            { "hasItem",    userGameData.hasItem },
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
                        userGameData.hasItem = bool.Parse(gameDataJson[0]["hasItem"].ToString());

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

    public void IncreaseHeart(int num)
    {
        // heart 추가하기
        Debug.Log("heart를 1 증가시킵니다.");
        userGameData.heart += num;
        UpdateGameMoney();
    }
    public void DecreaseHeart(int num)
    {
        // heart 추가하기

        Debug.Log("heart를 1 증가시킵니다.");
        //if(userGameData.heart >= num)
        //    userGameData.heart -= num;
        userGameData.heart = Mathf.Max(userGameData.heart - num, 0);
        UpdateGameMoney();
    }

    public void UpdateGameMoney()
    {
        // Step 3. 게임 재화 수정하기

        if (userGameData == null)
        {
            Debug.LogError("데이터가 존재하지 않습니다. Initialize 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param()
        {
            { "heart", userGameData.heart }
        };

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.LogError("유저의 inDate 정보가 없어 게임 정보 데이터 수정에 실패했습니다.");
        }
        else
        {
            Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param, callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log($"데이터 수정에 성공했습니다. : {callback}");
                    //action?.Invoke();
                }
                else
                {
                    Debug.LogError($"데이터 수정에 실패했습니다. : {callback}");
                }
            });
        }
        /*
        BackendReturnObject bro = null;

        Debug.Log("나의 데이터를 수정합니다");
        bro = Backend.GameData.Update("GAME_MONEY", new Where(), param);
        */
        
    }

    public bool BuyItem(int index, int price)
    {
        // index는 아이템이 여러개일때 유효함.

        if (userGameData.heart >= price)
        {
            DecreaseHeart(price);
            userGameData.hasItem = true;
            UpdateItem(); 
            return true;
        }
        // 구매 실패 : 재화 부족
        return false;
    }

    public void UpdateItem()
    {
        if (userGameData == null)
        {
            Debug.LogError("데이터가 존재하지 않습니다. Initialize 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param()
        {
            { "hasItem", userGameData.hasItem }
        };

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.LogError("유저의 inDate 정보가 없어 게임 정보 데이터 수정에 실패했습니다.");
        }
        else
        {
            Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param, callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log($"데이터 수정에 성공했습니다. : {callback}");
                    //action?.Invoke();
                }
                else
                {
                    Debug.LogError($"데이터 수정에 실패했습니다. : {callback}");
                }
            });
        }

    }


    public void EquipItem(int index)
    {
        // index는 아이템이 여러개일때 유효함.
        userGameData.equipHead = index;
        UpdateEquipment();
    }

    public void UnequipItem()
    {
        // index는 아이템이 여러개일때 유효함.
        userGameData.equipHead = 0;
        UpdateEquipment();
    }

    public void UpdateEquipment()
    {
        if (userGameData == null)
        {
            Debug.LogError("데이터가 존재하지 않습니다. Initialize 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param()
        {
            { "equipHead", userGameData.equipHead }
        };

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.LogError("유저의 inDate 정보가 없어 게임 정보 데이터 수정에 실패했습니다.");
        }
        else
        {
            Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param, callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log($"데이터 수정에 성공했습니다. : {callback}");
                    //action?.Invoke();
                }
                else
                {
                    Debug.LogError($"데이터 수정에 실패했습니다. : {callback}");
                }
            });
        }

    }
}
