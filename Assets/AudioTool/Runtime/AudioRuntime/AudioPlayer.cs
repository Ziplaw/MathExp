using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AudioEngine
{
    public class AudioPlayer : MonoBehaviour
    {
        //Bool for subscribe all the events founded on the scripts attached to the object with Action<string> structure
        public bool selectAllEvents;
        //Bool for open or close the settings window
        public bool configuration;
        //Bool for change to the automatic event creator implementation
        public bool automatic;

        /// <summary>
        /// <para>These fields are for get the reference of all the events attached to the actual gameobject and subscribe them to the play method</para>
        /// </summary>
        //The names of the events
        public List<string> allEventsNames = new List<String>();
        //The type of the events
        public List<string> allEventsTypes = new List<String>();
        //Bool for prevent the tool to subscribe the events more than once
        public bool allEventsSetted;
        
        
        //The list for get reference of each audio event 
        public List<AudioEvent> audioEvent = new List<AudioEvent>();
        //The list for configure the event, and create them
        public List<EventCreator> eventCreator = new List<EventCreator>();

        //Event creation
        //The serialize class for get all the fields that you need to create and subscribe the event to the play method
        [System.Serializable]
        public class EventCreator
        {
            public MonoBehaviour selectedScript;
            public MethodInfo selectedMethod;
            public int methodIndex;
            public string selectedTrack;

            public bool autoName = true;
            public string eventName;

            public string eventType;
        }

        //Audio Event
        //The serialize class for get all the fields that you need subscribe the event to the play method
        [System.Serializable]
        public class AudioEvent
        {
            //The index of the event selected at the script
            public int eventIndex;
            //The name of the event
            public string SelectedEventName;
            //The type of the event
            public string TypeName;
        }


        //Subscribe the events at the awake
        private void Awake()
        {
            if (automatic)
            {
                AutomaticEventSubscribe();
            }
            else
            {
                if (selectAllEvents)
                {
                    ManualEventsSubscribe();
                }
                else
                {
                    ManualEventSubscribe();
                }
            }

        }

        #region Subscribe Events
        
        void AutomaticEventSubscribe()
        {
            foreach (var ae in eventCreator)
            {
                if (ae.eventType != string.Empty)
                {
                    Type type = Type.GetType(ae.eventType);
                    EventInfo SelectedEvent = type.GetEvent(ae.eventName);

                    Component selectedComponent = GetComponent(type);

                    try
                    {
                            
                        Delegate handler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType,
                            AudioManager.Instance, typeof(AudioManager).GetMethod("PlayTrack"));
                        SelectedEvent.AddEventHandler(selectedComponent, handler);
                    }
                    catch (NullReferenceException)
                    {
                        Debug.LogError($"{this.GetType().Name}:\nYou have the event maker but you don't implement it.\nRemove the event maker from the list or create it to subscribe");
                    } 
                }
            }
        }

        void ManualEventsSubscribe()
        {
            //Subscribe all the events with a string as parameter by using a list of events info
            for (var i = 0; i < allEventsNames.Count; i++)
            {
                Type type = Type.GetType(allEventsTypes[i]);
                EventInfo sEvent = type.GetEvent(allEventsNames[i]);

                Component selectedComponent = GetComponent(type);

                Delegate handler = Delegate.CreateDelegate(sEvent.EventHandlerType, AudioManager.Instance,
                    typeof(AudioManager).GetMethod("PlayTrack"));
                sEvent.AddEventHandler(selectedComponent, handler);
            }
        }

        void ManualEventSubscribe()
        {
            //Subscribe by selecting each event that you want to subscribe it
            foreach (var ae in audioEvent)
            {
                Type type = Type.GetType(ae.TypeName);
                EventInfo SelectedEvent = type.GetEvent(ae.SelectedEventName);

                Component selectedComponent = GetComponent(type);
                        
                //Debug.Log(type + " | " + ae.TypeName);

                Delegate handler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType,
                    AudioManager.Instance, typeof(AudioManager).GetMethod("PlayTrack"));
                SelectedEvent.AddEventHandler(selectedComponent, handler);
            }
        }
        
        #endregion
    }
}
