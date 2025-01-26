using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.Rendering;
using Object = UnityEngine.Object;

namespace AkiDevCat.AVL.Components
{
    [CustomEditor(typeof(VolumetricLight)), CanEditMultipleObjects]
    public class VolumetricLightEditor : Editor
    {
        private LightEditor.Settings lightSettings = null;

        public void OnEnable()
        {
            lightSettings = new LightEditor.Settings(serializedObject);
            lightSettings.OnEnable();
        }

        private void OnDestroy()
        {
            lightSettings.OnDestroy();
        }

        private T ShowObjectProperty<T>(T value, bool allowSceneObjects, string label = "", bool isDisabled = false) where T : Object
        {
            T result;
            var previousValue = GUI.enabled;
            
            if (isDisabled)
            {
                GUI.enabled = false;
            }

            if (!string.IsNullOrEmpty(label))
            {
                result = (T)EditorGUILayout.ObjectField(new GUIContent(label), value, typeof(T), allowSceneObjects);
            }
            else
            {
                result = (T)EditorGUILayout.ObjectField(value, typeof(T), allowSceneObjects);
            }

            if (isDisabled)
            {
                GUI.enabled = previousValue;
            }

            return result;
        }

        private bool ShowBoolProperty(bool value, string label = "", bool isDisabled = false)
        {
            bool result;
            var previousValue = GUI.enabled;
            
            if (isDisabled)
            {
                GUI.enabled = false;
            }
            
            if (!string.IsNullOrEmpty(label))
            {
                result = EditorGUILayout.Toggle(new GUIContent(label), value);
            }
            else
            {
                result = EditorGUILayout.Toggle(value);
            }
            
            if (isDisabled)
            {
                GUI.enabled = previousValue;
            }

            return result;
        }
        
        private float ShowFloatProperty(float value, string label = "", bool isDisabled = false)
        {
            float result;
            var previousValue = GUI.enabled;
            
            if (isDisabled)
            {
                GUI.enabled = false;
            }
            
            if (!string.IsNullOrEmpty(label))
            {
                result = EditorGUILayout.FloatField(new GUIContent(label), value);
            }
            else
            {
                result = EditorGUILayout.FloatField(value);
            }
            
            if (isDisabled)
            {
                GUI.enabled = previousValue;
            }

            return result;
        }
        
        private float ShowSliderProperty(float value, float minValue, float maxValue, string label = "", bool isDisabled = false)
        {
            float result;
            var previousValue = GUI.enabled;
            
            if (isDisabled)
            {
                GUI.enabled = false;
            }
            
            if (!string.IsNullOrEmpty(label))
            {
                result = EditorGUILayout.Slider(new GUIContent(label), value, minValue, maxValue);
            }
            else
            {
                result = EditorGUILayout.Slider(value, minValue, maxValue);
            }
            
            if (isDisabled)
            {
                GUI.enabled = previousValue;
            }

            return result;
        }
        
        private Vector2 ShowVector2Property(Vector2 value, string label = "", bool isDisabled = false)
        {
            Vector2 result;
            var previousValue = GUI.enabled;
            
            if (isDisabled)
            {
                GUI.enabled = false;
            }
            
            if (!string.IsNullOrEmpty(label))
            {
                result = EditorGUILayout.Vector2Field(new GUIContent(label), value);
            }
            else
            {
                result = EditorGUILayout.Vector2Field("", value);
            }
            
            if (isDisabled)
            {
                GUI.enabled = previousValue;
            }

            return result;
        }
        
        private Color ShowColorProperty(Color value, string label = "", bool isDisabled = false)
        {
            Color result;
            var previousValue = GUI.enabled;
            
            if (isDisabled)
            {
                GUI.enabled = false;
            }
            
            if (!string.IsNullOrEmpty(label))
            {
                result = EditorGUILayout.ColorField(new GUIContent(label), value, true, false, false);
            }
            else
            {
                result = EditorGUILayout.ColorField(value);
            }
            
            if (isDisabled)
            {
                GUI.enabled = previousValue;
            }

            return result;
        }
        
        private T ShowEnumProperty<T>(T value, string label = "", bool isDisabled = false) where T : System.Enum
        {
            T result;
            var previousValue = GUI.enabled;
            
            if (isDisabled)
            {
                GUI.enabled = false;
            }
            
            if (!string.IsNullOrEmpty(label))
            {
                result = (T)EditorGUILayout.EnumPopup(new GUIContent(label), value);
            }
            else
            {
                result = (T)EditorGUILayout.EnumPopup(value);
            }
            
            if (isDisabled)
            {
                GUI.enabled = previousValue;
            }

            return result;
        }

        private void ShowHeader(string headerName)
        {
            EditorGUILayout.BeginVertical("dockHeader", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(" " + headerName, EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            var cmp = (VolumetricLight)target;
            var cmps = targets.Cast<VolumetricLight>().ToList();
            serializedObject.Update();

            cmps.ForEach(x => Undo.RecordObject(x, "Modified parameter in " + x.gameObject.name));
            

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("ProjectBrowserIconAreaBg");

            ShowHeader("General");
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            cmp.baseLightComponent = ShowObjectProperty(cmp.baseLightComponent, true, "  Base Light");
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.baseLightComponent = cmp.baseLightComponent);
                SceneView.RepaintAll();
            }

            if (cmp.baseLightComponent == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Get Component"))
                {
                    cmps.ForEach(x => x.baseLightComponent = x.GetComponent<Light>());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            if (cmp.baseLightComponent != null)
            {
                EditorGUI.BeginChangeCheck();
                cmp.copyLightComponent = ShowBoolProperty(cmp.copyLightComponent, "  Copy Settings");
                if (EditorGUI.EndChangeCheck())
                {
                    cmps.ForEach(x => x.copyLightComponent = cmp.copyLightComponent);
                    SceneView.RepaintAll();
                }

                if (cmp.copyLightComponent)
                {
                    EditorGUILayout.LabelField("  Copy Settings invokes parameters update every frame. Keep this performance implication in mind", new GUIStyle("flow node titlebar"));
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Copy From Light Component"))
                    {
                        // cmp.CopyFromLightComponentSettings();
                        cmps.ForEach(x => x.CopyFromLightComponentSettings());
                    }

                    // GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Copy To Light Component"))
                    {
                        // cmp.CopyToLightComponentSettings();
                        cmps.ForEach(x => x.CopyToLightComponentSettings());
                    }

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();

            ShowHeader("Volume");
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            cmp.lightShape = ShowEnumProperty(cmp.lightShape, "  Shape", cmp.CanCopyLightComponent);
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.lightShape = cmp.lightShape);
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            cmp.lightRange = ShowFloatProperty(cmp.lightRange, "  Range", cmp.CanCopyLightComponent);
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.lightRange = cmp.lightRange);
                SceneView.RepaintAll();
            }

            if (cmp.lightShape == LightShape.AreaHardEdge || cmp.lightShape == LightShape.UniformBox)
            {
                EditorGUI.BeginChangeCheck();
                cmp.lightRect = ShowVector2Property(cmp.lightRect, "  Rect", cmp.CanCopyLightComponent);
                if (EditorGUI.EndChangeCheck())
                {
                    cmps.ForEach(x => x.lightRect = cmp.lightRect);
                    SceneView.RepaintAll();
                }
            }

            switch (cmp.lightShape)
            {
                case LightShape.SpotSoftEdge:
                case LightShape.SpotHardEdge:

                    EditorGUI.BeginChangeCheck();
                    cmp.lightPrimaryAngle = ShowSliderProperty(cmp.lightPrimaryAngle, 0.0f, 179.9f, "  Outer Angle", cmp.CanCopyLightComponent);
                    if (EditorGUI.EndChangeCheck())
                    {
                        cmps.ForEach(x => x.lightPrimaryAngle = cmp.lightPrimaryAngle);
                        SceneView.RepaintAll();
                    }

                    EditorGUI.BeginChangeCheck();
                    cmp.lightSecondaryAngle = Mathf.Clamp(ShowSliderProperty(cmp.lightSecondaryAngle, 0.0f, 179.9f, "  Inner Angle", cmp.CanCopyLightComponent), 0.0f, cmp.lightPrimaryAngle - 1.0f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        cmps.ForEach(x => x.lightSecondaryAngle = cmp.lightSecondaryAngle);
                        SceneView.RepaintAll();
                    }

                    break;
                
                case LightShape.SpotHardEdgeSingle:
                    
                    EditorGUI.BeginChangeCheck();
                    cmp.lightPrimaryAngle = ShowSliderProperty(cmp.lightPrimaryAngle, 0.0f, 179.9f, "  Angle", cmp.CanCopyLightComponent);
                    if (EditorGUI.EndChangeCheck())
                    {
                        cmps.ForEach(x => x.lightPrimaryAngle = cmp.lightPrimaryAngle);
                        SceneView.RepaintAll();
                    }   

                    break;
                case LightShape.AreaHardEdge:
                     
                    EditorGUI.BeginChangeCheck();
                    cmp.mainLightAlignment = ShowBoolProperty(cmp.mainLightAlignment, "  Align to Main Light Direction");
                    if (EditorGUI.EndChangeCheck())
                    {
                        cmps.ForEach(x => x.mainLightAlignment = cmp.mainLightAlignment);
                        SceneView.RepaintAll();
                    }

                    if (!cmp.mainLightAlignment)
                    {
                        EditorGUI.BeginChangeCheck();
                        cmp.lightPrimaryAngle = ShowSliderProperty(cmp.lightPrimaryAngle, -89.9f, 89.9f, "  Shear Angle X", cmp.CanCopyLightComponent);
                        if (EditorGUI.EndChangeCheck())
                        {
                            cmps.ForEach(x => x.lightPrimaryAngle = cmp.lightPrimaryAngle);
                            SceneView.RepaintAll();
                        }  
                        
                        EditorGUI.BeginChangeCheck();
                        cmp.lightSecondaryAngle = ShowSliderProperty(cmp.lightSecondaryAngle, -89.9f, 89.9f, "  Shear Angle Y", cmp.CanCopyLightComponent);
                        if (EditorGUI.EndChangeCheck())
                        {
                            cmps.ForEach(x => x.lightSecondaryAngle = cmp.lightSecondaryAngle);
                            SceneView.RepaintAll();
                        }
                    }

                    break;
            }
            EditorGUILayout.Space();
            
            
            ShowHeader("Emission");
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            if (cmp.CanCopyLightComponent && GUI.enabled)
                GUI.enabled = false;
            // Conveniently taken from LightUI.Drawers.cs
            var colorTemperaturePopupValue = Convert.ToInt32(cmp.useColorTemperature);
            colorTemperaturePopupValue = EditorGUILayout.Popup(new GUIContent { text = "  Light Appearance" }, colorTemperaturePopupValue, LightUI.Styles.lightAppearanceOptions);
            GUI.enabled = true;
            cmp.useColorTemperature = Convert.ToBoolean(colorTemperaturePopupValue);
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.useColorTemperature = cmp.useColorTemperature);
                SceneView.RepaintAll();
            }

            if (!cmp.useColorTemperature)
            {
                EditorGUI.BeginChangeCheck();
                cmp.lightColor = ShowColorProperty(cmp.lightColor, "  Color", cmp.CanCopyLightComponent);
                if (EditorGUI.EndChangeCheck())
                {
                    cmps.ForEach(x => x.lightColor = cmp.lightColor);
                    SceneView.RepaintAll();
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                cmp.lightColor = ShowColorProperty(cmp.lightColor, "    Filter", cmp.CanCopyLightComponent);
                if (EditorGUI.EndChangeCheck())
                {
                    cmps.ForEach(x => x.lightColor = cmp.lightColor);
                    SceneView.RepaintAll();
                }
                
                if (cmp.CanCopyLightComponent && GUI.enabled)
                    GUI.enabled = false;

                // Conveniently taken from LightUI.Drawers.cs
                const int k_ValueUnitSeparator = 2;
                var lineRect = EditorGUILayout.GetControlRect();
                var labelRect = lineRect;
                labelRect.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(labelRect, "    Temperature");

                var temperatureSliderRect = lineRect;
                temperatureSliderRect.x += EditorGUIUtility.labelWidth + k_ValueUnitSeparator;
                temperatureSliderRect.width -= EditorGUIUtility.labelWidth + k_ValueUnitSeparator;
                
                // Value and unit label
                // Match const defined in EditorGUI.cs
                const int k_IndentPerLevel = 15;
                const int k_UnitWidth = 60 + k_IndentPerLevel;
                int indent = k_IndentPerLevel * EditorGUI.indentLevel;
                Rect valueRect = EditorGUILayout.GetControlRect();
                valueRect.width += indent - k_ValueUnitSeparator - k_UnitWidth;
                Rect unitRect = valueRect;
                unitRect.x += valueRect.width - indent + k_ValueUnitSeparator;
                unitRect.width = k_UnitWidth + .5f;
                EditorGUI.BeginChangeCheck();
                TemperatureSliderUIDrawer.Draw(lightSettings, serializedObject,
                    serializedObject.FindProperty("_lightColorTemperature"),
                    temperatureSliderRect);
                EditorGUI.PropertyField(valueRect, serializedObject.FindProperty("_lightColorTemperature"), CoreEditorStyles.empty);
                EditorGUI.LabelField(unitRect, "Kelvin");
                if (EditorGUI.EndChangeCheck())
                {
                    // This also invalidates value for the current component
                    cmps.ForEach(x => x.lightColorTemperature = cmp.lightColorTemperature);
                    cmps.ForEach(x => x.useColorTemperature = true);
                    SceneView.RepaintAll();
                }

                GUI.enabled = true;
            }

            EditorGUI.BeginChangeCheck();
            cmp.lightIntensity = ShowFloatProperty(cmp.lightIntensity, "  Intensity", cmp.CanCopyLightComponent);
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.lightIntensity = cmp.lightIntensity);
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            cmp.lightScattering = Mathf.Max(0.001f, ShowFloatProperty(cmp.lightScattering, "  Scattering"));
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.lightScattering = cmp.lightScattering);
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
            
            ShowHeader("Mask");
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            cmp.lightMask = ShowObjectProperty(cmp.lightMask, true, "  Mask Component");
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.lightMask = cmp.lightMask);
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
            
            ShowHeader("Culling");
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            cmp.distanceCullingEnabled = ShowBoolProperty(cmp.distanceCullingEnabled, "  Distance Culling");
            if (EditorGUI.EndChangeCheck())
            {
                cmps.ForEach(x => x.distanceCullingEnabled = cmp.distanceCullingEnabled);
                SceneView.RepaintAll();
            }

            if (cmp.distanceCullingEnabled)
            {
                EditorGUI.BeginChangeCheck();
                cmp.softCullingDistance = ShowFloatProperty(cmp.softCullingDistance, "    Soft Culling Distance");
                if (EditorGUI.EndChangeCheck())
                {
                    cmps.ForEach(x => x.softCullingDistance = cmp.softCullingDistance);
                    SceneView.RepaintAll();
                }

                EditorGUI.BeginChangeCheck();
                cmp.hardCullingDistance = ShowFloatProperty(cmp.hardCullingDistance, "    Hard Culling Distance");
                if (EditorGUI.EndChangeCheck())
                {
                    cmps.ForEach(x => x.hardCullingDistance = cmp.hardCullingDistance);
                    SceneView.RepaintAll();
                }
            }
            EditorGUILayout.Space();

            if (targets.Length > 1)
            {
                EditorGUILayout.LabelField("  Editing multiple components. Changes will be applied to each of them.",
                    new GUIStyle("flow node titlebar"));
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            
            cmps.ForEach(x => x.UpdateLightCacheForce());
        }
    }
}