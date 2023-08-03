using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///     Calls functionality when a physics trigger occurs
/// </summary>
public class OnTriggerWatered : MonoBehaviour
{
    [Serializable]
    public class TriggerEvent : UnityEvent<GameObject>
    {
    }

    [SerializeField] [Tooltip("If set, this trigger will only fire if the other gameobject has this tag.")]
    private string m_RequiredTag = string.Empty;

    [SerializeField] [Tooltip("Events to fire when a matcing object collides with this trigger.")]
    private TriggerEvent m_OnEnter = new();

    [SerializeField] [Tooltip("Events to fire when a matching object stops colliding with this trigger.")]
    private TriggerEvent m_OnExit = new();

    /// <summary>
    ///     If set, this trigger will only fire if the other gameobject has this tag.
    /// </summary>
    public string requiredTag => m_RequiredTag;

    /// <summary>
    ///     Events to fire when a matching object collides with this trigger.
    /// </summary>
    public TriggerEvent onEnter => m_OnEnter;

    /// <summary>
    ///     Events to fire when a matching object stops colliding with this trigger.
    /// </summary>
    public TriggerEvent onExit => m_OnExit;

    
    // public PlantType spawnIngredient;

    public GameObject[] ingredients;

    private bool isIngredientSpawned;

    public enum Level
    {
        Easy,
        Hard
    }

    public Level plantLevel;

    public Animator myAnimator;
    public RuntimeAnimatorController[] animators;

    private bool isAnimatorEnabled;
    private bool isFullyGrown;
    private string lastAnimationName = "Grow";

    private int dropCount;
    private static readonly int Grow = Animator.StringToHash("grow");

    private void Awake()
    {
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        m_OnEnter.AddListener(_ => gameObject.GetComponentInChildren<Animator>().enabled = true);
        
        // m_OnEnter.AddListener(EnableAnimator);
    }

    // private void EnableAnimator(GameObject _)
    // {
    //     gameObject.GetComponentInChildren<Animator>().enabled = true;
    // }

    private void Start()
    {
        if (plantLevel == Level.Easy)
            myAnimator.runtimeAnimatorController = animators[0];
        else if (plantLevel == Level.Hard) myAnimator.runtimeAnimatorController = animators[1];
    }

    // private void Update()
    // {
    //     // if (isFullyGrown && !isIngredientSpawned &&
    //     //     myAnimator.GetCurrentAnimatorStateInfo(0).IsName(lastAnimationName) &&
    //     //     myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0))
    //     // {
    //     //     ingredients[(int)spawnIngredient].SetActive(true);
    //     //     isIngredientSpawned = true;
    //     // }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (CanTrigger(other.gameObject))
            m_OnEnter?.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (CanTrigger(other.gameObject))
            m_OnExit?.Invoke(other.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (CanTrigger(other.gameObject) && !isFullyGrown)
        {
            if (!isAnimatorEnabled)
            {
                m_OnEnter?.Invoke(other);
                isAnimatorEnabled = true;
            }

            if (plantLevel == Level.Easy)
            {
                lastAnimationName = "Grow";
                isFullyGrown = true;
            }
            else if (plantLevel == Level.Hard)
            {
                dropCount++;
                if (dropCount == 200)
                {
                    // gameObject.GetComponent<JungleItemSpawner>()?.Spawn();
                    myAnimator.SetTrigger(Grow);
                    WaitAndTriggerPlantDone();

                    lastAnimationName = "grow3";
                    isFullyGrown = true;
                    // dropCount = 0;
                }
                else if (dropCount == 100)
                {
                    myAnimator.SetTrigger(Grow);
                }
            }
        }
    }

    private void WaitAndTriggerPlantDone()
    {
        var animLength = myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Observable.Timer(TimeSpan.FromSeconds(animLength)).Subscribe(_ =>
        {
            JungleEvents.Trigger_PlantGrowDone(gameObject);
        });
    }

    private bool CanTrigger(GameObject otherGameObject)
    {
        if (m_RequiredTag != string.Empty)
            return otherGameObject.CompareTag(m_RequiredTag);
        return true;
    }
}