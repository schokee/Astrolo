using Caliburn.Micro;

namespace Astrolo.Presentation.Core.Components
{
    public static class PartExtensions
    {
        public static IEnumerable<object> SelectAncestors(this IPart part)
        {
            for (var container = part?.Container; container != null; container = (container as IPart)?.Container)
            {
                yield return container;
            }
        }

        public static async Task NotifyAncestorsAsync<TMessage>(this IPart part, TMessage message)
        {
            foreach (var handler in part.SelectAncestors().OfType<IHandle<TMessage>>())
            {
                await handler.HandleAsync(message, CancellationToken.None);
            }
        }
    }
}
