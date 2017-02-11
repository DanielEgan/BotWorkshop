using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DinnerBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string ReservartionOption = "Reserve Table";
        private const string HelloOption        = "Say Hello"    ;

         public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome to Dinner Bot"  );
                  context.Wait     (this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Choice(
                context, 
                this.OnOptionSelected, 
                // Present two (2) options to user
                new List<string>() { ReservartionOption, HelloOption }, 
                String.Format("Hi, are you looking for to reserve a table or Just say hello?"), "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                //capture which option then selected
                string optionSelected = await result;
                switch (optionSelected)
                {
                    case ReservartionOption:
                        // Not implemented yet -- that's in the next lesson! 
                        break;
                    case HelloOption:
                        context.Call(new HelloDialog(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                //If too many attempts we send error to user and start all over. 
                await context.PostAsync($"Ooops! Too many attemps :( You can start again!");

                //This sets us in a waiting state, after running the prompt again. 
                context.Wait(this.MessageReceivedAsync);
            }
        }

        /// <summary>
        ///  User did not select a reservation. Loop back to original statement and ask if they would like to make one.
        /// </summary>
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}