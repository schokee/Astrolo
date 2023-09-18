namespace Astrolo.GeneKeys
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class MnemonicAttribute : Attribute
    {
        public MnemonicAttribute(string mnemonic)
        {
            Mnemonic = mnemonic;
        }

        public string Mnemonic { get; }
    }
}
