using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;

public class MIDITests : MonoBehaviour
{
    private string file = @"C:\Users\Alberto\Documents\GitHub Repos\BIRP Tests\Assets\MIDI to C#\Alan Walker - Faded (Piano Cover Tutorial - Easy) (midi by Carlo Prato) (www.cprato.com).mid";
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("--INFO PER SONG--");
        MidiFile midiFile = MidiFile.Read(file);
        var tempoMap = midiFile.GetTempoMap();
        Debug.Log("duration: " + midiFile.GetDuration(TimeSpanType.Metric));
        foreach (var valueChange in tempoMap.TempoLine)
        {
            Debug.Log("bpm: " + Mathf.Round((float)valueChange.Value.BeatsPerMinute));
        }
        
        var note = midiFile.GetNotes().ToList()[0];
        Debug.Log("--INFO PER NOTE--");


        foreach (var propertyInfo in typeof(Note).GetProperties())
        {
            Debug.Log(propertyInfo.Name + ": " + propertyInfo.GetValue(note));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
