﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace WealthFlow
{
    public partial class Card
    {
        public Card()
        {
            Transaction = new HashSet<Transaction>();
        }

        public int CardId { get; set; }
        public int CardNum { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public string Bank { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}