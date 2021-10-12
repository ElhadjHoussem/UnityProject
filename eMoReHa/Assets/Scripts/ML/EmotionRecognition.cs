using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class EmotionRecognition : MonoBehaviour
{


    // Model Parameters
    const int IMAGE_SIZE = 64;
    const string INPUT_NAME = "Input3";
    const string OUTPUT_NAME = "Plus692_Output_0";
    readonly List<string> OutputLabels = new List<string>() { 
        "neutral", 
        "happiness", 
        "surprise", 
        "sadness", 
        "anger", 
        "disgust", 
        "fear", 
        "contempt" 
    };


    public WebcamFeed _WebcamFeed;
    public Preprocess _Preprocess;
    public Text uiText;
    public NNInference _NNInference;

    private IWorker worker;


    // Start is called before the first frame update
    void Start()
    {
        worker = _NNInference.getWorker();
        
    }

    // Update is called once per frame
    void Update()
    {
        WebCamTexture webCamTexture = _WebcamFeed.GetCamImage();

        if (webCamTexture.didUpdateThisFrame && webCamTexture.width > 100)
        {
            _Preprocess.ScaleAndCropImage(webCamTexture, IMAGE_SIZE, RunModel);
        }

    }

    void RunModel(byte[] pixels)
    {
        StartCoroutine(RunModelRoutine(pixels));
    }
    IEnumerator RunModelRoutine(byte[] pixels)
    {
        Tensor tensor = TransformInput(pixels);

        var inputs = new Dictionary<string, Tensor> {
            { INPUT_NAME, tensor }
        };

        worker.Execute(inputs);
        Tensor outputTensor = worker.PeekOutput(OUTPUT_NAME);

        //get largest output
        List<float> temp = outputTensor.ToReadOnlyArray().ToList();
        float max = temp.Max();
        int index = temp.IndexOf(max);

        //set UI text
        uiText.text = OutputLabels[index];

        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
        yield return null;
    }
    //This model requires a single channel greyscale from 0-255
    Tensor TransformInput(byte[] pixels)
    {
        float[] singleChannel = new float[IMAGE_SIZE * IMAGE_SIZE];
        for (int i = 0; i < singleChannel.Length; i++)
        {
            Color color = new Color32(pixels[i * 3 + 0], pixels[i * 3 + 1], pixels[i * 3 + 2], 255);
            singleChannel[i] = color.grayscale * 255;
        }
        return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 1, singleChannel);
    }
}
