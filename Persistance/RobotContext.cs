using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class RobotContext : DbContext
{
    public RobotContext()
    {
    }

    public RobotContext(DbContextOptions<RobotContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Map> Maps { get; set; }

    public virtual DbSet<RobotCommand> RobotCommands { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseNpgsql("Host=localhost;Database=sit331;Username=postgres;Password=password")
                .LogTo(Console.Write)  // log SQL commands
                .EnableSensitiveDataLogging();                                    // show parameter values
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Map>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_map");

            entity.ToTable("maps");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Columns).HasColumnName("columns");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(800)
                .HasColumnName("description");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("modified_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Rows).HasColumnName("rows");
        });

        modelBuilder.Entity<RobotCommand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_robotcommand");

            entity.ToTable("robot_commands");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasMaxLength(800)
                .HasColumnName("description");
            entity.Property(e => e.IsMoveCommand).HasColumnName("is_move_command");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("modified_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Description)
                .HasMaxLength(800)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Modifieddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifieddate");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
        });

        
    }

    
}
