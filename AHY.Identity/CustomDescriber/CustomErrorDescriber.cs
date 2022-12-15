﻿using Microsoft.AspNetCore.Identity;

namespace AHY.Identity.CustomDescriber
{
    public class CustomErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new()
            {
                Code = "PasswordTooShort",
                Description = $"Parola en az {length} karakter olmalıdır."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "Parola en az bir alfanümberik karakter içermelidir."
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new()
            {
                Code = "DuplicatedUserName",
                Description = $"{userName} zaten alınmış."
            };
        }
    }
}
