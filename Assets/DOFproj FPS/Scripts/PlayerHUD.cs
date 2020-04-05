using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScreenFadeType{ FadeIn, FadeOut }

public class PlayerHUD : MonoBehaviour 
{
	// Inspector Assigned UI References
    [Header("UI Sliders")]
    [SerializeField] private Slider _healthSlider       = null;
    [SerializeField] private Slider _staminaSlider      = null;
    [SerializeField] private Slider _infectionSlider    = null;
    [SerializeField] private Slider _flashlightSlider  = null;
    [SerializeField] private Slider _nightVisionSlider  = null;

    [Header("UI Text")]
	[SerializeField] private Text		_notificationText   =	null;
	[SerializeField] private Text		_transcriptText		=	null;
	[SerializeField] private Text		_interactionText	=	null;

    [Header("PDA References")]
    [SerializeField] private GameObject _pdaOverlay         = null;
    [SerializeField] private Text       _pdaPerson          = null;
    [SerializeField] private Text       _pdaSubject         = null;
    [SerializeField] private Slider     _pdaAudioTimeline   = null;

    [Header("Shared Variables")]
    //[SerializeField] private SharedFloat            _health        = null;
    //[SerializeField] private SharedFloat            _stamina       = null;
    //[SerializeField] private SharedFloat            _infection     = null;
    //[SerializeField] private SharedFloat            _flashlight    = null;
    //[SerializeField] private SharedFloat            _nightVision   = null;
    [SerializeField] private SharedString           _interactionString = null;
    [SerializeField] private SharedString           _transcriptString  = null;
    [SerializeField] private SharedTimedStringQueue _notificationQueue = null;


    [Header("Additional")]
    [SerializeField] private Image 		_screenFade =	null;
    [SerializeField] private GameObject _crosshair = null;


	// Internals
	float _currentFadeLevel = 1.0f;
	IEnumerator _coroutine	= null;

	public void Start()
	{
		if (_screenFade)
		{
			Color color = _screenFade.color;
			color.a = _currentFadeLevel;
			_screenFade.color = color;
		}

       
	}

	public void Fade ( float seconds, ScreenFadeType direction )
	{
		if (_coroutine!=null) StopCoroutine(_coroutine); 
		float targetFade  = 0.0f;;

		switch (direction)
		{
			case ScreenFadeType.FadeIn:
			targetFade = 0.0f;
			break;

			case ScreenFadeType.FadeOut:
			targetFade = 1.0f;
			break;
		}

		_coroutine = FadeInternal( seconds, targetFade);
		StartCoroutine(_coroutine);
	}


	IEnumerator FadeInternal( float seconds, float targetFade )
	{
		if (!_screenFade) yield break;

		float timer = 0;
		float srcFade = _currentFadeLevel;
		Color oldColor = _screenFade.color;
		if (seconds<0.1f) seconds = 0.1f;

		while (timer<seconds)
		{
			timer+=Time.deltaTime;
			_currentFadeLevel = Mathf.Lerp( srcFade, targetFade, timer/seconds );
			oldColor.a = _currentFadeLevel;
			_screenFade.color = oldColor;
			yield return null;
		}

		oldColor.a = _currentFadeLevel = targetFade;
		_screenFade.color = oldColor;
	}

    void Update()
    {
       
        //if (_healthSlider != null && _health != null)
        //    _healthSlider.value = _health.value;

        //if (_staminaSlider != null && _stamina != null)
        //    _staminaSlider.value = _stamina.value;

        //if (_infectionSlider != null && _infection != null)
        //    _infectionSlider.value = _infection.value;

        //if (_flashlightSlider != null && _flashlight != null)
        //    _flashlightSlider.value = _flashlight.value;

        //if (_nightVisionSlider != null && _nightVision != null)
        //    _nightVisionSlider.value = _nightVision.value;

        if (_interactionText != null && _interactionString != null)
            _interactionText.text = _interactionString.value;

        if (_transcriptText != null && _transcriptString != null)
            _transcriptText.text = _transcriptString.value;

        if (_notificationText != null && _notificationQueue != null)
            _notificationText.text = _notificationQueue.text;

    }

    public void OnBeginAudio(InventoryItemAudio audioItem)
    {
        if (!audioItem) return;

        if (_pdaOverlay) _pdaOverlay.SetActive(true);
        if (_pdaPerson) _pdaPerson.text = audioItem.person;
        if (_pdaSubject) _pdaSubject.text = audioItem.subject;
    }

    public void OnUpdateAudio(float time)
    {
        if (_pdaAudioTimeline) _pdaAudioTimeline.value = time;
    }

    public void OnEndAudio()
    {
        if (_pdaOverlay) _pdaOverlay.SetActive(false);
    }
}
