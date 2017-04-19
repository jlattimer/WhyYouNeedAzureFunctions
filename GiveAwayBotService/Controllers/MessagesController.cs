using GiveAwayBotService.Forms;
using GiveAwayBotService.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace GiveAwayBotService.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        internal static IDialog<Entry> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(Entry.BuildForm))
                .Do(async (context, entry) =>
                {
                    try
                    {
                        var completed = await entry;

                        TableHelper.AddEntryToTable(completed.Name, completed.Phone.ToString(), context.Activity.From.Id,
                            context.Activity.Conversation.Id, context.Activity.Recipient.Id);

                        System.Threading.Thread.Sleep(3000);

                        await context.PostAsync("Processed your entry!");
                    }
                    catch (FormCanceledException<Entry> e)
                    {
                        var reply = e.InnerException == null ?
                            $"You quit on {e.Last}--maybe you can finish next time!" :
                            "Sorry, I've had a short circuit.  Please try again.";
                        await context.PostAsync(reply);
                    }
                });
        }

        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity == null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);

            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            switch (activity.GetActivityType())
            {
                case ActivityTypes.Message:
                    await Conversation.SendAsync(activity, MakeRootDialog);
                    break;
                case ActivityTypes.Event:
                    string json = activity.Value.ToString();
                    BotMessage botMessage = JsonConvert.DeserializeObject<BotMessage>(json);

                    IEnumerable<EntryTableEntity> entries = TableHelper.GetEntries();

                    foreach (EntryTableEntity entry in entries)
                    {
                        IMessageActivity newMessage = Activity.CreateMessageActivity();
                        newMessage.Type = ActivityTypes.Message;
                        newMessage.From = new ChannelAccount(entry.RecipientId);
                        newMessage.Conversation = new ConversationAccount(false, entry.ConversationId);
                        newMessage.Recipient = new ChannelAccount(entry.FromId);
                        newMessage.Text = botMessage.Message;
                        await connector.Conversations.SendToConversationAsync((Activity)newMessage);
                    }

                    break;
                //case ActivityTypes.ConversationUpdate:
                //case ActivityTypes.ContactRelationUpdate:
                //case ActivityTypes.Typing:
                //case ActivityTypes.DeleteUserData:
                default:
                    Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                    break;
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}