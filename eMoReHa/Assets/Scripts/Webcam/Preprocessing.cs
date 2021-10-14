using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

public class Preprocessing
{
    int index;
    RenderTexture renderTexture;
    Vector2 scale = new Vector2(1, 1);
    Vector2 offset = Vector2.zero;
    
    Queue<byte[]> obsQueue= new Queue<byte[]>();
    public Preprocessing(int index)
    {
        this.index = index;
    }
    public byte[] Dequeue()
    {
        return obsQueue.Dequeue();
    }
    public bool isQueueEmpty()
    {
        return !obsQueue.Any();
    }

    public void ScaleAndCropImage(WebCamTexture texture, int desiredSize_width, int desdesiredSize_height)
    {

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(desiredSize_width, desdesiredSize_height, 0, RenderTextureFormat.ARGB32);
        }

        scale.x = (float)texture.height / (float)texture.width;
        offset.x = (1 - scale.x) / 2f;
        Graphics.Blit(texture, renderTexture, scale, offset);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, OnCompleteReadback);

    }

    private void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {

        if (request.hasError)
        {
            Debug.Log("GPU readback error detected.");
            return;
        }

        obsQueue.Enqueue(request.GetData<byte>().ToArray());
    }
    public void OnDestroy()
    {
        obsQueue.Clear();
    }
}
