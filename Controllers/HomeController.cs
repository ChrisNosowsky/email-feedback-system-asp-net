using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using SendEmailInAspNetCore.Helpers;
using SendEmailInAspNetCore.Models;

namespace SendEmailInAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }


        [HttpPost]
        public IActionResult Contact(ContactViewModel contactViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string emailBody = "";

                    var message = new MimeMessage();

                    Console.WriteLine(contactViewModel.Email);

                    //Setting the To Email address
                    message.To.Add(new MailboxAddress("Admin", "admin@google.com"));
                    // Setting the From Email address
                    message.From.Add(new MailboxAddress(contactViewModel.Name, contactViewModel.Email));
                    // Email subject 
                    message.Subject = contactViewModel.Subject;
                    ViewBag.emailContent = contactViewModel;
                    // Email message body
                    emailBody = EmailHelper.RenderView(this, "Welcome", null);

                    message.Body = new TextPart(TextFormat.Html)
                    {
                        Text = emailBody
                    };
                    //Configure the e-mail
                    using (var emailClient = new SmtpClient())
                    {
                        emailClient.Connect("localhost", 25, false);
                        //emailClient.Authenticate("email", "password");
                        emailClient.Send(message);
                        emailClient.Disconnect(true);
                    }
                }
                catch (Exception e)
                {

                    ModelState.Clear();
                    ViewBag.Exception = $"Error:  {e.Message}";
                }

            }
            return View();
        }
    }
}
