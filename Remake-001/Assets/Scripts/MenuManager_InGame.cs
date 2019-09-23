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
    [Space(6)]//MessageBox
    public Transform MessageBox;
    public TextMeshProUGUI MessageBoxText;
    public Button MessageBoxOkButton;
    [Space(6)]//GameOverBox
    public Transform GameOverBox;
    public TextMeshProUGUI GameOverBoxText;
    public TMP_InputField GameOverBoxInput;
    public Button GameOverBoxSendButton;
    [Space(6)]
    public TextMeshProUGUI TimeTextBox;
    public TextMeshProUGUI ScoreTextBox;
    public AudioClip[] clips;

    [HideInInspector] public static int tasksToDisp;
    //public static Transform TaskListGlobal;
    public static MenuManager_InGame instance;
    //Variaveis inerente ao mapa;
    //public static float score;
    public static float timePassed => Time.time - PersistentScript.timeT0;
    public static bool isPlaying = false;//check if the game is not paused
    public static int totalItens;
    private AudioSource aSource;

    private void Awake()
    {

        instance = this;
        aSource = this.GetComponent<AudioSource>();

        ExtLibControl.OnCommandCalled += UserActionControl;
        tasksToDisp = TaskQueueList.childCount - 1;



        TaskQueueList.parent.GetComponent<Toggle>().onValueChanged
            .AddListener(v => { PersistentScript.ClickSfx(); TaskQueueList.gameObject.SetActive(v); if (v) UpdateTaskQueueList(); });    //tasklist

        FileMenuContent = FileMenu.GetChild(0).GetChild(0);

        FileMenu.GetComponentInChildren<TextMeshProUGUI>().text = PersistentScript.currentMap?.name + "-Menu";
        FileMenu.GetComponent<Toggle>().onValueChanged                      //FileMenuButton
            .AddListener(v => { PersistentScript.ClickSfx(); FileMenuContent.gameObject.SetActive(v); });

        FileMenuContent.GetChild(0).GetComponent<Toggle>().onValueChanged   //PauseButton
            .AddListener(v => { PersistentScript.ClickSfx(); PauseLevel(v); });

        FileMenuContent.GetChild(1).GetComponent<Toggle>().onValueChanged   //RestartButton
            .AddListener(v => { PersistentScript.ClickSfx(); ReloadLevel(); });

        FileMenuContent.GetChild(2).GetComponent<Toggle>().onValueChanged   //NewFileButton
            .AddListener(v => { PersistentScript.ClickSfx(); NewFile(); });

        FileMenuContent.GetChild(3).GetComponent<Toggle>().onValueChanged   //MainMenuButton
            .AddListener(delegate { PersistentScript.ClickSfx(); SceneManager.LoadScene(0); PersistentScript.currentMap = null; });

        //MessageBox
        MessageBoxOkButton.onClick.AddListener(delegate { PersistentScript.ClickSfx();
            MessageBox.gameObject.SetActive(false); PauseLevel(false);PersistentScript.incomingMessage = null; });

        //GameoverBox
        GameOverBoxSendButton.onClick.AddListener(delegate
        {
            PersistentScript.ClickSfx(); GameOverBox.gameObject.SetActive(false);
            var name = instance.GameOverBoxInput.text;
            if (!string.IsNullOrEmpty(name))
            { name = name.Replace('*', ' ').Replace('|', ' '); Highscores.AddNewHighscore(name + "§§" + DateTime.Now.ToOADate(), (int)PersistentScript.lastScore); }
            ReloadLevel();/*send score*/
        });



    }
    private void Start()
    {
        //isPlaying = true;
        PersistentScript.canRunUserActions = true;
        PersistentScript.currentScore = (PersistentScript.playType == 0) ? 0 : totalItens;
        PersistentScript.timeT0 = (PersistentScript.playType == 0) ? PersistentScript.currentMap.maxTime+.4f : 0;
        var audioS = GetComponent<AudioSource>();


        Instantiate(PersistentScript.currentMap.MapPrefab);
        PersistentScript.persistentScript.ItemCollection = GameObject.Find("ItemCollection")?.transform;
        PersistentScript.persistentScript.cameraHolder = GameObject.Find("Camera-holder").transform;

        if (clips.Length > 0)
        {
            audioS.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            audioS.Play();
        }
        if (ExtLibControl.currentUAction?.userAction.type == "restart")
            ExtLibControl.DeQueueAction();
        else
            ExtLibControl.ClearActionQueue();


        //starMessage
        //mudar para PersistentScript.currentMap.detailedDescription
        if (PersistentScript.incomingMessage == "Welcome")
        {
            ShowInfoMessage(PersistentScript.currentMap.longDescription);
 //           ShowInfoMessage(@"Bem vindo ao primeiro mapa, aqui sua aventura começar primeiramente vá {Map01-Menu/ NovoArquivoCpp} escreva seu programa e copile.
	//Ele deve ser capaz de direcionar o Robo ao quadro espaço amarelo, coletar seus itens e depositar no espaço vermelho, voltando ao espaço inicial para reposição de itens, o Jogo termina com três itens;
 //   Para pegar um item , <b>levante a gaiola</b> aproxime-se dele e <b>abaixe a gaiola</b> em cima. Já para largar-lo , basta erguer a gaiola");
        }
        else if (PersistentScript.incomingMessage != null)
        {
            ShowInfoMessage(PersistentScript.incomingMessage);
        }
        else if (PersistentScript.incomingMessage == null)
        {
            PauseLevel(false);
        }

        ScoreTextBox.transform.parent.GetComponent<TextMeshProUGUI>().text = (PersistentScript.playType == 0) ? "Pontuação:" : "Restantes:";

    }

    public static void PauseLevel(bool v)//TODO verificar...
    {
        ExtLibControl.ClearActionQueue();
        ExtLibControl.currentUAction = null;

        isPlaying = !v;
        instance.FileMenu.parent.GetChild(1).gameObject.SetActive(v);
        Time.timeScale = v ? 0 : PersistentScript.persistentScript.timescale;
        instance.aSource.pitch = v ? 0.5f : 1f;
    }

    public static void ShowInfoMessage(string message, bool restart = false)
    {
        if (restart)
        {
            PersistentScript.incomingMessage = message;
            ReloadLevel();PauseLevel(true);; return;
        }
        instance.MessageBoxText.text = message;
        instance.MessageBox.gameObject.SetActive(true);
        PauseLevel(true);


    }
    public static void ShowGOverMessage(int score)
    {
        if (score <= 0)
            ShowInfoMessage("Fim de jogo, você não obteve nenhuma pontuação", true);
        else
        {
            instance.GameOverBoxText.text = $"Fim de jogo!\n Sua pontuação é de {score} ponto{((score == 1) ? "" : "s")}.";
            PersistentScript.lastScore = score;
            instance.GameOverBox.gameObject.SetActive(true);
            PauseLevel(true);
        }
        ExtLibControl.ClearActionQueue();
    }

    private static void NewFile()
    {

        var path = Application.dataPath + "/User~/PetECA-Programs";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        if (!File.Exists(path + "/petecavirtual.h"))
            File.Copy(Application.dataPath + "/Resources/StreamingAssets/Libraries~/petecavirtual.h", path + "/petecavirtual.h", false);

        string fileName = PersistentScript.currentMap?.name + "-file";

        if (!File.Exists($"{path}/{fileName}.cpp"))
        {
            File.Copy(Application.dataPath + "/Resources/StreamingAssets/Libraries~/MasterExample.cpp", $"{path}/{fileName}.cpp", false);
        }
        else
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

    private void Update()
    {
        var t = PersistentScript.timeT0;
        var score = PersistentScript.currentScore;

        if (isPlaying) PersistentScript.timeT0 += ((PersistentScript.playType == 0) ? (t > -0.5f) ? -1 : 0 : 1) * Time.deltaTime;
        if (t < 0)
        {
            ShowGOverMessage(Mathf.RoundToInt(score));
        }

        TimeTextBox.text = $"{((int)t / 60) % 60,4:00}{(((t / 2) % 2 == 0) ? " " : ":")}{((int)t % 60),2:00}";
        ScoreTextBox.text = $"#{score,4:0000}";
    }

}
