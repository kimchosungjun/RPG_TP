namespace sapra.InfiniteLands
{
    //Output final node that has the exact same resolution output as the settings define
    public interface IOutput
    {
        public int PrepareNode(IndexManager manager,ref int currentCount, int resolution, float ratio, string requestGuid);
    }
}