using System.Collections.Generic;

namespace sapra.InfiniteLands{
    public abstract class ProcessableData{
        private List<object> processors = new();
        private DataManager manager;
        private string id;
        public ProcessableData(DataManager manager, string idData){
            this.manager = manager;
            this.id = idData;
        }
        public void AddProcessor(object processor){
            processors.Add(processor);
        }

        public void RemoveProcessor(object processor){
            processors.Remove(processor);
            if(processors.Count <= 0){
                manager?.Return(id);
            }
        }
    }
}