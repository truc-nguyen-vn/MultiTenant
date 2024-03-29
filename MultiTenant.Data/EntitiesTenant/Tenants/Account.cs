﻿using System.ComponentModel.DataAnnotations;

namespace MultiTenant.Data.EntitiesTenant.Tenants
{
    public class Account
    {
        [Display(Name = "ID Account")]
        public int IdAcc { get; set; }

        public string Name { get; set; }

        public string Age { get; set; }

        public string Email { get; set; }
    }
}
