// #define ANALYTICS_SDKS

using UnityEngine;
using UnityEngine.SceneManagement;

#if ANALYTICS_SDKS
using GameAnalyticsSDK;
#endif

public class Initializer : MonoBehaviour
{
#if ANALYTICS_SDKS
    const float SecondsToWait = 2f;

    float timer = 0f;

    void Start()
    {
        GameAnalytics.Initialize();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (GameAnalytics.IsRemoteConfigsReady() || timer >= SecondsToWait)
        {
            print("GA:IsRemoteConfigsReady: " + GameAnalytics.IsRemoteConfigsReady());
            print("GA:GetRemoteConfigsContentAsString: " + GameAnalytics.GetRemoteConfigsContentAsString());

            // ABTestManager.Instance.Init();

            SceneManager.LoadScene("Main");
        }
    }
#endif
}