using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    # region player properties
    
    [SerializeField] private float acceleration;
    [SerializeField] private float deaccel;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private Transform feetTransform;
    
    # endregion
    
    # region external properties
    
    [SerializeField] private LayerMask groundLayer;
    
    # endregion 

    # region Component_Refs
    
    private Rigidbody _rigidbody;
    private PlayerInputProcessor _inputProcessor;

    # endregion
    
    private Vector3 _velocity;
    private bool _isGrounded;

    private GroundFriction _currentGroundOn;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputProcessor = GetComponent<PlayerInputProcessor>();
    }
    
    private void Update()
    {
        GroundCheck();
        Move();
        ApplyFriction();
    }

    private void GroundCheck()
    {
        if (Physics.Raycast(feetTransform.position, -Vector3.up, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            _isGrounded = true;
            
            _currentGroundOn = hit.collider.GetComponent<GroundFriction>();
        }
        else
        {
            _isGrounded = false;

            _currentGroundOn = null;
        }
    }

    private void Move()
    {
        var input = _inputProcessor.MoveInput.normalized;
        
        Vector3 direction = new Vector3(input.x, 0, input.y);
        _rigidbody.linearVelocity += direction * (acceleration * Time.deltaTime);

        if (input.magnitude < 0.1f)
        {
            _rigidbody.AddForce(-_rigidbody.linearVelocity * (deaccel * Time.deltaTime));
        }
        else
        {
            _rigidbody.AddForce(-_rigidbody.linearVelocity * (maxSpeed * Time.deltaTime));
        }
    }

    private void ApplyFriction()
    {
        if (Physics.Raycast(feetTransform.position, -Vector3.up, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            if (_currentGroundOn != null)
            {
                _rigidbody.AddForce(-_rigidbody.linearVelocity * _currentGroundOn.friction);
            }
        }
    }
}
