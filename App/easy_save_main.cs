namespace EasySave
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("t'es dans le main");



            var logger = new EasySaveLogger("logs", "C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\");
            logger.logSaveFolderFilesProgression("Save1", "a", "b", "end", 0, 0, 0);
            
            
           /* var copy = new EasySaveFileSaver("C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\sourceRep\\", "C:\\Users\\gauti\\OneDrive\\Bureau\\CESI\\A3\\Semestre 1\\Programation système\\Projet\\TestEnv\\destinationRep\\", "Save-test", "Complete");
            copy.saveProcess("Incremental");
            //copy.saveProcess("Complete");
           */

            Console.ReadKey();
        }
    }
}