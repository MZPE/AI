﻿using System;
using System.Collections.Generic;
using EmailSkill.Dialogs.Shared.Resources.Strings;
using EmailSkill.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Solutions.Extensions;
using Microsoft.Bot.Builder.Solutions.Resources;
using Microsoft.Graph;

namespace EmailSkill.Util
{
    public class SpeakHelper
    {
        public static string ToSpeechEmailListString(List<Message> messages, TimeZoneInfo timeZone, int maxReadSize = 1)
        {
            string speakString = string.Empty;

            if (messages == null || messages.Count == 0)
            {
                return speakString;
            }

            var emailDetail = string.Empty;

            int readSize = Math.Min(messages.Count, maxReadSize);
            if (readSize >= 1)
            {
                emailDetail = ToSpeechEmailDetailOverallString(messages[0], timeZone);
            }

            speakString = emailDetail;
            return speakString;
        }

        public static string ToSpeechEmailDetailOverallString(Message message, TimeZoneInfo timeZone)
        {
            string speakString = string.Empty;

            if (message != null)
            {
                string time = message.ReceivedDateTime == null
                    ? CommonStrings.NotAvailable
                    : message.ReceivedDateTime.Value.UtcDateTime.ToRelativeString(timeZone);
                string sender = (message.Sender?.EmailAddress?.Name != null) ? message.Sender.EmailAddress.Name : EmailCommonStrings.UnknownSender;
                string subject = (message.Subject != null) ? message.Subject : EmailCommonStrings.EmptySubject;
                speakString = string.Format(EmailCommonStrings.FromDetailsFormatAll, sender, time, subject);
            }

            return speakString;
        }

        public static string ToSpeechEmailDetailString(Message message, TimeZoneInfo timeZone, bool isNeedContent = false)
        {
            string speakString = string.Empty;

            if (message != null)
            {
                string time = message.ReceivedDateTime == null
                    ? CommonStrings.NotAvailable
                    : message.ReceivedDateTime.Value.UtcDateTime.ToRelativeString(timeZone);
                string subject = message.Subject ?? EmailCommonStrings.EmptySubject;
                string sender = (message.Sender?.EmailAddress?.Name != null) ? message.Sender.EmailAddress.Name : EmailCommonStrings.UnknownSender;
                string content = message.Body.Content ?? EmailCommonStrings.EmptyContent;

                var stringFormat = isNeedContent ? EmailCommonStrings.FromDetailsWithContentFormat : EmailCommonStrings.FromDetailsFormatAll;
                speakString = string.Format(stringFormat, sender, time, subject, content);
            }

            return speakString;
        }

        public static string ToSpeechEmailSendDetailString(string detailSubject, string detailToRecipient, string detailContent)
        {
            string speakString = string.Empty;

            string subject = (detailSubject != string.Empty) ? detailSubject : EmailCommonStrings.EmptySubject;
            string toRecipient = (detailToRecipient != string.Empty) ? detailToRecipient : EmailCommonStrings.UnknownRecipient;
            string content = (detailContent != string.Empty) ? detailContent : EmailCommonStrings.EmptyContent;

            speakString = string.Format(EmailCommonStrings.ToDetailsFormat, subject, toRecipient, content);

            return speakString;
        }
    }
}
