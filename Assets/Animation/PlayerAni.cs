using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAni : MonoBehaviour
{

    public Animator playe;
    public Animator bubble;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            playe.SetBool("Walk",true);
            bubble.SetBool("Walk", true);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            playe.SetBool("Walk", false);
            bubble.SetBool("Walk", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playe.SetBool("Attack", true);
            bubble.SetBool("Attack", true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            playe.SetBool("Attack", false);
            bubble.SetBool("Attack", false);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playe.SetBool("Dash", true);
            bubble.SetBool("Dash", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            playe.SetBool("Dash", false);
            bubble.SetBool("Dash", false);
        }
    }
}
