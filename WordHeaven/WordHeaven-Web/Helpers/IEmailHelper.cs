﻿using System.IO;
using System.Threading.Tasks;

namespace WordHeaven_Web.Helpers
{
    public interface IEmailHelper
    {
        Responses SendEmail(string email, string subject, string message);

        Task SendEmailWithAttachment(string email, string subject, string message, MemoryStream attachment);
    }
}