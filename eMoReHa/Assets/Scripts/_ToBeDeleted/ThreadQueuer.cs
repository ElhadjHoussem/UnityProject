using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
public class ThreadQueuer : MonoBehaviour
{

 
    List<Action> functionsToRunInMainThread;

    public delegate void SameThingAsAction();


    void Start()
    {
        Debug.Log("start:: starting");
        functionsToRunInMainThread = new List<Action>();
        StratThreadedFunction(SlowFunctionThatDoesAUnityThing);
        Debug.Log("start:: done");

    }
    private void Update()
    {
        while (functionsToRunInMainThread.Count > 0)
        {
            Action someFunc = functionsToRunInMainThread[0];
            
            functionsToRunInMainThread.RemoveAt(0);

            someFunc();
        }
    }
    // public void StratThreadedFunctionWithDelegate(SameThingAsAction someFuntion) {} 
    public void StratThreadedFunction(Action someFuntion) //Action is a short form of a delegate for function signature <genereic> Action<int>
    {
        Thread T = new Thread(new ThreadStart( someFuntion ));
        T.Start();
    }
    public void QueueMainThreadFunction(Action someFunction)
    {
        functionsToRunInMainThread.Add(someFunction);
    }
    void SlowFunctionThatDoesAUnityThing()
    {
        Thread.Sleep(2000); // this does involve unity game objects
        Action aFunction = () =>
        {
            Debug.Log("The result of child Thread are being applied to unity game object safely");
            this.transform.position = new Vector3(1, 1, 1); // not allowed in child Thread

        };
        //aFunction(); // this call is not allowed in child Thread only in main thread; because it handles unity game objects
        QueueMainThreadFunction(aFunction);  // instead we can queue this portion of the complete funtion to be executed only from the main thread
    }

}
