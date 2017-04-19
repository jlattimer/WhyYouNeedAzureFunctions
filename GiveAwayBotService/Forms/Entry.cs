using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;

namespace GiveAwayBotService.Forms
{
    public enum PhoneType { Apple, Android, Windows }

    [Serializable]
    public class Entry
    {
        public string Name;
        public PhoneType? Phone;

        public static IForm<Entry> BuildForm()
        {
            OnCompletionAsyncDelegate<Entry> processEntry = async (context, state) =>
            {
                await context.PostAsync("We are currently processing your entry. We will message you the status.");
            };

            return new FormBuilder<Entry>()
                .Message("Sign up for the give away")
                .OnCompletion(processEntry)
                .Build();
        }
    }
}