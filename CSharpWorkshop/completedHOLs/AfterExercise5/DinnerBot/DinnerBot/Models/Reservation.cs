using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

namespace DinnerBot.Models
{   [Serializable]
    public class Reservation
    {

        public Reservation(string name)
        {
            this.Name = name;
        }

        public enum SpecialOccasionOptions
        {
            Birthday,
            Anniversary,
            Engagement,
            none
        }

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




    }
}