using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using mosPortal.Models;

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
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Concern> Concern { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAnswerOptionsPoll> UserAnswerOptionsPoll { get; set; }
        public virtual DbSet<UserConcern> UserConcern { get; set; }
        public virtual DbSet<Userrole> Userrole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=v22018127362578408.supersrv.de;port=3306;database=dbbuerger;uid=jonas;password=Jonas#1995");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address", "dbbuerger");

                entity.HasKey(e => e.Id);
               /* entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();*/

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Number)
                    .HasColumnName("number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Street)
                    .HasColumnName("street")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.ZipCode)
                    .HasColumnName("zipCode")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<AnswerOptions>(entity =>
            {
                entity.ToTable("answerOptions", "dbbuerger");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AnswerOptionsPoll>(entity =>
            {
                entity.ToTable("answerOptions_poll", "dbbuerger");

                entity.HasIndex(e => e.AnswerOptionsId)
                    .HasName("fk_answerOptions_has_poll_answerOptions1_idx");

                entity.HasIndex(e => e.PollId)
                    .HasName("fk_answerOptions_has_poll_poll1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

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

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment", "dbbuerger");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_comment_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_comment_User1");
            });

            modelBuilder.Entity<Concern>(entity =>
            {
                entity.ToTable("concern", "dbbuerger");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_concern_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Concern)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_concern_User1");
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.ToTable("poll", "dbbuerger");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_poll_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Approved)
                    .HasColumnName("approved")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.End).HasColumnName("end");

                entity.Property(e => e.NeedsLocalCouncil)
                    .HasColumnName("needsLocalCouncil")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Poll)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_poll_User1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "dbbuerger");

                entity.HasIndex(e => e.AdressId)
                    .HasName("fk_User_adress1_idx");

                entity.HasIndex(e => e.UserroleId1)
                    .HasName("fk_User_userrole1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.AdressId)
                    .HasColumnName("adress_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Birthday)
                    .HasColumnName("birthday")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Birthplace)
                    .HasColumnName("birthplace")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Firstname)
                    .HasColumnName("firstname")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.UserroleId1)
                    .HasColumnName("userrole_ID1")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Adress)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AdressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_adress1");

                entity.HasOne(d => d.UserroleId1Navigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserroleId1)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_userrole1");
            });

            modelBuilder.Entity<UserAnswerOptionsPoll>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.AnswerOptionsPollId });

                entity.ToTable("User_answerOptions_poll", "dbbuerger");

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

                entity.ToTable("User_concern", "dbbuerger");

                entity.HasIndex(e => e.ConcernId)
                    .HasName("fk_User_has_concern_concern1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_User_has_concern_User1_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("User_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ConcernId)
                    .HasColumnName("concern_ID")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Concern)
                    .WithMany(p => p.UserConcern)
                    .HasForeignKey(d => d.ConcernId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_has_concern_concern1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserConcern)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_has_concern_User1");
            });

            modelBuilder.Entity<Userrole>(entity =>
            {
                entity.ToTable("userrole", "dbbuerger");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });
        }
    }
}
