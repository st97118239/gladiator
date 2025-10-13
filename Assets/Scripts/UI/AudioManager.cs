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
    }

    public void PlayPlayerMelee()
    {
        sfxAudioSource.PlayOneShot(playerMelee);
    }

    public void PlayPlayerBow()
    {
        sfxAudioSource.PlayOneShot(playerBow);
    }

    public void PlayPlayerNetThrow()
    {
        sfxAudioSource.PlayOneShot(playerNetThrow);
    }

    public void PlayPlayerBlock()
    {
        sfxAudioSource.PlayOneShot(playerBlock);
    }

    public void PlayPlayerThrow()
    {
        sfxAudioSource.PlayOneShot(playerThrow);
    }

    public void PlayPlayerWalk()
    {
        sfxAudioSource.PlayOneShot(playerWalk);
    }

    public void PlayPlayerDash()
    {
        sfxAudioSource.PlayOneShot(playerDash);
    }

    public void PlayPlayerRage()
    {
        sfxAudioSource.PlayOneShot(playerRage);
    }

    public void PlayPlayerPotionUse()
    {
        sfxAudioSource.PlayOneShot(playerPotionUse);
    }

    public void PlayPlayerPickUp()
    {
        sfxAudioSource.PlayOneShot(playerPickUp);
    }

    public void PlayEnemyHit()
    {
        sfxAudioSource.PlayOneShot(enemyHit);
    }

    public void PlayEnemyAttack()
    {
        sfxAudioSource.PlayOneShot(enemyAttack);
    }

    public void PlayEnemyShoot()
    {
        sfxAudioSource.PlayOneShot(enemyShoot);
    }

    public void PlayEnemyCast()
    {
        sfxAudioSource.PlayOneShot(enemyCast);
    }

    public void PlayEnemySing()
    {
        sfxAudioSource.PlayOneShot(enemySing);
    }

    public void PlayStoneGolemJump()
    {
        sfxAudioSource.PlayOneShot(enemySing);
    }

    public void PlayMinotaurRoar()
    {
        sfxAudioSource.PlayOneShot(minotaurRoar);
    }

    public void PlayGriffonScreech()
    {
        sfxAudioSource.PlayOneShot(minotaurRoar);
    }

    public void PlayZeusThrow()
    {
        sfxAudioSource.PlayOneShot(zeusThrow);
    }

    public void PlayZeusThunderStrike()
    {
        sfxAudioSource.PlayOneShot(zeusThunderStrike);
    }

    public void PlayButtonHover()
    {
        sfxAudioSource.PlayOneShot(buttonHover);
    }

    public void PlayButtonSelect()
    {
        sfxAudioSource.PlayOneShot(buttonSelect);
    }

    public void PlayBGMMainMenu()
    {
        musicAudioSource.PlayOneShot(bgmMainMenu);
    }

    public void PlayBGMColosseum()
    {
        musicAudioSource.PlayOneShot(bgmColosseum);
    }

    public void PlayBGMForest()
    {
        musicAudioSource.PlayOneShot(bgmForest);
    }

    public void PlayBGMPlatform()
    {
        musicAudioSource.PlayOneShot(bgmPlatform);
    }

    public void PlayBGMOlympus()
    {
        musicAudioSource.PlayOneShot(bgmOlympus);
    }
}
