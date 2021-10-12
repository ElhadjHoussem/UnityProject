using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadingExample : MonoBehaviour
{
    // Start is called before the first frame update
    bool isRunning = false;
    Thread myThread;
    public System.Diagnostics.Stopwatch sw_global;
    void Start()
    {
        sw_global = new System.Diagnostics.Stopwatch();
        sw_global.Start();
        Debug.Log("Example Script::SlowJob() - Starting");
        //slowJob();//single Thread

        //StartCoroutine(slowJobCouroutine());// with coroutine
        
        myThread = new Thread(slowJob); // using Thread
        //run slowJob in Thread
        myThread.Start();

        Debug.Log("Example Script::SlowJob() - Started");
    }

    // Update is called once per frame
    void Update()
    {
        //if (isRunning) {
        //    Debug.Log("Update :: SlowJob is Running global elapsed time : "+ sw_global.ElapsedMilliseconds / 1000f);
        //}
        if (myThread.IsAlive)
        {
            Debug.Log("Update :: SlowJob is Running global elapsed time : " + sw_global.ElapsedMilliseconds / 1000f);
        }
        else { sw_global.Stop(); }

    }
    private void FixedUpdate()
    {
        if (myThread.IsAlive)
        {

            Debug.Log("Fixed-Update:: global elapsed time : " + sw_global.ElapsedMilliseconds / 1000f);
        }
    }
    void slowJob()
    {
        isRunning = true;
        Debug.Log("Example Script::SlowJob() - Doing SUFF that takes 2 ms each -- ");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for(int i= 0; i<1000; i++)
        {
            this.transform.Translate(Vector3.up * 0.002f);

            Thread.Sleep(2);// sleep for 2 ms
        }
        sw.Stop();
        Debug.Log("Example Script::SlowJob -Done ! Elapsed Time: " + sw.ElapsedMilliseconds / 1000f);
        isRunning = false;
    }
    IEnumerator slowJobCouroutine()
    {
        isRunning = true;
        Debug.Log("Example Script::SlowJob() - Doing SUFF that takes 2 ms each -- ");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (int i = 0; i < 1000; i++)
        {

            //Thread.Sleep(2);// sleep for 2 ms
            Debug.Log("Example Script::SlowJob() - 2 ms task Done -- ");

            yield return null;
        }
        sw.Stop();
        Debug.Log("Example Script::SlowJob -Done ! Elapsed Time: " + sw.ElapsedMilliseconds / 1000f);
        isRunning = false;
    }
}
