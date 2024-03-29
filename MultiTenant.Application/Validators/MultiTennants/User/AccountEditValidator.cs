﻿using FluentValidation;
using MultiTenant.Application.Models.MultiTenants.Account;

namespace MultiTenant.Application.Validators.MultiTenants.User
{
    public  class AccountEditValidator: AbstractValidator<AccountEdit>
    {
        public AccountEditValidator()
        {
            RuleFor(actor => actor.Name).NotEmpty();
            RuleFor(actor => actor.AccId).NotEmpty();
            RuleFor(actor => actor.Email).NotEmpty();
        }
    }
}
