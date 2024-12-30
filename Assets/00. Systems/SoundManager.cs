using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource mAudioSource;
    [SerializeField] private GameObject _soundResource;
    private Dictionary<string, GameObject> _activeSounds = new Dictionary<string, GameObject>();
    private void Awake()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    public bool SoundExists(string soundID)
    {
        return _activeSounds.ContainsKey(soundID);
    }
    

    public void SetAndPlaySound(string soundID, Transform tr, AudioClip clip)
    {
        GameObject sound = Instantiate(_soundResource, tr);
        sound.transform.SetParent(transform);
        sound.GetComponent<AudioSource>().clip = clip;
        sound.GetComponent<AudioSource>().Play();
        
        // Dictionary에 추가
        _activeSounds[soundID] = sound;
        
        StartCoroutine(DestroySound(soundID, sound));
    }

    public IEnumerator  DestroySound(string soundID, GameObject sound)
    {
        yield return new WaitForSeconds(sound.GetComponent<AudioSource>().clip.length);

        // 자동 삭제 처리 (재생 중단된 경우 이미 제거됨)
        if (_activeSounds.ContainsKey(soundID))
        {
            Destroy(sound);
            _activeSounds.Remove(soundID);
        }
    }
    
    public void StopSound(string soundID)
    {
        if (_activeSounds.TryGetValue(soundID, out GameObject sound))
        {
            // 사운드 재생 중단
            sound.GetComponent<AudioSource>().Stop();
            // 즉시 삭제
            Destroy(sound);
            _activeSounds.Remove(soundID);
        }
    }
    
}
