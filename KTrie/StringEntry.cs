namespace KTrie
{
    public struct StringEntry<TValue>
    {
        public StringEntry(string key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public TValue Value { get; }
    }
}