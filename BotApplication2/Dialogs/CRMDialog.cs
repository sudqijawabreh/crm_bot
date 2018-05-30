using BotApplication2.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BotApplication2.Dialogs
{
    public class CRMDialog
    {
        public static readonly IDialog<string> dialog = Chain.PostToChain()
            .Select(m => m.Text)
            .Switch(
            //new RegexCase<IDialog<string>>(new Regex("^hi", RegexOptions.IgnoreCase), (context, text) =>
            //{
            //    return Chain.ContinueWith(new GreetingDialog(), AfterGreetingContinuation);
            //}),
            new DefaultCase<string, IDialog<string>>((context, text) =>
             {
                 return Chain.ContinueWith(FormDialog.FromForm(Estimation.BuildForm, FormOptions.PromptInStart), AfterGreetingContinuation);

             })).Unwrap()
            .PostToUser();
            
            
            
        private static async Task<IDialog<string>> AfterGreetingContinuation(IBotContext context, IAwaitable<object> item)
        {
            var token = await item;
            var name = "User";
            context.UserData.TryGetValue<string>("Name", out name);
            return Chain.Return($"Thank You For Using CRM Bot.");
        }
    }
}