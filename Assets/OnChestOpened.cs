using System;
using LiveLarson.SoundSystem;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.XR.Content.Interaction;

public class OnChestOpened : MonoBehaviour
{
    private OnTrigger _onTrigger;

    [SerializeField] private string chestBoxSFX = "Assets/Audio/loot-box.mp3";

    // Start is called before the first frame update
    private bool isChestOpened;

    public Animator myAnimator;
    [SerializeField] private string finalItemPath = "Prefabs/Jungle/FinalItemCrane.prefab";
    [SerializeField] public Transform finalItemSpot;

    private void Awake()
    {
        _onTrigger = GetComponent<OnTrigger>();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        _onTrigger.OnChestOpened += OnChestboxOpened;
    }

    private void OnChestboxOpened()
    {
        if (!isChestOpened)
        {
            SoundService.PlaySfx(chestBoxSFX, transform.position);
            WaitAndSpawnFinalItem();
            isChestOpened = true;
        }
    }

    private void WaitAndSpawnFinalItem()
    {
        var animLength = myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Observable.Timer(TimeSpan.FromSeconds(animLength)).Subscribe(_ =>
        {
            Addressables.LoadAssetAsync<GameObject>(finalItemPath).Completed += op =>
            {
                var go = op.Result;
                var temp = Instantiate(go, finalItemSpot);
                temp.GetComponent<Floater>().enabled = true;
            };
        });
    }
}