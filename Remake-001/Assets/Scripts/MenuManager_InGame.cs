using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MenuManager_InGame : MonoBehaviour
{
    public Transform FileMenu;
    public Transform TaskQueueList;
    private Transform FileMenuContent;
    public Transform MessageBox;
    public TextMeshProUGUI MessageBoxText;
    public Button MessageBoxOkButton;

    [HideInInspector] public static int tasksToDisp;
    //public static Transform TaskListGlobal;
    public static MenuManager_InGame instance;
    //Variaveis inerente ao mapa;
    public static float score;
    public static float timePassed;
    public static bool isPlaying = false;//check if the game is not paused


    private void Awake()
    {

        instance = this;
        ExtLibControl.OnCommandCalled += UserActionControl;
        tasksToDisp = TaskQueueList.childCount - 1;



        TaskQueueList.parent.GetComponent<Toggle>().onValueChanged
            .AddListener(v => { TaskQueueList.gameObject.SetActive(v); if (v) UpdateTaskQueueList(); });

        FileMenuContent = FileMenu.GetChild(0).GetChild(0);

        FileMenu.GetComponentInChildren<TextMeshProUGUI>().text = PersistentScript.currentMap?.name + "-Menu";
        FileMenu.GetComponent<Toggle>().onValueChanged                      //FileMenuButton
            .AddListener(v => { FileMenuContent.gameObject.SetActive(v); });

        FileMenuContent.GetChild(0).GetComponent<Toggle>().onValueChanged   //PauseButton
            .AddListener(v => { PauseLevel(v); });

        FileMenuContent.GetChild(1).GetComponent<Toggle>().onValueChanged   //RestartButton
            .AddListener(v => { ReloadLevel(); });

        FileMenuContent.GetChild(2).GetComponent<Toggle>().onValueChanged   //NewFileButton
            .AddListener(v => { NewFile(); });

        FileMenuContent.GetChild(3).GetComponent<Toggle>().onValueChanged   //MainMenuButton
            .AddListener(delegate { SceneManager.LoadScene(0); PersistentScript.currentMap = null; });

        //MessageBox
        MessageBoxOkButton.onClick.AddListener(delegate { MessageBox.gameObject.SetActive(false); PauseLevel(false); });

        
        
    }
    private void Start()
    {
        isPlaying = true;
        PersistentScript.canRunUserActions = true;

        //starMessage
        //mudar para PersistentScript.currentMap.detailedDescription
        if (PersistentScript.incomingMessage == "Welcome")
        {
            ShowInfoMessage(@"Bem vindo ao primeiro mapa, aqui sua aventura começar primeiramente vá {Map01-Menu/ NovoArquivoCpp} escreva seu programa e copile.
	Ele deve ser capaz de direcionar o Robo ao quadro espaço amarelo, coletar seus itens e depositar no espaço vermelho, voltando ao espaço inicial para reposição de itens, o Jogo termina com três itens;
    Para pegar um item , <b>levante a gaiola</b> aproxime-se dele e <b>abaixe a gaiola</b> em cima. Já para largar-lo , basta erguer a gaiola");
        }
        else if (PersistentScript.incomingMessage != null)
        {
            ShowInfoMessage(PersistentScript.incomingMessage);
        }
    }

    public static void PauseLevel(bool v)//TODO verificar...
    {
        ExtLibControl.ClearActionQueue();
        ExtLibControl.currentUAction = null;

        isPlaying = !v;
        instance.FileMenu.parent.GetChild(1).gameObject.SetActive(v);
        Time.timeScale = v ? 0 : 1;
    }

    public static void ShowInfoMessage(string message)
    {
        instance.MessageBoxText.text = message;
        instance.MessageBox.gameObject.SetActive(true);
        PauseLevel(true);
        PersistentScript.incomingMessage = null;

    }

    private static void NewFile()
    {

        var path = Application.dataPath + "/User~/PetECA-Programs";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!File.Exists(path + "/petecavirtual.h"))
            File.Copy(Application.dataPath + "/Resources/Libraries~/petecavirtual.h", path + "/petecavirtual.h", false);

        string fileName = PersistentScript.currentMap?.name + "-file";

        if (!File.Exists($"{path}/{fileName}.cpp"))
        {
            File.Copy(Application.dataPath + "/Resources/Libraries~/MasterExample.cpp", $"{path}/{fileName}.cpp", false);
        }else
        {
            ShowInfoMessage($"O arquivo \"{fileName}.cpp\" já havia sido criado," +
                $" e está sendo aberto, assim como a pasta em que está contido." +
                $"\nVoce pode criar outro arquivo se quiser.");
            System.Diagnostics.Process.Start($"{path}");
        }

        System.Diagnostics.Process.Start($"{path}/{fileName}.cpp");
    }

    public static void ReloadLevel()
    {
        PersistentScript.ExecuteOnMainThread.Enqueue(delegate
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            ExtLibControl.ClearActionQueue();
        });
    }

    private void UserActionControl(object sender, ExtLibControl.UserAction a)
    {

        if (a.type == "pause")
        {
            PersistentScript.ExecuteOnMainThread.Enqueue(() => PauseLevel(true));
            //ExtLibControl.PServer2.SendMessage($"Pausing", ExtLibControl.PServer2.clientse);
            Debug.Log("<color=#006666>Pausing....</color>");
            ExtLibControl.DeQueueAction();
        }
        else if (a.type == "getTIME")
        {
            ExtLibControl.PServer2.SendMessage($"{timePassed}", ExtLibControl.PServer2.clientse);
            Debug.Log($"<color=#00ff00>Time goted: time ={timePassed} seconds...</color>");
            ExtLibControl.DeQueueAction();
        }

    }

    public static void UpdateTaskQueueList()
    {

        PersistentScript.ExecuteOnMainThread.Enqueue(
        delegate
        {
            if (instance.TaskQueueList.gameObject.activeSelf)
            {
                var lastNActions = ExtLibControl.userActions.Take(Mathf.Min(tasksToDisp, ExtLibControl.userActions.Count)).ToArray(); ;
                var len = lastNActions.Length;
                for (int i = 0; i < tasksToDisp; i++)
                {

                    instance.TaskQueueList.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = (i < len) ?
                        $"{lastNActions?[i].type} - ({lastNActions?[i].value})"
                        : "---";

                }
                var QLen = ExtLibControl.userActions.Count;
                instance.TaskQueueList.GetChild(tasksToDisp).GetComponent<TextMeshProUGUI>().text =
                    (QLen > tasksToDisp) ? $"(•••+{QLen - tasksToDisp})" : "OK";
            }

        }

        );

    }

    public IEnumerator WaitAndDo(float time, Action action)
    {
        Debug.Log($"<color=#000066>Waiting {time} seconds...</color>");
        yield return new WaitForSecondsRealtime(time);
        action();
    }

}
