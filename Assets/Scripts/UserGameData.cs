[System.Serializable]
public class UserGameData
{
    public int heart;       // 재화
    public int equipHead;   // 착용중인 기념품

    public void Reset()
    {
        heart = 0;
        equipHead = 0;      // 아무것도 착용 안 한 상태
    }
}
