using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testEventCreator : MonoBehaviour
{
    #region EventsRegion
    public event Action<string> MusicXEvent;

    #endregion

    public event Action<string> ey;
    
    void Start ( )
    {
        //Call the method for test the invoked event
        asss();
        ey?.Invoke("MusicX");
    }
    
    
    void Update()
    {
        
    }

    IEnumerator ass(string a = "not") 
    {
        MusicXEvent?.Invoke("MusicX"); 
        yield return null;
    }

    //asss is a method for test the event creator in methods with parameters
    void asss(string a = "", bool d = false, int i = 6)
    {
        
    }
}
