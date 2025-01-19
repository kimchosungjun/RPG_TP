using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace sapra.InfiniteLands
{
    [System.Serializable]
    public abstract class InfiniteLandsNode : UpdateableSO
    {
        [HideInInspector] public string guid;
        [HideInInspector] public bool expanded = true;
        [HideInInspector] public Vector2 position;
        [SerializeField] protected List<Connection> connections = new List<Connection>();

        public bool AddConnection(Connection connection)
        {
            FieldInfo[] inputFields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(a => a.GetCustomAttribute<InputAttribute>() != null).ToArray();
            
            FieldInfo canConnect = IsValidConnection(inputFields, connection);
            if (canConnect != null)
                SetConnection(canConnect, connection);
            return canConnect != null;
        }
        public void ClearConnections(){
            connections.Clear();
        }

        private void SetConnection(FieldInfo validField, Connection connection)
        {
            if(validField == null)
                return;

            if (validField.GetValue(this) is List<InfiniteLandsNode> list)
            {
                list.Add(connection.provider);
                validField.SetValue(this, list);
            }
            else{
                if(connections.Any(a => a.inputPortName.Equals(connection.inputPortName))){
                    Connection existingConnection = connections.First(a => a.inputPortName.Equals(connection.inputPortName));
                    RemoveConnection(existingConnection);
                }

                validField.SetValue(this, connection.provider);
            }
            connections.Add(connection);
        }
        

        private FieldInfo IsValidConnection(FieldInfo[] inputFields, Connection connection){
            FieldInfo field = inputFields.FirstOrDefault(a => a.Name.Equals(connection.inputPortName));
            if (field != default && connection.isValid)
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(this) is List<InfiniteLandsNode> list)
                    {
                        if(!list.Contains(connection.provider))
                            return field;
                        else
                            return null;
                    }
                    else{
                        Debug.LogError(field.FieldType + " is not supported, use List<InfiniteLandsNode> instead");
                        return null;
                    }
                }
                return field;
            }

            return null;
        }

        public int GetRandomIndex(){
            return guid.GetHashCode();
        }
        
        public void RemoveConnection(InfiniteLandsNode provider, string providerPortName, string inputPortName)
        {
            Connection targetConnection = connections.Find(a =>
                a.provider.Equals(provider) && a.providerPortName.Equals(providerPortName) &&
                a.inputPortName.Equals(inputPortName));

            RemoveConnection(targetConnection);
        }

        public void RemoveConnection(Connection connection)
        {
            FieldInfo[] inputFields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(a => a.GetCustomAttribute<InputAttribute>() != null).ToArray();
            FieldInfo field = inputFields.FirstOrDefault(a => a.Name.Equals(connection.inputPortName));
            if (field != default)
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(this) is List<InfiniteLandsNode> list)
                    {
                        list.Remove(connection.provider);
                        field.SetValue(this, list);
                    }
                    else
                        Debug.LogError(field.FieldType + " is not supported, use List<InfiniteLandsNode> instead");
                }
                else
                    field.SetValue(this, null);
            }

            connections.Remove(connection);
        }

        public void ClearInvalidConnections()
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (!connections[i].isValid)
                    connections.RemoveAt(i);
            }
            FieldInfo[] inputFields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(a => a.GetCustomAttribute<InputAttribute>() != null).ToArray();
            foreach (FieldInfo field in inputFields)
            {
                if (typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                    if (field.GetValue(this) is List<InfiniteLandsNode> list)
                        list.Clear();
            }

            List<Connection> corrected = new List<Connection>(connections);
            connections.Clear();
            foreach (Connection connection in corrected)
            {
                FieldInfo validField = IsValidConnection(inputFields, connection);
                if (validField != null)
                    SetConnection(validField,connection);
            }
        }

        public List<Connection> GetConnections()
        {
            ClearInvalidConnections();
            return connections;
        }

        #region Validation
        protected abstract InfiniteLandsNode[] Dependancies{get;} // Return all the dependancies used so that they are validated that are correctly set
        protected virtual bool ExtraValidationSteps() => true; // Extra step used after the dependancy check so that node can ensure that a node is correctly set

        public bool isValid{get; private set;}
        public void ValidationCheck(){
            bool isThisValid = PreInitilizeVerification();
            if(isThisValid){
                isThisValid = ChainDependencyCheck(this);
            }
            if(isThisValid){
                isThisValid = PostInitilizeVerification();
            }

            isValid = isThisValid;
        }

        private bool PreInitilizeVerification(){
            foreach(InfiniteLandsNode dep in Dependancies){
                if(dep == null){
                    Debug.LogWarningFormat("{0} is missing a connection", this.name);
                    return false;
                }
            }
            
            if(!ExtraValidationSteps())
                return false;

            return true;
        }
        private bool ChainDependencyCheck(InfiniteLandsNode og){
            foreach(InfiniteLandsNode dep in Dependancies){
                if(dep != null && og != null){
                    if(!dep.Equals(og)){
                        if(!dep.ChainDependencyCheck(og))
                            return false;
                    }
                    else{
                        Debug.LogErrorFormat("There's a loop in relation to {0}, please Fix to generate", dep.name);
                        return false;
                    }
                }
            }
            return true;
        }
        private bool PostInitilizeVerification(){
            foreach(InfiniteLandsNode dep in Dependancies){
                dep.ValidationCheck();
                if(!dep.isValid){
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}