using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace dot_net.Data
{
    public class AuthRepository : IAuthRepository
    {   
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var resp = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Username.ToLower().Equals(username.ToLower()));

            if(user == null)
            {
                resp.Success = false;
                resp.Message = "User not found.";
            }
            else if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))            {
                resp.Success = false;
                resp.Message = "Wrong password.";
            }
            else
            {
                resp.Data = user.Id.ToString();
            }

            return resp;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var resp = new ServiceResponse<int>();

            if (await UserExists(user.Username))
            {
                resp.Success = false;
                resp.Message = "User already exists.";
                return resp;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            resp.Data = user.Id;
            return resp;

        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(u => u.Username == username.ToLower()))
            {
                return true;
            }

            return  false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using( var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}