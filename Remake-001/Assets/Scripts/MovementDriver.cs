using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDriver : MonoBehaviour
{

    public int RobotType = 1;
    public float VelocidadeTranslacao = 4;
    public float VelocidadeRotacao = 100;
    public float VelocidadedaGarra = 25;

    private Rigidbody rigidbodyRobo;
    private SkinnedMeshRenderer meshRend;

    float vTranslacao, vRotacao;
    float desiredDisplacement;
    float ang = -1;


    float DesiredDisplacement {
        get => desiredDisplacement;
        set
        {
            desiredDisplacement = Mathf.Abs(value);
            if (desiredDisplacement <= 1E-8f)
            {
                desiredDisplacement = vRotacao = vTranslacao = 0;
                ExtLibControl.DeQueueAction();
            }
        }
    }
    //teoricPosition
    private Vector3 realPosition;
    private Vector3 initialFwdPosition, fwdPosition;
    private float tRotation;

    [HideInInspector]public bool clawState;//1 UP - 0 Down
    bool clawInAction;

    private void Start()
    {
        rigidbodyRobo = GetComponent<Rigidbody>();
        meshRend = GetComponentInChildren<SkinnedMeshRenderer>();
        ExtLibControl.OnCommandCalled += OnMoveCommand;

        //teoricPosition
        realPosition = transform.position;
        fwdPosition = transform.forward;
        initialFwdPosition = transform.forward;
    }

    private void OnMoveCommand(object sender, ExtLibControl.UserAction a)
    {
        if (a.target == RobotType) //target == BlueBot
        {
            if (a.type == "move") //type == Movement
            {
                vTranslacao = Mathf.Sign(a.value);
                DesiredDisplacement = a.value;

                realPosition += fwdPosition * Mathf.Abs(desiredDisplacement);
            }
            else if (a.type == "rot") //type == rotation
            {
                ang = -2;
                vRotacao = Mathf.Sign(a.value);
                float d = a.value % 360; d = (d > 0) ? d : d + 360;
                desiredDisplacement = d;

                tRotation = (tRotation + d) % 360;
            }
            else if (a.type == "garra")
            {
                clawState = !clawState;
                clawInAction = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (ang == -2)
        {
            float pAng = rigidbodyRobo.rotation.eulerAngles.y;
            pAng %= 360;if (pAng < 0) pAng += 360f;

            ang = (pAng + DesiredDisplacement) % 360;
            fwdPosition = Quaternion.Euler(0, tRotation, 0) * initialFwdPosition;

            var a = Mathf.Sign(pAng-ang);
            meshRend.SetBlendShapeWeight( (a >= 0) ? 1 : 2, 100);
        }

        var dspCorrection = (Mathf.Abs(DesiredDisplacement) < 0.1) ? Mathf.Abs(DesiredDisplacement) * 10 : 1;
        Vector3 displacement = (transform.forward * (vTranslacao * dspCorrection) * VelocidadeTranslacao * Time.fixedDeltaTime);

        if (vTranslacao != 0)
        {
            DesiredDisplacement -= displacement.magnitude;
            rigidbodyRobo.MovePosition(rigidbodyRobo.position + displacement);
        }
        else if ((realPosition - transform.position).magnitude > 0.01f)
        {
            transform.position = realPosition;
        }



        if (vRotacao != 0 && ang > 0)
        {
            var angN = rigidbodyRobo.rotation.eulerAngles.y;
            var diff = Mathf.Abs(ang - angN);
            var dang = Mathf.Min(diff, 360 - diff);

            if (Mathf.Abs(dang) < 0.001f)
            {
                meshRend.SetBlendShapeWeight(1, 0);
                meshRend.SetBlendShapeWeight(2, 0);

                DesiredDisplacement = 0;
                ang = -1;
            }
            else//terá rotação
            {
                Vector3 deltaRotation = new Vector3(0, vRotacao, 0) * VelocidadeRotacao * Time.fixedDeltaTime;
                rigidbodyRobo.MoveRotation(rigidbodyRobo.rotation * Quaternion.Euler(deltaRotation));
            }

        }

        if (clawInAction)
        {

            float clawProgress = meshRend.GetBlendShapeWeight(0);
            var clawControl = (clawState && clawProgress < 100) ? 1 : (!clawState && clawProgress > 0) ? -1 : 0;
            if (clawControl!=0)
            {
                meshRend.SetBlendShapeWeight(0,
                    Mathf.Clamp(clawProgress + clawControl * VelocidadedaGarra * Time.fixedDeltaTime * 100,
                    0, 100));
            }
            else
            {
                clawInAction = false;
                ExtLibControl.DeQueueAction();
            }
        }

    }

    private void OnGUI()
    {

        //var angN = rigidbodyRobo.rotation.eulerAngles.y;
        //var diff = Mathf.Abs(ang - angN);
        //var dang = Mathf.Abs(Mathf.Min(diff, 360 - diff));
        //if (DesiredDisplacement != 0)
        //{

        //    GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 100),
        //                    $"<color=#06357a><size=25><b>" +
        //                    $"Deslocando " +
        //                    $"{DesiredDisplacement:F2} u \n" +
        //                    ((vRotacao != 0) ? (
        //                    $"no sentido {((vRotacao == -1) ? "anti" : "")}horário \n " +
        //                    $"faltando {dang:F2}° " +
        //                    $"para {ang:F2}°") : "") +
        //                    $"</b></size></color>");
        //}
        //GUI.Label(new Rect(0, 0, Screen.width, Screen.height),
        //    $"<color=#000099>Fwd{tRotation}\nReal:{realPosition}\tDelta:{(realPosition - transform.position).magnitude:F5}\nDang{ang - angN}</color>");

    }
}
