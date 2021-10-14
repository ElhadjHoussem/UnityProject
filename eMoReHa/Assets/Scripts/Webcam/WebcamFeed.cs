using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamFeed : MonoBehaviour
{
    public int _Device = 1;
   
    public List<RawImage> _Displays = new List<RawImage>();
    WebCamTexture _WebCamTexture;
    List<AspectRatioFitter>  _Fitters = new List<AspectRatioFitter>();
    List<bool> _RatioSet = new List<bool>() { false,false};

    void Start()
    {
        for (int i = 0; i < _Displays.Count; i++) 
        {
            _Fitters.Add( _Displays[i].GetComponent<AspectRatioFitter>());

        }
        InitWebCam();

    }

    void Update()
    {
        {
            if (_WebCamTexture.width > 100)
            {
                SetAspectRatio();
            }
        }


    }
    void InitWebCam()
    {
        string WebCamName = WebCamTexture.devices[_Device].name;
        _WebCamTexture = new WebCamTexture(WebCamName, Screen.width, Screen.height, 30);
        _WebCamTexture.Play();

        Display();


    }
    private void SetAspectRatio()
    {
        for (int i = 0; i < _Fitters.Count; i++)
        {
            if (!_RatioSet[i])
            {
                _RatioSet[i] = true;
                _Fitters[i].aspectRatio = (float)_WebCamTexture.width / (float)_WebCamTexture.height;
            }


        }


    }

    private void Display()
    {
        for (int i = 0; i < _Displays.Count; i++)
        {
            _Displays[i].texture = _WebCamTexture;

        }

    }
    private void Play()
    {
        _WebCamTexture.Play();
        print(_Displays[0].texture);
    }
    public WebCamTexture GetCamImage(int i)
    {
        return (WebCamTexture) _Displays[i].texture;
    }
    public int CountDisplays()
    {
        return _Displays.Count;
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
