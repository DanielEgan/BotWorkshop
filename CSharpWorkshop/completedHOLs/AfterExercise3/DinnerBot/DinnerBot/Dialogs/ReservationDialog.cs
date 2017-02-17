using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace DinnerBot.Dialogs
{
    [Serializable]
    public class ReservationDialog
    {

        public enum SpecialOccasionOptions
        {
            Birthday,
            Anniversary,
            Engagement,
            none
        }

        public static IDialogContext context { get; set; }

        [Prompt(new string[] { "What is your name?" })]
        public string Name { get; set; }

        [Prompt(new string[] { "What is your email?" })]
        public string Email { get; set; }

        [Pattern(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$")]
        public string PhoneNumber { get; set; }

        [Prompt("What date would you like to dine with us? example: today, tomorrow, or any date like 04-06-2017 {||}", AllowDefault = BoolDefault.True)]
        [Describe("Reservation date, example: today, tomorrow, or any date like 04-06-2017")]
        public DateTime ReservationDate { get; set; }

        public DateTime ReservationTime { get; set; }

        [Prompt("How many people will be joining us?")]
        [Numeric(1, 20)]
        public int? NumberOfDinners;
        public SpecialOccasionOptions? SpecialOccasion;

        [Numeric(1, 5)]
        [Optional]
        [Describe("for how you enjoyed your experience with Dinner Bot today (optional)")]
        public double? Rating;

        public static IForm<ReservationDialog> BuildForm()
        {
            string userName = String.Empty;
            context.UserData.TryGetValue<string>("Name", out userName);
            return new FormBuilder<ReservationDialog>()
                .Field(new FieldReflector<ReservationDialog>(nameof(ReservationDialog.Name))
                    .SetActive((state) =>
                    {
                        state.Name = userName;
                        return String.IsNullOrEmpty(state.Name);

                    }))
                .Field(nameof(Email), validate: ValidateContactInformation)
                .Field(nameof(PhoneNumber))
                .Field(nameof(ReservationDate))
                .Field(new FieldReflector<ReservationDialog>(nameof(ReservationDialog.ReservationTime))
                    .SetPrompt(PerLinePromptAttribute("What time would you like to arrive?"))
                    ).AddRemainingFields()
                .Build();
        }

        private static Task<ValidateResult> ValidateContactInformation(ReservationDialog state, object response)
        {
            var result = new ValidateResult();
            string contactInfo = string.Empty;
            if (GetEmailAddress((string)response, out contactInfo))
            {
                result.IsValid = true;
                result.Value = contactInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter valid email address.";
            }
            return Task.FromResult(result);
        }

        private static bool GetEmailAddress(string response, out string contactInfo)
        {
            contactInfo = string.Empty;
            var match = Regex.Match(response, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            if (match.Success)
            {
                contactInfo = match.Value;
                return true;
            }
            return false;
        }

        private static PromptAttribute PerLinePromptAttribute(string pattern)
        {
            return new PromptAttribute(pattern)
            {
                ChoiceStyle = ChoiceStyleOptions.PerLine
            };
        }


    }
}