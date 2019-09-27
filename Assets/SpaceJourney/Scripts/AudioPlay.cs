using System.Collections;
using System.Collections.Generic;
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

    public void pauseBackGroundMusic()
    {
        backGroundMusic.Pause();
    }
    int enable = 0;
    private void Awake()
    {
        //if (playOnAwake)
        //{
        //    if (!backGroundMusic)
        //    {
        //        SoundManager.Instance.PlaySound(audio, Vector3.zero);
        //    }
        //    else
        //    {
        //        SoundManager.Instance.PlayBackgroundMusic(backGroundMusic);
        //    }
        //}
    }

    public IEnumerator delayAction(float pSec, System.Action pAction)
    {
        yield return new WaitForSeconds(pSec);
        pAction();

    }
    

    private void OnEnable()
    {

        if (playOnAwake)
        {
            if (!backGroundMusic)
            {
                //enable++;
                //if (enable <= 1)
                //{
                //    return;
                //}
                if (delay == 0)
                {
                    SoundManager.Instance.PlaySound(audios[Random.Range(randomAudio.x,randomAudio.y)], Vector3.zero);
                }
                else
                {
                    StartCoroutine(delayAction(delay, delegate
                    {
                        SoundManager.Instance.PlaySound(audios[Random.Range(randomAudio.x, randomAudio.y)], Vector3.zero);
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
