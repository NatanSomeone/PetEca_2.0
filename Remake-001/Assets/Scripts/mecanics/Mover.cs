using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Transform localPivot;
    public float speed;
    grabber gbb;

    private void Start()
    {
        gbb = this.GetComponentInChildren<grabber>();
    }
    private void Update()
    {

        if (!gbb.grabbing)
        {
            var hit = Physics.Raycast(transform.position, localPivot.forward)?1:0;
            transform.position += localPivot.forward * speed * Time.deltaTime * Input.GetAxis("Vertical") * hit;
            transform.RotateAround(localPivot.position, Vector3.up, Input.GetAxis("Horizontal") * speed * Time.deltaTime * 15f);
        }

    }
}
