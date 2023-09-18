namespace Astrolo.YiJing
{
    public static class ModelExtensions
    {
        public static BitArray ToBitArray(this Trigram trigram)
        {
            return new BitArray(TrigramInfo.TotalLines, (uint)trigram);
        }
    }
}
