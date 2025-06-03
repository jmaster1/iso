namespace Common.Lang
{
    /// <summary>
    /// responsible for converting object to string and back
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    public interface IStringConverter<T>//TODO : IConverter<T, string>
    {
        string ToString(T val);

        T FromString(string str);
    }
}