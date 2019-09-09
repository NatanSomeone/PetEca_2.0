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
    public static string incomingMessage;


    private void Awake()
    {
        if (persistentScript == null)
        { persistentScript = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);

        ExtLibControl.OnCommandCalled += UserActionsControl;

    }

    private void UserActionsControl(object sender, ExtLibControl.UserAction a)
    {
        if (a.type == "hold")
        {
            Debug.Log("holdCommand"+Time.time);
            ExecuteOnMainThread.Enqueue(()=>{
                Debug.Log("holdExecution"+Time.time);
                persistentScript.StartCoroutine(WaitAndDo(a.value, () => ExtLibControl.DeQueueAction(a)));
            });

        }


    }

    void Start()
    {
        ExtLibControl.INIT();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        IsPlaying = FindObjectOfType<MenuManager_InGame>() != null;
        ExtLibControl.ClearActionQueue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            ExtLibControl.DeQueueAction(ExtLibControl.userActions.Peek()); //Libera a ultima ação --para casos de encurralamento
        if (Input.GetKeyDown(KeyCode.X))
            ExtLibControl.ClearActionQueue(); //Limpa todas as Ações

        while(ExecuteOnMainThread.Count>0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }

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
