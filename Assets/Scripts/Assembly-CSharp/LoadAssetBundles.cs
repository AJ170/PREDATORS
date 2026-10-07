using System;
using System.Collections;
using UnityEngine;

public class LoadAssetBundles : MonoBehaviour
{
#if UNITY_ANDROID || UNITY_IOS
	private const float networkIdleTimeout = 10f;

	public Button buttonRetry;

	public Button buttonLater;

	public TextMesh TextMessage;

	public TextMesh TextDownloading;

	public TextMesh textDownloadProgress;

	public bool loadMainMenuIfSuccessful;

	private float downloadProgress;

	private float totalDownloadProgress;

	private static WWW download;

	public static WWW Download
	{
		get
		{
			return download;
		}
		set
		{
			download = value;
		}
	}

	private void Start()
	{
		buttonRetry.gameObject.SetActiveRecursively(false);
		buttonLater.gameObject.SetActiveRecursively(false);
		TextMessage.gameObject.SetActiveRecursively(false);
		buttonRetry.onPressBegin += OnButtonRetryClicked;
		buttonLater.onPressBegin += OnButtonLaterClicked;
		if (!Caching.IsVersionCached("http://www.angrymobgames.com/assetbundles/PredatorsCompiledScenes.unity3d", 1312) && Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
		{
			TextMessage.text = "You are about to download " + 57.3f + "MB\nthrough the carrier 3G network\ndo you want to continue?";
			TextMessage.gameObject.active = true;
			textDownloadProgress.gameObject.active = false;
			TextDownloading.gameObject.active = false;
			buttonRetry.buttonText.SetTextInChildren("Yes");
			buttonLater.gameObject.SetActiveRecursively(true);
			buttonRetry.gameObject.SetActiveRecursively(true);
		}
		else
		{
			StartTheDownload();
		}
	}

	private void StartTheDownload()
	{
		download = WWW.LoadFromCacheOrDownload("http://www.angrymobgames.com/assetbundles/PredatorsCompiledScenes.unity3d", 1312);
		if (!Caching.IsVersionCached("http://www.angrymobgames.com/assetbundles/PredatorsCompiledScenes.unity3d", 1312))
		{
			StartCoroutine(RetrieveBundle());
			return;
		}
		textDownloadProgress.gameObject.active = false;
		TextDownloading.text = "Loading";
		CheckDownloadResult();
	}

	public void OnButtonRetryClicked(object sender, EventArgs args)
	{
		buttonRetry.gameObject.SetActiveRecursively(false);
		buttonLater.gameObject.SetActiveRecursively(false);
		TextMessage.gameObject.active = false;
		TextDownloading.gameObject.active = true;
		StartTheDownload();
	}

	public void OnButtonLaterClicked(object sender, EventArgs args)
	{
		Application.Quit();
	}

	private void ShowErrorMessage()
	{
		if (Application.internetReachability != 0)
		{
			TextMessage.text = "Error downloading extra assets.\nMake sure you're connected\nto the internet and you have\nenough free space!";
		}
		else
		{
			TextMessage.text = "Internet not reachable!";
		}
		TextMessage.gameObject.active = true;
		textDownloadProgress.gameObject.active = false;
		TextDownloading.gameObject.active = false;
		buttonRetry.buttonText.SetTextInChildren("Retry");
		buttonLater.gameObject.SetActiveRecursively(true);
		buttonRetry.gameObject.SetActiveRecursively(true);
	}

	private IEnumerator NetworkIdleTooLongCheck()
	{
		yield return new WaitForSeconds(10f);
		StopCoroutine("RetrieveBundle");
		ShowErrorMessage();
	}

	private IEnumerator RetrieveBundle()
	{
		StopCoroutine("NetworkIdleTooLongCheck");
		StartCoroutine("NetworkIdleTooLongCheck");
		int frames = 0;
		while (!download.isDone)
		{
			if (download.error == null)
			{
				if (downloadProgress != download.progress)
				{
					StopCoroutine("NetworkIdleTooLongCheck");
					StartCoroutine("NetworkIdleTooLongCheck");
				}
				downloadProgress = download.progress;
			}
			if (frames < 5)
			{
				frames++;
			}
			else
			{
				TextDownloading.gameObject.active = true;
				textDownloadProgress.gameObject.SetActiveRecursively(true);
			}
			textDownloadProgress.text = (downloadProgress * 57.3f).ToString("00.00") + " MB / " + 57.3f.ToString("00.00") + " MB";
			yield return new WaitForSeconds(0.1f);
		}
		StopCoroutine("NetworkIdleTooLongCheck");
		CheckDownloadResult();
	}

	private void CheckDownloadResult()
	{
		if (download.error != null)
		{
			MonoBehaviour.print(download.error);
			ShowErrorMessage();
			return;
		}
		AssetBundle assetBundle = download.assetBundle;
		TextDownloading.text = "Loading";
		textDownloadProgress.gameObject.SetActiveRecursively(false);
		if (loadMainMenuIfSuccessful)
		{
			PlayMovieLoadMenu();
		}
		else
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}

	private void PlayMovieLoadMenu()
	{
		StartCoroutine(PlayMovieCR());
	}

	private IEnumerator PlayMovieCR()
	{
		yield return null;
#if !UNITY_STANDALONE
		Handheld.PlayFullScreenMovie("SplashVid.m4v", Color.black, FullScreenMovieControlMode.CancelOnInput);
		PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
#endif
	}
#endif
}
