namespace SolEx.Hurt.Model.Helpers
{
    public class CustomKeyValuePair<TKey, TValue>
    {
        private TKey _key;
        private TValue _value;
        public CustomKeyValuePair() { }
        public CustomKeyValuePair(TKey key, TValue value):this()
        {
            _key = key;
            _value = value;
        }
        public TKey Key { get { return _key; } set { _key = value; } }

        public TValue Value { get { return _value; } set { _value = value; } }

    }
    public class CustomKeyValuePair2<TKey, TValue, TValue2, TValue3>
    {
        private TValue2 _value2;
        private TKey _key;
        private TValue _value;
        private TValue3 _value3;
        public CustomKeyValuePair2() { }
        public CustomKeyValuePair2(TKey key, TValue value,TValue2 value2,TValue3 value3):this()
        {
            _key = key;
            _value = value;
            _value2 = value2;
            _value3 = value3;
        }
        public TKey Key { get { return _key; } set { _key = value; } }
        public TValue Value { get { return _value; } set { _value = value; } }
        public TValue2 Value2 { get { return _value2; } set { _value2 = value; } }
        public TValue3 Value3 { get { return _value3; } set { _value3 = value; } }
    }
}
