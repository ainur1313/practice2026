using System;


namespace task17
{
    public static class ExceptionHandler
    {
        public static void Handle(ICommand command, Exception ex)
        {
            Console.WriteLine(
                $"[ExceptionHandler] Command: {command.GetType().Name}, " +
                $"Exception: {ex.GetType().Name}, " +
                $"Message: {ex.Message}");
        }
    }
}
