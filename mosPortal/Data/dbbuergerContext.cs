using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using mosPortal.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace mosPortal.Data
{
    public partial class dbbuergerContext : DbContext
    {
        public dbbuergerContext()
        {
        }

        public dbbuergerContext(DbContextOptions<dbbuergerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AnswerOptions> AnswerOptions { get; set; }
        public virtual DbSet<AnswerOptionsPoll> AnswerOptionsPoll { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Concern> Concern { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAnswerOptionsPoll> UserAnswerOptionsPoll { get; set; }
        public virtual DbSet<UserConcern> UserConcern { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseMySql("Server=v22018127362578408.supersrv.de;Database=dbbuerger;User=jonas;Password=Jonas#1995;");
                optionsBuilder.UseMySql("Server=v22018127362578408.supersrv.de;Database=dbbuerger;User=jonas;Password=Jonas#1995;",
                mySqlOptions =>
                {
                    mySqlOptions.ServerVersion(new Version(5, 7, 24), ServerType.MySql);
                }
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Number)
                    .HasColumnName("number")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Street)
                    .HasColumnName("street")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.ZipCode)
                    .HasColumnName("zipCode")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<AnswerOptions>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<AnswerOptionsPoll>(entity =>
            {
                entity.ToTable("AnswerOptions_Poll");

                entity.HasIndex(e => e.AnswerOptionsId)
                    .HasName("fk_answerOptions_has_poll_answerOptions1_idx");

                entity.HasIndex(e => e.PollId)
                    .HasName("fk_answerOptions_has_poll_poll1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AnswerOptionsId)
                    .HasColumnName("answerOptions_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PollId)
                    .HasColumnName("poll_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.AnswerOptions)
                    .WithMany(p => p.AnswerOptionsPoll)
                    .HasForeignKey(d => d.AnswerOptionsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_answerOptions_has_poll_answerOptions1");

                entity.HasOne(d => d.Poll)
                    .WithMany(p => p.AnswerOptionsPoll)
                    .HasForeignKey(d => d.PollId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_answerOptions_has_poll_poll1");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(e => e.ConcernId)
                    .HasName("fk_Comment_Concern1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_comment_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ConcernId)
                    .HasColumnName("Concern_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType("varchar(5000)");

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Concern)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.ConcernId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Comment_Concern1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_comment_User1");
            });

            modelBuilder.Entity<Concern>(entity =>
            {
                entity.HasIndex(e => e.CategoryId)
                    .HasName("fk_Concern_Category1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_concern_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("Category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType("varchar(5000)");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Concern)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Concern_Category1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Concern)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_concern_User1");
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("fk_poll_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Approved)
                    .HasColumnName("approved")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.End)
                    .HasColumnName("end")
                    .HasColumnType("datetime");

                entity.Property(e => e.NeedsLocalCouncil)
                    .HasColumnName("needsLocalCouncil")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType("varchar(5000)");

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Poll)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_poll_User1");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.AddressId)
                    .HasName("fk_User_adress1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AddressId)
                    .HasColumnName("address_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Birthday)
                    .HasColumnName("birthday")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Birthplace)
                    .HasColumnName("birthplace")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Firstname)
                    .HasColumnName("firstname")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(45)");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_adress1");
            });

            modelBuilder.Entity<UserAnswerOptionsPoll>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.AnswerOptionsPollId });

                entity.ToTable("User_AnswerOptions_Poll");

                entity.HasIndex(e => e.AnswerOptionsPollId)
                    .HasName("fk_User_has_answerOptions_poll_answerOptions_poll1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_User_has_answerOptions_poll_User1_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AnswerOptionsPollId)
                    .HasColumnName("answerOptions_poll_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.AnswerOptionsPoll)
                    .WithMany(p => p.UserAnswerOptionsPoll)
                    .HasForeignKey(d => d.AnswerOptionsPollId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_has_answerOptions_poll_answerOptions_poll1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAnswerOptionsPoll)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_has_answerOptions_poll_User1");
            });

            modelBuilder.Entity<UserConcern>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ConcernId });

                entity.ToTable("User_Concern");

                entity.HasIndex(e => e.ConcernId)
                    .HasName("fk_User_Concern_Concern1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_User_has_concern_User1_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ConcernId)
                    .HasColumnName("Concern_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Concern)
                    .WithMany(p => p.UserConcern)
                    .HasForeignKey(d => d.ConcernId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_Concern_Concern1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserConcern)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_has_concern_User1");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("User_Role");

                entity.HasIndex(e => e.RoleId)
                    .HasName("fk_User_has_Role_Role1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_User_has_Role_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RoleId)
                    .HasColumnName("Role_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("User_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_has_Role_Role1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_has_Role_User1");
            });
        }
    }
}
