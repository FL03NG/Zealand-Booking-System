using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;

namespace Zealand_Booking_System_Library.Service
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            return Argon2.Hash(password);
        }

        public static bool Verify(string hash, string password)
        {
            return Argon2.Verify(hash, password);
        }
    }
}
