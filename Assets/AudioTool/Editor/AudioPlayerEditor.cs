using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AudioEngine;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
     //The script referenced
     AudioPlayer manager;

     //Serialize classes
     private SerializedProperty s_audioEvents;
     private SerializedProperty s_eventCreators;

     //Reorderable lists from the serialize classes
     private ReorderableList _reorderableAudioEvents;
     private ReorderableList _reorderableEventCreators;

     //Bool for update the height of the window
     private bool setH_Event;
     
     //First display of the custom script at the inspector
     private void OnEnable()
     {
         //Get the target script
         manager = target as AudioPlayer;
         
         //Get the classes
         s_audioEvents = serializedObject.FindProperty(nameof(manager.audioEvent));
         s_eventCreators = serializedObject.FindProperty(nameof(manager.eventCreator));
         
         //Drawing the reorderable lists
         #region ReorderableListTracks

         _reorderableAudioEvents = new ReorderableList(serializedObject, s_audioEvents, true, true, false, false);
         _reorderableAudioEvents.drawHeaderCallback = DrawHeaderTracks;
         _reorderableAudioEvents.drawElementCallback = DrawAudioEvents;
         _reorderableAudioEvents.drawFooterCallback = DrawFooterTracks;
         _reorderableAudioEvents.drawNoneElementCallback = DrawBackgroundNoTracks;
         

         _reorderableAudioEvents.elementHeightCallback = delegate(int index) {
             var element = _reorderableAudioEvents.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return 98 + margin;
             else return 20 + margin;
         };
         
         #endregion
         #region ReorderableListEvents

         _reorderableEventCreators = new ReorderableList(serializedObject, s_eventCreators, true, true, false, false);
         _reorderableEventCreators.drawHeaderCallback = DrawHeaderEvents;
         _reorderableEventCreators.drawElementCallback = DrawEventCreator;
         _reorderableEventCreators.drawFooterCallback = DrawFooterEvents;
         _reorderableEventCreators.drawNoneElementCallback = DrawBackgroundNoEvents;

         setH_Event = false;
         
         _reorderableEventCreators.elementHeightCallback = delegate(int index) {
             var element = _reorderableEventCreators.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return 270 + margin;
             else return 20 + margin;
         };

         #endregion
         
         //Bool for subscribe the events only once
         manager.allEventsSetted = false;
         
         //Change the icon of the script
         DrawIcon(manager);
     }

     //The override of the actual inspector display at the actual script referenced
     public override void OnInspectorGUI()
     {
         serializedObject.Update();

         using (new EditorGUILayout.VerticalScope("Box"))
         {
             //GUIStyle for the settings button
             var settingsButtonStyle = new GUIStyle(GUI.skin.button)
             {
                 normal = new GUIStyleState() {textColor = new Color(.2f,.6f,.8f)}, fontSize = 20,
                 font = (Font) Resources.Load("customFont"), alignment = TextAnchor.MiddleCenter
             };
             
             //The settings button
             if (GUILayout.Button("SETTINGS",settingsButtonStyle,GUILayout.Width(30), GUILayout.Height(30), GUILayout.ExpandWidth(true)))
             {
                 manager.configuration = !manager.configuration;
             }
             
             //If not in settings window
             if (!manager.configuration)
             {
                 //If not automatic event creator
                 if (!manager.automatic)
                 {
                     //The subscribe all events option
                     using (new EditorGUILayout.HorizontalScope("Box"))
                     {
                         EditorGUILayout.LabelField("Subscribe All Events");
                     
                         EditorGUILayout.Space();
                     
                         manager.selectAllEvents = EditorGUILayout.Toggle(manager.selectAllEvents);
                 
                         EditorGUILayout.Space();
                     }

                     //If select all events attached into the actual gameobject
                     if (manager.selectAllEvents)
                     {
                         //Get a list of the scripts attached to the acutal gameobject
                         List<MonoBehaviour> scripts = manager.GetComponents<MonoBehaviour>().Where(s => s.GetType().Name != manager.GetType().Name).ToList();

                         //Get a list of the match events at the scripts
                         List<EventInfo> matchEvents = GetMatchEvents(scripts);

                         //If all events aren't setted by a list reference
                         if (!manager.allEventsSetted)
                         {
                             //Clear the lists (name and type)
                             manager.allEventsNames.Clear();
                             manager.allEventsTypes.Clear();
                         
                             //for each event set the name and the type at the reference lists for subscribe the event later at the start
                             foreach (var mEvent in matchEvents)
                             {
                                 manager.allEventsNames.Add(mEvent.Name);
                                 manager.allEventsTypes.Add(mEvent.DeclaringType.AssemblyQualifiedName);
                             }

                             //Once setted the events, set the bool true
                             manager.allEventsSetted = true;
                         }

                         //Display the events that match the (Action<string>) structure
                         using (new EditorGUILayout.VerticalScope("Box"))
                         {
                             foreach (var mEvent in matchEvents)
                             {
                                 EditorGUILayout.LabelField(mEvent.Name);
                             }
                         }
                     }
                     else
                     {
                         //Horizontal Space for add and remove tracks
                         using (new EditorGUILayout.HorizontalScope())
                         {
                             if (GUILayout.Button("Add Event Subscriber"))
                             {
                                 manager.audioEvent.Add(new AudioPlayer.AudioEvent());
                             }

                             GUILayout.Space(10f);

                             if (GUILayout.Button("Remove Event Subscriber"))
                             {
                                 if (manager.audioEvent.Count - 1 >= 0) manager.audioEvent.RemoveAt(manager.audioEvent.Count - 1);
                             }
                         }

                         _reorderableAudioEvents.DoLayoutList();
                         
                         //reset the allEventSetted bool
                         manager.allEventsSetted = false;
                     }
                 }
                 else
                 {
                     using (new EditorGUILayout.VerticalScope("Box"))
                     {
                         //Create all the events from the list
                         if (GUILayout.Button("Create"))
                         {
                             if (AudioManager.Instance.tracks.Count > 0)
                             {
                                 CreateAllEvents();
                             }
                         }

                         //Subscribe all the events from the list
                         if (GUILayout.Button("Subscribe"))
                         {
                             if (AudioManager.Instance.tracks.Count > 0)
                             {
                                 for (int i = 0; i < manager.eventCreator.Count; i++)
                                 {
                                     SetEvents(i);
                                 }
                             }
                         }
                         
                         using (new EditorGUILayout.HorizontalScope())
                         {
                             if (GUILayout.Button("Add Event Creator"))
                             {
                                 manager.eventCreator.Add(new AudioPlayer.EventCreator());
                             }
                             
                             if (GUILayout.Button("Remove Event Creator"))
                             {
                                 if(manager.eventCreator.Count > 0) manager.eventCreator.RemoveAt(manager.eventCreator.Count-1);
                             }
                         }
                         
                         using (new EditorGUILayout.VerticalScope("Box"))
                         {
                             //Display each event
                             _reorderableEventCreators.DoLayoutList();
                         }
                     }
                 }
             }
             else
             {
                 //Settings Window
                 //Display the automatic invoking field for implement the creation of the events
                 using (new EditorGUILayout.HorizontalScope("Box"))
                 {
                     EditorGUILayout.LabelField("Automatic Invoking");
                 
                     EditorGUILayout.Space();
                 
                     manager.automatic = EditorGUILayout.Toggle(manager.automatic);
             
                     EditorGUILayout.Space();
                 }
             }

         }

         serializedObject.ApplyModifiedProperties();
     }

     //Automatic event creation methods
     #region Automatic creation of the events
     
     /// <summary>
     /// <para>This method create all the events at once and refresh the assets folder</para>
     /// <param name="M:AudioPlayerEditor:CreateAllEvents"></param>
     /// </summary>
     void CreateAllEvents()
     {
         if (manager.eventCreator.Count > 0)
         {
             for (int i = 0; i < manager.eventCreator.Count; i++)
             {
                 CreateEvent(i);
             }
             
             AssetDatabase.Refresh();
         }
     }

     /// <summary>
     /// <para>This method create the actual event at the index and refresh the assets folder if need it</para>
     /// </summary>
     /// <param name="M:AudioPlayerEditor:CreateAllEvents:i:refresh"></param>
     void CreateEvent(int i, bool refresh = false)
     {
         //Get the path of the selected script
         string path = GetPathFile(i);

         //Read the script and save it in a string
         StreamReader scriptReader = new StreamReader(path);

         string refScriptReader = scriptReader.ReadToEnd();
         scriptReader.Close();

         //Create the event if doesn't exist
         if (!refScriptReader.Contains(manager.eventCreator[i].eventName))
         {
             string indent = "    ";
             string indentMethod = indent + indent;
             
             string declaringEvent = indent + $"public event Action<string> {manager.eventCreator[i].eventName};";
             string trackReference = $"\"{manager.eventCreator[i].selectedTrack}\"";
             string invokingEvent = indentMethod + $"{manager.eventCreator[i].eventName}?.Invoke({trackReference});";
             
             if (refScriptReader.Contains("#region EventsRegion"))
             {
                 string scriptRefresh = DeclaringInvokeWithRegion(refScriptReader, i, declaringEvent);

                 string overwriteScript = InvokeInMethod(scriptRefresh, i, invokingEvent);

                 StreamWriter scriptOverWriter = new StreamWriter(path);
                 scriptOverWriter.Write(overwriteScript);
                 scriptOverWriter.Close();
                 
                 if(refresh) AssetDatabase.Refresh();
             }
             else
             {
                 string scriptRefresh = DeclaringInvokeWithoutRegion(refScriptReader, i, declaringEvent, indent);

                 string overwriteScript = InvokeInMethod(scriptRefresh, i, invokingEvent);
                
                 StreamWriter scriptOverWriter = new StreamWriter(path);
                 scriptOverWriter.Write(overwriteScript);
                 scriptOverWriter.Close();
                 
                 if(refresh) AssetDatabase.Refresh();
             }
         }
     }
     
     //Methods for find paths at any directory
     #region Path finding

     /// <summary>
     /// <para>This Method search the file that match with te name of the script selected into assets, library and package folders, by using the GetFiles method from the System.IO.Directory namespace</para>
     /// </summary>
     /// <param name="M:AudioPlayerEditor:CreateAllEvents:i"></param>
     /// <returns></returns>
     string GetPathFile(int i)
     {
         string path = "";
         string[] matchFiles = System.IO.Directory.GetFiles(Application.dataPath, $"{manager.eventCreator[i].selectedScript.GetType().Name}.cs", SearchOption.AllDirectories);
         if (matchFiles.Length != 0)
         {
             path = matchFiles[0].Replace("\\", "/");
             return path;
         }
         else
         {
             matchFiles = System.IO.Directory.GetFiles("Library", $"{manager.eventCreator[i].selectedScript.GetType().Name}.cs", SearchOption.AllDirectories);
             if (matchFiles.Length != 0)
             {
                 path = matchFiles[0].Replace("\\", "/");
                 return path;
             }
             else
             {
                 matchFiles = System.IO.Directory.GetFiles("Packages", $"{manager.eventCreator[i].selectedScript.GetType().Name}.cs", SearchOption.AllDirectories);
                 if (matchFiles.Length != 0)
                 {
                     path = matchFiles[0].Replace("\\", "/");
                     return path;
                 }
             }
         }

         return path;
     }
     
     #endregion

     //Overwrite the selecte script with the event creation
     #region Overwrite Script
     
     /// <summary>
     /// <para>This Method declare the events within the existing region</para>
     /// </summary>
     /// <param name="M:AudioPlayerEditor:DeclaringInvokeWithRegion:refScript:i:declaringInvoke"></param>
     /// <returns></returns>
     string DeclaringInvokeWithRegion(string refScript, int i, string declaringInvoke)
     {
         //hasta la region
         string eventFirstPart = refScript.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[0] + "#region EventsRegion\n";

         //el resto del script
         string restEventScriptPart = refScript.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[1];
                                             
         eventFirstPart += $"\n{declaringInvoke}";

         string firstPartScript = eventFirstPart + restEventScriptPart;

         if (!firstPartScript.Contains("using System;"))
         {
             firstPartScript = "using System;\n" + firstPartScript;
         }

         return firstPartScript;
     }
     
     /// <summary>
     /// <para>This Method declare the event into a region previously created</para>
     /// </summary>
     /// <param name="M:AudioPlayerEditor:DeclaringInvokeWithoutRegion:refScript:i:declaringInvoke:indent"></param>
     /// <returns></returns>
     string DeclaringInvokeWithoutRegion(string refScript, int i, string declaringInvoke, string indent)
     {
         //Priemera parte hasta el nombre de la clase (Añado la clase que se pierde al splitear)
         string s0 = refScript.Split(new string[] {$"{manager.eventCreator[i].selectedScript.GetType().Name}"}, StringSplitOptions.None)[0] + manager.eventCreator[i].selectedScript.GetType().Name;
         
         //El resto
         string s1 = refScript.Split(new string[] {$"{manager.eventCreator[i].selectedScript.GetType().Name}"}, StringSplitOptions.None)[1];
         
         //A partir de la primera llave justo despues de la definicion de la clase
         string t0 = s1.Split('{')[0] + "{";
         
         //join the first part
         string firstPartToFirstQuotes = s0 + t0;
         
         //El resto
         string t1 = refScript.Substring(firstPartToFirstQuotes.Length);
         

         // Set at the first { the region for the events
         string regionPart = firstPartToFirstQuotes + $"\n{indent}#region EventsRegion" + $"\n\n{indent}#endregion\n";

         string eventSettingPart = regionPart.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[0] + "#region EventsRegion";
         string restEventSettingPart = regionPart.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[1];

         eventSettingPart += $"\n{declaringInvoke}";
         
         //join the event region
         string eventDeclaredResult = eventSettingPart + restEventSettingPart;

         string firstPartScript = eventDeclaredResult + t1;
         
         if (!firstPartScript.Contains("using System;"))
         {
             firstPartScript = "using System;\n" + firstPartScript;
         }

         return firstPartScript;
     }
     
     /// <summary>
     /// <para>This Method create the event invoking into the selected method</para>
     /// </summary>
     /// <param name="M:AudioPlayerEditor:DeclaringInvokeWithoutRegion:refScript:i:invokingEvent"></param>
     /// <returns></returns>
     string InvokeInMethod(string scriptRefresh, int i, string invokingEvent)
     {
         //Find the method and invoke the event
         string methodName = manager.eventCreator[i].selectedMethod.Name;
         ParameterInfo[] parameters = manager.eventCreator[i].selectedMethod.GetParameters().ToArray();
         string methodFirstPart = "";
         int indexForMethod = 0;
         
         if (parameters.Length == 0)
         {
             methodFirstPart = FindFirstPartMethod();
         }
         else
         {
             methodFirstPart = FindFirstPartMethod(true);
         }

         string FindFirstPartMethod(bool parameters = false)
         {
             string methodFirstPart = "";
             
             int methodMatches = scriptRefresh.Split(new string[] {$"{methodName}"}, StringSplitOptions.None).Length;
             for (int j = 1; j < methodMatches; j++)
             {
                 string testSplitForFindMethod = $"{methodName}" + scriptRefresh.Split(new string[] {$"{methodName}"}, StringSplitOptions.None)[j];

                 string testRemovingAfterCalls = testSplitForFindMethod.Split(new string[] {"{"}, StringSplitOptions.RemoveEmptyEntries)[0] + "{";
                 testRemovingAfterCalls = testRemovingAfterCalls.Replace("\n", " ").Replace("\r", " ").Replace(" ", "");

                 string param = "";
                 
                 if (parameters)
                 {
                     if (testRemovingAfterCalls.Split('(',')').Length > 1)
                     {
                         param = testRemovingAfterCalls.Split('(',')')[1].ToLower();
                     }
                 }

                 if (testRemovingAfterCalls.ToLower().Contains(($"{methodName}({param})" + "{").ToLower()))
                 {
                     //Nos aseguramos de coger el metodo y no su invocación ni ningun comentario
                     indexForMethod = j - 1;
                     break;
                 }
                 else
                 {
                     continue;
                 }
             }
             
             if (indexForMethod > 0)
             {
                 //Get the method split
                 string scriptRefreshCopy = scriptRefresh;
                 for (int j = 0; j < indexForMethod; j++)
                 {
                     //Get the first splits before the method
                     methodFirstPart += scriptRefreshCopy.Split(new string[] {$"{methodName}"}, StringSplitOptions.None)[j] + $"{methodName}";
                 }
                 //Get the method split
                 methodFirstPart += scriptRefresh.Split(new string[] {$"{methodName}"}, StringSplitOptions.None)[indexForMethod] + $"{methodName}";
             }
             else
             {
                 methodFirstPart = scriptRefresh.Split(new string[] {$"{methodName}"}, StringSplitOptions.None)[indexForMethod] + $"{methodName}";
             }

             return methodFirstPart;
         }

         string restMethodPart = scriptRefresh.Split(new string[] {$"{methodName}"}, StringSplitOptions.None)[indexForMethod+1];
         
         string firstQuoteInMethodPart = restMethodPart.Split('{')[0] + '{';
         
         string firstPartToFirstMethod = methodFirstPart + firstQuoteInMethodPart;
     
         //El resto
         string restInMethodPart = scriptRefresh.Substring(firstPartToFirstMethod.Length);

         firstPartToFirstMethod += $"\n{invokingEvent}";

         string finalScriptResult = firstPartToFirstMethod + restInMethodPart;

         return finalScriptResult;
     }
     
     #endregion
     
     /// <summary>
     /// <para>This Method subscribe the event at the passed index if exist</para>
     /// </summary>
     /// <param name="M:AudioPlayerEditor:SetEvents:index"></param>
     /// <returns></returns>
     void SetEvents(int index)
     {
         string path = GetPathFile(index);

         StreamReader pathReader = new StreamReader(path);
         string refScript = pathReader.ReadToEnd();
         pathReader.Close();

         if (refScript.Contains(manager.eventCreator[index].eventName))
         {
             //Set the type of the event for subscribing it later
             EventInfo ev = manager.eventCreator[index].selectedScript.GetType().GetEvent(manager.eventCreator[index].eventName);
             if (ev != null) Debug.Log("Event: " + ev.Name + " | " + ev.DeclaringType.AssemblyQualifiedName + "\nEvent successfully subscribed");
             if (ev != null) manager.eventCreator[index].eventType = ev.DeclaringType.AssemblyQualifiedName;
         }
         else
         {
             //Event not created
             Debug.LogWarning(manager.GetType().Name + ": You're trying to subscribe the event but it doesn't exist.\nPlease, create it first.");
         }
     }
     
     #endregion
     
     //Drawing into the inspector the reorderable lists
     #region DrawingReorderableLists
     
     //Serialize all the fields of the audio event class into a reorderable list
     public void DrawAudioEvents(Rect position, int index, bool isActive, bool isFocused)
     {
         SerializedProperty property = _reorderableAudioEvents.serializedProperty.GetArrayElementAtIndex(index);
         
         position.width -= 34;
         position.height = 18;
        
         Rect dropdownRect = new Rect(position);
         dropdownRect.width = 10;
         dropdownRect.height = 10;
         dropdownRect.x += 10;
         dropdownRect.y += 5;
         
         property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, "Audio Event");
         
         position.x += 50;
         position.width -= 15;
        
         Rect fieldRect = new Rect(position);

         if (property.isExpanded)
         {
             if (manager.audioEvent.Count - 1 >= index)
             {
                 Space(ref fieldRect);
                 List<MonoBehaviour> scripts = manager.GetComponents<MonoBehaviour>().Where(s => s.GetType().Name != manager.GetType().Name).ToList();
             
                 if (scripts.Count > 0)
                 {
                     List<EventInfo> matchEvents = GetMatchEvents(scripts);

                     if (matchEvents.Count > 0)
                     {
                         string[] methodNames = matchEvents.Select(e => $"{e.DeclaringType} / {e.Name}()").ToArray();
                     
                         manager.audioEvent[index].eventIndex = EditorGUI.Popup(fieldRect, "Event Player", manager.audioEvent[index].eventIndex, methodNames);
                         if (manager.audioEvent[index].eventIndex >= 0)
                         {
                             //Debug.Log(manager.audioEvent[index].eventIndex);
                             manager.audioEvent[index].TypeName = matchEvents[manager.audioEvent[index].eventIndex].DeclaringType.AssemblyQualifiedName;
                             manager.audioEvent[index].SelectedEventName = matchEvents[manager.audioEvent[index].eventIndex].Name;
                         }
                         else
                         {
                             manager.audioEvent[index].SelectedEventName = matchEvents[0].Name;
                             manager.audioEvent[index].TypeName = matchEvents[0].DeclaringType.AssemblyQualifiedName;
                         }
                         
                         
                     }
                     else
                     {
                         float x = fieldRect.x;
                         var settingsButtonStyle = new GUIStyle(GUI.skin.button)
                         {
                             normal = new GUIStyleState() {textColor = new Color(.8f,.2f,.2f)}, fontSize = 20,
                             font = (Font) Resources.Load("customFont"), alignment = TextAnchor.MiddleCenter
                         };
                         fieldRect.x -= 60;
                         EditorGUI.LabelField(new Rect(fieldRect.position, new Vector2(EditorGUIUtility.currentViewWidth - 60, 30)), "You must add some event to any script of the component", settingsButtonStyle);
                         fieldRect.x = x;
                     }
                     
                     Space(ref fieldRect, 40);
                     
                     Rect buttonRect = new Rect(fieldRect.position, new Vector2(50, fieldRect.height));
                     buttonRect.x += (EditorGUIUtility.currentViewWidth * 0.5f)-buttonRect.x;
                     if (GUI.Button(buttonRect, "X"))
                     {
                         manager.audioEvent.Remove(manager.audioEvent.ElementAt(index));
                     }
                     
                     Space(ref fieldRect, 15);
                     
                     DrawUILine(fieldRect.x, fieldRect.y, 0, 17);
                     
                     Space(ref fieldRect);
                 }
                 else
                 {
                     if (property.isExpanded)
                     {
                         var settingsLabelStyle = new GUIStyle(GUI.skin.box)
                         {
                             normal = new GUIStyleState() {textColor = new Color(.8f,.2f,.2f)}, fontSize = 20,
                             font = (Font) Resources.Load("customFont"), alignment = TextAnchor.MiddleCenter
                         };
                         EditorGUI.LabelField(new Rect(new Vector2(fieldRect.x, fieldRect.y + 10), new Vector2(fieldRect.width, 50)), "You must add some script to subscribe any event",settingsLabelStyle);
                     }
                 }
             }
         }
         else
         {
             Rect buttonRect = new Rect(dropdownRect.position, new Vector2(50, 20));
             buttonRect.y -= 5;
             buttonRect.x += (EditorGUIUtility.currentViewWidth - (buttonRect.x * 2.5f));
             if (GUI.Button(buttonRect, "X"))
             {
                 manager.audioEvent.Remove(manager.audioEvent.ElementAt(index));
             }
         }
     }

     //Serialize all the fields of the audio event creator class into a reorderable list
     public void DrawEventCreator(Rect position, int index, bool isActive, bool isFocused)
     {
         if (manager.eventCreator.Count - 1 >= index)
         {
             SerializedProperty property = _reorderableEventCreators.serializedProperty.GetArrayElementAtIndex(index);
             
             position.width -= 34;
             position.height = 18;
            
             Rect dropdownRect = new Rect(position);
             dropdownRect.width = 10;
             dropdownRect.height = 10;
             dropdownRect.x += 10;
             dropdownRect.y += 5;
             
             property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, "Event Creator");
             
             position.x += 10;
             position.width -= 15;
            
             Rect fieldRect = new Rect(position);
             
             Space(ref fieldRect, 30);

             List<MonoBehaviour> scripts = manager.GetComponents<MonoBehaviour>().Where(s => s.GetType().Name != manager.GetType().Name).ToList();

             
             if (scripts.Count > 0)
             {
                 if (property.isExpanded)
                 {
                     float x = fieldRect.x;
                     if (scripts.Count > 0)
                     {
                         string[] scriptsNames = scripts.Select(s => s.GetType().Name).ToArray();

                         EditorGUI.LabelField(fieldRect, "Script");

                         fieldRect.x += 100;

                         int scriptIndex =
                             EditorGUI.Popup(
                                 new Rect(fieldRect.position, new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                                 scripts.ToList().IndexOf(manager.eventCreator[index].selectedScript), scriptsNames);
                         if (scriptIndex >= 0) manager.eventCreator[index].selectedScript = scripts[scriptIndex];

                         Space(ref fieldRect);

                         List<MethodInfo> methods = new List<MethodInfo>();
                         if (manager.eventCreator[index].selectedScript != null)
                         {
                             methods = manager.eventCreator[index].selectedScript.GetType()
                                 .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                 .Where(m => m.DeclaringType == manager.eventCreator[index].selectedScript.GetType() && !m.IsSpecialName).ToList();
                         }

                         if (methods.Count > 0)
                         {
                             string[] methodsNames = methods.Select(m => m.Name).ToArray();

                             fieldRect.x = x;
                             EditorGUI.LabelField(fieldRect, "Method");

                             fieldRect.x += 100;

                             manager.eventCreator[index].methodIndex =
                                 EditorGUI.Popup(new Rect(fieldRect.position, new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)), 
                                     manager.eventCreator[index].methodIndex, methodsNames);
                             if (manager.eventCreator[index].methodIndex >= 0) manager.eventCreator[index].selectedMethod = methods[manager.eventCreator[index].methodIndex];

                             Space(ref fieldRect);
                         }

                     }

                     if (AudioManager.Instance.tracks.Count > 0)
                     {
                         if (setH_Event) GetEventHeight();

                         string[] tracksNames = AudioManager.Instance.tracks.Select(t => t.name).ToArray();

                         fieldRect.x = x;
                         EditorGUI.LabelField(fieldRect, "Track");

                         fieldRect.x += 100;

                         int trackIndex =
                             EditorGUI.Popup(
                                 new Rect(fieldRect.position,
                                     new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                                 tracksNames.ToList().IndexOf(manager.eventCreator[index].selectedTrack), tracksNames);
                         if (trackIndex >= 0) manager.eventCreator[index].selectedTrack = tracksNames[trackIndex];

                         Space(ref fieldRect);

                         fieldRect.x = x;
                         EditorGUI.LabelField(fieldRect, "Auto name");

                         fieldRect.x += 100;

                         manager.eventCreator[index].autoName =
                             EditorGUI.Toggle(fieldRect, manager.eventCreator[index].autoName);

                         if (manager.eventCreator[index].autoName)
                         {
                             Space(ref fieldRect);

                             fieldRect.x = x;

                             EditorGUI.LabelField(fieldRect, "Event Name");

                             fieldRect.x += 100;

                             if (tracksNames.Length - 1 >= trackIndex && trackIndex >= 0)
                             {
                                 manager.eventCreator[index].eventName = $"{tracksNames[trackIndex]}Event";
                                 EditorGUI.LabelField(
                                     new Rect(fieldRect.position,
                                         new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                                     manager.eventCreator[index].eventName, new GUIStyle(GUI.skin.box));
                             }
                         }
                         else
                         {
                             Space(ref fieldRect);

                             fieldRect.x = x;

                             EditorGUI.LabelField(fieldRect, "Event Name");

                             fieldRect.x += 100;

                             manager.eventCreator[index].eventName = EditorGUI.TextField(
                                 new Rect(fieldRect.position,
                                     new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                                 manager.eventCreator[index].eventName);
                         }
                     }
                     else
                     {
                         if (!setH_Event) GetEventHeight();

                         fieldRect.x = x;
                         EditorGUI.LabelField(fieldRect, "You must have any track for use this component".ToUpper(),
                             new GUIStyle(GUI.skin.box));
                     }

                     Space(ref fieldRect, 30);
                     Rect createButtonRect = new Rect(fieldRect.position,
                         new Vector2(EditorGUIUtility.currentViewWidth - 70, fieldRect.height));
                     createButtonRect.x = x - 20;
                     
                     if (GUI.Button(createButtonRect, "Create Event"))
                     {
                         if (AudioManager.Instance.tracks.Count > 0)
                         {
                             CreateEvent(index, true);
                         }
                     }
                     
                     Space(ref fieldRect, 25);
                     Rect subscribeButton = new Rect(new Vector2(40, fieldRect.position.y),
                         new Vector2(EditorGUIUtility.currentViewWidth - 70, fieldRect.height));

                     if (GUI.Button(subscribeButton, "Subscribe Event"))
                     {
                         if (AudioManager.Instance.tracks.Count > 0)
                         {
                             SetEvents(index);
                         }
                     }

                     Space(ref fieldRect, 40);

                     Rect buttonRect = new Rect(fieldRect.position, new Vector2(50, fieldRect.height));
                     buttonRect.x += (EditorGUIUtility.currentViewWidth * 0.5f) - buttonRect.x;
                     if (GUI.Button(buttonRect, "X"))
                     {
                         manager.eventCreator.Remove(manager.eventCreator.ElementAt(index));
                     }

                     Space(ref fieldRect, 15);

                     fieldRect.x = x + 40;

                     DrawUILine(fieldRect.x, fieldRect.y, manager.eventCreator.Count);

                     Space(ref fieldRect);

                 }
                 else
                 {
                     Rect buttonRect = new Rect(dropdownRect.position, new Vector2(50, 20));
                     buttonRect.y -= 5;
                     buttonRect.x += (EditorGUIUtility.currentViewWidth - (buttonRect.x * 2.5f));
                     if (GUI.Button(buttonRect, "X"))
                     {
                         manager.eventCreator.Remove(manager.eventCreator.ElementAt(index));
                     }
                 }
             }
             else
             {
                 if (property.isExpanded)
                 {
                     var settingsLabelStyle = new GUIStyle(GUI.skin.box)
                     {
                         normal = new GUIStyleState() {textColor = new Color(.8f,.2f,.2f)}, fontSize = 20,
                         font = (Font) Resources.Load("customFont"), alignment = TextAnchor.MiddleCenter
                     };
                     EditorGUI.LabelField(new Rect(new Vector2(fieldRect.x, fieldRect.y + 50), new Vector2(fieldRect.width, 50)), "You must add some script to subscribe any event",settingsLabelStyle);
                 }
             }
         }
     }
     
     #endregion

     //Utility methods for to get info
     #region Utility methods
     
     /// <summary>
     /// <para>This Method get the events that match with the structure (Action<string>)</para>
     /// </summary>
     /// <param name="M:AudioPlayerEditor:GetMatchEvents:scripts"></param>
     /// <returns></returns>
     List<EventInfo> GetMatchEvents(List<MonoBehaviour> scripts)
     {
         List<EventInfo> allEvents = scripts.SelectMany(s => s.GetType().GetEvents()).ToList();
         List<EventInfo> matchEvents = new List<EventInfo>();
                     
         foreach (var eventInfo in allEvents)
         {
             if (eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(p => p.GetType()).SequenceEqual(typeof(AudioManager).GetMethod("PlayTrack")?.GetParameters().Select(p => p.GetType())))
             {
                 matchEvents.Add(eventInfo);
             }
         }

         return matchEvents;
     }
     
     #endregion

     //Drawing methods for the reorderable lists
     #region Displaying methods

     //Get the height of the audio event depending in the tracks count of the Audio Manager
     void GetEventHeight()
     {
         _reorderableEventCreators.elementHeightCallback = delegate(int index) {
             var element = _reorderableEventCreators.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return (AudioManager.Instance.tracks.Count > 0 ? 245 : 185) + margin;
             else return 20 + margin;
         };

         setH_Event = !setH_Event;
     }
     
     void DrawHeaderTracks(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "Audio Event Players";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
     
     void DrawHeaderEvents(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "Event Creator";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
    
     void DrawFooterTracks(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "By @babelgames_es";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
     
     void DrawFooterEvents(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "By @babelgames_es";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
    
     void DrawBackgroundNoTracks(Rect rect)
     {
         var greenStylePreset = new GUIStyle(GUI.skin.label);
         greenStylePreset.normal.textColor = new Color(.05f, .9f, .2f);
         string name = "Add Audio Events for attach any function to any track";
         EditorGUI.LabelField(rect, name, greenStylePreset);
     }
     
     void DrawBackgroundNoEvents(Rect rect)
     {
         var greenStylePreset = new GUIStyle(GUI.skin.label);
         greenStylePreset.normal.textColor = new Color(.05f, .9f, .2f);
         string name = "Add any event creator";
         EditorGUI.LabelField(rect, name, greenStylePreset);
     }

     #endregion
     
     //Utility methods for the drawing
     #region Utility display methods

     public void Space(ref Rect pos, float space = 30f)
     {
         pos.y += space;
     }
     
     public static void DrawUILine(float posX, float posY, int actualEvents = 0, float withAdd = 0, float thickness = 38, float padding = 30)
     {
         Rect r = new Rect(posX, posY, thickness, padding);
         r.width = EditorGUIUtility.currentViewWidth - (actualEvents > 1 ? 21 : 10);
         r.width += withAdd;
         r.height = 2;
         r.y+=padding * 0.3f;
         r.x-=70;
         r.width -= thickness;
         EditorGUI.DrawRect(r, Color.cyan);
     }
     
     private void DrawIcon(Component gameObject, int idx = 5)
     {
         //Debug.Log("Change Texture");
         GUIContent content = new GUIContent(Resources.Load("AudioPlayer_Icon") as Texture2D);
         var egu = typeof(EditorGUIUtility);
         var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
         var args = new object[] { gameObject, content.image };
         var setIcon = egu.GetMethod("SetIconForObject", flags, null, new Type[]{typeof(UnityEngine.Object), typeof(Texture2D)}, null);
         setIcon.Invoke(null, args);
     }

     #endregion
     
}