using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SignalIRServerTest.Models;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class EducationController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        //public EducationContext db = new EducationContext();

        public EducationController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet("Courses.SpecialityId={id}")]
        public List<CourseAttachment> GetCourseBySpeciality([FromRoute] int id)
        {
            var courses = _unitOfWork.CourseAttachmentRepository.Get(
                includeProperties: $"{nameof(CourseAttachment.IdCourseNavigation)},{nameof(CourseAttachment.IdAttachmanentNavigation)}",
                filter: c => c.IdCourseNavigation.IdSpeciality == id);

            return courses.ToList();
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
                foreach (var course in courses)
                {
                    if (addedCourse != null)
                    {
                        course.IdCourseNavigation = addedCourse;
                    }

                    course.IdCourseNavigation.IdSpecialityNavigation = null;

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
                var foundCourseAttachments = _unitOfWork
                    .CourseAttachmentRepository.Get(c => c.IdCourse == id);

                foundCourseAttachments.ToList().ForEach(a => 
                    _unitOfWork.CourseAttachmentRepository.Delete(a));

                _unitOfWork.CourseRepository.Delete(foundCourse);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [Authorize]
        [HttpPut("Courses")]
        public async Task<bool> ChangeCourse([FromBody] List<CourseAttachment> courses)
        {
            try
            {
                var firstCourse = courses.FirstOrDefault()?.IdCourse;

                var foundDbCourse = _unitOfWork.CourseRepository.Get(
                    includeProperties:$"{nameof(Course.CourseAttachments)}")
                    .FirstOrDefault(c => c.Id == firstCourse);

                bool isAllCoursesWithoutAttachments = courses.All(c => c.IdAttachmanentNavigation == null);

                if (foundDbCourse == null)
                {
                    return false;
                }

                var onChangeList = courses.Where(c => c.Id != 0).ToList();

                var onDeleteList = foundDbCourse.CourseAttachments
                    .Where(c => onChangeList.All(l => c.Id != l.Id));

                onDeleteList.ToList().ForEach(i =>
                {
                    _unitOfWork.CourseAttachmentRepository.Delete(i);
                    _unitOfWork.Save();
                });

                foreach (var courseAttachment in courses)
                {
                    if (courseAttachment.Id == 0)
                    {
                        _unitOfWork.CourseAttachmentRepository.Insert(courseAttachment);
                    }
                    else
                    {
                        var dbCourseAttachment =
                            foundDbCourse.CourseAttachments.FirstOrDefault(c => c.Id == courseAttachment.Id);

                        if (isAllCoursesWithoutAttachments)
                        {
                            dbCourseAttachment.IdAttachmanent = null;
                        }

                        foundDbCourse.Title = courseAttachment.IdCourseNavigation.Title;
                        foundDbCourse.Description = courseAttachment.IdCourseNavigation.Description;
                    }
                }

                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
