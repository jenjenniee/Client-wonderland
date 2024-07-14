using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class SaveAndOCRScript : MonoBehaviour
{
    public RawImage drawArea;
    private string apiUrl = "https://vdyug68ip2.apigw.ntruss.com/custom/v1/32243/624075526509ac00b89afc3b231e9cd3842118473c734a289851bb577420a7b2/general";
    private string secretKey = ""; // 여기에 secret key를 입력하세요.

    public void SaveImageAndSendOCR()
    {
        secretKey = GetComponent<APIKeyManager>().GetOCRAPIKey();
        StartCoroutine(SaveAndOCRCoroutine());
    }

    IEnumerator SaveAndOCRCoroutine()
    {
        // 이미지 저장
        Texture2D texture = drawArea.texture as Texture2D;
        byte[] bytes = texture.EncodeToPNG();
        string imagePath = Path.Combine(Application.persistentDataPath, "SavedImage.png");
        File.WriteAllBytes(imagePath, bytes);
        Debug.Log("Image saved to: " + imagePath);

        // OCR 요청
        yield return StartCoroutine(SendOCRRequest(imagePath));
    }

    IEnumerator SendOCRRequest(string imagePath)
    {
        string requestId = Guid.NewGuid().ToString();
        long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var requestJson = new JObject
        {
            ["images"] = new JArray
            {
                new JObject
                {
                    ["format"] = "png",
                    ["name"] = "demo"
                }
            },
            ["requestId"] = requestId,
            ["version"] = "V2",
            ["timestamp"] = timestamp
        };

        byte[] payload = System.Text.Encoding.UTF8.GetBytes(requestJson.ToString());
        byte[] fileData = File.ReadAllBytes(imagePath);

        List<IMultipartFormSection> form = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("message", payload),
            new MultipartFormFileSection("file", fileData, "image.png", "image/png")
        };

        UnityWebRequest www = UnityWebRequest.Post(apiUrl, form);
        www.SetRequestHeader("X-OCR-SECRET", secretKey);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            JObject result = JObject.Parse(www.downloadHandler.text);
            string resultJsonPath = Path.Combine(Application.persistentDataPath, "result.json");
            File.WriteAllText(resultJsonPath, result.ToString());

            string text = "";
            foreach (var field in result["images"][0]["fields"])
            {
                text += field["inferText"].ToString() + " ";
            }

            Debug.Log("Extracted Text: " + text);

            if (text.Length > 0)
            {
                text = text.ToLower();
                if (text[text.Length - 1] == ' ')
                {
                    text = text.Substring(0, text.Length - 1);
                }
            }

            GameObject.Find("ProblemController").GetComponent<ProblemController>().OnSubmitOCR(text);
            StartCoroutine(ClearAfterSubmit());
        }
    }
    IEnumerator ClearAfterSubmit()
    {
        yield return new WaitForSecondsRealtime(3f);
        GameObject.Find("OCRPanel").GetComponent<DrawScript>().InitializeTexture();
    }
}
