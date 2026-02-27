using UnityEngine;
using UnityEngine.UI;

public class UiExpBar : MonoBehaviour
{
    [SerializeField] private Image barExp;
    [SerializeField] private PlayerStatsSo playerStatsSO;
    [SerializeField] private PlayerMovement player;

    private void Awake()
    {
        player.onExpObtained += Player_onExpObtained;
    }

    private void Start()
    {
        barExp.fillAmount = playerStatsSO.CurrentExperience;
    }

    private void OnDestroy()
    {
        player.onExpObtained -= Player_onExpObtained;
    }

    private void Player_onExpObtained()
    {
        float lerp = playerStatsSO.CurrentExperience / playerStatsSO.ExpNeededForNextLvl;
        barExp.fillAmount = lerp;
        if (playerStatsSO.CurrentExperience == playerStatsSO.ExpNeededForNextLvl)
        {
            barExp.fillAmount = 0;
        }
        
    }
}
