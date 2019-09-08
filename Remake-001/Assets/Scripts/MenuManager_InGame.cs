using System;
using System.Collections;
using System.Collections.Generic;
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

    [HideInInspector] public static int tasksToDisp;
    public static Transform TaskListGlobal;
    //Variaveis inerente ao mapa;
    public static float score;
    public static float timePassed;
    public static bool isPlaying = true;//check if the game is not paused


    private void Awake()
    {
        ExtLibControl.OnCommandCalled += UserActionControl;
        tasksToDisp = TaskQueueList.childCount - 1;
        TaskListGlobal = TaskQueueList;

        TaskListGlobal.parent.GetComponent<Toggle>().onValueChanged
            .AddListener(v => { TaskListGlobal.gameObject.SetActive(v); if (v) UpdateTaskQueueList(); });

        FileMenuContent = FileMenu.GetChild(0).GetChild(0);

        FileMenu.GetComponentInChildren<TextMeshProUGUI>().text = PersistentScript.currentMap?.name + "-Menu";
        FileMenu.GetComponent<Toggle>().onValueChanged                      //FileMenuButton
            .AddListener(v => { FileMenuContent.gameObject.SetActive(v); });

        FileMenuContent.GetChild(0).GetComponent<Toggle>().onValueChanged   //PauseButton
            .AddListener(v =>                                               //
            {                                                               //
                isPlaying = !v;                                             //
                FileMenu.parent.GetChild(1).gameObject.SetActive(v);        //
            });

        FileMenuContent.GetChild(1).GetComponent<Toggle>().onValueChanged   //RestartButton
            .AddListener(v => { ReloadLevel(); });

        FileMenuContent.GetChild(3).GetComponent<Toggle>().onValueChanged   //MainMenuButton
            .AddListener(delegate { SceneManager.LoadScene(0); PersistentScript.currentMap = null; });



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
            //PauseGame();
            Debug.Log("<color=#006666>Pausing....</color>");
            ExtLibControl.DeQueueAction(a);
        }
        else if (a.type == "getTIME")
        {
            ExtLibControl.PServer2.SendMessage($"{timePassed}", ExtLibControl.PServer2.clientse);
            Debug.Log($"<color=#00ff00>Time goted: time ={timePassed} seconds...</color>");
            ExtLibControl.DeQueueAction(a);
        }
        
    }

    public static void UpdateTaskQueueList()
    {

        PersistentScript.ExecuteOnMainThread.Enqueue(
        delegate
        {
            if (TaskListGlobal.gameObject.activeSelf)
            {
                var lastNActions = ExtLibControl.userActions.Take(Mathf.Min(tasksToDisp, ExtLibControl.userActions.Count)).ToArray(); ;
                var len = lastNActions.Length;
                for (int i = 0; i < tasksToDisp; i++)
                {

                    TaskListGlobal.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = (i < len) ?
                        $"{lastNActions?[i].type} - ({lastNActions?[i].value})"
                        : "---";

                }
                var QLen = ExtLibControl.userActions.Count;
                TaskListGlobal.GetChild(tasksToDisp).GetComponent<TextMeshProUGUI>().text =
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
