﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace WealthFlow.Models
{
    public partial class Category
    {
        public Category()
        {
            Keyword = new HashSet<Keyword>();
            Transaction = new HashSet<Transaction>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Keyword> Keyword { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}