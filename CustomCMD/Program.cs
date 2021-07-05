namespace Custom.Cmd
{
    class Program
    {
        static void Main()
        {
            ProcessLogicUI.SayHello();

            while (true)
            {
                ProcessLogicUI.ShowCommands();
                var continueProcess = ProcessLogicUI.Process();
                if (!continueProcess)
                {
                    break;
                }
            }

            ProcessLogicUI.SayBye();
        }
    }
}