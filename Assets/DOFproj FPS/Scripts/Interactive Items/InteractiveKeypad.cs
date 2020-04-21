using DOFprojFPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractiveKeypad : InteractiveItem
{
	[SerializeField] protected Transform 			_elevator 		= null;
	[SerializeField] protected AudioCollection		_collection		= null;
	[SerializeField] protected int					_bank			= 0;
	[SerializeField] protected float				_activationDelay= 0.0f;	

	bool _isActivated	=	false;
    [SerializeField] public FPSController _mcontroller;
    [SerializeField] public PlayerStats _mplayerStats;
    [SerializeField] public Text statplayerText;

    public override string GetText ()
	{
		ApplicationManager appDatabase = ApplicationManager.instance;
		if (!appDatabase) return string.Empty;

		string powerState 		= appDatabase.GetGameState("POWER");
		string lockdownState	= appDatabase.GetGameState("LOCKDOWN");
        string accessCodeState = appDatabase.GetGameState("ACCESSCODE");

        // If we have not turned on the power
        if ( string.IsNullOrEmpty( powerState ) || !powerState.Equals("TRUE"))
		{
			return "Keypad : No Power";
		}
		else
		// Or we have not deactivated lockdown
		if ( string.IsNullOrEmpty( lockdownState ) || !lockdownState.Equals("FALSE"))
		{
			return "Keypad : Under Lockdown";
		}
		else
        // or we don't have the access code yet
        if (string.IsNullOrEmpty(accessCodeState) || !accessCodeState.Equals("TRUE"))
        {
            return "Keypad : Access Code Required";
        }

        // We have everything we need
        return "Keypad";
	}

    bool mIsinPerimeter;



    // ----------------------------------------------------------------------------
    // Name	:	OnTriggerEnter
    // Desc	:
    // ----------------------------------------------------------------------------
    void OnTriggerEnter(Collider other)
    {
        if (mIsinPerimeter) return;
        if (other.CompareTag("Player")) mIsinPerimeter = true;
    }

    // -----------------------------------------------------------------------------
    // Name	: OnTriggerExit
    // Desc	:
    // -----------------------------------------------------------------------------
    void OnTriggerExit(Collider other)
    {
        if (_isActivated) return;
        if (other.CompareTag("Player")) mIsinPerimeter = false; _mplayerStats.ShowMessageText("");
    }

    private void Awake()
    {
        _mplayerStats = FindObjectOfType<PlayerStats>();
        _mcontroller = FindObjectOfType<FPSController>();
       // statplayerText = GetComponent<Text>();
    }
    private void Update()
    {
        if (_isActivated) return;

        if (mIsinPerimeter)
        {
            ApplicationManager appDatabase = ApplicationManager.instance;
            if (!appDatabase) return;

            string powerState = appDatabase.GetGameState("POWER");
            string lockdownState = appDatabase.GetGameState("LOCKDOWN");
            string accessCodeState = appDatabase.GetGameState("ACCESSCODE");

            // If we have not turned on the power
            if (string.IsNullOrEmpty(powerState) || !powerState.Equals("TRUE"))
            {
                _mplayerStats.ShowMessageText("Keypad : No Power");
                return;
            }
            else
            // Or we have not deactivated lockdown
            if (string.IsNullOrEmpty(lockdownState) || !lockdownState.Equals("FALSE"))
            {
                _mplayerStats.ShowMessageText("Keypad : Under Lockdown");
                return;
            }
            else
            // or we don't have the access code yet
            if (string.IsNullOrEmpty(accessCodeState) || !accessCodeState.Equals("TRUE"))
            {
                _mplayerStats.ShowMessageText("Keypad : Access Code Required");
                return; 
            }


            if (_mplayerStats.gatePass == true)
            {
                _mplayerStats.ShowMessageText("Press 'Use' to Operate the elevator.");
                // statplayerText.text = "Press 'Use' to Scan the ID Pass.";
                if (Input.GetButton("Use"))
                {

                    Activate();
                }
            }
            else if (_mplayerStats.gatePass == false)
            {
                _mplayerStats.ShowMessageText("Press 'Inventory' and get your COVID-19 vaccine shot.");
                //  statplayerText.text = "Press 'Inventory' to use the ID Pass.";
            }
            else
            {
                string _gText = GetText();
                _mplayerStats.ShowMessageText(_gText);
            }


        
        }
      




    }
    public void Activate()
	{
 


        if (_isActivated) return;

     
        ApplicationManager appDatabase = ApplicationManager.instance;
		if (!appDatabase) return;
     
        string powerState 		= appDatabase.GetGameState("POWER");
		string lockdownState	= appDatabase.GetGameState("LOCKDOWN");
		string accessCodeState	= appDatabase.GetGameState("ACCESSCODE");

		if ( string.IsNullOrEmpty( powerState ) || !powerState.Equals("TRUE")) 				return;
		if ( string.IsNullOrEmpty( lockdownState ) || !lockdownState.Equals("FALSE"))		return;
        if (string.IsNullOrEmpty(accessCodeState) || !accessCodeState.Equals("TRUE")) return;

        // Delay the actual animation for the desired number of seconds
        StartCoroutine ( DoDelayedActivation());
    
        _isActivated = true;
	}

	protected IEnumerator DoDelayedActivation()
	{
		if (!_elevator) yield break;;

		// Play the sound
		if (_collection!=null)
		{
			AudioClip clip = _collection[ _bank ];
			if (clip)
			{
				if (AudioManager.instance)
					AudioManager.instance.PlayOneShotSound( _collection.audioGroup, 
															clip,
															_elevator.position, 
															_collection.volume, 
															_collection.spatialBlend,
															_collection.priority );
				
			}
		}

		// Wait for the desired delay
		yield return new WaitForSeconds( _activationDelay );

        // If we have a character manager
        if (_mcontroller != null)
        {
            // Make it a child of the elevator
            _mcontroller.transform.parent = _elevator;

            // Get the animator of the elevator and activate it animation
            Animator animator = _elevator.GetComponent<Animator>();
			if (animator) animator.SetTrigger( "Activate");

            //TODO NEED TO FREEZE
            // Freeze the FPS motor so it can rotate/jump/croach but can
            // not move off of the elevator.
            if (_mcontroller)
            {
                _mcontroller.freezeMovement = true;


                _mplayerStats.DoComplete();

                _mcontroller.lockCursor = false;
               
                               

               
            }
        }
    }
}
