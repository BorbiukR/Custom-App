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

        public AccountController(AppDBContext context)
        {
            db = context;
        }

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
                var user = db.Users.Where(a => a.Email == model.Email).FirstOrDefault();
                if (user != null)
                {
                    await Authenticate(model.Email);

                    if (!user.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid credential provided");
                }
            }
            ViewBag.Message = ModelState;
            return View();
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
            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                var isExsist = IsEmailExsist(model.Email);
                if (isExsist)
                {
                    ModelState.AddModelError("EmailExsist", "Email alredy exsist");
                    return View(model);
                }

                user.ActivationCode = Guid.NewGuid();

                model.Password = Crypto.Hash(model.Password);
                model.ConfirmPassword = Crypto.Hash(model.ConfirmPassword);

                user.IsEmailVerified = false;

                db.Users.Add(user);
                await db.SaveChangesAsync();

                SendVertificationLinkEmail(user.Email, user.ActivationCode.ToString());

                message = " Registration successfully done. Account activation link " +
                          " has been sent to your email id: " + user.Email;
                Status = true;
            }
            else
            {
                ModelState.AddModelError("", "Incorrect login and (or) password");
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;

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

            var fromEmail = new MailAddress("simplekek@gmail.com", "Custom App");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "borbiukroman";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your account is" +
                       " successfully created. Please click on the below link to verify your account" +
                       " <br/><br/><a href='" + verifyUrl + "'>" + verifyUrl + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                       "<br/><br/><a href=" + verifyUrl + ">Reset Password link</a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            using var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            smtp.Send(message);
        }

        [HttpGet]
        public IActionResult VerifyAccount(string id)
        {
            bool Status = false;

            var user = db.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
            if (user != null)
            {
                user.IsEmailVerified = true;
                db.SaveChanges();
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
        public IActionResult ForgotPassword(string Email)
        {
            string message = "";

            var user = db.Users.Where(a => a.Email == Email).FirstOrDefault();
            if (user != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVertificationLinkEmail(user.Email, resetCode, "ResetPassword");
                user.ResetPasswordCode = resetCode;
                db.SaveChanges();
                message = "Reset password link has been sent to your email id.";
            }
            else
            {
                message = "Account not found";
            }

            ViewBag.Message = message;
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
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";

            if (ModelState.IsValid)
            {
                var user = db.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.Password = Crypto.Hash(model.NewPassword);
                    user.ResetPasswordCode = "";
                    db.SaveChanges();
                    message = "New password updated successfully";
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
