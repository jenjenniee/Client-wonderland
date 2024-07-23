using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

static public class Loading
{
    static public bool isLoading;
    static public int mapLoading;
    static public bool isError = false;
    static public int sceneLoadedCount = 0;
    static public void OnError()
    {
        isError = true;
        Utils.LoadScene("Loby");
        isLoading = false;
        mapLoading = 0;
    }
    static public void SetLoading() { isLoading = true; }
    static public void CompleteLoad() { isLoading = false; sceneLoadedCount = 0; mapLoading = 3; }
}