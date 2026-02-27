using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISelectWeapon : MonoBehaviour
{
    [SerializeField] private WeaponDataSo weaponDataSO;
    [SerializeField] private Button longswordBtn;
    [SerializeField] private Button hammerBtn;
    [SerializeField] private Button daggerBtn;


    private void Awake()
    {
        longswordBtn.onClick.AddListener(OnLongswordBtnClicked);
        hammerBtn.onClick.AddListener(OnHammerBtnClicked);
        daggerBtn.onClick.AddListener(OnDaggerBtnClicked);
    }

    private void OnLongswordBtnClicked()
    {
        weaponDataSO.weaponId = "Longsword";
        SceneManager.LoadScene("Game");
    }

    private void OnHammerBtnClicked()
    {
        weaponDataSO.weaponId = "Hammer";
        SceneManager.LoadScene("Game");
    }

    private void OnDaggerBtnClicked()
    {
        weaponDataSO.weaponId = "Dagger";
        SceneManager.LoadScene("Game");
    }

}
