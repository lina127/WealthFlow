﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace WealthFlow.Models
{
    public partial class ExcludeKeyword
    {
        public int ExcludeKeywordId { get; set; }
        public string Name { get; set; }
        public int CardId { get; set; }

        public virtual Card Card { get; set; }
    }
}