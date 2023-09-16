using System;
using System.Collections.Generic;
using System.Linq;
using DataTables;
using LiveLarson.DataTableManagement;
using LiveLarson.Enums;
using LiveLarson.SoundSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashboardGadget : MonoBehaviour
{
    [SerializeField] private Transform previewParent;
    [SerializeField] private Transform gadgetParent;
    [SerializeField] private TextMeshProUGUI gadgetName;
    [SerializeField] private TextMeshProUGUI gadgetDescription;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject checkIcon;
    [SerializeField] private GameObject netPrefab;
    [SerializeField] private GameObject netPreviewPrefab;
    [SerializeField] private string sfxNetShoot = "Assets/Audio/Net Shoot.wav";

    //private List<GadgetInfo> _gadgetInfos;

    //private GadgetStat _gadgetStat;
    private int _currentID;
    private GameObject _currentTargetDebris;

    private void Awake()
    {
        //_gadgetInfos = DataTableManager.GadgetInfos.Values;
        //_gadgetStat = SaveAndLoadManager.Instance.GadgetStat;

        selectButton.onClick.AddListener(OnClickSelectButton);
        OuterSpaceEvent.OnGadgetShoot += OnGadgetShoot;
    }

    // private void RegisterEvents()
    // {
    //     //prevButton.onClick.AddListener(OnClickPrevButton);
    //     //nextButton.onClick.AddListener(OnClickNextButton);
    //  
    // }

    private void OnDestroy()
    {
        OuterSpaceEvent.OnGadgetShoot -= OnGadgetShoot;
    }

    private void OnGadgetShoot(GameObject targetDebris)
    {
        _currentTargetDebris = targetDebris;
        SoundService.PlaySfx(sfxNetShoot, transform.position);
    }

    // [Button]
    // private void OnClickPrevButton()
    // {
    //     var ownedGadgetIDs = _gadgetStat.Gadgets.Keys.ToList();
    //     var indexForCurrent = ownedGadgetIDs.IndexOf(_currentID);
    //     var indexForPrev = indexForCurrent < 1 ? ownedGadgetIDs.Count - 1 : indexForCurrent - 1;
    //     var prevGadgetInfo = _gadgetInfos.FirstOrDefault(p => p.ID == ownedGadgetIDs[indexForPrev]);
    //     Display(prevGadgetInfo);
    // }
    //
    // [Button]
    // private void OnClickNextButton()
    // {
    //     var ownedGadgetIDs = _gadgetStat.Gadgets.Keys.ToList();
    //     var indexForCurrent = ownedGadgetIDs.IndexOf(_currentID);
    //     var indexForNext = indexForCurrent + 1 == _gadgetStat.Gadgets.Count ? 0 : indexForCurrent + 1;
    //     var nextGadgetInfo = _gadgetInfos.FirstOrDefault(p => p.ID == ownedGadgetIDs[indexForNext]);
    //     Display(nextGadgetInfo);
    // }

    [Button]
    private void OnClickSelectButton()
    {
        // var gadgetInfo = DataTableManager.GadgetInfos.Find(_currentID);
        // if (gadgetInfo == default)
        //     return;

        var gadgetInfo = new GadgetInfo()
        {
            ID = 1001,
            GadgetType = GadgetType.Net,
            Name = "그물",
            Description = "잔해를 향해 그물을 발사하세요.\n- 중간 크기 이상의 잔해에 적합합니다.",
            PreviewPath = "Prefabs/NetDisplay.prefab",
            ModelPath = "Prefabs/Net.prefab",
            HasFromStart = true
        };
        
        //_gadgetStat.lastSelectedID = _currentID;
        checkIcon.gameObject.SetActive(true);
        OuterSpaceEvent.Trigger_GadgetSelected(gadgetInfo);

        for (var i = 0; i < gadgetParent.childCount; i++)
            Destroy(gadgetParent.GetChild(i).gameObject);
        var net = Instantiate(netPrefab);
        net.transform.position = gadgetParent.position;
        net.transform.forward = gadgetParent.forward;
        net.transform.SetParent(GameObject.FindWithTag("Re-positionableHandle").transform, true);
        var gadget = net.GetComponent<Gadget>();
        if (_currentTargetDebris != default) gadget.targetTransform = _currentTargetDebris.transform;
        gadget.Init();
        
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InitFirstDisplay();
    }

    private void InitFirstDisplay()
    {
        //var prevSelectedID = _gadgetStat.lastSelectedID;
        //var prevSelected = _gadgetInfos.FirstOrDefault(p => p.ID == prevSelectedID);
        //Display(prevSelected != default ? DataTableManager.GadgetInfos.Find(prevSelectedID) : _gadgetInfos.First());
        // Display(_gadgetInfos.First());
        Display(new GadgetInfo()
        {
            ID = 1001,
            GadgetType = GadgetType.Net,
            Name = "그물",
            Description = "잔해를 향해 그물을 발사하세요.\n- 중간 크기 이상의 잔해에 적합합니다.",
            PreviewPath = "Prefabs/NetDisplay.prefab",
            ModelPath = "Prefabs/Net.prefab",
            HasFromStart = true
        });
    }

    private void Display(GadgetInfo gadgetInfo)
    {
        // if (gadgetInfo.ID == _gadgetStat.lastSelectedID)
        // {
        //     //checkIcon.SetActive(true);
        //     selectButton.GetComponentInChildren<TextMeshProUGUI>().SetText("장착됨");
        // }
        // else
        // {
        //     selectButton.GetComponentInChildren<TextMeshProUGUI>().SetText("장착하기");
        // }
        _currentID = gadgetInfo.ID;

        // 3D model
        for (var i = 0; i < previewParent.childCount; i++)
            Destroy(previewParent.GetChild(i).gameObject);
        Instantiate(netPreviewPrefab, previewParent);
        // Addressables.InstantiateAsync(gadgetInfo.PreviewPath, previewParent).Completed += op =>
        // {
        //     // var go = op.Result;
        //     // go.transform.localScale *= 0.2f;
        // };

        gadgetName.text = gadgetInfo.Name;
        gadgetDescription.text = gadgetInfo.Description;
    }

    // private void OnDisable()
    // {
    //     SaveAndLoadManager.Instance.Save(_gadgetStat);
    // }
}