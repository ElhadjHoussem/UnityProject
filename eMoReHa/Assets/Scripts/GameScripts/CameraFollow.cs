using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float SmoothSpeed=0.0001f;
    public Vector3 offset_z = new Vector3(0, 0, -10);
    public Vector3 offset_boundery = new Vector3(2, 2, 0);
    public Transform Player;

    private void LateUpdate()
    {
        Vector3 TargetPositionInScene = Camera.main.ScreenToWorldPoint(Player.position);
        Vector3 TargetPosition = Player.position;
        TargetPosition.z = -10;// Camera.main.transform.position.z;
        transform.position = Vector3.Lerp(transform.position, TargetPosition, SmoothSpeed);
        //transform.position = Vector3.MoveTowards(transform.position, TargetPosition, 3 * Time.deltaTime / transform.localScale.x);



    }

    private bool PlayerOnEdgeBoundery(Vector3 targetPositionInScene)
    {
        
        Vector3 TopRightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        Vector3 ButtomLeftEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z));

        if (
            targetPositionInScene.x >= TopRightEdge.x ||
            targetPositionInScene.x <= ButtomLeftEdge.x ||
            targetPositionInScene.y >= TopRightEdge.y ||
            targetPositionInScene.y <= ButtomLeftEdge.y
          )
        {
            print((TopRightEdge, ButtomLeftEdge, targetPositionInScene));
            print(("targetPositionInScene.x > TopRightEdge.x  ", targetPositionInScene.x > TopRightEdge.x));
            print(("targetPositionInScene.x < ButtomLeftEdge.x", targetPositionInScene.x < ButtomLeftEdge.x));
            print(("targetPositionInScene.y > TopRightEdge.y", targetPositionInScene.y > TopRightEdge.y));
            print(("targetPositionInScene.y < ButtomLeftEdge.y", targetPositionInScene.y < ButtomLeftEdge.y));
            return true;

        }
        else return false;

    }
}
