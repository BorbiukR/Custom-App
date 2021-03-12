using Web.Models;
using Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Mail;
using System.Net;
using Web.ViewModels.Identity;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext db;

        public AccountController(AppDBContext context) => db = context;


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(a => a.Email == model.Email && a.Password == model.Password).FirstOrDefault();

                if (user != null)
                {
                    await Authenticate(model.Email);

                    if (!user.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid credential provided");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model, User user)
        {
            if (ModelState.IsValid)
            {
                var isExsist = IsEmailExsist(model.Email);

                if (isExsist)
                {
                    ModelState.AddModelError("EmailExsist", "Email alredy exsist");
                    return View(model);
                }

                user.ActivationCode = Guid.NewGuid();

                model.Password = Cryptography.Hash(model.Password);
                model.ConfirmPassword = Cryptography.Hash(model.ConfirmPassword);

                user.IsEmailVerified = false;

                db.Users.Add(user);
                await db.SaveChangesAsync();

                SendVertificationLinkEmail(user.Email, user.ActivationCode.ToString());

                ViewBag.Message = " Registration successfully done. Account activation link " +
                                  " has been sent to your email id: " + user.Email;
                user.IsEmailVerified = true;
            }
            else
            {
                ModelState.AddModelError("", "Incorrect login and (or) password");
            }

            ViewBag.Status = user.IsEmailVerified;

            return View(model);
        }

        [NonAction]
        public bool IsEmailExsist(string email)
        {
            var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
            return user != null;
        }

        [NonAction]
        public void SendVertificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = Request.Scheme + "://" + Request.Host + ":" + "/Account/" + emailFor + "/" + activationCode;

            MailMessage message = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress("robotaborbiuk@gmail.com", "Custom App")
            };
            message.To.Add(email);

            if (emailFor == "VerifyAccount")
            {
                message.Subject = "Your account is successfully created!";
                message.Body = "<br/><br/>We are excited to tell you that your account is" +
                               "successfully created. Please click on the below link to verify your account" +
                               "<br/><br/><a href='" + verifyUrl + "'>" + verifyUrl + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                message.Subject = "Reset Password";
                message.Body = "Hi,<br/><br/>We got request for reset your account password." +
                               " Please click on the below link to reset your password" +
                               "<br/><br/><a href=" + verifyUrl + ">Reset Password link</a>";
            }

            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("robotaborbiuk@gmail.com", "Robotaborbiuk1234");
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(message);
            };
        }

        [HttpGet]
        public async Task<IActionResult> VerifyAccount(string id)
        {
            bool Status = false;

            var user = db.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
            if (user != null)
            {
                user.IsEmailVerified = true;
                await db.SaveChangesAsync();
                Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid Request";
            }

            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            var user = db.Users.Where(a => a.Email == Email).FirstOrDefault();
            if (user != null)
            {
                string resetCode = Guid.NewGuid().ToString();

                SendVertificationLinkEmail(user.Email, resetCode, "ResetPassword");

                user.ResetPasswordCode = resetCode;

                await db.SaveChangesAsync();

                ViewBag.Message = "Reset password link has been sent to your email.";
            }
            else
            {
                ViewBag.Message = "Email not found";
            }
            return View();
        }

        public IActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return StatusCode(401);
            }

            var user = db.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                var model = new ResetPasswordModel
                {
                    ResetCode = id
                };
                return View(model);
            }
            else
            {
                return StatusCode(401);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.Password = Cryptography.Hash(model.NewPassword);

                    user.ResetPasswordCode = "";

                    await db.SaveChangesAsync();

                    ViewBag.Message = "New password updated successfully";
                }
            }
            else
            {
                ViewBag.Message = "Something invalid";
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}