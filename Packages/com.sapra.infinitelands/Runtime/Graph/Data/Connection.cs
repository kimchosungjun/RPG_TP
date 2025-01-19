using System;

namespace sapra.InfiniteLands
{
    [Serializable]
    public struct Connection
    {
        public InfiniteLandsNode provider;
        public string providerPortName;
        public string inputPortName;
        public bool isValid => provider != null && !providerPortName.Equals("") && !inputPortName.Equals("");
    }
}