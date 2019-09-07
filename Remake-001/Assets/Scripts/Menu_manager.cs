using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    //maps
    //opts
    [Header("Maps")]
    //maps
    //opts
    [Header("Updates")]
    //logs
    //check
    //credit




    private Menu selectedScreen;
    public Menu SelectedScreen {
        get => selectedScreen; set
        {
            selectedScreen = value;
            
            for (int i = 0; i < tabsContent.childCount; i++)
            {
                tabsContent.GetChild(i).gameObject.SetActive(i == (int)selectedScreen);
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

    public enum Menu
    {
        Start,
        Training,
        Maps,
        Updates
    }

}
//PODE SER GENERALIZADO
