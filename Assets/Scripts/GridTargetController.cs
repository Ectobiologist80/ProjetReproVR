using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(SphereCollider))]
public class GridTargetController : MonoBehaviour
{
    private enum TargetState
    {
        Hidden,
        IdleBlue,
        Moving,
        HitGreenCooldown
    }

    [Header("References")]
    [SerializeField] private Transform playerHead;

    [Header("Grid Layout")]
    [SerializeField] private float gridDistanceFromPlayer = 1.0f;
    [SerializeField] private float cellSpacing = 0.35f;

    [Header("Timing")]
    [SerializeField] private float stayDuration = 2.0f;
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private float hitGreenDuration = 1.0f;

    [Header("Colors")]
    [SerializeField] private Color idleColor = Color.blue;
    [SerializeField] private Color hitColor = Color.green;

    private Renderer _renderer;
    private Material _material;

    private TargetState _state = TargetState.Hidden;

    private Vector3[] _gridPositions;
    private int _currentIndex = -1;
    private int _previousIndex = -1;

    private float _stateTimer = 0f;

    private Vector3 _moveStart;
    private Vector3 _moveEnd;

    private bool _initialized = false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;

        SetVisible(false);
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.IsTrialRunning)
        {
            if (_state != TargetState.Hidden)
            {
                HideTarget();
            }

            return;
        }

        if (!_initialized)
        {
            InitializeGridAndStart();
        }

        switch (_state)
        {
            case TargetState.IdleBlue:
                UpdateIdleBlue();
                break;

            case TargetState.Moving:
                UpdateMoving();
                break;

            case TargetState.HitGreenCooldown:
                UpdateHitGreenCooldown();
                break;
        }
    }

    private void InitializeGridAndStart()
    {
        BuildGrid();
        MoveImmediatelyToRandomStart();
        EnterIdleBlueState();

        _initialized = true;
    }

    private void BuildGrid()
    {
        if (playerHead == null)
        {
            Debug.LogError("GridTargetController: playerHead is missing.");
            return;
        }

        Vector3 forward = playerHead.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = playerHead.right;
        right.y = 0f;
        right.Normalize();

        Vector3 center = playerHead.position + forward * gridDistanceFromPlayer;

        _gridPositions = new Vector3[9];
        int index = 0;

        for (int row = 1; row >= -1; row--)
        {
            for (int col = -1; col <= 1; col++)
            {
                Vector3 offset =
                    (right * col * cellSpacing) +
                    (Vector3.up * row * cellSpacing);

                _gridPositions[index] = center + offset;
                index++;
            }
        }
    }

    private void MoveImmediatelyToRandomStart()
    {
        int startIndex = Random.Range(0, 9);
        _currentIndex = startIndex;
        _previousIndex = startIndex;

        transform.position = _gridPositions[startIndex];
        FacePlayerYawOnly();
        SetVisible(true);
    }

    private void UpdateIdleBlue()
    {
        _stateTimer += Time.deltaTime;

        FacePlayerYawOnly();

        if (_stateTimer >= stayDuration)
        {
            BeginMoveToNextPosition();
        }
    }

    private void UpdateMoving()
    {
        _stateTimer += Time.deltaTime;

        float t = Mathf.Clamp01(_stateTimer / moveDuration);
        transform.position = Vector3.Lerp(_moveStart, _moveEnd, t);

        FacePlayerYawOnly();

        if (t >= 1f)
        {
            EnterIdleBlueState();
        }
    }

    private void UpdateHitGreenCooldown()
    {
        _stateTimer += Time.deltaTime;

        FacePlayerYawOnly();

        if (_stateTimer >= hitGreenDuration)
        {
            BeginMoveToNextPosition();
        }
    }

    private void BeginMoveToNextPosition()
    {
        int nextIndex = GetRandomNextIndex();

        _previousIndex = _currentIndex;
        _currentIndex = nextIndex;

        _moveStart = transform.position;
        _moveEnd = _gridPositions[nextIndex];

        _state = TargetState.Moving;
        _stateTimer = 0f;
    }

    private int GetRandomNextIndex()
    {
        if (_gridPositions == null || _gridPositions.Length != 9)
            return 0;

        int nextIndex = _currentIndex;

        while (nextIndex == _currentIndex)
        {
            nextIndex = Random.Range(0, 9);
        }

        return nextIndex;
    }

    private void EnterIdleBlueState()
    {
        _state = TargetState.IdleBlue;
        _stateTimer = 0f;
        _material.color = idleColor;
        SetVisible(true);
    }

    private void EnterHitGreenState()
    {
        _state = TargetState.HitGreenCooldown;
        _stateTimer = 0f;
        _material.color = hitColor;
        SetVisible(true);
    }

    private void HideTarget()
    {
        _state = TargetState.Hidden;
        _stateTimer = 0f;
        _initialized = false;
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (_renderer != null)
        {
            _renderer.enabled = visible;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = visible;
        }
    }

    private void FacePlayerYawOnly()
    {
        if (playerHead == null)
            return;

        Vector3 lookTarget = playerHead.position;
        lookTarget.y = transform.position.y;

        Vector3 dir = lookTarget - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance || !GameManager.Instance.IsTrialRunning)
            return;

        if (_state != TargetState.IdleBlue)
            return;

        if (!other.CompareTag("Blade"))
            return;

        GameManager.Instance.RegisterValidHit();
        EnterHitGreenState();
    }
}