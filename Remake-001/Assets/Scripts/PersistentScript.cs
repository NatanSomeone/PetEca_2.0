using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentScript : MonoBehaviour
{
    public static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();
    public static PersistentScript persistentScript;
    public static float timePassed;

    private void Awake()
    {
        if (persistentScript == null)
        { persistentScript = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);

    }


    void Start()
    {
        ExtLibControl.INIT();
        ExtLibControl.OnCommandCalled += UserActionControl;
    }

    private void UserActionControl(object sender, ExtLibControl.UserAction a)
    {
        //Debug.Log($"<color=#006600>I am red-->{a.type}</color>");
        if (a.type == "pause")
        {
            //PauseGame();
            //PauseCommand = true;
            Debug.Log("<color=#006666>Pausing....</color>");
            ExtLibControl.DeQueueAction();
        }
        else if (a.type == "restart")
        {
            //ReloadCommand = true;
            Debug.Log("<color=#006666>Reoloading....</color>");
            ExtLibControl.DeQueueAction();
        }
        else if (a.type == "hold")
        {
            ExecuteOnMainThread.Enqueue(() => { StartCoroutine(WaitAndDo(a.value, () => ExtLibControl.DeQueueAction())); });
        }
        else if (a.type == "getTIME")
        {
            ExtLibControl.PServer2.SendMessage($"{timePassed}", ExtLibControl.PServer2.clientse);
            Debug.Log($"<color=#00ff00>Time goted: time ={timePassed} seconds...</color>");
            ExtLibControl.DeQueueAction();
        }
    }

    public static IEnumerator WaitAndDo(float time, Action action)
    {
        Debug.Log($"<color=#000066>Waiting {time} seconds...</color>");
        yield return new WaitForSecondsRealtime(time);
        action();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            ExtLibControl.DeQueueAction();
        if (Input.GetKeyDown(KeyCode.X))
            ExtLibControl.userActions.Clear();

        while (ExecuteOnMainThread.Count > 0)
            ExecuteOnMainThread.Dequeue().Invoke();


    }
    private void OnApplicationQuit()
    {
        ExtLibControl.END();
    }
}
