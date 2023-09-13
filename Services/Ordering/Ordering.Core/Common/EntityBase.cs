namespace Ordering.Core.Common;

/**
 * This file will serve as common field for domain
 * That means every entity will have below props by default in ording Microservice
 */
public abstract class EntityBase
{
    //protected set is made to use in the derived classes.
    public int Id { get; protected set; }
    //Below Properties are Audit properties
    public string? CreatedBy { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}