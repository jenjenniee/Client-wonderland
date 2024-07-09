using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameMoney
{
    public int heart = 10;

    // 데이터를 디버깅하기 위한 함수입니다. (Debug.Log(GameMoney);)
    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"heart : {heart}");
        return result.ToString();
    }
}
