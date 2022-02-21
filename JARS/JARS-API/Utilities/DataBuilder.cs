namespace JARS_API.Utilities
{
    /// <summary>
    /// Helps creating a Data payload for a FCM Message.
    /// </summary>
    public class DataBuilder
    {
        private Dictionary<string, string>? _data = null;

        public DataBuilder()
        {
            _data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns the Data payload.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> Build()
        {
            if (_data == null)
            {
                return new Dictionary<string, string>();
            }
            return _data;
        }

        /// <summary>
        /// Add an entry into the Data payload.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataBuilder AddData(string key, string value)
        {
            _data!.Add(key, value);
            return this;
        }
    }
}
