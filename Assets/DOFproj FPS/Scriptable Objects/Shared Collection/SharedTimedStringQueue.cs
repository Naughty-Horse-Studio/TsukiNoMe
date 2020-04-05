using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ---------------------------------------------------------------------------
// Class    :   TimedStringQueue
// Desc     :   Handles the timed delivery of messages queued up. Callers
//              can enqueue new messages and get the currently focused
//              message using the 'text' property
// ----------------------------------------------------------------------------
[CreateAssetMenu(menuName = "Scriptable Objects/Shared Collections/Timed String Queue")]
public class SharedTimedStringQueue : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [TextArea(3, 10)]
    protected string _noteToDeveloper = "An automated timed message delivery queue.\n\nUsage:-\n\nQueue.Enqueue( 'My Message');\n\nDebug.Log(Queue.text);\n\nA CoroutineRunner Instance must exist in the current scene.";

    [SerializeField]
    protected float _dequeueDelay = 3.5f;

    // Internals
    protected float _nextDequeueTime = 0.0f;
    protected IEnumerator _coroutine = null;
    protected bool _paused = false;
    protected string _text = null;

    // Return currently active text
    public string text { get { return _text; } }

    // pause the queue
    public bool paused { get { return _paused; } set { _paused = value; } }

    // A string float queue
    private Queue<string> _queue = new Queue<string>();

    // --------------------------------------------------------------------------------------------
    // Name :   Enqueue
    // Desc :   Adds a string to the message queue
    // --------------------------------------------------------------------------------------------
    public void Enqueue(string message)
    {
        if (CoroutineRunner.Instance == null)
        {
            _text = "Timed Text Queue Error: No CoroutineRunner Object present";
            return;
        }
        _queue.Enqueue(message);

        if (_coroutine == null)
        {
            _coroutine = QueueProcessor();
            CoroutineRunner.Instance.StartCoroutine(_coroutine);
        }

       
    }

    // --------------------------------------------------------------------------------------------
    // Name :   QueueProcessor
    // Desc :   Coroutine that constantly processes the queue and its timing
    // --------------------------------------------------------------------------------------------
    protected IEnumerator QueueProcessor()
    {
        // Loop while there are messages in the queue
        while (true)
        {
            // If we are no paused
            if (!paused)
            {
                // Update timer (using unscaled time)
                _nextDequeueTime -= Time.unscaledDeltaTime;

                // Is it time for another dequeue
                if (_nextDequeueTime < 0.0f)
                {
                    // Nothing in the queue so break out of while loop and 
                    // end corotuine
                    if (_queue.Count == 0) break;

                    // Get the next string of text that will be the CURRENT text
                    // for the dequeue delay
                    _text = _queue.Dequeue();

                    // Set times so nothing happens again until the deqeue delay
                    _nextDequeueTime = _dequeueDelay;
                }
            }

            // Always remember :)
            yield return null;
        }

        // Nothing in queue so empty the current string and set coroutine to null
        // as the coroutine is about to end
        _text = null;
        _coroutine = null;
    }

    // --------------------------------------------------------------------------------------------
    // Name : Count
    // Desc : Returns the number of string in the queue
    // --------------------------------------------------------------------------------------------
    public int Count()
    {
        return _queue.Count;
    }

    public void OnAfterDeserialize() { _queue.Clear(); _text = null; }
    public void OnBeforeSerialize() { }
}
