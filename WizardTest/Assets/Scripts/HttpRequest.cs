using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpRequest : MonoBehaviour
{

    private const string STORAGE_URL = "http://localhost:50411/";

    public void Request(string endpoint, System.Func<WWW, IEnumerator> OnResponse)
    {
        WWW request = new WWW(STORAGE_URL + endpoint);
        StartCoroutine(OnResponse(request));
    }

    public void Request(string endpoint, string body, System.Func<WWW, IEnumerator> OnResponse)
    {
        body = WWW.EscapeURL(body);
        WWW request = new WWW(STORAGE_URL + endpoint + "?" + body);
        StartCoroutine(OnResponse(request));
    }
}
