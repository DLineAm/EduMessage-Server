using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SignalIRServerTest
{
    public partial class EducationContext : DbContext
    {
        public EducationContext()
        {
        }

        public EducationContext(DbContextOptions<EducationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseAttachment> CourseAttachments { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<EducationForm> EducationForms { get; set; }
        public virtual DbSet<EducationType> EducationTypes { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(Program.SchoolConnectionString);
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("Attachment");

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdTypeNavigation)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.IdType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attachment_AttachmentType");
            });

            modelBuilder.Entity<AttachmentType>(entity =>
            {
                entity.ToTable("AttachmentType");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("City");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.IdSpecialityNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdSpeciality)
                    .HasConstraintName("FK_Course_Speciality");
            });

            modelBuilder.Entity<CourseAttachment>(entity =>
            {
                entity.ToTable("CourseAttachment");

                entity.HasOne(d => d.IdAttachmanentNavigation)
                    .WithMany(p => p.CourseAttachments)
                    .HasForeignKey(d => d.IdAttachmanent)
                    .HasConstraintName("FK_CourseAttachment_Attachment");

                entity.HasOne(d => d.IdCourseNavigation)
                    .WithMany(p => p.CourseAttachments)
                    .HasForeignKey(d => d.IdCourse)
                    .HasConstraintName("FK_CourseAttachment_Course");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Device_User");
            });

            modelBuilder.Entity<EducationForm>(entity =>
            {
                entity.ToTable("EducationForm");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EducationType>(entity =>
            {
                entity.ToTable("EducationType");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.ToTable("Faculty");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Group");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.IdFacultyNavigation)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.IdFaculty)
                    .HasConstraintName("FK_Group_Faculty");

                entity.HasOne(d => d.IdSpecialityNavigation)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.IdSpeciality)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Group_Speciality");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.MessageContent).IsRequired();

                entity.Property(e => e.SendDate).HasColumnType("datetime");

                entity.HasOne(d => d.IdAttachmentsNavigation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.IdAttachments)
                    .HasConstraintName("FK_Message_Attachment");

                entity.HasOne(d => d.IdRecipientNavigation)
                    .WithMany(p => p.MessageIdRecipientNavigations)
                    .HasForeignKey(d => d.IdRecipient)
                    .HasConstraintName("FK_Message_User1");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.MessageIdUserNavigations)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_User");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.ToTable("School");

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdCityNavigation)
                    .WithMany(p => p.Schools)
                    .HasForeignKey(d => d.IdCity)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_School_City");

                entity.HasOne(d => d.IdEducationTypeNavigation)
                    .WithMany(p => p.Schools)
                    .HasForeignKey(d => d.IdEducationType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_School_EducationType");
            });

            modelBuilder.Entity<Speciality>(entity =>
            {
                entity.ToTable("Speciality");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.MiddleName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(16);

                entity.HasOne(d => d.IdCityNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdCity)
                    .HasConstraintName("FK_User_City");

                entity.HasOne(d => d.IdEducationFormNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdEducationForm)
                    .HasConstraintName("FK_User_EducationForm");

                entity.HasOne(d => d.IdGroupNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdGroup)
                    .HasConstraintName("FK_User_Group");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");

                entity.HasOne(d => d.IdSchoolNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdSchool)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_School");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
