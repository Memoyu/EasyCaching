namespace EasyCaching.FasterKv.Internal
{
    public class FasterKvCacheValue
    {
        public FasterKvCacheValue()
        {
        }

        public FasterKvCacheValue(byte[] val, long time)
        {
            Value = val;
            Expiration = time;
        }

        public byte[] Value { get; set; }

        public long Expiration { get; set; }
    }
}
