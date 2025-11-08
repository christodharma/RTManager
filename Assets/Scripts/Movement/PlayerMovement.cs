using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private Transform Player;
    private Vector2 MovementDirection = Vector2.zero;
    private Animator Animator;
    private int _Horizontal, _Vertical, _Velocity;
    public Vector2 MovementSpeed = new(0.1f, 0.1f);
    private void Awake()
    {
        Player = GetComponent<Transform>();
        Animator = GetComponent<Animator>();
        _Horizontal = Animator.StringToHash("Horizontal");
        _Vertical = Animator.StringToHash("Vertical");
        _Velocity = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 _CurrentVelocity = MovementDirection * MovementSpeed;
        Player.position += _CurrentVelocity;

        Animator.SetFloat(_Velocity, _CurrentVelocity.magnitude);
        Animator.SetFloat(_Horizontal, MovementDirection.x);
        Animator.SetFloat(_Vertical, MovementDirection.y);
    }

    public void ReadInput(InputAction.CallbackContext ctx)
    {
        MovementDirection = ctx.ReadValue<Vector2>();
    }
}
