using System.Collections.Generic;
using UnityEngine;

public class UIPerkSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject perkPanel;
    [SerializeField] private UIPerk perk1;
    [SerializeField] private UIPerk perk2;
    [SerializeField] private UIPerk perk3;

    private List<PerkType> allPerks = new List<PerkType>()
    {
        PerkType.Mushroom,
        PerkType.Meat,
        PerkType.Crown,
        PerkType.RedPotion,
        PerkType.Gasoline
    };

    private void Awake()
    {
        PlayerStats.onLevelUp += PlayerStats_onLevelUp;
    }

    private void OnDestroy()
    {
        PlayerStats.onLevelUp -= PlayerStats_onLevelUp;
    }

    private void PlayerStats_onLevelUp()
    {
        gameManager.PauseGame();
        perkPanel.SetActive(true);
        ShowPerks();
    }

    private void ShowPerks()
    {
        List<PerkType> randomPerks = GetRandomPerks(3);

        perk1.Setup(randomPerks[0]);
        perk2.Setup(randomPerks[1]);
        perk3.Setup(randomPerks[2]);
    }

    List<PerkType> GetRandomPerks(int amount)
    {
        List<PerkType> copy = new List<PerkType>(allPerks);
        List<PerkType> result = new List<PerkType>();

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }

    public void OnPerkSelected(PerkType perk)
    {
        PlayerStats.Instance.ApplyPerk(perk);
        gameManager.PauseGame();
        perkPanel.SetActive(false);
    }
}