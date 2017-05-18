using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DinnerBot.Dialogs
{
    [Serializable]
    public class UserInfoDialog : IDialog<IMessageActivity>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //Greet the user
            await context.PostAsync("Before we begin, we would like to know who we are talking to?");
            //call the respond method below
            await Respond(context);
            //call context.Wait and set the callback method
            context.Wait(MessageReceivedAsync);

        }

        private static async Task Respond(IDialogContext context)
        {
            //Variable to hold user name
            var userName = String.Empty;
            //check to see if we already have username stored
            context.UserData.TryGetValue<string>("Name", out userName);
            //If not, we will ask for it. 
            if (string.IsNullOrEmpty(userName))
            {
                //We ask here but dont capture it here, we do that in the MessageRecieved Async
                await context.PostAsync("What is your name?");
                //We set a value telling us that we need to get the name out of userdata
                context.UserData.SetValue<bool>("GetName", true);
            }
            else
            {
                //If name was already stored we will say hi to the user.
                await context.PostAsync(String.Format("Hi {0}.  How can I help you today?", userName));

            }
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            //variable to hold message coming in
            try
            {
                var message = await argument;
                //variable for userName
                var userName = String.Empty;
                //variable to hold whether or not we need to get name
                var getName = false;
                //see if name exists
                context.UserData.TryGetValue<string>("Name", out userName);
                //if GetName exists we assign it to the getName variable and replace false
                context.UserData.TryGetValue<bool>("GetName", out getName);
                //If we need to get name, we go in here.
                if (getName)
                {
                    //we get the username we stored above. and set getname to false
                    userName = message.Text;
                    context.UserData.SetValue<string>("Name", userName);
                    context.UserData.SetValue<bool>("GetName", true);

                    context.Wait(MessageReceivedAsync);
                }
                //await Respond(context);
                context.Done(message);
            }
            catch (Exception ex)
            {

                string message = ex.Message;
            }


        }



    }
}