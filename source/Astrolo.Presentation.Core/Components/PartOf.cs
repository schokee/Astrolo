using System.Xml.Serialization;
using Caliburn.Micro;

namespace Astrolo.Presentation.Core.Components
{
    [Serializable]
    public class PartOf<TContainer> : PropertyChangedBase, IPart
    {
        [XmlIgnore]
        public TContainer? Container { get; private set; }

        object? IPart.Container
        {
            get => Container;
            set
            {
                if (ReferenceEquals(value, Container)) return;

                var oldContainer = Container;

                Container = (TContainer?) value;
                NotifyOfPropertyChange();

                OnContainerChanged(oldContainer, Container);
            }
        }

        protected virtual void OnContainerChanged(TContainer? oldContainer, TContainer? newContainer)
        {
        }
    }
}
