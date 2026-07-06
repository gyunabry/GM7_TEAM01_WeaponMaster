using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class resolutionsSetting : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    private List<Resolution> resol;
    private int nowWidth;
    private int nowHeight;
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider MasterS;
    [SerializeField] private Slider BGMS;
    [SerializeField] private Slider SFXS;

    enum ScreenFull {border = 0, full, window};
    private void Awake()
    {
        LoadSet();
    }
    private void OnEnable()
    {
        float volMaster = PlayerPrefs.GetFloat("MasterV", 1.0f);
        float volBGM = PlayerPrefs.GetFloat("BGMV", 1.0f);
        float volSFX = PlayerPrefs.GetFloat("SFXV", 1.0f);
        int isFull = PlayerPrefs.GetInt("FullScreen", 0);
        MasterS.value = volMaster;
        BGMS.value = volBGM;
        SFXS.value = volSFX;
        if(isFull == 0)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else if(isFull == 1)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
        }
        else if(isFull == 2)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed);
        }
    }
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
        PlayerPrefs.SetInt("FullScreen", (int)ScreenFull.border);
        PlayerPrefs.Save();
    }
    public void SetFullScreen()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
        PlayerPrefs.SetInt("FullScreen", (int)ScreenFull.full);
        PlayerPrefs.Save();
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
        PlayerPrefs.SetInt("FullScreen", (int)ScreenFull.window);
        PlayerPrefs.Save();
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
        PlayerPrefs.SetFloat("MasterV", vo);
        PlayerPrefs.Save();
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
        PlayerPrefs.SetFloat("BGMV", vo);
        PlayerPrefs.Save();
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
        PlayerPrefs.SetFloat("SFXV", vo);
        PlayerPrefs.Save();
    }
    public void LoadSet()
    {
        float volMaster = PlayerPrefs.GetFloat("MasterV", 1.0f);
        float volBGM = PlayerPrefs.GetFloat("BGMV", 1.0f);
        float volSFX = PlayerPrefs.GetFloat("SFXV", 1.0f);
        bool isFull = PlayerPrefs.GetInt("FullScreen", 1) == 1;

        audioMixer.SetFloat("Master", volMaster);
        audioMixer.SetFloat("BGM", volBGM);
        audioMixer.SetFloat("SFX", volSFX);
    }
}
