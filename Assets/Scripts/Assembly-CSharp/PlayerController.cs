using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public class AnimCombo
	{
		public AnimCombo nextAnimationCombo;

		public AttackAnimation animationToPerform;

		public AttackAnimation secondaryAnimation;

		public bool animationTypeLight;

		public float animationComboLength;

		public float animationEndLength;
	}

	public enum AttackAnimation
	{
		None = 0,
		WristLightL = 1,
		WristLightR = 2,
		WristHeavyR = 3,
		WristSpinL = 4,
		WristScissor = 5,
		WristGrabStart = 6,
		WristGrabChopHi = 7,
		WristGrabChopLow = 8,
		WristGrabCutHalf = 9,
		WristGrabHeadOff = 10,
		SpearLightL = 11,
		SpearLightR = 12,
		SpearLightFront = 13,
		SpearHeavyL = 14,
		SpearHeavyR = 15,
		SpearHeavyFront = 16,
		SpearHeavySpin = 17,
		HurtHeavyMoveBack = 18,
		WhipLightL = 19,
		WhipLightR = 20,
		WhipSpin = 21,
		WhipHeavyR = 22,
		WhipHeavyF = 23,
		WhipDashGrabStart = 24
	}

	private enum WeaponType
	{
		Ranged = 0,
		Melee = 1
	}

	public enum InputDevice
	{
		Touch = 0,
		XperiaPlay = 1,
		PC = 2
	}

	public class Ability
	{
		public bool lightAttack;

		public bool heavyAttack;

		public bool spinAttack;

		public bool thermalVision;

		public bool trophyKill;

		public bool blockUnlocked;

		public bool plasmaGunUnlocked;

		public bool diskUnlocked;

		public bool spearUnlocked;

		public bool whipUnlocked;

		public bool netGunUnlocked;

		public bool cloakUnlocked;

		public bool impale;
	}

	private enum TipState
	{
		Show = 0,
		Hide = 1
	}

	private class Tip
	{
		protected Transform tipBackground;

		protected string message;

		protected float timeToWaitAtLeast;

		protected float timeToWaitUntilDisplay;

		protected float timeToWaitUntilHide;

		protected TipCondition displayCondition;

		protected TipCondition hideCondition;

		protected TipCondition tipAction;

		public string Message
		{
			get
			{
				return PlatformDependent.TranslateKeybinds(message);
			}
		}

		public float TimeWaitAtLeast
		{
			get
			{
				return timeToWaitAtLeast;
			}
		}

		public Tip()
		{
		}

		public Tip(Transform aGuiTexture, string aMessage, float aTimeToWaitUntilDisplay, float aTimeToWaitUntilHide, TipCondition aDisplayCondition, TipCondition aHideCondition, TipCondition aTipAction)
		{
			timeToWaitAtLeast = 0f;
			tipBackground = aGuiTexture;
			message = aMessage;
			timeToWaitUntilDisplay = aTimeToWaitUntilDisplay;
			timeToWaitUntilHide = aTimeToWaitUntilHide;
			displayCondition = aDisplayCondition;
			hideCondition = aHideCondition;
			tipAction = aTipAction;
		}

		public Tip(float aTimeToWaitAtLeast, Transform aGuiTexture, string aMessage, float aTimeToWaitUntilDisplay, float aTimeToWaitUntilHide, TipCondition aDisplayCondition, TipCondition aHideCondition, TipCondition aTipAction)
		{
			timeToWaitAtLeast = aTimeToWaitAtLeast;
			tipBackground = aGuiTexture;
			message = aMessage;
			timeToWaitUntilDisplay = aTimeToWaitUntilDisplay;
			timeToWaitUntilHide = aTimeToWaitUntilHide;
			displayCondition = aDisplayCondition;
			hideCondition = aHideCondition;
			tipAction = aTipAction;
		}

		public IEnumerator waitSecondsAtLeast()
		{
			yield return new WaitForSeconds(timeToWaitAtLeast);
		}

		public IEnumerator waitSecondsBeforeDisplay()
		{
			yield return new WaitForSeconds(timeToWaitUntilDisplay);
		}

		public IEnumerator waitSecondsBeforeHide()
		{
			yield return new WaitForSeconds(timeToWaitUntilHide);
		}

		public bool isDisplayable()
		{
			return displayCondition();
		}

		public virtual bool isUnderstood()
		{
			return hideCondition() && elapsedShowTipTime >= timeToWaitAtLeast;
		}

		public void display()
		{
			tipBackground.gameObject.SetActiveRecursively(true);
			((TextMesh)tipBackground.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = PlatformDependent.TranslateKeybinds(message);
		}

		public bool performTipAction()
		{
			return tipAction();
		}

		public void hide()
		{
			tipBackground.gameObject.SetActiveRecursively(false);
		}
	}

	private class TapTip : Tip
	{
		public TapTip(float aTimeToWaitAtLeast, Transform aGuiTexture, string aMessage, float aTimeToWaitUntilDisplay, float aTimeToWaitUntilHide, TipCondition aDisplayCondition, TipCondition aHideCondition, TipCondition aTipAction)
		{
			timeToWaitAtLeast = aTimeToWaitAtLeast;
			tipBackground = aGuiTexture;
			message = aMessage;
			timeToWaitUntilDisplay = aTimeToWaitUntilDisplay;
			timeToWaitUntilHide = aTimeToWaitUntilHide;
			displayCondition = aDisplayCondition;
			hideCondition = aHideCondition;
			tipAction = aTipAction;
		}

		public override bool isUnderstood()
		{
			return hideCondition() || elapsedShowTipTime >= timeToWaitAtLeast;
		}
	}

	private delegate bool TipCondition();

	private const float animWristAttackLightLHit = 0.1f;

	private const float animWristAttackLightLMove = 2f / 15f;

	private const float animWristAttackLightLEnd = 5f / 6f;

	private const float animWristAttackLightRHit = 0.1f;

	private const float animWristAttackLightRMove = 2f / 15f;

	private const float animWristAttackLightREnd = 5f / 6f;

	private const float animWristAttackHeavyRStart = 4f / 15f;

	private const float animWristAttackHeavyRMove = 0.2f;

	private const float animWristAttackHeavyREnd = 0.6f;

	private const float animWristAttackHeavySpinStart = 0.1f;

	private const float animWristAttackHeavySpinHit1 = 0.2f;

	private const float animWristAttackHeavySpinHit2 = 2f / 15f;

	private const float animWristAttackHeavySpinEnd = 19f / 30f;

	private const float animDashStartIdle = 1f / 6f;

	private const float animDashStartMove = 1f / 6f;

	private const float animDashLandMove = 1f / 3f;

	private const float animDashLandEnd = 0.5f;

	private const float animDashAttackLandMove = 7f / 30f;

	private const float animDashAttackScissorMove = 1f / 6f;

	private const float animDashAttackScissorEnd = 0.5f;

	private const float animWristGrabStartMove = 0.2f;

	private const float animWristGrabStartEnd = 1f / 3f;

	private const float animWristGrabChopHiStart = 7f / 30f;

	private const float animWristGrabChopHiEnd = 5f / 6f;

	private const float animWristGrabChopLowStart = 7f / 30f;

	private const float animWristGrabChopLowEnd = 23f / 30f;

	private const float animWristGrabCutHalf1MoveBack = 7f / 30f;

	private const float animWristGrabCutHalf2Idle = 8f / 15f;

	private const float animWristGrabCutHalf3MoveFwd = 1f / 15f;

	private const float animWristGrabCutHalf4Idle = 0.8f;

	private const float animWristGrabCutHalf5End = 1.0666667f;

	private const float animWristGrabHeadOffIdle1 = 13f / 15f;

	private const float animWristGrabHeadOffIdle2 = 17f / 30f;

	private const float animWristGrabHeadOff5End = 1.5333333f;

	private const float animSpearAttackLightRStartIdle = 2f / 15f;

	private const float animSpearAttackLightRStartMove = 0.2f;

	private const float animSpearAttackLightREnd = 1f / 3f;

	private const float animSpearAttackLightLStartIdle = 2f / 15f;

	private const float animSpearAttackLightLStartMove = 0.2f;

	private const float animSpearAttackLightLEnd = 1f / 3f;

	private const float animSpearAttackLightFStart = 7f / 30f;

	private const float animSpearAttackLightFMove = 1f / 6f;

	private const float animSpearAttackLightFEnd = 0.6f;

	private const float animSpearAttackHeavyRMove = 2f / 15f;

	private const float animSpearAttackHeavyREnd = 1f;

	private const float animSpearAttackHeavyLMove1 = 1f / 6f;

	private const float animSpearAttackHeavyLMove2 = 13f / 30f;

	private const float animSpearAttackHeavyLIdle3 = 1f / 15f;

	private const float animSpearAttackHeavyLIdle4 = 7f / 15f;

	private const float animSpearAttackHeavyLMove5 = 0.2f;

	private const float animSpearAttackHeavyLIdle6 = 0.2f;

	private const float animSpearAttackHeavyFStart = 17f / 30f;

	private const float animSpearAttackHeavyFEnd = 1.1666666f;

	private const float animSpearAttackHeavySpinStart = 11f / 30f;

	private const float animSpearAttackHeavySpinEnd = 1.3333334f;

	private const float animWhipLightR_Idle = 0.2f;

	private const float animWhipLightR_Move1 = 0.1f;

	private const float animWhipLightR_Move2 = 1f / 15f;

	private const float animWhipLightR_Idle_End = 0.3f;

	private const float animWhipLightL_Idle = 0.2f;

	private const float animWhipLightL_Move1 = 0.1f;

	private const float animWhipLightL_Move2 = 1f / 15f;

	private const float animWhipLightL_Idle_End = 0.3f;

	private const float animWhipSpin_Idle = 7f / 30f;

	private const float animWhipSpin_Move2Frames = 1f / 15f;

	private const float animWhipSpin_Idle_End = 8f / 15f;

	private const float animWhipHeavyR_Move = 2f / 15f;

	private const float animWhipHeavyR_Idle2 = 1f / 15f;

	private const float animWhipHeavyR_Idle3 = 1f / 15f;

	private const float animWhipHeavyF_Move = 11f / 30f;

	private const float animWhipHeavyF_Idle = 8f / 15f;

	private const float animGrappleStartIdle = 0.3f;

	private const float animGrappleDashIdle = 0.1f;

	private const float animGrappleDashMove = 7f / 30f;

	private const float animGrappleDashGrabMove = 0.2f;

	private const float animGrappleDashGrabIdle = 2f / 15f;

	private const float animGrappleFailIdle = 0.5f;

	private const float animYellLength = 2.26f;

	public GUITexture mouseCursor;

	private AttackAnimation currentPerformingAnimation;

	public int debugCurrentMissionAutoStart = 24;

	public int debugJungleType = 1;

	public bool slowMotion;

	public bool liteVersion = true;

	public float health = 1500f;

	private float healthUpperLimit = 4000f;

	public float energy = 39f;

	public float energyCloakConsume = 10f;

	public float moveSpeed = 5f;

	public float rotateSpeed = 8f;

	public float blockingDamageMultiplier = 0.2f;

	public SuperBlackPredator superBlackPredator;

	public Material materialCloak;

	public Material materialNormal;

	public Material materialBerserkers;

	public Material materialThermalCloaked;

	public Material[] bloodMaterialsHuman;

	public Material[] bloodMaterialsPredator;

	public Color bloodColorThermal = Color.yellow;

	public Color bloodColorHuman = Color.red;

	public Color bloodColorPredator = Color.green;

	public Color ColorDaytime = new Color(58f / 85f, 58f / 85f, 58f / 85f);

	public Color ColorNighttime = new Color(0.36078432f, 0.41960785f, 58f / 85f);

	public Color ColorRainy = new Color(0.36078432f, 0.41960785f, 58f / 85f);

	private Texture TextureCharactersNormal;

	private Texture TextureCharactersThermal;

	private Texture TextureBerserkersNormal;

	public Texture TextureBerserkersThermal;

	public ParticleEmitter particlePlasmaGunEmitter;

	public ParticleEmitter particleBloodSprayHeavy;

	public ParticleEmitter particleBloodFountainDeadHead;

	public ParticleEmitter particleBloodSprayLight;

	public ParticleEmitter particleSparks;

	public Transform bloodParent;

	private float damageHeavyLimit = 100f;

	public LineRenderer VisibilityLineRenderer;

	public LineRenderer LaserLineRenderer;

	public TrailRenderer TrailRendererHandR;

	public TrailRenderer TrailRendererHandL;

	public TrailRenderer TrailRendererSpearFront;

	public TrailRenderer TrailRendererSpearBack;

	public GameObject TrailDisc;

	public GameObject WhipWeapon;

	private Animation whipAnim;

	public Transform WhipMidPoint;

	public Transform WhipEndPoint;

	public TrailRenderer WhipTrailRenderer;

	public LineRenderer WhipLineRenderer;

	private Transform WhipXForm;

	public Material MaterialPlasmaGunParticle;

	public Material MaterialNetGun;

	public GameObject AO_Shadow;

	private WeaponType currentWeapon;

	public Transform TriangleProgress;

	public Material HealthBarMaterial;

	public float ControlPadRadius = 1f / 64f;

	private int[] symbols = new int[5] { 9, 9, 9, 9, 9 };

	public GameObject PauseMenuGroup;

	public GameObject OptionsMenuGroup;

	public GameObject CombosMenuGroup;

	public GameObject ObjectivesMenuGroup;

	public GameObject RestartMenuGroup;

	public GameObject QuitMenuGroup;

	public SimpleFollowCamera MainCamera;

	public TextMesh GUI_CombosText;

	public TextMesh GUI_ObjectivesText;

	public TextMesh GUI_ObjectivesTitleText;

	public Collider restartYesButton;

	public Collider restartNoButton;

	public Collider quitYesButton;

	public Collider quitNoButton;

	public Collider bloodButton;

	public Collider musicButton;

	public Collider sfxButton;

	public Camera cameraPause;

	public Button buttonResume;

	public TextMesh optionsTextRestart;

	public TextMesh optionsTextOptions;

	public TextMesh optionsTextComboList;

	public TextMesh optionsTextObjectives;

	public TextMesh optionsTextQuitToMenu;

	public TextMesh GUI_OptionsQuitConfirmText;

	public TextMesh GUI_OptionsRestartConfirmText;

	public Collider failMissionButtonQuit;

	public Collider failMissionButtonRetry;

	public Transform toolTipBackgroundMissionEnd;

	public TextMesh failMissionTextMessage;

	public Collider successMissionButtonOk;

	public TextMesh successMissionTextMessage;

	public GUITexture GUI_FadeToBlack;

	public float fadeToBlackTime = 90f;

	public Transform netGunPoolParent;

	public Transform discL1;

	public TrailRenderer discTrailRenderer;

	public GameObject HeadSoldierGeneric;

	public GameObject HeadRoyce;

	public GameObject HeadStans;

	public GameObject playerMesh;

	public GameObject faceMaskMesh1;

	public GameObject faceMaskMesh2;

	public GameObject faceMaskMesh3;

	public GameObject faceMaskMesh4;

	public GameObject WristBladeLMesh;

	public GameObject WristBladeRMesh;

	public GameObject PlasmaCannonMesh;

	public GameObject SpearMesh;

	public Transform BoneMixSpine;

	public Transform BoneMixChest;

	public Transform BoneMixHead;

	public Transform BoneMixUpperArmL;

	public Transform BoneMixLowerArmL;

	public Transform BoneMixUpperArmR;

	public Transform BoneMixLowerArmR;

	public MeshRenderer glowCircle;

	public Vector3 discOffset = new Vector3(0.39f, 0.97f, 0.37f);

	public Vector3 wristBladesOffset = new Vector3(0.1f, 1.12f, 0.7f);

	public Vector3 spearOffset = new Vector3(0.1f, 1.12f, 0.7f);

	public float muzzleFlashTime = 0.1f;

	public Transform groundPlane;

	public AudioClip soundDie;

	public AudioClip soundYell;

	public AudioClip soundChangeWeapon;

	public AudioClip soundChangeVisionMode;

	public AudioClip soundVisionModeLoop;

	public AudioClip soundTriangulateTarget;

	public AudioClip soundTriangulateInProgress;

	public AudioClip soundLockTarget;

	public AudioClip soundCloakOn;

	public AudioClip soundCloakFail;

	public AudioClip soundCloakOff;

	public AudioClip soundIdleClicks1;

	public AudioClip soundIdleClicks2;

	public AudioClip soundGetHit;

	public AudioClip soundGetHit2;

	public AudioClip soundGetHurtHeavy;

	public AudioClip soundGetHitBlocking;

	public AudioClip soundNetGunShoot;

	public AudioClip soundWristBladeAttack;

	public AudioClip soundWristBladeHitDry;

	public AudioClip soundWristBladeHitImpale;

	public AudioClip soundWristBladeHitSquish;

	public AudioClip soundWristBladeHitSquish2;

	public AudioClip soundTrophyRipHalf;

	public AudioClip soundWaterSplash;

	public AudioClip soundTrophyHeadOff;

	public AudioClip soundSpearAttack;

	public AudioClip soundWhipAttack1;

	public AudioClip soundWhipAttack2;

	public AudioClip soundWhipSpinAttack;

	public AudioClip soundWhipHit;

	public AudioClip soundWhipHitHeavy;

	public AudioClip soundSpearHit;

	public AudioClip soundSpearHit2;

	public AudioClip soundPlasmaGunShoot;

	public AudioClip soundDashJump;

	public AudioClip soundDiscThrow;

	public AudioClip soundDiscImpactEnemy;

	public AudioClip soundBreakApartEnemy;

	public AudioClip soundPredatorAngry;

	public AudioClip soundEnergyDepleted;

	public AudioClip soundEnergyRecharging;

	public AudioClip menuClick;

	public AudioClip menuClickBack;

	public AudioClip soundBloodSplatScreen;

	public Color invisibleColor = new Color(1f, 29f / 85f, 29f / 85f);

	public Color thermalColor = new Color(1f, 78f / 85f, 0.45490196f);

	public TriangleTarget triangleTarget;

	private float checkEnemyTargetsFrequency = 0.5f;

	private float dashRadius = 5f;

	private float timerComboHeavy = 0.4f;

	private float timerComboLight = 0.2f;

	private float timerHeavySpinInputTolerance = 0.6f;

	private float timerGrabEnemy = 1.3f;

	public int debugCurrentMaskType = 1;

	public int debugWristBladesLevel = 1;

	public int debugCombiStickLevel = 1;

	public int debugWhipLevel = 1;

	public int debugPlasmaGunLevel = 1;

	public int debugDiscLevel = 1;

	public int debugNetGunLevel = 1;

	public GameObject loadingScreen;

	public GameObject loadingScreenLeft;

	private float wristBladesCheckRadius = 2f;

	private float wristBladesDamageMin = 20f;

	private float wristBladesDamageMax = 35f;

	private float wristBladesDamageRadius = 1.23f;

	private float spearDamageMin = 35f;

	private float spearDamageMax = 55f;

	private float spearCheckRadius = 2.5f;

	private float spearDamageRadiusMin = 1f;

	private float spearDamageRadiusMax = 1.5f;

	private float whipDamageMin = 30f;

	private float whipDamageMax = 50f;

	private float whipCheckRadius = 5f;

	private float whipDamageRadiusMin = 2f;

	private float whipDamageRadiusMax = 3f;

	private float plasmaDamageMin = 50f;

	private float plasmaDamageMax = 80f;

	private float plasmaGunReloadTimeMin = 0.5f;

	private float plasmaGunReloadTimeMax = 0.2f;

	private float plasmaGunEnergyDecreaseRateMin = 0.15f;

	private float plasmaGunEnergyDecreaseRateMax = 0.08f;

	private float discDamageMin = 60f;

	private float discDamageMax = 120f;

	private float discThrowAgainTimeMin = 1.5f;

	private float discThrowAgainTimeMax = 1f;

	private float discEnergyConsumeMin = 15f;

	private float discEnergyConsumeMax = 7f;

	private float discDamageRadiusSqr = 1.69f;

	private float netGunReloadTimeMin = 1.5f;

	private float netGunReloadTimeMax = 1f;

	private float cloakEnergyDecreaseRate = 0.5f;

	private float wristBladesDamage = 40f;

	private float spearDamageRadius = 1f;

	private float spearDamage = 10f;

	private float whipDamage = 10f;

	private float whipDamageRadius = 1f;

	private BulletParticle plasmaBulletParticle;

	private float plasmaDamage = 10f;

	private float plasmaGunReloadTime = 1f / 15f;

	private float plasmaGunEnergyDecreaseRate = 0.03f;

	private float netGunSpeed = 22f;

	private float netGunReloadTime = 1f;

	private float netGunEnergyDecreaseRate = 1f;

	private float discSpeed = 28f;

	private float discDamage = 150f;

	private float discThrowAgainTime = 1f;

	private float discEnergyConsume = 7f;

	private Transform superPredatorTarget;

	private bool superPredatorIsInvisible;

	private Whip whipComponent;

	private Vector2 touchPosition = Vector2.zero;

	private int maxLevelUnlocked = 1;

	private Collider hitCollider;

	private float netGunCapturedTimeNetGunL1 = 5f;

	private float netGunCapturedTimeNetGunL2 = 7f;

	private float netGunCapturedTimeNetGunL3 = 10f;

	private float netGunCapturedTime = 5f;

	private Color TrailColorWeaponLevel1 = new Color(0.49803922f, 0.49803922f, 0.49803922f);

	private Color TrailColorWeaponLevel2 = new Color(0.76862746f, 48f / 85f, 0.12156863f);

	private Color TrailColorWeaponLevel3 = new Color(0.6901961f, 0f, 0f);

	private Color ColorNetGunLevel1 = new Color(0.21960784f, 0.23921569f, 0.2901961f);

	private Color ColorNetGunLevel2 = new Color(0.4117647f, 0.3019608f, 1f / 15f);

	private Color ColorPlasmaGunLevel1 = new Color(0f, 0.5137255f, 71f / 85f);

	private Color ColorPlasmaGunLevel2 = new Color(0.49803922f, 31f / 85f, 11f / 85f);

	private Color ColorPlasmaGunLevel3 = new Color(0.8784314f, 8f / 51f, 9f / 85f);

	private float TrailWidthStartLevel1 = 0.1f;

	private float TrailWidthStartLevel2 = 0.2f;

	private float TrailWidthStartLevel3 = 0.4f;

	private Vector3 ThermalFadeRightPosition = new Vector3(10.54f, 0f, 0.358f);

	private Vector3 ThermalFadeLeftPosition = new Vector3(-11.1f, 0f, 0.358f);

	private int touchZoneRadius = 64;

	private bool bloodOn = true;

	private bool tipActiveAndWaitingTap;

	private int currentJungleType = 1;

	private Vector3 takeDamageDirection;

	private bool waterTouching;

	private float cloakFailSeconds = 1f;

	private bool rainyEnvironment;

	private GameObject EnvironmentNormalVision;

	private GameObject EnvironmentThermalVision;

	private bool triangulationInProgress;

	private float animationRunSpeed = 1.1f;

	private GameObject faceMaskMesh;

	private int bloodMaterialsHumanCount;

	private int bloodMaterialsPredatorCount;

	private int currentMission;

	private Color currentCharactersColor;

	private float laserLineLength = 10f;

	private RaycastHit hitInfo;

	private Vector3 LaserLineStartOffset = new Vector3(-0.15f, 1.47f, 0.449999f);

	private AnimCombo[] wristComboAttack = new AnimCombo[4];

	private AnimCombo[] spearComboLightAttack = new AnimCombo[4];

	private AnimCombo[] spearComboHeavyAttack = new AnimCombo[3];

	private AnimCombo[] whipComboLightAttack = new AnimCombo[4];

	private AnimCombo[] whipComboHeavyAttack = new AnimCombo[2];

	private AnimCombo[] wristGrabComboAttack = new AnimCombo[3];

	private AnimCombo[] wristGrabComboSecondaryAttack = new AnimCombo[3];

	private AnimCombo wristScissorCombo;

	private AnimCombo spearScissorCombo;

	public LayerMask cameraCullingMaskNormal = -1073741825;

	public LayerMask laserLineCullingMask = 2048;

	public LayerMask cameraCullingMaskThermal = -131073;

	private Camera cameraMain;

	private GameObject headOffPrey;

	private bool performingAttackAnimation;

	private bool comboAttackLight;

	private bool comboAttackHeavy;

	private bool comboAttackSecondary;

	private float speedDash = 12f;

	private float speedWristAttack = 1.8f;

	private bool chargingHeavyAttack;

	private bool dashJumping;

	private bool meleeLightAttacking;

	private bool missionFailed;

	private bool grabbingEnemy;

	private bool gettingHurt;

	private bool blocking;

	private bool missionSuccess;

	private AttackInfo attackInfoPredator;

	private ArrayList enemyTargets;

	private Transform currentTarget;

	private Transform currentGrabbedTarget;

	private bool newTargetFound;

	private float timerNewTargetRotate = 2f;

	private float timerNewTargetRotateFull = 2f;

	private Quaternion targetRotation;

	private Transform targetTriangleTransform;

	private bool targetLocked;

	private int enemyTargetIndex;

	private int energyConsumers;

	private int currentWeaponType;

	private int weaponA;

	private int weaponB;

	private float energyIncreaseDelay = 0.1f;

	private bool cloakModeOn;

	private float reloadTime = 0.2f;

	private bool reloadedAtEnd;

	private Vector3 LeftStickCenterLocalPosition;

	private Color originalColor;

	private bool skipStats;

	private bool finishedShowingStats;

	private float maxHealth;

	private float healthMultiplier = 1f;

	private float energyMultiplier = 1f;

	private float moveSpeedMultiplier = 1f;

	private Animation anim;

	private bool statsMenu;

	private bool pauseMenu;

	private bool optionsMenu;

	private bool restartMenu;

	private bool quitMenu;

	private bool leftStickDown;

	private bool attackButtonDown;

	private bool blockButtonDown;

	private Rect leftStickBounds;

	private Rect weaponABounds;

	private Rect weaponBBounds;

	private Rect[] tipTexturesBounds = new Rect[7]
	{
		default(Rect),
		default(Rect),
		default(Rect),
		default(Rect),
		default(Rect),
		default(Rect),
		default(Rect)
	};

	private Vector2 leftStickCenter;

	private Rect iPadLeftScreenBounds;

	private Rect visionModeBounds;

	private Rect cloakModeBounds;

	private Rect buttonBlock;

	private Rect buttonAttack;

	private int leftStickTouchId = -1;

	private CharacterController playerController;

	private Vector2 lDiff;

	private Vector2 rDiff;

	private Vector3 lDiff3;

	private Vector3 rDiff3;

	private GlowEffect glowEffect;

	private NoiseEffect noiseEffect;

	private Vignetting vignetting;

	private bool m_paused;

	private bool moved;

	private bool shooting;

	private bool musicOn = true;

	private int weaponLevelWristBlades = 1;

	private int weaponLevelSpear = 1;

	private int weaponLevelWhip = 1;

	private int weaponLevelPlasmaGun = 1;

	private int weaponLevelDisc = 1;

	private int weaponLevelNetGun = 1;

	private Vector3 shootingPointOffset = new Vector3(-0.2453f, 1.4136f, 0.2465f);

	private bool shootingReloadableWeapon;

	private int netGunPoolCount;

	private ArrayList netGunPool;

	private int currentNetGunIndex;

	private bool dead;

	private bool isControllable = true;

	private bool musicButActive = true;

	private bool reloadReady = true;

	private Color originalHitColor;

	private Transform currentProjectileObject;

	private Transform xForm;

	private Transform enemyTempTransform;

	private bool thermalVisionMode;

	private bool pressedComboHeavy;

	private bool pressedComboLight;

	private bool pressedComboSecondary;

	private GameObject GUI_currentKillsIcon;

	private float minTargetDistanceSqr = 1f;

	private BaseEnemy grabbedEnemy;

	private float verticalAspectRatio = 1f;

	private int peopleKilledByDisk;

	private int currentSlot;

	private float thermalVisionUsageTime;

	private Queue<Transform> enemiesHitByDisk = new Queue<Transform>();

	public InputDevice inputDevice;

	private Touch androidTouch;

	private Ability ability = new Ability();

	private int currentTipTextureIndex;

	private int startIndex;

	private int endIndex;

	private ArrayList tips;

	private TipState currentTipState = TipState.Hide;

	private bool conditionPerformedHeavyAttackHit;

	public SurvivalMissionController survivalMissionController;

	private bool leftStickBlinked;

	private bool energyBlinked;

	private bool texture4IsTapped;

	private bool texture3IsTapped;

	private bool texture2IsTapped;

	private bool cloakButtonBlinked;

	private bool rangedWeaponBlinked;

	public static float elapsedShowTipTime;

	private bool tipIsUnderstood;

	public HUD hud;

	public HUD phoneHUD;

	public HUD tabletHUD;

	public GameObject BloodSplatScreenLong;

	private bool sfxOn
	{
		get
		{
			return GameConstants.sfxOn;
		}
		set
		{
			GameConstants.sfxOn = value;
		}
	}

	private bool paused
	{
		get
		{
			return m_paused;
		}
		set
		{
			m_paused = value;
		}
	}

	public bool ThermalVisionMode
	{
		get
		{
			return thermalVisionMode;
		}
	}

	public bool SuperPredatorIsInvisible
	{
		set
		{
			superPredatorIsInvisible = value;
		}
	}

	public Transform SuperPredatorTarget
	{
		set
		{
			superPredatorTarget = value;
		}
	}

	public float NetGunCapturedTime
	{
		get
		{
			return netGunCapturedTime;
		}
	}

	public int CurrentMission
	{
		get
		{
			return currentMission;
		}
	}

	public bool PredatorMoved
	{
		get
		{
			return moved;
		}
	}

	public int CurrentWeaponType
	{
		get
		{
			return currentWeaponType;
		}
	}

	public int MaxLevelUnlocked
	{
		get
		{
			return maxLevelUnlocked;
		}
	}

	public int PeopleKilledByDisk
	{
		get
		{
			return peopleKilledByDisk;
		}
		set
		{
			peopleKilledByDisk = value;
			if (peopleKilledByDisk >= 3 && currentMission == 16)
			{
				survivalMissionController.MissionCompleted = true;
			}
		}
	}

	private string AnimGrabStart
	{
		get
		{
			if (currentWeaponType == 4)
			{
				return "whip_grapple_dash_grab";
			}
			return "grab_start";
		}
	}

	public int JungleType
	{
		get
		{
			return currentJungleType;
		}
	}

	public Ability PredatorAbility
	{
		get
		{
			return ability;
		}
	}

	public AttackAnimation CurrentAttackAnimation
	{
		get
		{
			return currentPerformingAnimation;
		}
	}

	private GameObject horizontalLinesBackground
	{
		get
		{
			return hud.horizontalLinesBackground;
		}
	}

	private Transform GUI_Symbol1Parent
	{
		get
		{
			return hud.GUI_Symbol1Parent;
		}
	}

	private Transform GUI_Symbol2Parent
	{
		get
		{
			return hud.GUI_Symbol2Parent;
		}
	}

	private Transform GUI_Symbol3Parent
	{
		get
		{
			return hud.GUI_Symbol3Parent;
		}
	}

	private Transform GUI_Symbol4Parent
	{
		get
		{
			return hud.GUI_Symbol4Parent;
		}
	}

	private TextMesh GUI_BloodKillTextScore
	{
		get
		{
			return hud.GUI_BloodKillTextScore;
		}
	}

	private Transform GUI_BloodSplatScreen
	{
		get
		{
			return hud.GUI_BloodSplatScreen;
		}
	}

	private TextMesh missionStatus
	{
		get
		{
			return hud.missionStatus;
		}
	}

	private Transform LeftStick
	{
		get
		{
			return hud.LeftStick;
		}
	}

	private Transform BlockStick
	{
		get
		{
			return hud.BlockStick;
		}
	}

	private Transform AttackStick
	{
		get
		{
			return hud.AttackStick;
		}
	}

	private GameObject LeftControlPad
	{
		get
		{
			return hud.LeftControlPad;
		}
	}

	private GameObject GUI_Thermal
	{
		get
		{
			return hud.GUI_Thermal;
		}
	}

	private GameObject GUI_PauseButton
	{
		get
		{
			return hud.GUI_PauseButton;
		}
	}

	private GameObject GUI_Cloak
	{
		get
		{
			return hud.GUI_Cloak;
		}
	}

	private GameObject GUI_WeaponWristBlades_Active
	{
		get
		{
			return hud.GUI_WeaponWristBlades_Active;
		}
	}

	private GameObject GUI_WeaponCombiStick_Active
	{
		get
		{
			return hud.GUI_WeaponCombiStick_Active;
		}
	}

	private GameObject GUI_WeaponWhip_Active
	{
		get
		{
			return hud.GUI_WeaponWhip_Active;
		}
	}

	private GameObject GUI_WeaponPlasmaGun_Active
	{
		get
		{
			return hud.GUI_WeaponPlasmaGun_Active;
		}
	}

	private GameObject GUI_WeaponDisc_Active
	{
		get
		{
			return hud.GUI_WeaponDisc_Active;
		}
	}

	private GameObject GUI_WeaponNetGun_Active
	{
		get
		{
			return hud.GUI_WeaponNetGun_Active;
		}
	}

	private GameObject GUI_WeaponWristBlades_Inactive
	{
		get
		{
			return hud.GUI_WeaponWristBlades_Inactive;
		}
	}

	private GameObject GUI_WeaponCombiStick_Inactive
	{
		get
		{
			return hud.GUI_WeaponCombiStick_Inactive;
		}
	}

	private GameObject GUI_WeaponWhip_Inactive
	{
		get
		{
			return hud.GUI_WeaponWhip_Inactive;
		}
	}

	private GameObject GUI_WeaponPlasmaGun_Inactive
	{
		get
		{
			return hud.GUI_WeaponPlasmaGun_Inactive;
		}
	}

	private GameObject GUI_WeaponDisc_Inactive
	{
		get
		{
			return hud.GUI_WeaponDisc_Inactive;
		}
	}

	private GameObject GUI_WeaponNetGun_Inactive
	{
		get
		{
			return hud.GUI_WeaponNetGun_Inactive;
		}
	}

	private GameObject GUI_IconKills
	{
		get
		{
			return hud.GUI_IconKills;
		}
	}

	private GameObject GUI_IconTrophies
	{
		get
		{
			return hud.GUI_IconTrophies;
		}
	}

	private GameObject GUI_IconTimer
	{
		get
		{
			return hud.GUI_IconTimer;
		}
	}

	private GameObject GUI_IconDiscKills
	{
		get
		{
			return hud.GUI_IconDiscKills;
		}
	}

	private GameObject GUI_IconNetgunKills
	{
		get
		{
			return hud.GUI_IconNetgunKills;
		}
	}

	private GameObject GUI_IconWavesRemaining
	{
		get
		{
			return hud.GUI_IconWavesRemaining;
		}
	}

	private GameObject[] symbol1
	{
		get
		{
			return hud.symbol1;
		}
	}

	private GameObject[] symbol2
	{
		get
		{
			return hud.symbol2;
		}
	}

	private GameObject[] symbol3
	{
		get
		{
			return hud.symbol3;
		}
	}

	private GameObject[] symbol4
	{
		get
		{
			return hud.symbol4;
		}
	}

	private Transform GUI_PainParent
	{
		get
		{
			return hud.GUI_PainParent;
		}
	}

	private Transform GUI_PainParent_MinTransform
	{
		get
		{
			return hud.GUI_PainParent_MinTransform;
		}
	}

	private Transform GUI_PainParent_MaxTransform
	{
		get
		{
			return hud.GUI_PainParent_MaxTransform;
		}
	}

	private GameObject GUI_Pain1
	{
		get
		{
			return hud.GUI_Pain1;
		}
	}

	private GameObject GUI_Pain2
	{
		get
		{
			return hud.GUI_Pain2;
		}
	}

	private GameObject GUI_Pain3
	{
		get
		{
			return hud.GUI_Pain3;
		}
	}

	private GameObject GUI_Pain4
	{
		get
		{
			return hud.GUI_Pain4;
		}
	}

	private Transform TriangleProgressInitialLocation
	{
		get
		{
			return hud.TriangleProgressInitialLocation;
		}
	}

	private GameObject HealthBarGameObject
	{
		get
		{
			return hud.HealthBarGameObject;
		}
	}

	private GameObject HealthBarGameObjectClanLeader
	{
		get
		{
			return hud.HealthBarGameObjectClanLeader;
		}
	}

	private TextMesh BloodKillText
	{
		get
		{
			return hud.BloodKillText;
		}
	}

	private GameObject RainParticlesFront
	{
		get
		{
			return hud.RainParticlesFront;
		}
	}

	private GameObject RainParticlesBack
	{
		get
		{
			return hud.RainParticlesBack;
		}
	}

	private Transform GUI_ThermalFade
	{
		get
		{
			return hud.GUI_ThermalFade;
		}
	}

	private Transform[] tipTextures
	{
		get
		{
			return hud.tipTextures;
		}
	}

	public void HideKillsIconAndStatsMessage()
	{
		if ((bool)GUI_currentKillsIcon)
		{
			GUI_currentKillsIcon.gameObject.SetActiveRecursively(false);
		}
		if (survivalMissionController.MissionStatus.gameObject.active)
		{
			survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(false);
		}
	}

	private IEnumerator PainGUIShow(float percent)
	{
		if ((bool)vignetting)
		{
			vignetting.enabled = true;
			vignetting.chromaticAberrationIntensity = 0f;
		}
		GUI_Pain1.active = true;
		GUI_Pain2.active = true;
		yield return null;
		GUI_Pain3.active = true;
		GUI_Pain4.active = true;
		float timer2 = 0f;
		float amountToWaitStart = 0.1f * percent;
		float amountToWaitEnd = 0.3f * percent;
		for (; timer2 < amountToWaitStart; timer2 += Time.deltaTime)
		{
			if ((bool)vignetting)
			{
				vignetting.chromaticAberrationIntensity = Mathf.Lerp(0f, 13f, timer2 / amountToWaitStart);
			}
			yield return null;
		}
		for (timer2 = 0f; timer2 < amountToWaitEnd; timer2 += Time.deltaTime)
		{
			if ((bool)vignetting)
			{
				vignetting.chromaticAberrationIntensity = Mathf.Lerp(13f, 0f, timer2 / amountToWaitEnd);
			}
			yield return null;
		}
		GUI_Pain4.active = false;
		yield return null;
		yield return null;
		GUI_Pain3.active = false;
		yield return null;
		yield return null;
		GUI_Pain2.active = false;
		yield return null;
		yield return null;
		GUI_PainParent.gameObject.SetActiveRecursively(false);
		if ((bool)vignetting)
		{
			vignetting.enabled = false;
		}
	}

	private IEnumerator GrowShrink(Transform shrinkingTransform, Transform startTransform, Transform endTransform, float growTime, float shrinkTime)
	{
		float timer3 = growTime;
		shrinkingTransform.localScale = startTransform.localScale;
		timer3 = growTime;
		while (timer3 >= 0f)
		{
			shrinkingTransform.localScale = Vector3.Lerp(endTransform.localScale, startTransform.localScale, timer3 / growTime);
			timer3 -= Time.deltaTime;
			yield return null;
		}
		shrinkingTransform.position = endTransform.position;
		timer3 = shrinkTime;
		while (timer3 >= 0f)
		{
			shrinkingTransform.localScale = Vector3.Lerp(startTransform.localScale, endTransform.localScale, timer3 / shrinkTime);
			timer3 -= Time.deltaTime;
			yield return null;
		}
		shrinkingTransform.position = startTransform.position;
	}

	private IEnumerator TriangulateTarget(Transform newTarget)
	{
		if (superPredatorIsInvisible && !thermalVisionMode && newTarget == superPredatorTarget)
		{
			yield break;
		}
		TriangleProgress.position = TriangleProgressInitialLocation.position;
		TriangleProgress.gameObject.SetActiveRecursively(true);
		float timeToTriangulate = 1.6f;
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().clip = soundTriangulateInProgress;
			base.GetComponent<AudioSource>().loop = false;
			base.GetComponent<AudioSource>().Play();
		}
		float timer = timeToTriangulate;
		triangulationInProgress = true;
		while (timer > 0f)
		{
			TriangleProgress.position = Vector3.Lerp(newTarget.position + Vector3.up, TriangleProgressInitialLocation.position, timer / timeToTriangulate);
			if (!enemyTargets.Contains(newTarget))
			{
				triangulationInProgress = false;
				break;
			}
			timer -= Time.deltaTime;
			yield return null;
		}
		TriangleProgress.gameObject.SetActiveRecursively(false);
		if (triangulationInProgress)
		{
			triangulationInProgress = false;
			triangleTarget.gameObject.SetActiveRecursively(true);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundLockTarget);
			}
			triangleTarget.SetTarget(newTarget);
			targetLocked = true;
			newTargetFound = true;
			timerNewTargetRotate = timerNewTargetRotateFull;
		}
		else
		{
			base.GetComponent<AudioSource>().clip = soundTriangulateInProgress;
			base.GetComponent<AudioSource>().Stop();
		}
	}

	public void SetTriangleOnTarget(Transform newTarget)
	{
		if (AManager.instance.CinematicInProgress)
		{
			return;
		}
		if (thermalVisionMode)
		{
			triangleTarget.gameObject.SetActiveRecursively(true);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundTriangulateTarget);
				base.GetComponent<AudioSource>().PlayOneShot(soundLockTarget);
			}
			triangleTarget.SetTarget(newTarget);
			targetLocked = true;
			newTargetFound = true;
			timerNewTargetRotate = timerNewTargetRotateFull;
		}
		else if (!triangulationInProgress && !targetLocked && !AManager.instance.CinematicInProgress)
		{
			StopCoroutine("TriangulateTarget");
			StartCoroutine("TriangulateTarget", newTarget);
		}
	}

	public void StopTriangulatingTarget()
	{
		triangulationInProgress = false;
		targetLocked = false;
		StopCoroutine("TriangulateTarget");
		TriangleProgress.gameObject.SetActiveRecursively(false);
		if ((bool)triangleTarget)
		{
			triangleTarget.gameObject.SetActiveRecursively(false);
		}
	}

	public void AddEnemyTarget(Transform newTarget)
	{
		if (!enemyTargets.Contains(newTarget))
		{
			enemyTargetIndex = enemyTargets.Add(newTarget);
			if (enemyTargets.Count == 1 && currentWeapon == WeaponType.Ranged)
			{
				SetTriangleOnTarget(newTarget);
			}
			if (currentWeapon == WeaponType.Ranged)
			{
				CheckEnemyTargetsClosest();
			}
		}
	}

	public void CheckEnemyTargetsClosest()
	{
		int count = enemyTargets.Count;
		if (count <= 0)
		{
			return;
		}
		float num = 10000f;
		for (int i = 0; i < count; i++)
		{
			Transform transform = (Transform)enemyTargets[i];
			if ((transform.position - xForm.position).sqrMagnitude < num)
			{
				num = (transform.position - xForm.position).sqrMagnitude;
				enemyTargetIndex = i;
			}
		}
		if (enemyTargetIndex <= enemyTargets.Count && (Transform)enemyTargets[enemyTargetIndex] != triangleTarget.targetXForm)
		{
			triangleTarget.PlayTriangulateAnimation();
			SetTriangleOnTarget((Transform)enemyTargets[enemyTargetIndex]);
		}
	}

	private IEnumerator CheckEnemyTargets()
	{
		while (thermalVisionMode && currentWeapon == WeaponType.Ranged)
		{
			CheckEnemyTargetsClosest();
			yield return new WaitForSeconds(checkEnemyTargetsFrequency);
		}
	}

	public void RemoveEnemyTarget(Transform targetToRemove)
	{
		if (!enemyTargets.Contains(targetToRemove))
		{
			return;
		}
		enemyTargets.Remove(targetToRemove);
		if (triangleTarget.targetXForm == targetToRemove)
		{
			triangleTarget.targetXForm = null;
			targetLocked = false;
			newTargetFound = false;
			triangleTarget.gameObject.SetActiveRecursively(false);
		}
		if (currentWeapon == WeaponType.Ranged)
		{
			int count = enemyTargets.Count;
			if (count > 0)
			{
				enemyTargetIndex = 0;
				CheckEnemyTargetsClosest();
			}
		}
	}

	public int getEnemyTargetCount()
	{
		return enemyTargets.Count;
	}

	private void SwitchCloakMode()
	{
		if (cloakModeOn)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundCloakOff);
			}
			StopCoroutine("EnergyDecreaseCloak");
			if (energyConsumers > 0)
			{
				energyConsumers--;
			}
			if (currentWeapon == WeaponType.Melee)
			{
				StopCoroutine("EnergyIncrease");
				StartCoroutine("EnergyIncrease", 1f);
			}
			cloakModeOn = false;
			AManager.PredatorInvisible = false;
			playerMesh.GetComponent<Renderer>().material = materialNormal;
			if ((bool)faceMaskMesh)
			{
				faceMaskMesh.GetComponent<Renderer>().material = materialNormal;
			}
			WristBladeLMesh.GetComponent<Renderer>().material = materialNormal;
			WristBladeRMesh.GetComponent<Renderer>().material = materialNormal;
			PlasmaCannonMesh.GetComponent<Renderer>().material = materialNormal;
			SpearMesh.GetComponent<Renderer>().material = materialNormal;
			AO_Shadow.active = true;
		}
		else if (energy > energyCloakConsume + 2f)
		{
			StopCoroutine("EnergyIncrease");
			energy -= energyCloakConsume;
			energyConsumers++;
			StartCoroutine("EnergyDecreaseCloak", cloakEnergyDecreaseRate);
			UpdateEnergy();
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundCloakOn);
			}
			cloakModeOn = true;
			StartCoroutine(OffsetCloakMaterial());
			AManager.PredatorInvisible = true;
			if (thermalVisionMode)
			{
				playerMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				if ((bool)faceMaskMesh)
				{
					faceMaskMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				}
				WristBladeLMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				WristBladeRMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				PlasmaCannonMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				SpearMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				AO_Shadow.active = true;
			}
			else
			{
				playerMesh.GetComponent<Renderer>().material = materialCloak;
				if ((bool)faceMaskMesh)
				{
					faceMaskMesh.GetComponent<Renderer>().material = materialCloak;
				}
				WristBladeLMesh.GetComponent<Renderer>().material = materialCloak;
				WristBladeRMesh.GetComponent<Renderer>().material = materialCloak;
				PlasmaCannonMesh.GetComponent<Renderer>().material = materialCloak;
				SpearMesh.GetComponent<Renderer>().material = materialCloak;
				AO_Shadow.active = false;
			}
			if (rainyEnvironment || waterTouching)
			{
				StartCoroutine(CloakFail());
			}
		}
		else if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundEnergyDepleted);
		}
	}

	public void SwitchAutoAimMode(bool triangleOn)
	{
		if (triangleOn)
		{
			PlatformDependent.SetActiveIphoneGUI(AttackStick, true);
			PlatformDependent.SetActiveIphoneGUI(BlockStick, false);
			targetLocked = false;
			if ((bool)triangleTarget)
			{
				triangleTarget.gameObject.SetActiveRecursively(false);
			}
			if (enemyTargets.Count > 0)
			{
				SetTriangleOnTarget((Transform)enemyTargets[0]);
			}
			StopCoroutine("CheckEnemyTargets");
			StartCoroutine("CheckEnemyTargets");
			return;
		}
		triangulationInProgress = false;
		targetLocked = false;
		StopCoroutine("TriangulateTarget");
		TriangleProgress.gameObject.SetActiveRecursively(false);
		StopCoroutine("CheckEnemyTargets");
		if ((bool)triangleTarget)
		{
			triangleTarget.gameObject.SetActiveRecursively(false);
		}
		if (currentWeapon == WeaponType.Melee)
		{
			if (ability.blockUnlocked)
			{
				PlatformDependent.SetActiveIphoneGUI(BlockStick, true);
			}
			PlatformDependent.SetActiveIphoneGUI(AttackStick, true);
		}
	}

	private IEnumerator SlidePanel(Transform panel, Vector3 startPosition, Vector3 endPosition, float moveTime)
	{
		panel.gameObject.active = true;
		float timer = moveTime;
		panel.localPosition = startPosition;
		while (timer >= 0f)
		{
			panel.localPosition = Vector3.Lerp(endPosition, startPosition, timer / moveTime);
			timer -= Time.deltaTime;
			yield return null;
		}
		panel.gameObject.active = false;
	}

	public void SwitchThermalVisionMode()
	{
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundChangeVisionMode);
		}
		if (!thermalVisionMode)
		{
			if ((bool)glowEffect)
			{
				glowEffect.enabled = true;
			}
			if ((bool)noiseEffect)
			{
				noiseEffect.enabled = true;
			}
			thermalVisionUsageTime = Time.timeSinceLevelLoad;
			StartCoroutine(SlidePanel(GUI_ThermalFade, ThermalFadeRightPosition, ThermalFadeLeftPosition, 0.3f));
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().clip = soundVisionModeLoop;
				base.GetComponent<AudioSource>().loop = true;
				base.GetComponent<AudioSource>().Play();
				MainCamera.StopMusic();
			}
			energyConsumers++;
			EnvironmentThermalVision.SetActiveRecursively(true);
			EnvironmentNormalVision.SetActiveRecursively(false);
			materialNormal.SetTexture("_MainTex", TextureCharactersThermal);
			materialBerserkers.SetTexture("_MainTex", TextureBerserkersThermal);
			for (int i = 0; i < bloodMaterialsHumanCount; i++)
			{
				bloodMaterialsHuman[i].SetColor("_TintColor", bloodColorThermal);
			}
			for (int i = 0; i < bloodMaterialsPredatorCount; i++)
			{
				bloodMaterialsPredator[i].SetColor("_TintColor", bloodColorThermal);
			}
			thermalVisionMode = true;
			cameraMain.cullingMask = cameraCullingMaskThermal;
			materialNormal.color = ColorDaytime;
			materialBerserkers.color = ColorDaytime;
			if (!cloakModeOn)
			{
				playerMesh.GetComponent<Renderer>().material = materialNormal;
				if ((bool)faceMaskMesh)
				{
					faceMaskMesh.GetComponent<Renderer>().material = materialNormal;
				}
				WristBladeLMesh.GetComponent<Renderer>().material = materialNormal;
				WristBladeRMesh.GetComponent<Renderer>().material = materialNormal;
				PlasmaCannonMesh.GetComponent<Renderer>().material = materialNormal;
				SpearMesh.GetComponent<Renderer>().material = materialNormal;
			}
			else
			{
				playerMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				if ((bool)faceMaskMesh)
				{
					faceMaskMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				}
				WristBladeLMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				WristBladeRMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				PlasmaCannonMesh.GetComponent<Renderer>().material = materialThermalCloaked;
				SpearMesh.GetComponent<Renderer>().material = materialThermalCloaked;
			}
			if (currentMission == 31)
			{
				superBlackPredator.PlayerSwitchedThermalVision(true);
			}
			AO_Shadow.active = false;
			if (currentWeapon == WeaponType.Ranged && !AManager.instance.CinematicInProgress)
			{
				SwitchAutoAimMode(true);
			}
			return;
		}
		if ((bool)glowEffect)
		{
			glowEffect.enabled = false;
		}
		if ((bool)noiseEffect)
		{
			noiseEffect.enabled = false;
		}
		if (Time.timeSinceLevelLoad - thermalVisionUsageTime > 120f && !liteVersion)
		{
			//CrystalUnityBasic.Instance.PostAchievement("419966142", true, "Night Stalker", false);
		}
		StartCoroutine(SlidePanel(GUI_ThermalFade, ThermalFadeLeftPosition, ThermalFadeRightPosition, 0.3f));
		base.GetComponent<AudioSource>().clip = soundVisionModeLoop;
		base.GetComponent<AudioSource>().Stop();
		if (sfxOn && musicOn)
		{
			MainCamera.StartMusic();
		}
		energyConsumers--;
		if (currentWeapon == WeaponType.Melee)
		{
			StopCoroutine("EnergyIncrease");
			StartCoroutine("EnergyIncrease", 1f);
		}
		if (currentWeapon == WeaponType.Ranged)
		{
			LaserLineRenderer.gameObject.active = true;
			int count = enemyTargets.Count;
			if (count > 0 && !AManager.instance.CinematicInProgress)
			{
				enemyTargetIndex = 0;
				CheckEnemyTargetsClosest();
			}
		}
		EnvironmentThermalVision.SetActiveRecursively(false);
		EnvironmentNormalVision.SetActiveRecursively(true);
		materialNormal.SetTexture("_MainTex", TextureCharactersNormal);
		materialBerserkers.SetTexture("_MainTex", TextureBerserkersNormal);
		for (int i = 0; i < bloodMaterialsHumanCount; i++)
		{
			bloodMaterialsHuman[i].SetColor("_TintColor", bloodColorHuman);
		}
		for (int i = 0; i < bloodMaterialsPredatorCount; i++)
		{
			bloodMaterialsPredator[i].SetColor("_TintColor", bloodColorPredator);
		}
		thermalVisionMode = false;
		cameraMain.cullingMask = cameraCullingMaskNormal;
		materialNormal.color = currentCharactersColor;
		materialBerserkers.color = currentCharactersColor;
		if (!cloakModeOn)
		{
			playerMesh.GetComponent<Renderer>().material = materialNormal;
			if ((bool)faceMaskMesh)
			{
				faceMaskMesh.GetComponent<Renderer>().material = materialNormal;
			}
			WristBladeLMesh.GetComponent<Renderer>().material = materialNormal;
			WristBladeRMesh.GetComponent<Renderer>().material = materialNormal;
			PlasmaCannonMesh.GetComponent<Renderer>().material = materialNormal;
			SpearMesh.GetComponent<Renderer>().material = materialNormal;
			AO_Shadow.active = true;
		}
		else
		{
			playerMesh.GetComponent<Renderer>().material = materialCloak;
			if ((bool)faceMaskMesh)
			{
				faceMaskMesh.GetComponent<Renderer>().material = materialCloak;
			}
			WristBladeLMesh.GetComponent<Renderer>().material = materialCloak;
			WristBladeRMesh.GetComponent<Renderer>().material = materialCloak;
			PlasmaCannonMesh.GetComponent<Renderer>().material = materialCloak;
			SpearMesh.GetComponent<Renderer>().material = materialCloak;
		}
		if (currentMission == 31)
		{
			superBlackPredator.PlayerSwitchedThermalVision(false);
		}
	}

	private float StatsByWeaponLevel(float minValue, float maxValue, int[] stats, int currentLevel)
	{
		int num = stats.Length;
		if (currentLevel > num || currentLevel < 1)
		{
			return minValue;
		}
		int num2 = 100;
		int num3 = -10;
		for (int i = 1; i < num; i++)
		{
			if (stats[i] < num2)
			{
				num2 = stats[i];
			}
			if (stats[i] > num3)
			{
				num3 = stats[i];
			}
		}
		float num4 = (stats[currentLevel] - num2) / (num3 - num2);
		return minValue + (maxValue - minValue) * num4;
	}

	private void InitWeaponStats()
	{
		weaponLevelWristBlades = EncryptedPlayerPrefs.GetInt("PR_WristLevel_S" + currentSlot, debugWristBladesLevel);
		weaponLevelSpear = EncryptedPlayerPrefs.GetInt("PR_SpearLevel_S" + currentSlot, debugCombiStickLevel);
		weaponLevelWhip = EncryptedPlayerPrefs.GetInt("PR_WhipLevel_S" + currentSlot, debugWhipLevel);
		weaponLevelPlasmaGun = EncryptedPlayerPrefs.GetInt("PR_PlasmaGunLevel_S" + currentSlot, debugPlasmaGunLevel);
		weaponLevelDisc = EncryptedPlayerPrefs.GetInt("PR_DiskLevel_S" + currentSlot, debugDiscLevel);
		weaponLevelNetGun = EncryptedPlayerPrefs.GetInt("PR_NetGunLevel_S" + currentSlot, debugNetGunLevel);
		int num = EncryptedPlayerPrefs.GetInt(GameConstants.MASK_NUMBER_S + currentSlot, debugCurrentMaskType);
		if (liteVersion)
		{
			num = 1;
			weaponLevelWristBlades = 3;
			weaponLevelPlasmaGun = 0;
			weaponLevelSpear = 0;
			weaponLevelDisc = 3;
			weaponLevelNetGun = 0;
		}
		else if (currentMission == 34)
		{
			weaponLevelSpear = 3;
			weaponLevelDisc = 3;
			weaponLevelNetGun = 3;
			weaponLevelWhip = 3;
		}
		wristBladesDamage = StatsByWeaponLevel(wristBladesDamageMin, wristBladesDamageMax, GameConstants.WRIST_DAMAGE, weaponLevelWristBlades);
		spearDamageRadius = StatsByWeaponLevel(spearDamageRadiusMin, spearDamageRadiusMax, GameConstants.SPEAR_RANGE, weaponLevelSpear);
		spearDamage = StatsByWeaponLevel(spearDamageMin, spearDamageMax, GameConstants.SPEAR_DAMAGE, weaponLevelSpear);
		whipDamageRadius = StatsByWeaponLevel(whipDamageRadiusMin, whipDamageRadiusMax, GameConstants.WHIP_RANGE, weaponLevelWhip);
		whipDamage = StatsByWeaponLevel(whipDamageMin, whipDamageMax, GameConstants.WHIP_DAMAGE, weaponLevelWhip);
		plasmaDamage = StatsByWeaponLevel(plasmaDamageMin, plasmaDamageMax, GameConstants.PLASMA_GUN_DAMAGE, weaponLevelPlasmaGun);
		plasmaGunReloadTime = StatsByWeaponLevel(plasmaGunReloadTimeMin, plasmaGunReloadTimeMax, GameConstants.PLASMA_GUN_SPEED, weaponLevelPlasmaGun);
		plasmaGunEnergyDecreaseRate = StatsByWeaponLevel(plasmaGunEnergyDecreaseRateMin, plasmaGunEnergyDecreaseRateMax, GameConstants.PLASMA_GUN_ENERGY, weaponLevelPlasmaGun);
		if ((bool)particlePlasmaGunEmitter)
		{
			plasmaBulletParticle = (BulletParticle)particlePlasmaGunEmitter.GetComponent(typeof(BulletParticle));
		}
		plasmaBulletParticle.hitPoints = plasmaDamage;
		netGunReloadTime = StatsByWeaponLevel(netGunReloadTimeMin, netGunReloadTimeMax, GameConstants.NET_GUN_SPEED, weaponLevelNetGun);
		discDamage = StatsByWeaponLevel(discDamageMin, discDamageMax, GameConstants.DISK_DAMAGE, weaponLevelDisc);
		discThrowAgainTime = StatsByWeaponLevel(discThrowAgainTimeMin, discThrowAgainTimeMax, GameConstants.DISK_SPEED, weaponLevelDisc);
		discEnergyConsume = StatsByWeaponLevel(discEnergyConsumeMin, discEnergyConsumeMax, GameConstants.DISK_ENERGY, weaponLevelDisc);
		switch (weaponLevelWristBlades)
		{
		default:
			TrailRendererHandL.startWidth = TrailWidthStartLevel1;
			TrailRendererHandL.endWidth = 0.05f;
			TrailRendererHandR.startWidth = TrailWidthStartLevel1;
			TrailRendererHandR.endWidth = 0.05f;
			TrailRendererHandL.material.SetColor("_TintColor", TrailColorWeaponLevel1);
			TrailRendererHandR.material.SetColor("_TintColor", TrailColorWeaponLevel1);
			break;
		case 2:
			TrailRendererHandL.startWidth = TrailWidthStartLevel2;
			TrailRendererHandL.endWidth = TrailWidthStartLevel2 - 0.1f;
			TrailRendererHandR.startWidth = TrailWidthStartLevel2;
			TrailRendererHandR.endWidth = TrailWidthStartLevel2 - 0.1f;
			TrailRendererHandL.material.SetColor("_TintColor", TrailColorWeaponLevel2);
			TrailRendererHandR.material.SetColor("_TintColor", TrailColorWeaponLevel2);
			break;
		case 3:
			TrailRendererHandL.startWidth = TrailWidthStartLevel3;
			TrailRendererHandL.endWidth = TrailWidthStartLevel3 - 0.1f;
			TrailRendererHandR.startWidth = TrailWidthStartLevel3;
			TrailRendererHandR.endWidth = TrailWidthStartLevel3 - 0.1f;
			TrailRendererHandL.material.SetColor("_TintColor", TrailColorWeaponLevel3);
			TrailRendererHandR.material.SetColor("_TintColor", TrailColorWeaponLevel3);
			break;
		}
		switch (weaponLevelPlasmaGun)
		{
		default:
			plasmaBulletParticle.plasmaParticleLevel = 1;
			MaterialPlasmaGunParticle.SetColor("_TintColor", ColorPlasmaGunLevel1);
			glowCircle.material.SetColor("_TintColor", ColorPlasmaGunLevel1);
			break;
		case 2:
			plasmaBulletParticle.plasmaParticleLevel = 2;
			MaterialPlasmaGunParticle.SetColor("_TintColor", ColorPlasmaGunLevel2);
			glowCircle.material.SetColor("_TintColor", ColorPlasmaGunLevel2);
			break;
		case 3:
			plasmaBulletParticle.plasmaParticleLevel = 3;
			MaterialPlasmaGunParticle.SetColor("_TintColor", ColorPlasmaGunLevel3);
			glowCircle.material.SetColor("_TintColor", ColorPlasmaGunLevel3);
			break;
		}
		switch (weaponLevelWhip)
		{
		default:
			WhipTrailRenderer.startWidth = 0.8f;
			WhipTrailRenderer.endWidth = 0.05f;
			WhipTrailRenderer.material.SetColor("_TintColor", TrailColorWeaponLevel1);
			WhipLineRenderer.material.SetColor("_TintColor", TrailColorWeaponLevel1);
			break;
		case 2:
			WhipTrailRenderer.startWidth = 0.9f;
			WhipTrailRenderer.endWidth = 0.1f;
			WhipTrailRenderer.material.SetColor("_TintColor", TrailColorWeaponLevel2);
			WhipLineRenderer.material.SetColor("_TintColor", TrailColorWeaponLevel2);
			break;
		case 3:
			WhipTrailRenderer.startWidth = 1.05f;
			WhipTrailRenderer.endWidth = 0.1f;
			WhipTrailRenderer.material.SetColor("_TintColor", TrailColorWeaponLevel3);
			WhipLineRenderer.material.SetColor("_TintColor", TrailColorWeaponLevel3);
			break;
		}
		switch (weaponLevelNetGun)
		{
		default:
			netGunCapturedTime = netGunCapturedTimeNetGunL1;
			MaterialNetGun.SetColor("_TintColor", ColorNetGunLevel1);
			break;
		case 2:
			netGunCapturedTime = netGunCapturedTimeNetGunL2;
			MaterialNetGun.SetColor("_TintColor", ColorNetGunLevel2);
			break;
		case 3:
			netGunCapturedTime = netGunCapturedTimeNetGunL3;
			MaterialNetGun.SetColor("_TintColor", TrailColorWeaponLevel3);
			break;
		}
		switch (weaponLevelSpear)
		{
		default:
			TrailRendererSpearFront.startWidth = TrailWidthStartLevel1;
			TrailRendererSpearFront.endWidth = 0.05f;
			TrailRendererSpearBack.startWidth = TrailWidthStartLevel1;
			TrailRendererSpearBack.endWidth = 0.05f;
			TrailRendererSpearFront.material.SetColor("_TintColor", TrailColorWeaponLevel1);
			TrailRendererSpearBack.material.SetColor("_TintColor", TrailColorWeaponLevel1);
			break;
		case 2:
			TrailRendererSpearFront.startWidth = TrailWidthStartLevel2;
			TrailRendererSpearFront.endWidth = TrailWidthStartLevel2 - 0.1f;
			TrailRendererSpearBack.startWidth = TrailWidthStartLevel2;
			TrailRendererSpearBack.endWidth = TrailWidthStartLevel2 - 0.1f;
			TrailRendererSpearFront.material.SetColor("_TintColor", TrailColorWeaponLevel2);
			TrailRendererSpearBack.material.SetColor("_TintColor", TrailColorWeaponLevel2);
			break;
		case 3:
			TrailRendererSpearFront.startWidth = TrailWidthStartLevel3;
			TrailRendererSpearFront.endWidth = TrailWidthStartLevel3 - 0.1f;
			TrailRendererSpearBack.startWidth = TrailWidthStartLevel3;
			TrailRendererSpearBack.endWidth = TrailWidthStartLevel3 - 0.1f;
			TrailRendererSpearFront.material.SetColor("_TintColor", TrailColorWeaponLevel3);
			TrailRendererSpearBack.material.SetColor("_TintColor", TrailColorWeaponLevel3);
			break;
		}
		switch (weaponLevelDisc)
		{
		default:
			TrailDisc.GetComponent<Renderer>().material.SetColor("_TintColor", TrailColorWeaponLevel1);
			break;
		case 2:
			TrailDisc.GetComponent<Renderer>().material.SetColor("_TintColor", TrailColorWeaponLevel2);
			break;
		case 3:
			TrailDisc.GetComponent<Renderer>().material.SetColor("_TintColor", TrailColorWeaponLevel3);
			break;
		}
		switch (num)
		{
		default:
			faceMaskMesh = null;
			UnityEngine.Object.Destroy(faceMaskMesh1);
			UnityEngine.Object.Destroy(faceMaskMesh2);
			UnityEngine.Object.Destroy(faceMaskMesh3);
			UnityEngine.Object.Destroy(faceMaskMesh4);
			healthMultiplier = 1f;
			energyMultiplier = 1f;
			moveSpeedMultiplier = 1f;
			break;
		case 1:
			faceMaskMesh = faceMaskMesh1;
			faceMaskMesh1.active = true;
			UnityEngine.Object.Destroy(faceMaskMesh2);
			UnityEngine.Object.Destroy(faceMaskMesh3);
			UnityEngine.Object.Destroy(faceMaskMesh4);
			if (liteVersion)
			{
				healthMultiplier = 1.5f;
				energyMultiplier = 1.2f;
				moveSpeedMultiplier = 1f;
			}
			else
			{
				healthMultiplier = 1.1f;
				energyMultiplier = 1.2f;
				moveSpeedMultiplier = 1.15f;
			}
			break;
		case 2:
			faceMaskMesh = faceMaskMesh2;
			faceMaskMesh2.active = true;
			UnityEngine.Object.Destroy(faceMaskMesh1);
			UnityEngine.Object.Destroy(faceMaskMesh3);
			UnityEngine.Object.Destroy(faceMaskMesh4);
			healthMultiplier = 1.2f;
			energyMultiplier = 1.7f;
			moveSpeedMultiplier = 1.07f;
			break;
		case 3:
			faceMaskMesh = faceMaskMesh3;
			faceMaskMesh3.active = true;
			UnityEngine.Object.Destroy(faceMaskMesh1);
			UnityEngine.Object.Destroy(faceMaskMesh2);
			UnityEngine.Object.Destroy(faceMaskMesh4);
			healthMultiplier = 1.5f;
			energyMultiplier = 1.1f;
			moveSpeedMultiplier = 1f;
			break;
		case 4:
			faceMaskMesh = faceMaskMesh4;
			faceMaskMesh4.active = true;
			UnityEngine.Object.Destroy(faceMaskMesh1);
			UnityEngine.Object.Destroy(faceMaskMesh2);
			UnityEngine.Object.Destroy(faceMaskMesh3);
			healthMultiplier = 2f;
			energyMultiplier = 2f;
			moveSpeedMultiplier = 1.15f;
			break;
		}
		if (liteVersion)
		{
			healthMultiplier *= 1f + (float)EncryptedPlayerPrefs.GetInt("PR_TimesDiedInLite", 0) * 0.25f;
		}
		health *= healthMultiplier;
		maxHealth *= healthMultiplier;
		health = Mathf.Clamp(health, 0f, healthUpperLimit);
		maxHealth = Mathf.Clamp(maxHealth, 0f, healthUpperLimit);
		energyIncreaseDelay /= energyMultiplier;
		moveSpeed *= moveSpeedMultiplier;
		anim["run_fwd"].speed = animationRunSpeed * moveSpeedMultiplier;
		anim["run_bck"].speed = animationRunSpeed * moveSpeedMultiplier;
		anim["strafe_L"].speed = animationRunSpeed * moveSpeedMultiplier;
		anim["strafe_R"].speed = animationRunSpeed * moveSpeedMultiplier;
		UpdateHealth();
		StartCoroutine("RegenerateHealth");
		UpdateEnergy();
	}

	private void InitWeaponSlots()
	{
		weaponA = 1;
		weaponB = 5;
		currentWeapon = WeaponType.Melee;
		if (ability.blockUnlocked)
		{
			PlatformDependent.SetActiveIphoneGUI(BlockStick, true);
		}
		if (ability.lightAttack)
		{
			PlatformDependent.SetActiveIphoneGUI(AttackStick, true);
		}
		UpdateWeaponSlotTextures(true);
		netGunPoolCount = netGunPoolParent.childCount;
		netGunPool = new ArrayList(netGunPoolCount);
		for (int i = 0; i < netGunPoolCount; i++)
		{
			netGunPool.Add(netGunPoolParent.GetChild(i));
		}
		WeaponStopShooting();
	}

	public void StopControllingPlayer()
	{
		isControllable = false;
		anim.CrossFade("idle");
		WeaponStopShooting();
	}

	public void ResumeControllingPlayer()
	{
		isControllable = true;
	}

	private IEnumerator MuzzleFlash(float waitTime)
	{
		if (waitTime != 0f)
		{
			while (true)
			{
				glowCircle.enabled = true;
				yield return new WaitForSeconds(muzzleFlashTime);
				glowCircle.enabled = false;
				yield return new WaitForSeconds(waitTime);
			}
		}
		glowCircle.enabled = true;
	}

	private void WeaponStartShooting()
	{
		if (shooting)
		{
			return;
		}
		shooting = true;
		switch (currentWeaponType)
		{
		case 1:
		case 2:
		case 3:
		case 4:
			break;
		case 5:
		case 6:
			StartCoroutine("ReloadWeapon");
			if (!shootingReloadableWeapon)
			{
				shootingReloadableWeapon = true;
				reloadTime = plasmaGunReloadTime;
				StopCoroutine("EnergyIncrease");
				energyConsumers++;
				StartCoroutine("EnergyDecrease", plasmaGunEnergyDecreaseRate);
				StartCoroutine("ShootReloadableWeapon");
			}
			break;
		case 7:
		case 8:
			StartCoroutine("ReloadWeapon");
			if (!shootingReloadableWeapon)
			{
				shootingReloadableWeapon = true;
				reloadTime = discThrowAgainTime;
				if (!liteVersion)
				{
					StopCoroutine("EnergyIncrease");
				}
				StartCoroutine("ShootReloadableWeapon");
			}
			break;
		case 9:
			StartCoroutine("ReloadWeapon");
			if (!shootingReloadableWeapon)
			{
				shootingReloadableWeapon = true;
				reloadTime = netGunReloadTime;
				StopCoroutine("EnergyIncrease");
				energyConsumers++;
				StartCoroutine("EnergyDecrease", netGunEnergyDecreaseRate);
				StartCoroutine("ShootReloadableWeapon");
			}
			break;
		}
	}

	private IEnumerator PredatorClicks()
	{
		while (!dead)
		{
			yield return new WaitForSeconds(5f);
			if (moved || shooting)
			{
				continue;
			}
			switch (UnityEngine.Random.Range(1, 6))
			{
			case 2:
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundIdleClicks2);
				}
				break;
			case 3:
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundIdleClicks1);
				}
				break;
			}
		}
	}

	private void WeaponStopShooting()
	{
		if (!shooting)
		{
			return;
		}
		switch (currentWeaponType)
		{
		case 5:
		case 6:
			if (energy <= 0f && sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundEnergyDepleted);
			}
			StopCoroutine("ReloadWeapon");
			glowCircle.enabled = false;
			shootingReloadableWeapon = false;
			StopCoroutine("EnergyDecrease");
			if (energyConsumers > 0)
			{
				energyConsumers--;
			}
			StopCoroutine("EnergyIncrease");
			StartCoroutine("EnergyIncrease", 4f);
			StopCoroutine("ShootReloadableWeapon");
			if (!reloadedAtEnd)
			{
				StartCoroutine(FinishReloadingWeapon());
			}
			break;
		case 7:
		case 8:
			StopCoroutine("ReloadWeapon");
			glowCircle.enabled = false;
			shootingReloadableWeapon = false;
			if (!liteVersion)
			{
				StopCoroutine("EnergyIncrease");
				StartCoroutine("EnergyIncrease", 4f);
			}
			StopCoroutine("ShootReloadableWeapon");
			if (!reloadedAtEnd)
			{
				StartCoroutine(FinishReloadingWeapon());
			}
			break;
		case 9:
			if (energy <= 0f && sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundEnergyDepleted);
			}
			StopCoroutine("ReloadWeapon");
			glowCircle.enabled = false;
			shootingReloadableWeapon = false;
			StopCoroutine("EnergyDecrease");
			if (energyConsumers > 0)
			{
				energyConsumers--;
			}
			StopCoroutine("EnergyIncrease");
			StartCoroutine("EnergyIncrease", 4f);
			StopCoroutine("ShootReloadableWeapon");
			if (!reloadedAtEnd)
			{
				StartCoroutine(FinishReloadingWeapon());
			}
			break;
		}
		shooting = false;
	}

	private IEnumerator ReloadWeapon()
	{
		while (true)
		{
			if (reloadedAtEnd)
			{
				yield return null;
				continue;
			}
			yield return new WaitForSeconds(muzzleFlashTime);
			glowCircle.enabled = false;
			yield return new WaitForSeconds(reloadTime);
			reloadReady = true;
		}
	}

	private IEnumerator FinishReloadingWeapon()
	{
		reloadedAtEnd = true;
		yield return new WaitForSeconds(muzzleFlashTime);
		glowCircle.enabled = false;
		yield return new WaitForSeconds(reloadTime);
		reloadReady = true;
		reloadedAtEnd = false;
	}

	private IEnumerator OffsetCloakMaterial()
	{
		float offsetSpeed = 0.1f;
		while (cloakModeOn)
		{
			float rot = Mathf.Repeat(Time.time * offsetSpeed, 1f);
			materialCloak.SetTextureOffset("_NormalMap", new Vector2(0f - rot, rot));
			materialCloak.SetTextureOffset("_NormalMap2", new Vector2(rot, 0f - rot));
			yield return null;
		}
	}

	private IEnumerator DiscCut(GameObject disc)
	{
		Transform discXform = disc.transform;
		attackInfoPredator.Damage = discDamage;
		attackInfoPredator.AnimationNr = 6;
		peopleKilledByDisk = 0;
		enemiesHitByDisk.Clear();
		float removeTimer = 0f;
		while (disc.gameObject.active)
		{
			for (int t = 0; t < enemyTargets.Count; t++)
			{
				Transform enemyXform = (Transform)enemyTargets[t];
				if ((bool)enemyXform && (discXform.position - enemyXform.position).sqrMagnitude < discDamageRadiusSqr && !enemiesHitByDisk.Contains(enemyXform))
				{
					enemiesHitByDisk.Enqueue(enemyXform);
					attackInfoPredator.AttackerPosition = discXform.position;
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundDiscImpactEnemy);
					}
					enemyXform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			yield return null;
			removeTimer += Time.deltaTime;
			if (removeTimer > 0.5f)
			{
				removeTimer = 0f;
				if (enemiesHitByDisk.Count > 0)
				{
					enemiesHitByDisk.Dequeue();
				}
			}
		}
		survivalMissionController.MaxDiscCombo = peopleKilledByDisk;
		if (peopleKilledByDisk >= 2)
		{
			if (peopleKilledByDisk == 2)
			{
				ShowBloodSplatScreen(DeathType.DoubleSplice);
			}
			if (peopleKilledByDisk > 2)
			{
				ShowBloodSplatScreen(DeathType.SuperSplice);
			}
			if (peopleKilledByDisk > 3 && !liteVersion)
			{
				//CrystalUnityBasic.Instance.PostAchievement("419953296", true, "Tornado", false);
			}
		}
	}

	private IEnumerator ShootReloadableWeapon()
	{
		while (true)
		{
			if (reloadReady)
			{
				reloadReady = false;
				switch (currentWeaponType)
				{
				case 5:
				case 6:
					if (energy > 0f)
					{
						if (targetLocked)
						{
							anim.Play("plasma_shoot");
							if (sfxOn)
							{
								base.GetComponent<AudioSource>().PlayOneShot(soundPlasmaGunShoot);
							}
							particlePlasmaGunEmitter.Emit(1);
							glowCircle.enabled = true;
						}
						if (cloakModeOn)
						{
							SwitchCloakMode();
						}
					}
					else
					{
						WeaponStopShooting();
					}
					break;
				case 7:
				case 8:
					if (energy > discEnergyConsume)
					{
						if (!liteVersion)
						{
							energy -= discEnergyConsume;
							UpdateEnergy();
						}
						if (cloakModeOn)
						{
							SwitchCloakMode();
						}
						int targetsCount = enemyTargets.Count;
						discL1.gameObject.SetActiveRecursively(true);
						discL1.transform.position = xForm.TransformPoint(discOffset);
						discL1.transform.rotation = xForm.rotation;
						discL1.GetComponent<Rigidbody>().maxAngularVelocity = 0f;
						discL1.GetComponent<Rigidbody>().velocity = xForm.TransformDirection(new Vector3(0f, 0f, discSpeed));
						for (int t = 0; t < targetsCount; t++)
						{
							Physics.IgnoreCollision(discL1.GetComponent<Collider>(), ((Transform)enemyTargets[t]).GetComponent<Collider>());
						}
						Physics.IgnoreCollision(discL1.GetComponent<Collider>(), xForm.GetComponent<Collider>());
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundDiscThrow);
						}
						anim.Play("disc_throw");
						StartCoroutine("DiscCut", discL1.gameObject);
					}
					else
					{
						WeaponStopShooting();
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundEnergyDepleted);
						}
					}
					break;
				case 9:
					if (energy > 0f)
					{
						anim.Play("netgun_shoot");
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundNetGunShoot);
						}
						if (netGunPool.Count > currentNetGunIndex)
						{
							currentProjectileObject = (Transform)netGunPool[currentNetGunIndex];
						}
						else
						{
							Debug.LogError("not enough net guns set, index= " + currentNetGunIndex);
						}
						currentProjectileObject.gameObject.SetActiveRecursively(true);
						currentProjectileObject.GetComponent<Collider>().isTrigger = false;
						currentProjectileObject.GetComponent<Rigidbody>().maxAngularVelocity = 0f;
						currentProjectileObject.transform.position = xForm.TransformPoint(discOffset);
						currentProjectileObject.transform.rotation = xForm.rotation;
						currentProjectileObject.GetComponent<Rigidbody>().velocity = xForm.TransformDirection(new Vector3(0f, 0f, netGunSpeed));
						Physics.IgnoreCollision(currentProjectileObject.GetComponent<Collider>(), xForm.GetComponent<Collider>());
						if (currentNetGunIndex < netGunPoolCount - 1)
						{
							currentNetGunIndex++;
						}
						else
						{
							currentNetGunIndex = 0;
						}
					}
					else
					{
						WeaponStopShooting();
					}
					break;
				}
			}
			else
			{
				yield return null;
			}
			yield return null;
		}
	}

	private void PlayMoveAnimationsShootingTarget(Vector3 lVect, Vector3 rVect)
	{
		float num = Vector3.Dot(lVect, rVect);
		if (num > 0.5f)
		{
			anim.CrossFade("run_fwd");
		}
		else if (num < -0.5f)
		{
			anim.CrossFade("run_bck");
		}
		else if (Vector3.Cross(lVect, rVect).y > 0f)
		{
			anim.CrossFade("strafe_L");
		}
		else
		{
			anim.CrossFade("strafe_R");
		}
	}

	private void PlayMoveForwardAnimations()
	{
		if (currentWeaponType == 3)
		{
			anim.CrossFade("spear_run_fwd");
		}
		else if (currentWeaponType == 4)
		{
			anim.CrossFade("run_fwd");
			whipAnim.CrossFade("whip_retract");
		}
		else
		{
			anim.CrossFade("run_fwd");
		}
	}

	private void PlayMoveAnimationsShootingForward()
	{
		anim.CrossFade("run_fwd");
	}

	private void PlayIdleAnimations()
	{
		if (grabbingEnemy)
		{
			return;
		}
		TrailRendererHandL.gameObject.active = false;
		TrailRendererHandR.gameObject.active = false;
		TrailRendererSpearFront.gameObject.active = false;
		TrailRendererSpearBack.gameObject.active = false;
		WhipTrailRenderer.enabled = false;
		if (shooting)
		{
			anim.CrossFade("idle");
		}
		else if (!blocking)
		{
			switch (currentWeaponType)
			{
			case 1:
			case 2:
				anim.CrossFade("idle");
				break;
			case 3:
				anim.CrossFade("spear_idle");
				break;
			case 4:
				anim.CrossFade("idle");
				whipAnim.CrossFade("whip_idle");
				break;
			default:
				anim.CrossFade("idle");
				break;
			}
		}
	}

	public void ApplyDamageBlackPredatorPlasma(AttackInfo attackInfo)
	{
		if (xForm.position.y != 0f)
		{
			xForm.position = new Vector3(xForm.position.x, 0f, xForm.position.z);
		}
		if (dead)
		{
			return;
		}
		takeDamageDirection = attackInfo.AttackerPosition - xForm.position;
		bloodParent.rotation = Quaternion.LookRotation(takeDamageDirection);
		if (blocking)
		{
			particleSparks.transform.position = xForm.position + Vector3.up;
			particleSparks.transform.rotation = bloodParent.rotation;
			particleSparks.Emit();
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundGetHitBlocking);
			}
			attackInfo.Damage *= blockingDamageMultiplier;
		}
		else
		{
			if (bloodOn)
			{
				StopCoroutine("PainGUIShow");
				StartCoroutine("PainGUIShow", Mathf.Clamp01(attackInfo.Damage / damageHeavyLimit));
				StartCoroutine(GrowShrink(GUI_PainParent, GUI_PainParent_MinTransform, GUI_PainParent_MaxTransform, 0.01f, 0.3f));
				particleBloodSprayLight.Emit();
			}
			if (UnityEngine.Random.value < 0.5f)
			{
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundGetHit);
				}
			}
			else if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundGetHit2);
			}
		}
		if (attackInfo.Damage > damageHeavyLimit)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundGetHurtHeavy);
			}
			if (bloodOn)
			{
				particleBloodSprayHeavy.Emit();
			}
			if (!anim.IsPlaying("wrist_finish_cut_half") && !anim.IsPlaying("wrist_finish_head_off"))
			{
				if (grabbingEnemy)
				{
					StopGrabbingEnemy(true);
				}
				StopCoroutine("PerformAnimation");
				gettingHurt = false;
				performingAttackAnimation = false;
				conditionPerformedHeavyAttackHit = false;
				meleeLightAttacking = false;
				StartCoroutine("PerformAnimation", AttackAnimation.HurtHeavyMoveBack);
			}
		}
		health -= attackInfo.Damage;
		if (health <= 0f)
		{
			StopCoroutine("RegenerateHealth");
			health = 0f;
			UpdateHealth();
			Die();
			return;
		}
		if (cloakModeOn)
		{
			SwitchCloakMode();
		}
		StopCoroutine("RegenerateHealth");
		UpdateHealth();
		StartCoroutine("RegenerateHealth");
		energy = 0f;
		UpdateEnergy();
		StopCoroutine("EnergyIncrease");
		StartCoroutine("EnergyIncrease", 1f);
		if (thermalVisionMode)
		{
			SwitchThermalVisionMode();
		}
	}

	public void HurtMoveBack(AttackInfo attackInfo)
	{
		if (xForm.position.y != 0f)
		{
			xForm.position = new Vector3(xForm.position.x, 0f, xForm.position.z);
		}
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundGetHit);
		}
		takeDamageDirection = attackInfo.AttackerPosition - xForm.position;
		if (!anim.IsPlaying("wrist_finish_cut_half") && !anim.IsPlaying("wrist_finish_head_off"))
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(true);
			}
			StopCoroutine("PerformAnimation");
			gettingHurt = false;
			performingAttackAnimation = false;
			conditionPerformedHeavyAttackHit = false;
			meleeLightAttacking = false;
			StartCoroutine("PerformAnimation", AttackAnimation.HurtHeavyMoveBack);
		}
		if (cloakModeOn)
		{
			SwitchCloakMode();
		}
	}

	public void ApplyDamage(AttackInfo attackInfo)
	{
		if (xForm.position.y != 0f)
		{
			xForm.position = new Vector3(xForm.position.x, 0f, xForm.position.z);
		}
		if (dead)
		{
			return;
		}
		takeDamageDirection = attackInfo.AttackerPosition - xForm.position;
		bloodParent.rotation = Quaternion.LookRotation(takeDamageDirection);
		if (blocking)
		{
			particleSparks.transform.position = xForm.position + Vector3.up;
			particleSparks.transform.rotation = bloodParent.rotation;
			particleSparks.Emit();
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundGetHitBlocking);
			}
			attackInfo.Damage *= blockingDamageMultiplier;
		}
		else
		{
			if (bloodOn)
			{
				StopCoroutine("PainGUIShow");
				StartCoroutine("PainGUIShow", Mathf.Clamp01(attackInfo.Damage / damageHeavyLimit));
				StartCoroutine(GrowShrink(GUI_PainParent, GUI_PainParent_MinTransform, GUI_PainParent_MaxTransform, 0.01f + (70f - attackInfo.Damage) / 100f, 0.3f));
				particleBloodSprayLight.Emit();
			}
			if (UnityEngine.Random.value < 0.5f)
			{
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundGetHit);
				}
			}
			else if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundGetHit2);
			}
		}
		if (attackInfo.Damage > damageHeavyLimit)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundGetHurtHeavy);
			}
			if (bloodOn)
			{
				particleBloodSprayHeavy.Emit();
			}
			if (!anim.IsPlaying("wrist_finish_cut_half") && !anim.IsPlaying("wrist_finish_head_off"))
			{
				if (grabbingEnemy)
				{
					StopGrabbingEnemy(true);
				}
				StopCoroutine("PerformAnimation");
				gettingHurt = false;
				performingAttackAnimation = false;
				conditionPerformedHeavyAttackHit = false;
				meleeLightAttacking = false;
				StartCoroutine("PerformAnimation", AttackAnimation.HurtHeavyMoveBack);
			}
		}
		health -= attackInfo.Damage;
		if (health <= 0f)
		{
			GUI_PainParent.gameObject.SetActiveRecursively(false);
			StopCoroutine("RegenerateHealth");
			health = 0f;
			UpdateHealth();
			Die();
			return;
		}
		if (cloakModeOn)
		{
			SwitchCloakMode();
		}
		StopCoroutine("RegenerateHealth");
		UpdateHealth();
		StartCoroutine("RegenerateHealth");
	}

	private void UpdateHealth()
	{
		HealthBarMaterial.SetTextureOffset("_MainTex", new Vector2((1f - health / maxHealth) * 0.5f, 0f));
	}

	private IEnumerator RegenerateHealth()
	{
		float rate = 1f;
		float initialDelay = 2f;
		yield return new WaitForSeconds(initialDelay);
		while (health < maxHealth)
		{
			if (!paused)
			{
				health += rate;
				UpdateHealth();
			}
			yield return new WaitForSeconds(1f / 150f);
		}
	}

	private void Die()
	{
		StopAllCoroutines();
		StartCoroutine(DieCR());
		if (liteVersion)
		{
			EncryptedPlayerPrefs.SetInt("PR_TimesDiedInLite", EncryptedPlayerPrefs.GetInt("PR_TimesDiedInLite", 0) + 1);
		}
	}

	private IEnumerator DieCR()
	{
		MainCamera.ZoomIn();
		isControllable = false;
		dead = true;
		int slot = EncryptedPlayerPrefs.GetInt("PR_CurrentSlot");
		EncryptedPlayerPrefs.SetInt("PR_PredatorDeaths_S" + slot, EncryptedPlayerPrefs.GetInt("PR_PredatorDeaths_S" + slot, 0) + 1);
		if (currentMission == 32)
		{
			float timeSinceLevelLoad = Time.timeSinceLevelLoad;
			if (EncryptedPlayerPrefs.GetFloat("PR_LongestSurvivalTime_S" + slot, 0f) < timeSinceLevelLoad)
			{
				EncryptedPlayerPrefs.SetFloat("PR_LongestSurvivalTime_S" + slot, timeSinceLevelLoad);
			}
			string survivalMessage12 = string.Empty;
			string text = survivalMessage12;
			survivalMessage12 = text + Language.GetTxt("HONOR_POINTS_GAINED") + " " + survivalMissionController.HonorPointsOnCurrentMission;
			text = survivalMessage12;
			survivalMessage12 = text + "\n" + Language.GetTxt("HONOR_POINTS_TOTAL") + " " + (EncryptedPlayerPrefs.GetInt("PR_HonorPoints_S" + currentSlot, 0) + survivalMissionController.HonorPointsOnCurrentMission);
			text = survivalMessage12;
			survivalMessage12 = text + "\n" + Language.GetTxt("TROPHY_KILLS") + " " + survivalMissionController.EnemiesTrophyCount();
			text = survivalMessage12;
			survivalMessage12 = text + "\n" + Language.GetTxt("TOTAL_KILLS") + " " + survivalMissionController.MissionEnemiesKilled;
			text = survivalMessage12;
			survivalMessage12 = text + "\n" + Language.GetTxt("SURVIVAL_TIME") + " " + string.Format("{0:00}:{1:00}", (int)timeSinceLevelLoad / 60, (int)timeSinceLevelLoad % 60);
			setMissionSuccessToTrue(survivalMessage12);
		}
		else if (currentMission == 33)
		{
			string survivalMessage6 = string.Empty;
			string text = survivalMessage6;
			survivalMessage6 = text + Language.GetTxt("HONOR_POINTS_GAINED") + " " + survivalMissionController.HonorPointsOnCurrentMission;
			text = survivalMessage6;
			survivalMessage6 = text + "\n" + Language.GetTxt("HONOR_POINTS_TOTAL") + " " + (EncryptedPlayerPrefs.GetInt("PR_HonorPoints_S" + currentSlot, 0) + survivalMissionController.HonorPointsOnCurrentMission);
			text = survivalMessage6;
			survivalMessage6 = text + "\n" + Language.GetTxt("TROPHY_KILLS") + " " + survivalMissionController.EnemiesTrophyCount();
			text = survivalMessage6;
			survivalMessage6 = text + "\n" + Language.GetTxt("TOTAL_KILLS") + " " + survivalMissionController.MissionEnemiesKilled;
			text = survivalMessage6;
			survivalMessage6 = text + "\n" + Language.GetTxt("WAVES_CLEARED") + " " + (survivalMissionController.CurrentWaveIndex - 1);
			setMissionSuccessToTrue(survivalMessage6);
		}
		else
		{
			setMissionFailedToTrue();
		}
		if ((bool)currentGrabbedTarget)
		{
			grabbedEnemy = (BaseEnemy)currentGrabbedTarget.GetComponent(typeof(BaseEnemy));
			if ((bool)grabbedEnemy && grabbedEnemy.GrabbedVictim && !grabbedEnemy.Dead && !grabbedEnemy.Blocking)
			{
				currentGrabbedTarget.SendMessage("GrabbedStop", SendMessageOptions.DontRequireReceiver);
			}
		}
		LaserLineRenderer.gameObject.active = false;
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundDie);
		}
		if (UnityEngine.Random.value < 0.5f)
		{
			anim["die_fall_right"].time = 0f;
			anim.Play("die_fall_right");
		}
		else
		{
			anim["die_fall_forward"].time = 0f;
			anim.Play("die_fall_forward");
		}
		base.GetComponent<AudioSource>().Stop();
		if ((bool)soundDie && sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundDie);
		}
		WeaponStopShooting();
		yield return new WaitForSeconds(3f);
	}

	private IEnumerator EnergyDecrease(float energyDecreaseDelay)
	{
		while (energy > 0f)
		{
			if (currentWeaponType == 5 || currentWeaponType == 6 || currentWeaponType == 9)
			{
				if (targetLocked)
				{
					energy -= 1f;
				}
			}
			else
			{
				energy -= 1f;
			}
			UpdateEnergy();
			yield return new WaitForSeconds(energyDecreaseDelay);
		}
	}

	private IEnumerator EnergyDecreaseCloak(float energyDecreaseDelay)
	{
		if (currentMission != 8 && currentMission != 15 && currentMission != 29)
		{
			while (energy > 0f)
			{
				energy -= 1f;
				UpdateEnergy();
				yield return new WaitForSeconds(energyDecreaseDelay);
			}
		}
	}

	private IEnumerator EnergyIncrease(float delay)
	{
		if (energyConsumers > 0)
		{
			yield break;
		}
		yield return new WaitForSeconds(delay);
		if (energy < 39f)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundEnergyRecharging);
			}
			while (energy < 39f && energyConsumers <= 0)
			{
				LaserLineRenderer.enabled = true;
				energy += 1f;
				UpdateEnergy();
				yield return new WaitForSeconds(energyIncreaseDelay);
			}
		}
	}

	private void UpdateEnergy()
	{
		if (ability.cloakUnlocked)
		{
			if (energy >= 30f)
			{
				int num = (int)energy;
				num -= 30;
				symbol1[9].active = true;
				symbol2[9].active = true;
				symbol3[9].active = true;
				for (int i = 0; i < 9; i++)
				{
					symbol1[i].gameObject.active = false;
					symbol2[i].gameObject.active = false;
					symbol3[i].gameObject.active = false;
				}
				if (symbols[4] != num)
				{
					for (int i = 0; i < num; i++)
					{
						symbol4[i].gameObject.active = false;
					}
					symbol4[num].gameObject.active = true;
					for (int i = num + 1; i < 10; i++)
					{
						symbol4[i].gameObject.active = false;
					}
					symbols[4] = num;
				}
			}
			else if (energy >= 20f)
			{
				int num = (int)energy;
				num -= 20;
				symbol1[9].active = true;
				symbol2[9].active = true;
				symbol4[0].active = true;
				for (int i = 0; i < 9; i++)
				{
					symbol1[i].gameObject.active = false;
					symbol2[i].gameObject.active = false;
				}
				for (int i = 1; i < 10; i++)
				{
					symbol4[i].gameObject.active = false;
				}
				if (symbols[3] != num)
				{
					for (int i = 0; i < num; i++)
					{
						symbol3[i].gameObject.active = false;
					}
					symbol3[num].gameObject.active = true;
					for (int i = num + 1; i < 10; i++)
					{
						symbol3[i].gameObject.active = false;
					}
					symbols[3] = num;
				}
			}
			else if (energy >= 10f)
			{
				int num = (int)energy;
				num -= 10;
				symbol1[9].active = true;
				symbol3[0].active = true;
				symbol4[0].active = true;
				for (int i = 0; i < 9; i++)
				{
					symbol1[i].gameObject.active = false;
				}
				for (int i = 1; i < 10; i++)
				{
					symbol3[i].gameObject.active = false;
					symbol4[i].gameObject.active = false;
				}
				if (symbols[2] != num)
				{
					for (int i = 0; i < num; i++)
					{
						symbol2[i].gameObject.active = false;
					}
					symbol2[num].gameObject.active = true;
					for (int i = num + 1; i < 10; i++)
					{
						symbol2[i].gameObject.active = false;
					}
					symbols[2] = num;
				}
			}
			else
			{
				int num = (int)energy;
				symbol2[0].active = true;
				symbol3[0].active = true;
				symbol4[0].active = true;
				for (int i = 1; i < 10; i++)
				{
					symbol2[i].gameObject.active = false;
					symbol3[i].gameObject.active = false;
					symbol4[i].gameObject.active = false;
				}
				if (symbols[1] != num)
				{
					for (int i = 0; i < num; i++)
					{
						symbol1[i].gameObject.active = false;
					}
					symbol1[num].gameObject.active = true;
					for (int i = num + 1; i < 10; i++)
					{
						symbol1[i].gameObject.active = false;
					}
					symbols[1] = num;
				}
			}
		}
		else
		{
			for (int i = 0; i < 10; i++)
			{
				symbol1[i].gameObject.active = false;
				symbol2[i].gameObject.active = false;
				symbol3[i].gameObject.active = false;
				symbol4[i].gameObject.active = false;
			}
		}
		if (energy == 0f)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundEnergyDepleted);
			}
			if (cloakModeOn)
			{
				SwitchCloakMode();
			}
			LaserLineRenderer.enabled = false;
		}
	}

	private IEnumerator DashJump()
	{
		dashJumping = true;
		bool targetEnemyFound2 = false;
		float dashStartMoveDistance = speedDash * (1f / 6f);
		float dashAttackLandMoveDistance = speedDash * (7f / 30f);
		float bestTargetValue = 100f;
		Vector3 enemyDirection2 = xForm.forward;
		Vector3 testEnemyDirection2 = enemyDirection2;
		Collider[] colliders = ((currentWeaponType != 4) ? Physics.OverlapSphere(xForm.position, dashRadius) : Physics.OverlapSphere(xForm.position, dashRadius * 1.5f));
		Collider[] array = colliders;
		foreach (Collider hit in array)
		{
			if (hit.gameObject.layer != 11)
			{
				continue;
			}
			testEnemyDirection2 = hit.transform.position - xForm.position;
			float dotProduct = Vector3.Dot(lDiff3, testEnemyDirection2.normalized);
			if (dotProduct > 0.2f)
			{
				float currentTargetValue = 1f - dotProduct + testEnemyDirection2.sqrMagnitude / (dashRadius * dashRadius);
				if (currentTargetValue < bestTargetValue)
				{
					enemyDirection2 = testEnemyDirection2;
					bestTargetValue = currentTargetValue;
					currentTarget = hit.transform;
					targetEnemyFound2 = true;
				}
			}
		}
		switch (currentWeaponType)
		{
		case 1:
		case 2:
		case 4:
			anim["dash_start"].time = 0f;
			anim.CrossFade("dash_start", 0.1f);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundDashJump);
			}
			TrailRendererHandL.gameObject.active = false;
			WhipTrailRenderer.enabled = false;
			TrailRendererHandR.gameObject.active = false;
			TrailRendererSpearFront.gameObject.active = false;
			TrailRendererSpearBack.gameObject.active = false;
			yield return new WaitForSeconds(1f / 6f);
			break;
		case 3:
			anim["spear_dash_start"].time = 0f;
			anim.CrossFade("spear_dash_start", 0.1f);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundDashJump);
			}
			TrailRendererHandL.gameObject.active = false;
			WhipTrailRenderer.enabled = false;
			TrailRendererHandR.gameObject.active = false;
			TrailRendererSpearFront.gameObject.active = false;
			TrailRendererSpearBack.gameObject.active = false;
			yield return new WaitForSeconds(1f / 6f);
			break;
		}
		if (targetEnemyFound2)
		{
			float enemyDistance = enemyDirection2.magnitude;
			Vector3 dashDirection2 = enemyDirection2.normalized;
			targetRotation = Quaternion.LookRotation(dashDirection2);
			if (currentWeaponType == 4)
			{
				anim["whip_grapple_start"].time = 0f;
				anim.CrossFade("whip_grapple_start", 0.1f);
				whipComponent.CrossFadeAnim("whip_grapple_start", 0.1f);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundWhipAttack1);
				}
				TrailRendererHandL.gameObject.active = false;
				WhipTrailRenderer.enabled = true;
				TrailRendererHandR.gameObject.active = false;
				TrailRendererSpearFront.gameObject.active = false;
				TrailRendererSpearBack.gameObject.active = false;
				yield return new WaitForSeconds(0.3f);
				if (Physics.Linecast(xForm.position, currentTarget.position, laserLineCullingMask))
				{
					anim["whip_grapple_fail"].time = 0f;
					anim.CrossFade("whip_grapple_fail", 0.1f);
					whipAnim["whip_grapple_fail"].time = 0f;
					whipAnim.CrossFade("whip_grapple_fail", 0.1f);
				}
				else
				{
					anim["whip_grapple_dash"].time = 0f;
					anim.CrossFade("whip_grapple_dash", 0.1f);
					whipAnim["whip_grapple_dash"].time = 0f;
					whipAnim.CrossFade("whip_grapple_dash", 0.1f);
					if ((bool)currentTarget)
					{
						currentTarget.SendMessage("GrappleStart", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
						currentGrabbedTarget = currentTarget;
						whipComponent.GrappeledEnemy = currentTarget;
					}
					float timer8 = (enemyDistance - 0.8f) / speedDash;
					enemyDirection2 = currentTarget.position - xForm.position;
					while (timer8 > 0f)
					{
						playerController.Move(enemyDirection2.normalized * speedDash * Time.deltaTime);
						timer8 -= Time.deltaTime;
						xForm.rotation = Quaternion.Slerp(xForm.rotation, Quaternion.LookRotation(enemyDirection2), Time.deltaTime * rotateSpeed * 50f);
						enemyDirection2 = currentTarget.position - xForm.position;
						yield return null;
					}
					whipComponent.GrappeledEnemy = null;
					StartCoroutine("PerformAnimation", AttackAnimation.WristGrabStart);
				}
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				if (!grabbingEnemy)
				{
					currentGrabbedTarget = null;
				}
				if (enemyDistance < dashStartMoveDistance)
				{
					float timer8 = enemyDistance / dashStartMoveDistance / speedDash;
					while (timer8 > 0f)
					{
						timer8 -= Time.deltaTime;
						playerController.Move(dashDirection2 * speedDash * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					float timer8 = 1f / 6f;
					while (timer8 > 0f)
					{
						timer8 -= Time.deltaTime;
						playerController.Move(dashDirection2 * speedDash * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
					switch (currentWeaponType)
					{
					case 1:
					case 2:
						anim["dash_attack_land"].time = 0f;
						anim.CrossFade("dash_attack_land", 0.1f);
						break;
					case 3:
						anim["spear_dash_land"].time = 0f;
						anim.CrossFade("spear_dash_land", 0.1f);
						break;
					}
					timer8 = (enemyDistance - dashStartMoveDistance) / dashAttackLandMoveDistance / speedDash;
					while (timer8 > 0f)
					{
						timer8 -= Time.deltaTime;
						playerController.Move(dashDirection2 * speedDash * Time.deltaTime);
						yield return null;
					}
				}
				switch (currentWeaponType)
				{
				case 1:
				case 2:
					StartCoroutine("ComboAttackPerform", wristScissorCombo);
					break;
				case 3:
					StartCoroutine("ComboAttackPerform", spearScissorCombo);
					break;
				}
			}
		}
		else
		{
			Vector3 dashDirection2 = lDiff3;
			targetRotation = Quaternion.LookRotation(lDiff3);
			switch (currentWeaponType)
			{
			case 1:
			case 2:
			case 4:
			{
				float timer8 = 1f / 6f;
				while (timer8 > 0f)
				{
					timer8 -= Time.deltaTime;
					playerController.Move(dashDirection2 * speedDash * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				anim["dash_land"].time = 0f;
				anim.CrossFade("dash_land", 0.1f);
				timer8 = 1f / 3f;
				while (timer8 > 0f)
				{
					timer8 -= Time.deltaTime;
					playerController.Move(dashDirection2 * speedDash * Time.deltaTime);
					yield return null;
				}
				break;
			}
			case 3:
			{
				float timer8 = 1f / 6f;
				while (timer8 > 0f)
				{
					timer8 -= Time.deltaTime;
					playerController.Move(dashDirection2 * speedDash * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				anim["spear_dash_land"].time = 0f;
				anim.CrossFade("spear_dash_land", 0.1f);
				timer8 = 1f / 3f;
				while (timer8 > 0f)
				{
					timer8 -= Time.deltaTime;
					playerController.Move(dashDirection2 * speedDash * Time.deltaTime);
					yield return null;
				}
				break;
			}
			}
		}
		targetEnemyFound2 = false;
		dashJumping = false;
	}

	private Vector3 FindEnemyTargetInFront(float checkRadius, bool grabMode)
	{
		float num = 100f;
		Vector3 vector = Vector3.up;
		Vector3 vector2 = vector;
		Collider[] array = Physics.OverlapSphere(xForm.position, checkRadius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.gameObject.layer != 11)
			{
				continue;
			}
			vector2 = collider.transform.position - xForm.position;
			float num2 = ((!(lDiff3 == Vector3.zero)) ? Vector3.Dot(lDiff3, vector2.normalized) : Vector3.Dot(xForm.forward, vector2.normalized));
			if (!(num2 > 0.2f))
			{
				continue;
			}
			float num3 = 1f - num2 + vector2.sqrMagnitude / (checkRadius * checkRadius);
			if (!(num3 < num))
			{
				continue;
			}
			vector = vector2;
			num = num3;
			if (grabbingEnemy)
			{
				if (collider.transform != currentGrabbedTarget)
				{
					StopGrabbingEnemy(true);
				}
				currentGrabbedTarget = collider.transform;
			}
			else if (grabMode)
			{
				currentGrabbedTarget = collider.transform;
			}
			else
			{
				currentTarget = collider.transform;
			}
		}
		return vector.normalized;
	}

	private IEnumerator ComboAttackPerform(AnimCombo animCombo)
	{
		bool comboContinued3 = false;
		meleeLightAttacking = true;
		if (animCombo.nextAnimationCombo != null)
		{
			float timer2;
			if (animCombo.nextAnimationCombo.animationTypeLight)
			{
				StartCoroutine(ComboCheckInput(animCombo.animationComboLength - timerComboLight / 2f, timerComboLight / 2f, true));
				if (!performingAttackAnimation)
				{
					yield return StartCoroutine("PerformAnimation", animCombo.animationToPerform);
				}
				if (pressedComboSecondary && animCombo.secondaryAnimation != 0)
				{
					yield return StartCoroutine("PerformAnimation", animCombo.secondaryAnimation);
					meleeLightAttacking = false;
					yield break;
				}
				if (pressedComboLight)
				{
					StartCoroutine(ComboAttackPerform(animCombo.nextAnimationCombo));
					yield break;
				}
				timer2 = timerComboLight / 2f;
				comboContinued3 = false;
				while (timer2 > 0f)
				{
					if (comboAttackSecondary && animCombo.secondaryAnimation != 0)
					{
						yield return StartCoroutine("PerformAnimation", animCombo.secondaryAnimation);
						meleeLightAttacking = false;
						comboContinued3 = true;
						break;
					}
					if (comboAttackLight)
					{
						StartCoroutine(ComboAttackPerform(animCombo.nextAnimationCombo));
						comboContinued3 = true;
						break;
					}
					timer2 -= Time.deltaTime;
					yield return null;
				}
				if (!comboContinued3)
				{
					yield return new WaitForSeconds((animCombo.animationEndLength - timerComboLight / 2f) / 2f);
					meleeLightAttacking = false;
				}
				yield break;
			}
			StartCoroutine(ComboCheckInput(animCombo.animationComboLength - (timerComboHeavy + timerHeavySpinInputTolerance) / 2f, (timerComboHeavy + timerHeavySpinInputTolerance) / 2f, false));
			if (!performingAttackAnimation)
			{
				yield return StartCoroutine("PerformAnimation", animCombo.animationToPerform);
			}
			if (pressedComboSecondary && animCombo.secondaryAnimation != 0)
			{
				yield return StartCoroutine("PerformAnimation", animCombo.secondaryAnimation);
				meleeLightAttacking = false;
				yield break;
			}
			if (pressedComboHeavy)
			{
				StartCoroutine(ComboAttackPerform(animCombo.nextAnimationCombo));
				yield break;
			}
			timer2 = (timerComboHeavy + timerHeavySpinInputTolerance) / 2f;
			comboContinued3 = false;
			while (timer2 > 0f)
			{
				if (comboAttackSecondary && animCombo.secondaryAnimation != 0)
				{
					yield return StartCoroutine("PerformAnimation", animCombo.secondaryAnimation);
					meleeLightAttacking = false;
					comboContinued3 = true;
					break;
				}
				if (PlatformDependent.ComboAttackHeavy(comboAttackHeavy))
				{
					StartCoroutine(ComboAttackPerform(animCombo.nextAnimationCombo));
					comboContinued3 = true;
					break;
				}
				timer2 -= Time.deltaTime;
				yield return null;
			}
			if (!comboContinued3)
			{
				yield return new WaitForSeconds((animCombo.animationEndLength - (timerComboHeavy + timerHeavySpinInputTolerance) / 2f) / 2f);
				meleeLightAttacking = false;
			}
		}
		else
		{
			if (!performingAttackAnimation)
			{
				yield return StartCoroutine("PerformAnimation", animCombo.animationToPerform);
			}
			yield return new WaitForSeconds(animCombo.animationEndLength / 2f);
			meleeLightAttacking = false;
		}
	}

	private void FindPreyType()
	{
		if ((bool)HeadSoldierGeneric)
		{
			headOffPrey = HeadSoldierGeneric;
		}
	}

	private IEnumerator HideHeadOffPrey()
	{
		yield return new WaitForSeconds(1.2f);
		if ((bool)headOffPrey)
		{
			headOffPrey.active = false;
			headOffPrey = null;
		}
	}

	public void ActivateWhipWeapon()
	{
		whipComponent.ShowWhip();
		whipComponent.SetAnimationWrapMode();
		whipAnim["whip_idle"].wrapMode = WrapMode.Loop;
		whipAnim["whip_attack_heavy_R_charge"].speed = 0.5f;
		whipAnim["whip_retract"].wrapMode = WrapMode.Loop;
	}

	public void EnemyBrokeLooseFromGrabbing()
	{
		StopCoroutine("PerformAnimation");
		gettingHurt = false;
		if (currentWeaponType != 1 && currentWeaponType != 2)
		{
			WristBladeLMesh.gameObject.active = false;
			WristBladeRMesh.gameObject.active = false;
		}
		if (currentWeaponType == 3)
		{
			SpearMesh.gameObject.active = true;
		}
		else if (currentWeaponType == 4)
		{
			ActivateWhipWeapon();
		}
		grabbingEnemy = false;
		chargingHeavyAttack = false;
		performingAttackAnimation = false;
		conditionPerformedHeavyAttackHit = false;
		meleeLightAttacking = false;
		TrailRendererHandL.gameObject.active = false;
		TrailRendererHandR.gameObject.active = false;
		TrailRendererSpearFront.gameObject.active = false;
		TrailRendererSpearBack.gameObject.active = false;
		WhipTrailRenderer.enabled = false;
		MainCamera.ZoomOut();
	}

	private void StopGrabbingEnemy(bool stopLightAttacks)
	{
		if ((bool)currentGrabbedTarget)
		{
			grabbedEnemy = (BaseEnemy)currentGrabbedTarget.GetComponent(typeof(BaseEnemy));
			if ((bool)grabbedEnemy && grabbedEnemy.GrabbedVictim && !grabbedEnemy.Dead)
			{
				grabbedEnemy.GrabbedStopTest();
			}
			currentGrabbedTarget = null;
		}
		else if (grabbingEnemy)
		{
			Debug.LogError("currentTarget reference lost");
		}
		if (stopLightAttacks)
		{
			StopCoroutine("PerformAnimation");
			gettingHurt = false;
			performingAttackAnimation = false;
			conditionPerformedHeavyAttackHit = false;
			meleeLightAttacking = false;
		}
		grabbingEnemy = false;
		chargingHeavyAttack = false;
		if (currentWeaponType != 1 && currentWeaponType != 2)
		{
			WristBladeLMesh.gameObject.active = false;
			WristBladeRMesh.gameObject.active = false;
		}
		if (currentWeaponType == 3)
		{
			SpearMesh.gameObject.active = true;
		}
		else if (currentWeaponType == 4)
		{
			ActivateWhipWeapon();
		}
		TrailRendererHandL.gameObject.active = false;
		TrailRendererHandR.gameObject.active = false;
		TrailRendererSpearFront.gameObject.active = false;
		TrailRendererSpearBack.gameObject.active = false;
		WhipTrailRenderer.enabled = false;
		MainCamera.ZoomOut();
	}

	private IEnumerator PerformAnimation(AttackAnimation attackAnimationType)
	{
		bool targetEnemyFound = false;
		if (dead)
		{
			yield break;
		}
		if (!grabbingEnemy)
		{
			MainCamera.ZoomOut();
		}
		performingAttackAnimation = true;
		currentPerformingAnimation = attackAnimationType;
		switch (attackAnimationType)
		{
		case AttackAnimation.WristLightL:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
			}
			anim["attack_light_wrist_L"].time = 0f;
			anim.CrossFade("attack_light_wrist_L", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererHandL.gameObject.active = true;
			if (targetEnemyFound)
			{
				float timer11 = 0.1f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = wristBladesDamage;
				attackInfoPredator.AnimationNr = 2;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				timer11 = 2f / 15f;
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					yield return null;
				}
			}
			else
			{
				float timer11 = 0.23333335f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristLightR:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
			}
			anim["attack_light_wrist_R"].time = 0f;
			anim.CrossFade("attack_light_wrist_R", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererHandR.gameObject.active = true;
			if (targetEnemyFound)
			{
				float timer11 = 0.1f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = wristBladesDamage;
				attackInfoPredator.AnimationNr = 3;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				timer11 = 2f / 15f;
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					yield return null;
				}
			}
			else
			{
				float timer11 = 0.23333335f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererHandR.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristHeavyR:
		{
			if (!ability.heavyAttack)
			{
				break;
			}
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			meleeLightAttacking = true;
			anim["attack_heavy_R"].time = 0f;
			anim.CrossFade("attack_heavy_R", 0.1f);
			TrailRendererHandR.gameObject.active = true;
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
			}
			if (targetEnemyFound)
			{
				float timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = wristBladesDamage * 2f;
				attackInfoPredator.AnimationNr = 1;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(1.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeHitImpale);
					}
					conditionPerformedHeavyAttackHit = true;
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				float timer11 = 0.2f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererHandR.gameObject.active = false;
			yield return new WaitForSeconds(0.6f);
			meleeLightAttacking = false;
			break;
		}
		case AttackAnimation.WristSpinL:
		{
			if (!ability.spinAttack)
			{
				break;
			}
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererHandR.gameObject.active = true;
			TrailRendererHandL.gameObject.active = true;
			anim["attack_heavy_L"].time = 0f;
			anim.CrossFade("attack_heavy_L", 0.1f);
			yield return new WaitForSeconds(0.1f);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
			}
			if (targetEnemyFound)
			{
				float timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = wristBladesDamage;
				attackInfoPredator.AnimationNr = 5;
				if ((bool)currentTarget)
				{
					for (int t3 = 0; t3 < enemyTargets.Count; t3++)
					{
						enemyTempTransform = (Transform)enemyTargets[t3];
						if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && (xForm.position - enemyTempTransform.position).sqrMagnitude < wristBladesDamageRadius * wristBladesDamageRadius)
						{
							enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
						}
					}
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeHitDry);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
				}
				timer11 = 2f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = wristBladesDamage * 4f;
				attackInfoPredator.AnimationNr = 5;
				if ((bool)currentTarget)
				{
					for (int t2 = 0; t2 < enemyTargets.Count; t2++)
					{
						enemyTempTransform = (Transform)enemyTargets[t2];
						if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && (xForm.position - enemyTempTransform.position).sqrMagnitude < wristBladesDamageRadius * wristBladesDamageRadius)
						{
							enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
						}
					}
					MainCamera.ShakeCamera(0.5f);
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				float timer11 = 1f / 3f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						yield return null;
					}
				}
				attackInfoPredator.Damage = wristBladesDamage * 4f;
				attackInfoPredator.AnimationNr = 5;
				for (int t = 0; t < enemyTargets.Count; t++)
				{
					enemyTempTransform = (Transform)enemyTargets[t];
					if ((bool)enemyTempTransform && (xForm.position - enemyTempTransform.position).sqrMagnitude < wristBladesDamageRadius * wristBladesDamageRadius)
					{
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			TrailRendererHandR.gameObject.active = false;
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristScissor:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			anim["dash_attack_scissor"].time = 0f;
			anim.CrossFade("dash_attack_scissor", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererHandR.gameObject.active = true;
			TrailRendererHandL.gameObject.active = true;
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
			}
			if (targetEnemyFound)
			{
				attackInfoPredator.Damage = wristBladesDamage;
				attackInfoPredator.AnimationNr = 1;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				float timer11 = 1f / 6f;
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					yield return null;
				}
			}
			TrailRendererHandR.gameObject.active = false;
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristGrabStart:
		{
			if (!ability.impale)
			{
				break;
			}
			anim[AnimGrabStart].time = 0f;
			anim.CrossFade(AnimGrabStart, 0.1f);
			grabbingEnemy = false;
			if (currentWeaponType == 3 || currentWeaponType == 4)
			{
				WristBladeLMesh.gameObject.active = true;
				WristBladeRMesh.gameObject.active = true;
				SpearMesh.gameObject.active = false;
				whipComponent.HideWhip();
			}
			WhipTrailRenderer.enabled = false;
			TrailRendererHandL.gameObject.active = true;
			Vector3 attackDirection12;
			if (currentWeaponType == 4)
			{
				if ((bool)currentGrabbedTarget)
				{
					attackDirection12 = (currentGrabbedTarget.position - xForm.position).normalized;
					targetEnemyFound = true;
				}
				else
				{
					attackDirection12 = Vector3.up;
					targetEnemyFound = false;
				}
			}
			else
			{
				attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, true);
				if (attackDirection12 != Vector3.up)
				{
					targetEnemyFound = true;
				}
			}
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
			}
			if (targetEnemyFound)
			{
				float timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
					yield return null;
				}
				xForm.rotation = targetRotation;
				if ((bool)currentGrabbedTarget)
				{
					grabbedEnemy = (BaseEnemy)currentGrabbedTarget.GetComponent(typeof(BaseEnemy));
					if ((bool)grabbedEnemy)
					{
						if (!grabbedEnemy.Dead)
						{
							if (!grabbedEnemy.GrabbedVictim)
							{
								if (!grabbedEnemy.Blocking)
								{
									MainCamera.ShakeCamera(0.5f);
									grabbingEnemy = true;
									survivalMissionController.OpponentsImpaled++;
									attackInfoPredator.Damage = wristBladesDamage;
									attackInfoPredator.AnimationNr = 1;
									if (sfxOn)
									{
										base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeHitDry);
									}
									MainCamera.ZoomIn();
									currentGrabbedTarget.SendMessage("GrabStart", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
									StopCoroutine("GrabbingEnemy");
									StartCoroutine("GrabbingEnemy");
								}
								else
								{
									particleSparks.transform.position = xForm.position + Vector3.up;
									particleSparks.transform.rotation = bloodParent.rotation;
									particleSparks.Emit();
									if (sfxOn)
									{
										base.GetComponent<AudioSource>().PlayOneShot(soundGetHitBlocking);
									}
									if (currentWeaponType != 1 && currentWeaponType != 2)
									{
										WristBladeLMesh.gameObject.active = false;
										WristBladeRMesh.gameObject.active = false;
									}
									if (currentWeaponType == 3)
									{
										SpearMesh.gameObject.active = true;
									}
									else if (currentWeaponType == 4)
									{
										ActivateWhipWeapon();
									}
								}
							}
							else
							{
								Debug.LogWarning("victim: " + grabbedEnemy.gameObject.name + " was already being grabbed");
							}
						}
						else
						{
							if (currentWeaponType != 1 && currentWeaponType != 2)
							{
								WristBladeLMesh.gameObject.active = false;
								WristBladeRMesh.gameObject.active = false;
							}
							if (currentWeaponType == 3)
							{
								SpearMesh.gameObject.active = true;
							}
							else if (currentWeaponType == 4)
							{
								ActivateWhipWeapon();
							}
						}
					}
				}
				else
				{
					if (currentWeaponType != 1 && currentWeaponType != 2)
					{
						WristBladeLMesh.gameObject.active = false;
						WristBladeRMesh.gameObject.active = false;
					}
					if (currentWeaponType == 3)
					{
						SpearMesh.gameObject.active = true;
					}
					else if (currentWeaponType == 4)
					{
						ActivateWhipWeapon();
					}
				}
			}
			else
			{
				float timer11 = 0.2f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * speedWristAttack * Time.deltaTime);
						yield return null;
					}
				}
				yield return new WaitForSeconds(1f / 3f);
				if (currentWeaponType != 1 && currentWeaponType != 2)
				{
					WristBladeLMesh.gameObject.active = false;
					WristBladeRMesh.gameObject.active = false;
				}
				if (currentWeaponType == 3)
				{
					SpearMesh.gameObject.active = true;
				}
				else if (currentWeaponType == 4)
				{
					ActivateWhipWeapon();
				}
			}
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristGrabChopHi:
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
			}
			anim["grab_chop_Hi"].time = 0f;
			anim.CrossFade("grab_chop_Hi", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, true);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererHandL.gameObject.active = true;
			if (targetEnemyFound)
			{
				if (currentWeaponType == 3 || currentWeaponType == 4)
				{
					WristBladeLMesh.gameObject.active = true;
					WristBladeRMesh.gameObject.active = true;
					SpearMesh.gameObject.active = false;
					whipComponent.HideWhip();
				}
				if ((bool)currentGrabbedTarget)
				{
					grabbedEnemy = (BaseEnemy)currentGrabbedTarget.GetComponent(typeof(BaseEnemy));
					if ((bool)grabbedEnemy)
					{
						if (!grabbedEnemy.GrabbedVictim)
						{
							StopGrabbingEnemy(true);
							break;
						}
						if (!grabbedEnemy.Dead && !grabbedEnemy.Blocking)
						{
							attackInfoPredator.Damage = wristBladesDamage;
							attackInfoPredator.AnimationNr = 1;
							currentGrabbedTarget.SendMessage("ApplyDamageGrabbed", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
							StopCoroutine("GrabbingEnemy");
							StartCoroutine("GrabbingEnemy");
						}
						float timer11 = 7f / 30f;
						targetRotation = Quaternion.LookRotation(attackDirection12);
						while (timer11 > 0f)
						{
							timer11 -= Time.deltaTime;
							xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
							yield return null;
						}
						xForm.rotation = targetRotation;
						if ((bool)currentGrabbedTarget)
						{
							MainCamera.ShakeCamera(0.5f);
						}
					}
				}
			}
			else
			{
				StopGrabbingEnemy(true);
			}
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristGrabChopLow:
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
			}
			anim["grab_chop_Low"].time = 0f;
			anim.CrossFade("grab_chop_Low", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, true);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererHandL.gameObject.active = true;
			if (targetEnemyFound)
			{
				if (currentWeaponType == 3 || currentWeaponType == 4)
				{
					WristBladeLMesh.gameObject.active = true;
					WristBladeRMesh.gameObject.active = true;
					SpearMesh.gameObject.active = false;
					whipComponent.HideWhip();
				}
				if ((bool)currentGrabbedTarget)
				{
					grabbedEnemy = (BaseEnemy)currentGrabbedTarget.GetComponent(typeof(BaseEnemy));
					if ((bool)grabbedEnemy)
					{
						if (!grabbedEnemy.GrabbedVictim)
						{
							StopGrabbingEnemy(true);
							break;
						}
						if (!grabbedEnemy.Dead && !grabbedEnemy.Blocking)
						{
							attackInfoPredator.Damage = wristBladesDamage;
							attackInfoPredator.AnimationNr = 2;
							currentGrabbedTarget.SendMessage("ApplyDamageGrabbed", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
							StopCoroutine("GrabbingEnemy");
							StartCoroutine("GrabbingEnemy");
						}
						float timer11 = 7f / 30f;
						targetRotation = Quaternion.LookRotation(attackDirection12);
						while (timer11 > 0f)
						{
							timer11 -= Time.deltaTime;
							xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
							yield return null;
						}
						xForm.rotation = targetRotation;
						if ((bool)currentGrabbedTarget)
						{
							MainCamera.ShakeCamera(0.5f);
						}
					}
				}
			}
			else
			{
				StopGrabbingEnemy(true);
			}
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristGrabHeadOff:
		{
			if (!ability.trophyKill)
			{
				break;
			}
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, true);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			if (targetEnemyFound && !gettingHurt)
			{
				if (currentWeaponType == 3 || currentWeaponType == 4)
				{
					WristBladeLMesh.gameObject.active = true;
					WristBladeRMesh.gameObject.active = true;
					SpearMesh.gameObject.active = false;
					whipComponent.HideWhip();
				}
				if ((bool)currentGrabbedTarget)
				{
					grabbedEnemy = (BaseEnemy)currentGrabbedTarget.GetComponent(typeof(BaseEnemy));
					if ((bool)grabbedEnemy)
					{
						if (!grabbedEnemy.GrabbedVictim || grabbedEnemy.Dead || grabbedEnemy.Blocking)
						{
							StopGrabbingEnemy(true);
							break;
						}
						attackInfoPredator.Damage = wristBladesDamage;
						attackInfoPredator.AnimationNr = 4;
						grabbedEnemy.ApplyDamageGrabbed(attackInfoPredator);
						if (!grabbedEnemy.GrabbedVictim || grabbedEnemy.Dead || grabbedEnemy.Blocking)
						{
							StopGrabbingEnemy(true);
							break;
						}
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
						}
						anim["wrist_finish_head_off"].time = 0f;
						anim.CrossFade("wrist_finish_head_off", 0.1f);
						TrailRendererHandL.gameObject.active = true;
						if ((bool)grabbedEnemy.deadHead)
						{
							headOffPrey = grabbedEnemy.deadHead;
						}
						else
						{
							Debug.LogError("dead head not set on victim");
						}
						StopCoroutine("GrabbingEnemy");
						StartCoroutine("GrabbingEnemy");
						float timer11 = 13f / 15f;
						targetRotation = Quaternion.LookRotation(attackDirection12);
						while (timer11 > 0f)
						{
							timer11 -= Time.deltaTime;
							xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
							yield return null;
						}
						xForm.rotation = targetRotation;
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeHitSquish2);
						}
						yield return new WaitForSeconds(17f / 30f);
						if ((bool)currentGrabbedTarget)
						{
							MainCamera.ShakeCamera(0.5f);
							if (sfxOn)
							{
								base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeHitImpale);
								base.GetComponent<AudioSource>().PlayOneShot(soundTrophyHeadOff);
							}
							if ((bool)headOffPrey)
							{
								if (bloodOn && !(grabbedEnemy is BaseSuperPredator) && (bool)particleBloodFountainDeadHead)
								{
									particleBloodFountainDeadHead.Emit();
								}
								currentGrabbedTarget.SendMessage("CutHeadOff", SendMessageOptions.DontRequireReceiver);
								headOffPrey.active = true;
							}
						}
						StartCoroutine(HideHeadOffPrey());
						yield return new WaitForSeconds(23f / 30f);
						MainCamera.ZoomOut();
						if (currentWeaponType != 1 && currentWeaponType != 2)
						{
							WristBladeLMesh.gameObject.active = false;
							WristBladeRMesh.gameObject.active = false;
						}
						if (currentWeaponType == 3)
						{
							SpearMesh.gameObject.active = true;
						}
						else if (currentWeaponType == 4)
						{
							ActivateWhipWeapon();
						}
					}
				}
			}
			else
			{
				StopGrabbingEnemy(true);
			}
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.WristGrabCutHalf:
		{
			Vector3 attackDirection12 = FindEnemyTargetInFront(wristBladesCheckRadius, true);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			if (targetEnemyFound && !gettingHurt)
			{
				if ((bool)currentGrabbedTarget)
				{
					grabbedEnemy = (BaseEnemy)currentGrabbedTarget.GetComponent(typeof(BaseEnemy));
					if ((bool)grabbedEnemy)
					{
						if (!grabbedEnemy.GrabbedVictim || grabbedEnemy.Dead || grabbedEnemy.Blocking)
						{
							StopGrabbingEnemy(true);
							break;
						}
						attackInfoPredator.Damage = wristBladesDamage;
						attackInfoPredator.AnimationNr = 3;
						currentGrabbedTarget.SendMessage("ApplyDamageGrabbed", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
						TrailRendererHandL.gameObject.active = true;
						anim["wrist_finish_cut_half"].time = 0f;
						anim.CrossFade("wrist_finish_cut_half", 0.1f);
						if (currentWeaponType == 3 || currentWeaponType == 4)
						{
							WristBladeLMesh.gameObject.active = true;
							WristBladeRMesh.gameObject.active = true;
							SpearMesh.gameObject.active = false;
							whipComponent.HideWhip();
						}
						StopCoroutine("GrabbingEnemy");
						StartCoroutine("GrabbingEnemy");
						float timer11 = 7f / 30f;
						targetRotation = Quaternion.LookRotation(attackDirection12);
						while (timer11 > 0f)
						{
							timer11 -= Time.deltaTime;
							xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
							yield return null;
						}
						xForm.rotation = targetRotation;
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
						}
						yield return new WaitForSeconds(0.6f);
						if ((bool)currentGrabbedTarget)
						{
							MainCamera.ShakeCamera(0.5f);
						}
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWristBladeAttack);
						}
						yield return new WaitForSeconds(0.8f);
						if ((bool)currentGrabbedTarget)
						{
							MainCamera.ShakeCamera(0.5f);
							ShowBloodSplatScreen(DeathType.BodySplice);
							if (sfxOn)
							{
								base.GetComponent<AudioSource>().PlayOneShot(soundBreakApartEnemy);
								base.GetComponent<AudioSource>().PlayOneShot(soundTrophyRipHalf);
							}
						}
						yield return new WaitForSeconds(1.0666667f);
						MainCamera.ZoomOut();
						if (currentWeaponType != 1 && currentWeaponType != 2)
						{
							WristBladeLMesh.gameObject.active = false;
							WristBladeRMesh.gameObject.active = false;
						}
						if (currentWeaponType == 3)
						{
							SpearMesh.gameObject.active = true;
						}
						else if (currentWeaponType == 4)
						{
							ActivateWhipWeapon();
						}
					}
				}
			}
			else
			{
				StopGrabbingEnemy(true);
			}
			TrailRendererHandL.gameObject.active = false;
			break;
		}
		case AttackAnimation.SpearLightL:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			SpearMesh.gameObject.active = true;
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
			}
			anim["spear_attack_light_L"].time = 0f;
			anim.CrossFade("spear_attack_light_L", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(spearCheckRadius, false);
			TrailRendererSpearFront.gameObject.active = true;
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			if (targetEnemyFound)
			{
				float timer11 = 2f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 3.6f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = spearDamage;
				attackInfoPredator.AnimationNr = 2;
				attackInfoPredator.AttackerPosition = xForm.position;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t5 = 0; t5 < enemyTargets.Count; t5++)
				{
					enemyTempTransform = (Transform)enemyTargets[t5];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && (xForm.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius && (currentTarget.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius)
					{
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else
			{
				yield return new WaitForSeconds(2f / 15f);
				float timer11 = 0.2f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 3.6f * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 3.6f * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererSpearFront.gameObject.active = false;
			break;
		}
		case AttackAnimation.SpearLightR:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			SpearMesh.gameObject.active = true;
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
			}
			anim["spear_attack_light_R"].time = 0f;
			anim.CrossFade("spear_attack_light_R", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(spearCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererSpearBack.gameObject.active = true;
			if (targetEnemyFound)
			{
				float timer11 = 2f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 3.6f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = spearDamage;
				attackInfoPredator.AnimationNr = 3;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit2);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t7 = 0; t7 < enemyTargets.Count; t7++)
				{
					enemyTempTransform = (Transform)enemyTargets[t7];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && (xForm.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius && (currentTarget.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius)
					{
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else
			{
				yield return new WaitForSeconds(2f / 15f);
				float timer11 = 0.2f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 3.6f * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 3.6f * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererSpearBack.gameObject.active = false;
			break;
		}
		case AttackAnimation.SpearLightFront:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			SpearMesh.gameObject.active = true;
			anim["spear_attack_light_Front"].time = 0f;
			anim.CrossFade("spear_attack_light_Front", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(spearCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			TrailRendererSpearFront.gameObject.active = true;
			TrailRendererSpearBack.gameObject.active = true;
			if (targetEnemyFound)
			{
				float timer11 = 7f / 30f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				timer11 = 1f / 6f;
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
				}
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 3.48f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = spearDamage;
				attackInfoPredator.AnimationNr = 3;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit2);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				float timer11;
				if (lDiff3 != Vector3.zero)
				{
					attackDirection12 = lDiff3;
					timer11 = 7f / 30f;
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					yield return new WaitForSeconds(7f / 30f);
				}
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
				}
				timer11 = 1f / 6f;
				attackDirection12 = lDiff3;
				if (lDiff3 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 3.48f * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 3.48f * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererSpearFront.gameObject.active = false;
			TrailRendererSpearBack.gameObject.active = false;
			break;
		}
		case AttackAnimation.SpearHeavySpin:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			SpearMesh.gameObject.active = true;
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
			}
			anim["spear_attack_heavy_Spin"].time = 0f;
			anim.CrossFade("spear_attack_heavy_Spin", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(spearCheckRadius, false);
			TrailRendererSpearFront.gameObject.active = true;
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			if (targetEnemyFound)
			{
				float timer11 = 11f / 30f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = spearDamage * 4f;
				attackInfoPredator.AnimationNr = 5;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				yield return null;
				for (int t10 = 0; t10 < enemyTargets.Count; t10++)
				{
					enemyTempTransform = (Transform)enemyTargets[t10];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && (xForm.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius)
					{
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else
			{
				yield return new WaitForSeconds(11f / 30f);
				for (int t8 = 0; t8 < enemyTargets.Count; t8++)
				{
					Transform enemyXform = (Transform)enemyTargets[t8];
					if ((bool)enemyXform && (xForm.position - enemyXform.position).sqrMagnitude < spearCheckRadius * spearCheckRadius)
					{
						attackInfoPredator.Damage = spearDamage;
						attackInfoPredator.AnimationNr = 2;
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit);
						}
						enemyXform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			TrailRendererSpearFront.gameObject.active = false;
			break;
		}
		case AttackAnimation.SpearHeavyR:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			SpearMesh.gameObject.active = true;
			Vector3 attackDirection12 = FindEnemyTargetInFront(spearCheckRadius, false);
			TrailRendererSpearFront.gameObject.active = true;
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			anim["spear_attack_heavy_R"].time = 0f;
			anim.CrossFade("spear_attack_heavy_R", 0.1f);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
			}
			if (targetEnemyFound)
			{
				float timer11 = 2f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 2.67f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = spearDamage * 2f;
				attackInfoPredator.AnimationNr = 1;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(1.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t12 = 0; t12 < enemyTargets.Count; t12++)
				{
					enemyTempTransform = (Transform)enemyTargets[t12];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && (xForm.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius && (currentTarget.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius)
					{
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else
			{
				float timer11 = 0.2f;
				if (lDiff3 != Vector3.zero)
				{
					attackDirection12 = lDiff3;
					targetRotation = Quaternion.LookRotation(lDiff3);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 2.67f * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 2.67f * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererSpearFront.gameObject.active = false;
			break;
		}
		case AttackAnimation.SpearHeavyL:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			SpearMesh.gameObject.active = true;
			Vector3 attackDirection12 = FindEnemyTargetInFront(spearCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			anim["spear_attack_heavy_L"].time = 0f;
			anim.CrossFade("spear_attack_heavy_L", 0.1f);
			TrailRendererSpearFront.gameObject.active = true;
			if (targetEnemyFound)
			{
				float timer11 = 1f / 6f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 2.7f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
				}
				timer11 = 13f / 30f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 1.176f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				yield return new WaitForSeconds(1f / 15f);
				if ((bool)currentTarget)
				{
					attackInfoPredator.Damage = spearDamage * 2f;
					attackInfoPredator.AnimationNr = 1;
					MainCamera.ShakeCamera(1.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit2);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t13 = 0; t13 < enemyTargets.Count; t13++)
				{
					enemyTempTransform = (Transform)enemyTargets[t13];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && (xForm.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius && (currentTarget.position - enemyTempTransform.position).sqrMagnitude < spearDamageRadius * spearDamageRadius)
					{
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
				timer11 = 7f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 2.3f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
			}
			else
			{
				float timer11 = 1f / 6f;
				if (lDiff3 != Vector3.zero)
				{
					attackDirection12 = lDiff3;
					targetRotation = Quaternion.LookRotation(attackDirection12);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 2.7f * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 2.7f * Time.deltaTime);
						yield return null;
					}
				}
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
				}
				timer11 = 13f / 30f;
				if (lDiff3 != Vector3.zero)
				{
					attackDirection12 = lDiff3;
					targetRotation = Quaternion.LookRotation(attackDirection12);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 1.176f * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 1.176f * Time.deltaTime);
						yield return null;
					}
				}
				yield return new WaitForSeconds(8f / 15f);
				timer11 = 0.2f;
				if (lDiff3 != Vector3.zero)
				{
					attackDirection12 = lDiff3;
					targetRotation = Quaternion.LookRotation(attackDirection12);
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 2.3f * Time.deltaTime);
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
						yield return null;
					}
				}
				else
				{
					attackDirection12 = xForm.forward;
					while (timer11 > 0f)
					{
						timer11 -= Time.deltaTime;
						playerController.Move(attackDirection12 * 2.3f * Time.deltaTime);
						yield return null;
					}
				}
			}
			TrailRendererSpearFront.gameObject.active = false;
			break;
		}
		case AttackAnimation.SpearHeavyFront:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			SpearMesh.gameObject.active = true;
			TrailRendererSpearFront.gameObject.active = true;
			Vector3 attackDirection12 = FindEnemyTargetInFront(spearCheckRadius, false);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundSpearAttack);
			}
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			anim["spear_attack_heavy_Front"].time = 0f;
			anim.CrossFade("spear_attack_heavy_Front", 0.1f);
			if (targetEnemyFound)
			{
				float timer11 = 17f / 30f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.AttackerPosition = xForm.position;
				attackInfoPredator.Damage = spearDamage * 10f;
				attackInfoPredator.AnimationNr = 7;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(1.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundSpearHit);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (lDiff3 != Vector3.zero)
			{
				float timer11 = 17f / 30f;
				attackDirection12 = lDiff3;
				targetRotation = Quaternion.LookRotation(lDiff3);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
			}
			else
			{
				yield return new WaitForSeconds(17f / 30f);
			}
			TrailRendererSpearFront.gameObject.active = false;
			break;
		}
		case AttackAnimation.HurtHeavyMoveBack:
		{
			gettingHurt = true;
			isControllable = true;
			meleeLightAttacking = false;
			chargingHeavyAttack = false;
			dashJumping = false;
			grabbingEnemy = false;
			performingAttackAnimation = false;
			currentPerformingAnimation = AttackAnimation.None;
			conditionPerformedHeavyAttackHit = false;
			TrailRendererHandL.gameObject.active = false;
			TrailRendererHandR.gameObject.active = false;
			TrailRendererSpearFront.gameObject.active = false;
			TrailRendererSpearBack.gameObject.active = false;
			WhipTrailRenderer.enabled = false;
			anim["hurt_move_bck"].time = 0f;
			anim.CrossFade("hurt_move_bck");
			float timer11 = 0.8f;
			Vector3 takeDamageMoveDirection = -takeDamageDirection.normalized;
			targetRotation = Quaternion.LookRotation(takeDamageDirection);
			while (timer11 > 0f)
			{
				timer11 -= Time.deltaTime;
				xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed * 100f);
				playerController.Move(takeDamageMoveDirection * 1.5f * Time.deltaTime);
				yield return null;
			}
			gettingHurt = false;
			if (currentWeaponType != 1 && currentWeaponType != 2)
			{
				WristBladeLMesh.gameObject.active = false;
				WristBladeRMesh.gameObject.active = false;
			}
			if (currentWeaponType == 3)
			{
				SpearMesh.gameObject.active = true;
			}
			else if (currentWeaponType == 4)
			{
				ActivateWhipWeapon();
			}
			break;
		}
		case AttackAnimation.WhipLightL:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			WhipTrailRenderer.enabled = true;
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWhipAttack2);
			}
			anim["whip_attack_light_L"].time = 0f;
			anim.CrossFade("whip_attack_light_L", 0.1f);
			whipAnim["whip_attack_light_L"].time = 0f;
			whipAnim.CrossFade("whip_attack_light_L", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(whipCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			float timer11;
			if (targetEnemyFound)
			{
				timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				timer11 = 0.1f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = whipDamage;
				attackInfoPredator.AnimationNr = 2;
				attackInfoPredator.AttackerPosition = xForm.position;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWhipHit);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t15 = 0; t15 < enemyTargets.Count; t15++)
				{
					enemyTempTransform = (Transform)enemyTargets[t15];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWhipHit);
						}
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
				timer11 = 1f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				break;
			}
			yield return new WaitForSeconds(0.2f);
			timer11 = 1f / 6f;
			attackDirection12 = lDiff3;
			if (lDiff3 != Vector3.zero)
			{
				targetRotation = Quaternion.LookRotation(lDiff3);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
			}
			else
			{
				attackDirection12 = xForm.forward;
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					yield return null;
				}
			}
			break;
		}
		case AttackAnimation.WhipLightR:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			WhipTrailRenderer.enabled = true;
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWhipAttack1);
			}
			anim["whip_attack_light_R"].time = 0f;
			anim.CrossFade("whip_attack_light_R", 0.1f);
			whipComponent.CrossFadeAnim("whip_attack_light_R", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(whipCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			float timer11;
			if (targetEnemyFound)
			{
				timer11 = 0.2f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				timer11 = 0.1f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = whipDamage;
				attackInfoPredator.AnimationNr = 1;
				attackInfoPredator.AttackerPosition = xForm.position;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWhipHit);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t17 = 0; t17 < enemyTargets.Count; t17++)
				{
					enemyTempTransform = (Transform)enemyTargets[t17];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWhipHit);
						}
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
				timer11 = 1f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				break;
			}
			yield return new WaitForSeconds(0.2f);
			timer11 = 1f / 6f;
			attackDirection12 = lDiff3;
			if (lDiff3 != Vector3.zero)
			{
				targetRotation = Quaternion.LookRotation(lDiff3);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
			}
			else
			{
				attackDirection12 = xForm.forward;
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 4.5f * Time.deltaTime);
					yield return null;
				}
			}
			break;
		}
		case AttackAnimation.WhipSpin:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			WhipTrailRenderer.enabled = true;
			anim["whip_attack_spin"].time = 0f;
			anim.CrossFade("whip_attack_spin", 0.1f);
			whipComponent.CrossFadeAnim("whip_attack_spin", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(whipCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			yield return new WaitForSeconds(7f / 30f);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWhipSpinAttack);
			}
			attackInfoPredator.Damage = whipDamage;
			attackInfoPredator.AnimationNr = 5;
			attackInfoPredator.AttackerPosition = xForm.position;
			for (int i = 0; i < 4; i++)
			{
				float timer11 = 1f / 15f;
				if (!targetEnemyFound)
				{
					attackDirection12 = lDiff3;
				}
				if (attackDirection12 != Vector3.zero)
				{
					targetRotation = Quaternion.LookRotation(attackDirection12);
				}
				else
				{
					attackDirection12 = xForm.forward;
				}
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 5.5f * Time.deltaTime);
					if (attackDirection12 != Vector3.zero)
					{
						xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					}
					yield return null;
				}
				if (i == 1)
				{
					for (int t19 = 0; t19 < enemyTargets.Count; t19++)
					{
						enemyTempTransform = (Transform)enemyTargets[t19];
						if ((bool)enemyTempTransform && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipXForm.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
						{
							MainCamera.ShakeCamera(0.5f);
							if (sfxOn)
							{
								base.GetComponent<AudioSource>().PlayOneShot(soundWhipHit);
							}
							enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
						}
					}
					continue;
				}
				for (int t19 = 0; t19 < enemyTargets.Count; t19++)
				{
					enemyTempTransform = (Transform)enemyTargets[t19];
					if ((bool)enemyTempTransform && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
					{
						MainCamera.ShakeCamera(0.5f);
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWhipHit);
						}
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			break;
		}
		case AttackAnimation.WhipHeavyR:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			WhipTrailRenderer.enabled = true;
			anim["whip_attack_heavy_R"].time = 0f;
			anim.CrossFade("whip_attack_heavy_R", 0.1f);
			whipAnim["whip_attack_heavy_R"].time = 0f;
			whipAnim.CrossFade("whip_attack_heavy_R", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(whipCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWhipAttack1);
			}
			float timer11;
			if (targetEnemyFound)
			{
				timer11 = 2f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 3.42f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = whipDamage;
				attackInfoPredator.AnimationNr = 1;
				for (int t9 = 0; t9 < enemyTargets.Count; t9++)
				{
					enemyTempTransform = (Transform)enemyTargets[t9];
					if ((bool)enemyTempTransform && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
					{
						MainCamera.ShakeCamera(0.5f);
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
						}
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
				timer11 = 1f / 15f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t9 = 0; t9 < enemyTargets.Count; t9++)
				{
					enemyTempTransform = (Transform)enemyTargets[t9];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
						}
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
				break;
			}
			attackInfoPredator.Damage = whipDamage;
			attackInfoPredator.AnimationNr = 1;
			timer11 = 2f / 15f;
			if (lDiff3 != Vector3.zero)
			{
				attackDirection12 = lDiff3;
				targetRotation = Quaternion.LookRotation(lDiff3);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 3.42f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
			}
			else
			{
				attackDirection12 = xForm.forward;
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 3.42f * Time.deltaTime);
					yield return null;
				}
			}
			for (int t14 = 0; t14 < enemyTargets.Count; t14++)
			{
				enemyTempTransform = (Transform)enemyTargets[t14];
				if ((bool)enemyTempTransform && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
					}
					enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			yield return new WaitForSeconds(1f / 15f);
			for (int t14 = 0; t14 < enemyTargets.Count; t14++)
			{
				enemyTempTransform = (Transform)enemyTargets[t14];
				if ((bool)enemyTempTransform && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
					}
					enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			break;
		}
		case AttackAnimation.WhipHeavyF:
		{
			if (grabbingEnemy)
			{
				StopGrabbingEnemy(false);
			}
			WhipTrailRenderer.enabled = true;
			anim["whip_attack_heavy_front"].time = 0f;
			anim.CrossFade("whip_attack_heavy_front", 0.1f);
			whipAnim["whip_attack_heavy_front"].time = 0f;
			whipAnim.CrossFade("whip_attack_heavy_front", 0.1f);
			Vector3 attackDirection12 = FindEnemyTargetInFront(whipCheckRadius, false);
			if (attackDirection12 != Vector3.up)
			{
				targetEnemyFound = true;
			}
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundWhipAttack2);
			}
			float timer11;
			if (targetEnemyFound)
			{
				timer11 = 11f / 30f;
				targetRotation = Quaternion.LookRotation(attackDirection12);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 2.67f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
				attackInfoPredator.Damage = whipDamage * 4f;
				attackInfoPredator.AnimationNr = 7;
				attackInfoPredator.AttackerPosition = xForm.position;
				if ((bool)currentTarget)
				{
					MainCamera.ShakeCamera(1.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
					}
					currentTarget.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
				for (int t4 = 0; t4 < enemyTargets.Count; t4++)
				{
					enemyTempTransform = (Transform)enemyTargets[t4];
					if ((bool)enemyTempTransform && enemyTempTransform != currentTarget && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
						}
						enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
					}
				}
				break;
			}
			attackInfoPredator.Damage = whipDamage * 4f;
			attackInfoPredator.AnimationNr = 7;
			attackInfoPredator.AttackerPosition = xForm.position;
			timer11 = 11f / 30f;
			if (lDiff3 != Vector3.zero)
			{
				attackDirection12 = lDiff3;
				targetRotation = Quaternion.LookRotation(lDiff3);
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 2.67f * Time.deltaTime);
					xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
					yield return null;
				}
			}
			else
			{
				attackDirection12 = xForm.forward;
				while (timer11 > 0f)
				{
					timer11 -= Time.deltaTime;
					playerController.Move(attackDirection12 * 2.67f * Time.deltaTime);
					yield return null;
				}
			}
			for (int t6 = 0; t6 < enemyTargets.Count; t6++)
			{
				enemyTempTransform = (Transform)enemyTargets[t6];
				if ((bool)enemyTempTransform && ((WhipEndPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius || (WhipMidPoint.position - enemyTempTransform.position).sqrMagnitude < whipDamageRadius * whipDamageRadius))
				{
					MainCamera.ShakeCamera(0.5f);
					if (sfxOn)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundWhipHitHeavy);
					}
					enemyTempTransform.SendMessage("ApplyDamage", attackInfoPredator, SendMessageOptions.DontRequireReceiver);
				}
			}
			break;
		}
		}
		performingAttackAnimation = false;
		currentPerformingAnimation = AttackAnimation.None;
		conditionPerformedHeavyAttackHit = false;
	}

	private IEnumerator ComboLightAdd()
	{
		comboAttackLight = true;
		yield return null;
		yield return null;
		comboAttackLight = false;
	}

	private IEnumerator ComboSecondaryAdd()
	{
		comboAttackSecondary = true;
		yield return null;
		yield return null;
		comboAttackSecondary = false;
	}

	private IEnumerator ComboCheckInput(float initialDelay, float checkTime, bool lightAttack)
	{
		yield return new WaitForSeconds(initialDelay);
		float timer2;
		if (lightAttack)
		{
			pressedComboLight = false;
			timer2 = checkTime;
			while (timer2 > 0f)
			{
				if (comboAttackLight)
				{
					pressedComboLight = true;
				}
				if (comboAttackSecondary)
				{
					pressedComboSecondary = true;
				}
				timer2 -= Time.deltaTime;
				yield return null;
			}
			yield return new WaitForSeconds(checkTime);
			pressedComboLight = false;
			pressedComboSecondary = false;
			yield break;
		}
		pressedComboHeavy = false;
		bool atttackButtonWasAlwaysDown = true;
		timer2 = checkTime;
		while (timer2 > 0f)
		{
			if (comboAttackSecondary)
			{
				pressedComboSecondary = true;
			}
			if (!attackButtonDown)
			{
				atttackButtonWasAlwaysDown = false;
			}
			timer2 -= Time.deltaTime;
			yield return null;
		}
		pressedComboHeavy = atttackButtonWasAlwaysDown;
		yield return new WaitForSeconds(checkTime);
		pressedComboSecondary = false;
		pressedComboHeavy = false;
	}

	public void SwitchToPauseMenu()
	{
		if (!paused)
		{
			if (musicOn)
			{
				MainCamera.StopMusic();
			}
			PauseMenuGroup.SetActiveRecursively(true);
			OptionsMenuGroup.SetActiveRecursively(false);
			ObjectivesMenuGroup.SetActiveRecursively(true);
			CombosMenuGroup.SetActiveRecursively(false);
			RestartMenuGroup.SetActiveRecursively(false);
			QuitMenuGroup.SetActiveRecursively(false);
			optionsMenu = false;
			restartMenu = false;
			quitMenu = false;
			cameraPause.enabled = true;
			pauseMenu = true;
			paused = true;
			Time.timeScale = 0f;
			Time.fixedDeltaTime = 0f;
		}
	}

	private IEnumerator ComboHeavyCharge()
	{
		if (!meleeLightAttacking && !grabbingEnemy && !gettingHurt)
		{
			chargingHeavyAttack = true;
			switch (currentWeaponType)
			{
			case 1:
			case 2:
				if (!anim.IsPlaying("attack_heavy_R_charge"))
				{
					anim["attack_heavy_R_charge"].time = 0f;
					anim.CrossFade("attack_heavy_R_charge", 0.1f);
				}
				break;
			case 3:
				if (!anim.IsPlaying("spear_attack_heavy_R_charge"))
				{
					anim["spear_attack_heavy_R_charge"].time = 0f;
					anim.CrossFade("spear_attack_heavy_R_charge", 0.1f);
				}
				break;
			case 4:
				if (!anim.IsPlaying("whip_attack_heavy_R_charge"))
				{
					anim["whip_attack_heavy_R_charge"].time = 0f;
					anim.CrossFade("whip_attack_heavy_R_charge", 0.1f);
					whipAnim["whip_attack_heavy_R_charge"].time = 0f;
					whipAnim.CrossFade("whip_attack_heavy_R_charge", 0.1f);
				}
				break;
			}
		}
		comboAttackHeavy = false;
		bool heldAttackButton2 = false;
		float timer = timerComboHeavy;
		heldAttackButton2 = true;
		while (timer > 0f)
		{
			if (!attackButtonDown)
			{
				heldAttackButton2 = false;
				break;
			}
			timer -= Time.deltaTime;
			yield return null;
		}
		if (heldAttackButton2)
		{
			comboAttackHeavy = true;
			if (!meleeLightAttacking && !grabbingEnemy && !gettingHurt && !performingAttackAnimation)
			{
				chargingHeavyAttack = false;
				switch (currentWeaponType)
				{
				case 1:
				case 2:
					yield return StartCoroutine("PerformAnimation", AttackAnimation.WristHeavyR);
					break;
				case 3:
					StartCoroutine("ComboAttackPerform", spearComboHeavyAttack[0]);
					break;
				case 4:
					StartCoroutine("ComboAttackPerform", whipComboHeavyAttack[0]);
					break;
				}
			}
			yield return new WaitForSeconds(timerComboHeavy);
			comboAttackHeavy = false;
		}
		chargingHeavyAttack = false;
	}

	private IEnumerator GrabbingEnemy()
	{
		if (CurrentAttackAnimation == AttackAnimation.WristGrabHeadOff || CurrentAttackAnimation == AttackAnimation.WristGrabCutHalf)
		{
			yield return new WaitForSeconds(timerGrabEnemy);
		}
		yield return new WaitForSeconds(timerGrabEnemy);
		StopGrabbingEnemy(true);
	}

	public IEnumerator GUIControlBlink(Renderer guiControlRenderer, int timesToBlink, float timeUntilStart)
	{
		if (timeUntilStart > 0f)
		{
			yield return new WaitForSeconds(timeUntilStart);
		}
		for (int i = 0; i < timesToBlink; i++)
		{
			guiControlRenderer.enabled = false;
			yield return new WaitForSeconds(0.2f);
			guiControlRenderer.enabled = true;
			yield return new WaitForSeconds(0.2f);
		}
	}

	public IEnumerator GUIControlBlinkDisappear()
	{
		for (int i = 0; i < 2; i++)
		{
			LeftControlPad.GetComponent<Renderer>().enabled = true;
			LeftStick.GetComponent<Renderer>().enabled = true;
			yield return new WaitForSeconds(0.2f);
			LeftControlPad.GetComponent<Renderer>().enabled = false;
			LeftStick.GetComponent<Renderer>().enabled = false;
			yield return new WaitForSeconds(0.2f);
		}
	}

	public void ShowBloodSplatScreen(DeathType deathType)
	{
		StopCoroutine("BloodSplatScreenLeftShow");
		BloodSplatScreenLong.active = false;
		BloodKillText.gameObject.active = false;
		StartCoroutine("BloodSplatScreenLeftShow", deathType);
	}

	private IEnumerator BloodSplatScreenLeftShow(DeathType deathType)
	{
		switch (deathType)
		{
		case DeathType.BodyCut:
		case DeathType.DiscCut:
			BloodKillText.text = Language.GetTxt("BODY_CUT");
			break;
		case DeathType.BodySplice:
			BloodKillText.text = Language.GetTxt("BODY_SPLICE");
			break;
		case DeathType.TrophyKill:
			BloodKillText.text = Language.GetTxt("TROPHY_KILL");
			break;
		case DeathType.DoubleSplice:
			BloodKillText.text = Language.GetTxt("DOUBLE_SPLICE");
			break;
		case DeathType.SuperSplice:
			BloodKillText.text = Language.GetTxt("SUPER_SPLICE");
			break;
		case DeathType.VerticalCut:
			BloodKillText.text = Language.GetTxt("PERFECT_SPLIT");
			break;
		}
		if (bloodOn)
		{
			BloodSplatScreenLong.active = true;
		}
		BloodKillText.gameObject.active = true;
		yield return new WaitForSeconds(3f);
		BloodSplatScreenLong.active = false;
		BloodKillText.gameObject.active = false;
	}

	public void SetupControlsUI()
	{
		Vector2 vector = cameraMain.WorldToScreenPoint(GUI_WeaponWristBlades_Active.transform.position);
		Vector2 vector2 = cameraMain.WorldToScreenPoint(GUI_WeaponPlasmaGun_Active.transform.position);
		Vector2 vector3 = cameraMain.WorldToScreenPoint(AttackStick.transform.position);
		Vector2 vector4 = cameraMain.WorldToScreenPoint(BlockStick.transform.position);
		Vector2 vector5 = cameraMain.WorldToScreenPoint(GUI_Cloak.transform.position);
		Vector2 vector6 = cameraMain.WorldToScreenPoint(GUI_Thermal.transform.position);
		Vector2 vector7 = cameraMain.WorldToScreenPoint(GUI_PauseButton.transform.position);
		Vector2 vector8 = cameraMain.WorldToScreenPoint(tipTextures[2].position);
		Vector2 vector9 = cameraMain.WorldToScreenPoint(tipTextures[3].position);
		Vector2 vector10 = cameraMain.WorldToScreenPoint(tipTextures[4].position);
		leftStickCenter = cameraMain.WorldToScreenPoint(LeftStick.transform.position);
		Vector2 vector11;
		Vector2 vector12;
		Vector2 vector13;
		float num;
		float num2;
		float num3;
		float num4;
		float num5;
		float num6;
		float num7;
		float num8;
		if (PlatformDependent.tablet)
		{
			ControlPadRadius = 0.008662032f;
			verticalAspectRatio = (float)Screen.height / 768f;
			vector11 = new Vector2(385f, 198f) * verticalAspectRatio;
			vector12 = new Vector2(385f, 138f) * verticalAspectRatio;
			vector13 = new Vector2(385f, 198f) * verticalAspectRatio;
			num = 105f * verticalAspectRatio;
			num2 = 60f * verticalAspectRatio;
			num3 = 95f * verticalAspectRatio;
			num4 = 95f * verticalAspectRatio;
			num5 = 100f * verticalAspectRatio;
			num6 = 70f * verticalAspectRatio;
			num7 = 90f * verticalAspectRatio;
			num8 = 80f * verticalAspectRatio;
			iPadLeftScreenBounds = new Rect(0f, 0f, 500f * verticalAspectRatio, 650f * verticalAspectRatio);
			LeftStick.GetComponent<Renderer>().enabled = false;
			LeftControlPad.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			verticalAspectRatio = (float)Screen.height / 320f;
			vector11 = new Vector2(230f, 120f) * verticalAspectRatio;
			vector12 = new Vector2(230f, 78f) * verticalAspectRatio;
			vector13 = new Vector2(230f, 120f) * verticalAspectRatio;
			num = 78f * verticalAspectRatio;
			num2 = 65f * verticalAspectRatio;
			num3 = 75f * verticalAspectRatio;
			num4 = 75f * verticalAspectRatio;
			num5 = 70f * verticalAspectRatio;
			num6 = 60f * verticalAspectRatio;
			num7 = 70f * verticalAspectRatio;
			num8 = 50f * verticalAspectRatio;
		}
		weaponABounds = new Rect(vector.x - num / 2f, vector.y - num2 / 2f, num, num2);
		weaponBBounds = new Rect(vector2.x - num / 2f, vector2.y - num2 / 2f, num, num2);
		hud.buttonPause.rectangle = new Rect(vector7.x - num7 / 2f, vector7.y - num8 / 2f, num7, num8);
		visionModeBounds = new Rect(vector6.x - num5 / 2f, vector6.y - num6 / 2f, num5, num6);
		cloakModeBounds = new Rect(vector5.x - num5 / 2f, vector5.y - num6 / 2f, num5, num6);
		buttonBlock = new Rect(vector4.x - num3 / 2f, vector4.y - num4 / 2f, num3, num4);
		buttonAttack = new Rect(vector3.x - num3 / 2f, vector3.y - num4 / 2f, num3, num4);
		tipTexturesBounds[2] = new Rect(vector8.x - vector11.x / 2f, vector8.y - vector11.y / 2f, vector11.x, vector11.y);
		tipTexturesBounds[3] = new Rect(vector9.x - vector12.x / 2f, vector9.y - vector12.y / 2f, vector12.x, vector12.y);
		tipTexturesBounds[4] = new Rect(vector10.x - vector13.x / 2f, vector10.y - vector13.y / 2f, vector13.x, vector13.y);
	}

	private void ShowLoadingScreen()
	{
		if ((bool)loadingScreen)
		{
			loadingScreen.gameObject.SetActiveRecursively(true);
			failMissionButtonQuit.gameObject.SetActiveRecursively(false);
			failMissionButtonRetry.gameObject.SetActiveRecursively(false);
			failMissionTextMessage.gameObject.SetActiveRecursively(false);
			toolTipBackgroundMissionEnd.gameObject.SetActiveRecursively(false);
			horizontalLinesBackground.gameObject.active = false;
			successMissionButtonOk.gameObject.SetActiveRecursively(false);
			successMissionTextMessage.gameObject.SetActiveRecursively(false);
		}
		if ((bool)loadingScreenLeft)
		{
			loadingScreenLeft.SetActiveRecursively(true);
		}
	}

	public void OnPhoneSlidOpen(object sender, EventArgs ea)
	{
		AttackStick.gameObject.SetActiveRecursively(false);
		BlockStick.gameObject.SetActiveRecursively(false);
		LeftStick.gameObject.SetActiveRecursively(false);
		LeftControlPad.gameObject.SetActiveRecursively(false);
	}

	public void OnPhoneSlidClose(object sender, EventArgs ea)
	{
		AttackStick.gameObject.SetActiveRecursively(true);
		BlockStick.gameObject.SetActiveRecursively(true);
		LeftStick.gameObject.SetActiveRecursively(true);
		LeftControlPad.gameObject.SetActiveRecursively(true);
	}

	private void Awake()
	{
		buttonResume.onPressBegin += delegate
		{
			if (sfxOn && musicOn)
			{
				MainCamera.StartMusic();
			}
			PauseMenuGroup.SetActiveRecursively(false);
			OptionsMenuGroup.SetActiveRecursively(false);
			CombosMenuGroup.SetActiveRecursively(false);
			ObjectivesMenuGroup.SetActiveRecursively(false);
			RestartMenuGroup.SetActiveRecursively(false);
			QuitMenuGroup.SetActiveRecursively(false);
			pauseMenu = false;
			optionsMenu = false;
			restartMenu = false;
			quitMenu = false;
			paused = false;
			cameraPause.enabled = false;
			Time.timeScale = 1f;
			Time.fixedDeltaTime = 0.033f;
		};
		PlatformDependent.SetScreenOrientation(false);
		if (PlatformDependent.tablet)
		{
			hud = tabletHUD;
			UnityEngine.Object.Destroy(phoneHUD.gameObject);
		}
		else
		{
			hud = phoneHUD;
			UnityEngine.Object.Destroy(tabletHUD.gameObject);
		}
		hud.buttonPause.onPressBegin += delegate
		{
			SwitchToPauseMenu();
		};
		float num = (float)Screen.width / (float)Screen.height;
		Vector3 localScale = GUI_PainParent.localScale;
		localScale.x = 2f / 3f * num;
		GUI_PainParent.localScale = localScale;
		GUI_PainParent_MaxTransform.localScale = localScale;
		GUI_PainParent_MinTransform.localScale = GUI_PainParent_MinTransform.localScale * num * (2f / 3f);
		if ((bool)WhipWeapon)
		{
			whipComponent = (Whip)WhipWeapon.GetComponent(typeof(Whip));
		}
		paused = false;
		if (slowMotion)
		{
			Time.timeScale = 0.5f;
		}
		AManager.instance.humanTargets.Add(base.transform);
		AManager.instance.predatorTargets.Add(base.transform);
		GUI_PainParent.gameObject.SetActiveRecursively(false);
		bloodMaterialsHumanCount = bloodMaterialsHuman.Length;
		bloodMaterialsPredatorCount = bloodMaterialsPredator.Length;
		LoadResourcesTextures();
		PlatformDependent.HandleScreenDarken();
		PlatformDependent.HandleIphoneKeyboard();
		currentSlot = EncryptedPlayerPrefs.GetInt("PR_CurrentSlot");
		currentMission = EncryptedPlayerPrefs.GetInt("PR_CurrentMission_S" + currentSlot, debugCurrentMissionAutoStart);
		bloodMaterialsHumanCount = bloodMaterialsHuman.Length;
		bloodMaterialsPredatorCount = bloodMaterialsPredator.Length;
		RestoreMaterialsColors(false);
		string text = "_iPad";
		currentJungleType = EncryptedPlayerPrefs.GetInt("PR_CurrentJungleType", debugJungleType);
		switch (currentJungleType)
		{
		case 7:
			Application.LoadLevelAdditive("EnvironmentRocks" + text);
			materialNormal.color = ColorDaytime;
			currentCharactersColor = ColorDaytime;
			materialBerserkers.color = ColorDaytime;
			if ((bool)RainParticlesBack)
			{
				UnityEngine.Object.Destroy(RainParticlesBack.gameObject);
			}
			if ((bool)RainParticlesFront)
			{
				UnityEngine.Object.Destroy(RainParticlesFront.gameObject);
			}
			rainyEnvironment = false;
			break;
		case 1:
			Application.LoadLevelAdditive("EnvironmentJungleDay" + text);
			materialNormal.color = ColorDaytime;
			currentCharactersColor = ColorDaytime;
			materialBerserkers.color = ColorDaytime;
			if ((bool)RainParticlesBack)
			{
				UnityEngine.Object.Destroy(RainParticlesBack.gameObject);
			}
			if ((bool)RainParticlesFront)
			{
				UnityEngine.Object.Destroy(RainParticlesFront.gameObject);
			}
			rainyEnvironment = false;
			break;
		case 2:
			Application.LoadLevelAdditive("EnvironmentJungleNight" + text);
			materialNormal.color = ColorNighttime;
			materialBerserkers.color = ColorNighttime;
			currentCharactersColor = ColorNighttime;
			if ((bool)RainParticlesBack)
			{
				UnityEngine.Object.Destroy(RainParticlesBack.gameObject);
			}
			if ((bool)RainParticlesFront)
			{
				UnityEngine.Object.Destroy(RainParticlesFront.gameObject);
			}
			rainyEnvironment = false;
			break;
		case 3:
			Application.LoadLevelAdditive("EnvironmentDeadJungleDay" + text);
			materialNormal.color = ColorDaytime;
			materialBerserkers.color = ColorDaytime;
			currentCharactersColor = ColorDaytime;
			if ((bool)RainParticlesBack)
			{
				UnityEngine.Object.Destroy(RainParticlesBack.gameObject);
			}
			if ((bool)RainParticlesFront)
			{
				UnityEngine.Object.Destroy(RainParticlesFront.gameObject);
			}
			rainyEnvironment = false;
			break;
		case 4:
			Application.LoadLevelAdditive("EnvironmentDeadJungleNight" + text);
			materialNormal.color = ColorNighttime;
			currentCharactersColor = ColorNighttime;
			materialBerserkers.color = ColorNighttime;
			if ((bool)RainParticlesBack)
			{
				UnityEngine.Object.Destroy(RainParticlesBack.gameObject);
			}
			if ((bool)RainParticlesFront)
			{
				UnityEngine.Object.Destroy(RainParticlesFront.gameObject);
			}
			rainyEnvironment = false;
			break;
		case 5:
			Application.LoadLevelAdditive("EnvironmentJungleDayRain" + text);
			materialNormal.color = ColorRainy;
			currentCharactersColor = ColorRainy;
			materialBerserkers.color = ColorRainy;
			rainyEnvironment = true;
			break;
		case 6:
			Application.LoadLevelAdditive("EnvironmentDeadJungleDayRain" + text);
			materialNormal.color = ColorRainy;
			currentCharactersColor = ColorRainy;
			materialBerserkers.color = ColorRainy;
			rainyEnvironment = true;
			break;
		}
		cameraMain = MainCamera.GetComponent<Camera>();
		if (!cameraMain)
		{
			Debug.LogError("no Main Camera found");
		}
		enemyTargets = new ArrayList();
		cameraPause.enabled = true;
		if ((bool)loadingScreen)
		{
			((TextMesh)loadingScreen.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("LOADING");
		}
		ShowLoadingScreen();
		anim = (Animation)GetComponent(typeof(Animation));
		playerController = (CharacterController)GetComponent("CharacterController");
		maxHealth = health;
		LeftStickCenterLocalPosition = LeftStick.localPosition;
		InitWeaponStats();
		InitWeaponSlots();
		SetupControlsUI();
		musicOn = PlayerPrefs.GetInt("PR_MusicOn", 1) == 1;
		sfxOn = PlayerPrefs.GetInt("PR_SfxOn", 1) == 1;
		bloodOn = PlayerPrefs.GetInt("PR_BloodOn", 1) == 1;
		if (musicOn)
		{
			((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("ON");
		}
		else
		{
			MainCamera.StopMusic();
			((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("OFF");
		}
		if (sfxOn)
		{
			musicButActive = true;
			Utils.SfxOn = true;
			((TextMesh)sfxButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("SFX") + Language.GetTxt("ON");
		}
		else
		{
			((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("OFF");
			musicButActive = false;
			Utils.SfxOn = false;
			((TextMesh)sfxButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("SFX") + Language.GetTxt("OFF");
		}
		if (bloodOn)
		{
			AManager.BloodOn = true;
			((TextMesh)bloodButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("BLOOD") + Language.GetTxt("ON");
		}
		else
		{
			AManager.BloodOn = false;
			((TextMesh)bloodButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("BLOOD") + Language.GetTxt("OFF");
		}
		anim.wrapMode = WrapMode.Loop;
		anim["attack_light_wrist_L"].wrapMode = WrapMode.Once;
		anim["attack_light_wrist_R"].wrapMode = WrapMode.Once;
		anim["attack_heavy_R_charge"].wrapMode = WrapMode.Once;
		anim["attack_heavy_R_charge"].speed = 0.666f;
		anim["attack_heavy_R"].wrapMode = WrapMode.Once;
		anim["attack_heavy_L"].wrapMode = WrapMode.Once;
		anim["grab_start"].wrapMode = WrapMode.Once;
		anim["dash_start"].wrapMode = WrapMode.Once;
		anim["dash_pose"].wrapMode = WrapMode.ClampForever;
		anim["dash_land"].wrapMode = WrapMode.Once;
		anim["dash_attack_land"].wrapMode = WrapMode.Once;
		anim["dash_attack_scissor"].wrapMode = WrapMode.Once;
		anim["hurt_move_bck"].wrapMode = WrapMode.Once;
		anim["hurt_move_bck"].layer = 15;
		anim["grab_chop_Hi"].wrapMode = WrapMode.Once;
		anim["grab_chop_Low"].wrapMode = WrapMode.Once;
		anim["yell"].wrapMode = WrapMode.Once;
		anim["block_move_back"].wrapMode = WrapMode.Once;
		anim["block_break"].wrapMode = WrapMode.Once;
		anim["wrist_finish_cut_half"].wrapMode = WrapMode.Once;
		anim["wrist_finish_head_off"].wrapMode = WrapMode.Once;
		anim["die_fall_right"].wrapMode = WrapMode.ClampForever;
		anim["die_fall_forward"].wrapMode = WrapMode.ClampForever;
		anim["plasma_shoot"].wrapMode = WrapMode.Once;
		anim["disc_throw"].wrapMode = WrapMode.Once;
		anim["netgun_pose"].wrapMode = WrapMode.ClampForever;
		anim["netgun_shoot"].wrapMode = WrapMode.Once;
		anim["spear_attack_light_R"].wrapMode = WrapMode.Once;
		anim["spear_attack_light_L"].wrapMode = WrapMode.Once;
		anim["spear_attack_light_Front"].wrapMode = WrapMode.Once;
		anim["spear_attack_heavy_R"].wrapMode = WrapMode.Once;
		anim["spear_attack_heavy_R_charge"].wrapMode = WrapMode.Once;
		anim["spear_attack_heavy_L"].wrapMode = WrapMode.Once;
		anim["spear_attack_heavy_Front"].wrapMode = WrapMode.Once;
		anim["spear_attack_heavy_Spin"].wrapMode = WrapMode.Once;
		anim["spear_dash_start"].wrapMode = WrapMode.Once;
		anim["spear_dash_pose"].wrapMode = WrapMode.ClampForever;
		anim["spear_dash_land"].wrapMode = WrapMode.Once;
		anim["spear_grab_start"].wrapMode = WrapMode.Once;
		anim["spear_grab_chop_Hi"].wrapMode = WrapMode.Once;
		anim["spear_grab_chop_Low"].wrapMode = WrapMode.Once;
		anim["spear_finish_cut_half"].wrapMode = WrapMode.Once;
		anim["spear_finish_head_off"].wrapMode = WrapMode.Once;
		anim["spear_block_move_back"].wrapMode = WrapMode.Once;
		anim["spear_block_break"].wrapMode = WrapMode.Once;
		anim["whip_attack_light_R"].wrapMode = WrapMode.Once;
		anim["whip_attack_light_L"].wrapMode = WrapMode.Once;
		anim["whip_attack_spin"].wrapMode = WrapMode.Once;
		anim["whip_attack_heavy_R"].wrapMode = WrapMode.Once;
		anim["whip_attack_heavy_R_charge"].wrapMode = WrapMode.Once;
		anim["whip_attack_heavy_front"].wrapMode = WrapMode.Once;
		anim["whip_grapple_start"].wrapMode = WrapMode.Once;
		anim["whip_grapple_dash"].wrapMode = WrapMode.Once;
		anim["whip_grapple_dash_pose"].wrapMode = WrapMode.ClampForever;
		anim["whip_grapple_dash_grab"].wrapMode = WrapMode.Once;
		anim["whip_grapple_fail"].wrapMode = WrapMode.Once;
		anim["whip_attack_heavy_R_charge"].speed = 0.5f;
		whipAnim = WhipWeapon.GetComponent<Animation>();
		if ((bool)whipAnim)
		{
			whipAnim["whip_grapple_dash_pose"].wrapMode = WrapMode.ClampForever;
			whipAnim["whip_idle"].wrapMode = WrapMode.Loop;
			whipAnim["whip_attack_heavy_R_charge"].wrapMode = WrapMode.Once;
			whipAnim["whip_attack_heavy_R_charge"].speed = 0.5f;
			whipAnim["whip_retract"].wrapMode = WrapMode.Loop;
			WhipXForm = WhipWeapon.transform;
		}
		else
		{
			Debug.LogError("WhipWeapon Animation not set right");
		}
		anim["plasma_shoot"].AddMixingTransform(BoneMixChest, false);
		anim["plasma_shoot"].AddMixingTransform(BoneMixHead, false);
		anim["plasma_shoot"].AddMixingTransform(BoneMixLowerArmL, false);
		anim["plasma_shoot"].AddMixingTransform(BoneMixSpine, false);
		anim["plasma_shoot"].AddMixingTransform(BoneMixUpperArmL, false);
		anim["plasma_shoot"].AddMixingTransform(BoneMixUpperArmR, false);
		anim["plasma_shoot"].AddMixingTransform(BoneMixLowerArmR, false);
		anim["plasma_shoot"].blendMode = AnimationBlendMode.Additive;
		anim["plasma_shoot"].layer = 10;
		anim["disc_throw"].AddMixingTransform(BoneMixChest, false);
		anim["disc_throw"].AddMixingTransform(BoneMixHead, false);
		anim["disc_throw"].AddMixingTransform(BoneMixLowerArmL, false);
		anim["disc_throw"].AddMixingTransform(BoneMixSpine, false);
		anim["disc_throw"].AddMixingTransform(BoneMixUpperArmL, false);
		anim["disc_throw"].AddMixingTransform(BoneMixUpperArmR, false);
		anim["disc_throw"].AddMixingTransform(BoneMixLowerArmR, false);
		anim["disc_throw"].blendMode = AnimationBlendMode.Additive;
		anim["disc_throw"].layer = 10;
		anim["netgun_shoot"].AddMixingTransform(BoneMixChest, false);
		anim["netgun_shoot"].AddMixingTransform(BoneMixHead, false);
		anim["netgun_shoot"].AddMixingTransform(BoneMixLowerArmL, false);
		anim["netgun_shoot"].AddMixingTransform(BoneMixSpine, false);
		anim["netgun_shoot"].AddMixingTransform(BoneMixUpperArmL, false);
		anim["netgun_shoot"].AddMixingTransform(BoneMixUpperArmR, false);
		anim["netgun_shoot"].AddMixingTransform(BoneMixLowerArmR, false);
		anim["netgun_shoot"].blendMode = AnimationBlendMode.Additive;
		anim["netgun_shoot"].layer = 10;
		attackInfoPredator = new AttackInfo(wristBladesDamage, Vector3.zero, 1, true);
		Physics.IgnoreCollision(groundPlane.GetComponent<Collider>(), base.GetComponent<Collider>());
		GUI_ObjectivesText.text = string.Empty;
		wristComboAttack[0] = new AnimCombo();
		wristComboAttack[1] = new AnimCombo();
		wristComboAttack[2] = new AnimCombo();
		wristComboAttack[3] = new AnimCombo();
		wristScissorCombo = new AnimCombo();
		wristGrabComboAttack[0] = new AnimCombo();
		wristGrabComboAttack[1] = new AnimCombo();
		wristGrabComboAttack[2] = new AnimCombo();
		wristGrabComboSecondaryAttack[0] = new AnimCombo();
		wristGrabComboSecondaryAttack[1] = new AnimCombo();
		wristGrabComboSecondaryAttack[2] = new AnimCombo();
		spearComboLightAttack[0] = new AnimCombo();
		spearComboLightAttack[1] = new AnimCombo();
		spearComboLightAttack[2] = new AnimCombo();
		spearComboLightAttack[3] = new AnimCombo();
		spearComboHeavyAttack[0] = new AnimCombo();
		spearComboHeavyAttack[1] = new AnimCombo();
		spearComboHeavyAttack[2] = new AnimCombo();
		spearScissorCombo = new AnimCombo();
		whipComboLightAttack[0] = new AnimCombo();
		whipComboLightAttack[1] = new AnimCombo();
		whipComboLightAttack[2] = new AnimCombo();
		whipComboLightAttack[3] = new AnimCombo();
		whipComboHeavyAttack[0] = new AnimCombo();
		whipComboHeavyAttack[1] = new AnimCombo();
		wristScissorCombo.animationComboLength = 1f / 6f;
		wristScissorCombo.animationEndLength = 0.5f;
		wristScissorCombo.animationTypeLight = true;
		wristScissorCombo.animationToPerform = AttackAnimation.WristScissor;
		wristScissorCombo.nextAnimationCombo = wristComboAttack[0];
		wristScissorCombo.secondaryAnimation = AttackAnimation.WristGrabStart;
		wristComboAttack[0].animationComboLength = 0.23333335f;
		wristComboAttack[0].animationEndLength = 5f / 6f;
		wristComboAttack[0].animationTypeLight = true;
		wristComboAttack[0].animationToPerform = AttackAnimation.WristLightR;
		wristComboAttack[0].nextAnimationCombo = wristComboAttack[1];
		wristComboAttack[0].secondaryAnimation = AttackAnimation.WristGrabStart;
		wristComboAttack[1].animationComboLength = 0.23333335f;
		wristComboAttack[1].animationEndLength = 5f / 6f;
		wristComboAttack[1].animationTypeLight = true;
		wristComboAttack[1].animationToPerform = AttackAnimation.WristLightL;
		wristComboAttack[1].nextAnimationCombo = wristComboAttack[2];
		wristComboAttack[1].secondaryAnimation = AttackAnimation.WristGrabStart;
		wristComboAttack[2].animationComboLength = 0.23333335f;
		wristComboAttack[2].animationEndLength = 5f / 6f;
		wristComboAttack[2].animationTypeLight = true;
		wristComboAttack[2].animationToPerform = AttackAnimation.WristLightR;
		wristComboAttack[2].nextAnimationCombo = wristComboAttack[3];
		wristComboAttack[2].secondaryAnimation = AttackAnimation.WristGrabStart;
		wristComboAttack[3].animationComboLength = 13f / 30f;
		wristComboAttack[3].animationEndLength = 19f / 30f;
		wristComboAttack[3].animationTypeLight = false;
		wristComboAttack[3].animationToPerform = AttackAnimation.WristSpinL;
		wristComboAttack[3].nextAnimationCombo = null;
		wristGrabComboAttack[0].animationComboLength = 1f;
		wristGrabComboAttack[0].animationEndLength = 23f / 30f;
		wristGrabComboAttack[0].animationTypeLight = true;
		wristGrabComboAttack[0].animationToPerform = AttackAnimation.WristGrabChopLow;
		wristGrabComboAttack[0].nextAnimationCombo = wristGrabComboAttack[1];
		wristGrabComboAttack[0].secondaryAnimation = AttackAnimation.WristGrabCutHalf;
		if (liteVersion)
		{
			wristGrabComboAttack[1].animationComboLength = 1.4333334f;
			wristGrabComboAttack[1].animationEndLength = 1.5333333f;
			wristGrabComboAttack[1].animationTypeLight = true;
			wristGrabComboAttack[1].animationToPerform = AttackAnimation.WristGrabHeadOff;
			wristGrabComboAttack[1].nextAnimationCombo = null;
		}
		else
		{
			wristGrabComboAttack[1].animationComboLength = 1.0666666f;
			wristGrabComboAttack[1].animationEndLength = 5f / 6f;
			wristGrabComboAttack[1].animationTypeLight = true;
			wristGrabComboAttack[1].animationToPerform = AttackAnimation.WristGrabChopHi;
			wristGrabComboAttack[1].nextAnimationCombo = wristGrabComboAttack[2];
			wristGrabComboAttack[1].secondaryAnimation = AttackAnimation.WristGrabCutHalf;
			wristGrabComboAttack[2].animationComboLength = 1.4333334f;
			wristGrabComboAttack[2].animationEndLength = 1.5333333f;
			wristGrabComboAttack[2].animationTypeLight = true;
			wristGrabComboAttack[2].animationToPerform = AttackAnimation.WristGrabHeadOff;
			wristGrabComboAttack[2].nextAnimationCombo = null;
		}
		wristGrabComboSecondaryAttack[0].animationComboLength = 1f;
		wristGrabComboSecondaryAttack[0].animationEndLength = 23f / 30f;
		wristGrabComboSecondaryAttack[0].animationTypeLight = true;
		wristGrabComboSecondaryAttack[0].animationToPerform = AttackAnimation.WristGrabChopLow;
		wristGrabComboSecondaryAttack[0].nextAnimationCombo = wristGrabComboAttack[1];
		wristGrabComboSecondaryAttack[0].secondaryAnimation = AttackAnimation.WristGrabCutHalf;
		spearComboLightAttack[0].animationComboLength = 1f / 3f;
		spearComboLightAttack[0].animationEndLength = 1f / 3f;
		spearComboLightAttack[0].animationTypeLight = true;
		spearComboLightAttack[0].animationToPerform = AttackAnimation.SpearLightR;
		spearComboLightAttack[0].nextAnimationCombo = spearComboLightAttack[1];
		spearComboLightAttack[0].secondaryAnimation = AttackAnimation.WristGrabStart;
		spearComboLightAttack[1].animationComboLength = 1f / 3f;
		spearComboLightAttack[1].animationEndLength = 1f / 3f;
		spearComboLightAttack[1].animationTypeLight = true;
		spearComboLightAttack[1].animationToPerform = AttackAnimation.SpearLightL;
		spearComboLightAttack[1].nextAnimationCombo = spearComboLightAttack[2];
		spearComboLightAttack[1].secondaryAnimation = AttackAnimation.WristGrabStart;
		spearComboLightAttack[2].animationComboLength = 0.4f;
		spearComboLightAttack[2].animationEndLength = 0.6f;
		spearComboLightAttack[2].animationTypeLight = true;
		spearComboLightAttack[2].animationToPerform = AttackAnimation.SpearLightFront;
		spearComboLightAttack[2].nextAnimationCombo = spearComboLightAttack[3];
		spearComboLightAttack[2].secondaryAnimation = AttackAnimation.WristGrabStart;
		spearComboLightAttack[3].animationComboLength = 11f / 30f;
		spearComboLightAttack[3].animationEndLength = 1.3333334f;
		spearComboLightAttack[3].animationTypeLight = false;
		spearComboLightAttack[3].animationToPerform = AttackAnimation.SpearHeavySpin;
		spearComboLightAttack[3].nextAnimationCombo = null;
		spearComboHeavyAttack[0].animationComboLength = 2f / 15f;
		spearComboHeavyAttack[0].animationEndLength = 1f;
		spearComboHeavyAttack[0].animationTypeLight = false;
		spearComboHeavyAttack[0].animationToPerform = AttackAnimation.SpearHeavyR;
		spearComboHeavyAttack[0].nextAnimationCombo = spearComboHeavyAttack[1];
		spearComboHeavyAttack[0].secondaryAnimation = AttackAnimation.WristGrabStart;
		spearComboHeavyAttack[1].animationComboLength = 1.3333334f;
		spearComboHeavyAttack[1].animationEndLength = 0.2f;
		spearComboHeavyAttack[1].animationTypeLight = false;
		spearComboHeavyAttack[1].animationToPerform = AttackAnimation.SpearHeavyL;
		spearComboHeavyAttack[1].nextAnimationCombo = spearComboHeavyAttack[2];
		spearComboHeavyAttack[1].secondaryAnimation = AttackAnimation.WristGrabStart;
		spearComboHeavyAttack[2].animationComboLength = 17f / 30f;
		spearComboHeavyAttack[2].animationEndLength = 1.1666666f;
		spearComboHeavyAttack[2].animationTypeLight = false;
		spearComboHeavyAttack[2].animationToPerform = AttackAnimation.SpearHeavyFront;
		spearComboHeavyAttack[2].nextAnimationCombo = null;
		spearScissorCombo.animationComboLength = 1f / 6f;
		spearScissorCombo.animationEndLength = 0.5f;
		spearScissorCombo.animationTypeLight = true;
		spearScissorCombo.animationToPerform = AttackAnimation.WristScissor;
		spearScissorCombo.nextAnimationCombo = spearComboLightAttack[0];
		spearScissorCombo.secondaryAnimation = AttackAnimation.WristGrabStart;
		whipComboLightAttack[0].animationComboLength = 11f / 30f;
		whipComboLightAttack[0].animationEndLength = 0.3f;
		whipComboLightAttack[0].animationTypeLight = true;
		whipComboLightAttack[0].animationToPerform = AttackAnimation.WhipLightR;
		whipComboLightAttack[0].nextAnimationCombo = whipComboLightAttack[1];
		whipComboLightAttack[1].animationComboLength = 11f / 30f;
		whipComboLightAttack[1].animationEndLength = 0.3f;
		whipComboLightAttack[1].animationTypeLight = true;
		whipComboLightAttack[1].animationToPerform = AttackAnimation.WhipLightL;
		whipComboLightAttack[1].nextAnimationCombo = whipComboLightAttack[2];
		whipComboLightAttack[2].animationComboLength = 11f / 30f;
		whipComboLightAttack[2].animationEndLength = 0.3f;
		whipComboLightAttack[2].animationTypeLight = true;
		whipComboLightAttack[2].animationToPerform = AttackAnimation.WhipLightR;
		whipComboLightAttack[2].nextAnimationCombo = whipComboLightAttack[3];
		whipComboLightAttack[3].animationComboLength = 0.5f;
		whipComboLightAttack[3].animationEndLength = 8f / 15f;
		whipComboLightAttack[3].animationTypeLight = false;
		whipComboLightAttack[3].animationToPerform = AttackAnimation.WhipSpin;
		whipComboLightAttack[3].nextAnimationCombo = null;
		whipComboHeavyAttack[0].animationComboLength = 0.20000002f;
		whipComboHeavyAttack[0].animationEndLength = 1f / 15f;
		whipComboHeavyAttack[0].animationTypeLight = false;
		whipComboHeavyAttack[0].animationToPerform = AttackAnimation.WhipHeavyR;
		whipComboHeavyAttack[0].nextAnimationCombo = whipComboHeavyAttack[1];
		whipComboHeavyAttack[1].animationComboLength = 11f / 30f;
		whipComboHeavyAttack[1].animationEndLength = 8f / 15f;
		whipComboHeavyAttack[1].animationTypeLight = false;
		whipComboHeavyAttack[1].animationToPerform = AttackAnimation.WhipHeavyF;
		whipComboHeavyAttack[1].nextAnimationCombo = null;
		buttonResume.GetComponent<TextMesh>().text = Language.GetTxt("RESUME_GAME");
		optionsTextRestart.text = Language.GetTxt("RESTART");
		optionsTextOptions.text = Language.GetTxt("OPTIONS_WORD");
		optionsTextComboList.text = Language.GetTxt("COMBO_LIST_WORD");
		optionsTextObjectives.text = Language.GetTxt("OBJECTIVES_WORD");
		optionsTextQuitToMenu.text = Language.GetTxt("QUIT_TO_MENU_WORD");
		GUI_OptionsQuitConfirmText.text = Language.GetTxt("CURRENT_PROGRESS_WILL_BE_LOST") + "\n" + Language.GetTxt("DO_YOU_WANT_TO_CONTINUE");
		GUI_OptionsRestartConfirmText.text = Language.GetTxt("CURRENT_PROGRESS_WILL_BE_LOST") + "\n" + Language.GetTxt("DO_YOU_WANT_TO_CONTINUE");
		GUI_ObjectivesTitleText.text = Language.GetTxt("OBJECTIVES_WORD");
		((TextMesh)restartYesButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("YES");
		((TextMesh)restartNoButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("NO");
		((TextMesh)quitYesButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("YES");
		((TextMesh)quitNoButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("NO");
		((TextMesh)failMissionButtonRetry.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("RETRY");
		((TextMesh)failMissionButtonQuit.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("QUIT");
		if (Language.CurrentLang == SystemLanguage.Italian)
		{
			toolTipBackgroundMissionEnd.localScale = new Vector3(toolTipBackgroundMissionEnd.localScale.x, toolTipBackgroundMissionEnd.localScale.y * 1.22f, toolTipBackgroundMissionEnd.localScale.z);
		}
		PlatformDependent.HideMouseCursor(mouseCursor);
		PlatformDependent.DestroyUselessForPC(LeftStick.gameObject);
		PlatformDependent.DestroyUselessForPC(AttackStick.gameObject);
		PlatformDependent.DestroyUselessForPC(BlockStick.gameObject);
		PlatformDependent.DestroyUselessForPC(LeftControlPad.gameObject);
		PlatformDependent.DestroyUselessForPC(GUI_PauseButton);
	}

	private void LoadResourcesTextures()
	{
		TextureCharactersNormal = materialNormal.GetTexture("_MainTex");
		TextureBerserkersNormal = materialBerserkers.GetTexture("_MainTex");
		TextureCharactersThermal = materialThermalCloaked.GetTexture("_MainTex");
		if (!TextureCharactersNormal || !TextureCharactersThermal || !TextureBerserkersNormal)
		{
			Debug.LogError("texture resource loading failed");
		}
	}

	public void WaterEnter()
	{
		waterTouching = true;
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundWaterSplash);
		}
		if (cloakModeOn)
		{
			StartCoroutine(CloakFail());
		}
	}

	public void WaterExit()
	{
		waterTouching = false;
	}

	private void UpdateWeaponSlotTextures(bool showWeaponSlots)
	{
		if (currentWeapon == WeaponType.Melee)
		{
			LaserLineRenderer.gameObject.active = false;
			currentWeaponType = weaponA;
		}
		else if (currentWeapon == WeaponType.Ranged)
		{
			LaserLineRenderer.gameObject.active = true;
			currentWeaponType = weaponB;
		}
		WhipTrailRenderer.enabled = false;
		switch (currentWeaponType)
		{
		case 1:
		case 2:
			WristBladeLMesh.gameObject.active = true;
			WristBladeRMesh.gameObject.active = true;
			whipComponent.HideWhip();
			PlasmaCannonMesh.gameObject.active = false;
			SpearMesh.gameObject.active = false;
			break;
		case 3:
			WristBladeLMesh.gameObject.active = false;
			WristBladeRMesh.gameObject.active = false;
			PlasmaCannonMesh.gameObject.active = false;
			SpearMesh.gameObject.active = true;
			whipComponent.HideWhip();
			break;
		case 4:
			WristBladeLMesh.gameObject.active = false;
			WristBladeRMesh.gameObject.active = false;
			PlasmaCannonMesh.gameObject.active = false;
			SpearMesh.gameObject.active = false;
			ActivateWhipWeapon();
			break;
		case 5:
		case 6:
			WristBladeLMesh.gameObject.active = false;
			WristBladeRMesh.gameObject.active = false;
			PlasmaCannonMesh.gameObject.active = true;
			SpearMesh.gameObject.active = false;
			whipComponent.HideWhip();
			break;
		case 7:
		case 8:
		case 9:
			WristBladeLMesh.gameObject.active = false;
			WristBladeRMesh.gameObject.active = false;
			PlasmaCannonMesh.gameObject.active = false;
			SpearMesh.gameObject.active = false;
			whipComponent.HideWhip();
			break;
		}
		if (showWeaponSlots)
		{
			switch (currentWeaponType)
			{
			case 1:
			case 2:
				GUI_WeaponWristBlades_Active.active = true;
				GUI_WeaponCombiStick_Active.active = false;
				GUI_WeaponWhip_Active.active = false;
				GUI_WeaponWristBlades_Inactive.active = false;
				GUI_WeaponCombiStick_Inactive.active = false;
				GUI_WeaponWhip_Inactive.active = false;
				GUI_WeaponPlasmaGun_Active.active = false;
				GUI_WeaponDisc_Active.active = false;
				GUI_WeaponNetGun_Active.active = false;
				switch (weaponB)
				{
				case 5:
				case 6:
					GUI_WeaponPlasmaGun_Inactive.active = true;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				case 7:
				case 8:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = true;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				case 9:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = true;
					break;
				default:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				}
				break;
			case 3:
				GUI_WeaponWristBlades_Active.active = false;
				GUI_WeaponCombiStick_Active.active = true;
				GUI_WeaponWhip_Active.active = false;
				GUI_WeaponWristBlades_Inactive.active = false;
				GUI_WeaponCombiStick_Inactive.active = false;
				GUI_WeaponWhip_Inactive.active = false;
				GUI_WeaponPlasmaGun_Active.active = false;
				GUI_WeaponDisc_Active.active = false;
				GUI_WeaponNetGun_Active.active = false;
				switch (weaponB)
				{
				case 5:
				case 6:
					GUI_WeaponPlasmaGun_Inactive.active = true;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				case 7:
				case 8:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = true;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				case 9:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = true;
					break;
				default:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				}
				break;
			case 4:
				GUI_WeaponWristBlades_Active.active = false;
				GUI_WeaponCombiStick_Active.active = false;
				GUI_WeaponWhip_Active.active = true;
				GUI_WeaponWristBlades_Inactive.active = false;
				GUI_WeaponCombiStick_Inactive.active = false;
				GUI_WeaponWhip_Inactive.active = false;
				GUI_WeaponPlasmaGun_Active.active = false;
				GUI_WeaponDisc_Active.active = false;
				GUI_WeaponNetGun_Active.active = false;
				switch (weaponB)
				{
				case 5:
				case 6:
					GUI_WeaponPlasmaGun_Inactive.active = true;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				case 7:
				case 8:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = true;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				case 9:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = true;
					break;
				default:
					GUI_WeaponPlasmaGun_Inactive.active = false;
					GUI_WeaponDisc_Inactive.active = false;
					GUI_WeaponNetGun_Inactive.active = false;
					break;
				}
				break;
			case 5:
			case 6:
				GUI_WeaponPlasmaGun_Active.active = true;
				GUI_WeaponDisc_Active.active = false;
				GUI_WeaponNetGun_Active.active = false;
				GUI_WeaponWristBlades_Active.active = false;
				GUI_WeaponCombiStick_Active.active = false;
				GUI_WeaponWhip_Active.active = false;
				GUI_WeaponPlasmaGun_Inactive.active = false;
				GUI_WeaponDisc_Inactive.active = false;
				GUI_WeaponNetGun_Inactive.active = false;
				switch (weaponA)
				{
				case 1:
				case 2:
					GUI_WeaponWristBlades_Inactive.active = true;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				case 3:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = true;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				case 4:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = true;
					break;
				default:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				}
				break;
			case 7:
			case 8:
				GUI_WeaponPlasmaGun_Active.active = false;
				GUI_WeaponDisc_Active.active = true;
				GUI_WeaponNetGun_Active.active = false;
				GUI_WeaponWristBlades_Active.active = false;
				GUI_WeaponCombiStick_Active.active = false;
				GUI_WeaponWhip_Active.active = false;
				GUI_WeaponPlasmaGun_Inactive.active = false;
				GUI_WeaponDisc_Inactive.active = false;
				GUI_WeaponNetGun_Inactive.active = false;
				switch (weaponA)
				{
				case 1:
				case 2:
					GUI_WeaponWristBlades_Inactive.active = true;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				case 3:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = true;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				case 4:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = true;
					break;
				default:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				}
				break;
			case 9:
				GUI_WeaponPlasmaGun_Active.active = false;
				GUI_WeaponDisc_Active.active = false;
				GUI_WeaponNetGun_Active.active = true;
				GUI_WeaponWristBlades_Active.active = false;
				GUI_WeaponCombiStick_Active.active = false;
				GUI_WeaponWhip_Active.active = false;
				GUI_WeaponPlasmaGun_Inactive.active = false;
				GUI_WeaponDisc_Inactive.active = false;
				GUI_WeaponNetGun_Inactive.active = false;
				switch (weaponA)
				{
				case 1:
				case 2:
					GUI_WeaponWristBlades_Inactive.active = true;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				case 3:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = true;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				case 4:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = true;
					break;
				default:
					GUI_WeaponWristBlades_Inactive.active = false;
					GUI_WeaponCombiStick_Inactive.active = false;
					GUI_WeaponWhip_Inactive.active = false;
					break;
				}
				break;
			}
		}
		else
		{
			GUI_WeaponWristBlades_Active.active = false;
			GUI_WeaponWristBlades_Inactive.active = false;
			GUI_WeaponCombiStick_Active.active = false;
			GUI_WeaponCombiStick_Inactive.active = false;
			GUI_WeaponWhip_Active.active = false;
			GUI_WeaponWhip_Inactive.active = false;
			GUI_WeaponPlasmaGun_Active.active = false;
			GUI_WeaponPlasmaGun_Inactive.active = false;
			GUI_WeaponDisc_Active.active = false;
			GUI_WeaponDisc_Inactive.active = false;
			GUI_WeaponNetGun_Active.active = false;
			GUI_WeaponNetGun_Inactive.active = false;
		}
	}

	private IEnumerator CloakFail()
	{
		if (cloakModeOn)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundCloakFail);
			}
			yield return new WaitForSeconds(cloakFailSeconds);
			if (cloakModeOn)
			{
				SwitchCloakMode();
			}
		}
	}

	public void InitEnvironment(GameObject environmentNormalVision, GameObject environmentThermalVision)
	{
		EnvironmentNormalVision = environmentNormalVision;
		EnvironmentThermalVision = environmentThermalVision;
		if (!EnvironmentNormalVision)
		{
			Debug.LogError("EnvironmentNormalVision reference not found");
		}
		if (!EnvironmentThermalVision)
		{
			Debug.LogError("EnvironmentThermalVision reference not found");
		}
		if (thermalVisionMode)
		{
			EnvironmentThermalVision.SetActiveRecursively(true);
			EnvironmentNormalVision.SetActiveRecursively(false);
		}
		else
		{
			EnvironmentThermalVision.SetActiveRecursively(false);
			EnvironmentNormalVision.SetActiveRecursively(true);
		}
		if ((bool)loadingScreen)
		{
			loadingScreen.gameObject.SetActiveRecursively(false);
		}
		if ((bool)loadingScreenLeft)
		{
			loadingScreenLeft.SetActiveRecursively(false);
		}
		if (!paused)
		{
			cameraPause.enabled = false;
		}
		if (currentMission == 30)
		{
			Application.LoadLevelAdditive("ClanLeaderTiedUp_iPad");
			HealthBarGameObjectClanLeader.SetActiveRecursively(true);
		}
	}

	private void Start()
	{
		vignetting = cameraMain.GetComponent<Vignetting>();
		glowEffect = cameraMain.GetComponent<GlowEffect>();
		noiseEffect = cameraMain.GetComponent<NoiseEffect>();
		xForm = base.transform;
		targetTriangleTransform = triangleTarget.transform;
		StopCoroutine("EnergyIncrease");
		StartCoroutine("EnergyIncrease", 1f);
		if (liteVersion)
		{
			currentMission = 0;
			HealthBarGameObject.active = false;
			symbol1[9].active = false;
			symbol2[9].active = false;
			symbol3[9].active = false;
			symbol4[9].active = false;
			GUI_Cloak.SetActiveRecursively(false);
			GUI_Thermal.SetActiveRecursively(false);
			PlatformDependent.SetActiveIphoneGUI(AttackStick, false);
			PlatformDependent.SetActiveIphoneGUI(BlockStick, false);
			UpdateWeaponSlotTextures(false);
		}
		else
		{
			maxLevelUnlocked = EncryptedPlayerPrefs.GetInt("PR_LastMissionUnlocked_S" + currentSlot);
			if (currentMission == 34)
			{
				maxLevelUnlocked = currentMission;
			}
			if (debugCurrentMissionAutoStart > 20)
			{
				maxLevelUnlocked = currentMission;
			}
			if (maxLevelUnlocked > 1)
			{
				PlatformDependent.SetActiveIphoneGUI(AttackStick, true);
				ability.lightAttack = true;
				HealthBarGameObject.active = true;
			}
			else
			{
				HealthBarGameObject.active = false;
			}
			if (maxLevelUnlocked >= 2)
			{
				ability.heavyAttack = true;
			}
			if (maxLevelUnlocked >= 3)
			{
				ability.spinAttack = true;
			}
			if (maxLevelUnlocked >= 4)
			{
				ability.blockUnlocked = true;
			}
			if (maxLevelUnlocked >= 5)
			{
				ability.impale = true;
			}
			if (maxLevelUnlocked >= 6)
			{
				ability.trophyKill = true;
			}
			if (maxLevelUnlocked >= 8)
			{
				ability.cloakUnlocked = true;
				GUI_Cloak.active = true;
			}
			if (weaponLevelPlasmaGun > 0)
			{
				ability.plasmaGunUnlocked = true;
				UpdateWeaponSlotTextures(true);
			}
			if (maxLevelUnlocked >= 10)
			{
				ability.thermalVision = true;
			}
			if (ability.cloakUnlocked)
			{
				symbol1[9].active = true;
				symbol2[9].active = true;
				symbol3[9].active = true;
				symbol4[9].active = true;
			}
			else
			{
				symbol1[9].active = false;
				symbol2[9].active = false;
				symbol3[9].active = false;
				symbol4[9].active = false;
			}
			if (weaponLevelDisc > 0)
			{
				ability.diskUnlocked = true;
			}
			if (weaponLevelNetGun > 0)
			{
				ability.netGunUnlocked = true;
			}
			if (weaponLevelSpear > 0)
			{
				ability.spearUnlocked = true;
			}
			if (weaponLevelWhip > 0)
			{
				ability.whipUnlocked = true;
			}
			if (ability.cloakUnlocked)
			{
				GUI_Cloak.SetActiveRecursively(true);
			}
			else
			{
				GUI_Cloak.SetActiveRecursively(false);
			}
			if (ability.thermalVision)
			{
				GUI_Thermal.SetActiveRecursively(true);
			}
			else
			{
				GUI_Thermal.SetActiveRecursively(false);
			}
			if (ability.plasmaGunUnlocked)
			{
				UpdateWeaponSlotTextures(true);
			}
			else
			{
				UpdateWeaponSlotTextures(false);
			}
			if (ability.lightAttack)
			{
				PlatformDependent.SetActiveIphoneGUI(AttackStick, true);
			}
			else
			{
				PlatformDependent.SetActiveIphoneGUI(AttackStick, false);
			}
			if (ability.blockUnlocked)
			{
				PlatformDependent.SetActiveIphoneGUI(BlockStick, true);
			}
			else
			{
				PlatformDependent.SetActiveIphoneGUI(BlockStick, false);
			}
		}
		StartCoroutine(PredatorClicks());
		failMissionTextMessage.text = Language.GetTxt("MISSION_FAILED");
		if (currentMission == 8 || currentMission == 15 || currentMission == 21 || currentMission == 29)
		{
			TextMesh textMesh = failMissionTextMessage;
			textMesh.text = textMesh.text + "\n" + Language.GetTxt("YOU_WERE_DETECTED");
		}
		SetupCombosText();
		StartCoroutine("DisplayTips");
		PlatformDependent.StartBlinkDisappear(this);
		if (currentMission == 34)
		{
			StartCoroutine(FadeToBlack());
		}
		if (PlatformDependent.IsPC())
		{
			inputDevice = InputDevice.PC;
			if (hud != null)
			{
				if (hud.LeftStick != null) hud.LeftStick.gameObject.SetActiveRecursively(false);
				if (hud.AttackStick != null) hud.AttackStick.gameObject.SetActiveRecursively(false);
				if (hud.BlockStick != null) hud.BlockStick.gameObject.SetActiveRecursively(false);
				if (hud.LeftControlPad != null)
				{
					Renderer leftControlPadRenderer = hud.LeftControlPad.GetComponent<Renderer>();
					if (leftControlPadRenderer != null) leftControlPadRenderer.enabled = false;
				}
				if (hud.GUI_Thermal != null) hud.GUI_Thermal.SetActiveRecursively(false);
				if (hud.GUI_Cloak != null) hud.GUI_Cloak.SetActiveRecursively(false);
				if (hud.GUI_WeaponWristBlades_Active != null) hud.GUI_WeaponWristBlades_Active.SetActiveRecursively(false);
				if (hud.GUI_WeaponWristBlades_Inactive != null) hud.GUI_WeaponWristBlades_Inactive.SetActiveRecursively(false);
				if (hud.GUI_WeaponCombiStick_Active != null) hud.GUI_WeaponCombiStick_Active.SetActiveRecursively(false);
				if (hud.GUI_WeaponCombiStick_Inactive != null) hud.GUI_WeaponCombiStick_Inactive.SetActiveRecursively(false);
				if (hud.GUI_WeaponWhip_Active != null) hud.GUI_WeaponWhip_Active.SetActiveRecursively(false);
				if (hud.GUI_WeaponWhip_Inactive != null) hud.GUI_WeaponWhip_Inactive.SetActiveRecursively(false);
				if (hud.GUI_WeaponPlasmaGun_Active != null) hud.GUI_WeaponPlasmaGun_Active.SetActiveRecursively(false);
				if (hud.GUI_WeaponPlasmaGun_Inactive != null) hud.GUI_WeaponPlasmaGun_Inactive.SetActiveRecursively(false);
				if (hud.GUI_WeaponDisc_Active != null) hud.GUI_WeaponDisc_Active.SetActiveRecursively(false);
				if (hud.GUI_WeaponDisc_Inactive != null) hud.GUI_WeaponDisc_Inactive.SetActiveRecursively(false);
				if (hud.GUI_WeaponNetGun_Active != null) hud.GUI_WeaponNetGun_Active.SetActiveRecursively(false);
				if (hud.GUI_WeaponNetGun_Inactive != null) hud.GUI_WeaponNetGun_Inactive.SetActiveRecursively(false);
				if (hud.buttonPause != null) hud.buttonPause.pollForKey = KeyCode.Escape;
				if (hud.tipTextures != null)
				{
					for (int tipIdx = 0; tipIdx < hud.tipTextures.Length; tipIdx++)
					{
						Transform tipNode = hud.tipTextures[tipIdx];
						if (tipNode == null) continue;
						Transform ancestor = tipNode.parent;
						while (ancestor != null)
						{
							if (!ancestor.gameObject.activeSelf)
							{
								ancestor.gameObject.SetActive(true);
								Renderer ancestorRenderer = ancestor.GetComponent<Renderer>();
								if (ancestorRenderer != null) ancestorRenderer.enabled = false;
							}
							ancestor = ancestor.parent;
						}
					}
				}
			}
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void SetupCombosText()
	{
		GUI_CombosText.text = string.Empty;
		if (ability.spinAttack)
		{
			GUI_CombosText.text += Language.GetTxt("WRIST_BLADES");
			GUI_CombosText.text += "\n";
			TextMesh gUI_CombosText = GUI_CombosText;
			gUI_CombosText.text = gUI_CombosText.text + "\n" + Language.GetTxt("COMBO_WRIST_SPIN");
		}
		if (ability.impale)
		{
			TextMesh gUI_CombosText2 = GUI_CombosText;
			gUI_CombosText2.text = gUI_CombosText2.text + "\n" + Language.GetTxt("COMBO_WRIST_IMPALE");
			TextMesh gUI_CombosText3 = GUI_CombosText;
			gUI_CombosText3.text = gUI_CombosText3.text + "\n" + Language.GetTxt("COMBO_WRIST_SPLICE");
		}
		if (ability.trophyKill)
		{
			if (liteVersion)
			{
				TextMesh gUI_CombosText4 = GUI_CombosText;
				gUI_CombosText4.text = gUI_CombosText4.text + "\n" + Language.GetTxt("COMBO_WRIST_TROPHY_KILL_LITE");
			}
			else
			{
				TextMesh gUI_CombosText5 = GUI_CombosText;
				gUI_CombosText5.text = gUI_CombosText5.text + "\n" + Language.GetTxt("COMBO_WRIST_TROPHY_KILL");
			}
		}
		if (ability.spearUnlocked)
		{
			GUI_CombosText.text += "\n";
			TextMesh gUI_CombosText6 = GUI_CombosText;
			gUI_CombosText6.text = gUI_CombosText6.text + "\n" + Language.GetTxt("SPEAR");
			GUI_CombosText.text += "\n";
			TextMesh gUI_CombosText7 = GUI_CombosText;
			gUI_CombosText7.text = gUI_CombosText7.text + "\n" + Language.GetTxt("COMBO_SPEAR_HEAVY_ATTACK");
			TextMesh gUI_CombosText8 = GUI_CombosText;
			gUI_CombosText8.text = gUI_CombosText8.text + "\n" + Language.GetTxt("COMBO_SPEAR_SPIN");
		}
		if (ability.whipUnlocked)
		{
			GUI_CombosText.text += "\n";
			TextMesh gUI_CombosText9 = GUI_CombosText;
			gUI_CombosText9.text = gUI_CombosText9.text + "\n" + Language.GetTxt("WHIP");
			GUI_CombosText.text += "\n";
			TextMesh gUI_CombosText10 = GUI_CombosText;
			gUI_CombosText10.text = gUI_CombosText10.text + "\n" + Language.GetTxt("COMBO_WHIP_VERTICAL_SLICE");
			TextMesh gUI_CombosText11 = GUI_CombosText;
			gUI_CombosText11.text = gUI_CombosText11.text + "\n" + Language.GetTxt("COMBO_WHIP_GRAPPLE");
			TextMesh gUI_CombosText12 = GUI_CombosText;
			gUI_CombosText12.text = gUI_CombosText12.text + "\n" + Language.GetTxt("COMBO_WHIP_THE_CHOPPER");
		}
		GUI_CombosText.text = PlatformDependent.TranslateKeybinds(GUI_CombosText.text);
	}

	private IEnumerator FadeToBlack()
	{
		yield return new WaitForSeconds(fadeToBlackTime);
		if (currentMission != 34)
		{
			yield break;
		}
		if (!liteVersion)
		{
			//CrystalUnityBasic.Instance.PostAchievement("419918554", true, "Clan Leader", false);
		}
		float fadeTime = 4f;
		float timer = fadeTime;
		GUI_FadeToBlack.color = new Color(0f, 0f, 0f, 0f);
		GUI_FadeToBlack.gameObject.active = true;
		while (timer > 0f)
		{
			GUI_FadeToBlack.color = Color.Lerp(new Color(0f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 0f), timer / fadeTime);
			timer -= Time.deltaTime;
			yield return null;
		}
		if (currentMission == 34)
		{
			Time.timeScale = 1f;
			RestoreMaterialsColors(true);
			int tutorialSlot = EncryptedPlayerPrefs.GetInt("PR_CurrentSlot");
			if (tutorialSlot > 0)
			{
				int existingUnlocked = EncryptedPlayerPrefs.GetInt("PR_LastMissionUnlocked_S" + tutorialSlot);
				EncryptedPlayerPrefs.SetInt("PR_LastMissionUnlocked_S" + tutorialSlot, Mathf.Max(1, existingUnlocked));
			}
			if (liteVersion)
			{
				PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
			}
			else
			{
				PlatformDependent.LoadLevelWithLoadingScreen("MissionSelect3D");
			}
		}
	}

	private void SetRestartLevelPlayerPrefs()
	{
		EncryptedPlayerPrefs.SetFloat("PR_TimeLevel", 0.001f);
	}

	private void BlockButtonDown()
	{
		if (currentWeapon != WeaponType.Melee)
		{
			return;
		}
		blockButtonDown = true;
		StartCoroutine(ComboSecondaryAdd());
		if (meleeLightAttacking || chargingHeavyAttack || dashJumping || gettingHurt)
		{
			return;
		}
		if (grabbingEnemy)
		{
			StartCoroutine("ComboAttackPerform", wristGrabComboAttack[0]);
		}
		else if (!moved)
		{
			if (currentWeaponType == 3)
			{
				anim.CrossFade("spear_block", 0.1f);
			}
			else
			{
				anim.CrossFade("block", 0.1f);
			}
			blocking = true;
		}
		else
		{
			StartCoroutine(DashJump());
		}
	}

	private void AttackButtonDown()
	{
		attackButtonDown = true;
		if (currentWeapon == WeaponType.Melee)
		{
			StartCoroutine(ComboLightAdd());
			if (chargingHeavyAttack || dashJumping)
			{
				return;
			}
			if (grabbingEnemy)
			{
				if (!meleeLightAttacking && !gettingHurt)
				{
					StartCoroutine("ComboAttackPerform", wristGrabComboAttack[0]);
				}
			}
			else
			{
				StartCoroutine("ComboHeavyCharge");
			}
		}
		else
		{
			WeaponStartShooting();
		}
	}

	private void UpdateTouch(ref bool pressedAttackButtonDown, ref bool pressedAttackButtonUp, ref bool pressedBlockButtonDown, ref bool pressedBlockButtonUp, ref bool pressedCloak, ref bool pressedMeleeRangeChange, ref bool pressedMeleeWeaponChange, ref bool pressedThermal)
	{
		for (int i = 0; i < InputGUI.touches.Count; i++)
		{
			TouchGUI touch = InputGUI.GetTouch(i);
			touchPosition = touch.position;
			lDiff = touchPosition - leftStickCenter;
			leftStickDown = lDiff.sqrMagnitude < (float)(touchZoneRadius * touchZoneRadius);
			switch (touch.phase)
			{
			case TouchPhase.Began:
				if (tipActiveAndWaitingTap)
				{
					if (tipTexturesBounds[2].Contains(touchPosition))
					{
						texture2IsTapped = true;
					}
					if (tipTexturesBounds[3].Contains(touchPosition))
					{
						texture3IsTapped = true;
					}
					if (tipTexturesBounds[4].Contains(touchPosition))
					{
						texture4IsTapped = true;
					}
				}
				if (!isControllable)
				{
					break;
				}
				if (weaponABounds.Contains(touchPosition))
				{
					pressedMeleeWeaponChange = true;
				}
				else if (weaponBBounds.Contains(touchPosition))
				{
					pressedMeleeRangeChange = true;
				}
				else if (visionModeBounds.Contains(touchPosition))
				{
					pressedThermal = true;
				}
				else if (cloakModeBounds.Contains(touchPosition))
				{
					pressedCloak = true;
				}
				else if (buttonAttack.Contains(touchPosition))
				{
					pressedAttackButtonDown = true;
				}
				else if (buttonBlock.Contains(touchPosition))
				{
					pressedBlockButtonDown = true;
				}
				else if (PlatformDependent.tablet && iPadLeftScreenBounds.Contains(touchPosition))
				{
					leftStickDown = true;
					LeftStick.position = cameraMain.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 1f));
					LeftControlPad.transform.position = cameraMain.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 1f));
					LeftStickCenterLocalPosition = LeftStick.localPosition;
					leftStickCenter = cameraMain.WorldToScreenPoint(LeftStick.transform.position);
					LeftStick.GetComponent<Renderer>().enabled = true;
					LeftControlPad.GetComponent<Renderer>().enabled = true;
					lDiff3 = Vector3.zero;
					if (blockButtonDown && !meleeLightAttacking && !chargingHeavyAttack && !dashJumping && !grabbingEnemy)
					{
						StartCoroutine(DashJump());
					}
					else
					{
						moved = false;
					}
					leftStickTouchId = touch.fingerId;
				}
				else if (leftStickDown)
				{
					lDiff3 = new Vector3(touchPosition.x - leftStickCenter.x, 0f, touchPosition.y - leftStickCenter.y);
					LeftStick.localPosition = new Vector3(LeftStickCenterLocalPosition.x + (touchPosition.x - leftStickCenter.x) * ControlPadRadius, LeftStickCenterLocalPosition.y + (touchPosition.y - leftStickCenter.y) * ControlPadRadius, LeftStick.localPosition.z);
					if (blockButtonDown && !meleeLightAttacking && !chargingHeavyAttack && !dashJumping && !grabbingEnemy)
					{
						StartCoroutine(DashJump());
					}
					else if (lDiff3 != Vector3.zero)
					{
						moved = true;
					}
					else
					{
						moved = false;
					}
					leftStickTouchId = touch.fingerId;
				}
				break;
			case TouchPhase.Moved:
				if (!isControllable || leftStickTouchId != touch.fingerId)
				{
					break;
				}
				lDiff3 = new Vector3(touchPosition.x - leftStickCenter.x, 0f, touchPosition.y - leftStickCenter.y);
				if (leftStickDown)
				{
					if (PlatformDependent.tablet)
					{
						LeftStick.position = cameraMain.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 1f));
					}
					else
					{
						LeftStick.localPosition = new Vector3(LeftStickCenterLocalPosition.x + (touchPosition.x - leftStickCenter.x) * ControlPadRadius, LeftStickCenterLocalPosition.y + (touchPosition.y - leftStickCenter.y) * ControlPadRadius, LeftStick.localPosition.z);
					}
				}
				else
				{
					lDiff3.Normalize();
					LeftStick.localPosition = new Vector3(LeftStickCenterLocalPosition.x + lDiff3.x * (float)touchZoneRadius * ControlPadRadius, LeftStickCenterLocalPosition.y + lDiff3.z * (float)touchZoneRadius * ControlPadRadius, LeftStick.localPosition.z);
				}
				if (lDiff3.sqrMagnitude > 0.001f)
				{
					moved = true;
				}
				else
				{
					moved = false;
				}
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				if (leftStickTouchId == touch.fingerId)
				{
					leftStickDown = false;
					if (PlatformDependent.tablet)
					{
						LeftStick.GetComponent<Renderer>().enabled = false;
						LeftControlPad.GetComponent<Renderer>().enabled = false;
					}
					else
					{
						LeftStick.localPosition = LeftStickCenterLocalPosition;
					}
					moved = false;
					leftStickTouchId = -1;
				}
				else if (buttonAttack.Contains(touchPosition))
				{
					pressedAttackButtonUp = true;
				}
				else if (buttonBlock.Contains(touchPosition))
				{
					pressedBlockButtonUp = true;
				}
				break;
			}
		}
	}

	private void UpdateXperia(ref bool pressedAttackButtonDown, ref bool pressedAttackButtonUp, ref bool pressedBlockButtonDown, ref bool pressedBlockButtonUp, ref bool pressedCloak, ref bool pressedMeleeRangeChange, ref bool pressedMeleeWeaponChange, ref bool pressedThermal)
	{
		if (inputDevice == InputDevice.XperiaPlay)
		{
#if UNITY_ANDROID
			if (AndroidInput.touchCountSecondary > 0)
			{
				androidTouch = AndroidInput.GetSecondaryTouch(0);
				int num = AndroidInput.secondaryTouchHeight / 2;
				Vector3 vector = new Vector3(num, 0f, num);
				lDiff3 = new Vector3(androidTouch.position.x, 0f, androidTouch.position.y) - vector;
				lDiff3.z = 0f - lDiff3.z;
				if (lDiff3.sqrMagnitude > (float)(num * num))
				{
					lDiff3 = Vector3.zero;
				}
			}
#else
		    lDiff3 = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
#endif
			moved = lDiff3.sqrMagnitude > 0f;
			pressedCloak = Input.GetKeyDown(KeyCode.Escape);
		}
		pressedThermal = Input.GetKeyDown("joystick button 2");
		pressedMeleeWeaponChange = Input.GetKeyDown(KeyCode.LeftShift);
		pressedMeleeRangeChange = Input.GetKeyDown(KeyCode.RightShift);
		pressedAttackButtonDown = Input.GetKeyDown("joystick button 1");
		pressedAttackButtonUp = Input.GetKeyUp("joystick button 1");
		pressedBlockButtonDown = Input.GetKeyDown("joystick button 0");
		pressedBlockButtonUp = Input.GetKeyUp("joystick button 0");
		texture4IsTapped = pressedBlockButtonUp;
		texture3IsTapped = pressedBlockButtonUp;
		texture2IsTapped = pressedBlockButtonUp;
	}

	private void UpdatePC(ref bool pressedAttackButtonDown, ref bool pressedAttackButtonUp, ref bool pressedBlockButtonDown, ref bool pressedBlockButtonUp, ref bool pressedCloak, ref bool pressedMeleeRangeChange, ref bool pressedMeleeWeaponChange, ref bool pressedThermal)
	{
		lDiff3 = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		if (blocking)
		{
			lDiff3 = Vector3.zero;
		}
		moved = lDiff3.sqrMagnitude > 0f;
		pressedMeleeWeaponChange = Input.GetKeyDown(KeyCode.Q);
		pressedMeleeRangeChange = Input.GetKeyDown(KeyCode.E);
		pressedThermal = Input.GetKeyDown(KeyCode.F);
		pressedCloak = Input.GetKeyDown(KeyCode.C);
		pressedAttackButtonDown = Input.GetMouseButtonDown(0);
		pressedAttackButtonUp = Input.GetMouseButtonUp(0);
		pressedBlockButtonDown = Input.GetMouseButtonDown(1);
		pressedBlockButtonUp = Input.GetMouseButtonUp(1);
		texture4IsTapped = pressedBlockButtonUp;
		texture3IsTapped = pressedBlockButtonUp;
		texture2IsTapped = pressedBlockButtonUp;
	}

	private void Update()
	{
		texture4IsTapped = false;
		texture3IsTapped = false;
		texture2IsTapped = false;
		PlatformDependent.UpdateMouseCursorGUITexture(paused, mouseCursor);
		PlatformDependent.SetScreenOrientation(true);
		if (paused)
		{
			for (int i = 0; i < InputGUI.touches.Count; i++)
			{
				TouchGUI touch = InputGUI.GetTouch(i);
				touchPosition = touch.position;
				moved = false;
				if (touch.phase != 0)
				{
					continue;
				}
				hitCollider = InputGUI.GetHitCollider(touchPosition, cameraPause);
				if (!(hitCollider != null))
				{
					continue;
				}
				if (missionFailed)
				{
					if (failMissionButtonQuit == hitCollider)
					{
						Time.timeScale = 1f;
						RestoreMaterialsColors(true);
						if (liteVersion)
						{
							PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
						}
						else
						{
							PlatformDependent.LoadLevelWithLoadingScreen("MissionSelect3D");
						}
					}
					else if (failMissionButtonRetry == hitCollider)
					{
						Time.timeScale = 1f;
						SetRestartLevelPlayerPrefs();
						ShowLoadingScreen();
						RestoreMaterialsColors(true);
						LogRetryPress();
						Application.LoadLevel(Application.loadedLevel);
					}
				}
				else if (missionSuccess)
				{
					if (successMissionButtonOk == hitCollider)
					{
						Time.timeScale = 1f;
						ShowLoadingScreen();
						RestoreMaterialsColors(true);
						if (currentMission == 31)
						{
							PlatformDependent.LoadLevelWithLoadingScreen("StoryEnd_iPad");
						}
						else
						{
							PlatformDependent.LoadLevelWithLoadingScreen("MissionSelect3D");
						}
					}
				}
				else if (pauseMenu)
				{
					if (optionsTextRestart.GetComponent<Collider>() == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						OptionsMenuGroup.SetActiveRecursively(false);
						ObjectivesMenuGroup.SetActiveRecursively(false);
						CombosMenuGroup.SetActiveRecursively(false);
						RestartMenuGroup.SetActiveRecursively(true);
						QuitMenuGroup.SetActiveRecursively(false);
						optionsMenu = false;
						restartMenu = true;
						quitMenu = false;
					}
					else if (optionsTextOptions.GetComponent<Collider>() == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						OptionsMenuGroup.SetActiveRecursively(true);
						ObjectivesMenuGroup.SetActiveRecursively(false);
						CombosMenuGroup.SetActiveRecursively(false);
						RestartMenuGroup.SetActiveRecursively(false);
						QuitMenuGroup.SetActiveRecursively(false);
						optionsMenu = true;
						restartMenu = false;
						quitMenu = false;
					}
					else if (optionsTextComboList.GetComponent<Collider>() == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						OptionsMenuGroup.SetActiveRecursively(false);
						ObjectivesMenuGroup.SetActiveRecursively(false);
						CombosMenuGroup.SetActiveRecursively(true);
						RestartMenuGroup.SetActiveRecursively(false);
						QuitMenuGroup.SetActiveRecursively(false);
						optionsMenu = false;
						restartMenu = false;
						quitMenu = false;
					}
					else if (optionsTextObjectives.GetComponent<Collider>() == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						OptionsMenuGroup.SetActiveRecursively(false);
						ObjectivesMenuGroup.SetActiveRecursively(true);
						CombosMenuGroup.SetActiveRecursively(false);
						RestartMenuGroup.SetActiveRecursively(false);
						QuitMenuGroup.SetActiveRecursively(false);
						optionsMenu = false;
						restartMenu = false;
						quitMenu = false;
					}
					else if (optionsTextQuitToMenu.GetComponent<Collider>() == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						OptionsMenuGroup.SetActiveRecursively(false);
						ObjectivesMenuGroup.SetActiveRecursively(false);
						CombosMenuGroup.SetActiveRecursively(false);
						RestartMenuGroup.SetActiveRecursively(false);
						QuitMenuGroup.SetActiveRecursively(true);
						optionsMenu = false;
						restartMenu = false;
						quitMenu = true;
					}
				}
				else if (statsMenu)
				{
					if (!finishedShowingStats)
					{
						if (!skipStats)
						{
							skipStats = true;
						}
					}
					else
					{
						RestoreMaterialsColors(true);
						PlatformDependent.LoadLevelWithLoadingScreen("MissionSelect3D");
					}
				}
				if (optionsMenu)
				{
					if (musicButton == hitCollider)
					{
						if (musicButActive)
						{
							if (sfxOn)
							{
								base.GetComponent<AudioSource>().PlayOneShot(menuClick);
							}
							if (musicOn)
							{
								PlayerPrefs.SetInt("PR_MusicOn", 0);
								musicOn = false;
								MainCamera.StopMusic();
								((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("OFF");
							}
							else
							{
								PlayerPrefs.SetInt("PR_MusicOn", 1);
								musicOn = true;
								MainCamera.StartMusic();
								((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("ON");
							}
						}
						else if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClickBack);
						}
					}
					else if (sfxButton == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						if (sfxOn)
						{
							PlayerPrefs.SetInt("PR_SfxOn", 0);
							sfxOn = false;
							((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("OFF");
							musicButActive = false;
							MainCamera.StopMusic();
							Utils.SfxOn = false;
							((TextMesh)sfxButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("SFX") + Language.GetTxt("OFF");
							continue;
						}
						PlayerPrefs.SetInt("PR_SfxOn", 1);
						musicButActive = true;
						if (musicOn)
						{
							MainCamera.StartMusic();
							((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("ON");
						}
						else
						{
							((TextMesh)musicButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("MUSIC") + Language.GetTxt("OFF");
						}
						Utils.SfxOn = true;
						sfxOn = true;
						((TextMesh)sfxButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("SFX") + Language.GetTxt("ON");
					}
					else if (bloodButton == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						if (bloodOn)
						{
							PlayerPrefs.SetInt("PR_BloodOn", 0);
							bloodOn = false;
							AManager.BloodOn = false;
							((TextMesh)bloodButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("BLOOD") + Language.GetTxt("OFF");
						}
						else
						{
							PlayerPrefs.SetInt("PR_BloodOn", 1);
							bloodOn = true;
							AManager.BloodOn = true;
							((TextMesh)bloodButton.transform.GetChild(0).GetComponent(typeof(TextMesh))).text = Language.GetTxt("BLOOD") + Language.GetTxt("ON");
						}
					}
				}
				else if (restartMenu)
				{
					if (restartYesButton == hitCollider)
					{
						PauseMenuGroup.SetActiveRecursively(false);
						OptionsMenuGroup.SetActiveRecursively(false);
						CombosMenuGroup.SetActiveRecursively(false);
						ObjectivesMenuGroup.SetActiveRecursively(false);
						RestartMenuGroup.SetActiveRecursively(false);
						QuitMenuGroup.SetActiveRecursively(false);
						pauseMenu = false;
						optionsMenu = false;
						restartMenu = false;
						quitMenu = false;
						paused = false;
						cameraPause.enabled = false;
						Time.timeScale = 1f;
						Time.fixedDeltaTime = 0.033f;
						SetRestartLevelPlayerPrefs();
						ShowLoadingScreen();
						RestoreMaterialsColors(true);
						Application.LoadLevel(Application.loadedLevel);
					}
					else if (restartNoButton == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClickBack);
						}
						restartMenu = false;
						optionsMenu = false;
						quitMenu = false;
						RestartMenuGroup.SetActiveRecursively(false);
					}
				}
				else
				{
					if (!quitMenu)
					{
						continue;
					}
					if (quitYesButton == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClick);
						}
						PauseMenuGroup.SetActiveRecursively(false);
						OptionsMenuGroup.SetActiveRecursively(false);
						ObjectivesMenuGroup.SetActiveRecursively(false);
						CombosMenuGroup.SetActiveRecursively(false);
						RestartMenuGroup.SetActiveRecursively(false);
						QuitMenuGroup.SetActiveRecursively(false);
						optionsMenu = false;
						pauseMenu = false;
						restartMenu = false;
						quitMenu = false;
						paused = false;
						cameraPause.enabled = false;
						Time.timeScale = 1f;
						Time.fixedDeltaTime = 0.033f;
						RestoreMaterialsColors(true);
						if (liteVersion)
						{
							PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
						}
						else
						{
							PlatformDependent.LoadLevelWithLoadingScreen("MissionSelect3D");
						}
					}
					else if (quitNoButton == hitCollider)
					{
						if (sfxOn)
						{
							base.GetComponent<AudioSource>().PlayOneShot(menuClickBack);
						}
						restartMenu = false;
						optionsMenu = false;
						quitMenu = false;
						QuitMenuGroup.SetActiveRecursively(false);
					}
				}
			}
		}
		else
		{
			bool pressedMeleeWeaponChange = false;
			bool pressedMeleeRangeChange = false;
			bool pressedThermal = false;
			bool pressedCloak = false;
			bool pressedAttackButtonDown = false;
			bool pressedAttackButtonUp = false;
			bool pressedBlockButtonDown = false;
			bool pressedBlockButtonUp = false;
			if (inputDevice == InputDevice.PC)
			{
				UpdatePC(ref pressedAttackButtonDown, ref pressedAttackButtonUp, ref pressedBlockButtonDown, ref pressedBlockButtonUp, ref pressedCloak, ref pressedMeleeRangeChange, ref pressedMeleeWeaponChange, ref pressedThermal);
			}
			else
			{
				UpdateXperia(ref pressedAttackButtonDown, ref pressedAttackButtonUp, ref pressedBlockButtonDown, ref pressedBlockButtonUp, ref pressedCloak, ref pressedMeleeRangeChange, ref pressedMeleeWeaponChange, ref pressedThermal);
				UpdateTouch(ref pressedAttackButtonDown, ref pressedAttackButtonUp, ref pressedBlockButtonDown, ref pressedBlockButtonUp, ref pressedCloak, ref pressedMeleeRangeChange, ref pressedMeleeWeaponChange, ref pressedThermal);
			}
			lDiff3.Normalize();
			if (isControllable)
			{
				if (pressedMeleeWeaponChange && (ability.plasmaGunUnlocked || ability.diskUnlocked))
				{
					CycleThroughMeleeWeapons();
				}
				if (pressedMeleeRangeChange && (ability.plasmaGunUnlocked || ability.diskUnlocked))
				{
					CycleThroughRangedWeapons();
				}
				if (pressedThermal && ability.thermalVision)
				{
					ThermalButtonPressed();
				}
				if (pressedCloak && ability.cloakUnlocked)
				{
					SwitchCloakMode();
				}
				if (pressedAttackButtonDown && ability.lightAttack)
				{
					AttackButtonDown();
				}
				if (pressedBlockButtonDown && ability.blockUnlocked)
				{
					BlockButtonDown();
				}
				if (pressedAttackButtonUp)
				{
					AttackButtonUp();
				}
				if (pressedBlockButtonUp)
				{
					BlockButtonUp();
				}
			}
		}
		if (!isControllable || meleeLightAttacking || dashJumping)
		{
			return;
		}
		if (!chargingHeavyAttack)
		{
			if (!gettingHurt)
			{
				if (currentWeaponType == 4)
				{
					WhipTrailRenderer.enabled = false;
				}
				if (moved && !grabbingEnemy)
				{
					playerController.Move(lDiff3 * moveSpeed * Time.deltaTime);
					if (targetLocked)
					{
						PlayMoveAnimationsShootingTarget(lDiff3, rDiff3);
					}
					else if (shooting)
					{
						PlayMoveAnimationsShootingForward();
						xForm.rotation = Quaternion.LookRotation(lDiff3);
					}
					else
					{
						PlayMoveForwardAnimations();
						xForm.rotation = Quaternion.LookRotation(lDiff3);
					}
				}
				else
				{
					PlayIdleAnimations();
				}
			}
			else if (!anim.IsPlaying("hurt_move_bck"))
			{
				gettingHurt = false;
			}
			return;
		}
		switch (currentWeaponType)
		{
		case 1:
		case 2:
			if (!anim.IsPlaying("attack_heavy_R_charge"))
			{
				chargingHeavyAttack = false;
			}
			break;
		case 3:
			if (!anim.IsPlaying("spear_attack_heavy_R_charge"))
			{
				chargingHeavyAttack = false;
			}
			break;
		case 4:
			if (!anim.IsPlaying("whip_attack_heavy_R_charge"))
			{
				chargingHeavyAttack = false;
			}
			break;
		}
	}

	private void LogRetryPress()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("LEVEL", currentMission.ToString());
		//FlurryManager.Instance.LogEvent("RETRY", dictionary);
	}

	private void ThermalButtonPressed()
	{
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundChangeVisionMode);
		}
		SwitchThermalVisionMode();
	}

	private void CycleThroughMeleeWeapons()
	{
		if (currentWeapon != WeaponType.Melee)
		{
			StopCoroutine("EnergyIncrease");
			StartCoroutine("EnergyIncrease", 1f);
			SwitchAutoAimMode(false);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
			}
			WeaponStopShooting();
			currentWeapon = WeaponType.Melee;
			if (ability.blockUnlocked)
			{
				BlockStick.gameObject.active = true;
			}
			if (ability.lightAttack)
			{
				AttackStick.gameObject.active = true;
			}
			UpdateWeaponSlotTextures(true);
			return;
		}
		switch (weaponA)
		{
		case 1:
		case 2:
			if (ability.spearUnlocked)
			{
				weaponA = 3;
				UpdateWeaponSlotTextures(true);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
				}
			}
			break;
		case 3:
			if (ability.whipUnlocked)
			{
				weaponA = 4;
			}
			else
			{
				weaponA = 1;
			}
			UpdateWeaponSlotTextures(true);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
			}
			break;
		case 4:
			weaponA = 1;
			UpdateWeaponSlotTextures(true);
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
			}
			break;
		}
	}

	private void CycleThroughRangedWeapons()
	{
		if (currentWeapon != 0)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
			}
			SwitchAutoAimMode(true);
			BlockStick.gameObject.active = false;
			TrailRendererHandL.gameObject.active = false;
			TrailRendererHandR.gameObject.active = false;
			WhipTrailRenderer.enabled = false;
			TrailRendererSpearFront.gameObject.active = false;
			TrailRendererSpearBack.gameObject.active = false;
			if (ability.lightAttack)
			{
				AttackStick.gameObject.active = true;
			}
			WeaponStopShooting();
			currentWeapon = WeaponType.Ranged;
			UpdateWeaponSlotTextures(true);
			return;
		}
		switch (weaponB)
		{
		case 5:
		case 6:
			if (ability.diskUnlocked)
			{
				weaponB = 7;
				UpdateWeaponSlotTextures(true);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
				}
			}
			else if (ability.netGunUnlocked)
			{
				weaponB = 9;
				UpdateWeaponSlotTextures(true);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
				}
			}
			break;
		case 7:
		case 8:
			if (ability.netGunUnlocked)
			{
				weaponB = 9;
				UpdateWeaponSlotTextures(true);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
				}
			}
			else if (ability.plasmaGunUnlocked)
			{
				weaponB = 5;
				UpdateWeaponSlotTextures(true);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
				}
			}
			break;
		case 9:
			if (ability.plasmaGunUnlocked)
			{
				weaponB = 5;
				UpdateWeaponSlotTextures(true);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
				}
			}
			else if (ability.diskUnlocked)
			{
				weaponB = 7;
				UpdateWeaponSlotTextures(true);
				if (sfxOn)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
				}
			}
			break;
		}
	}

	private void AttackButtonUp()
	{
		attackButtonDown = false;
		if (currentWeapon == WeaponType.Melee)
		{
			if (!meleeLightAttacking && !dashJumping && !gettingHurt && !comboAttackHeavy)
			{
				chargingHeavyAttack = false;
				if (!grabbingEnemy)
				{
					switch (currentWeaponType)
					{
					case 1:
					case 2:
						StartCoroutine("ComboAttackPerform", wristComboAttack[0]);
						break;
					case 3:
						StartCoroutine("ComboAttackPerform", spearComboLightAttack[0]);
						break;
					case 4:
						StartCoroutine("ComboAttackPerform", whipComboLightAttack[0]);
						break;
					}
				}
			}
			StopCoroutine("ComboHeavyCharge");
		}
		else
		{
			WeaponStopShooting();
		}
	}

	private void BlockButtonUp()
	{
		blockButtonDown = false;
		blocking = false;
	}

	private void SwitchToRanged()
	{
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
		}
		SwitchAutoAimMode(true);
		TrailRendererHandL.gameObject.active = false;
		TrailRendererHandR.gameObject.active = false;
		WhipTrailRenderer.enabled = false;
		TrailRendererSpearFront.gameObject.active = false;
		TrailRendererSpearBack.gameObject.active = false;
		WeaponStopShooting();
		currentWeapon = WeaponType.Ranged;
		UpdateWeaponSlotTextures(true);
	}

	private void SwitchToMelee()
	{
		StopCoroutine("EnergyIncrease");
		StartCoroutine("EnergyIncrease", 1f);
		SwitchAutoAimMode(false);
		if (sfxOn)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundChangeWeapon);
		}
		WeaponStopShooting();
		currentWeapon = WeaponType.Melee;
		UpdateWeaponSlotTextures(true);
	}

	private void LateUpdate()
	{
		if (currentWeapon == WeaponType.Ranged)
		{
			LaserLineRenderer.SetPosition(0, xForm.TransformPoint(LaserLineStartOffset));
			if (Physics.Raycast(xForm.TransformPoint(LaserLineStartOffset), xForm.forward, out hitInfo, laserLineLength, laserLineCullingMask))
			{
				LaserLineRenderer.SetPosition(1, hitInfo.point);
			}
			else
			{
				LaserLineRenderer.SetPosition(1, xForm.TransformPoint(LaserLineStartOffset) + xForm.forward * laserLineLength);
			}
		}
		if (!targetLocked || !isControllable)
		{
			return;
		}
		if (newTargetFound)
		{
			rDiff3 = targetTriangleTransform.position - xForm.TransformPoint(shootingPointOffset);
			rDiff3.y = 0f;
			if (rDiff3.sqrMagnitude > minTargetDistanceSqr)
			{
				targetRotation = Quaternion.LookRotation(rDiff3);
				xForm.rotation = Quaternion.Slerp(xForm.rotation, targetRotation, Time.deltaTime * rotateSpeed);
			}
			rDiff3.Normalize();
			timerNewTargetRotate -= Time.deltaTime;
			if (timerNewTargetRotate < 0f)
			{
				newTargetFound = false;
			}
		}
		else
		{
			rDiff3 = targetTriangleTransform.position - xForm.TransformPoint(shootingPointOffset);
			rDiff3.y = 0f;
			if (rDiff3.sqrMagnitude > minTargetDistanceSqr)
			{
				xForm.rotation = Quaternion.LookRotation(rDiff3);
			}
			rDiff3.Normalize();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, wristBladesCheckRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(spearOffset), spearDamageRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(discOffset), 0.1f);
	}

	private void RestoreMaterialsColors(bool showLoadingScreen)
	{
		if ((bool)loadingScreen && showLoadingScreen)
		{
			ShowLoadingScreen();
		}
		materialNormal.color = ColorDaytime;
		materialBerserkers.color = ColorDaytime;
		materialNormal.SetTexture("_MainTex", TextureCharactersNormal);
		materialBerserkers.SetTexture("_MainTex", TextureBerserkersNormal);
		for (int i = 0; i < bloodMaterialsHumanCount; i++)
		{
			bloodMaterialsHuman[i].SetColor("_TintColor", bloodColorHuman);
		}
		for (int i = 0; i < bloodMaterialsPredatorCount; i++)
		{
			bloodMaterialsPredator[i].SetColor("_TintColor", bloodColorPredator);
		}
		HealthBarMaterial.SetTextureOffset("_MainTex", Vector2.zero);
	}

	private void OnApplicationQuit()
	{
		RestoreMaterialsColors(false);
	}

	public void setMissionFailedToTrue()
	{
		TrailRendererHandL.gameObject.active = false;
		TrailRendererHandR.gameObject.active = false;
		TrailRendererSpearFront.gameObject.active = false;
		TrailRendererSpearBack.gameObject.active = false;
		WhipTrailRenderer.enabled = false;
		StartCoroutine(MenuMissionFailed());
	}

	private IEnumerator MenuMissionFailed()
	{
		isControllable = false;
		if (!dead)
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundPredatorAngry);
			}
			anim.CrossFade("idle");
		}
		WeaponStopShooting();
		yield return new WaitForSeconds(2f);
		if (currentMission == 34)
		{
			Time.timeScale = 1f;
			RestoreMaterialsColors(true);
			if (liteVersion)
			{
				PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
			}
			else
			{
				PlatformDependent.LoadLevelWithLoadingScreen("MissionSelect3D");
			}
		}
		else
		{
			missionFailed = true;
			cameraPause.enabled = true;
			paused = true;
			setActiveMissionFailedMenu(true);
			Time.timeScale = 0f;
			horizontalLinesBackground.gameObject.active = true;
		}
	}

	private void setActiveMissionSuccessMenu(bool value)
	{
		successMissionButtonOk.gameObject.SetActiveRecursively(value);
		successMissionTextMessage.gameObject.SetActiveRecursively(value);
		toolTipBackgroundMissionEnd.gameObject.SetActiveRecursively(value);
	}

	public void setMissionSuccessToTrue(string message)
	{
		TrailRendererHandL.gameObject.active = false;
		TrailRendererHandR.gameObject.active = false;
		TrailRendererSpearFront.gameObject.active = false;
		TrailRendererSpearBack.gameObject.active = false;
		WhipTrailRenderer.enabled = false;
		if (CurrentAttackAnimation != AttackAnimation.WristGrabCutHalf && CurrentAttackAnimation != AttackAnimation.WristGrabHeadOff)
		{
			StopAllCoroutines();
		}
		if (currentMission == 8 && !ability.plasmaGunUnlocked)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsPlasmaUnlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_PlasmaGunLevel_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 1), 1);
		}
		if (!PlatformDependent.InAppPurchase && currentMission == 9 && EncryptedPlayerPrefs.GetInt("PR_Survival1Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1) == 0)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsSurvival1Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_Survival1Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
		}
		if (currentMission == 10 && !ability.spearUnlocked)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsSpearUnlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_SpearLevel_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 1), 1);
		}
		if (currentMission == 13 && !ability.netGunUnlocked)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsNetGunUnlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_NetGunLevel_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 1), 1);
		}
		if (currentMission == 15 && !ability.diskUnlocked)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsDiscUnlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_DiskLevel_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 1), 1);
		}
		if (currentMission == 25 && !ability.whipUnlocked)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsWhipUnlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_WhipLevel_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 1), 1);
		}
		if (!PlatformDependent.InAppPurchase && currentMission == 23 && EncryptedPlayerPrefs.GetInt("PR_Survival2Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1) == 0)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsSurvival2Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_Survival2Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
		}
		if (currentMission == 31 && EncryptedPlayerPrefs.GetInt("PR_MaskType4Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1) == 0)
		{
			EncryptedPlayerPrefs.SetInt("PR_PlayerKnowsMaskType4Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot"), 1);
			EncryptedPlayerPrefs.SetInt("PR_MaskType4Unlocked_S" + EncryptedPlayerPrefs.GetInt("PR_CurrentSlot", 1), 1);
		}
		StartCoroutine(MenuMissionSuccess(message));
	}

	private IEnumerator MenuMissionSuccess(string message)
	{
		AManager.instance.CinematicInProgress = true;
		isControllable = false;
		WeaponStopShooting();
		if (CurrentAttackAnimation == AttackAnimation.WristGrabCutHalf || CurrentAttackAnimation == AttackAnimation.WristGrabHeadOff)
		{
			anim.PlayQueued("yell");
		}
		else
		{
			if (sfxOn)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundYell);
			}
			anim["yell"].time = 0f;
			anim.CrossFade("yell");
		}
		yield return new WaitForSeconds(2.26f);
		missionSuccess = true;
		paused = true;
		cameraPause.enabled = true;
		setActiveMissionSuccessMenu(true);
		successMissionTextMessage.text = message;
		horizontalLinesBackground.gameObject.active = true;
		Time.timeScale = 0f;
		survivalMissionController.SavePlayerStats();
	}

	private void setActiveMissionFailedMenu(bool value)
	{
		failMissionButtonQuit.gameObject.SetActiveRecursively(value);
		failMissionButtonRetry.gameObject.SetActiveRecursively(value);
		failMissionTextMessage.gameObject.SetActiveRecursively(value);
		toolTipBackgroundMissionEnd.gameObject.SetActiveRecursively(value);
	}

	private void BuildArrayOfTips()
	{
		TipCondition aHideCondition = () => texture3IsTapped;
		TipCondition aHideCondition2 = () => texture4IsTapped;
		TipCondition tipCondition = () => true;
		TipCondition aDisplayCondition = delegate
		{
			Time.timeScale = 0f;
			isControllable = false;
			return true;
		};
		TipCondition aHideCondition3 = delegate
		{
			if (texture2IsTapped)
			{
				Time.timeScale = 1f;
				isControllable = true;
			}
			return texture2IsTapped;
		};
		int charsPerLine = 28;
		float aTimeToWaitAtLeast = 5f;
		if (liteVersion)
		{
			tips = new ArrayList();
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION1_TIP_MOVE", charsPerLine), 2f, 1f, () => currentWeapon == WeaponType.Melee, () => moved || texture3IsTapped, delegate
			{
				if (!leftStickBlinked)
				{
					leftStickBlinked = true;
					PlatformDependent.StartBlinking(LeftStick, this, 2, 0f);
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION1_TIP_LIGHT_ATTACK", charsPerLine), 2f, 1f, () => currentWeapon == WeaponType.Melee, () => currentPerformingAnimation == AttackAnimation.WristLightL || currentPerformingAnimation == AttackAnimation.WristLightR || texture3IsTapped, delegate
			{
				if (!ability.lightAttack)
				{
					ability.lightAttack = true;
					SetupCombosText();
					PlatformDependent.StartBlinking(AttackStick, this, 4, 0f);
					PlatformDependent.SetActiveIphoneGUI(AttackStick, true);
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION1_TIP_HEALTH_BAR", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.SpawnEnemiesMaxCurrent = 2;
					survivalMissionController.StartSpawningEnemies();
				}
				if (!HealthBarGameObject.active)
				{
					HealthBarGameObject.active = true;
					StartCoroutine(GUIControlBlink(HealthBarGameObject.GetComponent<Renderer>(), 4, 0f));
				}
				if (!ability.heavyAttack)
				{
					ability.heavyAttack = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION3_TIP_SPIN_COMBO", charsPerLine), 15f, 0.5f, () => currentWeapon == WeaponType.Melee, () => currentPerformingAnimation == AttackAnimation.WristSpinL || texture3IsTapped, delegate
			{
				if (!ability.spinAttack)
				{
					survivalMissionController.SpawnEnemiesMaxCurrent = 4;
					ability.spinAttack = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION_LITE_TIP_DASH_JUMP", charsPerLine), 2f, 2f, () => currentWeapon == WeaponType.Melee, () => dashJumping || texture3IsTapped, delegate
			{
				if (!ability.blockUnlocked)
				{
					survivalMissionController.ExtraSoldiersRifle = 50000;
					PlatformDependent.StartBlinking(BlockStick, this, 4, 0f);
					PlatformDependent.SetActiveIphoneGUI(BlockStick, true);
					ability.blockUnlocked = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION5_TIP_IMPALE_ENEMY", charsPerLine), 15f, 2f, () => currentWeapon == WeaponType.Melee, () => grabbingEnemy || texture3IsTapped, delegate
			{
				if (!ability.impale)
				{
					survivalMissionController.SpawnEnemiesMaxCurrent = 2;
					ability.impale = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION5_TIP_BODY_SLICE", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[4], Language.GetTxt("MISSION_LITE_TIP_TROPHY_KILL", charsPerLine), 2f, 2f, () => currentWeapon == WeaponType.Melee, () => currentPerformingAnimation == AttackAnimation.WristGrabHeadOff || texture4IsTapped, delegate
			{
				if (!ability.trophyKill)
				{
					AManager.difficultyLevel = 3f;
					ability.trophyKill = true;
					SetupCombosText();
					PlatformDependent.StartBlinking(BlockStick, this, 1, 0f);
					PlatformDependent.StartBlinking(AttackStick, this, 3, 0.6f);
				}
				return true;
			}));
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[5], Language.GetTxt("MISSION15_TIP_DISK_TOGGLE", charsPerLine), 20f, 2f, tipCondition, () => currentWeaponType == 7, delegate
			{
				if (!ability.diskUnlocked)
				{
					AManager.difficultyLevel = 1.5f;
					ability.diskUnlocked = true;
					weaponB = 7;
					UpdateWeaponSlotTextures(true);
					survivalMissionController.SpawnEnemiesMaxCurrent = 5;
				}
				if (!rangedWeaponBlinked)
				{
					rangedWeaponBlinked = true;
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Inactive.GetComponent<Renderer>(), 4, 0f));
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION_LITE_TIP_KILL_NIKOLAI", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			return;
		}
		switch (currentMission)
		{
		case 1:
			tips = new ArrayList();
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION1_TIP_MOVE", charsPerLine), 2f, 2f, () => currentWeapon == WeaponType.Melee, () => moved || texture3IsTapped, delegate
			{
				if (!leftStickBlinked)
				{
					leftStickBlinked = true;
					PlatformDependent.StartBlinking(LeftStick, this, 2, 0f);
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION1_TIP_LIGHT_ATTACK", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => currentPerformingAnimation == AttackAnimation.WristLightL || currentPerformingAnimation == AttackAnimation.WristLightR || texture3IsTapped, delegate
			{
				if (!ability.lightAttack)
				{
					ability.lightAttack = true;
					SetupCombosText();
					PlatformDependent.StartBlinking(AttackStick, this, 4, 0f);
					PlatformDependent.SetActiveIphoneGUI(AttackStick, true);
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION1_TIP_HEALTH_BAR", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					if (!survivalMissionController.MissionStatus.gameObject.active)
					{
						GUI_IconKills.gameObject.active = true;
						GUI_currentKillsIcon = GUI_IconKills;
						survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
					}
					survivalMissionController.StartSpawningEnemies();
				}
				if (!HealthBarGameObject.active)
				{
					HealthBarGameObject.active = true;
					StartCoroutine(GUIControlBlink(HealthBarGameObject.GetComponent<Renderer>(), 4, 0f));
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION1_TIP_KILL_5", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 2:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[0], Language.GetTxt("MISSION2_TIP_HEAVY_ATTACK", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => currentPerformingAnimation == AttackAnimation.WristHeavyR, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					if (!survivalMissionController.MissionStatus.gameObject.active)
					{
						GUI_IconKills.gameObject.active = true;
						GUI_currentKillsIcon = GUI_IconKills;
						survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
					}
					survivalMissionController.StartSpawningEnemies();
				}
				if (!ability.heavyAttack)
				{
					ability.heavyAttack = true;
					SetupCombosText();
					PlatformDependent.StartBlinking(AttackStick, this, 4, 0f);
				}
				return true;
			}));
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION2_TIP_HEAVY_ATTACK_BREAK_DEFENSE", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => conditionPerformedHeavyAttackHit || texture3IsTapped, () => true));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION2_TIP_KILL_REST", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 3:
			tips = new ArrayList();
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION3_TIP_SPIN_COMBO", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => currentPerformingAnimation == AttackAnimation.WristSpinL || texture3IsTapped, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					if (!survivalMissionController.MissionStatus.gameObject.active)
					{
						GUI_IconKills.gameObject.active = true;
						GUI_currentKillsIcon = GUI_IconKills;
						survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
					}
					survivalMissionController.StartSpawningEnemies();
				}
				if (!ability.spinAttack)
				{
					ability.spinAttack = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(3f, tipTextures[4], Language.GetTxt("MISSION3_TIP_HONOR_POINTS", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, () => true));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION3_TIP_KILL_15", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 4:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[1], Language.GetTxt("MISSION4_TIP_BLOCK_BUTTON", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => blocking, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				if (!ability.blockUnlocked)
				{
					PlatformDependent.StartBlinking(BlockStick, this, 4, 0f);
					PlatformDependent.SetActiveIphoneGUI(BlockStick, true);
					ability.blockUnlocked = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[3], Language.GetTxt("MISSION_LITE_TIP_DASH_JUMP", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => dashJumping || texture3IsTapped, () => true));
			tips.Add(new TapTip(4f, tipTextures[4], Language.GetTxt("MISSION4_TIP_DASH_COMBOS", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, () => true));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION4_TIP_KILL_15", charsPerLine), 2f, 0f, tipCondition, aHideCondition, tipCondition));
			break;
		case 5:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[0], Language.GetTxt("MISSION5_TIP_IMPALE_ENEMY", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => grabbingEnemy, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				if (!ability.impale)
				{
					ability.impale = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION5_TIP_BODY_SLICE", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION5_TIP_KILL_10", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 6:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[0], Language.GetTxt("MISSION6_TIP_TROPHY_KILL", charsPerLine), 2f, 0f, () => currentWeapon == WeaponType.Melee, () => currentPerformingAnimation == AttackAnimation.WristGrabHeadOff, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconTrophies.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconTrophies;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				if (!ability.trophyKill)
				{
					ability.trophyKill = true;
					SetupCombosText();
					PlatformDependent.StartBlinking(BlockStick, this, 1, 0f);
					PlatformDependent.StartBlinking(AttackStick, this, 3, 0.6f);
				}
				return true;
			}));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION6_TIP_COLLECT_4_MORE_TROPHIES", charsPerLine), 2f, 0f, tipCondition, aHideCondition, tipCondition));
			break;
		case 7:
			tips = new ArrayList();
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION7_TIP_SURVIVE_5_MIN", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconTimer.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconTimer;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			tips.Add(new TapTip(3f, tipTextures[3], Language.GetTxt("MISSION7_TIP_WATCH_FOR_SNIPERS", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 8:
			tips = new ArrayList();
			tips.Add(new TapTip(aTimeToWaitAtLeast, tipTextures[4], Language.GetTxt("MISSION8_TIP_CLOAK", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!ability.cloakUnlocked)
				{
					symbol1[9].active = true;
					symbol2[9].active = true;
					symbol3[9].active = true;
					symbol4[9].active = true;
					GUI_Cloak.active = true;
					ability.cloakUnlocked = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[1], Language.GetTxt("MISSION8_TIP_CLOAK_TOGGLE", charsPerLine), 2f, 0f, tipCondition, () => cloakModeOn, delegate
			{
				if (!cloakButtonBlinked)
				{
					cloakButtonBlinked = true;
					StartCoroutine(GUIControlBlink(GUI_Cloak.GetComponent<Renderer>(), 4, 2f));
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION8_TIP_CLOAK_DETERRENT", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION8_TIP_STEALTH_KILL", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION8_TIP_KILL_10_NOT_DETECTED", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 9:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION9_TIP_PLASMA_UNLOCKED", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!ability.plasmaGunUnlocked)
				{
					ability.plasmaGunUnlocked = true;
					UpdateWeaponSlotTextures(true);
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[5], Language.GetTxt("MISSION9_TIP_SWITCH_TO_PLASMA", charsPerLine), 2f, 0f, tipCondition, () => currentWeapon == WeaponType.Ranged, delegate
			{
				if (!rangedWeaponBlinked)
				{
					rangedWeaponBlinked = true;
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Inactive.GetComponent<Renderer>(), 4, 0f));
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION9_TIP_PLASMA_USE", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, tipCondition));
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION8_TIP_ENERGY", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!energyBlinked)
				{
					energyBlinked = true;
					int timesToBlink = 10;
					float timeUntilStart = 0f;
					StartCoroutine(GUIControlBlink(symbol1[9].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[9].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[9].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[9].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[8].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[8].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[8].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[8].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[7].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[7].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[7].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[7].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[6].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[6].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[6].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[6].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[5].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[5].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[5].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[5].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[4].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[4].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[4].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[4].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[3].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[3].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[3].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[3].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[2].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[2].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[2].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[2].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[1].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[1].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[1].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[1].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol1[0].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol2[0].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol3[0].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
					StartCoroutine(GUIControlBlink(symbol4[0].GetComponent<Renderer>(), timesToBlink, timeUntilStart));
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION8_TIP_ENERGY_VITAL_FOR_STUFF_TO_WORK", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION9_TIP_HUNT_SPREE_15", charsPerLine), 2f, 0f, tipCondition, aHideCondition, tipCondition));
			break;
		case 10:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[0], Language.GetTxt("MISSION10_TIP_THERMAL_VISION", charsPerLine), 2f, 0f, tipCondition, () => thermalVisionMode, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				if (!ability.thermalVision)
				{
					ability.thermalVision = true;
					GUI_Thermal.active = true;
					StartCoroutine(GUIControlBlink(GUI_Thermal.GetComponent<Renderer>(), 4, 0f));
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION10_TIP_KILL_STANS", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, () => true));
			break;
		case 11:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[5], Language.GetTxt("MISSION10_TIP_COMBI_STICK_UNLOCKED", charsPerLine), 2f, 0f, tipCondition, () => currentWeaponType == 3, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
					StartCoroutine(GUIControlBlink(GUI_WeaponWristBlades_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponWristBlades_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponCombiStick_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponCombiStick_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponWhip_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponWhip_Inactive.GetComponent<Renderer>(), 4, 0f));
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				if (!ability.spearUnlocked)
				{
					ability.spearUnlocked = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION11_TIP_KILL_20", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 12:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION12_TIP_SURVIVE_5_WAVES", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconWavesRemaining.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconWavesRemaining;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 13:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION13_TIP_KILL_ROYCE", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 14:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[5], Language.GetTxt("MISSION14_TIP_NETGUN_TOGGLE", charsPerLine), 2f, 0f, tipCondition, () => currentWeaponType == 9, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconNetgunKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconNetgunKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				if (!ability.netGunUnlocked)
				{
					ability.netGunUnlocked = true;
				}
				if (!rangedWeaponBlinked)
				{
					rangedWeaponBlinked = true;
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Inactive.GetComponent<Renderer>(), 4, 0f));
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION14_TIP_KILL_10", charsPerLine), 2f, 0f, tipCondition, aHideCondition, () => true));
			break;
		case 15:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION15_TIP_KILL_10_ISOLATION", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 16:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION16_TIP_KILL_ENEMIES_WITH_DISC", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!ability.diskUnlocked)
				{
					ability.diskUnlocked = true;
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconDiscKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconDiscKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[5], Language.GetTxt("MISSION15_TIP_DISK_TOGGLE", charsPerLine), 2f, 0f, tipCondition, () => currentWeaponType == 7, delegate
			{
				if (!rangedWeaponBlinked)
				{
					rangedWeaponBlinked = true;
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponPlasmaGun_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponDisc_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponNetGun_Inactive.GetComponent<Renderer>(), 4, 0f));
				}
				return true;
			}));
			break;
		case 17:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION17_TIP_KILL_HANZO", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 18:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION18_TIP_KILL_ISABELLE_AND_10_SNIPERS_NIGHT", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 19:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION19_TIP_KILL_MOMBASA", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 20:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[4], Language.GetTxt("MISSION20_TIP_KILL_CUCHILLO", charsPerLine), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconTimer.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconTimer;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 21:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION21_TIP_KILL_NIKOLAI", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 22:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION22_TIP_RESIST_5_WAVES", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconWavesRemaining.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconWavesRemaining;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 23:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION23_TIP_KILL_NOLAND", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconTrophies.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconTrophies;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 24:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION24_TIP_SURVIVE_DOG_ATTACK", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconTimer.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconTimer;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 25:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION25_TIP_TAUNT_THE_TRACKER", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 26:
			tips = new ArrayList();
			tips.Add(new Tip(aTimeToWaitAtLeast, tipTextures[5], Language.GetTxt("MISSION26_TIP_SWITCH_TO_WHIP", charsPerLine), 2f, 0f, tipCondition, () => currentWeaponType == 4, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
					StartCoroutine(GUIControlBlink(GUI_WeaponWristBlades_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponWristBlades_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponCombiStick_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponCombiStick_Inactive.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponWhip_Active.GetComponent<Renderer>(), 4, 0f));
					StartCoroutine(GUIControlBlink(GUI_WeaponWhip_Inactive.GetComponent<Renderer>(), 4, 0f));
				}
				if (!ability.whipUnlocked)
				{
					ability.whipUnlocked = true;
					SetupCombosText();
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION26_TIP_WHIP_COMBO_SPLICE", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION26_TIP_WHIP_COMBO_GRAPPLE", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION26_TIP_WHIP_COMBO_CHOPPER", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION26_TIP_WHIP_KILL", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 27:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION27_TIP_FALCONS", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 28:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION28_TIP_TUSKS_AND_TALONS", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 29:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION29_TIP_DIV_AND_CONQUER", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconTrophies.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconTrophies;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 30:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION30_TIP_CLAN_LEADER", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconTimer.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconTimer;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 31:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[3], Language.GetTxt("MISSION31_TIP_BERSERKER", charsPerLine), 2f, 0f, tipCondition, aHideCondition, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 32:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[0], Language.GetTxt("MISSION_DESCRIPTION_SURVIVAL_2", 24), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconKills.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconKills;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 33:
			tips = new ArrayList();
			tips.Add(new TapTip(6f, tipTextures[0], Language.GetTxt("MISSION_DESCRIPTION_SURVIVAL_2", 24), 2f, 0f, tipCondition, aHideCondition2, delegate
			{
				if (!survivalMissionController.MissionStarted)
				{
					survivalMissionController.StartSpawningEnemies();
				}
				if (!survivalMissionController.MissionStatus.gameObject.active)
				{
					GUI_IconWavesRemaining.gameObject.active = true;
					GUI_currentKillsIcon = GUI_IconWavesRemaining;
					survivalMissionController.MissionStatus.gameObject.SetActiveRecursively(true);
				}
				return true;
			}));
			break;
		case 34:
			tips = new ArrayList();
			tips.Add(new Tip(0f, tipTextures[2], Language.GetTxt("STORY_INTRO_MISSION_DESCRIPTION", 29), 0f, 0f, aDisplayCondition, aHideCondition3, () => true));
			break;
		}
	}

	private IEnumerator DisplayTips()
	{
		BuildArrayOfTips();
		elapsedShowTipTime = 0f;
		tipIsUnderstood = false;
		int i = 0;
		tipActiveAndWaitingTap = true;
		if (tips.Count > 0)
		{
			TextMesh gUI_ObjectivesText = GUI_ObjectivesText;
			gUI_ObjectivesText.text = gUI_ObjectivesText.text + "\n" + ((Tip)tips[0]).Message;
		}
		while (i < tips.Count)
		{
			switch (currentTipState)
			{
			case TipState.Hide:
				if (((Tip)tips[i]).isDisplayable())
				{
					yield return StartCoroutine(((Tip)tips[i]).waitSecondsBeforeDisplay());
					((Tip)tips[i]).display();
					((Tip)tips[i]).performTipAction();
					currentTipState = TipState.Show;
					elapsedShowTipTime = 0f;
					tipIsUnderstood = false;
				}
				break;
			case TipState.Show:
				elapsedShowTipTime += Time.deltaTime;
				if (!((Tip)tips[i]).isDisplayable())
				{
					((Tip)tips[i]).hide();
					currentTipState = TipState.Hide;
				}
				else
				{
					if (!tipIsUnderstood && !((Tip)tips[i]).isUnderstood())
					{
						break;
					}
					tipIsUnderstood = true;
					yield return StartCoroutine(((Tip)tips[i]).waitSecondsBeforeHide());
					((Tip)tips[i]).hide();
					if (i != 0)
					{
						if (liteVersion)
						{
							if (tips.Count - 1 == i || tips.Count - 2 == i)
							{
								TextMesh gUI_ObjectivesText2 = GUI_ObjectivesText;
								gUI_ObjectivesText2.text = gUI_ObjectivesText2.text + "\n" + ((Tip)tips[i]).Message;
							}
						}
						else
						{
							switch (currentMission)
							{
							case 9:
								if (i != 0 && i != 3)
								{
									TextMesh gUI_ObjectivesText5 = GUI_ObjectivesText;
									gUI_ObjectivesText5.text = gUI_ObjectivesText5.text + "\n" + ((Tip)tips[i]).Message;
								}
								break;
							case 8:
								if (i != 0)
								{
									TextMesh gUI_ObjectivesText4 = GUI_ObjectivesText;
									gUI_ObjectivesText4.text = gUI_ObjectivesText4.text + "\n" + ((Tip)tips[i]).Message;
								}
								break;
							default:
							{
								TextMesh gUI_ObjectivesText3 = GUI_ObjectivesText;
								gUI_ObjectivesText3.text = gUI_ObjectivesText3.text + "\n" + ((Tip)tips[i]).Message;
								break;
							}
							}
						}
					}
					i++;
					currentTipState = TipState.Hide;
				}
				break;
			}
			yield return null;
		}
		tipActiveAndWaitingTap = false;
	}
}
