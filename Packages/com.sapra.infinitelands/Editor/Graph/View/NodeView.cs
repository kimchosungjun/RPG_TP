using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{

    public class NodeView : Node, IRenderSerializableGraph
    {
        public InfiniteLandsNode node;
        public object GetDataToSerialize()=> node;
        Vector2 IRenderSerializableGraph.GetPosition()=> node.position;
        public string GetGUID()=> node.guid;

        public List<Port> ports;
        private Action recheck;
        public SerializedObject obj;

        private Image Preview;
        public NodeView(InfiniteLandsNode node)
        {
            this.node = node;
            obj = new SerializedObject(this.node);

            CustomNodeAttribute attribute = GetAttribute(node.GetType());
            if (!attribute.canCreate)
            {
                capabilities -= Capabilities.Deletable;
            }

            AddToClassList(attribute.type);
            this.title = attribute.name;
            this.viewDataKey = node.guid;
            style.left = node.position.x;
            style.top = node.position.y;
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.sapra.infinitelands/Editor/UIBuilder/NodeView.uss"));

            SetPosition(new Rect(node.position, Vector3.zero));
            CreateOutputs();
            CreateInputs();

            node.OnValuesUpdated -= CheckExtension;
            node.OnValuesUpdated += CheckExtension;

            VisualElement test = mainContainer.Q<VisualElement>("collapse-button");
            test.RegisterCallback<ClickEvent>(OnBoxClicked);

            //WORKAROUND for uncollapsable nodes 
            Port hiddenPort =
                InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(int));
            hiddenPort.style.display = DisplayStyle.None;
            if (inputContainer.childCount > 0)
                inputContainer.Add(hiddenPort);
            else
                outputContainer.Add(hiddenPort);
            //End of workaround

            recheck = null;
            FillExtensionContainer(attribute);
        }

        
        private void OnBoxClicked(ClickEvent evt)
        {
            // Only perform this action at the target, not in a parent
            if (evt.target != this)
            return;

            node.expanded = expanded;
        }
        protected void CheckExtension(){
            recheck?.Invoke();
            RefreshExpandedState();            
            RefreshPorts();
        }
        private void FillExtensionContainer(CustomNodeAttribute attribute){
            extensionContainer.Clear();
            DrawParameters();
            VisualElement buttons = new VisualElement();
            buttons.AddToClassList("more_buttons_layout");
            extensionContainer.Add(buttons);

            if (node is IHavePreview textureMaker && textureMaker.PreviewButtonEnabled())
                DrawPreview(buttons, textureMaker);
            if(attribute != null && !attribute.docs.Equals(""))
                DrawDocs(buttons, attribute.docs);

            RefreshExpandedState();            
            RefreshPorts();
        }
        protected void DrawPreview(VisualElement parent, IHavePreview generator)
        {
            Toggle GenerateImageFoldout = new Toggle();
            GenerateImageFoldout.AddToClassList("image-foldout");
            GenerateImageFoldout.AddToClassList("more_button");
            GenerateImageFoldout.SetValueWithoutNotify(generator.ShowPreview());
            GenerateImageFoldout.RegisterValueChangedCallback(a => {
                generator.TogglePreview(a.newValue, true);
            });
            parent.Add(GenerateImageFoldout);

            VisualElement imagePreview = PreviewVisualElement(generator);
            imagePreview.AddToClassList("Preview");
            
            ChangeImage(GenerateImageFoldout,imagePreview, generator.ShowPreview());
            generator.OnImageUpdated += (state) => {
                GenerateImageFoldout.SetValueWithoutNotify(state);
                UpdatePreview(generator);
                ChangeImage(GenerateImageFoldout,imagePreview, state);
            };

            extensionContainer.Add(imagePreview);
        }

        protected virtual VisualElement PreviewVisualElement(IHavePreview generator){
            Preview = new Image()
            {
                scaleMode = ScaleMode.ScaleToFit,
                image = generator.GetTemporalTexture() as Texture2D
            };
            return Preview;
        }

        protected virtual void UpdatePreview(IHavePreview generator){
            Preview.image = generator.GetTemporalTexture() as Texture2D;
        }


        private void ChangeImage(VisualElement GenerateImageFoldout, VisualElement image, bool value){
            string add = value.ToString();
            string remove = (!value).ToString();
            
            GenerateImageFoldout.RemoveFromClassList(remove);
            GenerateImageFoldout.AddToClassList(add);
            image.RemoveFromClassList(remove);
            image.AddToClassList(add);

        }
        protected void DrawDocs(VisualElement parent, string url)
        {
            Button OpenDocs = new Button(()=>{
                Application.OpenURL(url);
            });
            OpenDocs.AddToClassList("documentation");
            OpenDocs.AddToClassList("more_button");

            parent.Add(OpenDocs);
        }

        public CustomNodeAttribute GetAttribute(Type type)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attribute in attrs)
            {
                if (attribute is CustomNodeAttribute customAttribute)
                {
                    return customAttribute;
                }
            }

            return null;
        }

        protected virtual void CreateOutputs()
        {
            ports = new List<Port>();
            Type nodeT = node.GetType();

            Type[] interfaces = nodeT.GetInterfaces()
                .Where(a => a.IsGenericType && a.GetGenericTypeDefinition().Equals(typeof(IGive<>))).ToArray();
            foreach (Type inter in interfaces)
            {
                Type[] giver = inter.GetGenericArguments();
                foreach (Type gives in giver)
                {
                    Port output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, inter);
                    string[] parts = gives.ToString().Split('.');
                    string assetName = parts[parts.Length-1];
                    output.AddToClassList(assetName);
                    output.styleSheets.Add(
                        AssetDatabase.LoadAssetAtPath<StyleSheet>(
                            "Packages/com.sapra.infinitelands/Editor/UIBuilder/Port.uss"));

                    //string nameReady = ObjectNames.NicifyVariableName(assetName);
                    if (output != null)
                    {
                        output.portName = assetName;
                        outputContainer.Add(output);
                        ports.Add(output);
                    }
                }
            }
        }

        protected virtual void CreateInputs()
        {
            Type nodeT = node.GetType();
            FieldInfo[] fields = nodeT.GetFields(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] inputFields = fields.Where(a => a.GetCustomAttribute<InputAttribute>() != null).ToArray();

            foreach (FieldInfo field in inputFields)
            {
                InputAttribute attribute = field.GetCustomAttribute<InputAttribute>();
                Port input;
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi,
                        attribute.type);
                else
                    input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                        attribute.type);

                string className = attribute.type.ToString();
                if (attribute.type.GenericTypeArguments.Length > 0)
                    className = attribute.type.GenericTypeArguments[0].ToString();
                
                string[] parts = className.ToString().Split('.');
                string assetName = parts[parts.Length-1];

                input.AddToClassList(assetName);
                input.styleSheets.Add(
                    AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.sapra.infinitelands/Editor/UIBuilder/Port.uss"));
                if (attribute.optional)
                    input.AddToClassList("optional");
            
                if (input != null)
                {
                    input.portName = field.Name;
                    inputContainer.Add(input);
                    ports.Add(input);
                }
            }
        }

        protected virtual void DrawParameters()
        {
            FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttribute<HideInInspector>() != null ||
                    field.GetCustomAttribute<InputAttribute>() != null)
                    continue;
                
                VisualElement propertyField = GenerateProperty(field);
                ShowIfCustomAttribute attr = field.GetCustomAttribute<ShowIfCustomAttribute>();
                if(attr != null){
                    recheck += ()=> EnableOrDisableField(propertyField, attr, field.Name);
                }
                propertyField.Bind(obj);
                extensionContainer.Add(propertyField);
            }
            recheck?.Invoke();
        }

        private VisualElement GenerateProperty(FieldInfo field){
            string nameReady = ObjectNames.NicifyVariableName(field.Name);
            MinMaxCustomAttribute MinMaxAttribute = field.GetCustomAttribute<MinMaxCustomAttribute>();

            VisualElement propertyField;
            if(MinMaxAttribute != null){
                propertyField = new VisualElement();
                MinMaxSlider slider = new MinMaxSlider(nameReady, MinMaxAttribute.minValue, MinMaxAttribute.maxValue, MinMaxAttribute.minValue, MinMaxAttribute.maxValue)
                {
                    bindingPath = field.Name
                };
                Vector2Field value = new Vector2Field()
                {
                    label = "",
                    bindingPath = field.Name,
                };

                value.RegisterValueChangedCallback(a=>{
                    Vector2 clamped = new Vector2(Mathf.Max(MinMaxAttribute.minValue, a.newValue.x), Mathf.Min(MinMaxAttribute.maxValue, a.newValue.y));
                    value.SetValueWithoutNotify(clamped);
                });

                propertyField.Add(slider);
                propertyField.Add(value);
            } 
            else
            {

                propertyField = new PropertyField()
                {
                    label = nameReady,
                    bindingPath = field.Name
                };
            }
            return propertyField;
        }
        private void EnableOrDisableField(VisualElement propField, ShowIfCustomAttribute attr, string flName){
            var propertyInfo = node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault(a => a.Name.Equals(attr.propertyName));
            var fieldInfo = node.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault(a => a.Name.Equals(attr.propertyName));

            if(propertyInfo == default && fieldInfo == default)
                Debug.LogWarning("Wrong variable set for the ShowIf attribute in " + node.GetType()+" "+flName);
            else{
                bool value;
                if(propertyInfo != default)
                    value = (bool)propertyInfo.GetValue(node, null);
                else 
                    value = (bool)fieldInfo.GetValue(node);

                propField.visible = value;
                if(!value){
                    propField.style.height = 0;
                    propField.style.width = 0;
                }
                else{
                    propField.style.height = new StyleLength(StyleKeyword.Auto);
                    propField.style.width = new StyleLength(StyleKeyword.Auto);
                }
            }
        }
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, string.Format("{0}: {1}", "Moved", node.name));
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
        }
    }
}