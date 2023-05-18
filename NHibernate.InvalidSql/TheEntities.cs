namespace NHibernate.InvalidSql;

// These entities happen to match our domain & mappings.  I've anonymized them though.

public class UEntity
{
    public virtual UKey Id { get; set; }
    
    public virtual ISet<LEntity> LEntities { get; protected set; } = new HashSet<LEntity>();

    public override string ToString() => $"[{nameof(UEntity)}#{Id}]";
}

/// <summary>
/// Represents the ID of a <see cref="UEntity"/>.  This is a reference to a <see cref="PEntity"/> and an ID which
/// is unique only within a <see cref="PEntity"/>.
/// </summary>
public class UKey
{
    public virtual PEntity? PEntity { get; init; }
    
    public virtual long Id { get; init; }

    public override string ToString() => $"[{nameof(UKey)}#{nameof(PEntity)}={PEntity}, {nameof(Id)}={Id}]";

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null)) return false;
        if (PEntity is null) return false;
        if (obj is not UKey uKey) return false;
        if (uKey.PEntity is null) return false;

        return uKey.Id == Id && uKey.PEntity.Id == PEntity.Id;
    }

    public override int GetHashCode() => Id.GetHashCode() ^ (PEntity?.Id).GetValueOrDefault().GetHashCode();
}

public class PEntity
{
    public virtual long Id { get; set; }
    
    public virtual string PStatus { get; set; }

    public override string ToString() => $"[{nameof(PEntity)}#{Id}]";
}

public class DEntity
{
    public virtual long Id { get; set; }

    public virtual long LinkedEntityId { get; set; }
    
    public virtual string LinkedEntityType { get; set; }
    
    public virtual TEntity LinkedTEntity { get; protected set; }
}

public class LEntity
{
    public virtual long Id { get; set; }
    
    public virtual string TStatus { get; set; }

    public virtual TEntity TEntity { get; set; }

    public virtual UEntity UEntity { get; set; }
}

public class TEntity
{
    public virtual long Id { get; set; }
}