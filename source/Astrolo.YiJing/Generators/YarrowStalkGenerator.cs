namespace Astrolo.YiJing.Generators;

public sealed class YarrowStalkGenerator : IChangeGenerator
{
    public Movement Generate(Func<int, int> random)
    {
        static int Residual(int x)
        {
            var result = x % 4;
            return result == 0 ? 4 : result;
        }

        var stickCount = 49;

        for (var j = 0; j < 3; ++j)
        {
            stickCount -= stickCount % 2;

            var cutPoint = random(stickCount);
            var fromL = Residual(cutPoint);
            var fromR = Residual(stickCount - cutPoint);

            stickCount -= fromL + fromR;
        }

        switch (stickCount)
        {
            case 24:
                return new Movement(true, false);
            case 28:
                return new Movement(true, true);
            case 32:
                return new Movement(false, false);
            case 36:
                return new Movement(false, true);
            default:
                throw new ArgumentOutOfRangeException(nameof(stickCount));
        }
    }

    public override string ToString()
    {
        return "Yarrow Stalks";
    }
}
