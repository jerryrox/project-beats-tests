using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PBFramework.DB.Entities.Tests
{
    public class TestEntity : DatabaseEntity {

        [Indexed]
        public int Age { get; set; }

        [Indexed]
        [JsonProperty("Name")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [JsonIgnore]
        public int YearsLeft => 100 - Age;


        public TestEntity()
        {
            
        }
    }
}