using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light flashLight;
    [SerializeField] private bool isOn = false;
    private InputManager _input;

    private void Start()
    {
        _input = GetComponentInParent<InputManager>();
    }

    private void Update()
    {
        if (isOn)
        {
            flashLight.enabled = true;
        }
        else
        {
            flashLight.enabled = false;
        }

        Toggle();
    }

    private void Toggle()
    {
        if(_input.FlashLight)
        {
            isOn = !isOn;
        }
    }
}
