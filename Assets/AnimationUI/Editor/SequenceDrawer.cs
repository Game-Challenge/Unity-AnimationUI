using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[ExecuteInEditMode]
[CustomPropertyDrawer(typeof(Sequence))]
public class SequenceDrawer : PropertyDrawer
{
    float _height = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
    float _buttonWidth = 38;

    Rect _backgroundRectExtra = new Rect(19, -3, 33, 2);

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(!property.FindPropertyRelative("IsUnfolded").boolValue)
        {
            property.FindPropertyRelative("PropertyRectHeight").floatValue = _height;
            return _height;
        }


        //
        Sequence.Type sequenceType = (Sequence.Type)property.FindPropertyRelative("SequenceType").enumValueIndex;
        Sequence.ObjectType targetType = (Sequence.ObjectType)property.FindPropertyRelative("TargetType").enumValueIndex;
        if(sequenceType == Sequence.Type.Animation)
        {
            Sequence.ObjectType objectType = (Sequence.ObjectType)property.FindPropertyRelative("TargetType").enumValueIndex;
            float totalHeight = 0;

            if(objectType == Sequence.ObjectType.UnityEventDynamic)
            {
                totalHeight = _height*5 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("EventDynamic")) + EditorGUIUtility.standardVerticalSpacing;
                
                property.FindPropertyRelative("PropertyRectHeight").floatValue = totalHeight;
                return totalHeight;
            }

            if(property.FindPropertyRelative("TargetComp").GetSerializedValue<Component>() == null)return _height*6;
            
            if(objectType == Sequence.ObjectType.RectTransform)
            {
                Sequence.RtTask rtTask = (Sequence.RtTask)property.FindPropertyRelative("TargetRtTask").enumValueFlag;
                if(rtTask.HasFlag(Sequence.RtTask.AnchoredPosition))totalHeight+=_height*3;
                if(rtTask.HasFlag(Sequence.RtTask.LocalScale))totalHeight+=_height*3;
                if(rtTask.HasFlag(Sequence.RtTask.LocalEulerAngles))totalHeight+=_height*3;
                if(rtTask.HasFlag(Sequence.RtTask.SizeDelta))totalHeight+=_height*3;
                if(rtTask.HasFlag(Sequence.RtTask.AnchorMax))totalHeight+=_height*3;
                if(rtTask.HasFlag(Sequence.RtTask.AnchorMin))totalHeight+=_height*3;
                if(rtTask.HasFlag(Sequence.RtTask.Pivot))totalHeight+=_height*3;
            }
            else if(objectType == Sequence.ObjectType.Transform)
            {
                Sequence.TransTask transTask = (Sequence.TransTask)property.FindPropertyRelative("TargetTransTask").enumValueFlag;
                if(transTask.HasFlag(Sequence.TransTask.LocalPosition))totalHeight+=_height*3;
                if(transTask.HasFlag(Sequence.TransTask.LocalScale))totalHeight+=_height*3;
                if(transTask.HasFlag(Sequence.TransTask.LocalEulerAngles))totalHeight+=_height*3;
            }
            else if(objectType == Sequence.ObjectType.Image)
            {
                Sequence.ImgTask imgTask = (Sequence.ImgTask)property.FindPropertyRelative("TargetImgTask").enumValueFlag;
                if(imgTask.HasFlag(Sequence.ImgTask.Color))totalHeight+=_height*3;
                if(imgTask.HasFlag(Sequence.ImgTask.FillAmount))totalHeight+=_height*3;
            }




            property.FindPropertyRelative("PropertyRectHeight").floatValue = _height*6 + totalHeight;
            return _height*6 + totalHeight;
        }
#region others
        else if(sequenceType == Sequence.Type.Wait)
        {
            property.FindPropertyRelative("PropertyRectHeight").floatValue = _height * 3;
            return _height * 3;
        }
        else if(sequenceType == Sequence.Type.SetActiveAllInput)
        {
            property.FindPropertyRelative("PropertyRectHeight").floatValue = _height * 3;
            return _height * 3;
        }
        else if(sequenceType == Sequence.Type.SetActive)
        {
            property.FindPropertyRelative("PropertyRectHeight").floatValue = _height * 4;
            return _height * 4;
        }
        else if(sequenceType == Sequence.Type.SFX)
        {
            property.FindPropertyRelative("PropertyRectHeight").floatValue = _height * 3;
            return _height * 3;
        }
        else if(sequenceType == Sequence.Type.UnityEvent)
        {
            property.FindPropertyRelative("PropertyRectHeight").floatValue = _height + (EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Event"))+EditorGUIUtility.singleLineHeight);
            return _height + (EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Event"))+EditorGUIUtility.singleLineHeight);
        }

        property.FindPropertyRelative("PropertyRectHeight").floatValue = _height * 7;
        return _height * 7; //Not gonna happen

#endregion others
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.FindPropertyRelative("PropertyRectY").floatValue = position.y;

        Rect nextPosition = new Rect(position.x, position.y, position.width, _height);
        
        Rect backgroundRect = new Rect(_backgroundRectExtra.x, position.y+_backgroundRectExtra.y, position.width+_backgroundRectExtra.width, GetPropertyHeight(property, label)+_backgroundRectExtra.height);
        
        Sequence.Type sequenceType = (Sequence.Type)property.FindPropertyRelative("SequenceType").enumValueIndex;
        
#region label
        if(sequenceType == Sequence.Type.Animation)
        {
            EditorGUI.DrawRect(backgroundRect, new Color(1, 0, 0, 0.1f));

#region preview button
            if(GUI.Button(new Rect(position.x+position.width-_buttonWidth*2, position.y, _buttonWidth, _height), "Start"))
            {
                property.FindPropertyRelative("TriggerEnd").boolValue = true;
            }
            else if(GUI.Button(new Rect(position.x+position.width-_buttonWidth, position.y, _buttonWidth, _height), "End"))
            {
                property.FindPropertyRelative("TriggerStart").boolValue = true;
            }
#endregion preview button

            // Special labeling case for UnityEvent
            if((Sequence.ObjectType)property.FindPropertyRelative("TargetType").enumValueIndex == Sequence.ObjectType.UnityEventDynamic)
            {
                float time = property.FindPropertyRelative("StartTime").floatValue;
                
                property.FindPropertyRelative("IsUnfolded").boolValue //Fix for when the label is wrong
                    = EditorGUI.Foldout(nextPosition, property.FindPropertyRelative("IsUnfolded").boolValue, 
                    "At "+time.ToString()+"s [UnityEventDynamic]");
                
                if(!property.FindPropertyRelative("IsUnfolded").boolValue)return;
            }
        }
        else if(sequenceType == Sequence.Type.Wait)
            EditorGUI.DrawRect(backgroundRect, new Color(0, 0, 1, 0.1f));
        else if(sequenceType == Sequence.Type.SetActive)
            EditorGUI.DrawRect(backgroundRect, new Color(0, 1, 0, 0.1f));
        else if(sequenceType == Sequence.Type.SetActiveAllInput)
            EditorGUI.DrawRect(backgroundRect, new Color(1, 0, 1, 0.1f));
        else if(sequenceType == Sequence.Type.SFX)
            EditorGUI.DrawRect(backgroundRect, new Color(1, 1, 0, 0.1f));
        else if(sequenceType == Sequence.Type.UnityEvent)
        {
            float time = property.FindPropertyRelative("StartTime").floatValue;
            EditorGUI.DrawRect(backgroundRect, new Color(0, 1, 1, 0.1f));
            property.FindPropertyRelative("IsUnfolded").boolValue //Fix for when the label is wrong
                = EditorGUI.Foldout(nextPosition, property.FindPropertyRelative("IsUnfolded").boolValue, "At "+time.ToString()+"s [UnityEvent]");
            if(!property.FindPropertyRelative("IsUnfolded").boolValue)return;

        }
        if(sequenceType != Sequence.Type.UnityEvent && ((Sequence.ObjectType)property.FindPropertyRelative("TargetType").enumValueIndex != Sequence.ObjectType.UnityEventDynamic))
        {
            property.FindPropertyRelative("IsUnfolded").boolValue
                = EditorGUI.Foldout(nextPosition, property.FindPropertyRelative("IsUnfolded").boolValue, label);
            if(!property.FindPropertyRelative("IsUnfolded").boolValue)return;
        }
#endregion label
        
        

        // Type
        nextPosition.y += _height;
        EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("SequenceType"), new GUIContent("Type"));
        //
        if(sequenceType == Sequence.Type.Animation)
        {
#region setup animation
            nextPosition.y += _height;
            EditorGUI.PropertyField(new Rect(nextPosition.x, nextPosition.y, nextPosition.width*(0.61f), nextPosition.height), 
                property.FindPropertyRelative("EaseType"), new GUIContent("Ease")
            );

            EditorGUI.PropertyField(new Rect(nextPosition.x+nextPosition.width*(0.61f), nextPosition.y, nextPosition.width*(0.39f), nextPosition.height), 
                property.FindPropertyRelative("EasePower"), GUIContent.none
            );

            nextPosition.y += _height;
            EditorGUI.PropertyField(new Rect(nextPosition.x, nextPosition.y, nextPosition.width, nextPosition.height), 
                property.FindPropertyRelative("Duration")
            );
            // EditorGUI.LabelField(new Rect(nextPosition.x+nextPosition.width-10, nextPosition.y, 10, nextPosition.height),
            //     "s"
            // );

            nextPosition.y += _height;
            EditorGUI.PropertyField(new Rect(nextPosition.x, nextPosition.y, nextPosition.width*(0.61f), nextPosition.height), 
                property.FindPropertyRelative("TargetType"), new GUIContent("Target")
            ); 

            Sequence.ObjectType objectType = (Sequence.ObjectType)property.FindPropertyRelative("TargetType").enumValueIndex;
            if(objectType == Sequence.ObjectType.UnityEventDynamic)
            {
                nextPosition.y += _height;
                EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("EventDynamic"));
                return;
            }
            
            EditorGUI.PropertyField(new Rect(nextPosition.x+nextPosition.width*(0.61f), nextPosition.y, nextPosition.width*(0.39f), nextPosition.height), 
                property.FindPropertyRelative("TargetComp"), GUIContent.none
            );
#endregion setup animation
            
            
            //objectType, sequenceType
            nextPosition.y += _height;
            GUIContent startContent = new GUIContent("Start");
            GUIContent endContent = new GUIContent("End");
            

            if(property.FindPropertyRelative("TargetComp").GetSerializedValue<Component>() == null)return;
            
            if(objectType == Sequence.ObjectType.RectTransform)
            {
                EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("TargetRtTask"), new GUIContent("Task"));
                void DrawRtTask(string name)
                {
                    nextPosition.y += _height;
                    EditorGUI.LabelField(new Rect(nextPosition.x, nextPosition.y, nextPosition.width, _height),
                        new GUIContent(name)
                    );
                
                    nextPosition.y += _height;
                    if(GUI.Button(new Rect(nextPosition.x, nextPosition.y, nextPosition.width/4-5, _height),"Set Start"))
                    {
                        if(name == "AnchoredPosition")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().anchoredPosition;
                        else if(name == "LocalScale")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().localScale;
                        else if(name == "LocalEulerAngles")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().localEulerAngles;
                        else if(name == "SizeDelta")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().sizeDelta;
                        else if(name == "AnchorMin")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().anchorMin;
                        else if(name == "AnchorMax")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().anchorMax;
                        else if(name == "Pivot")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().pivot;
                    }
                    EditorGUI.PropertyField(
                        new Rect(nextPosition.x+nextPosition.width/4, nextPosition.y, nextPosition.width*3/4, _height),
                        property.FindPropertyRelative(name+"Start"), GUIContent.none
                    );

                    nextPosition.y += _height;
                    if(GUI.Button(new Rect(nextPosition.x, nextPosition.y, nextPosition.width/4-5, _height),"Set End"))
                    {
                        if(name == "AnchoredPosition")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().anchoredPosition;
                        else if(name == "LocalScale")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().localScale;
                        else if(name == "LocalEulerAngles")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().localEulerAngles;
                        else if(name == "SizeDelta")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().sizeDelta;
                        else if(name == "AnchorMin")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().anchorMin;
                        else if(name == "AnchorMax")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().anchorMax;
                        else if(name == "Pivot")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().pivot;
                    }
                    EditorGUI.PropertyField(
                        new Rect(nextPosition.x+nextPosition.width/4, nextPosition.y, nextPosition.width*3/4, _height),
                        property.FindPropertyRelative(name+"End"), GUIContent.none
                    );
                }
                Sequence.RtTask rtTask = (Sequence.RtTask)property.FindPropertyRelative("TargetRtTask").enumValueFlag;
                if(rtTask.HasFlag(Sequence.RtTask.AnchoredPosition))DrawRtTask("AnchoredPosition");
                if(rtTask.HasFlag(Sequence.RtTask.LocalScale))DrawRtTask("LocalScale");
                if(rtTask.HasFlag(Sequence.RtTask.LocalEulerAngles))DrawRtTask("LocalEulerAngles");
                if(rtTask.HasFlag(Sequence.RtTask.SizeDelta))DrawRtTask("SizeDelta");
                if(rtTask.HasFlag(Sequence.RtTask.AnchorMin))DrawRtTask("AnchorMin");
                if(rtTask.HasFlag(Sequence.RtTask.AnchorMax))DrawRtTask("AnchorMax");
                if(rtTask.HasFlag(Sequence.RtTask.Pivot))DrawRtTask("Pivot");
            }

            else if(objectType == Sequence.ObjectType.Transform)
            {
                EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("TargetTransTask"), new GUIContent("Task"));
                void DrawTransTask(string name)
                {
                    nextPosition.y += _height;
                    EditorGUI.LabelField(new Rect(nextPosition.x, nextPosition.y, nextPosition.width, _height),
                        new GUIContent(name)
                    );
                
                    nextPosition.y += _height;
                    if(GUI.Button(new Rect(nextPosition.x, nextPosition.y, nextPosition.width/4-5, _height),"Set Start"))
                    {
                        if(name == "LocalPosition")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<Transform>().localPosition;
                        else if(name == "LocalScale")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<Transform>().localScale;
                        else if(name == "LocalEulerAngles")property.FindPropertyRelative(name+"Start").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<Transform>().localEulerAngles;
                    }
                    EditorGUI.PropertyField(
                        new Rect(nextPosition.x+nextPosition.width/4, nextPosition.y, nextPosition.width*3/4, _height),
                        property.FindPropertyRelative(name+"Start"), GUIContent.none
                    );

                    nextPosition.y += _height;
                    if(GUI.Button(new Rect(nextPosition.x, nextPosition.y, nextPosition.width/4-5, _height),"Set End"))
                    {
                        if(name == "LocalPosition")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<Transform>().localPosition;
                        else if(name == "LocalScale")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<Transform>().localScale;
                        else if(name == "LocalEulerAngles")property.FindPropertyRelative(name+"End").vector3Value = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<Transform>().localEulerAngles;
                    }
                    EditorGUI.PropertyField(
                        new Rect(nextPosition.x+nextPosition.width/4, nextPosition.y, nextPosition.width*3/4, _height),
                        property.FindPropertyRelative(name+"End"), GUIContent.none
                    );
                }
                Sequence.TransTask transTask = (Sequence.TransTask)property.FindPropertyRelative("TargetTransTask").enumValueFlag;
                if(transTask.HasFlag(Sequence.TransTask.LocalPosition))DrawTransTask("LocalPosition");
                if(transTask.HasFlag(Sequence.TransTask.LocalScale))DrawTransTask("LocalScale");
                if(transTask.HasFlag(Sequence.TransTask.LocalEulerAngles))DrawTransTask("LocalEulerAngles");
            }

            else if(objectType == Sequence.ObjectType.Image)
            {
                EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("TargetImgTask"), new GUIContent("Task"));
                void DrawImgTask(string name)
                {
                    nextPosition.y += _height;
                    EditorGUI.LabelField(new Rect(nextPosition.x, nextPosition.y, nextPosition.width, _height),
                        new GUIContent(name)
                    );
                
                    nextPosition.y += _height;
                    if(GUI.Button(new Rect(nextPosition.x, nextPosition.y, nextPosition.width/4-5, _height),"Set Start"))
                    {
                        if(name == "Color")property.FindPropertyRelative(name+"Start").colorValue = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().GetComponent<Image>().color;
                        else if(name == "FillAmount")property.FindPropertyRelative(name+"Start").floatValue = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().GetComponent<Image>().fillAmount;
                    }
                    EditorGUI.PropertyField(
                        new Rect(nextPosition.x+nextPosition.width/4, nextPosition.y, nextPosition.width*3/4, _height),
                        property.FindPropertyRelative(name+"Start"), GUIContent.none
                    );

                    nextPosition.y += _height;
                    if(GUI.Button(new Rect(nextPosition.x, nextPosition.y, nextPosition.width/4-5, _height),"Set End"))
                    {
                        if(name == "Color")property.FindPropertyRelative(name+"End").colorValue = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().GetComponent<Image>().color;
                        else if(name == "FillAmount")property.FindPropertyRelative(name+"End").floatValue = 
                            property.FindPropertyRelative("TargetComp").GetSerializedValue<RectTransform>().GetComponent<Image>().fillAmount;
                    }
                    EditorGUI.PropertyField(
                        new Rect(nextPosition.x+nextPosition.width/4, nextPosition.y, nextPosition.width*3/4, _height),
                        property.FindPropertyRelative(name+"End"), GUIContent.none
                    );
                }
                Sequence.ImgTask imgTask = (Sequence.ImgTask)property.FindPropertyRelative("TargetImgTask").enumValueFlag;
                if(imgTask.HasFlag(Sequence.ImgTask.Color))DrawImgTask("Color");
                if(imgTask.HasFlag(Sequence.ImgTask.FillAmount))DrawImgTask("FillAmount");
            }


            

        }
#region others
        else if(sequenceType == Sequence.Type.Wait)
        {
            nextPosition.y += _height;
            EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("Duration"));
        }
        else if(sequenceType == Sequence.Type.SetActiveAllInput)
        {
            nextPosition.y += _height;
            EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("IsActivating"));
        }
        else if(sequenceType == Sequence.Type.SetActive)
        {
            nextPosition.y += _height;
            EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("TargetComp"), new GUIContent("GameObject"));
            nextPosition.y += _height;
            EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("IsActivating"));
        }
        else if(sequenceType == Sequence.Type.SFX)
        {
            nextPosition.y += _height;
            EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("SFX"));
        }
        else if(sequenceType == Sequence.Type.UnityEvent)
        {
            nextPosition.y += _height;
            EditorGUI.PropertyField(nextPosition, property.FindPropertyRelative("Event"));
        }
#endregion others
        


    }

    public int CountOnBits(long lValue)
    {
        int iCount = 0;
        while (lValue != 0)//Loop the value while there are still bits
        {
            lValue = lValue & (lValue - 1);//Remove the end bit
            iCount++;//Increment the count
        }
        //Return the count
        return iCount;
    }
    // public int CountOnBits(int x)
    // {
    //     int count = 0;
    //     while(x != 0)
    //     {
    //         if((x & 1) != 0) count++;
    //         x = x >> 1;
    //     }
    //     return count;
    // }
}