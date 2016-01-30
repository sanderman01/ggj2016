using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    public float MoveSpeed = 1; // This is temporary

    private const float JumpVelocity = 15;
    private const float FallVelocity = -20;
    private bool _grounded = false;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    [SerializeField]
    private Collider2D _upperBody;

    private enum CharacterState { Running, Jumping, Falling, Sliding, Dancing }
    private CharacterState _currentState = CharacterState.Running;

    private HashSet<Collision> _currentCollisions = new HashSet<Collision>();

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        FallingCheck();

        if (Input.GetKeyDown(KeyCode.Space)) StartJump(); // Temporary

        // This horizontal movement segment is temporary. 
        // Normally the character should not move much horizontally unless he gets knocked down and to the left due to a fail, 
        // or when he get some kind of boost as a reward for success.
        Vector2 v = _rigidbody.velocity;
        v.x = MoveSpeed;
        _rigidbody.velocity = v;


    }

    private void FallingCheck()
    {
        // Check if we are entering a fall. (This moment will happen at the highest point of a jump or when running off a platform.)
        if (_currentState != CharacterState.Falling && _rigidbody.velocity.y < 0)
        {
            // Change state
            _currentState = CharacterState.Falling;
            // Play animation
            _animator.Play("falling");
        }
    }

    public void StartJump()
    {
        // For the jump, we want to change direction instantly, so instead of using forces over time, we directly adjust velocity.
        if (_grounded)
        {
            // Change state
            _currentState = CharacterState.Jumping;
            _grounded = false;

            // Add upwards impulse
            Vector2 v = _rigidbody.velocity;
            v.y = JumpVelocity;
            _rigidbody.velocity = v;
            // Play Sound
            // TODO
            // Play Animation
            _animator.Play("jump");
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
            _animator.Play("slide");
        }
    }



    void OnCollisionEnter(Collision collisionInfo)
    {
        // Keeping track of the currently active collisions is simple way to determine if our character is currently grounded or not,
        // A large flaw of this method is that a head-on collision against a wall will also count as being grounded.
        if (!_currentCollisions.Contains(collisionInfo))
        {
            _currentCollisions.Add(collisionInfo);
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        _grounded = true;
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (_currentCollisions.Contains(collisionInfo))
        {
            _currentCollisions.Remove(collisionInfo);
        }
    }
}