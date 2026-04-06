using UnityEngine;

public class BladeReactiveTarget : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color hitColor = Color.red;

    private Renderer _renderer;
    private Material _material;
    private int _bladeContacts = 0;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();

        if (_renderer == null)
        {
            Debug.LogError($"BladeReactiveTarget on {name}: no Renderer found.");
            return;
        }

        // Creates an instance of the material for this object
        _material = _renderer.material;
        _material.color = idleColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Blade"))
            return;

        _bladeContacts++;
        UpdateColor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Blade"))
            return;

        _bladeContacts = Mathf.Max(0, _bladeContacts - 1);
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_material == null)
            return;

        _material.color = _bladeContacts > 0 ? hitColor : idleColor;
    }
}