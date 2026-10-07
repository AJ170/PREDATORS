using UnityEngine;
using UnityEngine.PSVita;
using System.Globalization;
using System.Threading;
using System.Collections;
using System.IO;
using UnityEngine.Video;

public class ExampleRenderTexturePlayback : MonoBehaviour
{
    public string m_MoviePath;
    public RenderTexture m_RenderTexture;
    public GUISkin m_Skin;
	bool m_IsPlaying = false;

	private void Awake()
	{
		System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
		Application.targetFrameRate = 60;
		PlatformDependent.HandleIphoneKeyboard();
		PlatformDependent.SetScreenOrientation(false);
		PlatformDependent.HandleScreenDarken();
	}
	
	void Start()
    {
        PSVitaVideoPlayer.Init(m_RenderTexture);
        PSVitaVideoPlayer.Play(m_MoviePath, PSVitaVideoPlayer.Looping.Continuous, PSVitaVideoPlayer.Mode.RenderToTexture);
    }

    void OnPreRender()
    {
        PSVitaVideoPlayer.Update();
    }

    void OnGUI()
    {
        GUI.skin = m_Skin;
        GUILayout.BeginArea(new Rect(10,10,100,Screen.height));
        if (GUILayout.Button("Skip"))
        {
			if (m_IsPlaying)
			{
				PSVitaVideoPlayer.Stop();
				PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
			}
			else
			{
				PSVitaVideoPlayer.Stop();
				PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
			}
        }
        GUILayout.EndArea();
    }

	void OnMovieEvent(int eventID)
	{
		PSVitaVideoPlayer.MovieEvent movieEvent = (PSVitaVideoPlayer.MovieEvent)eventID;
		switch (movieEvent)
		{
			case PSVitaVideoPlayer.MovieEvent.PLAY:
				m_IsPlaying = true;
				break;

			case PSVitaVideoPlayer.MovieEvent.STOP:
				m_IsPlaying = false;
				PlatformDependent.LoadLevelWithLoadingScreen("MainMenu3D_iPad");
				break;
		}
	}
}
