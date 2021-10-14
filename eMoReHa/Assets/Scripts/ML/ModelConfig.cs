using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public struct ModelConfig 
{
    private string _name;
    private int _imageWidth;
    private int _imageHeight;
    private int _colorTransform;
    private string _inputName;
    private string _iutputName;
    private List<string> _outputLabels;

    public ModelConfig(string name,int imageWidth, int imageHeight, string inputName, string outputName, List<string> labels,int colorTransform)
    {
        _name = name;
        _imageWidth = imageWidth;
        _imageHeight = imageHeight;
        _inputName = inputName;
        _iutputName = outputName;
        _outputLabels = labels;
        _colorTransform = colorTransform;

    }
    public string getName()
    {
        return _name;
    }
    public int getImageWidth()
    {
        return _imageWidth;
    }
    public int getImageHeight()
    {
        return _imageHeight;
    }
    public int getColorTransform()
    {
        return _colorTransform;
    }
    public string getInputName()
    {
        return _inputName;
    }
    public string getOutputName()
    {
        return _iutputName;
    }
    public List<string> getLabels()
    {
        return _outputLabels;
    }
}
