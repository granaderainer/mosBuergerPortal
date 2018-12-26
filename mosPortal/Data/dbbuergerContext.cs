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
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Concern> Concern { get; set; }
        public virtual DbSet<Poll> Poll { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAnswerOptionsPoll> UserAnswerOptionsPoll { get; set; }
        public virtual DbSet<UserConcern> UserConcern { get; set; }
        public virtual DbSet<UserRole> Userrole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=v22018127362578408.supersrv.de;port=3306;database=dbbuerger;uid=jonas;password=Jonas#1995");
                //optionsBuilder.UseMySQL("server=localhost;port=3306;database=dbbuerger;uid=root;password=geheim1!");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address", "dbbuerger");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

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
                entity.ToTable("AnswerOptions", "dbbuerger");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AnswerOptionsPoll>(entity =>
            {
                entity.ToTable("AnswerOptions_Poll", "dbbuerger");

                entity.HasIndex(e => e.AnswerOptionsId)
                    .HasName("fk_answerOptions_has_poll_answerOptions1_idx");

                entity.HasIndex(e => e.PollId)
                    .HasName("fk_answerOptions_has_poll_poll1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
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

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "dbbuerger");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment", "dbbuerger");

                entity.HasIndex(e => e.ConcernId)
                    .HasName("fk_Comment_Concern1_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_comment_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ConcernId)
                    .HasColumnName("Concern_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

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
                entity.ToTable("Concern", "dbbuerger");

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

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100)
                    .IsUnicode(false);

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
                entity.ToTable("Poll", "dbbuerger");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_poll_User1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
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

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "dbbuerger");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "dbbuerger");

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

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_User_adress1");
            });

            modelBuilder.Entity<UserAnswerOptionsPoll>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.AnswerOptionsPollId });

                entity.ToTable("User_AnswerOptions_Poll", "dbbuerger");

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

                entity.ToTable("User_Concern", "dbbuerger");

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
                entity.ToTable("User_Role", "dbbuerger");

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
