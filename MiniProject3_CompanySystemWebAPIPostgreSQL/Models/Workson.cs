using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

[PrimaryKey("Empno", "Projno")]
[Table("workson")]
public partial class Workson
{
    [Key]
    [Column("empno")]
    public int Empno { get; set; }

    [Key]
    [Column("projno")]
    public int Projno { get; set; }

    [Column("dateworked")]
    public DateOnly? Dateworked { get; set; }

    [Column("hoursworked")]
    public int? Hoursworked { get; set; }

    [NotMapped]
    [ForeignKey("Empno")]
    [InverseProperty("Worksons")]
    public virtual Employee? EmpnoNavigation { get; set; }

    [NotMapped]
    [ForeignKey("Projno")]
    [InverseProperty("Worksons")]
    public virtual Project? ProjnoNavigation { get; set; }

    [NotMapped]
    public DateDto? DateWorkedObject { get; set; }

    public void ConvertDateWorkedObjectToDateOnly()
    {
        if (DateWorkedObject != null)
        {
            Dateworked = new DateOnly(DateWorkedObject.Year, DateWorkedObject.Month, DateWorkedObject.Day);
        }
    }
}
