using System;
using UnityEngine;

public class JungleItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject purpleBug;
    [SerializeField] private GameObject oxygenBarrel;
    [SerializeField] private GameObject pinkLeaf;
    [SerializeField] private GameObject blueFlower;

    [SerializeField] private Transform parentTransform;

    [SerializeField] private AudioClip clip;

    // private string itemSpawnSFX = "Assets/Audio/ItemSpawn.mp3";
    private void Awake()
    {
        JungleEvents.OnPlantGrowDone += OnPlantGrowDone;
    }

    private void OnDestroy()
    {
        JungleEvents.OnPlantGrowDone -= OnPlantGrowDone;
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
        switch (GetComponent<JunglePlant>().plantType)
        {
            case PlantType.Leaf:
                var leaf = Instantiate(pinkLeaf, parentTransform);
                leaf.GetComponent<Floater>().enabled = true;
                break;
            case PlantType.Flower:
                var flower = Instantiate(blueFlower, parentTransform);
                flower.GetComponent<Floater>().enabled = true;
                break;
            case PlantType.Bug:
                var bug = Instantiate(purpleBug, parentTransform);
                bug.GetComponent<Floater>().enabled = true;
                break;
            case PlantType.Oxygen:
                var oxygen = Instantiate(oxygenBarrel, parentTransform);
                oxygen.GetComponent<Floater>().enabled = true;
                break;
        }
    }
}