using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Shared Variables/Shared Vector3")]
public class SharedVector3 : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private Vector3 _value = Vector3.zero;
    private Vector3 _runtimeValue = Vector3.zero;

    public Vector3 value
    {
        get { return this._runtimeValue; }
        set { this._runtimeValue = value; }
    }

    public float x
    {
        get { return this._runtimeValue.x; }
        set { this._runtimeValue.x = value; }
    }

    public float y
    {
        get { return this._runtimeValue.y; }
        set { this._runtimeValue.y = value; }
    }

    public float z
    {
        get { return this._runtimeValue.z; }
        set { this._runtimeValue.z = value; }
    }

    public void OnAfterDeserialize() { _runtimeValue = _value; }
    public void OnBeforeSerialize() { }
}
