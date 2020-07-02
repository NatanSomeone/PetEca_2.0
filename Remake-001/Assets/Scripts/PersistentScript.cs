using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentScript : MonoBehaviour
{
    //Ultilidade Publica
    public static PersistentScript instance;
    public static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();
    public static string userSecretValue;

    //GameVariables
    public static MapDisplayInfo currentMap;
    public static bool IsPlaying;
    public static bool canRunUserActions;
    public static string incomingMessage = "Welcome";
    public static float timeT0;
    public static float currentScore;
    public static int lastScore;
    public static int playType;//0_By-score * 1_By-time
    public static Menu_manager.User.UserObject usr;

    public AudioClip clickClip;
    public Transform ItemCollection;
    public Transform cameraHolder;

    public float timescale = 1;
    private float holdTime;
    private int currentCamera;
    public static bool DEBUGmode;

    public static void ClickSfx() => AudioSource.PlayClipAtPoint(instance?.clickClip, new Vector3(5, 1, 2));

    private void Awake()
    {
        if (instance == null)                                    //
        { instance = this; DontDestroyOnLoad(gameObject); gameObject.AddComponent<Highscores>();}      //garante que só tenha um desse script na cena
        else Destroy(gameObject);                                        //

        ExtLibControl.OnCommandCalled += UserActionsControl; //cahamado sempre que a proxima ação da fila é liberada

    }

    private void UserActionsControl(object sender, ExtLibControl.UserAction a)
    {
        //if (a.type == "hold")
        //{
        //    ExecuteOnMainThread.Enqueue(()=>{
        //    persistentScript.StartCoroutine(WaitAndDo(a.value, () => ExtLibControl.currentUAction.done = true ));
        //    });

        //}
    }

    void Start()
    {
        ExtLibControl.INIT();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        IsPlaying = FindObjectOfType<MenuManager_InGame>() != null;
    }

    void Update()
    {
        Time.timeScale = timescale;
        if (Input.GetKeyDown(KeyCode.Z))
            ExtLibControl.DeQueueAction(); //Libera a ultima ação --para casos de encurralamento
        if (Input.GetKeyDown(KeyCode.X))
            ExtLibControl.ClearActionQueue(); //Limpa todas as Ações


        float v = (Input.GetKeyDown(KeyCode.W) ? 1 : Input.GetKeyDown(KeyCode.S) ? -1 : 0);
        if (v != 0)
        {
            ExtLibControl.userActions.Enqueue(new ExtLibControl.UserAction("move", 0, .2f * v));
        }
        v = (Input.GetKeyDown(KeyCode.D) ? 1 : Input.GetKeyDown(KeyCode.A) ? -1 : 0);
        if (v != 0)
        {
            ExtLibControl.userActions.Enqueue(new ExtLibControl.UserAction("rot", 0, 15f * v));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExtLibControl.userActions.Enqueue(new ExtLibControl.UserAction("garra", 0));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExtLibControl.userActions.Enqueue(new ExtLibControl.UserAction("hold", -1, 3));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ExtLibControl.userActions.Enqueue(new ExtLibControl.UserAction("getTIME"));
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentCamera = (currentCamera + 1) % cameraHolder.childCount;
            ExtLibControl.userActions.Enqueue(new ExtLibControl.UserAction("cam", -1, currentCamera));
        }

        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            DEBUGmode = !DEBUGmode;
        }


        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }

        if (ExtLibControl.userActions.Count > 0)
        {
            if (((ExtLibControl.currentUAction == null) ? true : ExtLibControl.currentUAction.done) &&
                canRunUserActions && MenuManager_InGame.isPlaying)
            {
                ExtLibControl.MoveActionQueue();
            }

            if (!(ExtLibControl.currentUAction == null) && canRunUserActions && MenuManager_InGame.isPlaying)
            {
                var u = ExtLibControl.currentUAction.userAction;
                switch (u.type)
                {
                    case "holdReal" when u.target == -1:
                        ExtLibControl.currentUAction.userAction.target = 1;
                        StartCoroutine(WaitAndDo(u.value, () => ExtLibControl.DeQueueAction()));
                        break;
                    case "hold":
                        if (u.target == -1)
                        {
                            ExtLibControl.currentUAction.userAction.target = 1;
                            holdTime = u.value;
                        }
                        else if (holdTime < 1e-8)
                        {
                            ExtLibControl.DeQueueAction();
                        }
                        else
                        {
                            holdTime -= Time.deltaTime;
                        }
                        break;
                    case "restart" when u.target == -1:
                        ExtLibControl.currentUAction.userAction.target = 1;
                        MenuManager_InGame.ReloadLevel();
                        break;
                    case "speed":
                        timescale = u.value;
                        ExtLibControl.DeQueueAction();
                        break;
                    case "cam":
                        {
                            if (u.value < cameraHolder.childCount)
                                for (int i = 0; i < cameraHolder.childCount; i++)
                                {
                                    cameraHolder.GetChild(i).gameObject.SetActive(i == u.value);
                                }

                            ExtLibControl.DeQueueAction();
                            break;
                        }
                }
                //feedback ActionsBehaviours
                switch (u.type)
                {
                    case "getTIME":
                        PipeFeedback(timeT0);
                        break;
                    case "getCameraCount":
                        PipeFeedback(cameraHolder.childCount);
                        break;
                    case "getScore":
                        PipeFeedback((int)currentScore);
                        break;                    
                }
            }


        }


    }

    public static void PipeFeedback(float value)
    {
        ExtLibControl.PServer2.SendMessage($"{value}", ExtLibControl.PServer2.clientse);
        ExtLibControl.DeQueueAction();
    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(0, 30, Screen.width, Screen.height - 30),
        //    $"<color=#000099>" +
        //    $"\n\n{ExtLibControl.PServer2.clientse?.stream.CanWrite}" +
        //    $" {ExtLibControl.currentUAction?.userAction.type}:{ExtLibControl.currentUAction?.userAction.value}/" +
        //    $"-{ExtLibControl.userActions.Count}\n" +
        //    $"\tActionDone:   {((ExtLibControl.currentUAction == null) ? true : ExtLibControl.currentUAction.done)}\n" +
        //    $"\tCanRun:   {canRunUserActions}</color>");
    }

    private void OnApplicationQuit()
    {
        ExtLibControl.END();
    }

    public IEnumerator WaitAndDo(float time, Action action)
    {
        //Debug.Log($"<color=#000066>Waiting {time} seconds...</color>");
        yield return new WaitForSecondsRealtime(time);
        action.Invoke();
    }
}
