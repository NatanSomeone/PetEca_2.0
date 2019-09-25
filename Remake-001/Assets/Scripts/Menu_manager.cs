using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            o.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Nome: "+map.name;
            o.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = map.description;
            o.GetComponent<Toggle>().group = mapsContent.GetComponent<ToggleGroup>();
            o.GetComponent<Toggle>().onValueChanged.AddListener((v)=> { if (v) MapSelected(map); });
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
            .AddListener(delegate {
                loading.SetActive(true); PersistentScript.ClickSfx(); PersistentScript.currentMap= map;
                PersistentScript.playType = (score.isOn) ? 0 : 1; SceneManager.LoadScene(map.scene);
                string gameTypeDesc = (PersistentScript.playType==0)?
                $"\tSua pontuação é dada na quantia de peças que coletar em {map.maxTime}s":
                "\tSua pontuação é dada pelo menor tempo que conseguir coletar todas as pecas";        
                PersistentScript.incomingMessage = map.longDescription + "\n" + gameTypeDesc;}); 


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
           newsText.text = "NOTÍCIAS:\n"+WWW.downloadHandler.text+"\n\n\n";

    }

    IEnumerator UpdateLogsTab()
    {
        UnityWebRequest WWW = UnityWebRequest.Get("https://github.com/NatanSomeone/PetEca_2.0/raw/master/infoDataBase/Logs_pt-BR");
        yield return WWW.SendWebRequest();
        if (!(WWW.isNetworkError || WWW.isHttpError))
            logs.text = "REGISTROS:\n" + WWW.downloadHandler.text+"\n\n\n";

    }
    #endregion

    public enum Menu
    {
        Start,
        Training,
        Maps,
        Updates
    }

}
//PODE SER GENERALIZADO
