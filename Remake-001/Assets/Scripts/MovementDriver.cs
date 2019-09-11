using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDriver : MonoBehaviour
{

    public int RobotType = 1;
    public float VelocidadeTranslacao = 4;
    public float VelocidadeRotacao = 100;
    public float VelocidadedaGarra = 25;

    public Transform Claw;

    private Rigidbody rigidbodyRobo;
    //private SkinnedMeshRenderer meshRend;

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

    [HideInInspector] public bool clawState;//1 UP - 0 Down
    bool clawInAction;

    private void Start()
    {
        rigidbodyRobo = GetComponent<Rigidbody>();
        //meshRend = GetComponentInChildren<SkinnedMeshRenderer>();
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

                realPosition += fwdPosition * Mathf.Abs(desiredDisplacement) * vTranslacao;
            }
            else if (a.type == "rot") //type == rotation
            {
                ang = -2;
                vRotacao = Mathf.Sign(a.value);
                float d = a.value % 360; d = (d > 0) ? d : d + 360;
                desiredDisplacement = d;

                tRotation = nAng(tRotation + d);
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
            pAng %= 360; if (pAng < 0) pAng += 360f;

            ang = (pAng + DesiredDisplacement) % 360;
            fwdPosition = Quaternion.Euler(0, tRotation, 0) * initialFwdPosition;

            var a = Mathf.Sign(pAng - ang);
            //meshRend.SetBlendShapeWeight((a >= 0) ? 1 : 2, 100); //Girar rodinha
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
            rigidbodyRobo.MovePosition(realPosition);
        }



        if (vRotacao != 0)
        {
            var angN = rigidbodyRobo.rotation.eulerAngles.y;
            var dang = Mathf.DeltaAngle(tRotation, angN);
            if (Mathf.Abs(dang) < 2f)
            {
                //meshRend.SetBlendShapeWeight(1, 0);
                //meshRend.SetBlendShapeWeight(2, 0);//virar roda esquerda/direita

                rigidbodyRobo.transform.eulerAngles = Vector3.up * tRotation;
                DesiredDisplacement = 0;
                ang = -1;
            }
            else//terá rotação
            {
                dspCorrection = (dang < VelocidadeRotacao * 0.08f) ? dang * 1 / (VelocidadeRotacao * 0.08f) : 1;
                Vector3 deltaRotation = rotDelta = Vector3.up * vRotacao * VelocidadeRotacao * Time.fixedDeltaTime;
                rigidbodyRobo.MoveRotation(rigidbodyRobo.rotation * Quaternion.Euler(deltaRotation));
            }

        }

        if (clawInAction)
        {


            var clawAngle = (Claw.localEulerAngles.x - 360)%360;
            var clawControl = (clawState && clawAngle > -30) ? -1 : (!clawState && clawAngle < 0) ? 1 : 0;
            if (clawControl != 0)
            {
                Claw.localEulerAngles = Vector3.right *
                    Mathf.Clamp(clawAngle + (clawControl * Time.deltaTime * VelocidadedaGarra*10),
                    -30, 0);
            }
            else
            {
                clawInAction = false;
                ExtLibControl.DeQueueAction();
            }
            
        }

    }
    Vector3 rotDelta;

    private void OnGUI()
    {

        var angN = rigidbodyRobo.rotation.eulerAngles.y;
        var diff = Mathf.Abs(ang - angN);
        var dang = Mathf.Abs(Mathf.Min(diff, 360 - diff));
        

        //var clawAngle = (Claw.localEulerAngles.x - 360)%360;
        //var clawControl = (clawState && clawAngle > -30) ? -1 : (!clawState && clawAngle < 0) ? 1 : 0;
        //GUI.Label(new Rect(0, 100, Screen.width, Screen.height - 100), $"<color=#000099>\n\n" +
        //$"Garra {((clawState)?"Levantada":"Abaixada")}\n" +
        //$"currentAngle:{clawAngle:F2}\t -{clawControl}" +
        //$"</color>");

        //GUI.Label(new Rect(0, 100, Screen.width, Screen.height - 100),
        //    $"<color=#000099>\n\n" +
        //    $"Fwd{tRotation}\t{Mathf.DeltaAngle(tRotation, angN)} >>{angN}:{rotDelta.y:F3}" +
        //    $"\nReal:{realPosition}\t" +
        //    $"Delta:{(realPosition - transform.position).magnitude:F5}\n" +
        //    $"Dang{dang}\tDiff{diff}</color>");

    }

    public static float nAng(float ang)
    {
        ang %= 360; if (ang < 0) ang += 360f;
        return ang;
    }

}
//function dAng(a,b){return Math.min(Math.abs(a-b),360-Math.abs(a-b))}
