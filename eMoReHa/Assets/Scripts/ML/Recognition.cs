using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
public class Recognition : MonoBehaviour
{

    private ModelConfig ConfigGesture = new ModelConfig(
        320,//image width
        120,// image Height
        "conv2d_1_input", //input layer Name
        "dense_2", // output layer Name
        new List<string>() { "down", "palm", "l", "fist", "fist_moved", "thumb", "index", "ok", "palm_moved", "c"}// labels dict
        );

    private ModelConfig ConfigEmotion = new ModelConfig(
        64,//image width
        64,// image Height
        "Input3",//input layer Name
        "Plus692_Output_0",// output layer Name
        new List<string>() { "neutral", "happiness", "surprise", "sadness", "anger", "disgust", "fear", "contempt" }// label dict
        );


    public WebcamFeed _WebcamFeed;
    public Preprocess _Preprocess;


    public Text uiTextEmotions;
    public Text uiTextGesture;

    // Inferences of Network_MODELs

    public NNInference _NNInference_Emotion;
    public NNInference _NNInference_Gesture;


    // prediction Threads  
    private IWorker _worker_Emotion;
    private IWorker _worker_Gesture;

    // webcam Capture instance
    private WebCamTexture _webCamTexture;



    // Start is called before the first frame update
    void Start()
    {
        _worker_Emotion = _NNInference_Emotion.getWorker();
       //_worker_Gesture = _NNInference_Gesture.getWorker();
    }

    // Update is called once per frame
    private void Update()
    {
        _webCamTexture = _WebcamFeed.GetCamImage();

        if (_webCamTexture.didUpdateThisFrame && _webCamTexture.width > 100)
        {

            _Preprocess.ScaleAndCropImage(_webCamTexture, ConfigEmotion.getImageWidth(), ConfigEmotion.getImageHeight(), RunModelEmotion, 0);
            //_Preprocess.ScaleAndCropImage(_webCamTexture, ConfigGesture.getImageWidth(), ConfigGesture.getImageHeight(), RunModelGesture,1);

        }

    }


    void RunModelEmotion(byte[] pixels)
    {
        StartCoroutine(RunModelRoutineEmotion(pixels));
    }
    IEnumerator RunModelRoutineEmotion(byte[] pixels)
    {
        Tensor tensor = TransformInputEmotion(pixels);

        var inputs = new Dictionary<string, Tensor> {
            { ConfigEmotion.getInputName(), tensor }
        };

        _worker_Emotion.Execute(inputs);
        Tensor outputTensor = _worker_Emotion.PeekOutput(ConfigEmotion.getOutputName());

        //get largest output
        List<float> temp = outputTensor.ToReadOnlyArray().ToList();
        float max = temp.Max();
        int index = temp.IndexOf(max);

        //set UI text
        uiTextEmotions.text = ConfigEmotion.getLabels()[index];

        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
        yield return null;
    }
    //This model requires a single channel greyscale from 0-255
    Tensor TransformInputEmotion(byte[] pixels)
    {
        float[] singleChannel = new float[ConfigEmotion.getImageWidth() * ConfigEmotion.getImageHeight() ];
        for (int i = 0; i < singleChannel.Length; i++)
        {
            Color color = new Color32(pixels[i * 3 + 0], pixels[i * 3 + 1], pixels[i * 3 + 2], 255);
            singleChannel[i] = color.grayscale*255;
        }
        return new Tensor(1, ConfigEmotion.getImageWidth() , ConfigEmotion.getImageHeight(), 1, singleChannel);
    }


    void RunModelGesture(byte[] pixels)
    {
        StartCoroutine(RunModelRoutineGesture(pixels));
    }
    IEnumerator RunModelRoutineGesture(byte[] pixels)
    {
        Tensor tensor = TransformInputGesture(pixels);

        var inputs = new Dictionary<string, Tensor> {
            { ConfigGesture.getInputName(), tensor }
        };

        _worker_Gesture.Execute(inputs);


        Tensor outputTensor = _worker_Gesture.PeekOutput(ConfigGesture.getOutputName());

        Debug.Log(outputTensor);
        //get max output
        List<float> temp = outputTensor.ToReadOnlyArray().ToList();
        float max = temp.Max();
        int index = temp.IndexOf(max);

        //set UI text
        uiTextGesture.text = ConfigGesture.getLabels()[index];

        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
        yield return null;
    }
    //This model requires a single channel greyscale from 0-1
    Tensor TransformInputGesture(byte[] pixels)
    {
        float[] singleChannel = new float[ConfigGesture.getImageWidth() * ConfigGesture.getImageHeight()];
        for (int i = 0; i < singleChannel.Length; i++)
        {
            Color color = new Color32(pixels[i * 3 + 0], pixels[i * 3 + 1], pixels[i * 3 + 2], 255);
            singleChannel[i] = color.grayscale;
        }
        return new Tensor(1, ConfigGesture.getImageWidth() , ConfigGesture.getImageHeight(), 1, singleChannel);
    }

}

