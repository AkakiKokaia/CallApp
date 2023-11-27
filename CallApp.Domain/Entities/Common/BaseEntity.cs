using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Entities.Common
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual int? CreatedById { get; set; }
        public virtual DateTime? UpdatedAt { get; set; }
        public virtual int? UpdatedById { get; set; }
        public virtual DateTime? DeletedAt { get; set; }
        public virtual bool IsDeleted { get; set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.Now;
            DeletedAt = null;
            IsDeleted = false;
        }

        #region Domain Events Logic
        private List<INotification> _domainEvents;

        public IReadOnlyList<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
        #endregion
    }
}
