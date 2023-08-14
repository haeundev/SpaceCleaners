using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class JungleItemSpawner : MonoBehaviour
{
    [SerializeField] private string purpleBugPath = "Prefabs/Jungle/PurpleBug.prefab";
    [SerializeField] private string oxygenBarrelPath = "Prefabs/Jungle/OxygenBarrel.prefab";
    [SerializeField] private string pinkLeafPath = "Prefabs/Jungle/PinkLeaf.prefab";
    [SerializeField] private string blueFlowerPath = "Prefabs/Jungle/BlueFlower.prefab";

    [SerializeField] private Transform parentTransform;

    [SerializeField] private AudioClip clip;
    // private string itemSpawnSFX = "Assets/Audio/ItemSpawn.mp3";
    private void Awake()
    {
        JungleEvents.OnPlantGrowDone += OnPlantGrowDone;
    }

    private void OnPlantGrowDone(GameObject plantObj)
    {
        if (plantObj == gameObject)
        {
            // SoundService.PlaySfx(itemSpawnSFX, parentTransform.transform.position);
            // _audioSource.clip = clip;
            // _audioSource.Play();
            AudiosourceManager.instance.PlayClip(clip);
            
            
            Spawn();
            
            
        }
    }

    public void Spawn()
    {
        var path = "";
        switch (GetComponent<JunglePlant>().plantType)
        {
            case PlantType.Leaf:
                path = pinkLeafPath;
                break;
            case PlantType.Flower:
                path = blueFlowerPath;
                break;
            case PlantType.Bug:
                path = purpleBugPath;
                break;
            case PlantType.Oxygen:
                path = oxygenBarrelPath;
                break;
        }

        Addressables.LoadAssetAsync<GameObject>(path).Completed += op =>
        {
            var go = op.Result;
            var temp = Instantiate(go, parentTransform);
            temp.GetComponent<Floater>().enabled = true;
        };
    }
}

// public class PlantSoundPlayer : MonoBehaviour
// {
//     [SerializeField] private string sfxPath = "PlantGrow.wav";
//
//     private void Awake()
//     {
//         JungleEvents.OnPlantGrowDone += OnPlantGrowDone;
//     }
//
//     private void OnPlantGrowDone(GameObject plantObj)
//     {
//         SoundService.PlaySfx(sfxPath, plantObj.transform.position);
//     }
// }