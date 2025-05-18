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
    
    [Header("Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float deaccel;
    [SerializeField] private float dynamicDeaccel;
    
    [Header("Jump & Air")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float airDeaccel;
    [SerializeField] private int jumpCount;

    
    [Header("Speed Limits")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float airMaxSpeed;

    
    [Header("Ground Check")]
    [SerializeField] private Transform feetTransform;
    [SerializeField] private float groundCheckDistance;
    
    [Header("Wire-related members")]
    [SerializeField] private float wirePointDetectRadius;
    [SerializeField] private float minDistanceConst, maxDistanceConst, wireSwingForce;
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
    private bool _isGrounded;
    private int _jumpCount = 0;

    private bool _isWiring = false;
    private Collider[] _avilableWirePoints = new Collider[10];
    private Transform _currentWirePoint;

    private GroundFriction _currentGroundOn;
    
    private bool _isStun;
    private float _stunTimeElapsed;
    
    public Transform respawnPos;

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
        StunCheck();
        
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
        
        if (_isStun) 
            input = Vector2.zero;
    
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

        if (_isStun)
            airCof = airDeaccel / 2f;

        if (input.magnitude < 0.1f)
        {
            _rigidbody.AddForce(-velWithoutY * (airCof * groundFriction * deaccel * Time.deltaTime));
        }
        else
        {
            _rigidbody.AddForce(-velWithoutY * (airCof * dynamicDeaccel * Time.deltaTime));
        }
    }

    private void StunCheck()
    {
        if (!_isStun)
        {
            GetComponent<MeshRenderer>().materials[0].color = Color.white;
            return;
        }

        GetComponent<MeshRenderer>().materials[0].color = Color.red;
        _stunTimeElapsed -= Time.deltaTime;
        if (_stunTimeElapsed <= 0)
        {
            _isStun = false;
        }
    }
    
    private void Jump()
    {
        if (_isStun) return;
        
        if (_isGrounded || _jumpCount < jumpCount)
        {
            _jumpCount++;
            
            _rigidbody.linearVelocity -= _rigidbody.linearVelocity.y * Vector3.up;
            _rigidbody.AddForce(Vector3.up * (_jumpCount == 0 ? jumpForce : doubleJumpForce), ForceMode.Impulse);
        }
    }

    private void ScanWirePoints()
    {
        foreach (var i in _avilableWirePoints)
        {
            if (i != null)
                i.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.white;
        }
        
        int hit = Physics.OverlapSphereNonAlloc(transform.position, wirePointDetectRadius, _avilableWirePoints,
            LayerMask.GetMask("WirePoint"));

        if (hit == 0)
        {
            _avilableWirePoints = new Collider[10];
        }
        else
        {
            foreach (var i in _avilableWirePoints)
            {
                if (i != null)
                    i.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.yellow;
            }
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
        var point = GetAvailableWirePoint();

        if (_isStun) return;
        if (point == null) return;
        
        // 연결된 와이어를 통해 스윙하도록 힘을 준다.
        _rigidbody.linearVelocity = Vector3.zero;
        
        var vec0 = transform.position - point.transform.position;
        var vec1 = Vector3.Cross(vec0, Vector3.up);
        var vec2 = Vector3.Cross(vec0, vec1);
        
        _rigidbody.AddForce(vec2.normalized * wireSwingForce, ForceMode.Impulse);

        // 와이어 액션 상태로 바꾼다.
        _currentWirePoint = point.transform;
        _lineRenderer.enabled = true;
        _isWiring = true;

        // 와이어 포인트에 플레이어를 연결 시켜준다.
        var sprjt = _currentWirePoint.GetComponent<SpringJoint>();
        
        var minDistance = Vector3.Distance(point.transform.position, transform.position);
        
        sprjt.connectedBody = _rigidbody;
        sprjt.minDistance = minDistance / minDistanceConst;
        sprjt.maxDistance = minDistance / maxDistanceConst;
    }

    // 현재 _availableWirePoints 배열에서 와이어 연결 가능한 포인트가 있는지 체크
    // 연결 가능한 포인트가 없다면 null 반환
    private Collider GetAvailableWirePoint()
    {
        if (_avilableWirePoints[0] == null) return null;
        
        var point = _avilableWirePoints[0];
        float minDistance = Vector3.Distance(_avilableWirePoints[0].transform.position, transform.position);
        float tmpDistance = 0;
        
        for (int i = 0; i < _avilableWirePoints.Length; i++)
        {
            if (_avilableWirePoints[i] == null) continue;

            if (Camera.main.WorldToScreenPoint(point.transform.position).z < 0)
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

        if (Camera.main.WorldToScreenPoint(point.transform.position).z < 0)
            return null;

        return point;
    }

    public void GetHit(Vector3 knockback, float stunTime)
    {
        _isStun = true;
        _stunTimeElapsed = stunTime;

        if (_isWiring)
        {
            ToggleWireMode();
        }

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.AddForce(knockback, ForceMode.Impulse);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Respawn"))
        {
            _rigidbody.linearVelocity = Vector3.zero;
            transform.position = respawnPos.position;
        }
    }
}
