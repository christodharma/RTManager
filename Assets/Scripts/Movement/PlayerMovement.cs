using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D RigidBody;
    private Vector2 MovementDirection = Vector2.zero;
    private Animator Animator;
    private int _Horizontal, _Vertical, _Velocity;
    public Vector2 MovementSpeed = new(0.1f, 0.1f);
    private void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        _Horizontal = Animator.StringToHash("Horizontal");
        _Vertical = Animator.StringToHash("Vertical");
        _Velocity = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        Animator.SetFloat(_Velocity, RigidBody.linearVelocity.magnitude);
        Animator.SetFloat(_Horizontal, MovementDirection.x);
        Animator.SetFloat(_Vertical, MovementDirection.y);
    }

    void FixedUpdate()
    {
        RigidBody.linearVelocity = MovementDirection * MovementSpeed;
    }

    public void ReadInput(InputAction.CallbackContext ctx)
    {
        MovementDirection = ctx.ReadValue<Vector2>();
    }
}
