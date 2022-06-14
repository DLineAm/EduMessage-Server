using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SignalIRServerTest.Models;

#nullable disable

namespace SignalIRServerTest.Models.Context
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
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseAttachment> CourseAttachments { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<EducationForm> EducationForms { get; set; }
        public virtual DbSet<EducationType> EducationTypes { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageAttachment> MessageAttachments { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserConversation> UserConversations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(Program.SchoolConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("Attachment");

                entity.HasIndex(e => e.IdType, "IX_Attachment_IdType");

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

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.ToTable("Conversation");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.HasIndex(e => e.IdMainCourse, "IX_Course_IdMainCourse");
                entity.HasIndex(e => e.IdTeacher, "IX_Course_User");
                entity.HasIndex(e => e.IdTeacher, "IX_Course_CourseTask");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.IdMainCourseNavigation)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.IdMainCourse)
                    .HasConstraintName("FK_Course_MainCourse");

                entity.HasOne(d => d.IdTeacherNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.IdTeacher)
                    .HasConstraintName("FK_Course_User");

                entity.HasOne(d => d.IdCourseTaskNavigation)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.IdTask)
                    .HasConstraintName("FK_Course_CourseTask");
            });

            modelBuilder.Entity<CourseTask>(entity =>
            {
                entity.ToTable("CourseTask");
                entity.HasKey(d => d.Id);
            });

            modelBuilder.Entity<MainCourse>(entity =>
            {
                entity.ToTable("MainCourse");
                entity.HasIndex(e => e.IdSpeciality, "IX_MainCourse_IdSpeciality");
                entity.HasOne(d => d.IdSpecialityNavigation)
                    .WithMany(p => p.MainCourse)
                    .HasForeignKey(d => d.IdSpeciality)
                    .HasConstraintName("FK_MainCourse_Speciality");
            });

            modelBuilder.Entity<CourseAttachment>(entity =>
            {
                entity.ToTable("CourseAttachment");

                entity.Property(p => p.Mark).HasColumnType("smallint");

                entity.HasIndex(e => e.IdAttachmanent, "IX_CourseAttachment_IdAttachmanent");

                entity.HasIndex(e => e.IdCourse, "IX_CourseAttachment_IdCourse");

                //entity.HasIndex(e => e.IdCourse, "IX_CourseAttachment_IdStatus");

                entity.HasIndex(e => e.IdCourse, "IX_CourseAttachment_IdUser");

                entity.HasOne(d => d.IdAttachmanentNavigation)
                    .WithMany(p => p.CourseAttachments)
                    .HasForeignKey(d => d.IdAttachmanent)
                    .HasConstraintName("FK_CourseAttachment_Attachment");

                entity.HasOne(d => d.IdCourseNavigation)
                    .WithMany(p => p.CourseAttachments)
                    .HasForeignKey(d => d.IdCourse)
                    .HasConstraintName("FK_CourseAttachment_Course");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.CourseAttachments)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_CourseAttachment_User");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("Device");

                entity.HasKey(e => e.SerialNumber);

                entity.HasIndex(e => e.IdUser, "IX_Device_IdUser");

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

                entity.HasIndex(e => e.IdFaculty, "IX_Group_IdFaculty");

                entity.HasIndex(e => e.IdSpeciality, "IX_Group_IdSpeciality");

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

                entity.HasIndex(e => e.IdAttachments, "IX_Message_IdAttachments");

                entity.HasIndex(e => e.IdRecipient, "IX_Message_IdRecipient");

                entity.HasIndex(e => e.IdUser, "IX_Message_IdUser");

                entity.Property(e => e.MessageContent).IsRequired();

                entity.Property(e => e.SendDate).HasColumnType("datetime");

                entity.HasOne(d => d.IdConversationNavigation)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.IdConversation)
                    .HasConstraintName("FK_Message_Conversation");

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

            modelBuilder.Entity<MessageAttachment>(entity =>
            {
                entity.ToTable("MessageAttachment");

                entity.HasOne(d => d.IdAttachmentNavigation)
                    .WithMany(p => p.MessageAttachments)
                    .HasForeignKey(d => d.IdAttachment)
                    .HasConstraintName("FK_MessageAttachment_Attachment");

                entity.HasOne(d => d.IdMessageNavigation)
                    .WithMany(p => p.MessageAttachments)
                    .HasForeignKey(d => d.IdMessage)
                    .HasConstraintName("FK_MessageAttachment_Message");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.ToTable("School");

                entity.HasIndex(e => e.IdCity, "IX_School_IdCity");

                entity.HasIndex(e => e.IdEducationType, "IX_School_IdEducationType");

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

                entity.HasIndex(e => e.IdCity, "IX_User_IdCity");

                entity.HasIndex(e => e.IdEducationForm, "IX_User_IdEducationForm");

                entity.HasIndex(e => e.IdGroup, "IX_User_IdGroup");

                entity.HasIndex(e => e.IdRole, "IX_User_IdRole");

                entity.HasIndex(e => e.IdSchool, "IX_User_IdSchool");

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

                //entity.Property(e => e.Password)
                //    .IsRequired()
                //    .HasMaxLength(16);

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

            modelBuilder.Entity<UserConversation>(entity =>
            {
                entity.ToTable("UserConversation");

                entity.HasOne(d => d.IdConversationNavigation)
                    .WithMany(p => p.UserConversations)
                    .HasForeignKey(d => d.IdConversation)
                    .HasConstraintName("FK_UserConversation_Conversation");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UserConversations)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_UserConversation_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
