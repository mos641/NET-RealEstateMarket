namespace RealEstateMarket.Models.ViewModels
{
    public class BrokeragesViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Brokerage> Brokerages { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
        public IEnumerable<Advertisement> Advertisements { get; set;}
    }
}
