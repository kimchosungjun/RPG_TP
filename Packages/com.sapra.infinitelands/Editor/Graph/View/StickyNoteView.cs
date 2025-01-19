using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace sapra.InfiniteLands.Editor{

    public class StickyNoteView : StickyNote, IRenderSerializableGraph
    {
        public object GetDataToSerialize() => note;
        Vector2 IRenderSerializableGraph.GetPosition() => note.position;
        public string GetGUID() => note.guid;
        public StickyNoteBlock note{get; private set;}
        public StickyNoteView(StickyNoteBlock note):base(note.position){
            this.note = note;
        }

        public override bool IsGroupable() => true;
        public override bool IsCopiable() => true;

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            if(note == null)
                return;
            note.position.x = newPos.xMin;
            note.position.y = newPos.yMin;
        }

    }
}