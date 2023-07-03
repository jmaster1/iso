namespace Common.Lang
{
    /// <summary>
    /// responsible for converting object to string and back
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    public interface IStringConverter<T>
    {
        string ToString(T val);

        T FromString(string str);
    }
}