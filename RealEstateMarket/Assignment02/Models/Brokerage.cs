using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateMarket.Models
{
    public class Brokerage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Registration Number")]
        [Required]
        public string Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Fee { get; set; }

        public ICollection<Subscription>? Subscriptions { get; set; }

        public ICollection<Advertisement>? Advertisements { get; set; }

        // overrides get hash code function to compare Brokerages by Id
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        // overrides Equal function to compare Brokerages by Id -
        // for use in client subscriptions and splitting brokerages into ones scubscribed to and ones unsubscribed to with Except()
        public override bool Equals(object? objEval)
        {
            if (objEval is not Brokerage)
            {
                throw new ArgumentException("Object is not a Brokerage");
            }
            
            var brokerageEval = objEval as Brokerage;

            if (brokerageEval == null)
            {
                return false;
            }
            else
            {
                return this.Id.Equals(brokerageEval.Id);
            }
        }
    }
}
