using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button[] mainMenuButtons;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject storyMenu;
    [SerializeField] private GameObject selectWeaponMenu;
    [SerializeField] private Button creditsBackBtn;
    [SerializeField] private Button storyBackBtn;

    private void Awake()
    {
        mainMenuButtons[0].onClick.AddListener(OnPlayBtnClicked);
        mainMenuButtons[1].onClick.AddListener(OnOptionsBtnClicked);
        mainMenuButtons[2].onClick.AddListener(OnCreditsBtnClicked);
        mainMenuButtons[3].onClick.AddListener(OnExitBtnClicked);
        mainMenuButtons[4].onClick.AddListener(OnStoryBtnClicked);
        creditsBackBtn.onClick.AddListener(OnCreditsBackBtnClicked);
        storyBackBtn.onClick.AddListener(OnStoryBackBtnClicked);
    }

    private void OnDestroy()
    {
        mainMenuButtons[0].onClick.RemoveListener(OnPlayBtnClicked);
        mainMenuButtons[1].onClick.RemoveListener(OnOptionsBtnClicked);
        mainMenuButtons[2].onClick.RemoveListener(OnCreditsBtnClicked);
        mainMenuButtons[3].onClick.RemoveListener(OnExitBtnClicked);
        mainMenuButtons[4].onClick.RemoveListener(OnStoryBtnClicked);
        creditsBackBtn.onClick.RemoveListener(OnCreditsBackBtnClicked);
        storyBackBtn.onClick.RemoveListener(OnStoryBackBtnClicked);
    }

    private void OnStoryBtnClicked()
    {
        mainMenuPanel.gameObject.SetActive(false);
        storyMenu.SetActive(true);
    }

    private void OnPlayBtnClicked()
    {
        selectWeaponMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnOptionsBtnClicked()
    {
        mainMenuPanel.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }

    private void OnCreditsBtnClicked()
    {
        mainMenuPanel.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(true);
    }
    private void OnCreditsBackBtnClicked()
    {
        creditsMenu.gameObject.SetActive(false);
        mainMenuPanel.gameObject.SetActive(true);
    }
    private void OnStoryBackBtnClicked()
    {
        storyMenu.gameObject.SetActive(false);
        mainMenuPanel.gameObject.SetActive(true);
    }

    private void OnExitBtnClicked()
    {
        //Sale del estado "Play" del editor si estamos en el editor, de lo contrario sale de la aplicación si esta es una build.  
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
