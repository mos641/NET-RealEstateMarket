/*
i. int ClientId
ii. string BrokerageId


5. Refer to the lecture, and add the proper navigational properties to Subscription, Client and Brokerage models
    a. For Navigational properties you define in Client and Brokerage models, name both Subscriptions, and make sure that they navigate to Subscription model
*/

namespace RealEstateMarket.Models
{
    public class Subscription
    {
        public int ClientId { get; set; }

        public string BrokerageId { get; set; }

        public Client Client { get; set; }

        public Brokerage Brokerage { get; set; }
    }
}
