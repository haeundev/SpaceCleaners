using UnityEngine;

public enum RecyclableType
{
    Food,
    Can,
    Bottle,
}

public class Recyclable : MonoBehaviour
{
    [SerializeField] private RecyclableType recyclableType;
    public RecyclableType RecyclableType => recyclableType;
}