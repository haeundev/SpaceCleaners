using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCraftingReady : MonoBehaviour
{
    private bool isSpotTaken = false;

    private string spotHolderTag = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter(Collider other) //collider 만나면 가운데로 위치 / 회전하게
    {
        // if (other.gameObject.tag != "Untagged" && !isSpotTaken && spotHolderTag == "")
        // {
        //     print("othertag: "+other.gameObject.tag);
        //     OuterSpaceEvent.Trigger_CraftingReady(other.gameObject.tag, true);
        //     spotHolderTag = other.gameObject.tag;
        //     isSpotTaken = true;
        // }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Untagged" && !isSpotTaken && spotHolderTag == "")
        {
            print("stayothertag: "+other.gameObject.tag);
            OuterSpaceEvent.Trigger_CraftingReady(other.gameObject.tag, true);
            spotHolderTag = other.gameObject.tag;
            isSpotTaken = true;
        }
    }

    void OnTriggerExit(Collider other) //collider 만나면 가운데로 위치 / 회전하게
    {
        if (other.gameObject.tag != "Untagged" && isSpotTaken && spotHolderTag == other.gameObject.tag)
        {
            print("leaveothertag: "+other.gameObject.tag);
            OuterSpaceEvent.Trigger_CraftingReady(other.gameObject.tag, false);
            spotHolderTag = "";
            isSpotTaken = false;
        }
        
    }
}
