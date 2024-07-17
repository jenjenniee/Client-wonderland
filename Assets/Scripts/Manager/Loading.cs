using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

static public class Loading
{
    static public bool isLoading;
    static public bool isError = false;

    static public void OnError()
    {
        isError = true;
        Utils.LoadScene("Lobby");
        isLoading = false;
    }
    static public void SetLoading() { isLoading = true; }
    static public void CompleteLoad() { isLoading = false; }
}