using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableHelper
{
    public static void LoadGameObject(string addressableName, Action<GameObject> onLoading)
    {
        Addressables.LoadAssetAsync<GameObject>(addressableName).Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded) onLoading?.Invoke(op.Result);
            //_addressableHandle = op;
        };
    }
}