namespace Astrolo.YiJing;

public interface IChangeGenerator
{
    Movement Generate(Func<int, int> random);
}
