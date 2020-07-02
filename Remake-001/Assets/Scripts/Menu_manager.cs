using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Menu_manager : MonoBehaviour
{
    [Header(" ")]
    public Transform tabsContent;
    public GameObject loading;
    [Header("Start")]
    //news, docs, site
    public TextMeshProUGUI newsText;
    public TextMeshProUGUI docsText;
    public TextMeshProUGUI siteText;
    [Header("\tUser")]
    [SerializeField] public User user;


    [Header("Training")]
    public Transform mapsContent;
    public Transform playModeContent;
    public GameObject MapTemplate;
    public List<MapDisplayInfo> mapList;
    //maps
    //opts
    [Header("Maps")]
    //maps
    //opts
    [Header("Updates")]
    public TextMeshProUGUI logs;
    public TextMeshProUGUI check;
    public TextMeshProUGUI credit;

    public Transform infoBox;

    private Menu selectedScreen = Menu.Start;
    public Menu SelectedScreen {
        get => selectedScreen; set
        {
            selectedScreen = value;
            PersistentScript.ClickSfx();
            for (int i = 0; i < tabsContent.childCount; i++)
            {
                bool tabSelected = i == (int)selectedScreen;
                tabsContent.GetChild(i).gameObject.SetActive(tabSelected);
                tabsContent.parent.GetChild(1).GetChild(i).GetChild(1).gameObject.SetActive(tabSelected);
            }
        }
    }



    public void SelectScreen(int s) => SelectedScreen = (Menu)s;

    public void Exit_game()
    {
        PersistentScript.ClickSfx();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif   
    }


    private void Awake()
    {
        UpdateTexts();
        UpdateMapList();
        Highscores.OnHighscoresDownloaded += Highscores_OnHighscoresDownloaded;
        user = new User(user.signIn, user.signOut, user.name,user.profileIcon);
    }

    private void Highscores_OnHighscoresDownloaded(object sender, Highscore[] highscoreList)
    {
        if (highscoreList == null)
        {
            siteText.text = "PLACAR:\n --" + "Sem conexão, entre na rede para ver o placar ";
        }
        else
        {
            siteText.text = "PLACAR:\ndd/mm/aaaa ->pontuação: \"nome\" \n --" + string.Join("\n --", highscoreList) + "\n\n\n";
        }
    }

    private void UpdateMapList()
    {
        foreach (var map in mapList)
        {
            var o = Instantiate(MapTemplate, mapsContent);
            o.transform.GetChild(1).GetComponent<Image>().sprite = map.icon;
            o.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Nome: " + map.name;
            o.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = map.description;
            o.GetComponent<Toggle>().group = mapsContent.GetComponent<ToggleGroup>();
            o.GetComponent<Toggle>().onValueChanged.AddListener((v) => { if (v) MapSelected(map); });
        }
    }

    private void MapSelected(MapDisplayInfo map)
    {
        //mostra que mapa foi selecionado na tela do lado 
        //assim como as opções de tipo de jogo
        //e o botão para entrar no mapa
        //SceneManager.LoadScene(map.scene);

        playModeContent.GetChild(0).GetComponent<Image>().sprite = map.icon;//icon
        playModeContent.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = map.name;//Name
        var score = playModeContent.GetChild(3).GetChild(0).GetComponentInChildren<Toggle>();
        playModeContent.GetChild(2).GetComponent<Button>().onClick  //playButton
            .AddListener(delegate
            {
                loading.SetActive(true); PersistentScript.ClickSfx(); PersistentScript.currentMap = map;
                PersistentScript.playType = (score.isOn) ? 0 : 1; SceneManager.LoadScene(map.scene);
                string gameTypeDesc = (PersistentScript.playType == 0) ?
                $"\tSua pontuação é dada pela quantidade de peças coletadas em {map.maxTime}s" :
                "\tSua pontuação é inversamente proporcional ao tempo de coleta de todas as peças";
                PersistentScript.incomingMessage = map.longDescription + "\n" + gameTypeDesc;
            });


    }


    #region fetchSites 
    private void UpdateTexts()
    {
        StartCoroutine(UpdateNewsTab());
        StartCoroutine(UpdateLogsTab());
        Highscores.instance.DownloadHighscores();


    }

    IEnumerator UpdateNewsTab()
    {
        UnityWebRequest WWW = UnityWebRequest.Get("https://raw.githubusercontent.com/NatanSomeone/PetEca_2.0/master/infoDataBase/News_pt-BR.txt");
        yield return WWW.SendWebRequest();
        if (!(WWW.isNetworkError || WWW.isHttpError))
            newsText.text = "NOTÍCIAS:\n" + WWW.downloadHandler.text + "\n\n\n";

    }

    IEnumerator UpdateLogsTab()
    {
        UnityWebRequest WWW = UnityWebRequest.Get("https://github.com/NatanSomeone/PetEca_2.0/raw/master/infoDataBase/Logs_pt-BR");
        yield return WWW.SendWebRequest();
        if (!(WWW.isNetworkError || WWW.isHttpError))
            logs.text = "REGISTROS:\n" + WWW.downloadHandler.text + "\n\n\n";

    }
    #endregion

    public enum Menu
    {
        Start,
        Training,
        Maps,
        Updates
    }
    [Serializable]
    public struct User
    {
        public Button signIn;
        public Button signOut;
        public TextMeshProUGUI name;
        public RawImage profileIcon;
        private static string userSecret;
        public User(Button signIn, Button signOut, TextMeshProUGUI name, RawImage profileIcon)
        {
            this.signOut = signOut; this.signIn = signIn; this.name = name; this.profileIcon = profileIcon;
            this.signIn.onClick.AddListener(OnSignIN);
            this.signOut.onClick.AddListener(OnSignOUT);

            if (PersistentScript.usr != null)
            {
                signIn.interactable = false; signIn.gameObject.SetActive(false);
                name.transform.parent.gameObject.SetActive(true);
                name.text = PersistentScript.usr.name; profileIcon.texture = PersistentScript.usr.tex;
            }
        }

        private void OnSignOUT()
        {
            string url = $"https://petecavirtual.firebaseapp.com/auth?user={userSecret},out";
            Application.OpenURL(url);
            PersistentScript.usr = null;
            name.transform.parent.gameObject.SetActive(false);
            signIn.interactable = true; signIn.gameObject.SetActive(true);
        }

        private void OnSignIN()
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new System.Random();
                var data = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
                md5.Initialize(); md5.ComputeHash(Convert.FromBase64String(data));
                userSecret = new string(Convert.ToBase64String(md5.Hash).Select(c => ";/?:@&=+$,".Contains(c) ? chars[random.Next(chars.Length)] : c).ToArray());
                PersistentScript.userSecretValue = userSecret;
            }
            string url = $"https://petecavirtual.firebaseapp.com/auth?user={userSecret}";
            Application.OpenURL(url);
            PersistentScript.instance.StartCoroutine(RequestUser(url));
        }

        public IEnumerator RequestUser(string uri)//TODO arrumar isso
        {
            signIn.interactable = false;
            var attempts = -12;
            yield return new WaitForSecondsRealtime(5);
            while (++attempts < 0)
            {
                using (UnityWebRequest req = UnityWebRequest.Get(uri))
                {
                    req.SetRequestHeader("Accept", "application/json");
                    req.timeout = 2;
                    yield return req.SendWebRequest();
                    if (!(req.isNetworkError || req.isHttpError))
                    {
                        if (req.downloadHandler.text.Length > 1)
                        {

                            signIn.gameObject.SetActive(false);
                            name.transform.parent.gameObject.SetActive(true);
                            PersistentScript.usr = JsonUtility.FromJson<UserObject>(req.downloadHandler.text);
                            Debug.Log($"{JsonUtility.ToJson(PersistentScript.usr)}");
                            name.text = PersistentScript.usr.name;


                            var texReq = UnityWebRequestTexture.GetTexture(PersistentScript.usr.icon);
                            yield return texReq.SendWebRequest();
                            if (!(texReq.isNetworkError || texReq.isHttpError))
                            { profileIcon.texture = ((DownloadHandlerTexture)texReq.downloadHandler).texture; PersistentScript.usr.tex = profileIcon.texture; }
                            break;
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(3);
            }
            signIn.interactable = attempts < 0;
        }
        [Serializable] public class UserObject { public string name, givenName, familyName, icon, email; public Texture tex; }
    }


}
//PODE SER GENERALIZADO
