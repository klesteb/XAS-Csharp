using System;

namespace XAS.Rest.Client.Serializers.Lib {

    public class StateValue {

        public string Type { get; private set; }
        public object Value { get; private set; }

        public StateValue(Object value, String type) {

            if (string.IsNullOrEmpty(type)) {

                throw new ArgumentNullException(nameof(type));

            }

            Type = type;
            Value = value;

        }

    }

}
