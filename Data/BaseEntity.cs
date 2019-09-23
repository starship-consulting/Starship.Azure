using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Starship.Core.Interfaces;

namespace Starship.Azure.Data {
    public abstract class BaseEntity : HasIdentity {

        protected BaseEntity() {
            CreationDate = DateTime.UtcNow;
        }

        public string GetId() {
            return Id.ToString();
        }

        public void SetId(object value) {
            Id = int.Parse(value.ToString());
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }
    }
}