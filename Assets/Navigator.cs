using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;

public class Navigator : MonoBehaviour
{
    public NavMeshData data;

    public Transform target;

    private NavMeshPath path;

    private Thread navMeshThread;
    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();


        navMeshThread = new Thread(MyCalculatePath) {IsBackground = true};

        navMeshThread.Start( new NavMeshThreadParameters(transform.position, target.position));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        
    }

    private void Update()
    {
        if (!navMeshThread.IsAlive)
        {
            navMeshThread.Start();
        }
        // NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        transform.position += (path.corners[1] - transform.position).normalized * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = 0; i < path.corners.Length; i++)
            {
                var x = i / (float) path.corners.Length;
                Gizmos.color = new Color(x,x,x,1);
                if (i != path.corners.Length - 1) Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                Gizmos.DrawWireSphere(path.corners[i], .5f);
            }
        }
    }


    void MyCalculatePath(object parameters)
    {
        var position = ((NavMeshThreadParameters) parameters).position;
        var targetPosition = ((NavMeshThreadParameters) parameters).targetPosition;
         
        NavMesh.CalculatePath(position, targetPosition, NavMesh.AllAreas, path);
    }
    
    
    
    
}

public class NavMeshThreadParameters
{
    public Vector3 position;
    public Vector3 targetPosition;

    public NavMeshThreadParameters(Vector3 position, Vector3 targetPosition)
    {
        this.position = position;
        this.targetPosition = targetPosition;
    }
}
