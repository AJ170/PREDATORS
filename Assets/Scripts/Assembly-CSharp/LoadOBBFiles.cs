using System.Collections;
using UnityEngine;

public class LoadOBBFiles : MonoBehaviour
{
    public TextMesh message;

    public GameObject assetLoader;

    private string exPath;

    private string mainPath;

    private string bundleFullPath;

    private WWW download;

    private void Start()
    {
#if UNITY_ANDROID
        PlayMovieLoadMenu();
#else
        MonoBehaviour.print("OBB integration skipped (video intro component owns boot transition).");
#endif
    }

    private void VerifyAndIntegrateOBBFiles()
    {
        exPath = GooglePlayDownloader.GetExpansionFilePath();
        if (exPath == null)
        {
            ShowMessage("External storage is not available!");
            DelayedExitApplication(3f);
            return;
        }
        MonoBehaviour.print("ExPath: " + exPath);
        mainPath = GooglePlayDownloader.GetMainOBBPath(exPath);
        if (mainPath == null)
        {
            MonoBehaviour.print("Main path not found, downloading additional files");
            ShowMessage("Downloading additional files");
            GooglePlayDownloader.FetchOBB();
            StartCoroutine(WaitForFileDownload());
        }
        else
        {
            IntegrateOBB();
        }
    }

    private void IntegrateOBB()
    {
        MonoBehaviour.print("Integrating additional files");
        bundleFullPath = "jar:file://" + mainPath + "!/PredatorsCompiledScenes.unity3d";
        MonoBehaviour.print("BUndle full path: " + bundleFullPath);
        download = WWW.LoadFromCacheOrDownload(bundleFullPath, 1312);
#if UNITY_ANDROID
        if (Caching.IsVersionCached(bundleFullPath, 1312))
        {
            MonoBehaviour.print("All assets included,starting game");
            if (download.error != null)
            {
                MonoBehaviour.print("BUNDLE LOADING ERROR : " + download.error);
                ShowMessage("Error loading assets");
            }
            else
            {
                PlayMovieLoadMenu();
            }
        }
        else
        {
            MonoBehaviour.print("Including downloaded assets");
            StartCoroutine(LoadAssetBundleFromOBB());
        }
#endif
    }

    private IEnumerator WaitForFileDownload()
    {
        while (true)
        {
            mainPath = GooglePlayDownloader.GetMainOBBPath(exPath);
            if (mainPath != null)
            {
                break;
            }
            yield return new WaitForSeconds(1f);
        }
        IntegrateOBB();
    }

    private IEnumerator LoadAssetBundleFromOBB()
    {
        while (!download.isDone)
        {
            if (download.error != null)
            {
                MonoBehaviour.print("BUNDLE LOADING ERROR : " + download.error);
                ShowMessage("Error loading assets");
                DelayedExitApplication(2f);
            }
            else
            {
                ShowMessage("Loading Assets " + (download.progress * 100f).ToString("00.0") + " %");
            }
            yield return null;
        }
        if (download.error != null)
        {
            MonoBehaviour.print("BUNDLE LOADING ERROR : " + download.error);
            ShowMessage("Error loading assets");
            DelayedExitApplication(2f);
        }
        else
        {
            PlayMovieLoadMenu();
        }
    }

    private void ShowMessage(string msg)
    {
        message.gameObject.SetActiveRecursively(true);
        message.text = msg;
    }

    private void DelayedExitApplication(float delay)
    {
        StartCoroutine(DelayedExitApplicationCR(delay));
    }

    private IEnumerator DelayedExitApplicationCR(float delay)
    {
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }

    private void PlayMovieLoadMenu()
    {
        StartCoroutine(PlayMovieCR());
    }

    private IEnumerator PlayMovieCR()
    {
        yield return null;
#if UNITY_ANDROID
        Handheld.PlayFullScreenMovie("SplashVid.m4v", Color.black, FullScreenMovieControlMode.CancelOnInput);
        yield return new WaitForSeconds(0.2f);
        PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
#endif
    }
}
