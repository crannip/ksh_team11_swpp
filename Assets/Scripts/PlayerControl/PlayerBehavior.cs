using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBehavior : MonoBehaviour
{
    /********************************************************************************/
    [Header("Player Properties")]
    # region player properties
    
    [SerializeField] private float acceleration;
    [SerializeField] private float deaccel;
    [SerializeField] private float dynamicDeaccel;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float airMaxSpeed;

    [SerializeField] private float airDeaccel;

    [SerializeField] private Transform feetTransform;

    [SerializeField] private float groundCheckDistance;
    
    [SerializeField] private float wirePointDetectRadius;

    [SerializeField] private int jumpCount;

    public float min, max, forr;
    # endregion
    
    /********************************************************************************/
    [Header("external Properties")]
    # region external properties

    [SerializeField] private GameObject cameraObject;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wirePointLayer;
    
    # endregion 

    /********************************************************************************/
    [Header("Player Object Components")]
    # region Component_Refs
    
    private Rigidbody _rigidbody;
    private PlayerInputProcessor _inputProcessor;
    private LineRenderer _lineRenderer;

    # endregion
    
    /********************************************************************************/
    private Vector3 _velocity;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private int _jumpCount = 0;

    [SerializeField] private bool _isWiring = false;
    [SerializeField] private Collider[] _avilableWirePoints = new Collider[10];
    [SerializeField] private Transform _currentWirePoint;

    [SerializeField] private GroundFriction _currentGroundOn;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wirePointDetectRadius);
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputProcessor = GetComponent<PlayerInputProcessor>();
        _lineRenderer = GetComponent<LineRenderer>();

        _inputProcessor.jumpEvent.AddListener(Jump);
        _inputProcessor.shotEvent.AddListener(ToggleWireMode);

        _lineRenderer.enabled = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        GroundCheck();
        
        if (!_isWiring)
        {
            ScanWirePoints();
            Move();
        }
        else
        {
            RenderWire();
        }
    }

    private void GroundCheck()
    {
        if (Physics.Raycast(feetTransform.position, -Vector3.up, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            _isGrounded = true;
            _jumpCount = 0;
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
        direction = Quaternion.AngleAxis(cameraObject.transform.rotation.eulerAngles.y, Vector3.up) * direction;
    
        _rigidbody.linearVelocity += direction * (acceleration * Time.deltaTime);
        
        Vector3 velWithoutY = _rigidbody.linearVelocity;
        velWithoutY.y = 0;

        float groundFriction = _currentGroundOn != null ? _currentGroundOn.friction : 1f;
        float airCof = _isGrounded ? 1 :
             (_rigidbody.linearVelocity.magnitude > maxSpeed
                ? (_rigidbody.linearVelocity.magnitude > airMaxSpeed ? 1 : airDeaccel)
                : 1f);
        
        Debug.Log(_rigidbody.linearVelocity.magnitude);

        if (input.magnitude < 0.1f)
        {
            _rigidbody.AddForce(-velWithoutY * (airCof * groundFriction * deaccel * Time.deltaTime));
        }
        else
        {
            _rigidbody.AddForce(-velWithoutY * (airCof * dynamicDeaccel * Time.deltaTime));
        }
    }
    

    private void Jump()
    {
        if (_isGrounded || _jumpCount < jumpCount)
        {
            _jumpCount++;
            
            _rigidbody.linearVelocity -= _rigidbody.linearVelocity.y * Vector3.up;
            _rigidbody.AddForce(Vector3.up * (_jumpCount == 0 ? jumpForce : doubleJumpForce), ForceMode.Impulse);
        }
    }

    private void ScanWirePoints()
    {
        int hit = Physics.OverlapSphereNonAlloc(transform.position, wirePointDetectRadius, _avilableWirePoints,
            LayerMask.GetMask("WirePoint"));

        if (hit == 0)
        {
            _avilableWirePoints = new Collider[10];
        }
    }

    private void ToggleWireMode()
    {
        if (_isWiring)
        {
            StopWiring();
        }
        else
        {
            TryConnectWire();
        }
    }
    private void TryConnectWire()
    { 
        if (_avilableWirePoints[0] == null) return;
        
        var point = _avilableWirePoints[0];
        float minDistance = Vector3.Distance(_avilableWirePoints[0].transform.position, transform.position);
        float tmpDistance = 0;

        for (int i = 0; i < _avilableWirePoints.Length; i++)
        {
            if (_avilableWirePoints[i] == null) continue;

            if (Camera.main.WorldToScreenPoint(point.transform.position).z < minDistance)
            {
                minDistance = Vector3.Distance(_avilableWirePoints[i].transform.position, transform.position);
                point = _avilableWirePoints[i];
                continue;
            }
            if ((tmpDistance = Vector3.Distance(_avilableWirePoints[i].transform.position, transform.position)) < minDistance)
            {
                minDistance = tmpDistance;
                point = _avilableWirePoints[i];
            }
        }
        
        var vec0 = transform.position - point.transform.position;
        var vec1 = Vector3.Cross(vec0, Vector3.up);
        var vec2 = Vector3.Cross(vec1, vec0);
        
        _rigidbody.AddForce(vec2.normalized * forr, ForceMode.Impulse);

        _currentWirePoint = point.transform;
        _lineRenderer.enabled = true;
        _isWiring = true;

        var sprjt = _currentWirePoint.GetComponent<SpringJoint>();
        
        _rigidbody.linearVelocity = Vector3.zero;

        sprjt.connectedBody = _rigidbody;
        sprjt.minDistance = minDistance / 2f;
        sprjt.maxDistance = minDistance / 3f;
    }

    private void RenderWire()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _currentWirePoint.position);
    }

    private void StopWiring()
    {
        _currentWirePoint.GetComponent<SpringJoint>().connectedBody = null;

        _currentWirePoint = null;
        _lineRenderer.enabled = false;
        _isWiring = false;
    }
}
