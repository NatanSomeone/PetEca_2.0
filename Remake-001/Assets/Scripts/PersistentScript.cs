using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentScript : MonoBehaviour
{
    //Ultilidade Publica
    public static PersistentScript persistentScript;
    public static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();


    //GameVariables
    public static MapDisplayInfo currentMap;
    public static bool IsPlaying;
    public static bool canRunUserActions;
    public static string incomingMessage = "Welcome";


    public Transform ItemCollection;

    public float timescale = 1;



    private void Awake()
    {
        if (persistentScript == null)                                    //
        { persistentScript = this; DontDestroyOnLoad(gameObject); }      //garante que só tenha um desse script na cena
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
        ItemCollection = GameObject.Find("ItemCollection").transform;
        IsPlaying = FindObjectOfType<MenuManager_InGame>() != null;
        ExtLibControl.ClearActionQueue();
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
            if (!(ExtLibControl.currentUAction == null))
            {
                var u = ExtLibControl.currentUAction.userAction;
                if (u.type == "hold" && u.target == -1)
                {
                    ExtLibControl.currentUAction.userAction.target = 1;
                    StartCoroutine(WaitAndDo(u.value, () => ExtLibControl.DeQueueAction()));
                }
            }

        }


    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(0, 30, Screen.width, Screen.height - 30),
        //    $"<color=#000099> {ExtLibControl.currentUAction?.userAction.type}:{ExtLibControl.currentUAction?.userAction.value}/" +
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
        Debug.Log($"<color=#000066>Waiting {time} seconds...</color>");
        yield return new WaitForSecondsRealtime(time);
        action.Invoke();
    }
}
