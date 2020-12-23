namespace DataLayer.Interfaces
{
    public interface IConfigurationDataRepository
    {
        void AddUpdateAppSettings(string key, string value);
    }
}
