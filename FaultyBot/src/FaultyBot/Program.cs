namespace FaultyBot
{
    public class Program
    {
        public static void Main(string[] args) => 
            new FaultyBot().RunAndBlockAsync(args).GetAwaiter().GetResult();
    }
}
