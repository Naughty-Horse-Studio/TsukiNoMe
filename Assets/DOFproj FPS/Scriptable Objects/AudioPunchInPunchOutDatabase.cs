/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioPunchInPunchOutInfo
{
    public AudioClip Clip = null;
    public float StartTime = 0.0f;
    public float EndTime = 0.0f;
}

[CreateAssetMenu(fileName = "New Audio Punch-In Punch-Out Database")]
public class AudioPunchInPunchOutDatabase : ScriptableObject
{
    // Inspector Assigned
    [SerializeField] protected List<AudioPunchInPunchOutInfo> dataList = new List<AudioPunchInPunchOutInfo>();

    //Internal
    protected Dictionary<AudioClip, AudioPunchInPunchOutInfo> dataDictionary = new Dictionary<AudioClip, AudioPunchInPunchOutInfo>();

    protected void OnEnable()
    {
        foreach (AudioPunchInPunchOutInfo info in dataList)
        {
            if (info.Clip)
            {
                dataDictionary[info.Clip] = info;
            }
        }
    }

    public AudioPunchInPunchOutInfo GetClipInfo(AudioClip clip)
    {
        if (dataDictionary.ContainsKey(clip))
        {
            return dataDictionary[clip];
        }

        return null;
    }
}