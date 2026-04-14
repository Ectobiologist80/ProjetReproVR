using UnityEngine;

public class StartButtonTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameSessionUI gameSessionUI;
    [SerializeField] private Renderer buttonRenderer;

    [Header("Visual Feedback")]
    [SerializeField] private Color idleColor = Color.green;
    [SerializeField] private Color pressedColor = Color.yellow;
    [SerializeField] private Color disabledColor = Color.gray;
    [SerializeField] private float pressedColorDuration = 0.2f;

    [Header("Valid Tags")]
    [SerializeField] private string[] validTags = { "Blade" };

    private Material _material;
    private float _pressedTimer = 0f;
    private bool _isFlashing = false;

    private void Awake()
    {
        if (buttonRenderer == null)
        {
            buttonRenderer = GetComponent<Renderer>();
        }

        if (buttonRenderer != null)
        {
            _material = buttonRenderer.material;
        }

        RefreshVisualState();
    }

    private void Update()
    {
        if (_isFlashing)
        {
            _pressedTimer -= Time.deltaTime;

            if (_pressedTimer <= 0f)
            {
                _isFlashing = false;
                RefreshVisualState();
            }
        }
        else
        {
            RefreshVisualState();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasValidTag(other))
            return;

        if (gameSessionUI == null)
            return;

        bool accepted = gameSessionUI.RequestStartGame();

        if (accepted)
        {
            FlashPressedColor();
        }
    }

    private bool HasValidTag(Collider other)
    {
        foreach (string tagName in validTags)
        {
            if (other.CompareTag(tagName))
                return true;
        }

        return false;
    }

    private void FlashPressedColor()
    {
        _isFlashing = true;
        _pressedTimer = pressedColorDuration;

        if (_material != null)
        {
            _material.color = pressedColor;
        }
    }

    private void RefreshVisualState()
    {
        if (_material == null || gameSessionUI == null || _isFlashing)
            return;

        _material.color = gameSessionUI.IsStartInteractionLocked
            ? disabledColor
            : idleColor;
    }
}