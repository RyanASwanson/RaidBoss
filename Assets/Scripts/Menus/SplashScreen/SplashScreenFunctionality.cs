using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SplashScreenFunctionality : MonoBehaviour
{
    [SerializeField] private float _startingDelay;
    [SerializeField] private float _displayBufferTime;
    private WaitForSeconds _displayBufferWait;
    
    [SerializeField] private SplashScreenDisplay[] _splashScreenDisplays;
    
    // Start is called before the first frame update
    void Start()
    {
        StartSplashScreenProcess();
    }

    private void StartSplashScreenProcess()
    {
        StartCoroutine(SplashScreenProcess());
    }

    private IEnumerator SplashScreenProcess()
    {
        yield return new WaitForSeconds(_startingDelay);
        float fadeProgress = 0;
        
        for (int i = 0; i < _splashScreenDisplays.Length; i++)
        {
            _splashScreenDisplays[i].DisplayEvent?.Invoke();

            fadeProgress = 0;

            while (fadeProgress < 1)
            {
                fadeProgress += Time.deltaTime / _splashScreenDisplays[i].FadeInDuration;
                _splashScreenDisplays[i].DisplayCanvasGroup.alpha = fadeProgress;
                yield return null;
            }
            
            _splashScreenDisplays[i].DisplayCanvasGroup.alpha = 1;
            
            yield return new WaitForSeconds(_splashScreenDisplays[i].GetTimeSpentNotFading());
            
            while (fadeProgress > 0)
            {
                fadeProgress -= Time.deltaTime / _splashScreenDisplays[i].FadeOutDuration;
                _splashScreenDisplays[i].DisplayCanvasGroup.alpha = fadeProgress;
                yield return null;
            }

            _splashScreenDisplays[i].DisplayCanvasGroup.alpha = 0;
        }

        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneLoadManager.Instance.LoadSceneByEnum(ELoadableScenes.MainMenu);
    }
}

[System.Serializable]
public class SplashScreenDisplay
{
    public float DisplayDuration;
    public float FadeInDuration;
    public float FadeOutDuration;
    
    public CanvasGroup DisplayCanvasGroup;

    public UnityEvent DisplayEvent;
    
    public float GetTimeSpentNotFading() => DisplayDuration - (FadeInDuration + FadeOutDuration);
}