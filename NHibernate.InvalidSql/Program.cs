// See https://aka.ms/new-console-template for more information

using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.InvalidSql;
using NHibernate.Mapping.ByCode;
using Environment = System.Environment;

var config = new Configuration();
config.DataBaseIntegration(x =>
{
    x.ConnectionString = "Data Source=.;Database=nh_invalid_sql;Integrated Security=true";
    x.Driver<SqlClientDriver>();
    x.Dialect<MsSql2012Dialect>();
    x.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
});
var mapper = new ModelMapper();
mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
config.AddDeserializedMapping(mapper.CompileMappingForAllExplicitlyAddedEntities(), "MappingByCode");

var sessionFactory = config.BuildSessionFactory();
using var session = sessionFactory.OpenSession();

var pEntities = session.Query<PEntity>().Where(p => p.PStatus == "Good PStatus");
var dEntities = session.Query<DEntity>().Where(d => d.Id == 1);
    
// The reason for the three separate .Where clauses is that in our own impl, these three expressions come from three
// separate & independent specification classes.  Each represents an independent business rule.
var query = session.Query<UEntity>()
                       .Where(u => pEntities.Any(p => u.Id.PEntity == p))
                       .Where(u => dEntities.Any(d => u.LEntities.Any(l => l.TEntity == d.LinkedTEntity)))
                        // The part of this query which states !u.LEntities.Any() is not strictly necessary, but works
                        // around some other suboptimal SQL in other scenarios.
                       .Where(u => !u.LEntities.Any() || u.LEntities.All(l => l.TStatus != "Bad TStatus"));

try
{
    var uEntities = query.ToList();
    Console.WriteLine(String.Join(Environment.NewLine, uEntities));
}
catch (Exception e)
{
    Console.WriteLine(e);
    Environment.Exit(1);
}

Environment.Exit(0);
