using Astrolo.Explorer.Model;

namespace Astrolo.Explorer.Messages;

public sealed class ActiveProfileChanged
{
    public ActiveProfileChanged(GeneKeyProfile geneKeyProfile)
    {
            GeneKeyProfile = geneKeyProfile;
        }

    public GeneKeyProfile GeneKeyProfile { get; }
}
