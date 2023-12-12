using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputData : MonoBehaviour
{
    public enum InputType
    {
        LeftController,
        RightController
    }

    private InputDevice _leftController;
    private InputDevice _rightController;
    // private InputDevice _hmd;


    public void GetValue(InputType inputType, out Vector2 value)
    {
        InitializeInputDevices();
        value = Vector2.zero;
        switch (inputType)
        {
            case InputType.LeftController:
                _leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out value);
                break;
            case InputType.RightController:
                _rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out value);
                break;
        }
    }

    private void Update()
    {
        if (!_leftController.isValid || !_rightController.isValid)
            InitializeInputDevices();
    }

    private void InitializeInputDevices()
    {
        if (!_leftController.isValid)
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left,
                ref _leftController);
        if (!_rightController.isValid)
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right,
                ref _rightController);
        // if (!_hmd.isValid)
        //     InitializeInputDevice(InputDeviceCharacteristics.HeadMounted, ref _hmd);
    }

    private void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice)
    {
        var devices = new List<InputDevice>();
        //Call InputDevices to see if it can find any devices with the characteristics we're looking for
        InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);

        //Our hands might not be active and so they will not be generated from the search.
        //We check if any devices are found here to avoid errors.
        if (devices.Count > 0) inputDevice = devices[0];
    }
}