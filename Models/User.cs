﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace WealthFlow.Models
{
    public partial class User
    {
        public User()
        {
            Card = new HashSet<Card>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Card> Card { get; set; }
    }
}