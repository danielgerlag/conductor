using Conductor.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Conductor.Storage.Models
{
    public class StoredDefinition
    {
        public ObjectId Id { get; set; }

        public Definition Definition { get; set; }
    }
}
