using UnityEngine;
using DaiMangou.ProRadarBuilder;

public class UIPanControls : MonoBehaviour
{

    [TextArea(10, 100)]
    public string Info = " ";
    public _3DRadar _3DRadar_;
    public _2DRadar _2DRadar_;
    public Animator animator;
    public float PanSpeed;
    public enum Axis
    {
        x, y, z
    }
    public Axis axis = Axis.x;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Pressed"))
        {
            switch (axis)
            {
                case Axis.x:

                    if (_3DRadar_)
                        _3DRadar_.RadarDesign.Pan.x += PanSpeed * Time.deltaTime;

                    if (_2DRadar_)
                        _2DRadar_.RadarDesign.Pan.x += PanSpeed * Time.deltaTime;
                    break;

                case Axis.y:

                    if (_3DRadar_)
                        _3DRadar_.RadarDesign.Pan.y += PanSpeed * Time.deltaTime;

                    if (_2DRadar_)
                        _2DRadar_.RadarDesign.Pan.y += PanSpeed * Time.deltaTime;
                    break;
                case Axis.z:

                    if (_3DRadar_)
                        _3DRadar_.RadarDesign.Pan.z += PanSpeed * Time.deltaTime;

                    if (_2DRadar_)
                        _2DRadar_.RadarDesign.Pan.z += PanSpeed * Time.deltaTime;
                    break;
            }
        }

    }
}
