using System.Collections;
using System.Collections.Generic;
using EazyEngine.Space;
using UnityEngine;
using EazyEngine.Tools;

public enum TypeNotifySfx { TurnSound, TurnMusic, PlaySound, PlayMusic }
public struct SfxNotifi
{
    public object value;
    public TypeNotifySfx type;
    public SfxNotifi(TypeNotifySfx pType, object pValue)
    {
        value = pValue;
        type = pType;
    }
}

public class SoundManager : PersistentSingleton<SoundManager>
{

    [Header("Music")]
    public bool musicOn = true;
    /// the music volume
    [Range(0, 1)]
    public float MusicVolume = 0.3f;

    [Header("Sound Effects")]
    public bool sfxOn = true;
    /// the sound fx volume
    [Range(0, 1)]
    public float SfxVolume = 1f;

    public AudioSource _backgroundMusic;
    public static List<AudioSource> PoolInGameAudios = new List<AudioSource>();
    protected GameObject parrentSound;
    public List<AudioClip> ingnoreClips = new List<AudioClip>();
    protected override void Awake()
    {
        base.Awake();
        SfxOn = PlayerPrefs.GetInt("Sound", 1) == 1 ? true : false;
        MusicOn = PlayerPrefs.GetInt("Music", 1) == 1 ? true : false;

    }
    public bool SfxOn
    {
        get
        {
            return sfxOn;
        }

        set
        {
            bool isChange = false;
            sfxOn = value;
            PlayerPrefs.SetInt("Sound", sfxOn ? 1 : 0);
            AudioListener.volume =value ? 1 :0;
        }
    }

    public bool MusicOn
    {
        get
        {
            return musicOn;
        }

        set
        {
            bool isChange = musicOn != value;
            musicOn = value;
            PlayerPrefs.SetInt("Music", musicOn ? 1 : 0);
            if (isChange)
            {
                EzEventManager.TriggerEvent(new SfxNotifi(TypeNotifySfx.TurnMusic, value));
            }
            if (!value)
            {
                var audios = GameObject.FindObjectsOfType<AudioSource>();
                foreach (var pAudio in audios)
                {
                    pAudio.Stop();
                }
            }
            else
            {
                var audios = GameObject.FindObjectsOfType<AudioSource>();
                foreach (var pAudio in audios)
                {
                    pAudio.Play();
                }
            }
        }
    }

    /// <summary>
    /// Plays a background music.
    /// Only one background music can be active at a time.
    /// </summary>
    /// <param name="Clip">Your audio clip.</param>
    public virtual void PlayBackgroundMusic(AudioSource Music)
    {
        if (_backgroundMusic != null && Music.clip == _backgroundMusic.clip) return;
        // if the music's been turned off, we do nothing and exit
        if (!sfxOn)
            return;
        if (_backgroundMusic != null && _backgroundMusic.clip.name == Music.clip.name)
        {
            return;
        }
        // if we already had a background music playing, we stop it
        if (_backgroundMusic != null)
        {
            _backgroundMusic.clip.UnloadAudioData();
            _backgroundMusic.Stop();
        }
        // we set the background music clip
        _backgroundMusic = Music;
        // we set the music's volume
        _backgroundMusic.volume = MusicVolume;
        // we set the loop setting to true, the music will loop forever
        _backgroundMusic.loop = true;
        // we start playing the background music
        _backgroundMusic.Play();
    }
    //Dictionary<AudioClip,GameObject> sounds = new List<AudioClip,GameObject>();
    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <returns>An audiosource</returns>
    /// <param name="sfx">The sound clip you want to play.</param>
    /// <param name="location">The location of the sound.</param>
    /// <param name="loop">If set to true, the sound will loop.</param>
    public virtual AudioSource PlaySound(AudioClip sfx, Vector3 location, bool loop = false,float pFactorSpeed = 1)
    {
        if (!SfxOn || !sfx)
            return null;
       

        if (ingnoreClips.Contains(sfx)) return null;
        if (!parrentSound)
        {
            parrentSound = new GameObject();
            parrentSound.name = "Sound";
        }
        AudioSource audioSource = PoolInGameAudios.FindAndClean<AudioSource>(x => (!x.gameObject.activeSelf && x.clip == sfx),x => x.IsDestroyed());
        if (!audioSource)
        {
            // we create a temporary game object to host our audio source
            GameObject temporaryAudioHost = new GameObject(sfx.name);
            temporaryAudioHost.transform.parent = parrentSound.transform;
            // we set the temp audio's position
            temporaryAudioHost.transform.position = location;
            // we add an audio source to that host
            audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            // we set that audio source clip to the one in paramaters
            audioSource.clip = sfx;
            if (!loop && GameManager.Instance.inGame )
            {
                PoolInGameAudios.Add(audioSource);
            }
        }
        audioSource.gameObject.SetActive(true);
        // we set the audio source volume to the one in parameters
        audioSource.volume = SfxVolume * pFactorSpeed;
        // we set our loop setting
        audioSource.loop = loop;
        // we start playing the sound
        audioSource.Play();

        if (!loop)
        {
            if (GameManager.Instance.inGame)
            {
                ingnoreClips.Add(sfx);
            }

            StartCoroutine(delayAction(0.1f, delegate
            {
                ingnoreClips.Remove(sfx);
            }));
            StartCoroutine(delayAction(sfx.length, delegate
            {
                if (GameManager.Instance.inGame && PoolInGameAudios.Contains(audioSource))
                {
                    audioSource.gameObject.SetActive(false);
                }
                else if(!audioSource.IsDestroyed())
                {
                    Destroy(audioSource.gameObject);
                }
            }));
        }

        // we return the audiosource reference
        return audioSource;
    }
    private IEnumerator delayAction(float pDelay , System.Action pAction)
    {
        yield return new WaitForSeconds(pDelay);
        pAction();
    }
    /// <summary>
    /// Stops the looping sounds if there are any
    /// </summary>
    /// <param name="source">Source.</param>
    public virtual void StopLoopingSound(AudioSource source)
    {
        if (source != null)
        {
            Destroy(source.gameObject);
        }
    }

    private void LateUpdate()
    {
        //if (_backgroundMusic && MusicOn && !_backgroundMusic.isPlaying)
        //{
        //    AudioSource oldMusic = _backgroundMusic;
        //    _backgroundMusic = Instantiate(_backgroundMusic.gameObject, _backgroundMusic.transform.parent).GetComponent<AudioSource>();
        //    Destroy(oldMusic.gameObject);
        //}
    }
}
