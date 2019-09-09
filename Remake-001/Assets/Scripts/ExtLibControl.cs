﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtLibControl
{

    public static NamedPipeServer PServer1;
    public static NamedPipeServer PServer2;

    public static event EventHandler<UserAction> OnCommandCalled;

    public static UActionHolder currentUAction;

    public class UActionHolder
    {
        public UserAction userAction;
        public bool done;
        public float t0;
    }

    public static Queue<UserAction> userActions = new Queue<UserAction>();

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public struct UserAction
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public string type; //tipo de ação
        public int target; //Id do Robo
        public float value; //dado vinculado

        public UserAction(string type, int target, float value)
        { this.type = type; this.target = target; this.value = value; }

        public UserAction(string type, int target)
        { this.type = type; this.target = target; this.value = -1; }

        public UserAction(string type)
        { this.type = type; this.target = -1; this.value = -1; }

        public static bool operator ==(UserAction a, UserAction b) => (a.type == b.type) && (a.target == b.target) && (a.value == b.value);
        public static bool operator !=(UserAction a, UserAction b) => (a.type != b.type) || (a.target != b.target) || (a.value != b.value);
    }

    public static void INIT() //inicializa a comunicação por "pipes"
    {
        PServer1 = new NamedPipeServer(@"\\.\pipe\PetServerPipe0", 0);//sread
        PServer2 = new NamedPipeServer(@"\\.\pipe\PetServerPipe1", 1);//swrite
        PServer1.Start();
        PServer2.Start();
        NamedPipeServer.OnDataReceived += SeverResponse;//sempre que onDataR.. for chamado , chama serverRes.. com o parâmetro 
    }


    private static void SeverResponse(object sender, string readValue) //responde ao recebimento de dados, lendo o que foi rescebido e criando açoes
    {
        string st = $"Recebido->{readValue}";
        if (PersistentScript.IsPlaying) //ignora tudo se não tiver jogando
        {
            UserAction action = new UserAction();

            var v = readValue.Split(' ');

            switch (v[0])
            {
                case "MOVE":
                    if (v.Length == 3)
                    {
                        st = $"Movendo {((v[1] == "R0") ? "Robo Vermelho" : "Robo Azul")} {float.Parse(v[2])} unidades";

                        action = new UserAction("move", (v[1] == "R1") ? 1 : 0, float.Parse(v[2]));
                    }
                    break;
                case "ROTATE":
                    if (v.Length == 3)
                    {
                        st = $"Rotacionando {((v[1] == "R0") ? "Robo Vermelho" : "Robo Azul")} {float.Parse(v[2])} unidades";

                        action = new UserAction("rot", (v[1] == "R1") ? 1 : 0, float.Parse(v[2]));
                    }
                    break;
                case "PAUSE":

                    st = $"Pausando...";
                    action = new UserAction("pause");

                    break;
                case "GARRA":
                    if (v.Length == 2)
                    {
                        st = $"Agindo na garra do {((v[1] == "R0") ? "Robo Vermelho" : "Robo Azul")}";
                        action = new UserAction("garra", (v[1] == "R1") ? 1 : 0);
                    }
                    break;
                case "CAM"://não implementada
                    if (v.Length == 2)
                    {
                        st = $"Alternando para camera {v[1]}";
                        action = new UserAction("cam", -1, float.Parse(v[1]));
                    }
                    break;
                case "WAIT"://não implementada
                    if (v.Length == 2)
                    {
                        st = $"Segurando a fila por  {v[1]} segundos";
                        action = new UserAction("hold", -1, float.Parse(v[1]));
                    }
                    break;
                case "getTIME":

                    action = new UserAction("getTIME");

                    break;
                case "RESTART":
                    Debug.Log("<color=#006666>Reiniciando mapa....</color>");
                    MenuManager_InGame.ReloadLevel();
                    break;
                default:
                    break;
            }
            userActions.Enqueue(action);

            //Debug.Log($"{st}\n{userActions.Count} ações na fila");

            //if (userActions.Count == 1) //encarrilha açoes
            //    MoveActionQueue();
        };

    }

    public static void DeQueueAction() //tira a ultima ação de fila *sem rodar
    {
        currentUAction = null; // quando pronta é nula
        MenuManager_InGame.UpdateTaskQueueList();
        
    }

    public static void MoveActionQueue()//chama a proxima ação da fila
    {
        if ((userActions.Count > 0))
        {
            var usrAction = userActions.Dequeue();
            OnCommandCalled?.Invoke(null, usrAction); // a ação atual é a da frente na fila
            currentUAction = new UActionHolder { userAction = usrAction, t0 = Time.time, done = false };
        }
        else{
            currentUAction = null;
        }
    }

    public static void ClearActionQueue() //limpa a fila de ações
    {
        DeQueueAction();
        userActions.Clear();
    }

    public static void END()
    {
        PServer1.StopServer();
        PServer2.StopServer();

    }


}
