using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Models;

public partial class CompanyContext : DbContext
{
    public CompanyContext()
    {
    }

    public CompanyContext(DbContextOptions<CompanyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Workson> Worksons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Deptno).HasName("department_pkey");

            entity.HasOne(d => d.MgrempnoNavigation).WithMany(p => p.Departments)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("department_mgrempno_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Empno).HasName("employee_pkey");

            entity.HasOne(d => d.DeptnoNavigation).WithMany(p => p.Employees).HasConstraintName("fk_dept");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Projno).HasName("project_pkey");

            entity.HasOne(d => d.DeptnoNavigation).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("project_deptno_fkey");
        });

        modelBuilder.Entity<Workson>(entity =>
        {
            entity.HasKey(e => new { e.Empno, e.Projno }).HasName("workson_pkey");

            entity.HasOne(d => d.EmpnoNavigation).WithMany(p => p.Worksons).HasConstraintName("workson_empno_fkey");

            entity.HasOne(d => d.ProjnoNavigation).WithMany(p => p.Worksons).HasConstraintName("workson_projno_fkey");
        });

        //modelBuilder.Entity<Workson>()
        //.Ignore(w => w.EmpnoNavigation)
        //.Ignore(w => w.ProjnoNavigation)
        //.Ignore(w => w.DateWorkedObject);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
