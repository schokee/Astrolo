using Astrolo.Explorer.Model;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Filtering
{
    public class ProfileFilter : HexagramFilter
    {
        private const string NoProfile = "(none)";

        private GeneKeyProfile _profile;

        public ProfileFilter() : base(NoProfile)
        {
            Category = "Hologenetic Profile";
        }

        public GeneKeyProfile Profile
        {
            get => _profile;
            set
            {
                if (Set(ref _profile, value))
                {
                    Label = Profile?.Name ?? NoProfile;
                }
            }
        }

        public override bool Includes(HexagramFigure figure)
        {
            return Profile?.Points.Any(profilePoint => profilePoint.GeneKey.Number == figure.Number) != false;
        }
    }
}
