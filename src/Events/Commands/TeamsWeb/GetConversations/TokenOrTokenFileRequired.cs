using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Events.TeamsWeb
{
    public interface ITokenOrTokenFile
    {
        string Token { get; set;}
        string TokenFile { get; set;}
    }

    public static class TokenOrTokenFileExtensions
    {
        public static string GetToken(this ITokenOrTokenFile tokenOrFile)
        {
            if (!string.IsNullOrEmpty(tokenOrFile.Token))
                return tokenOrFile.Token;
            return File.ReadAllText(tokenOrFile.TokenFile).Trim();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TokenOrTokenFileRequired : ValidationAttribute
    {
        public TokenOrTokenFileRequired() : base("Either --token or --token-file must be specified, but not both") {}

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is ITokenOrTokenFile tokenOrFile)
            {
                if (string.IsNullOrWhiteSpace(tokenOrFile.Token) && string.IsNullOrWhiteSpace(tokenOrFile.TokenFile))
                    return new ValidationResult("Either --token or --token-file must be specified");
                if (!string.IsNullOrWhiteSpace(tokenOrFile.Token) && !string.IsNullOrWhiteSpace(tokenOrFile.TokenFile))
                    return new ValidationResult("Cannot specify both --token and --token-file");
                if (!string.IsNullOrEmpty(tokenOrFile.TokenFile) && !File.Exists(tokenOrFile.TokenFile))
                    return new ValidationResult("Token file does not exist");
                return ValidationResult.Success;
            }
            else
            {
                throw new InvalidCastException($"Commands passed to this must implement {nameof(ITokenOrTokenFile)}");
            }
        }
    }
}