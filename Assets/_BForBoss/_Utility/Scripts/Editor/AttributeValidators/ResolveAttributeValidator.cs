using System;
using Sirenix.OdinInspector.Editor.Validation;

[assembly: RegisterValidator(typeof(Perigon.Utility.ResolveAttributeValidator))]

namespace Perigon.Utility
{
    public class ResolveAttributeValidator : AttributeValidator<ResolveAttribute>
    {
        protected override void Validate(ValidationResult result)
        {
            Type fieldType = Property.Info.TypeOfValue;

           var isFieldTypeStruct = fieldType.IsValueType && !fieldType.IsPrimitive && fieldType != typeof(decimal) &&
                fieldType != typeof(DateTime) && !fieldType.IsEnum
            if (fieldType.IsValueType && !fieldType.IsPrimitive && fieldType != typeof(decimal) &&
                fieldType != typeof(DateTime) && !fieldType.IsEnum)
            {
                result.ResultType = ValidationResultType.Error;
                result.Message = "Unable to Resolve Struct Declared fields directly." +
                                 "\nPlease add [Resolve] onto specific fields within the struct instead";
            }
        }
    }
}
