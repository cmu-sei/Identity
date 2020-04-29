// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;

namespace Identity.Accounts.Exceptions
{
    public class AccountException : Exception {
        public AccountException() {}
        public AccountException(string message) : base(message) {}
    }
    public class AccountLockedException : AccountException {
        public AccountLockedException(string duration) : base(duration){}
    }
    public class AccountDisabledException : AccountException { }
    public class AccountNotFoundException: AccountException { }
    public class AccountNotUniqueException : AccountException { }
    public class AccountNotConfirmedException : AccountException { }
    public class AccountTokenInvalidException : AccountException { }
    public class MultipleCredentialsNotAllowedException : AccountException { }
    public class AccountRemovalException : AccountException { }
    public class AuthenticationFailureException : AccountException { }
    public class PasswordComplexityException : AccountException { }
    public class PasswordHistoryException : AccountException { }
    public class PasswordExpiredException : AccountException { }
    public class RegistrationDomainException : AccountException { }

}
