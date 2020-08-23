using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace CareerWeb.Controllers
{
    public class EmployeeController : Controller
    {
       // private object db;

        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchCandidate()
        {
            ViewBag.JobListMain = new JobMajorDao().ListJobMain();
            ViewBag.JobListSub = new JobMajorDao().ListJobSub();
            ViewBag.AreaList = new AreaDao().ListArea();
            return View();

        }

        public ActionResult SearchCandidateResult(string KeyWord = "0", int AreaID = 0, int JobID = 0)
        {
            ViewBag.JobListMain = new JobMajorDao().ListJobMain();
            ViewBag.AreaList = new AreaDao().ListArea();
            ViewBag.Language = new LanguageDao().ReturnList();
            ViewBag.ListExperience = new ExperienceDao().ListExperiences();
            ViewBag.ListSalary = new SalaryDao().ListSalary();
            ViewBag.ListPositionEmployee = new PositionEmployeeDao().ReturnList();
            ViewBag.ListLevelLearning = new LevelLearningDao().ReturnList();


            var Model = new UserDao().ListUserFit(KeyWord, AreaID, JobID).ToList();
            return View(Model);


        }
        public ActionResult ListAndCreateOffer(int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }
            var accID = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accID);
            var listOffer = new OfferJobDao().ListByEmployee(acc.UserId);
            ViewBag.ListMajor = new JobMajorDao().ListJobMain();
            ViewBag.ListSkill = new JobMajorDao().ListJobSub();
            ViewBag.ListArea = new AreaDao().ListArea();
            ViewBag.ListPosition = new PositionEmployeeDao().ReturnList();
            ViewBag.ListSalary = new SalaryDao().ListSalary();
            ViewBag.ListLearning = new LevelLearningDao().ReturnList();
            ViewBag.ListExperience = new ExperienceDao().ListExperiences();
            return View(listOffer);
        }


        [HttpPost]
        public JsonResult CreateAccountInfor(Guid UserID, Guid EnterpriseID, string EmployeeName, int Position, string Sex, string BirthDay, string Email, string Mobile, string Code)
        {
            var employeeInfor = new Employee();
            employeeInfor.EmployeeID = UserID;
            employeeInfor.EnterpriseID = EnterpriseID;
            employeeInfor.EmployeeName = EmployeeName;
            employeeInfor.Position = Position;
            employeeInfor.Sex = Sex;
            employeeInfor.Email = Email;
            employeeInfor.Mobile = Mobile;
            string[] splitDate = BirthDay.Split('-');
            employeeInfor.BirthDay = new DateTime(int.Parse(splitDate[0]), int.Parse(splitDate[1]), int.Parse(splitDate[2]));
            var ent = new EnterpriseDao().FindById(EnterpriseID);
            if (ent.Code == Code)
            {
                var checkInsertEmployee = new EmployeeDao().InsertEmployee(employeeInfor);
                return Json(new
                {
                    status = checkInsertEmployee,
                    codeInput = true
                });
            }
            return Json(new
            {
                codeInput = false
            });
        }
        public ActionResult ListArticleEmployee()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }
            var accID = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accID);
            var ListArticle = new ArticleDao().ListByEmployee(acc.UserId);
            ViewBag.ListCategory = new CategoryArticleDao().ListParent();
            return View(ListArticle);
        }
        [HttpPost]
        public JsonResult WorkInvitationData(Guid userId, Guid offerId, String date, String salary, String address, String note)
        {
            var workinvitation = new WorkInvitation();
            workinvitation.UserID = userId;
            workinvitation.OfferID = offerId;
            workinvitation.StartDay = date;
            workinvitation.Salary = salary;
            workinvitation.Address = address;
            workinvitation.Note = note;
            workinvitation.Status = "waiting";
            var checkInsertWorkInvitation = new WorkInvitationDao().InsertWorkInvitation(workinvitation);
            if (checkInsertWorkInvitation == false)
            {
                return Json(new
                {
                    status = false
                });
            }
            var checkUpdate = new AppliedCandidateDao().UpdateStatus(userId, offerId, "Mời làm");
            var checkInterviewUpdate = new InterviewDao().UpdateStatus(userId, offerId, "done");
            return Json(new
            {
                status = true
            });
        }

        [HttpPost]
        public JsonResult checkStatusWorkInvitation(Guid userId, Guid offerId)
        {
            var workinvitation = new WorkInvitationDao().findWorkInvitation(userId, offerId);
            if (workinvitation != null)
            {
                var user = new UserDao().FindById(userId);
                var offer = new OfferJobDao().FindByID(offerId);
                return Json(new
                {
                    having = true,
                    User = user.UserName,
                    WorkInvitation = workinvitation.WorkInvitationID,
                    Offer = offer.OfferName,
                    StartDay = workinvitation.StartDay,
                    Salary = workinvitation.Salary,
                    Address = workinvitation.Address,
                    Note = workinvitation.Note,
                    Status = workinvitation.Status
                }); ;
            }
            return Json(new
            {
                having = false
            });

        }
        [HttpPost]
        public JsonResult InterviewData(Guid userId, Guid offerId, Guid employeeId, String date, String time, String address, String note)
        {
            var interview = new Interview();
            interview.Date = date;
            interview.Time = time;
            interview.Address = address;
            interview.Note = note;
            interview.UserID = userId;
            interview.OfferID = offerId;
            interview.EmployeeID = employeeId;
            interview.Status = "waiting";
            var checkInsertInterview = new InterviewDao().InsertInterview(interview);
            if (checkInsertInterview == false)
            {
                return Json(new
                {
                    status = false
                });
            }
            var checkUpdate = new AppliedCandidateDao().UpdateStatus(userId, offerId, "Phỏng vấn");
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult DeleteSavedCandidate(Guid userId)
        {
            var accID = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accID);
            var employee = new EmployeeDao().FindById(acc.UserId);
            var enterpriseId = employee.EnterpriseID;
           // Guid enterpriseId = new Guid("4c0bba20-27f2-4838-8cb9-ce3a80ce7784");
            var savedCandidate = new SavedCandidateDao().findCandidate(userId, enterpriseId);
            var checkDeleteCandidate = new SavedCandidateDao().DeleteCandidate(savedCandidate);
            if (checkDeleteCandidate == false)
            {
                return Json(new
                {
                    status = false
                });
            }
            return Json(new
            {
                status = true
            });
        }
        public ActionResult JobApplicationsUser()
        {
            var accid = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accid);
            var employee = new EmployeeDao().FindById(acc.UserId);
            var enterpriseId = employee.EnterpriseID;

           // Guid enterpriseId = new Guid("4c0bba20-27f2-4838-8cb9-ce3a80ce7784");

            ViewBag.AppliedCandidate = new AppliedCandidateDao().AppliedCandidateList(enterpriseId);
            ViewBag.SavedCandidate = new SavedCandidateDao().ListCandidateSave(enterpriseId);



            return View();
        }
        public ActionResult ShowDetailCandidate(Guid UserId)
        {
            ViewBag.ListExperience = new UserExperienceDao().ListByUser(UserId);
            ViewBag.ListLanguage = new UserForeignLanguageDao().ListByUser(UserId);
            var showInfo = new UserDao().InfoUser(UserId);
            var saveName = "";
            foreach (var item in showInfo)
            {
                for (var i = 0; i < item.listJob.Count; i += 1)
                {
                    saveName += (item.listJob[i]) + ", ";
                }
                saveName.Remove(saveName.Length - 1);
            }
            ViewBag.ListFullJobName = saveName;
            return View(showInfo);
        }

        public ActionResult Interview(Guid userId, Guid offerId)
        {
            ViewBag.CandidateDetail = new UserDao().FindById(userId);
            ViewBag.OfferDetail = new OfferJobDao().FindByID(offerId);
            if (new AppliedCandidateDao().findCandidate(userId, offerId).Status == "Đã nộp hồ sơ")
            {
                var checkUpdate = new AppliedCandidateDao().UpdateStatus(userId, offerId, "Đã xem hồ sơ");
            }
            ViewBag.CandidateApplied = new AppliedCandidateDao().findCandidate(userId, offerId);
            ViewBag.WorkInvitation = new WorkInvitationDao().findWorkInvitation(userId, offerId);
            return View();
        }

        [HttpPost]
        public JsonResult SaveCandidate(Guid userId)
        {
            var accID = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accID);
            var employee = new EmployeeDao().FindById(acc.UserId);
            var enterpriseId = employee.EnterpriseID;
            var savedCandidate = new SavedCandidate();
         //   Guid enterpriseId = new Guid("4c0bba20-27f2-4838-8cb9-ce3a80ce7784");
            DateTime date = DateTime.Today;
            savedCandidate.UserID = userId;
            savedCandidate.EnterpriseID = enterpriseId;
            savedCandidate.CreateDate = date.ToString("dd/MM/yyyy");
            var checkInsertCandidate = new SavedCandidateDao().InsertCandidate(savedCandidate);
            if (checkInsertCandidate == false)
            {
                return Json(new
                {
                    status = false
                });
            }
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult checkSavedCandidate(List<Guid> userId)
        {
            var accID = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accID);
            var employee = new EmployeeDao().FindById(acc.UserId);
            var enterpriseId = employee.EnterpriseID;
            List<Boolean> save = new List<bool>();
           // Guid enterpriseId = new Guid("4c0bba20-27f2-4838-8cb9-ce3a80ce7784");

            if (userId != null)
            {
                foreach (var item in userId)
                {
                    var user = new UserDao().FindById(item);
                    if (new SavedCandidateDao().findCandidate(user.UserId, enterpriseId) != null)
                    {
                        var check = true;
                        save.Add(check);
                    }
                    else
                    {
                        var check = false;
                        save.Add(check);
                    }
                }
                return Json(new
                {
                    savedList = save
                });
            }

            return Json(new
            {
                saveList = save
            });

        }

        public ActionResult EmployeeList()
        {
            var accID = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accID);
            var employee = new EmployeeDao().FindById(acc.UserId);
            var enterpriseId = employee.EnterpriseID;
           // Guid enterpriseId = new Guid("4c0bba20-27f2-4838-8cb9-ce3a80ce7784");
            var employees = new EmployeeDao().ListEmployee(enterpriseId);
            return View(employees);
        }

        public ActionResult InterviewSchedule()
        {
            var accID = int.Parse(User.Identity.Name);
            var acc = new AccountDao().FindAccountById(accID);
            var employee = new EmployeeDao().FindById(acc.UserId);
            var enterpriseId = employee.EnterpriseID;

           // Guid enterpriseId = new Guid("4c0bba20-27f2-4838-8cb9-ce3a80ce7784");
            var interview = new InterviewDao().ListInterviewByEnterprise(enterpriseId);
            return View(interview);
        }
        [HttpPost]
        public JsonResult FailedCV(Guid userId, Guid offerId)
        {

            var checkUpdate = new AppliedCandidateDao().UpdateStatus(userId, offerId, "Loại hồ sơ");
            if (checkUpdate == true)
            {
                return Json(new
                {
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }

        [HttpPost]
        public JsonResult FailedInterview(Guid userId, Guid offerId)
        {

            var checkUpdate1 = new AppliedCandidateDao().UpdateStatus(userId, offerId, "Trượt pv");
            var checkUpdate2 = new InterviewDao().UpdateStatus(userId, offerId, "fail");
            if (checkUpdate1 == true)
            {
                return Json(new
                {
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }
        [HttpPost]
        public JsonResult DeleteEmployee(Guid employeeId)
        {
            var check = new EmployeeDao().Delete(employeeId);
            if (check == false)
            {
                return Json(new
                {
                    status = false
                });
            }
            return Json(new
            {
                status = true
            });
        }


    }
}