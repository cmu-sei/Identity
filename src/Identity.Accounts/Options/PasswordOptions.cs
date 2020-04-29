// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

namespace Identity.Accounts.Options
{
    public class PasswordOptions
    {
        public string ComplexityExpression { get; set; } = @"(?=^.{8,}$)(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[`~!@#$%^&*\(\)\-_=+\[\]\{\}\\|;:'"",<\.>/?\ ]).*$";
        public string ComplexityText { get; set; } = "At least 8 characters containing uppercase and lowercase letters, numbers, and symbols";
        public int History { get; set; }
        public int Age { get; set; }
        public int ResetTokenExpirationMinutes { get; set; } = 60;
        public string InitialResetCode { get; set; }
    }
}
