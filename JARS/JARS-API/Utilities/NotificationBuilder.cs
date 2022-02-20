using FirebaseAdmin.Messaging;

namespace JARS_API.Utilities
{
    /// <summary>
    /// Helps creating a Notification payload for a FCM Message.
    /// </summary>
    public class NotificationBuilder
    {
        private Notification? _notification = null;

        public NotificationBuilder()
        {
            _notification = new Notification();
        }

        /// <summary>
        /// Returns the Notification payload.
        /// </summary>
        /// <returns></returns>
        public Notification Build()
        {
            if (_notification == null)
            {
                return new Notification();
            }
            return _notification;
        }

        /// <summary>
        /// Sets the title of the Notification. Optional.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public NotificationBuilder AddTitle(string title)
        {
            _notification!.Title = title;
            return this;
        }

        /// <summary>
        /// Sets the body of the Notification. Optional.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public NotificationBuilder AddBody(string body)
        {
            _notification!.Body = body;
            return this;
        }

        /// <summary>
        /// Sets the image address of the Notification. Optional.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public NotificationBuilder AddImageUrl(string imageUrl)
        {
            _notification!.ImageUrl = imageUrl;
            return this;
        }
    }
}
