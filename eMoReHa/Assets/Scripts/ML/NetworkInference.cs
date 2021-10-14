using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;

public class NetworkInference:MonoBehaviour
{
    public List<string> modelNames=new List<string>();
    public List<NNModel> modelAssets=new List<NNModel>();

    private Dictionary<string, ModelConfig> _modelConfigs = new Dictionary<string, ModelConfig>();

    private List<Model> _nn_RuntimeModels = new List<Model>();
    private List<IWorker> _workers = new List<IWorker>();
    private ModelConfigsLoader _modelConfigLoader = new ModelConfigsLoader();

    void Start()
    {
        for (int i = 0; i < modelAssets.Count; i++)
        {
            _nn_RuntimeModels.Add(ModelLoader.Load(modelAssets[i]));
            _workers.Add(WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, _nn_RuntimeModels[i]));
            _modelConfigs.Add(modelNames[i], _modelConfigLoader.GetConfig(modelNames[i]));

        }


    }
    public ModelConfig GetModelConfig(string Name)
    {
        return _modelConfigs[Name];
    }
    public List<IWorker> GetWorkers()
    {
        return _workers;
    }
    public int CountModels()
    {
        return modelAssets.Count;
    }
}
