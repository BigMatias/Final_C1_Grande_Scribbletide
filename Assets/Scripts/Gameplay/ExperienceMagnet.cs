using UnityEngine;

public class ExperienceMagnet : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerStatsSo playerStatsSO;

    private CircleCollider2D collider;

    private void Awake()
    {
        PlayerStats.onMagnetUpgraded += PlayerStats_onMagnetUpgraded;
        collider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        collider.radius = playerStatsSO.BaseExpMagnetRadius;
    }

    private void OnDestroy()
    {
        PlayerStats.onMagnetUpgraded -= PlayerStats_onMagnetUpgraded;
    }

    private void PlayerStats_onMagnetUpgraded()
    {
        collider.radius = playerStatsSO.CurrentExpMagnetRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)Layers.Experience)
        {
            ExperienceGem gem = collision.GetComponent<ExperienceGem>();

            if (gem != null)
            {
                gem.StartAttracting(playerTransform);
            }
        }
    }
}