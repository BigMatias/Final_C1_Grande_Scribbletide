using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    [SerializeField] private ExperienceGemState gemState;
    [SerializeField] private float gemAttractSpeed;

    public ExperienceType Type;
    private float gemExpGiven;
    private Animator animator;
    private Transform target;
    private bool isBeingAttracted = false;

    private static readonly int State = Animator.StringToHash("State");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gemState = ExperienceGemState.Idle;
        animator.SetInteger(State, (int)gemState);
    }

    private void Update()
    {
        if (isBeingAttracted)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, gemAttractSpeed * Time.deltaTime);
        }
    }

    public void Initialize(ExperienceType type, float value)
    {
        this.Type = type;
        this.gemExpGiven = value;
    }

    public float GetValue() => gemExpGiven;

    public void GemPickedUp()
    {
        gemState = ExperienceGemState.PickedUp;
        animator.SetInteger(State, (int)gemState);
    }

    public void PickedUpAnimFinished()
    {
        gameObject.SetActive(false);
        ExperiencePool.Instance.ReturnExperience(this);
    }
    public void StartAttracting(Transform player)
    {
        target = player;
        isBeingAttracted = true;
    }

}