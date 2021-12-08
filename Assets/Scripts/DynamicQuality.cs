using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicQuality : MonoBehaviour
{
    public Color[] colors;
    public GameObject[] disableOnLowSettings;

    public float lowerThreshold;
    public float upperThreshold;
    public int tokenDelay;

    public float currentTime;
    int frameCount;

    public Text text;
    public bool monitorFPSOnly;
    private void Start()
    {
        QualitySettings.SetQualityLevel(1, true);
        text.color = colors[QualitySettings.GetQualityLevel()];
        StartCoroutine(FPSCounter());
    }

    void Update()
    {
        frameCount++;
    }
    IEnumerator FPSCounter()
    {
        int token = 0;
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            text.text = frameCount.ToString();
            if (frameCount < lowerThreshold || frameCount > upperThreshold)
                token++;
            else
                token = 0;
            if (token >= tokenDelay && !monitorFPSOnly)
            {
                token = 0;
                if (frameCount < lowerThreshold)
                    QualitySettings.DecreaseLevel(true);
                else
                    QualitySettings.IncreaseLevel(true);
                var i = QualitySettings.GetQualityLevel();
                text.color = colors[i];
                if (i == 0)
                    foreach (var j in disableOnLowSettings)
                        j.SetActive(false);
                else
                    foreach (var j in disableOnLowSettings)
                        j.SetActive(true);

            }
            frameCount = 0;
        }
    }
}
