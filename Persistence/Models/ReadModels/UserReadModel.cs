﻿using System;

namespace Persistence.Models.ReadModels
{
    public class UserReadModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
    }
}