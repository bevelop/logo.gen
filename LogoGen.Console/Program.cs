namespace LogoGen.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("Please specify the settings file path as the first argument.");
                return;
            }

            var results = new LogoGenerator().GenerateBatch(args[0]);

            foreach (var r in results)
            {
                if (!r.Succeeded)
                    System.Console.WriteLine(r.Exception.ToString());
            }

            System.Console.WriteLine("Finished");
        }
    }
}
