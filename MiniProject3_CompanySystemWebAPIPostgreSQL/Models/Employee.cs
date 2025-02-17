﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

[Table("employee")]
public partial class Employee
{
    [Key]
    [Column("empno")]
    public int Empno { get; set; }

    [Column("fname")]
    [StringLength(255)]
    public string Fname { get; set; } = null!;

    [Column("lname")]
    [StringLength(255)]
    public string Lname { get; set; } = null!;

    [Column("address")]
    [StringLength(255)]
    public string Address { get; set; } = null!;

    [Column("dob")]
    public DateOnly Dob { get; set; }

    [Column("sex")]
    [StringLength(255)]
    public string Sex { get; set; } = null!;

    [Column("position")]
    [StringLength(255)]
    public string Position { get; set; } = null!;

    [Column("deptno")]
    public int? Deptno { get; set; }

    [InverseProperty("MgrempnoNavigation")]
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    [ForeignKey("Deptno")]
    [InverseProperty("Employees")]
    public virtual Department? DeptnoNavigation { get; set; }

    [InverseProperty("EmpnoNavigation")]
    public virtual ICollection<Workson> Worksons { get; set; } = new List<Workson>();

    [NotMapped]
    public DateDto? DobObject { get; set; }

    public void ConvertDobObjectToDateOnly()
    {
        if (DobObject != null)
        {
            Dob = new DateOnly(DobObject.Year, DobObject.Month, DobObject.Day);
        }
    }
}
