/*
i. int Id
ii. string LastName
    1. Add the ‘Required’ attribute
    2. Add the ‘StringLength’ attribute, with a value of 50
    3. Set the display name to ‘Last Name’
iii. string FirstName
    1. Add appropriate attributes like LastName’s
    2. Display name should be First Name’
iv. DateTime BirthDate
    1. Set the attributes:
        a. [DataType(DataType.Date)]
        b. [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        c. [Display(Name = "Birth Date")]
v. string FullName
    1. this is a calculated field from: LastName + “, “ + FirstName


5. Refer to the lecture, and add the proper navigational properties to Subscription, Client and Brokerage models
    a. For Navigational properties you define in Client and Brokerage models, name both Subscriptions, and make sure that they navigate to Subscription model
*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateMarket.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        public string FullName
        {
            get
            {
                return (LastName + ", " + FirstName);
            }
        }

        public ICollection<Subscription>? Subscriptions { get; set; }
    }
}
