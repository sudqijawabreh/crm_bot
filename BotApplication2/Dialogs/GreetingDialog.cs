using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotApplication2.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hello this is the CRM bot!");
            Respond(context);

            context.Wait(MessageReceivedAsync);

        }

        public static async Task Respond(IDialogContext context)
        {

            var userName = String.Empty;
            if (string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("What is your name?");
                context.UserData.SetValue<bool>("getName", true);
            }
            else
            {
                await context.PostAsync(String.Format("Hi {0} . How can I help you ? ", userName));
            }
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var userName = String.Empty;
            var getName = false;
            context.UserData.TryGetValue<String>("Name", out userName);
            context.UserData.TryGetValue<bool>("getName", out getName);

            if (getName)
            {
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("getName", false);
            }
            await Respond(context);
            context.Done(message);
        }
    }
}