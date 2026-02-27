using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OptionsMenu;
    [SerializeField] private GameDataSo gameDataSO;

    private bool gamePaused = false;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        gameDataSO.GameTimer = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused)
            {
                PauseGame();
                PauseMenu.gameObject.SetActive(true);
                OptionsMenu.gameObject.SetActive(false);
            }
            else
            {
                PauseGame();
                PauseMenu.gameObject.SetActive(false);
                OptionsMenu.gameObject.SetActive(false);
            }
        }

        if (!gamePaused)
        {
            gameDataSO.GameTimer += Time.deltaTime;
        }
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0f;
            gamePaused = true;

        }
        else
        {
            Time.timeScale = 1f;
            gamePaused = false;
        }
    }

}
