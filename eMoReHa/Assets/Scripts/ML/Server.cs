using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System;
using System.Threading;

public class Server : MonoBehaviour
{

    #region public fields
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
    public Preprocessing[] _preprocessors;
    private WebCamTexture[] _webCamTextures;
    private ModelConfig[] _modelConfigs;
    private List<IWorker> _workers;
    private int model_index = 0;
    private int[] color_transform = new int[] { 255, 1 };
    #endregion private fields



    private void Awake()
    {

        _webCamTextures = new WebCamTexture[webcamFeed.CountDisplays()];
        _modelConfigs = new ModelConfig[] { _ConfigEmotion, _ConfigGesture };
        _preprocessors = new Preprocessing[nnInference.CountModels()];
        _workers = nnInference.getWorkers();
    }

    void Start()
    {
        _preprocessors = Enumerable.Range(0, nnInference.CountModels()).Select(i => new Preprocessing(i)).ToArray();

    }


    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < webcamFeed.CountDisplays(); i++)
        {
            model_index = i;
            _webCamTextures[model_index] = webcamFeed.GetCamImage(model_index);
            if (_webCamTextures[model_index].didUpdateThisFrame && _webCamTextures[model_index].width > 100)
            {

                _preprocessors[model_index].ScaleAndCropImage(_webCamTextures[model_index], _modelConfigs[model_index].getImageWidth(), _modelConfigs[model_index].getImageHeight());

            }
            if (!_preprocessors[model_index].isQueueEmpty())
            {
                byte[] pixels = _preprocessors[model_index].Dequeue();
                RunModel(pixels, model_index);
            }


        }


    }



    void RunModel(byte[] pixels,int model_index)
    {
        StartCoroutine(RunModelRoutine(pixels, model_index));
    }
    IEnumerator RunModelRoutine(byte[] pixels,int model_index)
    {
        Tensor tensor = TransformInput(pixels, model_index);

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
    Tensor TransformInput(byte[] pixels,int model_index)
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
        for (int i = 0; i < _preprocessors.Length; i++)
        {
            _preprocessors[i].OnDestroy();

        }

    }

    private void RunModelNoRoutine(byte[] pixels, int model_index)
    {
        Tensor tensor = TransformInput(pixels, model_index);

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
        setUiText(_modelConfigs[model_index].getLabels()[index], model_index);
        //uiText[model_index].text = _modelConfigs[model_index].getLabels()[index];

        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
    }

    private void setUiText(string text, int model_index)
    {
        uiText[model_index].text = text;
    }

}
