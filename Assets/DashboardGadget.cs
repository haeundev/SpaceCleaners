using System.Collections.Generic;
using System.Linq;
using DataTables;
using DevFeatures.SaveSystem;
using DevFeatures.SaveSystem.Model;
using LiveLarson.DataTableManagement;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class DashboardGadget : MonoBehaviour
{
    [SerializeField] private Transform objParent;
    [SerializeField] private TextMeshProUGUI gadgetName;
    [SerializeField] private TextMeshProUGUI gadgetDescription;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    private List<GadgetInfo> _gadgetInfos;
    private GadgetStat _gadgetStat;
    private int _currentID;
    
    private void Awake()
    {
        _gadgetInfos = DataTableManager.GadgetInfos.Values;
        _gadgetStat = SaveAndLoadManager.Instance.GadgetStat;

        RegisterEvents();
    }

    private void RegisterEvents()
    {
        prevButton.onClick.AddListener(OnClickPrevButton);
        nextButton.onClick.AddListener(OnClickNextButton);
    }

    [Button]
    private void OnClickPrevButton()
    {
        var ownedGadgetIDs = _gadgetStat.Gadgets.Keys.ToList();
        var indexForCurrent = ownedGadgetIDs.IndexOf(_currentID);
        var indexForPrev = indexForCurrent < 1 ? ownedGadgetIDs.Count - 1 : indexForCurrent - 1;
        var prevGadgetInfo = _gadgetInfos.FirstOrDefault(p => p.ID == ownedGadgetIDs[indexForPrev]);
        Display(prevGadgetInfo);
    }

    [Button]
    private void OnClickNextButton()
    {
        var ownedGadgetIDs = _gadgetStat.Gadgets.Keys.ToList();
        var indexForCurrent = ownedGadgetIDs.IndexOf(_currentID);
        var indexForNext = indexForCurrent + 1 == _gadgetStat.Gadgets.Count ? 1 : indexForCurrent + 1;
        var nextGadgetInfo = _gadgetInfos.FirstOrDefault(p => p.ID == ownedGadgetIDs[indexForNext]);
        Display(nextGadgetInfo);
    }

    private void OnEnable()
    {
        InitFirstDisplay();
    }

    private void InitFirstDisplay()
    {
        var prevSelectedID = _gadgetStat.lastSelectedID;
        var prevSelected = _gadgetInfos.FirstOrDefault(p => p.ID == prevSelectedID);
        Display(prevSelected != default ? DataTableManager.GadgetInfos.Find(prevSelectedID) : _gadgetInfos.First());
    }

    private void Display(GadgetInfo gadgetInfo)
    {
        _currentID = gadgetInfo.ID;

        // 3D model
        for (var i = 0; i < objParent.childCount; i++)
            Destroy(objParent.GetChild(i).gameObject);
        Addressables.InstantiateAsync(gadgetInfo.ModelPath, objParent).Completed += op =>
        {
            // var go = op.Result;
            // go.transform.localScale *= 0.2f;
        };

        gadgetName.text = gadgetInfo.Name;
        gadgetDescription.text = gadgetInfo.Description;
    }

    private void OnDisable()
    {
        _gadgetStat.lastSelectedID = _currentID;
        SaveAndLoadManager.Instance.Save(_gadgetStat);
    }
}