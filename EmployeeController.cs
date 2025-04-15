using EmployeeManagmentProjectTask.DbContext;
using EmployeeManagmentProjectTask.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagmentProjectTask.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            ViewBag.Departmet = DepartmentDDl();
            ViewBag.Designation = DesignationDDl();

            EmployeeModel model = new EmployeeModel();
            var familymember = new List<FamilyMemberModel>();

            CommanModel comman = new CommanModel()
            {
                employeemodel = model,
                familyMembers = familymember
            };

            model.EmpCode = GenerateCode();
            return View(comman);
        }
        public ActionResult EmployeePartial()
        {
            ViewBag.Departmet = DepartmentDDl();
            ViewBag.Designation = DesignationDDl();
            EmployeeModel model = new EmployeeModel();
            model.EmpCode = GenerateCode();
            return View(model);
        }
        public ActionResult FamilyMemberPartial()
        {
            return View();
        }
        public ActionResult EmployeeReport()
        {
            DbEmployeeManagementTaskEntities db = new DbEmployeeManagementTaskEntities();


            return View(db.SpEmployee().ToList());
        }
        public string GenerateCode()
        {
            DbEmployeeManagementTaskEntities db = new DbEmployeeManagementTaskEntities();
            int EMPCount = db.EmployeeTables.Count() + 1;
            string formattedEMPCount;

            if (EMPCount < 10)
            {
                formattedEMPCount = "EMP00" + EMPCount.ToString();
            }
            else if (EMPCount < 100)
            {
                formattedEMPCount = "EMP0" + EMPCount.ToString();
            }
            else
            {
                formattedEMPCount = "EMP" + EMPCount.ToString();
            }

            return formattedEMPCount;
        }
        public List<SelectListItem> DepartmentDDl()
        {
            var Departmentlist = new List<SelectListItem>();
            //   Departmentlist.Add(new SelectListItem { Text = "--Select--", Value = "0" });
            try
            {

                DbEmployeeManagementTaskEntities db = new DbEmployeeManagementTaskEntities();

                var DepartmetgetData = (from x in db.Departmenttables
                                        select new
                                        {
                                            x.Departmentid,
                                            x.DepartmentName

                                        });
                foreach (var item in DepartmetgetData)
                {
                    Departmentlist.Add(new SelectListItem { Text = item.DepartmentName.ToString(), Value = item.Departmentid.ToString() });
                }


            }
            catch (Exception)
            {

                throw;
            }
            return Departmentlist;
        }
        public List<SelectListItem> DesignationDDl()
        {
            var Designationlist = new List<SelectListItem>();
            // Designationlist.Add(new SelectListItem { Text = "--Select--", Value = "0" });
            try
            {

                DbEmployeeManagementTaskEntities db = new DbEmployeeManagementTaskEntities();

                var DesignationgetData = (from x in db.DesignationTables
                                          select new
                                          {
                                              x.DesignationId,
                                              x.DesignationName

                                          });
                foreach (var item in DesignationgetData)
                {
                    Designationlist.Add(new SelectListItem { Text = item.DesignationName.ToString(), Value = item.DesignationId.ToString() });
                }


            }
            catch (Exception)
            {
                throw;
            }
            return Designationlist;
        }
        public ActionResult SaveOrUpdate(FormCollection collection)
        {
            int Empid = 0;


            // string dobString = collection.Get("DOB");
            // string dateOfJoin = collection.Get("DateOfJoin");


            DbEmployeeManagementTaskEntities db = new DbEmployeeManagementTaskEntities();
            if (Convert.ToInt32(collection.Get("EmployeeId")) == 0)
            {
                EmployeeTable model = new EmployeeTable()
                {
                    EmpCode = Convert.ToString(collection.Get("EmpCode")),
                    FirstName = Convert.ToString(collection.Get("FirstName")),
                    LastName = Convert.ToString(collection.Get("LastName")),
                    Mobile = Convert.ToString(collection.Get("Mobile")),
                    Email = Convert.ToString(collection.Get("Email")),
                    DateOfJoin = Convert.ToDateTime(collection.Get("DateOfJoin")),
                    DOB = Convert.ToDateTime(collection.Get("DOB")),

                    Departmentid = Convert.ToInt32(collection.Get("Departmentid")),
                    Designationid = Convert.ToInt32(collection.Get("Designationid")),

                };
                db.Entry(model).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                Empid = model.EmployeeId;
            }
            else
            {
                EmployeeTable model = new EmployeeTable()
                {
                    EmployeeId = Convert.ToInt32(collection.Get("EmployeeId")),
                    EmpCode = Convert.ToString(collection.Get("EmpCode")),
                    FirstName = Convert.ToString(collection.Get("FirstName")),
                    LastName = Convert.ToString(collection.Get("LastName")),
                    Mobile = Convert.ToString(collection.Get("Mobile")),
                    Email = Convert.ToString(collection.Get("Email")),
                    DateOfJoin = Convert.ToDateTime(collection.Get("DateOfJoin")),
                    DOB = Convert.ToDateTime(collection.Get("DOB")),

                    Departmentid = Convert.ToInt32(collection.Get("Departmentid")),
                    Designationid = Convert.ToInt32(collection.Get("Designationid")),

                };
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                Empid = model.EmployeeId;
            }
            string[] Familyid = collection.Get("item.Familyid").Split(',');
            string[] Relation = collection.Get("item.Relation").Split(',');
            string[] Name = collection.Get("item.Name").Split(',');
            string[] DOB = collection.Get("item.FDOB").Split(',');
            string[] Mobile = collection.Get("item.Mobile").Split(',');
            string[] Email = collection.Get("item.Email").Split(',');
            int arrayLength = Relation.Length;

            for (int i = 0; i < arrayLength; i++)
            {
                int currentFamilyid;
                int.TryParse(Familyid[i], out currentFamilyid);
                string currentRelation = Relation[i];
                string currentName = Name[i];
                DateTime currentDOB = Convert.ToDateTime(DOB[i]);
                string currentMobile = Mobile[i];
                string currentEmail = Email[i];

                var existingFamilyMember = db.FamilyMembers
               .Where(f => f.Familyid == currentFamilyid).FirstOrDefault();

                if (existingFamilyMember == null)
                {
                    FamilyMember model = new FamilyMember()
                    {
                        EmployeeId = Empid,
                        Relation = currentRelation,
                        Name = currentName,
                        FDOB = currentDOB,
                        Mobile = currentMobile,
                        Email = currentEmail
                    };
                    db.Entry(model).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();
                    TempData["Good"] = "Your Data Saved Successfuly..!";

                }
                else
                {

                    existingFamilyMember.Familyid = currentFamilyid;
                    existingFamilyMember.EmployeeId = Empid;
                    existingFamilyMember.Relation = currentRelation;
                    existingFamilyMember.Name = currentName;
                    existingFamilyMember.FDOB = currentDOB;
                    existingFamilyMember.Mobile = currentMobile;
                    existingFamilyMember.Email = currentEmail;


                    db.Entry(existingFamilyMember).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["Good"] = "Your Data Update Successfuly..!";

            }


            return RedirectToAction("Index");
        }


        public ActionResult GetById(int id)
        {
            ViewBag.Departmet = DepartmentDDl();
            ViewBag.Designation = DesignationDDl();
            DbEmployeeManagementTaskEntities db = new DbEmployeeManagementTaskEntities();

            var data = new EmployeeTable();

            data = db.EmployeeTables.Where(x => x.EmployeeId == id).FirstOrDefault();

            EmployeeModel model = new EmployeeModel()
            {

                EmployeeId = data.EmployeeId,
                EmpCode = data.EmpCode,
                FirstName = data.FirstName,
                LastName = data.LastName,
                DateOfJoin = data.DateOfJoin,
                DOB = data.DOB,
                Mobile = data.Mobile,
                Email = data.Email,
                Departmentid = data.Departmentid,
                Designationid = data.Designationid,
            };

            var FdataList = db.FamilyMembers
           .Where(x => x.EmployeeId == id)
           .ToList();

            List<FamilyMemberModel> list = new List<FamilyMemberModel>();
            foreach (var item in FdataList)
            {
                var Fmodel = new FamilyMemberModel()
                {
                    Familyid = item.Familyid,
                    Name = item.Name,
                    Relation = item.Relation,
                    Mobile = item.Mobile,
                    Email = item.Email,
                    //EmployeeId = Fdata.EmployeeId,  
                    FDOB = item.FDOB,
                };
                list.Add(Fmodel);
            }
            CommanModel cmd = new CommanModel()
            {
                employeemodel = model,
                familyMembers = list

            };
            return View("Index", cmd);
        }
        public ActionResult Delete(int id)
        {
            DbEmployeeManagementTaskEntities db = new DbEmployeeManagementTaskEntities();


            var familyMembers = db.FamilyMembers.Where(f => f.EmployeeId == id).ToList();
            foreach (var member in familyMembers)
            {
                db.FamilyMembers.Remove(member);
            }
            var emp = db.EmployeeTables.Find(id);
            if (emp != null)
            {
                db.EmployeeTables.Remove(emp);
            }

            db.SaveChanges();
            TempData["Delete"] = "Data Deleted Successfully!";
            return RedirectToAction("EmployeeReport");
        }

    }
}