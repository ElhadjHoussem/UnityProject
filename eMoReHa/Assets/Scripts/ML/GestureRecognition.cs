using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class GestureRecognition : MonoBehaviour
{

    // Model Parameters
    const int IMAGE_SIZE_width = 320;
    const int IMAGE_SIZE_height = 120;
    const string INPUT_NAME = "conv2d_1_input";
    const string OUTPUT_NAME = "dense_2";

    readonly List<string> OutputLabels = new List<string>() {
        "down", "palm", "l", "fist", "fist_moved", "thumb", "index", "ok", "palm_moved", "c"
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
        Debug.Log("worker Set");

    }

    // Update is called once per frame
    void Update()
    {
        WebCamTexture webCamTexture = _WebcamFeed.GetCamImage();

        if (webCamTexture.didUpdateThisFrame && webCamTexture.width > 100)
        {
            _Preprocess.ScaleAndCropImage(webCamTexture, IMAGE_SIZE_width, IMAGE_SIZE_height, RunModel);
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
    //This model requires a single channel greyscale from 0-1
    Tensor TransformInput(byte[] pixels)
    {
        float[] singleChannel = new float[IMAGE_SIZE_width * IMAGE_SIZE_height];
        for (int i = 0; i < singleChannel.Length; i++)
        {
            Color color = new Color32(pixels[i * 3 + 0], pixels[i * 3 + 1], pixels[i * 3 + 2], 255);
            singleChannel[i] = color.grayscale;
        }
        return new Tensor(1, IMAGE_SIZE_width, IMAGE_SIZE_height, 1, singleChannel);
    }
}
