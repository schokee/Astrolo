namespace Astrolo.GeneKeys;

[AttributeUsage(AttributeTargets.All)]
public sealed class MnemonicAttribute(string mnemonic) : Attribute
{
    public string Mnemonic { get; } = mnemonic;
}
