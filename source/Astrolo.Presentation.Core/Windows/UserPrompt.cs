using System.Windows;

namespace Astrolo.Presentation.Core.Windows
{
    public class UserPrompt : IUserPrompt
    {
        public void Inform(string message, string? caption = null)
        {
            Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Question);
        }

        public void Warn(string message, string? caption = null)
        {
            Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Alert(string message, string? caption = null)
        {
            Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public UserResponse Ask(string message, string? caption = null)
        {
            return Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes
                ? UserResponse.Accept
                : UserResponse.Reject;
        }

        public UserResponse AskOrCancel(string message, string? caption = null)
        {
            switch (Show(message, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    return UserResponse.Accept;
                case MessageBoxResult.No:
                    return UserResponse.Reject;
                default:
                    return UserResponse.Cancel;
            }
        }

        public UserResponse Confirm(string message, string? caption = null)
        {
            switch (Show(message, caption, MessageBoxButton.OKCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.OK:
                    return UserResponse.Accept;
                default:
                    return UserResponse.Cancel;
            }
        }

        protected virtual MessageBoxResult Show(string messageBoxText, string? caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return MessageBox.Show(messageBoxText, caption, button, icon);
        }
    }
}
