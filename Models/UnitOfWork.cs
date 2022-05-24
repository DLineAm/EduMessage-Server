using System;
using SignalIRServerTest.Models.Context;

namespace SignalIRServerTest.Models
{
    public class UnitOfWork : IDisposable
    {
        private readonly EducationContext _context = new EducationContext();
        private EducationRepository<CourseAttachment> _courseAttachmentRepository;
        private EducationRepository<User> _userRepository;
        private EducationRepository<Course> _courseRepository;
        private EducationRepository<City> _cityRepository;
        private EducationRepository<School> _schoolRepository;
        private EducationRepository<Speciality> _specialityRepository;
        private EducationRepository<Group> _groupRepository;
        private EducationRepository<Role> _roleRepository;
        private EducationRepository<EducationForm> _educationFormRepository;
        private EducationRepository<Conversation> _conversationFormRepository;
        private EducationRepository<UserConversation> _userConversationFormRepository;
        private EducationRepository<Message> _messageRepository;
        private EducationRepository<MessageAttachment> _messageAttachmentRepository;
        private EducationRepository<Attachment> _attachmentRepository;
        private EducationRepository<Device> _deviceRepository;


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

        public EducationRepository<Conversation> ConversationFormRepository => _conversationFormRepository
        ??= new EducationRepository<Conversation>(_context);

        public EducationRepository<UserConversation> UserConversationFormRepository => _userConversationFormRepository
        ??= new EducationRepository<UserConversation>(_context);

        public EducationRepository<Message> MessageRepository => _messageRepository
        ??= new EducationRepository<Message>(_context);

        public EducationRepository<MessageAttachment> MessageAttachmentRepository => _messageAttachmentRepository
        ??= new EducationRepository<MessageAttachment>(_context);

        public EducationRepository<Attachment> AttachmentRepository => _attachmentRepository
        ??= new EducationRepository<Attachment>(_context);

        public EducationRepository<Device> DeviceRepository => _deviceRepository
        ??= new EducationRepository<Device>(_context);

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed;


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
            }

            this._disposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(true);
        }

    }
}