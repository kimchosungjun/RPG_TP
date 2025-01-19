
namespace sapra.InfiniteLands
{
    public struct IndexAndResolution{
        public int Index;
        public int Resolution;
        public int Length;

        public IndexAndResolution(int index, int resolution){
            Index = index;
            Resolution = resolution;
            Length = MapTools.LengthFromResolution(Resolution);
        }
    }
}