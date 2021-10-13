using UnityEngine;
using Unity.Barracuda;
using System.Collections.Generic;

public class NetworkInference:MonoBehaviour
{
    public List<NNModel> modelAssets=new List<NNModel>();
    private List<Model> nn_RuntimeModels = new List<Model>();
    private List<IWorker> workers = new List<IWorker>();

    void Start()
    {
        for (int i = 0; i < modelAssets.Count; i++)
        {
            nn_RuntimeModels.Add(ModelLoader.Load(modelAssets[i]));
            workers.Add(WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, nn_RuntimeModels[i]));
        }
  


    }

    public List<IWorker> getWorkers()
    {
        return workers;
    }
    public int CountModels()
    {
        return modelAssets.Count;
    }
}
