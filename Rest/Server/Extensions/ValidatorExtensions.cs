using System;
using System.Text.RegularExpressions;

using FluentValidation;
using FluentValidation.Validators;

namespace XAS.Rest.Server.Extensions {

    /// <summary>
    /// Initialize the validators for usage.
    /// </summary>
    /// 
    public static class ValidatorExtensions {

        public static IRuleBuilderOptions<T, String> IsInteger<T, String>(this IRuleBuilder<T, String> ruleBuilder) {

            return ruleBuilder.SetValidator(new IsIntegerValidator());

        }

        public static IRuleBuilderOptions<T, String> IsDateTime<T, String>(this IRuleBuilder<T, String> ruleBuilder) {

            return ruleBuilder.SetValidator(new IsDateTimeValidator());

        }

        public static IRuleBuilderOptions<T, String> IsBoolean<T, String>(this IRuleBuilder<T, String> ruleBuilder) {

            return ruleBuilder.SetValidator(new IsBooleanValidator());

        }

        public static IRuleBuilderOptions<T, String> IsPort<T, String>(this IRuleBuilder<T, String> ruleBuilder) {

            return ruleBuilder.SetValidator(new IsPortValidator());

        }

    }

    /// <summary>
    /// Check to see if a string is an integer.
    /// </summary>
    /// 
    public class IsIntegerValidator: PropertyValidator {

        private readonly Regex regex;
        const string expression = @"\d+";

        public IsIntegerValidator() : base("{PropertyName} must be an integer") {

            regex = new Regex(expression, RegexOptions.IgnoreCase);

        }

        protected override Boolean IsValid(PropertyValidatorContext context) {

            bool stat = false;

            if (context.PropertyValue != null) {

                stat = true;

                if (!regex.IsMatch((string)context.PropertyValue)) {

                    stat = false;

                }

            }

            return stat;

        }

    }

    /// <summary>
    /// Check to see if a string is a boolean value.
    /// </summary>
    /// 
    public class IsBooleanValidator: PropertyValidator {

        public IsBooleanValidator() : base("{PropertyName} in not a boolean value") { }

        protected override Boolean IsValid(PropertyValidatorContext context) {

            bool stat = true;

            try {

                Convert.ToBoolean((string)context.PropertyValue);

            } catch {

                stat = false;

            }

            return stat;

        }

    }

    /// <summary>
    /// Check the string to see if it is a valid DateTime.
    /// </summary>
    /// 
    public class IsDateTimeValidator: PropertyValidator {

        public IsDateTimeValidator() : base("{PropertyName} in not a valid datetime") { }

        protected override Boolean IsValid(PropertyValidatorContext context) {

            DateTime dt;
            bool stat = false;

            if (DateTime.TryParse((string)context.PropertyValue, out dt)) {

                stat = true;

            }

            return stat;

        }

    }

    /// <summary>
    /// Check the string to see if it is a valid IP Port.
    /// </summary>
    /// 
    public class IsPortValidator: PropertyValidator {

        public IsPortValidator() : base("{PropertyName} in not a valid IP port") { }

        protected override Boolean IsValid(PropertyValidatorContext context) {

            bool stat = true;

            try {

                var port = Convert.ToUInt16((string)context.PropertyValue);

                if ((port > 0) && (port <= 65535)) {

                    stat = true;

                }

            } catch {

                stat = false;

            }

            return stat;

        }

    }


}
