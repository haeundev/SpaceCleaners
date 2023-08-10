using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBarrelDestoyed : MonoBehaviour
{
    private GameObject jungleHUD;

    void Awake()
    {
        jungleHUD = GameObject.FindGameObjectWithTag("JungleHUD");
    }
    void OnDisable()
    {
        jungleHUD.GetComponent<JungleHUD>().OnOxygenLevelUpdated();
    }
}
