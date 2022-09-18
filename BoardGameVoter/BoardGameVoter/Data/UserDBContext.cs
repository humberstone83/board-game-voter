﻿using BoardGameVoter.Data.Shared;
using BoardGameVoter.Models.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace BoardGameVoter.Data
{
    public class UserDBContext : DbContextBase<User>
    {
        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options)
        {
        }
    }
}