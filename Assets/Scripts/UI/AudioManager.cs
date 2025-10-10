using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    [SerializeField] private AudioClip playerHit;
    [SerializeField] private AudioClip playerMelee;
    [SerializeField] private AudioClip playerBow;
    [SerializeField] private AudioClip playerNetThrow;
    [SerializeField] private AudioClip playerBlock;
    [SerializeField] private AudioClip playerThrow;
    [SerializeField] private AudioClip playerWalk;
    [SerializeField] private AudioClip playerDash;
    [SerializeField] private AudioClip playerRage;
    [SerializeField] private AudioClip playerPotionUse;
    [SerializeField] private AudioClip playerPickUp;

    [SerializeField] private AudioClip enemyHit;
    [SerializeField] private AudioClip enemyAttack;
    [SerializeField] private AudioClip enemyShoot;
    [SerializeField] private AudioClip enemyCast;
    [SerializeField] private AudioClip enemySing;
    [SerializeField] private AudioClip stoneGolemJump;
    [SerializeField] private AudioClip minotaurRoar;
    [SerializeField] private AudioClip griffonScreech;
    [SerializeField] private AudioClip zeusThrow;
    [SerializeField] private AudioClip zeusThunderStrike;

    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip buttonSelect;

    [SerializeField] private AudioClip bgmMainMenu;
    [SerializeField] private AudioClip bgmColosseum;
    [SerializeField] private AudioClip bgmForest;
    [SerializeField] private AudioClip bgmPlatform;
    [SerializeField] private AudioClip bgmOlympus;

    public void PlayPlayerHit()
    {
        sfxAudioSource.PlayOneShot(playerHit);
        Debug.Log("Player Hit"); // DEBUGGING
    }

    public void PlayPlayerMelee()
    {
        sfxAudioSource.PlayOneShot(playerMelee);
        Debug.Log("Player Melee"); // DEBUGGING
    }

    public void PlayPlayerBow()
    {
        sfxAudioSource.PlayOneShot(playerBow);
        Debug.Log("Player Bow"); // DEBUGGING
    }

    public void PlayPlayerNetThrow()
    {
        sfxAudioSource.PlayOneShot(playerNetThrow);
        Debug.Log("Player Net Throw"); // DEBUGGING
    }

    public void PlayPlayerBlock()
    {
        sfxAudioSource.PlayOneShot(playerBlock);
        Debug.Log("Player Block"); // DEBUGGING
    }

    public void PlayPlayerThrow()
    {
        sfxAudioSource.PlayOneShot(playerThrow);
        Debug.Log("Player Throw"); // DEBUGGING
    }

    public void PlayPlayerWalk()
    {
        sfxAudioSource.PlayOneShot(playerWalk);
        Debug.Log("Player Walk"); // DEBUGGING
    }

    public void PlayPlayerDash()
    {
        sfxAudioSource.PlayOneShot(playerDash);
        Debug.Log("Player Dash"); // DEBUGGING
    }

    public void PlayPlayerRage()
    {
        sfxAudioSource.PlayOneShot(playerRage);
        Debug.Log("Player Rage"); // DEBUGGING
    }

    public void PlayPlayerPotionUse()
    {
        sfxAudioSource.PlayOneShot(playerPotionUse);
        Debug.Log("Player Potion Use"); // DEBUGGING
    }

    public void PlayPlayerPickUp()
    {
        sfxAudioSource.PlayOneShot(playerPickUp);
        Debug.Log("Player Pick Up"); // DEBUGGING
    }

    public void PlayEnemyHit()
    {
        sfxAudioSource.PlayOneShot(enemyHit);
        Debug.Log("Enemy Hit"); // DEBUGGING
    }

    public void PlayEnemyAttack()
    {
        sfxAudioSource.PlayOneShot(enemyAttack);
        Debug.Log("Enemy Melee"); // DEBUGGING
    }

    public void PlayEnemyShoot()
    {
        sfxAudioSource.PlayOneShot(enemyShoot);
        Debug.Log("Enemy Shoot"); // DEBUGGING
    }

    public void PlayEnemyCast()
    {
        sfxAudioSource.PlayOneShot(enemyCast);
        Debug.Log("Enemy Cast"); // DEBUGGING
    }

    public void PlayEnemySing()
    {
        sfxAudioSource.PlayOneShot(enemySing);
        Debug.Log("Enemy Sing"); // DEBUGGING
    }

    public void PlayStoneGolemJump()
    {
        sfxAudioSource.PlayOneShot(enemySing);
        Debug.Log("Stone Golem Jump"); // DEBUGGING
    }

    public void PlayMinotaurRoar()
    {
        sfxAudioSource.PlayOneShot(minotaurRoar);
        Debug.Log("Minotaur Roar"); // DEBUGGING
    }

    public void PlayGriffonScreech()
    {
        sfxAudioSource.PlayOneShot(minotaurRoar);
        Debug.Log("Griffon Screech"); // DEBUGGING
    }

    public void PlayZeusThrow()
    {
        sfxAudioSource.PlayOneShot(zeusThrow);
        Debug.Log("Zeus Throw"); // DEBUGGING
    }

    public void PlayZeusThunderStrike()
    {
        sfxAudioSource.PlayOneShot(zeusThunderStrike);
        Debug.Log("Zeus Thunder Strike"); // DEBUGGING
    }

    public void PlayButtonHover()
    {
        sfxAudioSource.PlayOneShot(buttonHover);
        Debug.Log("Button Hover"); // DEBUGGING
    }

    public void PlayButtonSelect()
    {
        sfxAudioSource.PlayOneShot(buttonSelect);
        Debug.Log("Button Select"); // DEBUGGING
    }

    public void PlayBGMMainMenu()
    {
        musicAudioSource.PlayOneShot(bgmMainMenu);
        Debug.Log("BGM Main Menu"); // DEBUGGING
    }

    public void PlayBGMColosseum()
    {
        musicAudioSource.PlayOneShot(bgmColosseum);
        Debug.Log("BGM Colosseum"); // DEBUGGING
    }

    public void PlayBGMForest()
    {
        musicAudioSource.PlayOneShot(bgmForest);
        Debug.Log("BGM Forest"); // DEBUGGING
    }

    public void PlayBGMPlatform()
    {
        musicAudioSource.PlayOneShot(bgmPlatform);
        Debug.Log("BGM Platform"); // DEBUGGING
    }

    public void PlayBGMOlympus()
    {
        musicAudioSource.PlayOneShot(bgmOlympus);
        Debug.Log("BGM Olympus"); // DEBUGGING
    }
}
