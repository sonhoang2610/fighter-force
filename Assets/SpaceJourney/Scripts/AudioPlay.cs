using System.Collections;
using System.Collections.Generic;
using EazyEngine.Space;
using UnityEngine;



public class AudioPlay : MonoBehaviour {
    [SerializeField]
    AudioClip[] audios;
    [SerializeField]
    Pos randomAudio = new Pos(0,0);
    [SerializeField]
    AudioSource backGroundMusic = null;
    [SerializeField]
    bool playOnAwake = true;
    [SerializeField]
    bool isLoop = true;
    [SerializeField]
    float delay = 0;
    protected List<AudioSource> cacheAudios = new List<AudioSource>();
    public bool runAllGame = true;
    public float factorVolume = 1;
    public void pauseBackGroundMusic()
    {
        backGroundMusic.Pause();
    }
    int enable = 0;
    private void Awake()
    {
        for (int i = 0; i < audios.Length; ++i)
        {
            if (audios[i]!= null && audios[i].loadState != AudioDataLoadState.Loaded)
            {
                audios[i].LoadAudioData();
            }
          
        }

        if (backGroundMusic && backGroundMusic.clip.loadState != AudioDataLoadState.Loaded &&
            backGroundMusic.clip.loadState != AudioDataLoadState.Loading)
        {
            backGroundMusic.clip.LoadAudioData();
        }
    }

    public IEnumerator delayAction(float pSec, System.Action pAction)
    {
        yield return new WaitForSeconds(pSec);
        pAction();

    }

    private void OnDestroy()
    {
        if (backGroundMusic)
        {
            backGroundMusic.Stop();
       
        }
    }

    private void OnDisable()
    {
       // if (!GameManager.Instance.inGame) return;
        for(int i = cacheAudios.Count -1; i >= 0; --i)
        {
            if (cacheAudios[i].IsDestroyed())
            {
                cacheAudios.RemoveAt(i);
                continue;
            }
            if (cacheAudios[i].clip != null)
            {
                Destroy(cacheAudios[i].gameObject,0);
            }
        }
        cacheAudios.Clear();
    }
    
    
    private void OnEnable()
    {

        if (playOnAwake)
        {
            if (!runAllGame && ( !LevelManger.InstanceRaw || !LevelManger.Instance.IsMatching ))
            {
                return;
            }
            if (!backGroundMusic)
            {
                //enable++;
                //if (enable <= 1)
                //{
                //    return;
                //}
                if (delay == 0)
                {
                   var pAudio = SoundManager.Instance.PlaySound(audios[Random.Range(randomAudio.x,randomAudio.y)], Vector3.zero,isLoop,factorVolume);
                    if (isLoop && pAudio && !cacheAudios.Contains(pAudio))
                    {
                        cacheAudios.Add(pAudio);
                    }
                }
                else
                {
                    StartCoroutine(delayAction(delay, delegate
                    {
                        var pAudio = SoundManager.Instance.PlaySound(audios[Random.Range(randomAudio.x, randomAudio.y)], Vector3.zero,isLoop,factorVolume);
                        if (isLoop && pAudio && !cacheAudios.Contains(pAudio))
                        {
                            cacheAudios.Add(pAudio);
                        }
                    }));
                }
            }
            else
            {
                SoundManager.Instance.PlayBackgroundMusic(backGroundMusic);
            }
        }
    }
}
