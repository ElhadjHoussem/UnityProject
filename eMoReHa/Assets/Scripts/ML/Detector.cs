using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System;

public class Detector : MonoBehaviour
{
    #region public fields
    public Preprocess1 preprocess;
    public WebCamFeed1 webcamFeed;
    public List<Text> uiText;
    public NetworkInference nnInference;
    #endregion public


    #region Models Configuration
    private ModelConfig _ConfigGesture = new ModelConfig(
    "Gesture",
    320,//image width
    120,// image Height
    "conv2d_1_input", //input layer Name
    "dense_2", // output layer Name
    new List<string>() { "down", "palm", "l", "fist", "fist_moved", "thumb", "index", "ok", "palm_moved", "c" }// labels dict
    );

    private ModelConfig _ConfigEmotion = new ModelConfig(
        "Emotion",
        64,//image width
        64,// image Height
        "Input3",//input layer Name
        "Plus692_Output_0",// output layer Name
        new List<string>() { "neutral", "happiness", "surprise", "sadness", "anger", "disgust", "fear", "contempt" }// label dict
        );
    #endregion Models Configuration

    #region private fields
    private WebCamTexture[] _webCamTextures;
    private ModelConfig[] _modelConfigs;
    private List<IWorker> _workers;
    private int model_index = 0;
    private int[] color_transform = new int[]{255,1};
    #endregion private fields


    // Start is called before the first frame update
    void Start()
    {
        _webCamTextures = new WebCamTexture[webcamFeed.CountDisplays()];
        _modelConfigs = new ModelConfig[] { _ConfigEmotion, _ConfigGesture };
        _workers = nnInference.getWorkers();

    }

    // Update is called once per frame
    void Update()
    {


        for (int i = 0; i < webcamFeed.CountDisplays(); i++)
        {
            model_index = 0;
            _webCamTextures[model_index] = webcamFeed.GetCamImage(model_index);
            if (_webCamTextures[model_index].didUpdateThisFrame && _webCamTextures[model_index].width > 100)
                preprocess.ScaleAndCropImage(_webCamTextures[model_index], _modelConfigs[model_index].getImageWidth(), _modelConfigs[model_index].getImageHeight(),RunModel) ;
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
            { _modelConfigs[model_index].getInputName(), tensor }
        };

        _workers[model_index].Execute(inputs);
        Tensor outputTensor = _workers[model_index].PeekOutput(_modelConfigs[model_index].getOutputName());

        // get max prediction
        List<float> temp = outputTensor.ToReadOnlyArray().ToList();
        float max = temp.Max();
        int index = temp.IndexOf(max);

        //set UI text
        uiText[model_index].text = _modelConfigs[model_index].getLabels()[index];

        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
        yield return null;
    }
    Tensor TransformInput(byte[] pixels)
    {
        print(model_index + "  " + _modelConfigs[model_index].getName());
        float[] singleChannel = new float[_modelConfigs[model_index].getImageWidth() * _modelConfigs[model_index].getImageHeight()];
        for (int i = 0; i < singleChannel.Length; i++)
        {
            Color color = new Color32(pixels[i * 3 + 0], pixels[i * 3 + 1], pixels[i * 3 + 2], 255);
            singleChannel[i] = color.grayscale * color_transform[model_index];
        }
        return new Tensor(1, _modelConfigs[model_index].getImageWidth(), _modelConfigs[model_index].getImageHeight(), 1, singleChannel);
    }
    private void OnDestroy()
    {
        for (int i = 0; i < _workers.Count; i++)
        {
            _workers[i]?.Dispose();

        }
    }
}
