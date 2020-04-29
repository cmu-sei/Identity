// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool HasCertificate(
            this HttpRequest request,
            string header,
            out X509Certificate2 cert
        ){
            cert = request.HttpContext.Connection.ClientCertificate;
            if (cert == null)
            {
                string xcert = request.Headers[header];
                if (!String.IsNullOrEmpty(xcert))
                {
                    cert = new X509Certificate2(Convert.FromBase64String(xcert));
                }
            }
            return cert != null;
        }

        public static bool HasValidatedSubject(
            this HttpRequest request,
            string certHeader,
            string subjectHeader,
            string verifyHeader,
            out string subject
        ){
            subject = request.GetCertificateSubject(certHeader, subjectHeader);
            string verify = request.Headers[verifyHeader];
            return !string.IsNullOrEmpty(subject) && verify.StartsWith("success",StringComparison.OrdinalIgnoreCase);
        }

        public static string GetCertificateSubject(
            this HttpRequest request,
            string certHeader,
            string subjectHeader
        ){
            if (request.HasCertificate(certHeader, out X509Certificate2 certificate2))
                return certificate2.Subject;

            if (string.IsNullOrEmpty(subjectHeader))
                return "";

            return request.Headers[subjectHeader];
        }

        public static string GetCertificateIssuer(
            this HttpRequest request,
            string certHeader,
            string issuerHeader
        ){
            if (request.HasCertificate(certHeader, out X509Certificate2 certificate2))
                return certificate2.Issuer;

            if (string.IsNullOrEmpty(issuerHeader))
                return "";

            return request.Headers[issuerHeader];
        }

        public static bool IsPrivileged(this ClaimsPrincipal user)
        {
            return user.IsInRole(AppConstants.AdminRole) || user.IsInRole(AppConstants.ManagerRole);
        }

        public static bool IsPrivilegedOrSelf(this ClaimsPrincipal user, string subject)
        {
            return user.IsPrivileged() || user.GetSubjectId() == subject;
        }

        public static string GetSubjectId(this ClaimsPrincipal user)
        {
            string id = user.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (string.IsNullOrEmpty(id))
                id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return id;
        }

        public static string GetSubjectName(this ClaimsPrincipal user)
        {
            return user.Identity.Name;
        }

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(JwtClaimTypes.Role)?.Value;
        }

    }
}
