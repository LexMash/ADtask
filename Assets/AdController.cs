using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections;

public class AdController : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private Button _showAdButton;
    [SerializeField] private Text _logText;
    [SerializeField] private bool _testMode = true;
    [SerializeField] private RectTransform _loadingImage;

    private string _gameId = "4778209";
    private string _adUnitId = "Rewarded_Android";

    private Coroutine _loading;

    private void Awake()
    {
        Advertisement.Initialize(_gameId, _testMode);
    }

    private void OnEnable()
    {
        _showAdButton.interactable = false;

        LoadAd();
    }

    public void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        _logText.text += $"Loading Ad:  + {_adUnitId} \n";
        Advertisement.Load(_adUnitId, this);

        _loadingImage.GetComponent<Image>().enabled = true;

        _loading = StartCoroutine(Loading());
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        _logText.text += $"Ad Loaded:  + {adUnitId} \n";

        _loadingImage.GetComponent<Image>().enabled = false;

        StopCoroutine(_loading);

        if (adUnitId.Equals(_adUnitId))
        {
            _showAdButton.onClick.AddListener(ShowAd);

            _showAdButton.interactable = true;
        }
    }

    public void ShowAd()
    {
        /*        if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Debug.Log("Error. Check internet connection!");
                    _logText.text += "Error. Check internet connection!\n";

                    _showAdButton.interactable = true;
                    return;
                }*/

        _showAdButton.interactable = false;

        _loadingImage.GetComponent<Image>().enabled = true;

        _loading = StartCoroutine(Loading());

        Advertisement.Show(_adUnitId, this);
    }

    private IEnumerator Loading()
    {
        var rotation = Quaternion.Euler(0f, 0f, 30f);

        var pause = new WaitForSeconds(0.2f);

        for(; ; )
        {
            _loadingImage.localRotation *= rotation;
            yield return pause;
        }
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            _logText.text += "Unity Ads Rewarded Ad Completed\n";

            Advertisement.Load(_adUnitId, this);
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        _logText.text += $"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}\n";
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        _logText.text += $"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}\n";
    }

    public void OnUnityAdsShowStart(string adUnitId) 
    {
    }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        _showAdButton.onClick.RemoveAllListeners();
    }
}
