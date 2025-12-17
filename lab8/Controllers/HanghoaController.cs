using buoi5.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.IO;
using System.Linq;

using System.Web;
using static System.Net.WebRequestMethods;

namespace buoi5.Controllers
{
    public class HanghoaController : Controller
    {
        private QLBHContext db = new QLBHContext();
        Models.QLBHContext obj = new Models.QLBHContext();
      
        public IActionResult Index()
        {
            var hanghoa = db.Hanghoa.Include(h => h.MaloaiNavigation).Include(h => h.MansxNavigation);
            return View(hanghoa.ToList());
        }
        public IActionResult chiTietHanghoa(string id)
        {
            
            var a = obj.Hanghoa.Include(h => h.MaloaiNavigation).Include(h => h.MansxNavigation).Where(n => n.Mahang == id).FirstOrDefault();
            return View(a);


        }
        public IActionResult formThemHH()
        {
            ViewBag.DSLHH = new SelectList(db.Loaihanghoa.ToList(), "Maloai", "Maloai");
            ViewBag.DSNSX = new SelectList(db.Nhasanxuat.ToList(), "Mansx", "Mansx");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemHH([FromForm] Models.HangHoaModel hanghoa)
        {
            Hanghoa n = db.Hanghoa.Find(hanghoa.Mahang);

            if (n == null && ModelState.IsValid)
            {
                Hanghoa h = new Hanghoa();
                h.Mahang = hanghoa.Mahang;
                h.Tenhang = hanghoa.Tenhang;
                h.Dongia = hanghoa.Dongia;
                h.Donvitinh = hanghoa.Donvitinh;
                h.Maloai = hanghoa.Maloai;
                h.Mansx = hanghoa.Mansx;
                h.MaloaiNavigation = db.Loaihanghoa.Find(hanghoa.Mansx);
                h.MansxNavigation = db.Nhasanxuat.Find(hanghoa.Maloai);

                if (hanghoa.Hinh.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", hanghoa.Mahang + "_" + hanghoa.Hinh.FileName);
                    using (var s = System.IO.File.Create(path))
                    {
                        hanghoa.Hinh.CopyTo(s);
                    }
                    h.Hinh = "/images/" + hanghoa.Mahang + "_" + hanghoa.Hinh.FileName;
                }
                else
                {
                    h.Hinh = "";
                }

                db.Hanghoa.Add(h);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else if (n != null)
            {
                //ViewBag.nv = n;
                ModelState.AddModelError("Mahang", "Mã hàng hóa này đã tồn tại!");
                // return View("loiThemHH", hanghoa);
                return View("formThemHH", hanghoa);
            }
            else
            {
                return View("formThemHH");
            }
        }

        [HttpGet]
        public ActionResult loiThemHH(Models.HangHoaModel hanghoa)
        {

            return View(hanghoa);
        }


        public IActionResult xemNsx(string id)
        {
            Nhasanxuat x = db.Nhasanxuat.Find(id); 

            return PartialView(x);
        }
        public IActionResult xemLHH(string id)
        {
            Loaihanghoa x = db.Loaihanghoa.Find(id); 

            return PartialView(x);
        }


        [HttpGet]
        public IActionResult formXoaHH(string id)
        {
            ViewBag.flag = ((db.Chitietphieudathang.Where(x => x.Mahang == id).Count()) + (db.Chitietphieugiaohang.Where(x => x.Mahang == id).Count()));
            Hanghoa hh = db.Hanghoa.Find(id);

            return View(hh);

        }

        [HttpPost]
        public IActionResult xoaHH(string id)
        {
            Hanghoa hh = db.Hanghoa.Find(id);

            if (!string.IsNullOrEmpty(hh.Hinh))
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", hh.Hinh.TrimStart('/'));
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }
            db.Hanghoa.Remove(hh);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult formsuaHH(string id)
        {
            Hanghoa hh = db.Hanghoa.Find(id);
            SelectList DSNSX = new SelectList(db.Nhasanxuat.ToList(), "Mansx", "Tennsx");
            SelectList DSLHH = new SelectList(db.Loaihanghoa.ToList(), "Maloai", "Tenloai");

            ViewBag.DSNSX = DSNSX;
            ViewBag.DSLHH = DSLHH;
            return View(hh);
        }

        [HttpPost]
        public IActionResult suaHH([FromForm] Models.HangHoaModel hanghoacu)
        {
            // 1. Tìm đối tượng gốc trong DB
            Hanghoa hhmoi = db.Hanghoa.Find(hanghoacu.Mahang);
            if (hhmoi == null) return NotFound();

            // 2. Cập nhật thông tin
            hhmoi.Tenhang = hanghoacu.Tenhang;
            hhmoi.Dongia = hanghoacu.Dongia;
            hhmoi.Donvitinh = hanghoacu.Donvitinh;
            hhmoi.Maloai = hanghoacu.Maloai;
            hhmoi.Mansx = hanghoacu.Mansx;

            // Sửa lại tìm đúng ID cho Navigation
            hhmoi.MaloaiNavigation = db.Loaihanghoa.Find(hanghoacu.Maloai);
            hhmoi.MansxNavigation = db.Nhasanxuat.Find(hanghoacu.Mansx);

            // 3. Xử lý hình ảnh (Dùng hanghoacu.Hinh là IFormFile)
            if (hanghoacu.Hinh != null && hanghoacu.Hinh.Length > 0)
            {
                string fileName = hhmoi.Mahang + "_" + Path.GetFileName(hanghoacu.Hinh.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var s = System.IO.File.Create(path))
                {
                    hanghoacu.Hinh.CopyTo(s);
                }
                // Gán đường dẫn chuỗi vào db
                hhmoi.Hinh = "/images/" + fileName;
            }

            // 4. QUAN TRỌNG: Lưu thay đổi
            db.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}
