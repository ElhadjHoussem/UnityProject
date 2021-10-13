using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamFeed : MonoBehaviour
{
    public int _Device = 0;
    public RawImage _Display;
    WebCamTexture _WebCamTexture;
    AspectRatioFitter _Fitter;
    bool _RatioSet;

    void Start()
    {
        _Fitter = GetComponent<AspectRatioFitter>();
        InitWebCam();
    }

    // Update is called once per frame
    void Update()
    {
        if (_WebCamTexture.width > 100 && !_RatioSet)
        {
            _RatioSet = true;
            SetAspectRatio();
        }

    }

    private void SetAspectRatio()
    {
        _Fitter.aspectRatio = (float)_WebCamTexture.width / (float)_WebCamTexture.height;
    }
    void InitWebCam()
    {
        string WebCamName = WebCamTexture.devices[_Device].name;
        _WebCamTexture = new WebCamTexture(WebCamName, Screen.width, Screen.height, 30);

        Display();
        _WebCamTexture.Play();
    }
    private void Display()
    {
        _Display.texture = _WebCamTexture;
    }
    public WebCamTexture GetCamImage()
    {
        return _WebCamTexture;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}
