using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class JungleItemSpawner : MonoBehaviour
{
    [SerializeField] private string blueBugPath = "Prefabs/Jungle/BlueBug.prefab";
    [SerializeField] private string oxygenBarrelPath = "Prefabs/Jungle/OxygenBarrel.prefab";
    [SerializeField] private string pinkLeafPath = "Prefabs/Jungle/PinkLeaf.prefab";
    [SerializeField] private string redFlowerPath = "Prefabs/Jungle/RedFlower.prefab";

    [SerializeField] private Transform parentTransform;

    private void Awake()
    {
        JungleEvents.OnPlantGrowDone += OnPlantGrowDone;
    }

    private void OnPlantGrowDone(GameObject plantObj)
    {
        if (plantObj == gameObject)
        {
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
                path = redFlowerPath;
                break;
            case PlantType.Bug:
                path = blueBugPath;
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








public class PlantSoundPlayer : MonoBehaviour
{
    [SerializeField] private string sfxPath = "PlantGrow.wav";

    private void Awake()
    {
        JungleEvents.OnPlantGrowDone += OnPlantGrowDone;
    }

    private void OnPlantGrowDone(GameObject plantObj)
    {
        SoundService.PlaySfx(sfxPath, plantObj.transform.position);
    }
}