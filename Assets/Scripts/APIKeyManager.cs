using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIKeyManager : MonoBehaviour
{
    private string[] apiKey;

    void Start()
    {
        TextAsset apiKeyFile = Resources.Load<TextAsset>("apikey");
        if (apiKeyFile != null)
        {
            apiKey = apiKeyFile.text.Split(',');
        }
        else
        {
            Debug.LogError("API key file not found.");
        }
    }

    public string GetOCRAPIKey()
    {
        return apiKey[1];
    }
    public string GetTTSAPIKey()
    {
        return apiKey[0];
    }
}
