using System.Collections.Generic;

[System.Serializable]
public class UserGameData
{
    public int heart;       // 재화
    public string equipHead;   // 착용중인 기념품
    public Dictionary<string, bool> hasItem = new Dictionary<string, bool>();

    public void Reset()
    {
        heart = 0;
        equipHead = "i001";      // 아무것도 착용 안 한 상태
        hasItem.Clear();
        hasItem["i001"] = true;
    }
}