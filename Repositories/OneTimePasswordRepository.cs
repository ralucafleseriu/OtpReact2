using Newtonsoft.Json;
using OtpReact.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OtpReact.Repositories
{
    public class OneTimePasswordRepository
    {
        private static string FilePath { get; set; }
        public OneTimePasswordRepository(string filePath)
        {
            FilePath = filePath;
        }

        public async Task<IList<OneTimePassword>> ReadActiveOneTimePasswords()
        {
            if (File.Exists(FilePath) == false)
            {
                File.Open(FilePath, FileMode.CreateNew);
                return new List<OneTimePassword>();
            }
            using (var stream = new StreamReader(File.OpenRead(FilePath)))
            {
                var json = await stream.ReadToEndAsync();
                List<OneTimePassword> items = JsonConvert.DeserializeObject<List<OneTimePassword>>(json);
                return items;
            }
        }

        public async Task WriteActiveOneTimePasswords(IList<OneTimePassword> activePasswords)
        {
            if (File.Exists(FilePath) == false)
            {
                File.Open(FilePath, FileMode.CreateNew);
            }
            using (var stream = new StreamWriter(File.OpenWrite(FilePath)))
            {
                var json = JsonConvert.SerializeObject(activePasswords);
                await stream.WriteAsync(json);
            }
        }
    }
}
