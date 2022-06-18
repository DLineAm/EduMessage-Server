using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using SignalIRServerTest.Models;
using SignalIRServerTest.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class EducationController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        private readonly ILogger<EducationController> _logger;
        //public EducationContext db = new EducationContext();

        public EducationController(UnitOfWork unitOfWork, ILogger<EducationController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("Courses.IdMainCourse={id:int}&WithoutUsers={withoutUsers:bool}")]
        public List<CourseAttachment> GetCourseBySpeciality([FromRoute] int id,[FromRoute] bool withoutUsers)
        {
            var courses = _unitOfWork.CourseAttachmentRepository.Get(
                includeProperties: $"{nameof(CourseAttachment.IdCourseNavigation)},{nameof(CourseAttachment.IdAttachmanentNavigation)}," +
                                   $"{nameof(CourseAttachment.IdCourseNavigation)}.{nameof(Course.IdTeacherNavigation)}," +
                                   $"{nameof(CourseAttachment.IdCourseNavigation)}.{nameof(Course.IdCourseTaskNavigation)}," +
                                   $"{nameof(CourseAttachment.IdCourseNavigation)}.{nameof(Course.IdTestFrameNavigation)}.{nameof(TestFrame.TestPages)}.{nameof(TestPage.TestVariants)}," +
                                   $"{nameof(CourseAttachment.IdUserNavigation)}",
                filter: c => c.IdCourseNavigation.IdMainCourse == id && (withoutUsers ? c.IdUser == null : c.IdUser != null));


            return courses.ToList();
        }

        [Authorize]
        [HttpPost("Tasks/Add")]
        public bool AddTask([FromBody] List<CourseAttachment> attachments)
        {
            try
            {
                DeleteUnusedCourseAttachment(attachments, true);

                foreach (var item in attachments.Where(a => a.Id == 0))
                {
                    _unitOfWork.CourseAttachmentRepository.Insert(item);
                }

                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(AddTask)));
                return false;
            }
        }

        [Authorize]
        [HttpGet("Courses/Tasks.IdUser={id:int}&IdCourse={idCourse:int}")]
        public List<CourseAttachment> GetCourseAttachments([FromRoute] int id, [FromRoute] int idCourse)
        {
            var courses = _unitOfWork.CourseAttachmentRepository.Get(
                includeProperties: $"{nameof(CourseAttachment.IdCourseNavigation)},{nameof(CourseAttachment.IdAttachmanentNavigation)}," +
                                   $"{nameof(CourseAttachment.IdCourseNavigation)}.{nameof(Course.IdTeacherNavigation)}," +
                                   $"{nameof(CourseAttachment.IdCourseNavigation)}.{nameof(Course.IdCourseTaskNavigation)}",
                filter: c => c.IdCourse == idCourse && c.IdUser == id);

            return courses.ToList();
        }

        [Authorize]
        [HttpGet("Courses/Tasks.IdCourse={idCourse:int}")]
        public List<CourseAttachment> GetCourseAttachmentsByCourseId([FromRoute] int idCourse)
        {
            var courses = _unitOfWork.CourseAttachmentRepository.Get(
                includeProperties: $"{nameof(CourseAttachment.IdCourseNavigation)},{nameof(CourseAttachment.IdAttachmanentNavigation)}," +
                                   $"{nameof(CourseAttachment.IdCourseNavigation)}.{nameof(Course.IdTeacherNavigation)}," +
                                   $"{nameof(CourseAttachment.IdCourseNavigation)}.{nameof(Course.IdCourseTaskNavigation)}," +
                                   $"{nameof(CourseAttachment.IdUserNavigation)}",
                filter: c => c.IdCourse == idCourse && c.IdUser != null);

            return courses.ToList();
        }

        [Authorize]
        [HttpPut("Courses/Tasks.IdCourse={idCourse:int}&IdUser={idUser:int}&Mark={mark}")]
        public async Task<bool> ChangeTaskMark(int idCourse, int idUser, int mark)
        {
            if (!byte.TryParse(mark.ToString(), out var markByte))
            {
                return false;
            }
            try
            {
                var courses = _unitOfWork.CourseAttachmentRepository
                    .Get(filter: c => c.IdCourse == idCourse && c.IdUser == idUser);

                foreach (var course in courses)
                {
                    course.SendTime = DateTime.Now;
                    //course.Comment = comment;
                    course.Mark = markByte;
                }

                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [Authorize]
        [HttpPut("Courses/Tasks.IdCourse={idCourse:int}&IdUser={idUser:int}")]
        public async Task<bool> ChangeTaskComment(int idCourse, int idUser, [FromBody] string comment)
        {
            try
            {
                var courses = _unitOfWork.CourseAttachmentRepository
                    .Get(filter: c => c.IdCourse == idCourse && c.IdUser == idUser);

                foreach (var course in courses)
                {
                    course.Comment = comment;
                    //course.SendTime = DateTime.Now;
                    //course.Comment = comment;
                    //course.Mark = markByte;
                }

                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [Authorize]
        [HttpGet("Courses.IdMainCourse={id:int}&IncludeProperties=false")]
        public List<Course> GetCourseBySpecialityWithoutIncludings([FromRoute] int id)
        {
            var courses = _unitOfWork.CourseRepository
                .Get(filter: c => c.IdMainCourse == id);

            return courses.ToList();
        }

        [Authorize]
        [HttpGet("Courses.IdSpeciality={id:int}")]
        public List<MainCourse> GetMainCoursesById([FromRoute] int id)
        {
            var courses = _unitOfWork.MainCourseRepository.Get(
                filter: c => c.IdSpeciality == id);

            return courses.ToList();
        }

        [Authorize]
        [HttpPut("Courses.ChangeSpecialityId")]
        public async Task<bool> ChangeSpecialityId([FromBody] List<KeyValuePair<int, int>> Ids)
        {
            try
            {
                foreach (var (mainCourseId, specialityId) in Ids)
                {
                    var mainCourse = _unitOfWork.MainCourseRepository
                        .GetById(mainCourseId);

                    if (mainCourse == null)
                    {
                        continue;
                    }

                    mainCourse.IdSpeciality = specialityId;
                }

                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(ChangeMainCourseId)));
                return false;
            }
        }

        [Authorize]
        [HttpPut("Courses.ChangeMainCourseId")]
        public async Task<bool> ChangeMainCourseId([FromBody] List<KeyValuePair<int, int>> Ids)
        {
            try
            {
                foreach (var (courseId, mainCourseId) in Ids)
                {
                    var course = _unitOfWork.CourseRepository
                        .GetById(courseId);

                    if (course == null)
                    {
                        return false;
                    }

                    course.IdMainCourse = mainCourseId;
                }

                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(ChangeMainCourseId)));
                return false;
            }
        }

        [Authorize]
        [HttpPost("Courses.FromList")]
        public async Task<KeyValuePair<int, List<int>>> AddCourseAsync([FromBody] List<CourseAttachment> courses)
        {
            var resultList = new List<CourseAttachment>();
            try
            {
                Course addedCourse = null;
                var courseId = -1;
                var position =  _unitOfWork.CourseRepository.Get().Max(c => c.Position);
                foreach (var course in courses)
                {
                    if (addedCourse != null)
                    {
                        course.IdCourseNavigation = addedCourse;
                    }

                    var pages = course.IdCourseNavigation.IdTestFrameNavigation?.TestPages;

                    if (pages != null)
                    {
                        foreach (var testPage in pages)
                        {
                            testPage.IdTestTypeNavigation = null;
                        }
                    }

                    course.IdCourseNavigation.IdMainCourseNavigation = null;
                    course.IdCourseNavigation.Position = position + 1;

                    var entry = _unitOfWork.CourseAttachmentRepository.Insert(course);
                    _unitOfWork.Save();

                    addedCourse = entry.Entity.IdCourseNavigation;
                    courseId = entry.Entity.IdCourseNavigation.Id;
                    resultList.Add(entry.Entity);
                }


                return new KeyValuePair<int, List<int>>(courseId, resultList.Select(c => c.Id).ToList());
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(AddCourseAsync)));
                return new KeyValuePair<int, List<int>>(-1, null);
            }
        }      

        [Authorize]
        [HttpDelete("Courses.id={id}")]
        public bool DeleteCourse([FromRoute] int id)
        {
            try
            {
                var foundCourse = _unitOfWork.CourseRepository.GetById(id);
                var task = foundCourse.IdCourseTaskNavigation;
                var foundCourseAttachments = _unitOfWork
                    .CourseAttachmentRepository.Get(c => c.IdCourse == id);

                foundCourseAttachments.ToList().ForEach(a => 
                    _unitOfWork.CourseAttachmentRepository.Delete(a));

                if (foundCourse.IdTestFrame is int idTestFrame)
                {
                    var testFrame = _unitOfWork.TestFrameRepository
                        .GetById(idTestFrame);

                    _unitOfWork.TestFrameRepository
                        .Delete(testFrame);

                    _unitOfWork.Save();
                }

                _unitOfWork.CourseRepository.Delete(foundCourse);
                if (task != null)
                {
                    _unitOfWork.CourseTaskRepository.Delete(task);
                }
                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(DeleteCourse)));
                return false;
            }
        }

        /// <summary>
        /// Изменяет позицию курса
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="isAscending"></param>
        /// <returns>KeyValuePair() - пары значений. Первый int - позиция измененного требуемого курса, второй - id близжайшего курса с измененной позицией, третий - измененная позиция близжайшего курса</returns>
        [Authorize]
        [HttpPut("Courses/ChangePosition.CourseId={courseId:int}&IsAscending={isAscending:bool}")]
        public async Task<KeyValuePair<int, KeyValuePair<int, int>>> ChangeCoursePosition([FromRoute] int courseId, [FromRoute] bool isAscending)
        {
            try
            {
                var course = _unitOfWork.CourseRepository
                    .GetById(courseId);

                if (course?.Position == null)
                {
                    return new KeyValuePair<int, KeyValuePair<int, int>>();
                }

                var courseWithNeededPosition = GetNearestCourseWithPosition(
                    (int)course.IdMainCourse, (int)course.Position, isAscending);

                if (courseWithNeededPosition == null)
                {
                    return new KeyValuePair<int, KeyValuePair<int, int>>();
                }

                (courseWithNeededPosition.Position, course.Position) = 
                    (course.Position, courseWithNeededPosition.Position);

                _unitOfWork.Save();

                var result = new KeyValuePair<int, KeyValuePair<int, int>>((int)course.Position,
                    new KeyValuePair<int, int>(
                        courseWithNeededPosition.Id, (int)courseWithNeededPosition.Position));

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(ChangeCoursePosition)));
                return new KeyValuePair<int, KeyValuePair<int, int>>();
            }
        }

        private Course GetNearestCourseWithPosition(int mainCourseId, int position, bool isAscending)
        {
            Course courseWithNeededPosition;

            if (isAscending)
            {
                courseWithNeededPosition = _unitOfWork.CourseRepository
                    .GetByExpression(c => c.Position > position && c.IdMainCourse == mainCourseId);
            }
            else
            {
                var courses = _unitOfWork.CourseRepository
                    .Get(filter: c => c.IdMainCourse == mainCourseId && c.Position != null)
                    .OrderBy(c => c.Position);
                courseWithNeededPosition = courses
                    .LastOrDefault(c => c.Position < position);
            }

            return courseWithNeededPosition;
        }

        private void DeleteUnusedCourseAttachment(List<CourseAttachment> courseAttachments, bool searchWithUser)
        {
            var firstCourseId = courseAttachments.FirstOrDefault()
                .IdCourse;
            var foundDbCourse = _unitOfWork.CourseRepository
                .Get(includeProperties: $"{nameof(Course.CourseAttachments)}",
                    filter: c => c.Id == firstCourseId)
                .FirstOrDefault();
            var onChangeList = courseAttachments.Where(c => c.Id != 0).ToList();

            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return;
            }

            var claims = identity.Claims;

            var id = claims.First().Value;

            var onDeleteList = foundDbCourse.CourseAttachments
                .Where(c => onChangeList.All(l => c.Id != l.Id) && (searchWithUser ? c.IdUser.ToString() == id : c.IdUser == null));

            onDeleteList.ToList()
                .ForEach(i =>
                {
                    _unitOfWork.CourseAttachmentRepository
                        .Delete(i);
                    _unitOfWork.Save();
                });
        }

        [Authorize]
        [HttpPut("Courses")]
        public async Task<bool> ChangeCourse([FromBody] List<CourseAttachment> courses)
        {
            try
            {
                var firstCourse = courses.FirstOrDefault()?.IdCourseNavigation;
                var firstCourseId = firstCourse?.Id ?? courses.FirstOrDefault().IdCourse;

                var foundDbCourse = _unitOfWork.CourseRepository.Get(
                    includeProperties:$"{nameof(Course.CourseAttachments)}," +
                                      $"{nameof(Course.IdCourseTaskNavigation)}")
                    .FirstOrDefault(c => c.Id == firstCourseId);

                bool isAllCoursesWithoutAttachments = courses
                    .All(c => c.IdAttachmanentNavigation == null);

                if (foundDbCourse == null)
                {
                    return false;
                }

                DeleteUnusedCourseAttachment(courses, firstCourse == null);

                if (firstCourse == null)
                {
                    AddOrUpdateCourseAttachments(courses, foundDbCourse, isAllCoursesWithoutAttachments);
                    _unitOfWork.Save();
                    return true;
                }

                foundDbCourse.Title = firstCourse?.Title;
                foundDbCourse.Description = firstCourse?.Description;

                if (firstCourse.IdCourseTaskNavigation == null)
                {
                    if (firstCourse.IdTask == 0 && firstCourse.IdCourseTaskNavigation != null)
                    {
                        _unitOfWork.CourseTaskRepository
                            .Delete(foundDbCourse.IdCourseTaskNavigation);
                    }
                    else
                    {
                        foundDbCourse.IdTask = firstCourse.IdTask;
                    }
                }
                else
                {
                    var task = _unitOfWork.CourseTaskRepository
                        .GetById(firstCourse.IdCourseTaskNavigation.Id);
                    if (task != null)
                    {
                        task.Description = firstCourse.IdCourseTaskNavigation.Description;
                        task.EndTime = firstCourse.IdCourseTaskNavigation.EndTime;
                        //return true;
                    }
                    else
                    {
                        foundDbCourse.IdCourseTaskNavigation = firstCourse.IdCourseTaskNavigation;
                        //foundDbCourse.IdTask = firstCourse.IdTask;
                    }

                    
                    //foundDbCourse.IdCourseTaskNavigation = firstCourse.IdCourseTaskNavigation;
                }

                _unitOfWork.CourseRepository.Update(foundDbCourse);

                AddOrUpdateCourseAttachments(courses, foundDbCourse, isAllCoursesWithoutAttachments);

                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(ChangeCourse)));
                return false;
            }
        }

        private void AddOrUpdateCourseAttachments(List<CourseAttachment> courses, Course foundDbCourse, bool isAllCoursesWithoutAttachments)
        {
            foreach (var courseAttachment in courses)
            {
                if (courseAttachment.Id == 0)
                {
                    if (courseAttachment.IdAttachmanent != 0 &&
                        courseAttachment.IdAttachmanent != null)
                    {
                        courseAttachment.IdAttachmanentNavigation = null;
                    }

                    courseAttachment.IdCourseNavigation = foundDbCourse;
                    _unitOfWork.CourseAttachmentRepository.Insert(courseAttachment);
                }
                else
                {
                    var dbCourseAttachment =
                        foundDbCourse.CourseAttachments
                            .FirstOrDefault(c => c.Id == courseAttachment.Id);

                    if (isAllCoursesWithoutAttachments)
                    {
                        dbCourseAttachment.IdAttachmanent = null;
                    }

                    //foundDbCourse.Title = courseAttachment.IdCourseNavigation.Title;
                    //foundDbCourse.Description = courseAttachment.IdCourseNavigation.Description;
                    dbCourseAttachment.IdAttachmanentNavigation = courseAttachment.IdAttachmanentNavigation;
                    dbCourseAttachment.IdAttachmanent = courseAttachment.IdAttachmanent;
                    _unitOfWork.CourseAttachmentRepository.Update(dbCourseAttachment);
                }
            }
        }

        [Authorize]
        [HttpPost("Courses.AddSpeciality")]
        public async Task<int> AddSpeciality([FromBody] Speciality item)
        {
            if (item == null)
            {
                return -1;
            }

            try
            {
                var entity = _unitOfWork.SpecialityRepository
                    .Insert(item).Entity;

                _unitOfWork.Save();

                _unitOfWork.Save();
                return entity.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(AddSpeciality)));
                return -1;
            }
        }

        [Authorize]
        [HttpPost("Courses.AddMainCourse")]
        public async Task<int> AddMainCourse([FromBody] MainCourse item)
        {
            if (item == null)
            {
                return -1;
            }

            try
            {
                var entity = _unitOfWork.MainCourseRepository
                    .Insert(item).Entity;

                _unitOfWork.Save();
                return entity.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(AddMainCourse)));
                return -1;
            }
        }

        [Authorize]
        [HttpPut("Courses.ChangeSpeciality")]
        public async Task<bool> ChangeSpeciality([FromBody] Speciality speciality)
        {
            if (speciality == null)
            {
                return false;
            }

            try
            {
                var dbSpeciality = _unitOfWork.SpecialityRepository
                    .GetById(speciality.Id);
                if (dbSpeciality == null)
                {
                    return false; 
                }

                dbSpeciality.Code = speciality.Code;
                dbSpeciality.Title = speciality.Title;

                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(ChangeSpeciality)));
                return false;
            }
        }

        [Authorize]
        [HttpPut("Courses.ChangeMainCourse")]
        public async Task<bool> ChangeMainCourse([FromBody] MainCourse course)
        {
            if (course == null)
            {
                return false;
            }

            try
            {
                var dbCourse = _unitOfWork.MainCourseRepository
                    .GetById(course.Id);
                if (dbCourse == null)
                {
                    return false; 
                }

                dbCourse.Title = course.Title;

                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(ChangeMainCourse)));
                return false;
            }
        }

        [Authorize]
        [HttpDelete("Courses.IdMainCourse={id:int}")]
        public async Task<bool> DeleteMainCourse([FromRoute] int id)
        {
            if (id == 0)
            {
                return false;
            }

            try
            {
                var mainCourse = _unitOfWork.MainCourseRepository
                    .GetById(id);
                if (mainCourse == null)
                {
                    return false;
                }

                DeleteMainCourse(mainCourse);

                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(DeleteSpeciality)));
                return false;
            }
        }

        [Authorize]
        [HttpGet("Tests/Types")]
        public async Task<IEnumerable<TestType>> GetTestTypes()
        {
            return _unitOfWork.TestTypeRepository
                .Get();
        }

        [Authorize]
        [HttpPut("Tests")]
        public async Task<bool> ChangeTest([FromBody] TestFrame testFrame)
        {
            try
            {
                var dbTestFrame = _unitOfWork.TestFrameRepository
                    .Get(filter: t=> t.Id == testFrame.Id,
                        includeProperties: $"{nameof(TestFrame.TestPages)}")
                    .FirstOrDefault();

                if (dbTestFrame == null)
                {
                    return false;
                }

                dbTestFrame.EndDate = testFrame.EndDate;

                foreach (var testPage in dbTestFrame.TestPages)
                {
                    _unitOfWork.TestPageRepository
                        .Delete(testPage);
                }

                _unitOfWork.Save();

                foreach (var testPage in testFrame.TestPages)
                {
                    testPage.Id = 0;
                    testPage.IdTestTypeNavigation = null;
                    foreach (var testVariant in testPage.TestVariants)
                    {
                        testVariant.Id = 0;
                        testVariant.IdTestPage = null;
                    }
                    _unitOfWork.TestPageRepository
                        .Insert(testPage);
                }
                _unitOfWork.Save();


                dbTestFrame.TestPages = testFrame.TestPages;
                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(ChangeTest)));
                return false;
            }
        }

        [Authorize]
        [HttpDelete("Tests.Id={id:int}")]
        public async Task<bool> DeleteTest([FromRoute] int id)
        {
            try
            {
                var testFrame = _unitOfWork.TestFrameRepository
                    .GetById(id);

                if (testFrame == null)
                {
                    return false;
                }

                _unitOfWork.TestFrameRepository
                    .Delete(testFrame);

                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(ChangeTest)));
                return false;
            }
        }

        [Authorize]
        [HttpDelete("Courses.SpecialityId={id:int}")]
        public async Task<bool> DeleteSpeciality([FromRoute] int id)
        {
            if (id == 0)
            {
                return false;
            }

            try
            {
                var speciality = _unitOfWork.SpecialityRepository
                    .GetById(id);
                if (speciality == null)
                {
                    return false;
                }

                var mainCourses = _unitOfWork.MainCourseRepository
                    .Get(filter: c => c.IdSpeciality == id);

                foreach (var mainCourse in mainCourses)
                {
                    DeleteMainCourse(mainCourse);
                }

                _unitOfWork.SpecialityRepository
                    .Delete(speciality.Id);

                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(
                    e.GetType(), nameof(DeleteSpeciality)));
                return false;
            }
        }

        private bool DeleteMainCourse(MainCourse mainCourse)
        {
            var courses = _unitOfWork.CourseAttachmentRepository
                .Get(includeProperties: $"{nameof(CourseAttachment.IdCourseNavigation)}",
                    filter: c => c.IdCourseNavigation.IdMainCourse == mainCourse.Id);

            foreach (var courseAttachment in courses)
            {
                _unitOfWork.CourseAttachmentRepository
                    .Delete(courseAttachment.Id);
            }

            _unitOfWork.MainCourseRepository.Delete(mainCourse.Id);

            return true;
        }
    }
}
