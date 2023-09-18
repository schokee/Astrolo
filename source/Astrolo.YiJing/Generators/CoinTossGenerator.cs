namespace Astrolo.YiJing.Generators
{
    public sealed class CoinTossGenerator : IChangeGenerator
    {
        public Movement Generate(Func<int, int> random)
        {
            Line RandomLine() => random(2) % 2 == 1;

            return new Movement(RandomLine(), RandomLine());
        }

        public override string ToString()
        {
            return "Coin Toss";
        }
    }
}
