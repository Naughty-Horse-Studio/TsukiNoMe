using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockdownTrigger : MonoBehaviour 
{
	[SerializeField] protected float				_downloadTime		=	10.0f;
	[SerializeField] protected Slider				_downloadBar		=	null;
	[SerializeField] protected Text					_hintText			=	null;
	[SerializeField] protected MaterialController	_materialController	=	null;
	[SerializeField] protected GameObject			_lockedLight		=	null;
	[SerializeField] protected GameObject			_unlockedLight		=	null;
	
	private ApplicationManager _applicationManager 		=	null;
	private GameSceneManager	_gameSceneManager		=	null;
	private bool				_inTrigger				=	false;
	private float				_downloadProgress		=	0.0f;
	private AudioSource			_audioSource			=	null;
	private bool				_downloadComplete		=	false;


	void OnEnable()
	{
		// Fetch application Database, Game Scene Manager and the
		// AudioSource Component
		_applicationManager = ApplicationManager.instance;
		_audioSource		 = GetComponent<AudioSource>();

		// Reset Download Progress
		_downloadProgress = 0.0f;

		// Instruct the material controller to register itself with the gamescene
		// manager so that it can be restored on exit.
		if (_materialController!=null 	)
			_materialController.OnStart();

		// If we have an app database
		if (_applicationManager!=null)
		{
			// Get the state of Lockdown if it exists
			string lockedDown =  _applicationManager.GetGameState( "LOCKDOWN"); 

			if (string.IsNullOrEmpty(lockedDown) || lockedDown.Equals("TRUE"))
			{
				if (_materialController!=null) 	_materialController.Activate(false);
				if (_unlockedLight) 			_unlockedLight.SetActive(false);
				if (_lockedLight) 				_lockedLight.SetActive(true);
				_downloadComplete = false;
			}
			else		
			if (lockedDown.Equals("FALSE"))
			{
				if (_materialController!=null) 	_materialController.Activate(true);
				if (_unlockedLight) 			_unlockedLight.SetActive(true);
				if (_lockedLight) 				_lockedLight.SetActive(false);
				_downloadComplete = true;
			}
		}

		// Set all UI Elements to starting condition
		ResetSoundAndUI();
	}

	// ----------------------------------------------------------------------------
	// Name	:	Update
	// Desc	:	Called each frame by Unity to update this object.
	// ----------------------------------------------------------------------------
	void Update()
	{
		// This computer has already been emptied of information
		if (_downloadComplete) return;

		// Are we in the trigger
		if (_inTrigger)
		{
			// and are we holding down the activation key
			if (Input.GetButton ("Use"))
			{
				// Play the downloading sound
				if (_audioSource && !_audioSource.isPlaying) 
					_audioSource.Play ();

				// Increase with delta time clamping to max time
				_downloadProgress = Mathf.Clamp(_downloadProgress+Time.deltaTime, 0.0f, _downloadTime);	

				// If download is not fully complete then update UI to reflect download progress
				if (_downloadProgress != _downloadTime)
				{
					if (_downloadBar)
					{
						_downloadBar.gameObject.SetActive(true);
						_downloadBar.value = _downloadProgress/_downloadTime;
					}
					return;
				}
				else
				{
					// Otherwise download is complete
					_downloadComplete = true;

					// Turn off UI elements
					ResetSoundAndUI ();

					// Change Hint Text to show success
					if (_hintText) _hintText.text = "Successful Deactivation";

					// Shutdown lockdown
					_applicationManager.SetGameState( "LOCKDOWN", "FALSE"); 

					// Swap texture Over
					if (_materialController!=null) 	_materialController.Activate(true);
					if (_unlockedLight) 			_unlockedLight.SetActive(true);
					if (_lockedLight) 		_lockedLight.SetActive(false);

					// Job done
					return;
				}
			}
		}

		// Reset UI and Sound
		_downloadProgress = 0.0f;
		ResetSoundAndUI ();
	}

	// -----------------------------------------------------------------------------
	// Name	:	ResetSoundAndUI
	// Desc	:	Stop any audio playing, reset download progress and hide UI elements
	// -----------------------------------------------------------------------------
	void ResetSoundAndUI()
	{
		if (_audioSource && _audioSource.isPlaying) _audioSource.Stop();
		if (_downloadBar)
		{
			_downloadBar.value = _downloadProgress;
			_downloadBar.gameObject.SetActive(false);
		}

		if (_hintText)	_hintText.text	   = "Hold 'Use' Button to Deactivate";
	}

	// ----------------------------------------------------------------------------
	// Name	:	OnTriggerEnter
	// Desc	:
	// ----------------------------------------------------------------------------
	void OnTriggerEnter( Collider other )
	{
		if (_inTrigger || _downloadComplete) return;
		if (other.CompareTag("Player") )  _inTrigger = true;
	}

	// -----------------------------------------------------------------------------
	// Name	: OnTriggerExit
	// Desc	:
	// -----------------------------------------------------------------------------
	void OnTriggerExit( Collider other)
	{
		if (_downloadComplete) return;
		if (other.CompareTag("Player") )_inTrigger = false;
	}
}
