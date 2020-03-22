using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class DisableShadowCasting : MonoBehaviour {

	// Use this for initialization
	void OnEnable()
    {
      
        GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	}
	
	
}
