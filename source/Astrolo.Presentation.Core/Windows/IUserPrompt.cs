namespace Astrolo.Presentation.Core.Windows
{
    public enum UserResponse
    {
        Cancel,
        Accept,
        Reject
    }

    public interface IUserPrompt
    {
        void Inform(string message, string? caption = null);

        void Warn(string message, string? caption = null);

        void Alert(string message, string? caption = null);

        UserResponse Ask(string message, string? caption = null);

        UserResponse AskOrCancel(string message, string? caption = null);

        UserResponse Confirm(string message, string? caption = null);
    }
}
