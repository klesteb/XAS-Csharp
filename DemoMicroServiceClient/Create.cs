using System;
using System.Collections.Generic;

using DemoModelCommon.DataStructures;

namespace DemoMicroServiceClient {

    /// <summary>
    /// Utility class to create Dinosaur data structures from JSON.
    /// </summary>
    /// 
    public static class Create {

        /// <summary>
        /// Create a new DinosaurDTO object.
        /// </summary>
        /// <param name="json">Parsed JSON data.</param>
        /// <returns>A new DinosaurDTO object.</returns>
        /// 
        public static DinosaurDTO DinosaurDTO(dynamic json) {

            var dto = new DinosaurDTO();

            foreach (KeyValuePair<string, object> item in json) {

                switch (item.Key) {
                    case "id":
                        dto.Id = Convert.ToInt32(item.Value);
                        break;
                    case "name":
                        dto.Name = (String)item.Value;
                        break;
                    case "status":
                        dto.Status = (String)item.Value;
                        break;
                    case "height":
                        dto.Height = Convert.ToInt32(item.Value);
                        break;
                }

            }

            return dto;

        }

    }

}
