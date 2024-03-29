﻿using BoardGameVoter.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameVoter.Models.EntityModels.Users
{
    [Table("Users")]
    public class User : EntityBase
    {
        public int Age { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }

        [NotMapped]
        public List<UserFriend> Friends { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string LastName { get; set; }
        public int Logins { get; set; }

        [NotMapped]
        public List<UserNotification> Notifications { get; set; }

        [NotMapped]
        public UserPassword Password { get; set; }

        public string UserName { get; set; }
    }
}