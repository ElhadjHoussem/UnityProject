using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ModelConfig 
{
    private int ImageWidth;
    private int ImageHeight;
    private string InputName;
    private string OutputName;
    private List<string> OutputLabels;

    public ModelConfig(int imageWidth, int imageHeight, string inputName, string outputName, List<string> labels)
    {
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
        InputName = inputName;
        OutputName = outputName;
        OutputLabels = labels;

    }
    public int getImageWidth()
    {
        return ImageWidth;
    }
    public int getImageHeight()
    {
        return ImageHeight;
    }
    public string getInputName()
    {
        return InputName;
    }
    public string getOutputName()
    {
        return OutputName;
    }
    public List<string> getLabels()
    {
        return OutputLabels;
    }
}
