using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;

namespace AudioEngine
{
    [System.Serializable]
    public class AudioManager : MonoBehaviour
    {
        //Static reference for singleton pattern
        private static AudioManager _instance;

        //Property with a getter for take allways a reference of the audio manager in the scene
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Take the instance by the finding using the class reference
                    _instance = FindObjectsOfType<AudioManager>().FirstOrDefault();
                }

                return _instance;
            }
        }

        //The track list for setting audio in the inspector
        public List<AudioTrack> tracks = new List<AudioTrack>();
        
        //The mixers of the game project
        public List<AudioTrackMixer> mixers = new List<AudioTrackMixer>();
        
        //MixerGroupFindStuff
        //The popup with all the mixer references at the mixer list
        public List<string> mixerGroupPopup = new List<String>();
        //The index of the actual mixer selected for each track
        public List<int> mixerIndex = new List<Int32>();
        
        [System.Serializable]
        public class AudioTrack
        {
            //Name of the track reference
            public string name;
            //Clip of the track reference
            public AudioClip clip;
            //Mixer of the track reference
            public AudioMixerGroup mixer;

            //Bool for autonaming the track reference
            public bool autoName = true;
            //Bool for play the track on the awake
            public bool playOnAwake = false;
            //Bool for loop the track
            public bool loop = false;
            //Bool for display the 3D options for the actual track
            public bool dimensional = false;

            
            [Range(0, 256)] public int priority = 150;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(-3f, 3f)] public float pitch = 1f;
            [Range(0f, 1f)] public float spatialBlend = 0f;

            //3D options
            //The gameobject reference for set the audio source
            public GameObject objectReference;
            //Float for the min and manx distance of the track (the audio listener will only hear it within this range)
            public float minDistance = 1f;
            public float maxDistance = 500f;

            //The audio source reference for the initalization
            [HideInInspector] public AudioSource h_source;
            
        }

        //A class for serialize and configure all the mixers of the project into the track list
        [System.Serializable]
        public class AudioTrackMixer
        {
            //Name of the mixer
            public string name;
            //Reference of the mixer group
            public AudioMixerGroup mixerGroup;
            //A dropdown bool for the custom field
            public bool dropdownMixer;
        }

        #region Unity Functions

        //Initialize Singleton and set all the audio in the scene
        private void Awake()
        {
            // if the singleton hasn't been initialized yet
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            //Set the audio
            SetAudioInScene();
        }
        
        #endregion

        #region Public Functions

        /// <summary>
        /// <para>SetAudioInScene method is called when the game start for initialize all the audio sources with the track list previously configured</para>
        /// </summary>
        /// <param name="M:AudioManager.SetAudioInScene"></param>
        public void SetAudioInScene()
        {
            foreach (AudioTrack t in tracks)
            {
                //Check if the audio is a 3d audio track
                //If there is an object reference, then we can set the audio source at the reference object
                if (t.dimensional && t.objectReference != null)
                {
                    t.h_source = t.objectReference.AddComponent<AudioSource>();
                    t.h_source.clip = t.clip;
                    t.h_source.outputAudioMixerGroup = t.mixer;
                    t.h_source.playOnAwake = t.playOnAwake;
                    t.h_source.loop = t.loop;
                    t.h_source.priority = t.priority;
                    t.h_source.volume = t.volume;
                    t.h_source.pitch = t.pitch;
                    t.spatialBlend = 1f;
                    t.h_source.spatialBlend = t.spatialBlend;
                    t.h_source.minDistance = t.minDistance;
                    t.h_source.maxDistance = t.maxDistance;
                }
                else
                {
                    //Normal Setting
                    t.h_source = gameObject.AddComponent<AudioSource>();
                    t.h_source.clip = t.clip;
                    t.h_source.outputAudioMixerGroup = t.mixer;
                    t.h_source.playOnAwake = t.playOnAwake;
                    t.h_source.loop = t.loop;
                    t.h_source.priority = t.priority;
                    t.h_source.volume = t.volume;
                    t.h_source.pitch = t.pitch;
                    t.h_source.spatialBlend = t.spatialBlend;
                }

                //Play Sounds on Awake if the field is true
                if (t.h_source.playOnAwake) t.h_source.Play();
            }
        }

        /// <summary>
        /// <para>Play Track is a method for search any track in the list by the name and play it</para>
        /// </summary>
        /// <param name="M:AudioManager.PlayTrack.String"></param>
        public void PlayTrack(string name)
        {
            foreach (var t in tracks)
            {
                if (t.name == name)
                {
                    t.h_source.Play();
                    break;
                }
            }
        }

        #endregion
    }
}