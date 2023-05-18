using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.InvalidSql;

public class UEntityMapping : ClassMapping<UEntity>
{
    public UEntityMapping()
    {
        Table("u_entity");
        
        ComponentAsId(x => x.Id, m =>
        {
            m.ManyToOne(x => x.PEntity, p => p.Column("p_id"));
            m.Property(x => x.Id, p => p.Column("id"));
        });
        
        Set(x => x.LEntities, m =>
        {
            m.Inverse(true);
            m.Key(k =>
            {
                k.Columns(c => c.Name("p_id"), c => c.Name("u_id"));
                k.NotNullable(true);
            });
        },
            a => a.OneToMany());
    }
}

public class PEntityMapping : ClassMapping<PEntity>
{
    public PEntityMapping()
    {
        Table("p_entity");
        
        Id(x => x.Id, p => p.Column("id"));
        
        Property(x => x.PStatus, p => p.Column("p_status"));
    }
}

public class DEntityMapping : ClassMapping<DEntity>
{
    public DEntityMapping()
    {
        Table("d_entity");
        
        Id(x => x.Id, p => p.Column("id"));
        
        Property(x => x.LinkedEntityId, p => p.Column("linked_entity_id"));
        
        Property(x => x.LinkedEntityType, p => p.Column("linked_entity_type"));
        
        /* I recognise that this is poor schema design, in which we have a "foreign key type" column,
         * meaning that we cannot use a normal many to one mapping.
         * This is a legacy database schema in which we require backwards compatibility with a legacy app.
         */
        ManyToOne(x => x.LinkedTEntity, m =>
        {
            m.Cascade(Cascade.None);
            m.Update(false);
            m.Insert(false);
            m.NotFound(NotFoundMode.Ignore);
            m.NotNullable(false);
            m.Fetch(FetchKind.Join);
            m.Formula("CASE WHEN linked_entity_type = 'T' THEN linked_entity_id ELSE NULL END");
        });
    }
}

public class LEntityMapping : ClassMapping<LEntity>
{
    public LEntityMapping()
    {
        Table("l_entity");
        
        Id(x => x.Id, p => p.Column("id"));
        
        Property(x => x.TStatus, p => p.Column("t_status"));
        
        ManyToOne(x => x.TEntity, m => m.Column("t_id"));
        
        ManyToOne(x => x.UEntity, m => m.Columns(c => c.Name("p_id"), c => c.Name("u_id")));
    }
}

public class TEntityMapping : ClassMapping<TEntity>
{
    public TEntityMapping()
    {
        Table("t_entity");
        
        Id(x => x.Id, p => p.Column("id"));
    }
}
