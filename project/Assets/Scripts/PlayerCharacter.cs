using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    private const float MoveSpeed = 0; // This is temporary

    private const float JumpVelocity = 15;
    private const float FallVelocity = -20;
    private bool _grounded = false;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    [SerializeField]
    private Collider2D _upperBody;

    [System.Serializable]
    private enum CharacterState { Running, Jumping, Falling, Sliding, Dancing }
    [SerializeField]
    private CharacterState _currentState = CharacterState.Running;

    private HashSet<Collision2D> _currentCollisions = new HashSet<Collision2D>();

    [SerializeField]
    private Transform _groundCheckPosition;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        ToRunning();
    }

    void Update()
    {
        // Temporary controls
        if (Input.GetKeyDown(KeyCode.Space)) StartJump();
        if (Input.GetKeyDown(KeyCode.DownArrow)) StartSlide();
        if (Input.GetKeyUp(KeyCode.DownArrow)) StopSlide();
    }

    void FixedUpdate()
    {
        GroundCheck();
        FallingCheck();
        RunningCheck();

        // This horizontal movement segment is temporary.
        // Normally the character should not move much horizontally unless he gets knocked down and to the left due to a fail, 
        // or when he get some kind of boost as a reward for success.
        Vector2 v = _rigidbody.velocity;
        v.x = MoveSpeed;
        _rigidbody.velocity = v;
    }

    private void GroundCheck()
    {
        _grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheckPosition.position, 0.2f);
        for (int i = 0; i < colliders.Length; ++i)
        {
            // if we find a collider that is not part of this gameObject, we'll consider ourselves grounded.
            if (colliders[i].gameObject != this.gameObject) { _grounded = true; }
        }
    }

    private void FallingCheck()
    {
        // Check if we are entering a fall. (This moment will happen at the highest point of a jump or when running off a platform.)
        if (!_grounded && _currentState != CharacterState.Falling && _rigidbody.velocity.y < -0.1f)
        {
            _currentState = CharacterState.Falling;
            _animator.Play("falling");
        }
    }

    private void RunningCheck()
    {
        // Check if we should be entering the running state.
        if (_grounded && _currentState == CharacterState.Falling)
            ToRunning();
    }

    private void ToRunning()
    {
        _currentState = CharacterState.Running;
        _animator.Play("running");
        _upperBody.enabled = true;
    }

    [ContextMenu("Jump")]
    public void StartJump()
    {
        // For the jump, we want to change direction instantly, so instead of using forces over time, we directly adjust velocity.
        if (_grounded)
        {
            // Change state
            _currentState = CharacterState.Jumping;

            // Add upwards impulse
            Vector2 v = _rigidbody.velocity;
            v.y = JumpVelocity;
            _rigidbody.velocity = v;
            // Play Sound
            // TODO
            // Play Animation

            _animator.Play("jumping", 0, 0);
        }
    }

    public void StartSlide()
    {
        if (_grounded)
        {
            // Change state
            _currentState = CharacterState.Sliding;
            // Disable upper body collider
            _upperBody.enabled = false;
            // Play Sound
            // TODO
            // Play Animation
            _animator.Play("sliding");
        }
    }

    public void StopSlide()
    {
        // If we are sliding
        if (_currentState == CharacterState.Sliding)
        {
            // Go back to running state
            ToRunning();
        }
    }
}