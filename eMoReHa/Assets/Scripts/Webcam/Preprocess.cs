using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Preprocess : MonoBehaviour {

    RenderTexture renderTexture;
    Vector2 scale = new Vector2(1, 1);
    Vector2 offset = Vector2.zero;

    UnityAction<byte[]> callback_G;
    UnityAction<byte[]> callback_E;
    UnityAction<byte[]> callback;



    public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize, UnityAction<byte[]> callback)
    {

        this.callback = callback;

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize, desiredSize, 0, RenderTextureFormat.ARGB32);
        }

        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(webCamTexture, renderTexture, scale, offset);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
    }

    public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize_width,int desdesiredSize_height, UnityAction<byte[]> callback)
    {

        this.callback = callback;

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize_width, desdesiredSize_height, 0, RenderTextureFormat.ARGB32);
        }

        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(webCamTexture, renderTexture, scale, offset);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
    }

    public void ScaleAndCropImage(WebCamTexture webCamTexture, int desiredSize_width, int desdesiredSize_height, UnityAction<byte[]> callback, int callback_index)
    { 
   
        if (callback_index == 0)
        {
            this.callback_E = callback;

        }
        else
        {
            this.callback_G = callback;
        }




        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize_width, desdesiredSize_height, 0, RenderTextureFormat.ARGB32);
        }

        scale.x = (float)webCamTexture.height / (float)webCamTexture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(webCamTexture, renderTexture, scale, offset);
        if (callback_index == 0)
        {
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadbackEmotion);

        }
        else
        {
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadbackGesture);
        }
    }


    void OnCompleteReadbackEmotion(AsyncGPUReadbackRequest request) {

        if (request.hasError) {
            Debug.Log("GPU readback error detected.");
            return;
        }

        this.callback_E.Invoke(request.GetData<byte>().ToArray());
    }
    void OnCompleteReadbackGesture(AsyncGPUReadbackRequest request)
    {

        if (request.hasError)
        {
            Debug.Log("GPU readback error detected.");
            return;
        }

        this.callback_G.Invoke(request.GetData<byte>().ToArray());
    }
    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {

        if (request.hasError)
        {
            Debug.Log("GPU readback error detected.");
            return;
        }

        this.callback.Invoke(request.GetData<byte>().ToArray());

    }

}