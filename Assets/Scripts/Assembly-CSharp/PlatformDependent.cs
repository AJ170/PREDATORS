using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public static class PlatformDependent
{
	public static bool tablet = IsIPad();

	public static Action<bool> onPromoDataReceived;

	public static bool Predators_3D_Promo_Period
	{
		get
		{
			return EncryptedPlayerPrefs.GetInt("PREDATOR_3D_PROMO", 0) == 1;
		}
	}

	public static bool InAppPurchase
	{
		get
		{
			return true;
		}
	}

	public static void SetScreenTimeout()
	{
		Screen.sleepTimeout = 60;
	}

	private static bool IsIPad()
	{
		Debug.Log("screen width: " + (float)Screen.width / Screen.dpi);
		return (float)Screen.width / Screen.dpi > 4f;
	}

	public static bool IsPC()
	{
		RuntimePlatform p = Application.platform;
		return p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXPlayer || p == RuntimePlatform.LinuxPlayer || p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.LinuxEditor;
	}

	public static bool IsVita()
	{
#if UNITY_PSP2
		return true;
#else
		return false;
#endif
	}

	private const float VITA_CURSOR_SPEED = 800f;

	private static Vector2 vitaCursorPosition;

	private static bool vitaCursorInitialized;

	private static int vitaCursorUpdatedFrame = -1;

	private static void UpdateVitaCursorPosition()
	{
		if (vitaCursorUpdatedFrame == Time.frameCount)
		{
			return;
		}
		vitaCursorUpdatedFrame = Time.frameCount;
		if (!vitaCursorInitialized)
		{
			vitaCursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
			vitaCursorInitialized = true;
		}
		float stickX = Input.GetAxis("Left Joystick Horizontal");
		float stickY = Input.GetAxis("Left Joystick Vertical");
		// unscaledDeltaTime so the cursor still moves while Time.timeScale is 0 during pause
		vitaCursorPosition.x = Mathf.Clamp(vitaCursorPosition.x + stickX * VITA_CURSOR_SPEED * Time.unscaledDeltaTime, 0f, Screen.width);
		vitaCursorPosition.y = Mathf.Clamp(vitaCursorPosition.y - stickY * VITA_CURSOR_SPEED * Time.unscaledDeltaTime, 0f, Screen.height);
	}

	private static void PositionCursorGUITexture(GUITexture mouseCursor, Vector2 screenPosition)
	{
		Rect pixelInset = mouseCursor.pixelInset;
		mouseCursor.pixelInset = new Rect(screenPosition.x - pixelInset.height / 3f, screenPosition.y - pixelInset.height, pixelInset.width, pixelInset.height);
	}

	public static string TranslateKeybinds(string text)
	{
		if (!IsPC() || string.IsNullOrEmpty(text))
		{
			return text;
		}
		text = Regex.Replace(text, @"\bthe\s+ranged\s+weapon[,\s]+key\s*2\b", "E", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bthe\s+ranged\s+weapon\s+icon\b", "E", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bthe\s+ranged\s+weapon\s+slot\b", "E", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bthe\s+ranged\s+weapon\b", "E", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bthe\s+melee\s+weapon\s+slot\b", "Q", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bthe\s+melee\s+weapon\s+icon\b", "Q", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bthe\s+melee\s+weapon\b", "Q", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bthe\s+left\s+stick\b", "WASD", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bleft\s+stick\b", "WASD", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bbutton\s+A\b", "LMB", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bbutton\s+B\b", "RMB", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bbutton\s+T\b", "F", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bDash\s+Jump\b", "Hold RMB+WASD", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bDash\b", "Hold RMB+WASD", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bMove\b", "WASD", RegexOptions.IgnoreCase);
		text = Regex.Replace(text, @"\bA\b", "LMB");
		text = Regex.Replace(text, @"\bB\b", "RMB");
		return text;
	}

	public static void LoadLevelWithLoadingScreen(string aLevelName)
	{
		EmptySceneLoader.sceneToLoad = aLevelName;
		Application.LoadLevel("EmptySceneLoader");
	}

	public static void HideMouseCursor(GUITexture mouseCursor)
	{
		if (IsVita())
		{
			if (mouseCursor != null)
			{
				mouseCursor.gameObject.SetActive(false);
			}
			return;
		}
		UnityEngine.Object.Destroy(mouseCursor.gameObject);
	}

	private static bool IsReverseOrientation()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return SystemInfo.deviceModel == "Amazon KFTT" || SystemInfo.deviceModel == "Amazon KFJWI" || SystemInfo.deviceModel == "Amazon KFJWA";
		}
		return false;
	}

	public static void SetScreenOrientation(bool inUpdate)
	{
		if (IsVita())
		{
			Cursor.visible = false;
		}
		bool flag = IsReverseOrientation();
		if (!inUpdate)
		{
			SetScreenTimeout();
		}
		if (!tablet)
		{
			return;
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft && Screen.orientation != ScreenOrientation.LandscapeLeft)
		{
			if (flag)
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
			else
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeRight && Screen.orientation != ScreenOrientation.LandscapeRight)
		{
			if (flag)
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
			else
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
		}
	}

	public static IEnumerator GetPromoPeriodData()
	{
		WWW www = new WWW("http://www.angrymobgames.com/promo_period_predators_3d.txt");
		yield return www;
		if (www.error != null)
		{
			Debug.LogError(www.error);
			yield break;
		}
		string[] param = www.text.Split(',');
		if (param.Length != 6)
		{
			yield break;
		}
		if (DateTime.Now > new DateTime(int.Parse(param[0]), int.Parse(param[1]), int.Parse(param[2])) && DateTime.Now < new DateTime(int.Parse(param[3]), int.Parse(param[4]), int.Parse(param[5])))
		{
			EncryptedPlayerPrefs.SetInt("PREDATOR_3D_PROMO", 1);
			if (onPromoDataReceived != null)
			{
				onPromoDataReceived(true);
			}
		}
		else
		{
			EncryptedPlayerPrefs.SetInt("PREDATOR_3D_PROMO", 0);
			if (onPromoDataReceived != null)
			{
				onPromoDataReceived(false);
			}
		}
	}

	public static void UpdateMouseCursorGUITexture(GUITexture mouseCursor)
	{
		if (!IsVita() || mouseCursor == null)
		{
			return;
		}
		if (!mouseCursor.gameObject.activeSelf)
		{
			mouseCursor.gameObject.SetActive(true);
		}
		UpdateVitaCursorPosition();
		PositionCursorGUITexture(mouseCursor, vitaCursorPosition);
	}

	public static int InputTouchCount()
	{
		if (Input.touchCount > 0)
		{
			return Input.touchCount;
		}
		if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
		{
			return 1;
		}
		if (IsVita() && (Input.GetKey("joystick button 0") || Input.GetKeyUp("joystick button 0")))
		{
			return 1;
		}
		return 0;
	}

	public static TouchGUI GetTouch(int aID)
	{
		TouchGUI result = default(TouchGUI);
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(aID);
			result.position = touch.position;
			result.phase = touch.phase;
			result.fingerId = touch.fingerId;
			result.deltaPosition = touch.deltaPosition;
			return result;
		}
		if (IsVita())
		{
			UpdateVitaCursorPosition();
			result.position = vitaCursorPosition;
			result.fingerId = 0;
			result.deltaPosition = Vector2.zero;
			if (Input.GetKeyDown("joystick button 0"))
			{
				result.phase = TouchPhase.Began;
			}
			else if (Input.GetKeyUp("joystick button 0"))
			{
				result.phase = TouchPhase.Ended;
			}
			else
			{
				result.phase = TouchPhase.Stationary;
			}
			return result;
		}
		result.position = Input.mousePosition;
		result.fingerId = 0;
		result.deltaPosition = Vector2.zero;
		if (Input.GetMouseButtonDown(0))
		{
			result.phase = TouchPhase.Began;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			result.phase = TouchPhase.Ended;
		}
		else
		{
			result.phase = TouchPhase.Stationary;
		}
		return result;
	}

	public static int CheckMissionSelectArrows(Collider buttonArrowLeftCampaignSquares, Collider buttonArrowRightCampaignSquares, Collider hitCollider)
	{
		if (buttonArrowLeftCampaignSquares == hitCollider)
		{
			return -1;
		}
		if (buttonArrowRightCampaignSquares == hitCollider)
		{
			return 1;
		}
		return 0;
	}

	public static void SetActiveMissionSelectArrows(Collider buttonArrowLeftCampaignSquares, Collider buttonArrowRightCampaignSquares, bool aValue)
	{
		buttonArrowLeftCampaignSquares.gameObject.SetActiveRecursively(aValue);
		buttonArrowRightCampaignSquares.gameObject.SetActiveRecursively(aValue);
	}

	public static void SetActiveIphoneGUI(Transform transform, bool p)
	{
		transform.gameObject.active = p;
	}

	public static void SetActivePC_MAC(UnityEngine.Object gameObject, bool p)
	{
	}

	public static bool ComboAttackHeavy(bool comboAttackHeavy)
	{
		return comboAttackHeavy;
	}

	public static void DestroyUselessForPC(GameObject gameObject)
	{
	}

	public static void StartBlinkDisappear(PlayerController monoBehavior)
	{
		if (tablet)
		{
			monoBehavior.StartCoroutine(monoBehavior.GUIControlBlinkDisappear());
		}
	}

	public static void StartBlinking(Transform aTransform, PlayerController aPlayer, int timesToBlink, float timeUntilStart)
	{
		aPlayer.GUIControlBlink(aTransform.GetComponent<Renderer>(), timesToBlink, timeUntilStart);
	}

	public static bool TextureTapped(Rect rect, Vector3 touchPosition)
	{
		return rect.Contains(touchPosition);
	}

	public static bool ButtonPressedBegin(Rect aRect, Vector2 touchPosition, string buttonName)
	{
		return aRect.Contains(touchPosition);
	}

	public static int GetTouchCountIncludingKeys()
	{
		return InputGUI.touchCount;
	}

	public static void CheckIfOnlyKeysPressed(ref TouchGUI touch)
	{
	}

	public static void UpdateMouseCursorGUITexture(bool Paused, GUITexture mouseCursor)
	{
		if (!IsVita() || mouseCursor == null)
		{
			return;
		}
		if (!Paused)
		{
			if (mouseCursor.gameObject.activeSelf)
			{
				mouseCursor.gameObject.SetActive(false);
			}
			return;
		}
		if (!mouseCursor.gameObject.activeSelf)
		{
			mouseCursor.gameObject.SetActive(true);
		}
		UpdateVitaCursorPosition();
		PositionCursorGUITexture(mouseCursor, vitaCursorPosition);
	}

	public static bool ReleasedLeftStick(int leftStickTouchId, int p)
	{
		return leftStickTouchId == p;
	}

	public static bool ReleasedButton(int attackButtonTouchId, int p, string p_2)
	{
		return attackButtonTouchId == p;
	}

	public static void DismissCrystalUI()
	{
		//CrystalUnityBasic.Instance.DeactivateUi();
	}

	internal static void HandleCrystalAndFoxButtons(Collider mainMenuCrystalButton, Collider hitCollider, AudioSource audio, bool sfxOn, AudioClip soundMenuClick, bool liteVersion)
	{
		if (mainMenuCrystalButton == hitCollider)
		{
			if (sfxOn)
			{
				audio.PlayOneShot(soundMenuClick);
			}
			if (liteVersion)
			{
				Application.OpenURL("http://itunes.apple.com/us/app/predators/id373352436?mt=8");
			}
			else
			{
				//CrystalUnityBasic.Instance.ActivateUi();
			}
		}
	}

	public static void SetCrystalButtonName(TextMesh textMesh)
	{
	}

	internal static bool HandleHellsingButton(Collider extrasHelsingsFireButton, Collider hitCollider, AudioSource audio, bool sfxOn, AudioClip soundMenuClick, bool showingCredits, ref int creditsIndex, GameObject CreditsGroup1, GameObject CreditsGroup2, TextMesh textExtrasMenuName)
	{
		return false;
	}

	internal static void SetPositionForGuerrilaBob(Collider extrasGuerrillaButton, Collider extrasHelsingsFireButton)
	{
	}

	internal static void HandleScreenDarken()
	{
		Screen.sleepTimeout = 30;
	}

	internal static bool GetHighDetail()
	{
		return true;
	}

	internal static void SetLowMedDetail(ref bool lowDetailIPhone3G, ref bool medDetailIPhone3GS)
	{
		lowDetailIPhone3G = false;
		medDetailIPhone3GS = true;
	}

	public static void HandleIphoneKeyboard()
	{
		Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToLandscapeLeft = false;
	}

	public static void UpdateMouseCursorGUITextureInTrophyRoom(GUITexture mouseCursor)
	{
		if (!IsVita() || mouseCursor == null)
		{
			return;
		}
		if (!mouseCursor.gameObject.activeSelf)
		{
			mouseCursor.gameObject.SetActive(true);
		}
		UpdateVitaCursorPosition();
		PositionCursorGUITexture(mouseCursor, vitaCursorPosition);
	}
}
