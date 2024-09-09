using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Avoider : MonoBehaviour
{
    private AvoiderMovement avoiderMovement;

    [SerializeField] private float moveSpeed = 5f; // 이동 속도

    [SerializeField] private float JumpForce = 10f;

    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 finalMovement;

    void Start()
    {
        if(avoiderMovement == null)
           avoiderMovement = GetComponent<AvoiderMovement>();

        if(characterController == null)
            characterController = GetComponent<CharacterController>();
        
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {

        finalMovement = avoiderMovement.HandleMovement(LocalPlayerInputManager.instance.moveInput_2P, moveSpeed);


        characterController.Move(finalMovement * Time.deltaTime);

    }
}
