using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class resolutionsSetting : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    private List<Resolution> resol;
    private int nowWidth;
    private int nowHeight;
    [SerializeField] private AudioMixer audioMixer;
    public void Start()
    {
        resol = new List<Resolution>();
        resol.AddRange(Screen.resolutions);

        dropdown.ClearOptions();

        List<string> options = new List<string>();
        int crindex = 0;

        for (int i = 0; i < resol.Count; i++) 
        { 
            string option = resol[i].width + "x" + resol[i].height + " (" + resol[i].refreshRateRatio.value.ToString("F0") + "Hz)";
            options.Add(option);
            if (resol[i].width == Screen.currentResolution.width && resol[i].height == Screen.currentResolution.height
                && Mathf.Approximately((float)resol[i].refreshRateRatio.value, (float)Screen.currentResolution.refreshRateRatio.value))
            {
                crindex = i;
            }
        }
        dropdown.AddOptions(options);
        dropdown.value = crindex;
        dropdown.RefreshShownValue();
    }

    public void SetResolution(int crindex)
    {
        Resolution res = resol[crindex];
        nowWidth = res.width;
        nowHeight = res.height;
        Screen.SetResolution(res.width, res.height, false);
    }
    public void SetBorderless()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
    }
    public void SetFullScreen()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
    }
    public void SetWindow()
    {
        if (nowWidth == 0 || nowHeight == 0)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed);
        }
        else
        {
            Screen.SetResolution(nowWidth, nowHeight, FullScreenMode.Windowed);
        }
    }
    public void SetMaster(float vo)
    {
        float dbV;
        if(vo <= 0)
        {
            dbV = -80f;
        }
        else
        {
            dbV = Mathf.Log10(vo) * 20;
        }
        audioMixer.SetFloat("Master", dbV);
    }
    public void SetBGM(float vo)
    {
        float dbV;
        if (vo <= 0)
        {
            dbV = -80f;
        }
        else
        {
            dbV = Mathf.Log10(vo) * 20;
        }
        audioMixer.SetFloat("BGM", dbV);
    }
    public void SetSFX(float vo)
    {
        float dbV;
        if (vo <= 0)
        {
            dbV = -80f;
        }
        else
        {
            dbV = Mathf.Log10(vo) * 20;
        }
        audioMixer.SetFloat("SFX", dbV);
    }
}
