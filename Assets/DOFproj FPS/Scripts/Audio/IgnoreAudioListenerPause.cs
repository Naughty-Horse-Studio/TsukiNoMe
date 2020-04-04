using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreAudioListenerPause : MonoBehaviour
{
    [SerializeField] protected AudioSource _source = null;

    // Start is called before the first frame update
    void Start()
    {
        if (_source)
            _source.ignoreListenerPause = true;
    }

   
}
