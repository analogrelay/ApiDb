using System;
using System.Text.RegularExpressions;

namespace ApiDb.Model
{
    public struct ModelVersion: IEquatable<ModelVersion>
    {
        private static readonly Regex _modelVersionFormat = new Regex(@"^\d\d\d\d-\d\d-\d\d$");

        public string Value { get; }

        public static ModelVersion Current => v2019_08_02;

        public static readonly ModelVersion v2019_08_02 = new ModelVersion("2019-08-02");
        public static readonly ModelVersion Empty = new ModelVersion(string.Empty);

        public ModelVersion(string value)
        {
            if(value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if(!string.IsNullOrEmpty(value) && !_modelVersionFormat.IsMatch(value))
            {
                throw new FormatException("Model versions should be dates in the form: yyyy-MM-dd");
            }

            Value = value;
        }

        public bool Equals(ModelVersion other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is ModelVersion other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(ModelVersion left, ModelVersion right) => left.Equals(right);
        public static bool operator !=(ModelVersion left, ModelVersion right) => !left.Equals(right);
    }
}
