using System;

namespace SignalIRServerTest.Models
{
    public class UnitOfWork : IDisposable
    {
        private EducationContext _context = new EducationContext();
        private EducationRepository<CourseAttachment> _courseAttachmentRepository;
        private EducationRepository<User> _userRepository;
        private EducationRepository<Course> _courseRepository;
        private EducationRepository<City> _cityRepository;
        private EducationRepository<School> _schoolRepository;
        private EducationRepository<Speciality> _specialityRepository;
        private EducationRepository<Group> _groupRepository;
        private EducationRepository<Role> _roleRepository;
        private EducationRepository<EducationForm> _educationFormRepository;


        public EducationRepository<CourseAttachment> CourseAttachmentRepository => _courseAttachmentRepository 
            ??= new EducationRepository<CourseAttachment>(_context);

        public EducationRepository<User> UserRepository => _userRepository 
            ??= new EducationRepository<User>(_context);

        public EducationRepository<Course> CourseRepository => _courseRepository 
            ??= new EducationRepository<Course>(_context);

        public EducationRepository<City> CityRepository => _cityRepository
            ??= new EducationRepository<City>(_context);

        public EducationRepository<School> SchoolRepository => _schoolRepository
            ??= new EducationRepository<School>(_context);

        public EducationRepository<Speciality> SpecialityRepository => _specialityRepository
            ??= new EducationRepository<Speciality>(_context);

        public EducationRepository<Group> GroupRepository => _groupRepository
            ??= new EducationRepository<Group>(_context);

        public EducationRepository<Role> RoleRepository => _roleRepository
            ??= new EducationRepository<Role>(_context);

        public EducationRepository<EducationForm> EducationFormRepository => _educationFormRepository
            ??= new EducationRepository<EducationForm>(_context);

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
            }

            this.disposed = true;
        }

    }
}