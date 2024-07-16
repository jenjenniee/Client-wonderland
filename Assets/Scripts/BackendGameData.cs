using UnityEngine;
using BackEnd;
using TMPro;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
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

    public static List<ItemData> itemList = new List<ItemData>();

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
        };
        foreach (var item in userGameData.hasItem)
        {
            param.Add(item.Key, item.Value);
        }

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
                //Debug.Log($"���� ���� �ҷ����� ����: {callback}");
                //Debug.Log("랄랼랄");

                // JSON �Ľ�
                try
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)
                    {
                        //Debug.Log("�����Ͱ� �������� �ʽ��ϴ�.");
                    }
                    else
                    {
                        gameDataRowInDate = gameDataJson[0]["inDate"].ToString();
                        userGameData.heart = int.Parse(gameDataJson[0]["heart"].ToString());
                        userGameData.equipHead = (gameDataJson[0]["equipHead"].ToString());
                        //userGameData.hasItem = bool.Parse(gameDataJson[0]["hasItem"].ToString());

                        foreach (var key in gameDataJson[0].Keys)
                        {
                            Debug.Log($"Key: {key}, Value: {gameDataJson[0][key]}");
                            if (key.StartsWith("i")) {
                                userGameData.hasItem[key] = gameDataJson[0][key].ToString() == "True";
                            }
                        }
                        LoadChartData();
                        onGameDataLoadEvent?.Invoke();
                        Utils.LoadScene(SceneNames.Loby);
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

    public bool BuyItem(string itemId, int price)
    {
        // index는 아이템이 여러개일때 유효함.
        if (userGameData.hasItem.ContainsKey(itemId))
        {
            if (userGameData.heart >= price)
            {
                DecreaseHeart(price);
                userGameData.hasItem[itemId] = true;
                UpdateItem();
                return true;
            }
        }
        
        // 구매 실패 : 재화 부족
        return false;
    }

    public void UpdateItem()
    {
        Debug.Log("아오아아아아아;");
        if (userGameData == null)
        {
            Debug.LogError("데이터가 존재하지 않습니다. Initialize 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param();
        foreach (var item in userGameData.hasItem)
        {
            param.Add(item.Key, item.Value);
        }

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


    public void EquipItem(string itemId)
    {
        if (userGameData.hasItem.ContainsKey(itemId))
        {
            userGameData.equipHead = itemId;
        }
        // index는 아이템이 여러개일때 유효함.
        UpdateEquipment();
    }

    public void UnequipItem()
    {
        userGameData.equipHead = "i001";
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
    // 차트 데이터 불러오기
    public void LoadChartData()
    {
        Backend.Chart.GetChartContents("131331", callback => {
            if (callback.IsSuccess())
            {
                try
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    if (gameDataJson.Count <= 0)
                    {
                        //Debug.Log("�����Ͱ� �������� �ʽ��ϴ�.");
                    }
                    else
                    {
                        for (int i=0;i<gameDataJson.Count;i++)
                        {
                            ItemData item = new ItemData();
                            item.itemID = gameDataJson[i]["itemID"].ToString();
                            item.itemName = gameDataJson[i]["itemName"].ToString();
                            item.itemPrice = int.Parse(gameDataJson[i]["itemPrice"].ToString());
                            itemList.Add(item);

                            //Debug.Log($"{item.itemID}: {userGameData.hasItem[item.itemID]}");
                            // 사용자의 hasItem 초기화
                            if (!userGameData.hasItem.ContainsKey(item.itemID))
                            {
                                userGameData.hasItem[item.itemID] = false;
                                Debug.Log($"{item.itemID}: {userGameData.hasItem[item.itemID]}");
                            }

                        }
                        Debug.Log("차트 데이터 불러오기 성공");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogError("차트 데이터 불러오기 실패: " + callback.GetMessage());
            }
        });
    }
}


[System.Serializable]
public class ItemData
{
    public string itemID;
    public string itemName;
    public int itemPrice;
}