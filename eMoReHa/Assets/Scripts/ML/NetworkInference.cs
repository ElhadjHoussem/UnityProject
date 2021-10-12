using UnityEngine;
using Unity.Barracuda;


public class NetworkInference
{
    public NNModel modelAsset;
    private Model nn_RuntimeModel;
    private IWorker worker;

    void Start()
    {
        nn_RuntimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, nn_RuntimeModel);

    }

    public IWorker getWorker()
    {
        return worker;
    }
}
