using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
namespace BotApplication2.Models
{
    public enum CrmTypes
    {
        Cloude,
        OnPremise,
        //[Describe("Don't Know")]
        //DoNotKnow,
    }
    public enum edition
    {

        [Describe("Over 10 : go to enterprise edition")]
        Over10UsersGoToEnterpriseEdition,
        [Describe("Under 10 : go to Business edition")]
        Under10UsersGoToBusinessEdition,
    }

    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".","\"{0}\" is not a {&} option.")]
    public class Estimation
    {

        [Describe("Database size to migrate")]
        [Prompt("What database size do you expect to migrate over?")]
        public int DatabaseSize;
        [Optional]
        [Template(TemplateUsage.NoPreference,"None")]
        public string Specials;

        [Prompt("How many users are you expecting to use the software?{||}")]
        public edition? NumberOfUsersYouAreExpectingToUseTheSoftware;
        
        //[Optional]
        //[Template(TemplateUsage.NoPreference,"None")]
        [Prompt("Are you upgrading to Dynamics 365 from CRM online or onprem?")]
        public CrmTypes? UpgradeFrom;

        //[Prompt("What is the number of users you expect will be using the software")]
        public int numberOfUsers;
        [Prompt("Are you current on your maintenance{||}?")]
        public bool? UnderMaintenance;
        [Prompt("Ok great,how many users will need to use {&}?")]
        public int SalesApp;
        [Prompt("What about {&}?")]
        public int CustomerService;
        [Prompt("And how many users for the {&}?")]
        public int FieldService;
        [Prompt("What about number of users for {&}?")]
        public int Marketing;
        [Prompt("and finally how  many users for {&}?")]
        public int SocialListening;
        [Describe("number of users who will use multiple apps ?")]
        public int multipleApps;
        public static IForm<Estimation> BuildForm()
        {
            //return new FormBuilder<Estimation>().Build();
            return new FormBuilder<Estimation>()
                .Message("Hi I’m a dynamics 365 Customer Engagement licensing bot. " +
                " I can ask you questions to help you estimate your license costs for Dynamics software to help you build and support customer relationships")
                .Field(new FieldReflector<Estimation>(nameof(NumberOfUsersYouAreExpectingToUseTheSoftware))
                .SetNext((value, state) => 
                {
                    state.numberOfUsers = state.NumberOfUsersYouAreExpectingToUseTheSoftware == edition.Under10UsersGoToBusinessEdition ? 10 : 0;
                    return new NextStep { Direction = StepDirection.Next};
                }))
                .Field(nameof(UpgradeFrom))
                .Field(new FieldReflector<Estimation>(nameof(UnderMaintenance))
                    .SetActive((state)=>state.UpgradeFrom==CrmTypes.OnPremise))
                .Field(nameof(DatabaseSize))
                .Field(new FieldReflector<Estimation>(nameof(numberOfUsers)).SetActive((state) => false))
                .Field(new FieldReflector<Estimation>(nameof(SalesApp)).SetActive((state) => state.NumberOfUsersYouAreExpectingToUseTheSoftware == edition.Over10UsersGoToEnterpriseEdition))
                .Field(new FieldReflector<Estimation>(nameof(CustomerService)).SetActive((state) => state.NumberOfUsersYouAreExpectingToUseTheSoftware == edition.Over10UsersGoToEnterpriseEdition))
                .Field(new FieldReflector<Estimation>(nameof(FieldService)).SetActive((state) => state.NumberOfUsersYouAreExpectingToUseTheSoftware == edition.Over10UsersGoToEnterpriseEdition))
                .Field(new FieldReflector<Estimation>(nameof(Marketing)).SetActive((state) => state.NumberOfUsersYouAreExpectingToUseTheSoftware == edition.Over10UsersGoToEnterpriseEdition))
                .Field(new FieldReflector<Estimation>(nameof(SocialListening)).SetActive((state) => state.NumberOfUsersYouAreExpectingToUseTheSoftware == edition.Over10UsersGoToEnterpriseEdition)
                
                .SetNext((value, state) => 
                {

                    state.numberOfUsers = state.SalesApp + state.CustomerService + state.FieldService + state.Marketing + state.SocialListening;
                    return new NextStep {
                    Direction = StepDirection.Next};
                })
                .SetValidate(async (state, value) =>
                {
                    
                    return new ValidateResult { Value = value, IsValid = true };
                }))
                .Field(new FieldReflector<Estimation>(nameof(Specials))
                .SetValidate(async (state, value) => {
                    return new ValidateResult { Value = value, IsValid = true };
                })
                .SetActive((state) => false)
                .SetType(null)
                //.SetActive((state)=>state.NumberOfUsersYouAreExpectingToUseTheSoftware==edition.Over10UsersGoToEnterpriseEdition)
                .SetDefine(async (state, field) =>
                {
                    field
                    .AddDescription("cookie", "Free cookie")
                    .AddTerms("cookie", "cookie")
                    .AddDescription("drink", "free large drink")
                    .AddTerms("drink", "drink", "free drink");
                    return true;
                }))
                .Field(nameof(multipleApps),"Of your {numberOfUsers} number of users, how many will need to use multiple apps ?")
                //.Prompter(async (context, prompt, state, field) => { return prompt; })
                .Confirm(async (state) =>
                {

                    return new PromptAttribute("is this your selection :\n{*}");
                }
                )
                .Build();
        }
    }
}