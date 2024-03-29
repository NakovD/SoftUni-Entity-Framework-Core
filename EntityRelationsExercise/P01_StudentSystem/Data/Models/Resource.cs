﻿namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using P01_StudentSystem.Enums;

public class Resource
{
    [Key]
    public int ResourceId { get; set; }

    [Unicode(true)]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Unicode(false)]
    public string Url { get; set; } = null!;

    public ResourceType ResourceType { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }

    public virtual Course? Course { get; set; }
}
