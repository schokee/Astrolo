using Caliburn.Micro;

namespace Astrolo.Presentation.Core.Components
{
    public class ProgressState : PropertyChangedBase, IProgress<string>
    {
        private string _description;
        private int _stepsCompleted;
        private int _totalSteps;

        public ProgressState(string description)
        {
            _description = description;
        }

        public string Description
        {
            get => _description;
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsIndeterminate => StepsCompleted < 0 || TotalSteps <= 0;

        public int PercentCompleted => IsIndeterminate
            ? 0
            : StepsCompleted < TotalSteps ? 100 * StepsCompleted / TotalSteps : 100;

        public int StepsCompleted
        {
            get => _stepsCompleted;
            set
            {
                if (value == _stepsCompleted) return;
                _stepsCompleted = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(IsIndeterminate));
                NotifyOfPropertyChange(nameof(PercentCompleted));
            }
        }

        public int TotalSteps
        {
            get => _totalSteps;
            set
            {
                if (value == _totalSteps) return;
                _totalSteps = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(IsIndeterminate));
                NotifyOfPropertyChange(nameof(PercentCompleted));
            }
        }

        public void Report(string value)
        {
            Description = value;
        }

        public override string ToString()
        {
            return Description;
        }

        public static implicit operator ProgressState(string description)
        {
            return new ProgressState(description);
        }
    }
}
