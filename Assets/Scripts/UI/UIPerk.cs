using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPerk : MonoBehaviour
{
    private PerkType perkType;
    private UIPerkSelectionManager manager;

    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Sprite mushroomSprite;
    [SerializeField] private Sprite meatSprite;
    [SerializeField] private Sprite crownSprite;
    [SerializeField] private Sprite redPotionSprite;
    [SerializeField] private Sprite gasolineSprite;

    private void Awake()
    {
        manager = GetComponentInParent<UIPerkSelectionManager>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Setup(PerkType type)
    {
        perkType = type;
        gameObject.SetActive(true);

        switch (type)
        {
            case PerkType.Mushroom:
                titleText.text = "Mushroom";
                descriptionText.text = "+1 Attack Speed";
                iconImage.sprite = mushroomSprite;
                break;

            case PerkType.Meat:
                titleText.text = "Meat";
                descriptionText.text = "+1 Damage";
                iconImage.sprite = meatSprite;
                break;

            case PerkType.Crown:
                titleText.text = "Crown";
                descriptionText.text = "+1 XP Attraction Range";
                iconImage.sprite = crownSprite;
                break;

            case PerkType.RedPotion:
                titleText.text = "Red Potion";
                descriptionText.text = "+10 Max HP";
                iconImage.sprite = redPotionSprite;
                break;

            case PerkType.Gasoline:
                titleText.text = "Gasoline";
                descriptionText.text = "+1 Player Speed";
                iconImage.sprite = gasolineSprite;
                break;
        }
    }

    private void OnClick()
    {
        manager.OnPerkSelected(perkType);
    }
}