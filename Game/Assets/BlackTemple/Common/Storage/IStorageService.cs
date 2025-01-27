namespace BlackTemple.Common
{
    public interface IStorageService
    {
        void Save(string key, object data);

        T Load<T>(string key);

        T LoadDefault<T>(string key);

        void Remove(string key);

        void Clear();

        bool IsDataExists(string key);
    }
}