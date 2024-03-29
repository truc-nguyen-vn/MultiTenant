﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiTenant.Data.EntitiesTenant.MultiTenants
{
    public class Tenant
    {
        [Key]
        [Display(Name = "ID Tenant")]
        public int TenantId { get; set; }

        public string URL { get; set; }

        [Display(Name = "Database Name")]
        public string DbName { get; set; }

        public string Favicon { get; set; }

        public ICollection<AccountTenant> AccountTenants { get; set; }

    }
}
