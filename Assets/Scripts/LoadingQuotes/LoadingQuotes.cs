using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 
using UnityEngine.SceneManagement; 
using TMPro;

public class LoadingQuotes : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI displayText;
    [SerializeField]
    private TYPE type = TYPE.ANY;
    public enum TYPE 
    {
        ANY = 0,
        WEATHER,
        TEMPERATURE,
        NUMBERS,
        JOKE,
        INSPIRATION,

        Count,
    };

    private Dictionary<TYPE, string> uris;

    private void Awake()
    {
        uris = new Dictionary<TYPE, string>();
        uris.Add(TYPE.WEATHER, "https://api.openweathermap.org/data/2.5/weather?id=1880252&units=metric&appid=1da8afae9dcdd376fed14d72a3513b5b");
        uris.Add(TYPE.TEMPERATURE, "https://api.openweathermap.org/data/2.5/weather?id=1880252&units=metric&appid=1da8afae9dcdd376fed14d72a3513b5b");
        uris.Add(TYPE.NUMBERS, "http://numbersapi.com/random");
        uris.Add(TYPE.JOKE, "https://us-central1-dadsofunny.cloudfunctions.net/DadJokes/random/jokes");
        uris.Add(TYPE.INSPIRATION, "https://api.quotable.io/random");
    }

    private void Start()
    {
        displayText.text = "null";
        GetQuote(type);
    }
    public void GetQuote(TYPE type = TYPE.ANY)
    {
        if (type == TYPE.ANY)
            type = (TYPE)Random.Range((int)TYPE.ANY + 1, (int)TYPE.Count);

        Debug.Log("Getting quote...");

        if (uris.ContainsKey(type))
            StartCoroutine(GetRequest(uris[type], type));
        else
            Debug.LogError("Missing uri");
    }

    private string FormatResult(string result, TYPE type)
    {
        string ret = result;
        switch (type)
        {
            default:
            case TYPE.NUMBERS:                
                break;
            case TYPE.WEATHER:
                OpenWeatherJson w = JsonUtility.FromJson<OpenWeatherJson>(result);
                ret = w.GetWeather();
                break;
            case TYPE.TEMPERATURE:
                OpenWeatherJson t = JsonUtility.FromJson<OpenWeatherJson>(result);
                ret = t.GetTemp();
                break;
            case TYPE.JOKE:
                DadJoke d = JsonUtility.FromJson<DadJoke>(result);
                ret = d.GetJoke();
                break;
            case TYPE.INSPIRATION:
                FamousQuotes f = JsonUtility.FromJson<FamousQuotes>(result);
                ret = f.GetQuote();
                break;
        }
        return ret;
    }

    private IEnumerator GetRequest(string uri, TYPE type)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError)
            Debug.LogErrorFormat("Error getting from {0}, {1}", uri, uwr.error);
        else if (uwr.isDone)
        { 
            displayText.text = FormatResult(uwr.downloadHandler.text, type);
            Debug.LogFormat("Got quote : {0}", displayText.text);
        }
    }


    #region Json Parsing and Result Formatting
    
    #region Weather 
    [System.Serializable]
    private struct OpenWeatherJson
    {
        [System.NonSerialized]
        public string text;

        public string GetWeather()
        { 
            return string.Format("You should look out your window, I see {0}.", weather[0].description);

        }
        public string GetTemp() 
        {
            return string.Format("Current temperatures are {0}, but it feels like {1}.", main.temp, main.feels_like); 
        }

        public Temp main;
        public Weather[] weather;
    }
    [System.Serializable]
    private struct Weather
    {
        //public int id;
        //public string main;
        public string description;
        //public string icon;
    }
    [System.Serializable]
    private struct Temp
    {
        public float temp;
        public float feels_like;
        //public float temp_min;
        //public float temp_max;
        //public int pressure;
        //public int humidity;
    }
    #endregion

    #region Jokes 
    [System.Serializable]
    private struct DadJoke
    {
        public string GetJoke()
        {
            return setup + "\n" + punchline;
        }

        //public int id;
        //public string type;
        public string setup;
        public string punchline;
    }
    #endregion

    #region Inspiration 
    [System.Serializable]
    private struct FamousQuotes
    {
        public string GetQuote()
        {
            return content + "\n-" + author;
        }

        //public string id;
        public string content;
        public string author;
        //public string[] tags;
        //public int length;
    }
    #endregion

    #endregion
}
