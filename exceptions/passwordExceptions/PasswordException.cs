namespace PeyulErp.Exceptions
{
    public class PasswordException : Exception
    {
        public PasswordException() { }
        public PasswordException(string message) : base(message) { }
    }
}