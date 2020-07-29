using System;

namespace Identity.Accounts.Models
{
    public class AccountStats
    {
        public int AccountsCreated { get; set; }
        public int AccountsAuthed { get; set; }
        public DateTime Since { get; set; }
    }
}
