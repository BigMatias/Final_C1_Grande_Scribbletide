using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] private ExperienceGemState gemState;
    [SerializeField] private float attractSpeed;

    public PickableType Type;
    private int value;
    private Animator animator;
    private Transform target;
    private bool isBeingAttracted = false;

    private static readonly int State = Animator.StringToHash("State");

    private void Awake()
    {
        if (Type != PickableType.HealItem)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (Type != PickableType.HealItem)
        {
            gemState = ExperienceGemState.Idle;
            animator.SetInteger(State, (int)gemState);
        }
    }

    private void Update()
    {
        if (isBeingAttracted)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, attractSpeed * Time.deltaTime);
        }
    }

    public void Initialize(PickableType type, int value)
    {
        this.Type = type;
        this.value = value;
    }

    public int GetValue() => value;

    public void PickableCollected()
    {   
        if (Type == PickableType.HealItem)
        {
            gameObject.SetActive(false);
            PickablePool.Instance.ReturnPickable(this);
        }
        else
        {
            gemState = ExperienceGemState.PickedUp;
            animator.SetInteger(State, (int)gemState);
        }
    }

    public void PickedUpAnimFinished()
    {
        gameObject.SetActive(false);
        PickablePool.Instance.ReturnPickable(this);
    }

    public void StartAttracting(Transform player)
    {
        target = player;
        isBeingAttracted = true;
    }

}