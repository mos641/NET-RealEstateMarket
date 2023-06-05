namespace RealEstateMarket.Models.ViewModels
{
    public class ClientSubscriptionsViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<Brokerage> Subscribed { get; set; }
        public IEnumerable<Brokerage> NonSubscribed { get; set; }
    }
}
