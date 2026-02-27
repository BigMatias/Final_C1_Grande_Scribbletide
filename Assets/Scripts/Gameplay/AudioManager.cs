using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponDataSo weaponDataSo;
    [SerializeField] private EnemyDataSo enemyDataSo;
    [Header("AudioClips")]
    [SerializeField] private AudioClip[] music;
    [SerializeField] private AudioClip buttonClickedAudio;

    [Header("AudioSources")]
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceSfx;

    private int lastMusicIndex = -1;

    private void Awake()
    {
        PlayerMovement.onPlayerDied += PlayerMovement_playerDied;
        UIButton.onButtonClicked += UIButton_onButtonClicked;
        PlayerWeaponCollider.onAttackBlocked += PlayerWeaponCollider_onAttackBlocked;
        EnemyController.onEnemyDie += EnemyController_onEnemyDie;
        PlayerWeaponCollider.onEnemyHit += PlayerWeaponCollider_onEnemyHit;
    }

    private void Start()
    {
        PlayRandomMusic();
    }

    private void Update()
    {
        if (!audioSourceMusic.isPlaying)
        {
            PlayRandomMusic();
        }
    }
    private void OnDestroy()
    {
        PlayerMovement.onPlayerDied -= PlayerMovement_playerDied;
        UIButton.onButtonClicked -= UIButton_onButtonClicked;
        PlayerWeaponCollider.onAttackBlocked -= PlayerWeaponCollider_onAttackBlocked;
        EnemyController.onEnemyDie -= EnemyController_onEnemyDie;
        PlayerWeaponCollider.onEnemyHit -= PlayerWeaponCollider_onEnemyHit;
    }
    private void PlayerWeaponCollider_onEnemyHit()
    {
        audioSourceSfx.PlayOneShot(weaponDataSo.WeaponHitSound);

    }

    private void EnemyController_onEnemyDie()
    {
        audioSourceSfx.PlayOneShot(enemyDataSo.EnemyDeadAudio);
    }

    private void PlayerWeaponCollider_onAttackBlocked()
    {
        audioSourceSfx.PlayOneShot(weaponDataSo.WeaponBlockSound);
    }

    private void PlayRandomMusic()
    {
        if (music.Length == 0) return;

        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, music.Length);
        }
        while (randomIndex == lastMusicIndex && music.Length > 1);

        lastMusicIndex = randomIndex;

        audioSourceMusic.clip = music[randomIndex];
        audioSourceMusic.Play();
    }

    private void UIButton_onButtonClicked()
    {
        audioSourceSfx.PlayOneShot(buttonClickedAudio);
    }

    private void PlayerMovement_playerDied()
    {
        audioSourceMusic.Stop();
    }

    public void ReproduceClip(AudioClip audioClip)
    {
        audioSourceSfx.PlayOneShot(audioClip);
    }
}