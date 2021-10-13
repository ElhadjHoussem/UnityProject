using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Preprocess1 : MonoBehaviour
{
    RenderTexture renderTexture;
    Vector2 scale = new Vector2(1, 1);
    Vector2 offset = Vector2.zero;
    UnityAction<byte[]> callback;


    public void  ScaleAndCropImage(WebCamTexture texture, int desiredSize_width, int desdesiredSize_height, UnityAction<byte[]> callback)
    {

        this.callback = callback;


        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize_width, desdesiredSize_height, 0, RenderTextureFormat.ARGB32);
        }

        scale.x = (float)texture.height / (float)texture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(texture, renderTexture, scale, offset);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {

        if (request.hasError)
        {
            Debug.Log("GPU readback error detected.");
            return;
        }

        callback.Invoke(request.GetData<byte>().ToArray());

    }
}