using System.Collections.Generic;

[System.Serializable]
public class UserGameData
{
    public int heart;       // ��ȭ
    public string equipHead;   // �������� ���ǰ
    public Dictionary<string, bool> hasItem = new Dictionary<string, bool>();

    public void Reset()
    {
        heart = 0;
        equipHead = "i001";      // �ƹ��͵� ���� �� �� ����
        hasItem.Clear();
        hasItem["i001"] = true;
    }
}