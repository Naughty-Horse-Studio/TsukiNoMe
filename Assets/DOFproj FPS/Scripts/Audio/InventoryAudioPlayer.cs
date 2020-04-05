using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Special Unity Events for broadcasting a weapon change event
[System.Serializable]
public class InventoryAudioBeginEvent : UnityEvent<InventoryItemAudio> { }

[System.Serializable]
public class InventoryAudioUpdateEvent : UnityEvent<float> { }

// ------------------------------------------------------------------------------------------------
// CLASS    :   InventoryAudioPlayer (Part of the Inventory System)
// DESC     :   The component used by our Inventory System to playback audio recordings
// ------------------------------------------------------------------------------------------------
public class InventoryAudioPlayer : MonoBehaviour
{
    // Singleton members
    protected static InventoryAudioPlayer _instance = null;
    public static InventoryAudioPlayer instance { get { return _instance; } }

    // Inspector Assigned
    [Header("Shared Variables")]
    [SerializeField] protected SharedVector3            _playerPosition = null;
    [SerializeField] protected SharedTimedStringQueue   _notificationQueue = null;
    [SerializeField] protected SharedString             _transcriptText = null;

    [Header("Audio Configuration")]
    [SerializeField] protected AudioCollection          _stateNotificationSounds = null;

    // Events
    [Header("Event Listeners")]
    public InventoryAudioBeginEvent     OnBeginAudio    = new InventoryAudioBeginEvent();
    public InventoryAudioUpdateEvent    OnUpdateAudio   = new InventoryAudioUpdateEvent();
    public UnityEvent                   OnEndAudio      = new UnityEvent();

    // Internals
    protected AudioSource   _audioSource    = null;
    protected IEnumerator     _coroutine      = null;

    // --------------------------------------------------------------------------------------------
    // Name :   Awake
    // Desc :   Initialize the Audio Player
    // --------------------------------------------------------------------------------------------
    protected void Awake()
    {
        // Store singleton instance reference
        _instance = this;

        // Store Audio Source component
        _audioSource = GetComponent<AudioSource>();

        // Configure audio source to play even when time is paused
        if (_audioSource)
            _audioSource.ignoreListenerPause = true;
    }

    // --------------------------------------------------------------------------------------------
    // Name :   PlayAudio
    // Desc :   Called (usually by Inventory System) to start playing an Inventory Audio Item.
    //          This raises the BeginAudio event and then starts up the Update coroutine.
    // --------------------------------------------------------------------------------------------
    public void PlayAudio(InventoryItemAudio audioItem)
    {
        Debug.Log("Gotcha!");
        // Stop the coroutine if was running previously
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        // Stop playing any sound that the audio source is already playing
        if (_audioSource && _audioSource.isPlaying)
        {
            // Stop audio playing
            _audioSource.Stop();

            // Clear transcript text
            if (_transcriptText)
                _transcriptText.value = null;
        }

        // Failure - so fire stop event immediately
        if (!audioItem || !_audioSource || !audioItem.audioCollection)
        {
            StopAudio();
            return;
        }

        // Inventory Items always have USE clips in third bank
        AudioClip clip = audioItem.audioCollection[2];
        if (!clip)
        {
            StopAudio();
            return;
        }

        // Configure Audio Source
        _audioSource.clip           = clip;
        _audioSource.volume         = audioItem.audioCollection.volume;
        _audioSource.spatialBlend   = audioItem.audioCollection.spatialBlend;
        _audioSource.priority       = audioItem.audioCollection.priority;
        _audioSource.Play();

        // Fire Begin Event
        OnBeginAudio.Invoke(audioItem);

        // Start  Update Coroutine
        _coroutine = UpdateAudio(audioItem);
        StartCoroutine(_coroutine);
    }
    
    // --------------------------------------------------------------------------------------------
    // Name :   StopAudio
    // Desc :   Called when the sound stops playing or called to manually stop the sound.
    //          Makes sure coroutine is stopped and EndAudio event is raised.
    // --------------------------------------------------------------------------------------------
    public void StopAudio()
    {
        // Reset Audio Source
        if (_audioSource)
            _audioSource.clip = null;

        // Stop corotuine if still running
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        // Remove transcript text
        if (_transcriptText)
            _transcriptText.value = null;

        // Let all listeners know the audio is over
        OnEndAudio.Invoke();
    }

    // --------------------------------------------------------------------------------------------
    // Name :   UpdateAudio (Coroutine)
    // Desc :   Is called each frame while the audio clip is playing.
    // --------------------------------------------------------------------------------------------         
    protected IEnumerator UpdateAudio(InventoryItemAudio audioItem)
    {
        // If we have a valid InventoryItem and if this object has an audio source
        if (audioItem && _audioSource)
        {
            // Used for keeping track of timeline
            int previousStateKeyIndex = 0;
            int previousCaptionKeyIndex = 0;

            // Used to store a reference of Audio Item's State and Caption keys 
            // This is NOT a copy of the keys so don't alter
            List<TimedStateKey> stateKeys = audioItem.stateKeys;
            List<TimedCaptionKey> captionKeys = audioItem.captionKeys;

            // Keep iterating while sound clip is playing
            while (_audioSource.isPlaying)
            {
                // Invoke Update Event with normalized time
                OnUpdateAudio.Invoke(_audioSource.time / _audioSource.clip.length);

                // Do we have state keys to process and a valid app manager
                if (stateKeys != null && ApplicationManager.instance)
                {
                    // Loop from the previous key we found that we have not yet executed
                    for (int i = previousStateKeyIndex; i < stateKeys.Count; i++)
                    {
                        // Is it a legit key?
                        TimedStateKey keyFrame = stateKeys[i];
                        if (keyFrame != null)
                        {
                            // If we haven't reached this key yet then store this
                            // as our previous key and abort so we can test from this
                            // key next time
                            if (keyFrame.Time > _audioSource.time)
                            {
                                previousStateKeyIndex = i;
                                break;
                            }

                            // Set the state described by the keyframe
                            if (ApplicationManager.instance.SetGameState(keyFrame.Key, keyFrame.Value))
                            {
                                // Add Key Message to Shared Notification Queue
                                if (_notificationQueue)
                                    _notificationQueue.Enqueue(keyFrame.UIMessage);

                                // Play notification Sound
                                if (AudioManager.instance && _stateNotificationSounds)
                                {
                                    AudioClip clip = _stateNotificationSounds.audioClip;
                                    if (clip)
                                    {
                                        AudioManager.instance.PlayOneShotSound(_stateNotificationSounds.audioGroup,
                                                                                clip,
                                                                                _playerPosition ? _playerPosition.value : Vector3.zero,
                                                                                _stateNotificationSounds.volume,
                                                                                _stateNotificationSounds.spatialBlend,
                                                                                _stateNotificationSounds.priority,
                                                                                0.0f,
                                                                                true);
                                    }
                                }
                            }

                            previousStateKeyIndex++;
                        }
                    }
                }

                // Do we have Caption keys to process
                if (captionKeys != null)
                {
                    // Loop from the previous key we found that we have not yet executed
                    for (int i = previousCaptionKeyIndex; i < captionKeys.Count; i++)
                    {
                        // Is it a legit key?
                        TimedCaptionKey keyFrame = captionKeys[i];
                        if (keyFrame != null)
                        {
                            // If we haven't reached this key yet then store this
                            // as our previous key and abort so we can test from this
                            // key next time
                            if (keyFrame.Time > _audioSource.time)
                            {
                                previousCaptionKeyIndex = i;
                                break;
                            }

                            // Set the global shared transcript variable to the caption text
                            if (_transcriptText)
                                _transcriptText.value = keyFrame.Text;

                            previousCaptionKeyIndex++;
                        }
                    }
                }

                yield return null;

            }
        }

        // Stop the audio and invoke ending event
        StopAudio();
    }
}

