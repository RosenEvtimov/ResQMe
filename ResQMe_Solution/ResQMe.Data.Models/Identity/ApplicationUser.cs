namespace ResQMe.Data.Models.Identity
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;
    using static ResQMe.GCommon.EntityValidation;

    public class ApplicationUser : IdentityUser
    {
        [StringLength(MaxUserFirstNameLength)]
        public string? FirstName { get; set; }

        [StringLength(MaxUserLastNameLength)]
        public string? LastName { get; set; }

        [StringLength(MaxUserAddressLength)]
        public string? Address { get; set; }

        [StringLength(MaxUserCityLength)]
        public string? City { get; set; }

        /* Reference to all adoption requests for the current user */
        public virtual ICollection<AdoptionRequest> AdoptionRequests { get; set; } = new List<AdoptionRequest>();
    }
}