using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] Rigidbody2D rb;
    private Vector2 moveDirection;
    [SerializeField] private Animator animator;
    private bool isMovementEnabled = true; // Hareket aktif mi?

    void Update()
    {
        if (isMovementEnabled && moveDirection != Vector2.zero) 
        {
            animator.SetBool("IsWalking", true);
            animator.SetFloat("Horizontal", moveDirection.x);
            animator.SetFloat("Vertical", moveDirection.y);
        }
        else 
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void FixedUpdate() 
    {
        if (isMovementEnabled)
        {
            rb.MovePosition(rb.position + moveDirection * walkSpeed * Time.fixedDeltaTime);
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        isMovementEnabled = enabled;
        if (!enabled)
        {
            moveDirection = Vector2.zero; // Hareketi durdur
        }
        
    }
    
    public bool GetMovementEnabled()
    {
        return isMovementEnabled;
    }

    void OnMove(InputValue value) 
    {
        if (isMovementEnabled)
        {
            moveDirection = value.Get<Vector2>();
        }
    }
    
    
}
