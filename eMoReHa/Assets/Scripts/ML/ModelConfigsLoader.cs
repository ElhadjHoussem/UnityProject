using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConfigsLoader
{
    private readonly Dictionary<string, ModelConfig> _configDict=new Dictionary<string, ModelConfig>();
    


    public ModelConfigsLoader()
    {
        ModelConfig _ConfigGesture = new ModelConfig(
        "Gesture",
        320,//inut width
        120,// inut Height
        "conv2d_1_input", //input layer Name
        "dense_2", // output layer Name
        new List<string>() { "down", "palm", "l", "fist", "fist_moved", "thumb", "index", "ok", "palm_moved", "c" },// labels dict
        1
        );
        
        ModelConfig _ConfigEmotion = new ModelConfig(
        "Emotion",
        64,//inut width
        64,// inut Height
        "Input3",//input layer Name
        "Plus692_Output_0",// output layer Name
        new List<string>() { "neutral", "happiness", "surprise", "sadness", "anger", "disgust", "fear", "contempt" },// label dict
        255
        );

        _configDict.Add("Gesture", _ConfigGesture);
        _configDict.Add("Emotion", _ConfigEmotion);
    }

    public ModelConfig GetConfig(string Name)
    {
        return _configDict[Name];
    }
}
