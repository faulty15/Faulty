namespace FaultyBot.Services
{
    public interface ILocalization
    {
        string this[string key] { get; }
    }
}
