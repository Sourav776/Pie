using PieChart.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System;
using Microsoft.Ajax.Utilities;

namespace PieChart.Controllers
{
    public class CarController : Controller
    {
        private pieChartEntities db = new pieChartEntities();

        // GET: CARs
        public ActionResult Index()
        {
            return View(db.CAR.ToList());
        }

        // GET: CARs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAR cAR = db.CAR.Find(id);
            if (cAR == null)
            {
                return HttpNotFound();
            }
            return View(cAR);
        }

        // GET: CARs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CARs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CAR_ID,MANUFACTURER,MODEL,YEAR,PRODUCING_COUNTRY")] CAR cAR)
        {
            if (ModelState.IsValid)
            {
                db.CAR.Add(cAR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cAR);
        }

        // GET: CARs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAR cAR = db.CAR.Find(id);
            if (cAR == null)
            {
                return HttpNotFound();
            }
            return View(cAR);
        }

        // POST: CARs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CAR_ID,MANUFACTURER,MODEL,YEAR,PRODUCING_COUNTRY")] CAR cAR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cAR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cAR);
        }

        // GET: CARs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CAR cAR = db.CAR.Find(id);
            if (cAR == null)
            {
                return HttpNotFound();
            }
            return View(cAR);
        }

        // POST: CARs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CAR cAR = db.CAR.Find(id);
            db.CAR.Remove(cAR);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public JsonResult GetSearchValue(string search)
        {
            try
            {

                var searchData = db.CAR.Where(x => x.MODEL.Contains(search) || x.MANUFACTURER.Contains(search) || x.PRODUCING_COUNTRY.Contains(search)).Distinct().ToList();
                var ret = new List<ReturnData>();
                foreach (var item in searchData)
                {
                    var unit = new ReturnData();

                    unit.Key = item.MANUFACTURER;
                    ret.Add(unit);
                    unit.Key = item.MODEL;
                    ret.Add(unit);
                    unit.Key = item.PRODUCING_COUNTRY;
                    ret.Add(unit);
                }
                if(ret.Any())
                {
                    ret = ret.DistinctBy(x=>x.Key).ToList();
                }
                //foreach(var item in searchData)
                //{
                //    foreach(var val in item)
                //    {
                //        var ret = new ReturnData();

                //    }
                //}

                //List <CarModel> allsearch = db.CAR.Where(x =>  x.MODEL.Contains(search)).Select(x => new CarModel
                //{
                //    MODEL=x.MODEL
                //}).Distinct().ToList();
                return new JsonResult { Data = ret, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch(Exception ex)
            {
                return new JsonResult { Data = "Error", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public ActionResult GetData()
        {
            try
            {
                var data = db.CAR.GroupBy(x => x.MANUFACTURER).ToList();
                var returnObj = new List<ReturnData>();
                foreach (var item in data)
                {
                    var unit = new ReturnData();
                    unit.Key = item.Key;
                    unit.Number = item.Count();
                    returnObj.Add(unit);
                }
                return Json(returnObj, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FilterDataChart(string value)
        {
            try
            {
                if (value == "")
                {
                    var data = db.CAR.GroupBy(x => x.MANUFACTURER).ToList();
                    var returnObj = new List<ReturnData>();
                    foreach (var item in data)
                    {
                        var unit = new ReturnData();
                        unit.Key = item.Key;
                        unit.Number = item.Count();
                        returnObj.Add(unit);
                    }
                    return Json(returnObj, JsonRequestBehavior.AllowGet);
                }
                else

                {
                    var data = db.CAR.Where(x => x.MODEL.Contains(value)).GroupBy(x => x.MANUFACTURER).ToList();
                    var returnObj = new List<ReturnData>();
                    foreach (var item in data)
                    {
                        var unit = new ReturnData();
                        unit.Key = item.Key;
                        unit.Number = item.Count();
                        returnObj.Add(unit);
                    }
                    return Json(returnObj, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public PartialViewResult FilterDataTable(string value)
        {
           
                var data = new List<CAR>();
                if (value == "")
                    data = db.CAR.ToList();
                else
                    data = db.CAR.Where(x => x.MODEL.Contains(value)).ToList();
                return PartialView("CarList", data);
            
            
        }
        public  ActionResult StoreToDataBase( HttpPostedFileBase file)
        {
            try
            {
                string filePath = string.Empty;
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                file.SaveAs(filePath);

                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);

                //Execute a loop over the rows.
                int cnt = 0;
                foreach (string row in csvData.Split('\n'))
                {
                    cnt++;
                    if (cnt== 1) continue;
                    if (!string.IsNullOrEmpty(row))
                    {
                        var car = new CAR();
                        car.MANUFACTURER = row.Split(',')[0];
                        car.MODEL = row.Split(',')[1];
                        car.YEAR = row.Split(',')[2];
                        car.PRODUCING_COUNTRY = row.Split(',')[3];
                        db.CAR.Add(car);
                        db.SaveChanges();
                    }
                    
                }


                return Json("Success",JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("Error",JsonRequestBehavior.AllowGet);
            }
        }
        public class ReturnData
        {
            public string Key { get; set; }
            public long Number { get; set; }
        }
     public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }

}
