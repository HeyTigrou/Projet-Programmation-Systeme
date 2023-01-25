namespace Poc
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, World!");

            /*
            var logger = new LoggerClass("logs", "C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\");
            logger.makeJson("Save1", "a", "b", "end", 0, 0, 0, 0);
            */
            var copy = new ProcessClass("C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\sourceRep\\", "C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\destinationRep\\", "Save-test", "Complete");
            copy.process("Incremental");
            Console.ReadKey();
        }
    }
}