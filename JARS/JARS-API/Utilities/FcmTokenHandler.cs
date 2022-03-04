using FirebaseAdmin.Messaging;
using JARS_DAL.Models;
using JARS_DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace JARS_API.Utilities
{
    public static class FcmTokenHandler
    {
        private static IAccountDeviceRepository? _accountDeviceRepository = new AccountDeviceRepository();

        /// <summary>
        /// This method should be used after invoking SendMessagesAsync of FirebaseCloudMessagingUtility to delete any invalid tokens.
        /// </summary>
        /// <param name="response">BatchReponse is returned from SendMessagesAsync.</param>
        /// <param name="messages">List of Messages which were used as parameter in SendMessagesAsync.</param>
        /// <returns></returns>
        public static async Task HandleBatchResponse(BatchResponse response, List<Message> messages)
        {
            if (response.SuccessCount > 0 && response.FailureCount == 0)
            {
                return;
            }
            if (response.FailureCount > 0)
            {
                await HandleInvalidTokens(GetInvalidTokenIndexes(response), messages);
            }
        }

        private static List<int> GetInvalidTokenIndexes(BatchResponse response)
        {
            List<int> invalidIndexes = new List<int>();
            for (int i = 0; i < response.Responses.Count; i++)
            {
                if (response.Responses[i].Exception != null)
                {
                    invalidIndexes.Add(i);
                }
            }
            return invalidIndexes;
        }

        private static async Task HandleInvalidTokens(List<int> invalidIndexes, List<Message> messages)
        {
            List<string> invalidTokens = new List<string>();
            foreach (int invalidIndex in invalidIndexes)
            {
                invalidTokens.Add(messages[invalidIndex].Token);
            }
            IEnumerable<AccountDevice?> accountDevices = await _accountDeviceRepository!.GetListFromTokensAsync(invalidTokens);
            try
            {
                await _accountDeviceRepository.DeleteListAsync(accountDevices);
            }
            catch (DbUpdateConcurrencyException)
            {
                // to-do logging
            }
            catch (DbUpdateException)
            {
                // to-do logging
            }
            catch (Exception)
            {
                // to-do logging
            }
        }
    }
}
