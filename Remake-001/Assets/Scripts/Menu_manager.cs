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
        playModeContent.GetChild(2).GetComponent<Button>().onClick
            .AddListener(delegate { SceneManager.LoadScene(map.scene);PersistentScript.currentMap= map; }); //playButton

    }


    #region fetchSites 
    private void UpdateTexts()
    {
        StartCoroutine(UpdateNewsTab());
        StartCoroutine(UpdateLogsTab());
    }

    IEnumerator UpdateNewsTab()
    {
        UnityWebRequest WWW = UnityWebRequest.Get("https://raw.githubusercontent.com/NatanSomeone/PetEca_2.0/master/infoDataBase/News_pt-BR.txt");
        yield return WWW.SendWebRequest();
        if (!(WWW.isNetworkError || WWW.isHttpError))
           newsText.text = "NOTÍCIAS:\n"+WWW.downloadHandler.text;

    }

    IEnumerator UpdateLogsTab()
    {
        UnityWebRequest WWW = UnityWebRequest.Get("https://github.com/NatanSomeone/PetEca_2.0/raw/master/infoDataBase/Logs_pt-BR");
        yield return WWW.SendWebRequest();
        if (!(WWW.isNetworkError || WWW.isHttpError))
            logs.text = "REGISTROS:\n" + WWW.downloadHandler.text;

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
