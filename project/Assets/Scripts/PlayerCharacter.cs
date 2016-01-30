using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour
{
    private const float JumpVelocity = 15;
    private const float FallVelocity = -20;
    private bool _grounded = false;

    private Rigidbody2D _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartJump();
    }

    void LateUpdate()
    {

    }

    public void StartJump()
    {
        // For the jump, we want to change direction instantly, so instead of using forces over time, we directly adjust velocity.
        //if (_grounded)
        {
            Vector2 v = _rigidbody.velocity;
            v.y = JumpVelocity;
            _rigidbody.velocity = v;
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        _grounded = true;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        _grounded = true;
    }
}