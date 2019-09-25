using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class Highscores : MonoBehaviour
{

    const string privateCode = "03H6tXFud0eql1UUQsg0TQrBkLiuBCL0KpKBzbgc4zlg";
    const string publicCode = "5d84be6bd1041303ecbfcb2b";
    const string webURL = "http://dreamlo.com/lb/";

    //DisplayHighscores highscoreDisplay;
    public Highscore[] highscoresList;
    public static Highscores instance;

    public static event EventHandler<Highscore[]> OnHighscoresDownloaded;

    void Awake()
    {
        //highscoreDisplay = GetComponent<DisplayHighscores>();
        instance = this;
    }

    public static void AddNewHighscore(string username, int score)
    {
        instance.StartCoroutine(instance.UploadNewHighscore(username, score));
    }

    IEnumerator UploadNewHighscore(string username, int score)
    {
        UnityWebRequest WWW = UnityWebRequest.Get($"{webURL}{privateCode}/add/{UnityWebRequest.EscapeURL(username)}/{score}");
        yield return WWW.SendWebRequest();

        if (!(WWW.isNetworkError || WWW.isHttpError))
        {
            //print("Upload Successful");
            //DownloadHighscores();
        }
        else
        {
            Debug.LogWarning("Error uploading: " + WWW.error);
        }
    }

    public void DownloadHighscores()
    {
        StartCoroutine("DownloadHighscoresFromDatabase");
    }

    IEnumerator DownloadHighscoresFromDatabase()
    {
        UnityWebRequest WWW = UnityWebRequest.Get($"{webURL}{publicCode}/pipe/100");
        
        yield return WWW.SendWebRequest();

         if (!(WWW.isNetworkError || WWW.isHttpError))
        {
            FormatHighscores(WWW.downloadHandler.text);
            OnHighscoresDownloaded?.Invoke(null, highscoresList);
        }
        else
        {
            Debug.LogWarning("Error Downloading: " + WWW.error);
            OnHighscoresDownloaded?.Invoke(null, null);
        }
    }

    void FormatHighscores(string textStream)
    {
        System.Globalization.CultureInfo invariantCulture = System.Globalization.CultureInfo.InvariantCulture;
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0].Split("§§".ToCharArray())[0].Replace('+',' ');
            int score = int.Parse(entryInfo[1], invariantCulture);
            DateTime time = DateTime.Parse(entryInfo[4], invariantCulture);
            highscoresList[i] = new Highscore(username, score,time);
            //print(highscoresList[i].username + ": " + highscoresList[i].score);
        }
    }

}

public struct Highscore
{
    public string username;
    public int score;
    DateTime time;

    public Highscore(string _username, int _score,DateTime _time)
    {
        username = _username;
        score = _score;
        time = _time;
    }

    public override string ToString()
    {
        return $"{time,11:dd/MM/yyyy}->{score,12}:  {username}\t";
    }
}