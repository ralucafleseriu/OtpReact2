using OtpReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OtpReact.Repositories;

namespace OtpReact.Services
{
    public class OneTimePasswordService
    {
        private readonly OneTimePasswordRepository _oneTimePasswordRepository;
        public OneTimePasswordService(string filePath = "activePasswords.txt")
        {
            _oneTimePasswordRepository = new OneTimePasswordRepository(filePath);
        }

        public async Task<OneTimePassword> GenerateNewOneTimePassword(Guid userId, DateTime dateTime)
        {
            var activeOneTimePasswords = await _oneTimePasswordRepository.ReadActiveOneTimePasswords();
            if (activeOneTimePasswords == null) { activeOneTimePasswords = new List<OneTimePassword>(); }
            //check if there's already an assigned password
            activeOneTimePasswords = activeOneTimePasswords.Where(psw =>
                                        psw.UserId != userId
                                        //[TODO]: replace with a worker: this cleanup of expired passwords violates single responsibility principle
                                        || psw.ExpirationDateTime < dateTime).ToList();

            var passwordString = GeneratePasswordString();
            var thirtySeconds =  new TimeSpan(0, 0, 30);
            var otp = new OneTimePassword(userId, passwordString, dateTime.Add(thirtySeconds));
            activeOneTimePasswords.Add(otp);
            await _oneTimePasswordRepository.WriteActiveOneTimePasswords(activeOneTimePasswords);
            return otp;
        }

        internal async Task<bool> ValidateOneTimePassword(Guid userId, string oneTimePassword)
        {
            var activeOneTimePasswords = await _oneTimePasswordRepository.ReadActiveOneTimePasswords();
            var now = DateTime.UtcNow;
            var isValid = activeOneTimePasswords.Any(p => p.UserId == userId
                       && p.Password == oneTimePassword
                       && p.ExpirationDateTime >= now);
            return isValid;
        }

        private string GeneratePasswordString(int passwordLength = 5)
        {
            var otp = new StringBuilder();
            var rnd = new Random();
            for (int i = 0; i < passwordLength; i++)
            {
                otp.Append(rnd.Next(0, 9).ToString());
            }
            return otp.ToString();
        }
    }
}
