using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ------------------------------------------------------------------------------------------------
// CLASS	:	TimedStateKey
// DESC		:	Describes a Time at which a state should be set in the 
//              Application State Dictionary.
// ------------------------------------------------------------------------------------------------
[System.Serializable]
public class TimedStateKey
{
    public float Time = 0.0f;
    public string Key = null;
    public string Value = null;
    public string UIMessage = null;
}

// ------------------------------------------------------------------------------------------------
// CLASS    :   TimedCaptionKey
// DESC     :   Describes a caption and a time to display that caption
// ------------------------------------------------------------------------------------------------
[System.Serializable]
public class TimedCaptionKey
{
    public float Time = 0.0f;
    [TextArea(3, 10)]
    public string Text = null;
}

// ------------------------------------------------------------------------------------------------
// CLASS    :   InventoryItemAudio
// DESC     :   Describes an Audio Inventory Item Scriptable Object
// ------------------------------------------------------------------------------------------------
[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Items/Audio")]
public class InventoryItemAudio : InventoryItem
{
    // Inspector Assigned
    [Header("Audio Log Properties")]
    [Tooltip("The Author of the Audio Log")]
    [SerializeField] private string _person = null;
    [Tooltip("The Subject of the Audio Log")]
    [SerializeField] private string _subject = null;
    [Tooltip("The Image for this Audio Log")]
    [SerializeField] private Texture2D _image = null;

    [Header("State Change Keys")]
    [Tooltip("A list of timed state changes to occur throughout the timeline of the Audio Log.")]
    [SerializeField] private List<TimedStateKey> _stateKeys = new List<TimedStateKey>();

    [Header("Caption Keys")]
    [Tooltip("A list of timed captions to display throughout the timeline of the Audio Log.")]
    [SerializeField] private List<TimedCaptionKey> _captions = new List<TimedCaptionKey>();

    // Public properties
    public string                   person      { get { return _person;     } }
    public string                   subject     { get { return _subject;    } }
    public Texture2D                image       { get { return _image;      } }
    public List<TimedStateKey>      stateKeys   { get { return _stateKeys;  } }
    public List<TimedCaptionKey>    captionKeys { get { return _captions;   } }


    // -----------------------------------------------------------
    // Name	:	Use
    // Desc	:	I override this to perform no-op. For uadio based
    //			Inventory items the inventory system itself handles
    //			the playing of the audio
    // -----------------------------------------------------------
    public override InventoryItem Use(Vector3 position, bool playAudio = true, Inventory inventory = null)
    {
        return null;
    }
}
