using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelect : MonoBehaviour
{
	public enum INPUT_EVENT
	{
		NO_CHANGE = 0,
		PRESS = 1,
		RELEASE = 2,
		TAP = 3,
		MOVE = 4,
		MOVE_OFF = 5,
		DRAG = 6
	}

	public enum ORIENTATION
	{
		HORIZONTAL = 0,
		VERTICAL = 1
	}

	private enum MenuState
	{
		MissionSelect = 0,
		GearRoom = 1,
		TrophyRoom = 2,
		StatsWall = 3,
		Story = 4
	}

	private const int charsPerLine = 40;

	protected const float EZreboundSpeed = 1f;

	protected const float EZoverscrollAllowance = 0.5f;

	protected const float EZscrollDecelCoef = 0.4f;

	protected const float EZlowPassKernelWdithInSeconds = 0.03f;

	protected const float EZscrollDeltaUpdateInterval = 0.0166f;

	protected const float EZlowPassFilterFactor = 83f / 150f;

	protected const float reboundSpeed = 20f;

	protected const float scrollMax = 0.1f;

	protected const float contentExtents = 200f;

	private const int survivalPrice = 10000;

	private const int waveAttackPrice = 10000;

	private string[] missionTitles;

	private string[] missionDescriptions;

	private string[] missionDescriptionsSurvival;

	private string[] missionTitlesSurvival;

	public GUITexture mouseCursor;

	public float squaresDragMovement = 20f;

	public Transform[] trophyRoomStuffToHideWhenInMissionSelect;

	public Transform cameraTransformWithYAt180;

	public Camera trophyRoomCamera;

	public Camera missionSelectCamera;

	public Camera buyPointsCamera;

	public GameObject horizontalLinesMissionSelect;

	public Transform cameraPositionTrophies;

	public Transform cameraPositionGear;

	public Transform cameraPositionStats;

	public Transform cameraPositionCorridorStart;

	public TextMesh gearRoomText;

	public TextMesh trophyRoomText;

	public TextMesh mainMenuText;

	public TextMesh missionDescription;

	public TextMesh missionTitle;

	public Transform missionSelectParent;

	public Transform[] movingMatrices;

	private float dragTolerance = 5f;

	public float minimumSnappDistance = 2f;

	public float snappPosition = 0.5f;

	public float acceleration = 0.8f;

	public float distanceBetweenIcons = 0.2f;

	public float rightScreenMargin = 0.8f;

	public float leftScreenMargin = 0.2f;

	public float angleLeftLimit = 0.1f;

	public float matrixResetPosition = -0.7f;

	public Button buttonBackBuyPoints;

	public PointsButton[] buttonsPoints;

	public Transform purchaseInProgressParent;

	public TextMesh purchaseMessage;

	public Collider ButtonAreaPlay;

	public Collider ButtonAreaGearRoom;

	public Button ButtonAreaMainMenu;

	public Collider buttonBuyHonorPointsMissionSelect;

	public Collider buttonGetPointsTrophyRoom;

	public Collider ButtonSurvival;

	public Collider ButtonCampaign;

	public Collider buttonArrowLeftCampaignSquares;

	public Collider buttonArrowRightCampaignSquares;

	public Collider[] poolCampaignMissionSquares;

	public Collider[] poolSurvivalMissionSquares;

	public AudioClip soundMissionSelected;

	public AudioClip soundButtonPressed;

	public AudioClip soundPlayPressed;

	public TextMesh cuchilloGUIText;

	public TextMesh stansGUIText;

	public TextMesh royceGUIText;

	public TextMesh isabelleGUIText;

	public GameObject StoryGroupPart1;

	public GameObject StoryGroupPart2;

	public GameObject StoryGroupPart3;

	public TextMesh StoryGroupPart1Text;

	public TextMesh StoryGroupPart2Text;

	public Collider buttonBackTrophy;

	public Collider buttonTrophiesLeft;

	public Collider buttonTrophiesRight;

	public Collider buttonGearRight;

	public Collider buttonStatisticsLeft;

	public AudioClip soundSkullPressed;

	public AudioClip soundWeaponPressed;

	public AudioClip soundWeaponUpgraded;

	public AudioClip soundWeaponTooExpensive;

	public TextMesh honorPointsLanguage;

	public TextMesh gearPriceLanguage;

	public TextMesh blackPredatorsStatsKilled;

	public TextMesh berserkersStatsKilled;

	public TextMesh dogsStatsKilled;

	public TextMesh hanzoStatsKilled;

	public TextMesh nikolaiStatsKilled;

	public TextMesh isabeleStatsKilled;

	public TextMesh nolanStatsKilled;

	public TextMesh soldierStatsKilled;

	public TextMesh mombasaStatsKilled;

	public TextMesh royceStatsKilled;

	public TextMesh cuchilloStatsKilled;

	public TextMesh stansStatsKilled;

	public TextMesh blackPredatorsStatsKilledLanguage;

	public TextMesh berserkersStatsKilledLanguage;

	public TextMesh dogsStatsKilledLanguage;

	public TextMesh hanzoStatsKilledLanguage;

	public TextMesh nikolaiStatsKilledLanguage;

	public TextMesh isabeleStatsKilledLanguage;

	public TextMesh nolanStatsKilledLanguage;

	public TextMesh soldierStatsKilledLanguage;

	public TextMesh mombasaStatsKilledLanguage;

	public TextMesh royceStatsKilledLanguage;

	public TextMesh cuchilloStatsKilledLanguage;

	public TextMesh stansStatsKilledLanguage;

	public TextMesh blackPredatorsStatsTrophies;

	public TextMesh berserkersStatsTrophies;

	public TextMesh dogsStatsTrophies;

	public TextMesh hanzoStatsTrophies;

	public TextMesh nikolaiStatsTrophies;

	public TextMesh isabeleStatsTrophies;

	public TextMesh nolanStatsTrophies;

	public TextMesh soldierStatsTrophies;

	public TextMesh mombasaStatsTrophies;

	public TextMesh royceStatsTrophies;

	public TextMesh cuchilloStatsTrophies;

	public TextMesh stansStatsTrophies;

	public TextMesh blackPredatorsStatsTrophiesLanguage;

	public TextMesh berserkersStatsTrophiesLanguage;

	public TextMesh dogsStatsTrophiesLanguage;

	public TextMesh hanzoStatsTrophiesLanguage;

	public TextMesh nikolaiStatsTrophiesLanguage;

	public TextMesh isabeleStatsTrophiesLanguage;

	public TextMesh nolanStatsTrophiesLanguage;

	public TextMesh soldierStatsTrophiesLanguage;

	public TextMesh mombasaStatsTrophiesLanguage;

	public TextMesh royceStatsTrophiesLanguage;

	public TextMesh cuchilloStatsTrophiesLanguage;

	public TextMesh stansStatsTrophiesLanguage;

	public TextMesh soldiersWordLanguage;

	public Collider newGearUnlockedTexture;

	public Color weaponLocked;

	public Color weaponUnlocked;

	public Color skullNoTrophyKills;

	public Color skullWithTrophyKills;

	public TextMesh nameMesh;

	public TextMesh levelMesh;

	public TextMesh honorPointsAvailableMesh;

	public TextMesh descriptionMesh;

	public TextMesh attributesRow1Mesh;

	public TextMesh attributesRow2Mesh;

	public TextMesh attributesRow3Mesh;

	public TextMesh upgradeMesh;

	public TextMesh costMesh;

	public TextMesh equipMesh;

	public Transform gearStats;

	public Transform whipTransform;

	public Transform wristBladesTransform;

	public Transform combiStickTransform;

	public Transform diskTransform;

	public Transform netGunTransform;

	public Transform plasmaGunTransform;

	public Transform mask1Transform;

	public Transform mask2Transform;

	public Transform mask3Transform;

	public Transform mask4Transform;

	public Transform predatorLairMovement;

	public Transform[] row1Values;

	public Transform[] row2Values;

	public Transform[] row3Values;

	public Transform[] row1ValuesEmpty;

	public Transform[] row2ValuesEmpty;

	public Transform[] row3ValuesEmpty;

	public TextMesh stats1Text;

	public TextMesh stats2Text;

	public TextMesh stats1TextValues;

	public TextMesh stats2TextValues;

	public TextMesh statsGameStatisticsText;

	public Transform statsPanel;

	public Transform ScrollListParent;

	private Collider ScrollListCollider;

	public ORIENTATION orientation;

	public Vector2 EZviewableArea = new Vector2(3.929f, 0.609f);

	public float EZitemSpacing = 0.15f;

	protected float EZcontentExtents;

	protected float EZscrollPos;

	protected GameObject EZmover;

	private Vector2 touchPosition = Vector2.zero;

	private INPUT_EVENT currentInputEvent;

	private Transform cameraXform;

	private Ray ray;

	private Ray prevRay;

	private Plane ctrlPlane = default(Plane);

	private bool isTap;

	private Vector3 inputDelta = Vector3.zero;

	private Vector2 initialTouchPosition = Vector2.zero;

	private bool blockInput;

	protected bool EZisScrolling;

	protected bool EZnoTouch;

	private float EZscrollInertia;

	protected float EZscrollMax;

	private float EZscrollDelta;

	private Collider hitCollider;

	private bool sfxOn = true;

	private bool musicOn = true;

	private bool inputActive;

	private int currentMission;

	private MissionSquare[] missionSquaresSurvival;

	private MissionSquare[] missionSquaresCampaign;

	private string textToDisplay = string.Empty;

	private bool displayCampaignMissions = true;

	private int storySlide = 1;

	private float angleY;

	private float angleX;

	private float angleZ;

	private Vector3 positionLeft;

	private bool zoomedIn;

	private bool cameraMoved;

	private Transform trophyStats;

	private Texture selectedMissionTexture;

	public static int availableHonorPoints;

	private int goodIndex;

	private Quaternion cameraTargetRotation;

	private Vector3 cameraTargetPosition;

	private int currentSlot;

	private int selectedMission;

	private int selectedWeapon;

	private int savedSelectedCampaignMission;

	private float distance;

	private int slot;

	private int maxLevelUnlocked;

	private MenuState menuState;

	private static List<string> consumingProductList = new List<string>();

	private GearOnTheWall[] gearsOnTheWall;

	public TextMesh buyPointsMenuTextMesh;

	protected float scrollPos;

	public Vector2 viewableArea;

	protected TouchGUI touch;

	private Transform currentZoomedSkull;

	private float blockInputTimer;

	private bool isInBuyPointsMenu;

	private bool ButtonHarwareBackPressed
	{
		get
		{
			return Input.GetKeyDown(KeyCode.Escape);
		}
	}

	public bool survivalUnlocked
	{
		get
		{
			return EncryptedPlayerPrefs.GetInt("PR_Survival1Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 0), 0) == 1;
		}
		set
		{
			EncryptedPlayerPrefs.SetInt("PR_Survival1Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 0), value ? 1 : 0);
		}
	}

	public bool waveAttackUnlocked
	{
		get
		{
			return EncryptedPlayerPrefs.GetInt("PR_Survival2Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 0), 0) == 1;
		}
		set
		{
			EncryptedPlayerPrefs.SetInt("PR_Survival2Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 0), value ? 1 : 0);
		}
	}

	private IEnumerator StartMission()
	{
		yield return null;
		PlatformDependent.LoadLevelWithLoadingScreen("GamePlay_iPad");
	}

	private IEnumerator DelayedHidePurchasePlane(float delay)
	{
		yield return new WaitForSeconds(delay);
		ShowTransactionMenu(false);
	}

	private IEnumerator DelayedHideBuyPointsScreen(float delay)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(ShowPointsMenuDelayed(false));
	}

	private void OnPurchaseCompletedAmazon(string sku)
	{
		ProcessPurchase(sku);
	}

	private void OnPurchaseComplete(GooglePurchase purchase)
	{
		GoogleIAB.consumeProduct(purchase.productId);
	}

	private void OnPurchaseFailed(string msg)
	{
		purchaseMessage.text = Language.GetTxt("PURCHASE_FAILED");
		StartCoroutine(DelayedHidePurchasePlane(2f));
	}

	private void OnConsumedProduct(GooglePurchase purchase)
	{
		string productId = purchase.productId;
		ProcessPurchase(productId);
	}

	private void ProcessPurchase(string productID)
	{
		int @int = EncryptedPlayerPrefs.GetInt("PR_CurrentSlot");
		int int2 = EncryptedPlayerPrefs.GetInt("PR_HonorPoints_S" + @int, 0);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("SLOT", @int.ToString());
		int num = 0;
		if (productID.Equals("com.fde.predators.2000points.m"))
		{
			purchaseMessage.text = Language.GetTxt("POINTS_RECEIVED", 35) + " " + 2000;
			EncryptedPlayerPrefs.SetInt("PR_HonorPoints_S" + @int, int2 + 2000);
			num = int2 + 2000;
			dictionary.Add("POINTS", "2000");
		}
		else if (productID.Equals("com.fde.predators.5500honorpoints.m"))
		{
			purchaseMessage.text = Language.GetTxt("POINTS_RECEIVED", 35) + " " + 5500;
			EncryptedPlayerPrefs.SetInt("PR_HonorPoints_S" + @int, int2 + 5500);
			num = int2 + 5500;
			dictionary.Add("POINTS", "5500");
		}
		else if (productID.Equals("com.fde.predators.12000points.m"))
		{
			purchaseMessage.text = Language.GetTxt("POINTS_RECEIVED", 35) + " " + 12000;
			EncryptedPlayerPrefs.SetInt("PR_HonorPoints_S" + @int, int2 + 12000);
			num = int2 + 12000;
			dictionary.Add("POINTS", "12000");
		}
		else if (productID.Equals("com.fde.predators.26000points.m"))
		{
			purchaseMessage.text = Language.GetTxt("POINTS_RECEIVED", 35) + " " + 26000;
			EncryptedPlayerPrefs.SetInt("PR_HonorPoints_S" + @int, int2 + 26000);
			num = int2 + 26000;
			dictionary.Add("POINTS", "26000");
		}
		else if (productID.Equals("com.fde.predators.70000points.m"))
		{
			purchaseMessage.text = Language.GetTxt("POINTS RECEIVED", 35) + " " + 70000;
			EncryptedPlayerPrefs.SetInt("PR_HonorPoints_S" + @int, int2 + 70000);
			num = int2 + 70000;
			dictionary.Add("POINTS", "70000");
		}
		availableHonorPoints = num;
		honorPointsAvailableMesh.text = num.ToString();
		trophyRoomText.text = Language.GetTxt("HONOR_POINTS") + "\n" + availableHonorPoints;
		//FlurryManager.Instance.LogEvent("IAP_PURCHASE", dictionary);
		StartCoroutine(DelayedHidePurchasePlane(3f));
	}

	private void OnQueryInventorySuccessfull(List<GooglePurchase> purchases, List<GoogleSkuInfo> inventory)
	{
		foreach (GooglePurchase purchase in purchases)
		{
			GoogleIAB.consumeProduct(purchase.productId);
		}
	}

	private void OnDestroy()
	{
		RemoveEventHandlers();
	}

	private void RemoveEventHandlers()
	{
		if (GameConstants.AmazonAppstore)
		{
			AmazonIAPConnector.Instance.purchaseFailedEvent -= OnPurchaseFailed;
			AmazonIAPConnector.Instance.purchaseSucceededEvent -= OnPurchaseCompletedAmazon;
			return;
		}
		GoogleIABManager.purchaseSucceededEvent -= OnPurchaseComplete;
		GoogleIABManager.purchaseFailedEvent -= OnPurchaseFailed;
		GoogleIABManager.consumePurchaseSucceededEvent += OnConsumedProduct;
		GoogleIABManager.consumePurchaseFailedEvent += OnPurchaseFailed;
		GoogleIABManager.queryInventorySucceededEvent -= OnQueryInventorySuccessfull;
	}

	private void Start()
	{
		AddEventHandlers();
		slot = EncryptedPlayerPrefs.GetInt("PR_CurrentSlot");
		maxLevelUnlocked = EncryptedPlayerPrefs.GetInt("PR_LastMissionUnlocked_S" + slot, 0);
		InitializeFirst();
		gearPriceLanguage.text = Language.GetTxt("PRICE");
		honorPointsLanguage.text = Language.GetTxt("HONOR_POINTS");
		if (!musicOn)
		{
			base.GetComponent<AudioSource>().Stop();
		}
		menuState = MenuState.MissionSelect;
		trophyRoomCamera.enabled = false;
		missionSelectCamera.enabled = true;
		buttonTrophiesLeft.gameObject.active = false;
		buttonTrophiesRight.gameObject.active = false;
		buttonGearRight.gameObject.active = false;
		buttonStatisticsLeft.gameObject.active = false;
		SetTrophyRoomVisible(false);
		missionSquaresCampaign = new MissionSquare[poolCampaignMissionSquares.Length];
		for (int i = 0; i < missionSquaresCampaign.Length; i++)
		{
			missionSquaresCampaign[i] = new MissionSquare(poolCampaignMissionSquares[i], i + 1);
		}
		missionSquaresSurvival = new MissionSquare[poolSurvivalMissionSquares.Length];
		for (int j = 0; j < missionSquaresSurvival.Length; j++)
		{
			missionSquaresSurvival[j] = new MissionSquare(poolSurvivalMissionSquares[j]);
		}
		availableHonorPoints = EncryptedPlayerPrefs.GetInt("PR_HonorPoints_S" + slot, 0);
		InitText();
		if (maxLevelUnlocked == 31)
		{
			/*CrystalUnityBasic.Instance.PostAchievement("419889911", true, "Ultimate Predator", false);
			if (EncryptedPlayerPrefs.GetInt("PR_PredatorDeaths_S" + slot, 5) == 0)
			{
				CrystalUnityBasic.Instance.PostAchievement("419963137", true, "Invincible", false);
			}*/
		}
		ShowCampaignMissions();
		missionSquaresCampaign[0].transform.parent = EZmover.transform;
		EZcontentExtents = 0f;
		EZcontentExtents += distanceBetweenIcons;
		for (int k = 1; k < missionSquaresCampaign.Length; k++)
		{
			missionSquaresCampaign[k].transform.position = new Vector3(missionSquaresCampaign[k - 1].transform.position.x + distanceBetweenIcons, missionSquaresCampaign[k - 1].transform.position.y, missionSquaresCampaign[k - 1].transform.position.z);
			missionSquaresCampaign[k].transform.parent = EZmover.transform;
			EZcontentExtents += distanceBetweenIcons;
		}
		UpdateContentExtents(0f);
		for (int l = 1; l < missionSquaresSurvival.Length; l++)
		{
			missionSquaresSurvival[l].transform.position = new Vector3(missionSquaresSurvival[l - 1].transform.position.x + distanceBetweenIcons, missionSquaresSurvival[l - 1].transform.position.y, missionSquaresSurvival[l - 1].transform.position.z);
		}
		selectedMission = EncryptedPlayerPrefs.GetInt("PR_CurrentMission_S" + slot, maxLevelUnlocked);
		if (selectedMission >= missionSquaresCampaign.Length)
		{
			selectedMission = missionSquaresCampaign.Length - 1;
		}
		if (selectedMission > maxLevelUnlocked)
		{
			selectedMission = maxLevelUnlocked;
		}
		missionSquaresCampaign[selectedMission].SetUnlockedSelected();
		if (displayCampaignMissions)
		{
			missionTitle.text = missionTitles[selectedMission];
		}
		else
		{
			missionTitle.text = missionTitlesSurvival[selectedMission];
		}
		if (selectedMission <= maxLevelUnlocked)
		{
			if (displayCampaignMissions)
			{
				missionDescription.text = missionDescriptions[selectedMission];
			}
			else
			{
				missionDescription.text = missionDescriptionsSurvival[selectedMission];
			}
		}
		else if (displayCampaignMissions)
		{
			missionDescription.text = Language.GetTxt("LEVEL_LOCKED", 40);
		}
		else
		{
			missionDescription.text = missionDescriptionsSurvival[selectedMission];
		}
		SetActiveMissionSelect(true);
		missionSquaresCampaign[0].SetUnlockedNotSelected();
		if (maxLevelUnlocked > missionSquaresCampaign.Length - 1)
		{
			maxLevelUnlocked = missionSquaresCampaign.Length - 1;
		}
		missionSquaresCampaign[selectedMission].SetUnlockedSelected();
		updateHPTexts();
		gearsOnTheWall = new GearOnTheWall[10];
		GearOnTheWall.row1Values = row1Values;
		GearOnTheWall.row2Values = row2Values;
		GearOnTheWall.row3Values = row3Values;
		GearOnTheWall.row1ValuesEmpty = row1ValuesEmpty;
		GearOnTheWall.row2ValuesEmpty = row2ValuesEmpty;
		GearOnTheWall.row3ValuesEmpty = row3ValuesEmpty;
		GearOnTheWall.weaponColorLocked = weaponLocked;
		GearOnTheWall.weaponColorUnlocked = weaponUnlocked;
		gearsOnTheWall[0] = new WristBlades(wristBladesTransform, upgradeMesh, attributesRow1Mesh, costMesh, nameMesh, descriptionMesh, levelMesh, honorPointsAvailableMesh, Language.GetTxt("WRIST_BLADES_MULTILINE"), EncryptedPlayerPrefs.GetInt("PR_WristLevel_S" + slot, 1), "PR_WristLevel_S" + slot, GameConstants.WRIST_DAMAGE, GameConstants.WRIST_PRICE);
		gearsOnTheWall[1] = new CombiStick(Language.GetTxt("COMBI_STICK_DESCRIPTION", 55), combiStickTransform, upgradeMesh, attributesRow1Mesh, attributesRow2Mesh, costMesh, nameMesh, descriptionMesh, levelMesh, honorPointsAvailableMesh, Language.GetTxt("SPEAR_MULTILINE"), EncryptedPlayerPrefs.GetInt("PR_SpearLevel_S" + slot, 0), "PR_SpearLevel_S" + slot, GameConstants.SPEAR_DAMAGE, GameConstants.SPEAR_RANGE, GameConstants.SPEAR_PRICE);
		gearsOnTheWall[2] = new NetGunOnTheWall(netGunTransform, upgradeMesh, attributesRow1Mesh, costMesh, nameMesh, descriptionMesh, levelMesh, honorPointsAvailableMesh, Language.GetTxt("NET_GUN_MULTILINE"), EncryptedPlayerPrefs.GetInt("PR_NetGunLevel_S" + slot, 0), "PR_NetGunLevel_S" + slot, GameConstants.NET_GUN_SPEED, GameConstants.NET_GUN_PRICE);
		gearsOnTheWall[3] = new GearOnTheWall(Language.GetTxt("DISC_DESCRIPTION", 57), diskTransform, upgradeMesh, attributesRow1Mesh, attributesRow2Mesh, attributesRow3Mesh, costMesh, nameMesh, descriptionMesh, levelMesh, honorPointsAvailableMesh, Language.GetTxt("SPIN_BLADE"), EncryptedPlayerPrefs.GetInt("PR_DiskLevel_S" + slot, 0), "PR_DiskLevel_S" + slot, GameConstants.DISK_DAMAGE, GameConstants.DISK_ENERGY, GameConstants.DISK_SPEED, GameConstants.DISK_PRICE);
		gearsOnTheWall[4] = new GearOnTheWall(Language.GetTxt("PLASMA_GUN_DESCRIPTION", 55), plasmaGunTransform, upgradeMesh, attributesRow1Mesh, attributesRow2Mesh, attributesRow3Mesh, costMesh, nameMesh, descriptionMesh, levelMesh, honorPointsAvailableMesh, Language.GetTxt("PLASMA_GUN_MULTILINE"), EncryptedPlayerPrefs.GetInt("PR_PlasmaGunLevel_S" + slot, 0), "PR_PlasmaGunLevel_S" + slot, GameConstants.PLASMA_GUN_DAMAGE, GameConstants.PLASMA_GUN_ENERGY, GameConstants.PLASMA_GUN_SPEED, GameConstants.PLASMA_GUN_PRICE);
		gearsOnTheWall[5] = new MaskOnTheWall(EncryptedPlayerPrefs.GetInt("PR_MaskType1Unlocked_S" + slot, 0), mask1Transform, equipMesh, attributesRow1Mesh, attributesRow2Mesh, attributesRow3Mesh, honorPointsAvailableMesh, nameMesh, descriptionMesh, levelMesh, costMesh, Language.GetTxt("MASK_TYPE1_NAME"), Language.GetTxt("MASK1_DESCRIPTION", 55), "PR_MaskType1Unlocked_S" + slot, GameConstants.MASK_TYPE_1, GameConstants.MASK_HEALTH_PER_MASK_TYPE, GameConstants.MASK_ENERGY_PER_MASK_TYPE, GameConstants.MASK_SPEED_PER_MASK_TYPE);
		gearsOnTheWall[6] = new MaskOnTheWall(EncryptedPlayerPrefs.GetInt("PR_MaskType2Unlocked_S" + slot, 0), mask2Transform, equipMesh, attributesRow1Mesh, attributesRow2Mesh, attributesRow3Mesh, honorPointsAvailableMesh, nameMesh, descriptionMesh, levelMesh, costMesh, Language.GetTxt("MASK_TYPE2_NAME"), Language.GetTxt("MASK2_DESCRIPTION", 55), "PR_MaskType2Unlocked_S" + slot, GameConstants.MASK_TYPE_2, GameConstants.MASK_HEALTH_PER_MASK_TYPE, GameConstants.MASK_ENERGY_PER_MASK_TYPE, GameConstants.MASK_SPEED_PER_MASK_TYPE);
		gearsOnTheWall[7] = new MaskOnTheWall(EncryptedPlayerPrefs.GetInt("PR_MaskType3Unlocked_S" + slot, 0), mask3Transform, equipMesh, attributesRow1Mesh, attributesRow2Mesh, attributesRow3Mesh, honorPointsAvailableMesh, nameMesh, descriptionMesh, levelMesh, costMesh, Language.GetTxt("MASK_TYPE3_NAME"), Language.GetTxt("MASK3_DESCRIPTION", 55), "PR_MaskType3Unlocked_S" + slot, GameConstants.MASK_TYPE_3, GameConstants.MASK_HEALTH_PER_MASK_TYPE, GameConstants.MASK_ENERGY_PER_MASK_TYPE, GameConstants.MASK_SPEED_PER_MASK_TYPE);
		gearsOnTheWall[8] = new MrBlackMaskOnTheWall(EncryptedPlayerPrefs.GetInt("PR_MaskType4Unlocked_S" + slot, 0), mask4Transform, equipMesh, attributesRow1Mesh, attributesRow2Mesh, attributesRow3Mesh, honorPointsAvailableMesh, nameMesh, descriptionMesh, levelMesh, costMesh, Language.GetTxt("MASK_TYPE4_NAME"), Language.GetTxt("MASK4_DESCRIPTION", 55), "PR_MaskType4Unlocked_S" + slot, GameConstants.MASK_TYPE_4, GameConstants.MASK_HEALTH_PER_MASK_TYPE, GameConstants.MASK_ENERGY_PER_MASK_TYPE, GameConstants.MASK_SPEED_PER_MASK_TYPE);
		gearsOnTheWall[9] = new CombiStick(Language.GetTxt("WHIP_DESCRIPTION", 55), whipTransform, upgradeMesh, attributesRow1Mesh, attributesRow2Mesh, costMesh, nameMesh, descriptionMesh, levelMesh, honorPointsAvailableMesh, Language.GetTxt("WHIP_MULTILINE"), EncryptedPlayerPrefs.GetInt("PR_WhipLevel_S" + slot, 0), "PR_WhipLevel_S" + slot, GameConstants.WHIP_DAMAGE, GameConstants.WHIP_RANGE, GameConstants.WHIP_PRICE);
		stats1Text.text = Language.GetTxt("STATS_PAGE_1");
		stats2Text.text = Language.GetTxt("STATS_PAGE_2");
		statsGameStatisticsText.text = Language.GetTxt("GAME_STATISTICS");
		RefreshStatsWallText();
		if (currentMission == 34)
		{
			menuState = MenuState.Story;
			trophyRoomCamera.enabled = false;
			missionSelectCamera.enabled = true;
			buttonTrophiesLeft.gameObject.active = false;
			buttonTrophiesRight.gameObject.active = false;
			buttonGearRight.gameObject.active = false;
			buttonStatisticsLeft.gameObject.active = false;
			StoryGroupPart1.gameObject.SetActiveRecursively(true);
			missionSelectParent.gameObject.SetActiveRecursively(false);
			StoryGroupPart1Text.text = Language.GetTxt("STORY_SLIDE_MISSIONSELECT", 42);
			StoryGroupPart2Text.text = Language.GetTxt("STORY_SLIDE2_MISSIONSELECT", 42);
		}
		textToDisplay = string.Empty;
		if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsDiscUnlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("SPIN_BLADE") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsDiscUnlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsMaskType1Unlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("MASK_TYPE1_NAME") + " " + Language.GetTxt("MASK") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsMaskType1Unlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsMaskType2Unlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("MASK_TYPE2_NAME") + " " + Language.GetTxt("MASK") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsMaskType2Unlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsMaskType3Unlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("MASK_TYPE3_NAME") + " " + Language.GetTxt("MASK") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsMaskType3Unlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsMaskType4Unlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("MASK_TYPE4_NAME") + " " + Language.GetTxt("MASK") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsMaskType4Unlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsNetGunUnlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("NET_GUN") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsNetGunUnlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsPlasmaUnlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("PLASMA_GUN") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsPlasmaUnlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsSpearUnlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("SPEAR") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsSpearUnlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsWhipUnlocked_S" + slot, 0) == 1)
		{
			textToDisplay = Language.GetTxt("WHIP") + " " + Language.GetTxt("UNLOCKED");
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsWhipUnlocked_S" + slot, 0);
		}
		if (textToDisplay == string.Empty)
		{
			StartCoroutine(NextMissionSlide());
			return;
		}
		menuState = MenuState.GearRoom;
		trophyRoomCamera.enabled = true;
		missionSelectCamera.enabled = false;
		buttonTrophiesLeft.gameObject.active = false;
		buttonTrophiesRight.gameObject.active = false;
		buttonGearRight.gameObject.active = true;
		buttonStatisticsLeft.gameObject.active = false;
		SetTrophyRoomVisible(true);
		SetActiveMissionSelect(false);
		predatorLairMovement.GetComponent<Animation>().Stop("skulls_loop");
		predatorLairMovement.GetComponent<Animation>().Stop("weapons_loop");
		ResetCamera();
		predatorLairMovement.GetComponent<Animation>()["weapons_intro"].wrapMode = WrapMode.Once;
		predatorLairMovement.GetComponent<Animation>()["weapons_loop"].wrapMode = WrapMode.Loop;
		predatorLairMovement.GetComponent<Animation>()["weapons_intro"].time = 0f;
		predatorLairMovement.GetComponent<Animation>().Play("weapons_intro");
		predatorLairMovement.GetComponent<Animation>().PlayQueued("weapons_loop", QueueMode.CompleteOthers);
		StartCoroutine(UpdateCoroutine());
	}

	private void AddEventHandlers()
	{
		if (GameConstants.AmazonAppstore)
		{
			AmazonIAPConnector.Instance.purchaseFailedEvent += OnPurchaseFailed;
			AmazonIAPConnector.Instance.purchaseSucceededEvent += OnPurchaseCompletedAmazon;
			return;
		}
		GoogleIABManager.purchaseSucceededEvent += OnPurchaseComplete;
		GoogleIABManager.consumePurchaseSucceededEvent += OnConsumedProduct;
		GoogleIABManager.purchaseFailedEvent += OnPurchaseFailed;
		GoogleIABManager.consumePurchaseFailedEvent -= OnPurchaseFailed;
		GoogleIABManager.queryInventorySucceededEvent += OnQueryInventorySuccessfull;
		GoogleIAB.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2BYwqLSdC9RFsukxPI34W1Cozwy4QLtmcaU29iIacqtc53oz9ZsK4hdVA+NeL66mC8E+aKNNCkgSGQ3oD3RVRo1LzI90OD/LVKQ5Q5C0rn+pfgYywjIgtGArpQG5RW9sCiyXy8lturS5W4EZN+/6NGzF4K0njlUaAOuW2L/NbIkNzTwxGdE9fJTzHxTzz20r/X3zwT7QmGZqWHlZWNzMq+Uo2R0REcVlokovSiBecVBi3yaa1B3YepfTksv/r/rQKQmd19pk2QQz8JpX/YVZOi7jf2BV7FnDJribCykYm7Nkx47KPH+nI+3jYYbJUf+sTgOHVHEA0eu2hs6rzxP2GwIDAQAB");
	}

	private void InitText()
	{
		buttonBuyHonorPointsMissionSelect.SetTextInChildren(Language.GetTxt("GET_POINTS"));
		buyPointsMenuTextMesh.text = Language.GetTxt("PURCHASE_POINTS");
		isabelleGUIText.text = Language.GetTxt("ELITE_SNIPER");
		cuchilloGUIText.text = Language.GetTxt("DRUG_GANG_ENFORCER");
		stansGUIText.text = Language.GetTxt("MASS_MURDERER");
		royceGUIText.text = Language.GetTxt("MERCENARY_SOLDIER");
		ButtonAreaPlay.SetTextInChildren(Language.GetTxt("PLAY"));
		gearRoomText.text = Language.GetTxt("GEAR");
		trophyRoomText.text = Language.GetTxt("HONOR_POINTS") + "\n" + availableHonorPoints;
		mainMenuText.text = Language.GetTxt("MAIN_MENU_WORD");
		blackPredatorsStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		dogsStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		berserkersStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		hanzoStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		nikolaiStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		isabeleStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		nolanStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		soldierStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		mombasaStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		royceStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		cuchilloStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		stansStatsKilledLanguage.text = Language.GetTxt("TOTAL_KILLED_TROPHY_ROOM");
		blackPredatorsStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		dogsStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		berserkersStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		hanzoStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		nikolaiStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		isabeleStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		nolanStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		soldierStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		mombasaStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		royceStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		cuchilloStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
		stansStatsTrophiesLanguage.text = Language.GetTxt("TROPHY_KILLS_TROPHY_ROOM");
	}

	private void PollInputMissionSelect()
	{
		if (InputGUI.touchCount <= 0)
		{
			return;
		}
		touch = InputGUI.GetTouch(0);
		switch (touch.phase)
		{
		case TouchPhase.Began:
			inputActive = true;
			currentInputEvent = INPUT_EVENT.PRESS;
			inputDelta = Vector3.zero;
			initialTouchPosition = touch.position;
			isTap = true;
			break;
		case TouchPhase.Moved:
			currentInputEvent = INPUT_EVENT.DRAG;
			inputDelta = touch.deltaPosition;
			touchPosition = touch.position;
			if (isTap && (Mathf.Abs(initialTouchPosition.x - touchPosition.x) > dragTolerance || Mathf.Abs(initialTouchPosition.y - touchPosition.y) > dragTolerance))
			{
				isTap = false;
			}
			break;
		case TouchPhase.Ended:
		case TouchPhase.Canceled:
			if (isTap)
			{
				currentInputEvent = INPUT_EVENT.TAP;
			}
			else
			{
				currentInputEvent = INPUT_EVENT.RELEASE;
			}
			inputDelta = touch.deltaPosition;
			inputActive = false;
			break;
		case TouchPhase.Stationary:
			currentInputEvent = INPUT_EVENT.NO_CHANGE;
			inputDelta = Vector3.zero;
			break;
		}
		touchPosition = touch.position;
		prevRay = ray;
		ray = missionSelectCamera.ScreenPointToRay(touchPosition);
	}

	private void ProcessInputMissionSelect()
	{
		if (InputGUI.touchCount > 0)
		{
			hitCollider = InputGUI.GetHitCollider(touchPosition, missionSelectCamera);
			if (currentInputEvent == INPUT_EVENT.TAP && hitCollider != null)
			{
				int num = PlatformDependent.CheckMissionSelectArrows(buttonArrowLeftCampaignSquares, buttonArrowRightCampaignSquares, hitCollider);
				if (num != 0)
				{
					StartCoroutine(SlideToSelectedMission(true));
					MissionSquarePressed(Mathf.Clamp(selectedMission + num, 0, 30));
				}
				else if (ButtonAreaPlay == hitCollider)
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundPlayPressed);
					}
					if (displayCampaignMissions)
					{
						if (maxLevelUnlocked >= selectedMission)
						{
							LogLevelStart();
							EncryptedPlayerPrefs.SetInt("PR_CurrentMission_S" + slot, selectedMission + 1);
							SetSelectedMissionEnvironment();
							StartCoroutine(StartMission());
						}
					}
					else
					{
						if (selectedMission == 0)
						{
							if (survivalUnlocked)
							{
								EncryptedPlayerPrefs.SetInt("PR_CurrentMission_S" + slot, 32 + selectedMission);
								SetSelectedMissionEnvironment();
								StartCoroutine(StartMission());
							}
							else if (availableHonorPoints >= 10000)
							{
								availableHonorPoints -= 10000;
								EncryptedPlayerPrefs.SetInt("PR_HonorPoints_S" + slot, availableHonorPoints);
								trophyRoomText.text = Language.GetTxt("HONOR_POINTS") + "\n" + availableHonorPoints;
								survivalUnlocked = true;
								ShowSurvivalMissions();
							}
						}
						if (selectedMission == 1)
						{
							if (waveAttackUnlocked)
							{
								EncryptedPlayerPrefs.SetInt("PR_CurrentMission_S" + slot, 32 + selectedMission);
								SetSelectedMissionEnvironment();
								StartCoroutine(StartMission());
							}
							else if (availableHonorPoints >= 10000)
							{
								availableHonorPoints -= 10000;
								EncryptedPlayerPrefs.SetInt("PR_HonorPoints_S" + slot, availableHonorPoints);
								trophyRoomText.text = Language.GetTxt("HONOR_POINTS") + "\n" + availableHonorPoints;
								waveAttackUnlocked = true;
								ShwoSurvivalMissionsWaveSelected();
							}
						}
					}
				}
				else if (ButtonAreaGearRoom == hitCollider)
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					menuState = MenuState.GearRoom;
					trophyRoomCamera.enabled = true;
					missionSelectCamera.enabled = false;
					buttonTrophiesLeft.gameObject.active = false;
					buttonTrophiesRight.gameObject.active = false;
					buttonGearRight.gameObject.active = true;
					buttonStatisticsLeft.gameObject.active = false;
					SetTrophyRoomVisible(true);
					SetActiveMissionSelect(false);
					buttonGetPointsTrophyRoom.gameObject.SetActiveRecursively(false);
					buttonBackTrophy.gameObject.SetActiveRecursively(false);
					statsPanel.gameObject.SetActiveRecursively(true);
					stats2Text.gameObject.SetActiveRecursively(false);
					predatorLairMovement.GetComponent<Animation>().Stop("skulls_loop");
					predatorLairMovement.GetComponent<Animation>().Stop("weapons_loop");
					ResetCamera();
					predatorLairMovement.GetComponent<Animation>()["weapons_intro"].wrapMode = WrapMode.Once;
					predatorLairMovement.GetComponent<Animation>()["weapons_loop"].wrapMode = WrapMode.Loop;
					predatorLairMovement.GetComponent<Animation>()["weapons_intro"].time = 0f;
					predatorLairMovement.GetComponent<Animation>().Play("weapons_intro");
					predatorLairMovement.GetComponent<Animation>().PlayQueued("weapons_loop", QueueMode.CompleteOthers);
				}
				else if (buttonBuyHonorPointsMissionSelect == hitCollider)
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					ShowBuyPointsMenu(true);
				}
				else if (!displayCampaignMissions && ButtonCampaign == hitCollider)
				{
					selectedMission = savedSelectedCampaignMission;
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					ShowCampaignMissions();
					snappPosition = -1.68f;
					StopAllCoroutines();
					StartCoroutine(NextMissionSlide());
				}
				else if (displayCampaignMissions && ButtonSurvival == hitCollider)
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					savedSelectedCampaignMission = selectedMission;
					ShowSurvivalMissions();
				}
				else if (!displayCampaignMissions)
				{
					for (int i = 0; i < missionSquaresSurvival.Length; i++)
					{
						if (!(missionSquaresSurvival[i].SquareCollider == hitCollider))
						{
							continue;
						}
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundMissionSelected);
						}
						selectedMission = i;
						missionTitle.text = missionTitlesSurvival[selectedMission];
						ButtonAreaPlay.SetTextInChildren(Language.GetTxt("PLAY"));
						switch (selectedMission)
						{
						case 0:
							if (survivalUnlocked)
							{
								missionSquaresSurvival[selectedMission].SetUnlockedSelected();
								missionDescription.text = missionDescriptionsSurvival[0];
							}
							else
							{
								missionSquaresSurvival[selectedMission].SetLockedSelected();
								ButtonAreaPlay.SetTextInChildren(Language.GetTxt("UNLOCK"));
								missionDescription.text = Language.GetTxt("UNLOCK") + " " + Language.GetTxt("SURVIVAL") + "\n" + Language.GetTxt("PRICE") + " : " + 10000 + " " + Language.GetTxt("HONOR_POINTS");
							}
							if (waveAttackUnlocked)
							{
								missionSquaresSurvival[1].SetUnlockedNotSelected();
							}
							else
							{
								missionSquaresSurvival[1].SetLockedNotSelected();
							}
							break;
						case 1:
							if (survivalUnlocked)
							{
								missionSquaresSurvival[0].SetUnlockedNotSelected();
							}
							else
							{
								missionSquaresSurvival[0].SetLockedNotSelected();
							}
							if (waveAttackUnlocked)
							{
								missionSquaresSurvival[selectedMission].SetUnlockedSelected();
								missionDescription.text = missionDescriptionsSurvival[1];
								break;
							}
							missionSquaresSurvival[selectedMission].SetLockedSelected();
							ButtonAreaPlay.SetTextInChildren(Language.GetTxt("UNLOCK"));
							missionDescription.text = Language.GetTxt("UNLOCK") + " " + Language.GetTxt("WAVE_ATTACK") + "\n" + Language.GetTxt("PRICE") + " : " + 10000 + " " + Language.GetTxt("HONOR_POINTS");
							break;
						}
						break;
					}
				}
			}
			if (displayCampaignMissions && hitCollider != null)
			{
				if (hitCollider == ScrollListCollider)
				{
					switch (currentInputEvent)
					{
					case INPUT_EVENT.NO_CHANGE:
						if (inputActive)
						{
							ListDragged();
						}
						break;
					case INPUT_EVENT.DRAG:
						if (!isTap)
						{
							ListDragged();
						}
						break;
					case INPUT_EVENT.RELEASE:
					case INPUT_EVENT.TAP:
						PointerReleased();
						break;
					}
				}
				else
				{
					for (int j = 0; j < missionSquaresCampaign.Length; j++)
					{
						if (!(missionSquaresCampaign[j].SquareCollider == hitCollider))
						{
							continue;
						}
						switch (currentInputEvent)
						{
						case INPUT_EVENT.NO_CHANGE:
							if (inputActive)
							{
								ListDragged();
							}
							break;
						case INPUT_EVENT.DRAG:
							if (!isTap)
							{
								ListDragged();
							}
							break;
						case INPUT_EVENT.TAP:
							MissionSquarePressed(j);
							PointerReleased();
							break;
						case INPUT_EVENT.RELEASE:
							PointerReleased();
							break;
						}
						break;
					}
				}
			}
		}
		if (EZisScrolling && EZnoTouch && displayCampaignMissions)
		{
			EZscrollDelta -= EZscrollDelta * 0.4f * (Time.deltaTime / 0.166f);
			if (Mathf.Abs(EZscrollDelta) < 1E-05f)
			{
				EZscrollDelta = 0f;
			}
			if (EZscrollPos < 0f)
			{
				EZscrollPos -= EZscrollPos * 1f * (Time.deltaTime / 0.166f);
				EZscrollDelta *= Mathf.Clamp01(1f + EZscrollPos / EZscrollMax);
			}
			else if (EZscrollPos > 1f)
			{
				EZscrollPos -= (EZscrollPos - 1f) * 1f * (Time.deltaTime / 0.166f);
				EZscrollDelta *= Mathf.Clamp01(1f - (EZscrollPos - 1f) / EZscrollMax);
			}
			ScrollListTo(EZscrollPos + EZscrollDelta);
			if (EZscrollPos >= 0f && EZscrollPos <= 1f && EZscrollDelta == 0f)
			{
				EZisScrolling = false;
			}
		}
		else
		{
			EZscrollInertia = Mathf.Lerp(EZscrollInertia, EZscrollDelta, 83f / 150f);
		}
	}

	private void LogLevelStart()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("LEVEL_NR", (selectedMission + 1).ToString());
		dictionary.Add("SLOT", slot.ToString());
		//FlurryManager.Instance.LogEvent("LEVEL_START", dictionary);
	}

	private void MissionSquarePressed(int index)
	{
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundMissionSelected);
		}
		if (selectedMission <= maxLevelUnlocked)
		{
			missionSquaresCampaign[selectedMission].SetUnlockedNotSelected();
		}
		else
		{
			missionSquaresCampaign[selectedMission].SetLockedNotSelected();
		}
		selectedMission = index;
		if (maxLevelUnlocked < missionSquaresCampaign.Length)
		{
			for (int i = maxLevelUnlocked + 1; i < missionSquaresCampaign.Length; i++)
			{
				missionSquaresCampaign[i].SetLockedNotSelected();
			}
		}
		if (index < maxLevelUnlocked)
		{
		}
		missionTitle.text = missionTitles[selectedMission];
		if (selectedMission <= maxLevelUnlocked)
		{
			missionSquaresCampaign[index].SetUnlockedSelected();
			missionDescription.text = missionDescriptions[selectedMission];
		}
		else
		{
			missionSquaresCampaign[index].SetLockedSelected();
			missionDescription.text = Language.GetTxt("LEVEL_LOCKED", 40);
		}
	}

	public void PointerReleased()
	{
		EZnoTouch = true;
		EZscrollDelta = EZscrollInertia;
		EZscrollInertia = 0f;
	}

	public void ListDragged()
	{
		if (inputDelta.sqrMagnitude == 0f)
		{
			EZscrollDelta = 0f;
			return;
		}
		ctrlPlane.SetNormalAndPosition(EZmover.transform.forward * -1f, EZmover.transform.position);
		float enter;
		ctrlPlane.Raycast(ray, out enter);
		Vector3 position = ray.origin + ray.direction * enter;
		ctrlPlane.Raycast(prevRay, out enter);
		Vector3 position2 = prevRay.origin + prevRay.direction * enter;
		position = ScrollListParent.InverseTransformPoint(position);
		position2 = ScrollListParent.InverseTransformPoint(position2);
		Vector3 vector = position - position2;
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			EZscrollDelta = (0f - vector.x) / (EZcontentExtents + EZitemSpacing - EZviewableArea.x);
			EZscrollDelta *= base.transform.localScale.x;
		}
		else
		{
			EZscrollDelta = vector.y / (EZcontentExtents + EZitemSpacing - EZviewableArea.y);
			EZscrollDelta *= base.transform.localScale.y;
		}
		float num = EZscrollPos + EZscrollDelta;
		if (num > 1f)
		{
			EZscrollDelta *= Mathf.Clamp01(1f - (num - 1f) / EZscrollMax);
		}
		else if (num < 0f)
		{
			EZscrollDelta *= Mathf.Clamp01(1f + num / EZscrollMax);
		}
		ScrollListTo(EZscrollPos + EZscrollDelta);
		EZnoTouch = false;
		EZisScrolling = true;
	}

	protected void UpdateContentExtents(float change)
	{
		EZcontentExtents += change;
		if (orientation == ORIENTATION.HORIZONTAL)
		{
			EZscrollMax = EZviewableArea.x / (EZcontentExtents + EZitemSpacing - EZviewableArea.x) * 0.5f;
		}
		else
		{
			EZscrollMax = EZviewableArea.y / (EZcontentExtents + EZitemSpacing - EZviewableArea.y) * 0.5f;
		}
		ScrollListTo(EZscrollPos);
	}

	public void ScrollListTo(float pos)
	{
		if (orientation == ORIENTATION.VERTICAL)
		{
			float num = EZcontentExtents + EZitemSpacing - EZviewableArea.y;
			EZmover.transform.localPosition = Vector3.up * Mathf.Clamp(num, 0f, num) * pos;
		}
		else
		{
			float num = EZcontentExtents + EZitemSpacing - EZviewableArea.x;
			EZmover.transform.localPosition = Vector3.right * Mathf.Clamp(num, 0f, num) * (0f - pos);
		}
		EZscrollPos = pos;
	}

	private void ShwoSurvivalMissionsWaveSelected()
	{
		PlatformDependent.SetActiveMissionSelectArrows(buttonArrowLeftCampaignSquares, buttonArrowRightCampaignSquares, false);
		displayCampaignMissions = false;
		HideCampaignMissions();
		for (int i = 0; i < missionSquaresSurvival.Length; i++)
		{
			missionSquaresSurvival[i].gameObject.SetActiveRecursively(false);
			missionSquaresSurvival[i].gameObject.active = true;
			missionSquaresSurvival[i].SetUnlockedNotSelected();
		}
		selectedMission = 1;
		ButtonAreaPlay.SetTextInChildren(Language.GetTxt("PLAY"));
		if (survivalUnlocked)
		{
			missionSquaresSurvival[0].SetUnlockedNotSelected();
		}
		else
		{
			missionSquaresSurvival[0].SetLockedNotSelected();
		}
		missionSquaresSurvival[1].SetUnlockedSelected();
		missionDescription.text = missionDescriptionsSurvival[1];
		missionTitle.text = missionTitlesSurvival[1];
		snappPosition = -0.8f;
	}

	private void ShowSurvivalMissions()
	{
		PlatformDependent.SetActiveMissionSelectArrows(buttonArrowLeftCampaignSquares, buttonArrowRightCampaignSquares, false);
		displayCampaignMissions = false;
		HideCampaignMissions();
		for (int i = 0; i < missionSquaresSurvival.Length; i++)
		{
			missionSquaresSurvival[i].gameObject.SetActiveRecursively(false);
			missionSquaresSurvival[i].gameObject.active = true;
			missionSquaresSurvival[i].SetUnlockedNotSelected();
		}
		selectedMission = 0;
		ButtonAreaPlay.SetTextInChildren(Language.GetTxt("PLAY"));
		if (survivalUnlocked)
		{
			missionSquaresSurvival[0].SetUnlockedSelected();
			missionDescription.text = missionDescriptionsSurvival[0];
		}
		else
		{
			missionSquaresSurvival[0].SetLockedSelected();
			ButtonAreaPlay.SetTextInChildren(Language.GetTxt("UNLOCK"));
			missionDescription.text = Language.GetTxt("UNLOCK") + " " + Language.GetTxt("SURVIVAL") + "\n" + Language.GetTxt("PRICE") + " : " + 10000 + " " + Language.GetTxt("HONOR_POINTS");
		}
		if (waveAttackUnlocked)
		{
			missionSquaresSurvival[1].SetUnlockedNotSelected();
		}
		else
		{
			missionSquaresSurvival[1].SetLockedNotSelected();
		}
		missionTitle.text = missionTitlesSurvival[0];
		snappPosition = -0.8f;
	}

	private void HideMissionSquaresSurvival()
	{
		for (int i = 0; i < missionSquaresSurvival.Length; i++)
		{
			missionSquaresSurvival[i].gameObject.SetActiveRecursively(false);
		}
	}

	private void HideCampaignMissions()
	{
		for (int i = 0; i < missionSquaresCampaign.Length; i++)
		{
			missionSquaresCampaign[i].gameObject.SetActiveRecursively(false);
		}
	}

	private void ShowCampaignMissions()
	{
		displayCampaignMissions = true;
		PlatformDependent.SetActiveMissionSelectArrows(buttonArrowLeftCampaignSquares, buttonArrowRightCampaignSquares, true);
		HideMissionSquaresSurvival();
		for (int i = 0; i < missionSquaresCampaign.Length; i++)
		{
			missionSquaresCampaign[i].gameObject.SetActiveRecursively(false);
			missionSquaresCampaign[i].gameObject.active = true;
			missionSquaresCampaign[i].SetUnlockedNotSelected();
		}
		if (maxLevelUnlocked < missionSquaresCampaign.Length)
		{
			for (int j = maxLevelUnlocked + 1; j < missionSquaresCampaign.Length; j++)
			{
				missionSquaresCampaign[j].SetLockedNotSelected();
			}
		}
		missionTitle.text = missionTitles[selectedMission];
		if (selectedMission > maxLevelUnlocked)
		{
			missionSquaresCampaign[selectedMission].SetLockedSelected();
			missionDescription.text = Language.GetTxt("LEVEL_LOCKED", 40);
		}
		else
		{
			missionSquaresCampaign[selectedMission].SetUnlockedSelected();
			missionDescription.text = missionDescriptions[selectedMission];
		}
	}

	private IEnumerator SlideToSelectedMission(bool ignoreInput)
	{
		if (selectedMission < missionSquaresCampaign.Length)
		{
			float extraX = 2f;
			if (selectedMission > 28 || selectedMission < 3)
			{
				extraX = 0.37f;
			}
			distance = missionSquaresCampaign[selectedMission].transform.position.x - (rightScreenMargin - extraX);
			while ((double)distance > 0.001 && (ignoreInput || InputGUI.touchCount <= 0))
			{
				ScrollListTo(EZscrollPos + distance * Time.deltaTime / 10f);
				distance = missionSquaresCampaign[selectedMission].transform.position.x - (rightScreenMargin - extraX);
				yield return null;
			}
			distance = missionSquaresCampaign[selectedMission].transform.position.x - (leftScreenMargin + extraX);
			while ((double)distance < -0.001 && (ignoreInput || InputGUI.touchCount <= 0))
			{
				ScrollListTo(EZscrollPos + distance * Time.deltaTime / 10f);
				distance = missionSquaresCampaign[selectedMission].transform.position.x - (leftScreenMargin + extraX);
				yield return null;
			}
		}
	}

	private IEnumerator NextMissionSlide()
	{
		StartCoroutine(MoveMatrices());
		if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsSurvival1Unlocked_S" + slot, 0) == 1)
		{
			yield return StartCoroutine(NewStuffIsUnlockedOnMissionSelectScreen(Language.GetTxt("MISSION_TITLE_SURVIVAL_1", 40) + " " + Language.GetTxt("UNLOCKED")));
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsSurvival1Unlocked_S" + slot, 0);
		}
		else if (EncryptedPlayerPrefs.GetInt("PR_PlayerKnowsSurvival2Unlocked_S" + slot, 0) == 1)
		{
			yield return StartCoroutine(NewStuffIsUnlockedOnMissionSelectScreen(Language.GetTxt("MISSION_STORY_AFTER23", 40)));
			yield return StartCoroutine(NewStuffIsUnlockedOnMissionSelectScreen(Language.GetTxt("MISSION_TITLE_SURVIVAL_2", 40) + " " + Language.GetTxt("UNLOCKED")));
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsSurvival2Unlocked_S" + slot, 0);
		}
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(SlideToSelectedMission(false));
		StartCoroutine(UpdateCoroutine());
	}

	private void RefreshStatsWallText()
	{
		int @int = EncryptedPlayerPrefs.GetInt("PR_IsabeleTrophies_S" + currentSlot, 0);
		int int2 = EncryptedPlayerPrefs.GetInt("PR_NikolaiTrophies_S" + currentSlot, 0);
		int int3 = EncryptedPlayerPrefs.GetInt("PR_NolanTrophies_S" + currentSlot, 0);
		int int4 = EncryptedPlayerPrefs.GetInt("PR_CuchilloTrophies_S" + currentSlot, 0);
		int int5 = EncryptedPlayerPrefs.GetInt("PR_HanzoTrophies_S" + currentSlot, 0);
		int int6 = EncryptedPlayerPrefs.GetInt("PR_SoldierTrophies_S" + currentSlot, 0);
		int int7 = EncryptedPlayerPrefs.GetInt("PR_MombasaTrophies_S" + currentSlot, 0);
		int int8 = EncryptedPlayerPrefs.GetInt("PR_RoyceTrophies_S" + currentSlot, 0);
		int int9 = EncryptedPlayerPrefs.GetInt("PR_StansTrophies_S" + currentSlot, 0);
		int num = @int + int2 + int3 + int4 + int5 + int6 + int7 + int8 + int9;
		int num2 = 0;
		if (EncryptedPlayerPrefs.GetInt("PR_MaskType1Unlocked_S" + slot) == 1)
		{
			num2++;
		}
		if (EncryptedPlayerPrefs.GetInt("PR_MaskType2Unlocked_S" + slot) == 1)
		{
			num2++;
		}
		if (EncryptedPlayerPrefs.GetInt("PR_MaskType3Unlocked_S" + slot) == 1)
		{
			num2++;
		}
		if (EncryptedPlayerPrefs.GetInt("PR_MaskType4Unlocked_S" + slot) == 1)
		{
			num2++;
		}
		int num3 = EncryptedPlayerPrefs.GetInt("PR_WristLevel_S" + currentSlot, 0) + EncryptedPlayerPrefs.GetInt("PR_SpearLevel_S" + currentSlot, 0) + EncryptedPlayerPrefs.GetInt("PR_DiskLevel_S" + currentSlot, 0) + EncryptedPlayerPrefs.GetInt("PR_NetGunLevel_S" + currentSlot, 0) + EncryptedPlayerPrefs.GetInt("PR_PlasmaGunLevel_S" + currentSlot, 0) + EncryptedPlayerPrefs.GetInt("PR_WhipLevel_S" + currentSlot, 0);
		if (num3 == 15)
		{
			//CrystalUnityBasic.Instance.PostAchievement("419972038", true, "Armory", false);
		}
		int int10 = EncryptedPlayerPrefs.GetInt("PR_BodiesSplit_S" + slot, 0);
		if (int10 > 100)
		{
			//CrystalUnityBasic.Instance.PostAchievement("419939333", true, "Butcher", false);
		}
		if (num > 100)
		{
			//CrystalUnityBasic.Instance.PostAchievement("419930443", true, "Head Hunter", false);
		}
		int int11 = EncryptedPlayerPrefs.GetInt("PR_NetGunCaptures_S" + currentSlot, 0);
		if (int11 > 50)
		{
			//CrystalUnityBasic.Instance.PostAchievement("419932462", true, "Trapper", false);
		}
		stats1TextValues.text = EncryptedPlayerPrefs.GetInt("PR_LastMissionUnlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 0), 0) + "/31\n" + num3 + "/18\n" + num2 + "/4\n" + availableHonorPoints + "\n" + num + "\n" + EncryptedPlayerPrefs.GetInt("PR_StealthKills_S" + currentSlot, 0);
		stats2TextValues.text = EncryptedPlayerPrefs.GetInt("PR_OponentsImpaled_S" + slot, 0) + "\n" + int10.ToString() + "\n" + EncryptedPlayerPrefs.GetInt("PR_VerticalSplits_S" + slot, 0) + "\n" + EncryptedPlayerPrefs.GetFloat("PR_LongestSurvivalTime_S" + slot, 0f).TimeFormat() + "\n" + EncryptedPlayerPrefs.GetInt("PR_PredatorDeaths_S" + slot, 0) + "\n" + int11.ToString() + "\n" + EncryptedPlayerPrefs.GetInt("PR_MaxDiskCombo_S" + currentSlot, 0);
	}

	private IEnumerator WeaponIsUnlockedCR(string text)
	{
		newGearUnlockedTexture.gameObject.SetActiveRecursively(true);
		((TextMesh)newGearUnlockedTexture.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = text;
		while (true)
		{
			if (InputGUI.touchCount > 0)
			{
				TouchGUI touch = InputGUI.GetTouch(0);
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					break;
				}
			}
			yield return null;
		}
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
		}
		yield return new WaitForSeconds(soundButtonPressed.length);
		newGearUnlockedTexture.gameObject.SetActiveRecursively(false);
	}

	private IEnumerator NewStuffIsUnlockedOnMissionSelectScreen(string text)
	{
		horizontalLinesMissionSelect.gameObject.SetActiveRecursively(true);
		((TextMesh)horizontalLinesMissionSelect.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = text;
		yield return new WaitForSeconds(2f);
		while (true)
		{
			if (InputGUI.touchCount > 0)
			{
				TouchGUI touch = InputGUI.GetTouch(0);
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					break;
				}
			}
			yield return null;
		}
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
		}
		yield return new WaitForSeconds(soundButtonPressed.length);
		horizontalLinesMissionSelect.gameObject.SetActiveRecursively(false);
	}

	private void SetActiveMissionSelect(bool value)
	{
		missionSelectParent.gameObject.SetActiveRecursively(value);
		if (value)
		{
			if (displayCampaignMissions)
			{
				ShowCampaignMissions();
			}
			else
			{
				ShowSurvivalMissions();
			}
		}
		else
		{
			HideCampaignMissions();
			HideMissionSquaresSurvival();
		}
	}

	private IEnumerator ShowPointsMenuDelayed(bool aValue)
	{
		if (menuState == MenuState.MissionSelect)
		{
			missionSelectCamera.enabled = !aValue;
		}
		else
		{
			trophyRoomCamera.enabled = !aValue;
		}
		buyPointsCamera.enabled = aValue;
		buttonBackBuyPoints.transform.parent.gameObject.SetActiveRecursively(aValue);
		if (aValue)
		{
			string[] products = new string[5] { "com.fde.predators.2000points.m", "com.fde.predators.5500honorpoints.m", "com.fde.predators.12000points.m", "com.fde.predators.26000points.m", "com.fde.predators.70000points.m" };
			if (GameConstants.AmazonAppstore)
			{
				if (!AmazonIAPConnector.Instance.IsBillingAvailable())
				{
					ShowTransactionMenu(true);
					purchaseMessage.text = Language.GetTxt("PURCHASE_NOT_AVAILABLE");
					StartCoroutine(DelayedHidePurchasePlane(1f));
					StartCoroutine(DelayedHideBuyPointsScreen(1f));
				}
			}
			else if (!IAPConnector.IAPavailable)
			{
				ShowTransactionMenu(true);
				purchaseMessage.text = Language.GetTxt("PURCHASE_NOT_AVAILABLE");
				StartCoroutine(DelayedHidePurchasePlane(1f));
				StartCoroutine(DelayedHideBuyPointsScreen(1f));
			}
			else
			{
				GoogleIAB.queryInventory(products);
			}
		}
		yield return null;
		ButtonAreaMainMenu.enabled = !aValue;
		isInBuyPointsMenu = aValue;
	}

	private void ShowBuyPointsMenu(bool aValue)
	{
		StartCoroutine(ShowPointsMenuDelayed(aValue));
	}

	private void ShowBuyPointsMenuButtons(bool aValue)
	{
		buttonBackBuyPoints.transform.parent.gameObject.SetActiveRecursively(aValue);
	}

	private void ShowTransactionMenu(bool show)
	{
		ShowBuyPointsMenuButtons(!show);
		purchaseInProgressParent.gameObject.SetActiveRecursively(show);
		buttonBackBuyPoints.GetComponent<Collider>().enabled = !show;
		PointsButton[] array = buttonsPoints;
		foreach (Button button in array)
		{
			button.GetComponent<Collider>().enabled = !show;
		}
	}

	private void InitializeFirst()
	{
		missionTitles = new string[31]
		{
			Language.GetTxt("MISSION_TITLE_1", 40),
			Language.GetTxt("MISSION_TITLE_2", 40),
			Language.GetTxt("MISSION_TITLE_3", 40),
			Language.GetTxt("MISSION_TITLE_4", 40),
			Language.GetTxt("MISSION_TITLE_5", 40),
			Language.GetTxt("MISSION_TITLE_6", 40),
			Language.GetTxt("MISSION_TITLE_7", 40),
			Language.GetTxt("MISSION_TITLE_8", 40),
			Language.GetTxt("MISSION_TITLE_9", 40),
			Language.GetTxt("MISSION_TITLE_10", 40),
			Language.GetTxt("MISSION_TITLE_11", 40),
			Language.GetTxt("MISSION_TITLE_12", 40),
			Language.GetTxt("MISSION_TITLE_13", 40),
			Language.GetTxt("MISSION_TITLE_14", 40),
			Language.GetTxt("MISSION_TITLE_15", 40),
			Language.GetTxt("MISSION_TITLE_16", 40),
			Language.GetTxt("MISSION_TITLE_17", 40),
			Language.GetTxt("MISSION_TITLE_18", 40),
			Language.GetTxt("MISSION_TITLE_19", 40),
			Language.GetTxt("MISSION_TITLE_20", 40),
			Language.GetTxt("MISSION_TITLE_21", 40),
			Language.GetTxt("MISSION_TITLE_22", 40),
			Language.GetTxt("MISSION_TITLE_23", 40),
			Language.GetTxt("MISSION_TITLE_24", 40),
			Language.GetTxt("MISSION_TITLE_25", 40),
			Language.GetTxt("MISSION_TITLE_26", 40),
			Language.GetTxt("MISSION_TITLE_27", 40),
			Language.GetTxt("MISSION_TITLE_28", 40),
			Language.GetTxt("MISSION_TITLE_29", 40),
			Language.GetTxt("MISSION_TITLE_30", 40),
			Language.GetTxt("MISSION_TITLE_31", 40)
		};
		missionDescriptions = new string[31]
		{
			Language.GetTxt("MISSION_DESCRIPTION_1", 40),
			Language.GetTxt("MISSION_DESCRIPTION_2", 40),
			Language.GetTxt("MISSION_DESCRIPTION_3", 40),
			Language.GetTxt("MISSION_DESCRIPTION_4", 44),
			Language.GetTxt("MISSION_DESCRIPTION_5", 40),
			Language.GetTxt("MISSION_DESCRIPTION_6", 40),
			Language.GetTxt("MISSION_DESCRIPTION_7", 40),
			Language.GetTxt("MISSION_DESCRIPTION_8", 40),
			Language.GetTxt("MISSION_DESCRIPTION_9", 40),
			Language.GetTxt("MISSION_DESCRIPTION_10", 43),
			Language.GetTxt("MISSION_DESCRIPTION_11", 40),
			Language.GetTxt("MISSION_DESCRIPTION_12", 40),
			Language.GetTxt("MISSION_DESCRIPTION_13", 43),
			Language.GetTxt("MISSION_DESCRIPTION_14", 43),
			Language.GetTxt("MISSION_DESCRIPTION_15", 40),
			Language.GetTxt("MISSION_DESCRIPTION_16", 40),
			Language.GetTxt("MISSION_DESCRIPTION_17", 40),
			Language.GetTxt("MISSION_DESCRIPTION_18", 43),
			Language.GetTxt("MISSION_DESCRIPTION_19", 40),
			Language.GetTxt("MISSION_DESCRIPTION_20", 40),
			Language.GetTxt("MISSION_DESCRIPTION_21", 40),
			Language.GetTxt("MISSION_DESCRIPTION_22", 40),
			Language.GetTxt("MISSION_DESCRIPTION_23", 40),
			Language.GetTxt("MISSION_DESCRIPTION_24", 40),
			Language.GetTxt("MISSION_DESCRIPTION_25", 40),
			Language.GetTxt("MISSION_DESCRIPTION_26", 40),
			Language.GetTxt("MISSION_DESCRIPTION_27", 40),
			Language.GetTxt("MISSION_DESCRIPTION_28", 40),
			Language.GetTxt("MISSION_DESCRIPTION_29", 40),
			Language.GetTxt("MISSION_DESCRIPTION_30", 40),
			Language.GetTxt("MISSION_DESCRIPTION_31", 40)
		};
		missionDescriptionsSurvival = new string[2]
		{
			Language.GetTxt("MISSION_DESCRIPTION_SURVIVAL_1", 40),
			Language.GetTxt("MISSION_DESCRIPTION_SURVIVAL_2", 40)
		};
		missionTitlesSurvival = new string[2]
		{
			Language.GetTxt("MISSION_TITLE_SURVIVAL_1", 40),
			Language.GetTxt("MISSION_TITLE_SURVIVAL_2", 40)
		};
		PointsButton[] array = buttonsPoints;
		foreach (PointsButton pointsButton in array)
		{
			pointsButton.onPressBegin += delegate(object sender, EventArgs ea)
			{
				if (GameConstants.AmazonAppstore)
				{
					if (AmazonIAPConnector.Instance.IsBillingAvailable())
					{
						purchaseMessage.text = Language.GetTxt("TRANSACTION_IN_PROGRESS", 35);
						ShowTransactionMenu(true);
						switch ((sender as PointsButton).points)
						{
						case 2000:
							AmazonIAPConnector.Instance.PurchaseProduct("com.fde.predators.2000points.m");
							break;
						case 5500:
							AmazonIAPConnector.Instance.PurchaseProduct("com.fde.predators.5500honorpoints.m");
							break;
						case 12000:
							AmazonIAPConnector.Instance.PurchaseProduct("com.fde.predators.12000points.m");
							break;
						case 26000:
							AmazonIAPConnector.Instance.PurchaseProduct("com.fde.predators.26000points.m");
							break;
						case 70000:
							AmazonIAPConnector.Instance.PurchaseProduct("com.fde.predators.70000points.m");
							break;
						}
					}
				}
				else if (IAPConnector.IAPavailable)
				{
					purchaseMessage.text = Language.GetTxt("TRANSACTION_IN_PROGRESS", 35);
					ShowTransactionMenu(true);
					switch ((sender as PointsButton).points)
					{
					case 2000:
						GoogleIAB.purchaseProduct("com.fde.predators.2000points.m");
						break;
					case 5500:
						GoogleIAB.purchaseProduct("com.fde.predators.5500honorpoints.m");
						break;
					case 12000:
						GoogleIAB.purchaseProduct("com.fde.predators.12000points.m");
						break;
					case 26000:
						GoogleIAB.purchaseProduct("com.fde.predators.26000points.m");
						break;
					case 70000:
						GoogleIAB.purchaseProduct("com.fde.predators.70000points.m");
						break;
					}
				}
			};
		}
		if (maxLevelUnlocked == 0)
		{
			HideBuyPointsButton();
		}
		buttonBackBuyPoints.onPressBegin += delegate
		{
			ShowBuyPointsMenu(false);
		};
		ButtonAreaMainMenu.onPressBegin += delegate
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
			}
			PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
		};
		PlatformDependent.HideMouseCursor(mouseCursor);
		PlatformDependent.SetScreenOrientation(false);
		buttonBackTrophy.SetTextInChildren(Language.GetTxt("BACK"));
		buttonGetPointsTrophyRoom.SetTextInChildren(Language.GetTxt("GET_POINTS"));
		PlatformDependent.HandleScreenDarken();
		sfxOn = PlayerPrefs.GetInt("PR_SfxOn", 1) == 1;
		musicOn = PlayerPrefs.GetInt("PR_MusicOn", 1) == 1;
		cameraXform = base.transform;
		GetPlayerStats();
		EZmover = new GameObject();
		EZmover.name = "Mover";
		EZmover.transform.parent = ScrollListParent;
		EZmover.transform.localPosition = Vector3.zero;
		EZmover.transform.localRotation = Quaternion.identity;
		EZmover.transform.localScale = Vector3.one;
		if ((bool)ScrollListParent)
		{
			ScrollListCollider = ScrollListParent.GetComponent<Collider>();
		}
	}

	private void HideBuyPointsButton()
	{
		buttonGetPointsTrophyRoom.GetComponent<Renderer>().enabled = false;
		buttonGetPointsTrophyRoom.enabled = false;
		buttonGetPointsTrophyRoom.DeactivateChildRenderer();
		buttonBuyHonorPointsMissionSelect.GetComponent<Renderer>().enabled = false;
		buttonBuyHonorPointsMissionSelect.enabled = false;
		buttonBuyHonorPointsMissionSelect.DeactivateChildRenderer();
	}

	private void ResetCamera()
	{
		cameraXform.localPosition = cameraPositionCorridorStart.localPosition;
		cameraXform.localRotation = cameraPositionCorridorStart.localRotation;
		predatorLairMovement.GetComponent<Animation>().Stop("weapons_loop");
		predatorLairMovement.GetComponent<Animation>().Stop("skulls_loop");
		predatorLairMovement.GetComponent<Animation>().Stop("stats_loop");
		cameraMoved = false;
	}

	private void SetTrophyRoomVisible(bool value)
	{
		for (int i = 0; i < trophyRoomStuffToHideWhenInMissionSelect.Length; i++)
		{
			trophyRoomStuffToHideWhenInMissionSelect[i].gameObject.active = value;
		}
		trophyRoomStuffToHideWhenInMissionSelect[trophyRoomStuffToHideWhenInMissionSelect.Length - 1].gameObject.SetActiveRecursively(value);
	}

	private void BackButtonPressedInTrophyRoom()
	{
		trophyRoomText.text = Language.GetTxt("HONOR_POINTS") + "\n" + availableHonorPoints;
		SetActiveMissionSelect(true);
		if (displayCampaignMissions)
		{
			ShowCampaignMissions();
			missionSquaresCampaign[0].SetUnlockedNotSelected();
			missionSquaresCampaign[selectedMission].SetUnlockedSelected();
			StopAllCoroutines();
			StartCoroutine(NextMissionSlide());
		}
		else
		{
			ShowSurvivalMissions();
		}
		menuState = MenuState.MissionSelect;
		trophyRoomCamera.enabled = false;
		missionSelectCamera.enabled = true;
		buttonTrophiesLeft.gameObject.active = false;
		buttonTrophiesRight.gameObject.active = false;
		buttonGearRight.gameObject.active = false;
		buttonStatisticsLeft.gameObject.active = false;
		SetTrophyRoomVisible(false);
		buttonBackTrophy.transform.parent.gameObject.SetActiveRecursively(false);
		buttonGetPointsTrophyRoom.transform.parent.gameObject.SetActiveRecursively(false);
	}

	private void UpdateTrophyRoom()
	{
		if (ButtonHarwareBackPressed)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
			}
			BackButtonPressedInTrophyRoom();
		}
		else
		{
			if (InputGUI.touchCount <= 0)
			{
				return;
			}
			if (currentInputEvent == INPUT_EVENT.TAP)
			{
				if (!zoomedIn)
				{
					hitCollider = InputGUI.GetHitCollider(touchPosition, trophyRoomCamera);
					if (!(hitCollider != null))
					{
						return;
					}
					if (buttonGetPointsTrophyRoom == hitCollider)
					{
						ShowBuyPointsMenu(true);
					}
					else if (buttonBackTrophy == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
						}
						BackButtonPressedInTrophyRoom();
					}
					else if (buttonTrophiesRight == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
						}
						menuState = MenuState.StatsWall;
						trophyRoomCamera.enabled = true;
						missionSelectCamera.enabled = false;
						buttonTrophiesLeft.gameObject.active = false;
						buttonTrophiesRight.gameObject.active = false;
						buttonGearRight.gameObject.active = false;
						buttonStatisticsLeft.gameObject.active = true;
						RefreshStatsWallText();
						statsPanel.gameObject.SetActiveRecursively(true);
						stats2Text.gameObject.SetActiveRecursively(false);
						cameraMoved = true;
						cameraTargetPosition = cameraPositionStats.position;
						cameraTargetRotation = cameraPositionStats.rotation;
						if (zoomedIn)
						{
							zoomedIn = false;
							trophyStats.gameObject.SetActiveRecursively(false);
						}
					}
					else if (buttonTrophiesLeft == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
						}
						menuState = MenuState.GearRoom;
						trophyRoomCamera.enabled = true;
						missionSelectCamera.enabled = false;
						buttonTrophiesLeft.gameObject.active = false;
						buttonTrophiesRight.gameObject.active = false;
						buttonGearRight.gameObject.active = true;
						buttonStatisticsLeft.gameObject.active = false;
						cameraMoved = true;
						cameraTargetPosition = cameraPositionGear.position;
						cameraTargetRotation = cameraPositionGear.rotation;
						if (zoomedIn)
						{
							zoomedIn = false;
							trophyStats.gameObject.SetActiveRecursively(false);
						}
					}
					else
					{
						cameraMoved = true;
						trophyStats = hitCollider.transform.GetChild(0);
						cameraTargetPosition = hitCollider.transform.position + new Vector3(0f, 0f, 0.08f);
						currentZoomedSkull = hitCollider.transform;
						trophyStats.gameObject.SetActiveRecursively(true);
						trophyStats.parent = currentZoomedSkull.parent;
						cameraTargetRotation = cameraPositionTrophies.rotation;
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundSkullPressed);
						}
						zoomedIn = true;
					}
				}
				else
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					trophyStats.gameObject.SetActiveRecursively(false);
					trophyStats.parent = currentZoomedSkull;
					cameraMoved = true;
					cameraTargetPosition = cameraPositionTrophies.position;
					cameraTargetRotation = cameraPositionTrophies.rotation;
					zoomedIn = false;
				}
			}
			else
			{
				if (currentInputEvent != INPUT_EVENT.DRAG)
				{
					return;
				}
				float x = touch.deltaPosition.x;
				if (x < -20f && Mathf.Abs(angleY) < angleLeftLimit)
				{
					menuState = MenuState.StatsWall;
					trophyRoomCamera.enabled = true;
					missionSelectCamera.enabled = false;
					buttonTrophiesLeft.gameObject.active = false;
					buttonTrophiesRight.gameObject.active = false;
					buttonGearRight.gameObject.active = false;
					buttonStatisticsLeft.gameObject.active = true;
					RefreshStatsWallText();
					statsPanel.gameObject.SetActiveRecursively(true);
					stats2Text.gameObject.SetActiveRecursively(false);
					cameraMoved = true;
					cameraTargetPosition = cameraPositionStats.position;
					cameraTargetRotation = cameraPositionStats.rotation;
					if (zoomedIn)
					{
						zoomedIn = false;
						trophyStats.gameObject.SetActiveRecursively(false);
					}
					blockInputTimer = 1f;
				}
				else if (x > 20f && Mathf.Abs(angleY / 2f) < angleLeftLimit)
				{
					menuState = MenuState.GearRoom;
					trophyRoomCamera.enabled = true;
					missionSelectCamera.enabled = false;
					buttonTrophiesLeft.gameObject.active = false;
					buttonTrophiesRight.gameObject.active = false;
					buttonGearRight.gameObject.active = true;
					buttonStatisticsLeft.gameObject.active = false;
					cameraMoved = true;
					cameraTargetPosition = cameraPositionGear.position;
					cameraTargetRotation = cameraPositionGear.rotation;
					if (zoomedIn)
					{
						zoomedIn = false;
						trophyStats.gameObject.SetActiveRecursively(false);
					}
					blockInputTimer = 1f;
				}
			}
		}
	}

	private void UpdateGearRoom()
	{
		if (ButtonHarwareBackPressed)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
			}
			BackButtonPressedInTrophyRoom();
		}
		else
		{
			if (InputGUI.touchCount <= 0)
			{
				return;
			}
			if (currentInputEvent == INPUT_EVENT.TAP)
			{
				hitCollider = InputGUI.GetHitCollider(touchPosition, trophyRoomCamera);
				if (!zoomedIn)
				{
					if (!(hitCollider != null))
					{
						return;
					}
					if (buttonGetPointsTrophyRoom == hitCollider)
					{
						ShowBuyPointsMenu(true);
						return;
					}
					if (buttonBackTrophy == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
						}
						BackButtonPressedInTrophyRoom();
						return;
					}
					if (buttonGearRight == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
						}
						menuState = MenuState.TrophyRoom;
						trophyRoomCamera.enabled = true;
						missionSelectCamera.enabled = false;
						buttonTrophiesLeft.gameObject.active = true;
						buttonTrophiesRight.gameObject.active = true;
						buttonGearRight.gameObject.active = false;
						buttonStatisticsLeft.gameObject.active = false;
						if (zoomedIn)
						{
							zoomedIn = false;
							gearStats.gameObject.SetActiveRecursively(false);
						}
						cameraMoved = true;
						cameraTargetPosition = cameraPositionTrophies.position;
						cameraTargetRotation = cameraPositionTrophies.rotation;
						return;
					}
					goodIndex = -1;
					for (int i = 0; i < gearsOnTheWall.Length; i++)
					{
						if (gearsOnTheWall[i].GearTransform == hitCollider.transform)
						{
							goodIndex = i;
							break;
						}
					}
					if (goodIndex < 0)
					{
						return;
					}
					cameraMoved = true;
					cameraTargetPosition = hitCollider.transform.position + new Vector3(-0.08f, 0.005f, 0.06f);
					gearsOnTheWall[goodIndex].activateAndRefresh(gearStats, hitCollider.transform.GetChild(0), cameraTransformWithYAt180);
					cameraTargetRotation = cameraPositionGear.rotation;
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWeaponPressed);
					}
					zoomedIn = true;
				}
				else if (hitCollider != null)
				{
					if (upgradeMesh.transform == hitCollider.transform || equipMesh.transform == hitCollider.transform)
					{
						if (gearsOnTheWall[goodIndex].Price < availableHonorPoints)
						{
							LogWeaponUpgrade();
							if (sfxOn)
							{
								base.GetComponent<AudioSource>().PlayOneShot(soundWeaponUpgraded);
							}
						}
						else if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWeaponTooExpensive);
						}
						gearsOnTheWall[goodIndex].upgrade();
					}
					else
					{
						gearStats.gameObject.SetActiveRecursively(false);
						cameraMoved = true;
						cameraTargetPosition = cameraPositionGear.position;
						cameraTargetRotation = cameraPositionGear.rotation;
						zoomedIn = false;
					}
				}
				else
				{
					gearStats.gameObject.SetActiveRecursively(false);
					cameraMoved = true;
					cameraTargetPosition = cameraPositionGear.position;
					cameraTargetRotation = cameraPositionGear.rotation;
					zoomedIn = false;
				}
			}
			else
			{
				if (touch.phase != TouchPhase.Moved)
				{
					return;
				}
				float x = touch.deltaPosition.x;
				if (x < -20f)
				{
					menuState = MenuState.TrophyRoom;
					trophyRoomCamera.enabled = true;
					missionSelectCamera.enabled = false;
					buttonTrophiesLeft.gameObject.active = true;
					buttonTrophiesRight.gameObject.active = true;
					buttonGearRight.gameObject.active = false;
					buttonStatisticsLeft.gameObject.active = false;
					if (zoomedIn)
					{
						zoomedIn = false;
						gearStats.gameObject.SetActiveRecursively(false);
					}
					cameraMoved = true;
					cameraTargetPosition = cameraPositionTrophies.position;
					cameraTargetRotation = cameraPositionTrophies.rotation;
					blockInputTimer = 1f;
				}
			}
		}
	}

	private void LogWeaponUpgrade()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("SLOT", slot.ToString());
		dictionary.Add("EQUIPMENT", gearsOnTheWall[goodIndex].Name);
		dictionary.Add("LEVEL", gearsOnTheWall[goodIndex].Level.ToString());
		//FlurryManager.Instance.LogEvent("UPGRADE", dictionary);
	}

	private void updateHPTexts()
	{
	}

	private void UpdateCameraPositionAndRotation()
	{
		angleY = cameraTargetRotation.eulerAngles.y - cameraXform.eulerAngles.y;
		angleZ = cameraTargetRotation.eulerAngles.z - cameraXform.eulerAngles.z;
		positionLeft = cameraTargetPosition - cameraXform.position;
		cameraXform.Rotate(Vector3.up, angleY * Time.deltaTime * 2f);
		if (angleZ < 70f)
		{
			cameraXform.Rotate(Vector3.forward, angleZ * Time.deltaTime * 2f);
		}
		cameraXform.position += positionLeft * Time.deltaTime * 2f;
	}

	private void UpdateStatsWall()
	{
		if (ButtonHarwareBackPressed)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
			}
			BackButtonPressedInTrophyRoom();
		}
		else
		{
			if (InputGUI.touchCount <= 0)
			{
				return;
			}
			if (currentInputEvent == INPUT_EVENT.TAP)
			{
				hitCollider = InputGUI.GetHitCollider(touchPosition, trophyRoomCamera);
				if (!(hitCollider != null))
				{
					return;
				}
				if (buttonGetPointsTrophyRoom == hitCollider)
				{
					ShowBuyPointsMenu(true);
				}
				else if (buttonBackTrophy == hitCollider)
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					BackButtonPressedInTrophyRoom();
				}
				else if (buttonStatisticsLeft == hitCollider)
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					menuState = MenuState.TrophyRoom;
					trophyRoomCamera.enabled = true;
					missionSelectCamera.enabled = false;
					buttonTrophiesLeft.gameObject.active = true;
					buttonTrophiesRight.gameObject.active = true;
					buttonGearRight.gameObject.active = false;
					buttonStatisticsLeft.gameObject.active = false;
					cameraMoved = true;
					cameraTargetPosition = cameraPositionTrophies.position;
					cameraTargetRotation = cameraPositionTrophies.rotation;
				}
				else
				{
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
					}
					if (stats1Text.gameObject.active)
					{
						stats1Text.gameObject.SetActiveRecursively(false);
						stats2Text.gameObject.SetActiveRecursively(true);
					}
					else
					{
						stats1Text.gameObject.SetActiveRecursively(true);
						stats2Text.gameObject.SetActiveRecursively(false);
					}
				}
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				float x = touch.deltaPosition.x;
				if (x > 20f)
				{
					menuState = MenuState.TrophyRoom;
					trophyRoomCamera.enabled = true;
					missionSelectCamera.enabled = false;
					buttonTrophiesLeft.gameObject.active = true;
					buttonTrophiesRight.gameObject.active = true;
					buttonGearRight.gameObject.active = false;
					buttonStatisticsLeft.gameObject.active = false;
					cameraMoved = true;
					cameraTargetPosition = cameraPositionTrophies.position;
					cameraTargetRotation = cameraPositionTrophies.rotation;
				}
			}
		}
	}

	private void UpdateByMenuStates()
	{
		if (blockInputTimer > 0f)
		{
			blockInputTimer -= Time.deltaTime;
		}
		switch (menuState)
		{
		case MenuState.MissionSelect:
			if (!blockInput)
			{
				PollInputMissionSelect();
				ProcessInputMissionSelect();
			}
			break;
		case MenuState.TrophyRoom:
			if (blockInputTimer <= 0f)
			{
				PollInputMissionSelect();
				UpdateTrophyRoom();
			}
			break;
		case MenuState.GearRoom:
			if (blockInputTimer <= 0f)
			{
				PollInputMissionSelect();
				UpdateGearRoom();
			}
			break;
		case MenuState.StatsWall:
			PollInputMissionSelect();
			UpdateStatsWall();
			break;
		case MenuState.Story:
			UpdateStory();
			break;
		}
	}

	private void UpdateStory()
	{
		if (InputGUI.touchCount > 0 && InputGUI.GetTouch(0).phase == TouchPhase.Began)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundButtonPressed);
			}
			if (storySlide == 1)
			{
				storySlide = 2;
				StoryGroupPart1.SetActiveRecursively(false);
				StoryGroupPart2.SetActiveRecursively(true);
				StoryGroupPart3.SetActiveRecursively(false);
				return;
			}
			if (storySlide == 2)
			{
				storySlide = 3;
				StoryGroupPart1.SetActiveRecursively(false);
				StoryGroupPart2.SetActiveRecursively(false);
				StoryGroupPart3.SetActiveRecursively(true);
				return;
			}
			StoryGroupPart1.SetActiveRecursively(false);
			StoryGroupPart2.SetActiveRecursively(false);
			StoryGroupPart3.SetActiveRecursively(false);
			SetTrophyRoomVisible(false);
			menuState = MenuState.MissionSelect;
			trophyRoomCamera.enabled = false;
			missionSelectCamera.enabled = true;
			buttonTrophiesLeft.gameObject.active = false;
			buttonTrophiesRight.gameObject.active = false;
			buttonGearRight.gameObject.active = false;
			buttonStatisticsLeft.gameObject.active = false;
			SetActiveMissionSelect(true);
		}
	}

	private void GetPlayerStats()
	{
		currentSlot = EncryptedPlayerPrefs.GetInt("PR_CurrentSlot");
		currentMission = EncryptedPlayerPrefs.GetInt("PR_CurrentMission_S" + currentSlot, 34);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		int num13 = 0;
		int num14 = 0;
		int num15 = 0;
		int num16 = 0;
		int num17 = 0;
		int num18 = 0;
		string key = "PR_NikolaiKills_S" + currentSlot;
		num2 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_IsabeleKills_S" + currentSlot;
		num6 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_RoyceKills_S" + currentSlot;
		num = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_CuchilloKills_S" + currentSlot;
		num4 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_HanzoKills_S" + currentSlot;
		num7 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_StansKills_S" + currentSlot;
		num5 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_NolanKills_S" + currentSlot;
		num8 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_SoldierKills_S" + currentSlot;
		num9 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_MombasaKills_S" + currentSlot;
		num3 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_BlackPredatorKills_S" + currentSlot;
		int @int = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_BerserkerKills_S" + currentSlot;
		int int2 = EncryptedPlayerPrefs.GetInt(key, 0);
		key = "PR_DogKills_S" + currentSlot;
		int int3 = EncryptedPlayerPrefs.GetInt(key, 0);
		string key2 = "PR_NikolaiTrophies_S" + currentSlot;
		num10 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_IsabeleTrophies_S" + currentSlot;
		num15 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_RoyceTrophies_S" + currentSlot;
		num11 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_CuchilloTrophies_S" + currentSlot;
		num13 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_HanzoTrophies_S" + currentSlot;
		num16 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_StansTrophies_S" + currentSlot;
		num14 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_NolanTrophies_S" + currentSlot;
		num17 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_SoldierTrophies_S" + currentSlot;
		num18 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_MombasaTrophies_S" + currentSlot;
		num12 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_BlackPredatorTrophies_S" + currentSlot;
		int int4 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_BerserkerTrophies_S" + currentSlot;
		int int5 = EncryptedPlayerPrefs.GetInt(key2, 0);
		key2 = "PR_DogTrophies_S" + currentSlot;
		int int6 = EncryptedPlayerPrefs.GetInt(key2, 0);
		if (num10 > 0 && num11 > 0 && num12 > 0 && num13 > 0 && num14 > 0 && num15 > 0 && num16 > 0 && num17 > 0 && int4 > 0 && int5 > 0 && int6 > 0)
		{
			//CrystalUnityBasic.Instance.PostAchievement("419683035", true, "Skull Collector", false);
		}
		blackPredatorsStatsKilled.text = @int.ToString();
		berserkersStatsKilled.text = int2.ToString();
		dogsStatsKilled.text = int3.ToString();
		hanzoStatsKilled.text = num7.ToString();
		nolanStatsKilled.text = num8.ToString();
		isabeleStatsKilled.text = num6.ToString();
		soldierStatsKilled.text = num9.ToString();
		mombasaStatsKilled.text = num3.ToString();
		royceStatsKilled.text = num.ToString();
		cuchilloStatsKilled.text = num4.ToString();
		stansStatsKilled.text = num5.ToString();
		nikolaiStatsKilled.text = num2.ToString();
		blackPredatorsStatsTrophies.text = int4.ToString();
		berserkersStatsTrophies.text = int5.ToString();
		dogsStatsTrophies.text = int6.ToString();
		hanzoStatsTrophies.text = num16.ToString();
		nolanStatsTrophies.text = num17.ToString();
		isabeleStatsTrophies.text = num15.ToString();
		soldierStatsTrophies.text = num18.ToString();
		mombasaStatsTrophies.text = num12.ToString();
		royceStatsTrophies.text = num11.ToString();
		cuchilloStatsTrophies.text = num13.ToString();
		stansStatsTrophies.text = num14.ToString();
		nikolaiStatsTrophies.text = num10.ToString();
		soldiersWordLanguage.text = Language.GetTxt("SOLDIERS");
		if (num16 == 0)
		{
			hanzoStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			hanzoStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num17 == 0)
		{
			nolanStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			nolanStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num15 == 0)
		{
			isabeleStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			isabeleStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num18 == 0)
		{
			soldierStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			soldierStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num12 == 0)
		{
			mombasaStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			mombasaStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num11 == 0)
		{
			royceStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			royceStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num13 == 0)
		{
			cuchilloStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			cuchilloStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num14 == 0)
		{
			stansStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			stansStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (num10 == 0)
		{
			nikolaiStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			nikolaiStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (int4 == 0)
		{
			blackPredatorsStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			blackPredatorsStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (int5 == 0)
		{
			berserkersStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			berserkersStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
		if (int6 == 0)
		{
			dogsStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullNoTrophyKills;
		}
		else
		{
			dogsStatsTrophies.transform.parent.parent.GetComponent<Renderer>().material.color = skullWithTrophyKills;
		}
	}

	private IEnumerator MoveMatrices()
	{
		while (true)
		{
			float deltaTime = Time.deltaTime * 2f;
			movingMatrices[0].transform.Translate(deltaTime, 0f, 0f);
			movingMatrices[1].transform.Translate(deltaTime * 0.4f, 0f, 0f);
			movingMatrices[2].transform.Translate(deltaTime * 0.7f, 0f, 0f);
			movingMatrices[3].transform.Translate(deltaTime * 0.5f, 0f, 0f);
			movingMatrices[4].transform.Translate(deltaTime * 0.3f, 0f, 0f);
			movingMatrices[5].transform.Translate(deltaTime * 0.9f, 0f, 0f);
			for (int i = 0; i < 6; i++)
			{
				if (movingMatrices[i].transform.position.x < matrixResetPosition)
				{
					movingMatrices[i].transform.Translate(2f * matrixResetPosition, 0f, 0f);
				}
			}
			yield return null;
		}
	}

	private void Update()
	{
		PlatformDependent.SetScreenOrientation(true);
		if (menuState == MenuState.GearRoom || menuState == MenuState.StatsWall || menuState == MenuState.TrophyRoom)
		{
			PlatformDependent.UpdateMouseCursorGUITextureInTrophyRoom(mouseCursor);
		}
		else
		{
			PlatformDependent.UpdateMouseCursorGUITexture(mouseCursor);
		}
	}

	private IEnumerator UpdateCoroutine()
	{
		while (true)
		{
			if (!isInBuyPointsMenu)
			{
				UpdateByMenuStates();
				if (cameraMoved)
				{
					UpdateCameraPositionAndRotation();
				}
				if ((menuState == MenuState.GearRoom || menuState == MenuState.StatsWall || menuState == MenuState.TrophyRoom) && !buttonBackTrophy.gameObject.active)
				{
					yield return new WaitForSeconds(2f);
					buttonBackTrophy.transform.parent.gameObject.SetActiveRecursively(true);
					buttonGetPointsTrophyRoom.transform.parent.gameObject.SetActiveRecursively(true);
				}
				if (menuState == MenuState.GearRoom && textToDisplay != string.Empty)
				{
					yield return new WaitForSeconds(1f);
					yield return StartCoroutine(WeaponIsUnlockedCR(textToDisplay));
					textToDisplay = string.Empty;
				}
			}
			yield return null;
		}
	}

	private void AllignAfter(int index)
	{
		for (int i = index + 1; i < missionSquaresCampaign.Length; i++)
		{
			missionSquaresCampaign[i].transform.position = new Vector3(missionSquaresCampaign[i - 1].transform.position.x + distanceBetweenIcons, missionSquaresCampaign[i - 1].transform.position.y, missionSquaresCampaign[i - 1].transform.position.z);
		}
		for (int num = index - 1; num >= 0; num--)
		{
			missionSquaresCampaign[num].transform.position = new Vector3(missionSquaresCampaign[num + 1].transform.position.x - distanceBetweenIcons, missionSquaresCampaign[num + 1].transform.position.y, missionSquaresCampaign[num + 1].transform.position.z);
		}
	}

	private void SetSelectedMissionEnvironment()
	{
		switch (selectedMission)
		{
		case 0:
		case 1:
		case 2:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 1);
			break;
		case 3:
		case 4:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 7);
			break;
		case 5:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 1);
			break;
		case 6:
		case 7:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 2);
			break;
		case 8:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 1);
			break;
		case 9:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 2);
			break;
		case 10:
		case 11:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 5);
			break;
		case 12:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 1);
			break;
		case 13:
		case 14:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 4);
			break;
		case 15:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 7);
			break;
		case 16:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 1);
			break;
		case 17:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 6);
			break;
		case 18:
		case 19:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 7);
			break;
		case 20:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 4);
			break;
		case 21:
		case 22:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 3);
			break;
		case 23:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 7);
			break;
		case 24:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 5);
			break;
		case 25:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 2);
			break;
		case 26:
		case 27:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 7);
			break;
		case 28:
		case 29:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 4);
			break;
		case 30:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 7);
			break;
		case 31:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 1);
			break;
		case 32:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 3);
			break;
		default:
			EncryptedPlayerPrefs.SetInt("PR_CurrentJungleType", 1);
			break;
		}
	}
}
