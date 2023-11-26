using DaiMangou.ProRadarBuilder;
using UnityEngine;

public class LerpValue : MonoBehaviour
{
    public _2DRadar _2DRadar_;

    public _3DRadar _3DRadar_;

    [TextArea(10, 100)] public string Info = " ";

    public float min = 170, max = 800;
    private bool scaleUp;

    public bool start;
    private float t;

    public void AutoToggle(int i)
    {
        start = !start;
    }

    private void Update()
    {
        if (!start) return;
        if (_3DRadar_)
        {
            if (_3DRadar_.RadarDesign.SceneScale == min)
            {
                scaleUp = true;
                t = 0;
            }
            if (_3DRadar_.RadarDesign.SceneScale == max)
            {
                scaleUp = false;
                t = 0;
            }
            t += 0.15f * Time.deltaTime;
            _3DRadar_.RadarDesign.SceneScale = scaleUp ? Mathf.Lerp(min, max, t) : Mathf.Lerp(max, min, t);
        }
        if (!_2DRadar_) return;
        if (_2DRadar_.RadarDesign.SceneScale == min)
        {
            scaleUp = true;
            t = 0;
        }
        if (_2DRadar_.RadarDesign.SceneScale == max)
        {
            scaleUp = false;
            t = 0;
        }
        t += 0.15f * Time.deltaTime;
        _2DRadar_.RadarDesign.SceneScale = scaleUp ? Mathf.Lerp(min, max, t) : Mathf.Lerp(max, min, t);
    }
}