using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement pMove;

    void Awake()
    {
        pMove = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W)) pMove.JumpFunction();

        pMove.ChangeMoveVelocity(Input.GetAxisRaw("Horizontal"));
    }
}
