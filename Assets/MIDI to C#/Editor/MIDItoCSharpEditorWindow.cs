using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEditor;
using UnityEngine;

public class MIDItoCSharpEditorWindow : EditorWindow
{
    private static MIDItoCSharpEditorWindow window;
    
    [MenuItem("Tools/MIDItoC#")]
    public static void Open()
    {
        window = GetWindow<MIDItoCSharpEditorWindow>("MIDItoC#");
    }

    private SerializedObject serializedObject;
    public string path;
    public List<NoteInfo> noteList;
    private Vector2 scrollPos;
    
    private void OnEnable()
    {
        serializedObject = new SerializedObject(window);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Load MIDI file"))
        {
            path = EditorUtility.OpenFilePanel("MIDI File", "Assets", "mid");
            noteList = GetNotesInfo(path).ToList();
        }

        if (!string.IsNullOrEmpty(path))
        {
            using var scrollview = new GUILayout.ScrollViewScope(scrollPos);
            scrollPos = scrollview.scrollPosition;
            
            GUILayout.Label($"Loaded MIDI File: {path}");
            GUILayout.Label($"Number of notes = {noteList.Count}");
            foreach (var noteInfo in noteList)
            {
                GUILayout.Label($"|\n    ProgramNumber: {noteInfo.ProgramNumber}\n    Time: {noteInfo.Time}\n    Length: {noteInfo.Length}\n    NoteNumber: {noteInfo.NoteNumber}");
            }
        }
    }

    
    [Serializable]
    public sealed class NoteInfo
    {
        public int? ProgramNumber { get; set; }
        public long Time { get; set; }
        public long Length { get; set; }
        public int NoteNumber { get; set; }
    }
 
    private static IEnumerable<NoteInfo> GetNotesInfo(string filePath)
    {
        var midiFile = MidiFile.Read(filePath);
        // build the program changes map
 
        var programChanges = new Dictionary<FourBitNumber, Dictionary<long, SevenBitNumber>>();
        foreach (var timedEvent in midiFile.GetTimedEvents())
        {

            var programChangeEvent = timedEvent.Event as ProgramChangeEvent;
            if (programChangeEvent == null)
                continue;
 
            var channel = programChangeEvent.Channel;
 
            Dictionary<long, SevenBitNumber> changes;
            if (!programChanges.TryGetValue(channel, out changes))
                programChanges.Add(channel, changes = new Dictionary<long, SevenBitNumber>());
 
            changes[timedEvent.Time] = programChangeEvent.ProgramNumber;
        }
 
        // collect notes info
 
        return midiFile.GetNotes()
            .Select(n => new NoteInfo
            {
                ProgramNumber = GetProgramNumber(n.Channel, n.Time, programChanges),
                Time = n.Time,
                Length = n.Length,
                NoteNumber = n.NoteNumber
            });
    }
 
    private static int? GetProgramNumber(FourBitNumber channel, long time, Dictionary<FourBitNumber, Dictionary<long, SevenBitNumber>> programChanges)
    {
        Dictionary<long, SevenBitNumber> changes;
        if (!programChanges.TryGetValue(channel, out changes))
            return null;
 
        var times = changes.Keys.Where(t => t <= time).ToArray();
        return times.Any()
            ? (int?)changes[times.Max()]
            : null;
    }
}
