using FirebaseAdmin.Messaging;

namespace JARS_API.Utilities
{
    /// <summary>
    /// Helps creating a FCM Message.
    /// While the Notification or Data payload is not required for the FCM Message to be valid, not supplying any of them will result in the client not receiving any information.
    /// </summary>
    public class MessageBuilder
    {
        private Message? _message = null;

        public MessageBuilder()
        {
            _message = new Message();
        }

        /// <summary>
        /// Returns the FCM Message.
        /// </summary>
        /// <returns>Message</returns>
        public Message Build()
        {
            if (_message == null)
            {
                return new Message();
            }
            return _message;
        }

        /// <summary>
        /// Add a FcmToken to the FCM Message. FcmToken is required for a FCM Message to be valid.
        /// </summary>
        /// <param name="fcmToken"></param>
        /// <returns>MessageBuilder</returns>
        public MessageBuilder AddToken(string fcmToken)
        {
            _message!.Token = fcmToken;
            return this;
        }

        /// <summary>
        /// Set the Notification payload for the FCM Message. Optional.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns>MessageBuilder</returns>
        public MessageBuilder AddNotification(Notification notification)
        {
            _message!.Notification = notification;
            return this;
        }

        /// <summary>
        /// Set the Data payload for the FCM Message. Optional.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>MessageBuilder</returns>
        public MessageBuilder AddData(Dictionary<string, string> data)
        {
            _message!.Data = data;
            return this;
        }
    }
}
