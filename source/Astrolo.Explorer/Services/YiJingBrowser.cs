using Astrolo.YiJing;

namespace Astrolo.Explorer.Services;

internal class YiJingBrowser : IYiJingBrowser
{
    public void ViewInfo(HexagramFigure hexagram)
    {
        var uri = new Uri($"https://divination.com/iching/lookup/{hexagram.Number}-2/");
        uri.Browse();
    }

    public void ViewReading(IEnumerable<Movement> change)
    {
        var code = string.Concat(change.Select(x => x.IsChange
            ? x.StartState.IsYang ? "9" : "6"
            : x.StartState.IsYang ? "7" : "8"));

        var uri = new Uri($"https://www.onlineclarity.co.uk/reading/free-online-i-ching/your-reading/?lines={code}&text=barrett");
        uri.Browse();
    }
}
