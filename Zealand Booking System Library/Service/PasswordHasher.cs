using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;

namespace Zealand_Booking_System_Library.Service
{
    /// <summary>
    /// Centralizes password hashing and verification logic.
    /// This prevents security-sensitive code from being scattered across the system
    /// and makes it easier to update or replace the hashing strategy later.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Converts a plain-text password into a secure hash before storage,
        /// so raw passwords are never persisted or passed around.
        /// </summary>
        public static string Hash(string password)
        {
            return Argon2.Hash(password);
        }
        /// <summary>
        /// Verifies a password against an existing hash,
        /// allowing authentication without exposing the original password.
        /// </summary>
        public static bool Verify(string hash, string password)
        {
            return Argon2.Verify(hash, password);
        }
    }
}
