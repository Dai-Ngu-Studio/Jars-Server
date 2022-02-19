using FirebaseAdmin.Messaging;

namespace JARS_API.Utilities
{
    public static class FirebaseCloudMessagingUtility
    {
        /// <summary>
        /// Sends a message to the FCM service for it to be delivered to the client app.
        /// </summary>
        /// <param name="message">Message must include a FcmToken.</param>
        public static async void SendMessageAsync(Message message)
        {
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (FirebaseMessagingException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends all messages in the given list. Up to 100 messages can be batched.
        /// Messages are not required to have the same FcmToken, Notification or Data.
        /// For example, two different messages with two different FcmTokens can be in the same list.
        /// This is the preferred method for sending multiple messages efficiently.
        /// </summary>
        /// <param name="messages">Each Message must include a FcmToken.</param>
        public static async void SendMessagesAsync(List<Message> messages)
        {
            try
            {
                BatchResponse response = await FirebaseMessaging.DefaultInstance.SendAllAsync(messages);
            }
            catch (FirebaseMessagingException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
