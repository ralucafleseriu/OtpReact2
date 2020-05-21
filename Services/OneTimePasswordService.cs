using Newtonsoft.Json;
using OtpReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OtpReact.Services
{
    public class OneTimePasswordService
    {

        public OneTimePasswordService()
        {
        }

        public async Task<OneTimePassword> GenerateNewOneTimePassword(Guid userId, DateTime dateTime)
        {
            var activeOneTimePasswords = await ReadActiveOneTimePasswords();
            if (activeOneTimePasswords == null) { activeOneTimePasswords = new List<OneTimePassword>(); }
            //check if there's already an assigned password
            activeOneTimePasswords = activeOneTimePasswords.Where(psw =>
                                        psw.UserId != userId
                                        //[TODO]: replace with a worked: this cleanup violates single responsibility principle
                                        || psw.ExpirationDateTime < DateTime.UtcNow).ToList();

            var passwordString = GeneratPasswordString();
            var thirtySeconds =  new TimeSpan(0, 0, 30);
            var otp = new OneTimePassword(userId, passwordString, DateTime.UtcNow.Add(thirtySeconds));
            activeOneTimePasswords.Add(otp);
            await WriteActiveOneTimePasswords(activeOneTimePasswords);
            return otp;
        }

        internal async Task<bool> ValidateOneTimePassword(Guid userId, string oneTimePassword)
        {

            var activeOneTimePasswords = await ReadActiveOneTimePasswords();
            var now = DateTime.UtcNow;
            var isValid = activeOneTimePasswords.Any(p => p.UserId == userId
                       && p.Password == oneTimePassword
                       && p.ExpirationDateTime >= now);
            return isValid;
        }

        private async Task<IList<OneTimePassword>> ReadActiveOneTimePasswords()
        {
            var filePath = "activePasswords.txt";
            if (File.Exists(filePath) == false)
            {
                File.Open(filePath, FileMode.CreateNew);
                return new List<OneTimePassword>();
            }
            using (var stream = new StreamReader(File.OpenRead(filePath)))
            {
                var json = await stream.ReadToEndAsync();
                List<OneTimePassword> items = JsonConvert.DeserializeObject<List<OneTimePassword>>(json);
                return items;
            }
        }

        private async Task WriteActiveOneTimePasswords(IList<OneTimePassword> activePasswords)
        {
            var filePath = "activePasswords.txt";
            if (File.Exists(filePath) == false)
            {
                File.Open(filePath, FileMode.CreateNew);
            }
            using (var stream = new StreamWriter(File.OpenWrite(@"activePasswords.txt")))
            {
                var json = JsonConvert.SerializeObject(activePasswords);
                await stream.WriteAsync(json);
            }
        }

        private string GeneratPasswordString(int passwordLength = 5)
        {
            StringBuilder otp = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < passwordLength; i++)
            {
                otp.Append(rnd.Next(0, 9).ToString());
            }
            return otp.ToString();
        }
    }
}
