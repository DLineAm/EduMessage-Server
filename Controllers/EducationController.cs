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
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public EducationContext db = new EducationContext();

        [Authorize]
        [HttpGet("Courses.SpecialityId={id}")]
        public List<CourseAttachment> GetCourseBySpeciality([FromRoute] int id)
        {
            var courses = _unitOfWork.CourseAttachmentRepository.Get(
                includeProperties: $"{nameof(CourseAttachment.IdCourseNavigation)},{nameof(CourseAttachment.IdAttachmanentNavigation)}",
                filter: c => c.IdCourseNavigation.IdSpeciality == id);

            return courses.ToList();
        }

        //[HttpGet("Courses.Id={id}")]
        //public List<Course> GeSpeciality([FromRoute] int id)
        //{
        //    var courses = _unitOfWork.CourseAttachmentRepository.Get(
        //        includeProperties: $"{nameof(CourseAttachment.IdCourseNavigation)},{nameof(CourseAttachment.IdAttachmanentNavigation)}",
        //        filter: c => c.IdCourseNavigation.IdSpeciality == id);
        //    //var list = await db.CourseAttachments
        //    //    .Include(c => c.IdCourseNavigation)
        //    //    .Include(c => c.IdAttachmanentNavigation)
        //    //    .Where(c => c.IdCourseNavigation.IdSpeciality == id)
        //    //    .ToListAsync();
        //    return courses.Select(c => c.IdCourseNavigation).ToList();
        //}

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
                    //bool isAttachmentNotNull = course.IdAttachmanentNavigation != null;
                    //db.Entry(course).State = EntityState.Added;

                    //if (isAttachmentNotNull)
                    //{
                    //    db.Entry(course.IdAttachmanentNavigation).State = EntityState.Detached;

                    //}

                    //if (courseId != -1)
                    //{
                    //    course.IdCourseNavigation.Id = courseId;
                    //    db.Entry(course.IdCourseNavigation).State = EntityState.Unchanged;
                    //}
                    //else
                    //{
                    //    db.Entry(course.IdCourseNavigation).State = EntityState.Added;
                    //}

                    //db.Entry(course.IdCourseNavigation.IdSpecialityNavigation).State = EntityState.Unchanged;

                    //if (isAttachmentNotNull)
                    //{
                    //    db.Entry(course.IdAttachmanentNavigation).State = EntityState.Added;
                    //}

                    //var entry = await db.CourseAttachments.AddAsync(course);
                    //await db.SaveChangesAsync();

                    //resultList.Add(entry.Entity);

                    //if (isAttachmentNotNull)
                    //{
                    //    db.Entry(course.IdAttachmanentNavigation).State = EntityState.Detached;
                    //}

                    //db.Entry(course.IdCourseNavigation.IdSpecialityNavigation).State = EntityState.Detached;
                    //db.Entry(course.IdCourseNavigation).State = EntityState.Detached;

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

                //var coursesList = await db.CourseAttachments
                //       .Where(c => c.IdCourse == id)
                //       .ToListAsync();

                //db.CourseAttachments.RemoveRange(coursesList);
                //var course = await db.Courses.FindAsync(id);
                //db.Courses.Remove(course);
                //await db.SaveChangesAsync();

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

                //foreach (var item in onDeleteList)
                //{
                //    _unitOfWork.CourseAttachmentRepository.Delete(item);
                //    _unitOfWork.Save();

                //    //courses.Remove(item);

                //    //db.Entry(item).State = EntityState.Deleted;
                //    //db.CourseAttachments.Remove(item);
                //    //await db.SaveChangesAsync();

                //    //courses.Remove(item);
                //}

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

                //var firstCourse = courses
                //    .Select(c => c.IdCourse)
                //    .FirstOrDefault();

                //var findedDbCourse = await db.Courses
                //    .Include(c => c.CourseAttachments)
                //    .FirstOrDefaultAsync(c => c.Id == firstCourse);

                //if (findedDbCourse == null)
                //{
                //    return false;
                //}

                //var courseAttachments = findedDbCourse.CourseAttachments.ToList();

                //var onChangeList = courses.Where(c => c.Id != 0).ToList();

                //var onDeleteList = courseAttachments.Where(c => onChangeList.All(l => c.Id != l.Id));

                //foreach (var item in onDeleteList)
                //{
                //    db.Entry(item).State = EntityState.Deleted;
                //    db.CourseAttachments.Remove(item);
                //    await db.SaveChangesAsync();

                //    courses.Remove(item);
                //}

                //foreach (var item in courses)
                //{
                //    if (item.Id == 0)
                //    {

                //        db.Entry(item).State = EntityState.Added;
                //        if (item.IdAttachmanentNavigation != null)
                //        {
                //            db.Entry(item.IdAttachmanentNavigation).State = EntityState.Added;
                //        }

                //        var r = db.Attach(findedDbCourse);
                //        r.CurrentValues.SetValues(item.IdCourseNavigation);

                //        await db.CourseAttachments.AddAsync(item);
                //        await db.SaveChangesAsync();

                //        continue;
                //    }
                //    else
                //    {
                //        var r = db.Attach(findedDbCourse);

                //        var attachment = db.CourseAttachments.Find(item.Id);

                //        var entry = db.CourseAttachments
                //            .Attach(attachment);

                //        //Если был удален единственный прикрепленный файл
                //        if (entry.Entity.IdAttachmanent != null && item.IdAttachmanent == null)
                //        {
                //            entry.Entity.IdAttachmanent = null;
                //        }

                //        item.IdCourseNavigation.IdSpecialityNavigation = await db.Specialities.FindAsync(item.IdCourseNavigation.IdSpeciality);

                //        r.CurrentValues.SetValues(item.IdCourseNavigation);

                //        await db.SaveChangesAsync();
                //    }
                //}
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


    }
}
