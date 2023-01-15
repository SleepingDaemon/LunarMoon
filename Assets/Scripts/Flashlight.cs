using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light flashLight = null;
    [SerializeField] private bool isOn = true;
    [SerializeField] private GameObject helmetCap = null;

    private SkinnedMeshRenderer _helmetRend = null;
    private Material _lightMat = null;

    private void Start()
    {
        _helmetRend = helmetCap.GetComponent<SkinnedMeshRenderer>();
        _lightMat = _helmetRend.materials[1];
        _lightMat.SetFloat("_EmissiveExposureWeight", 1);
    }

    private void Update()
    {
        if (isOn)
        {
            flashLight.enabled = true;
            _lightMat.SetFloat("_EmissiveExposureWeight", 0);
        }
        else
        {
            flashLight.enabled = false;
            _lightMat.SetFloat("_EmissiveExposureWeight", 1);
        }
    }

    private void OnFlashlight(InputValue value)
    {
        if (value.isPressed)
            isOn = !isOn;
    }
}
