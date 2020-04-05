using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName ="Scriptable Objects/Shared Variables/Shared String")]
public class SharedString : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private string _value = null;
    private string _runtimeValue = null;

    public string value
    {
        get { return this._runtimeValue; }
        set { this._runtimeValue = value;}
    }
    public void OnAfterDeserialize() { _runtimeValue = _value; }
    public void OnBeforeSerialize() { }
}
