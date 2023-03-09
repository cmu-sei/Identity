// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using Identity.Accounts.Models;
using Tests.Common;
using Xunit;

namespace Identity.Accounts.Tests
{
    public class CertificateTests : TestCore
    {

        [Theory]
        [InlineData("C=US, O=U.S. Government, OU=Department of Energy, CN=\"FIRST LAST (Affiliate)+UID=89001234567890\"", "first.last", "89001234567890.Department of Energy")]
        [InlineData("C=US, O=U.S. Government, OU=Department of Homeland Security, OU=DHS HQ, OU=People, CN=\"FIRST M LAST (affiliate)+UID=0754321000.DHS HQ\"", "first.last", "0754321000.DHS HQ")]
        [InlineData("C=US, O=U.S. Government, OU=DoD, OU=PKI, OU=CONTRACTOR, CN=LAST.FIRST.MIDDLE.1234567890", "first.last", "1234567890.DoD")]
        [InlineData("CN=LAST.FIRST.MIDDLE.1234567890,OU=CONTRACTOR,OU=PKI,OU=DoD,O=U.S. Government,C=US", "first.last", "1234567890.DoD")]
        [InlineData("CN=FIRST M LAST (affiliate)+UID=0754321000.DHS HQ,OU=People,OU=DHS HQ,OU=Department of Homeland Security,O=U.S. Government,C=US", "first.last", "0754321000.DHS HQ")]
        [InlineData("UID=0754321000.DHS HQ+CN=FIRST M LAST (affiliate),OU=People,OU=DHS HQ,OU=Department of Homeland Security,O=U.S. Government,C=US", "first.last", "0754321000.DHS HQ")]
        [InlineData("CN=FIRST LAST (Affiliate)+UID=89001234567890,OU=Department of Energy,O=U.S. Government,C=US", "first.last", "89001234567890.Department of Energy")]
        [InlineData("UID=0123456789.DHS HQ+CN=FIRST I LAST,OU=People,OU=DHS HQ,OU=Department of Homeland Security,O=U.S. Government,C=US", "first.last", "0123456789.DHS HQ")]
        // [InlineData("O=ORG,OU=TEST,CN=FIRST M LAST-TWO", "first.last-two")]
        // [InlineData("O=ORG,OU=TEST,CN=FIRST M LAST TWO", "first.last.two")]
        // [InlineData("O=ORG,OU=TEST,CN=FIRST MIDDLE LAST TWO", "first.middle.last.two")]
        // [InlineData("O=ORG,OU=ORG,OU=TEST,CN=FIRST MIDDLE LAST (meta)+UID=12345", "first.middle.last")]
        // [InlineData("O=ORG,CN=FIRST M L'AST,OU=TEST", "first.last")]
        // [InlineData("O=ORG,CN=FIRST M 'LAS'T,OU=TEST", "first.last")]
        // [InlineData("O=ORG,CN=FIRST M ORG'PO,OU=TEST", "first.orgpo")]
        // [InlineData("O=ORG,CN=FIRST M ORG'PO'DO,OU=TEST", "first.orgpodo")]
        public void Subjects_Parse(string subject, string result, string externalId)
        {
            var r = new CertificateSubjectDetail(subject);
            Assert.True(r.UserName == result);
            Assert.True(r.ExternalId == externalId);
        }

        [Theory]
        [InlineData("admin@this.ws", "Admin")]
        [InlineData("admin.last@this.ws", "Admin Last")]
        [InlineData("first.i.last.ctr@that.site", "First Last Ctr")]
        [InlineData("First.Last", "First Last")]
        [InlineData("username+123@this.ws", "Username")]
        public void DeriveNameFromEmail(string email, string result)
        {
            var c = new Credentials { Username = email };
            Assert.True(c.DisplayName == result);
        }

    }
}
