using UnityEngine;
using UnityEngine.UI;

public class FocusGauge : MonoBehaviour
{
    [SerializeField] private float fullFocusDuration = 1f;
    private static FocusGauge _instance;
    private float _axisFloat;
    private static float _elapsedTime;
    private static bool _isActive;
    private Image _image;

    private void Start()
    {
        _instance = this;
        _image = GetComponent<Image>();
    }

    public static void OnFocusEnter()
    {
        if (_instance == default)
            return;
        _instance.FillSprite(0f);
        _elapsedTime = 0f;
        _isActive = true;
    }

    public static void OnFocusExit()
    {
        _isActive = false;
        _elapsedTime = 0f;
        _instance.FillSprite(0f);
    }

    private void Update()
    {
        if (_isActive == false)
        {
            _elapsedTime = 0f;
            _instance.FillSprite(0f);
            return;
        }

        if (_elapsedTime < fullFocusDuration)
        {
            _elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(_elapsedTime / fullFocusDuration);
            FillSprite(t);
        }
    }

    public void FillSprite(float f)
    {
        _image.fillAmount = f;
    }
}