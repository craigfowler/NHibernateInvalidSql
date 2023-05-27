# NHibernate regression

This project demonstrates a reproduction case for [a bug/regression I have found in NHibernate].  The bug causes NHibernate to produce syntactically invalid SQL in the described scenario.
The _last good version_ in which the bug cannot be reproduced is v5.3.10.
The _first bad version_ in which I can reproduce the issue is v5.3.11.
I have also been able to reproduce in v5.4.2 which as at the time of writing, is the latest version.

The nature of the issue is that, for the mappings described here and the query performed, NHibernate generates invalid SQL which is rejected by the database.

[a bug/regression I have found in NHibernate]: https://github.com/nhibernate/nhibernate-core/issues/3306

## How to reproduce

This solution/project has three build configurations defined: `NH5310`, `NH5311` & `NH542`.
If the configuration is not specified when building/running, then effectively `NH5311` will be used by default.

### Prerequisites

* You'll need an MS SQL Server installed at `localhost`, with a database named `nh_invalid_sql` accessible via Windows Integrated Security.
    * You may tweak these details in `Program.cs` in the connection string at line 15 if your environment differs.
* You'll need a **dotnet SDK v6** or higher installed.

### Reproduction steps

1. Clone this repo
2. Ensure you have satisfied the prerequisites above
3. _(Optional)_ setup that database with the database schema from `InitialSchema.sql`
   * The nature of the error is apparent even if you don't set up the schema
   * Skipping the schema setup is helpful when diagnosing because the 'table does not exist' exception from the driver will reveal the SQL which NHibernate sent to the DB
4. Run the app using NHibernate 5.3.10: `dotnet run --project NHibernate.InvalidSql -c NH5310` and note the output
5. Run the app using NHibernate 5.3.11: `dotnet run --project NHibernate.InvalidSql -c NH5311` and note the output
6. Run the app using NHibernate 5.4.2: `dotnet run --project NHibernate.InvalidSql -c NH542` and note the output

### Expected result

When using _any version_ of NHibernate (steps 4, 5 or 6 above), the application should complete without error and emit
a result looking like the sample result below.
If you skipped the initial schema setup (step 3 above) then you should expect a crash error from the SQL driver, in which the error indicates that the SQL
was syntactically correct but that a database object did not exist, in my case `System.Data.SqlClient.SqlException: Invalid object name 'u_entity'.`.

Here's the sample expected result if the schema is set-up.

```text
[UEntity#[UKey#PEntity=[PEntity#1], Id=1]]
```

### Actual result

When using NHibernate 5.3.10 (step 4 above) you receive the expected result but when using either NHbernate 5.3.11 or 5.4.2 (steps 5 & 6 above)
you receive a different error message because the SQL sent to the database is **syntactically invalid**.
The error message generated from the database is:

```text
System.Data.SqlClient.SqlException: Incorrect syntax near 't_entity'.
Incorrect syntax near ')'.
Incorrect syntax near ')'.
```

## Analysis

I have placed the SQL produced by NHibernate 5.3.10 into a file named `SQL5310.sql` (reformatted for readability) and the SQL produced by NHibernate 5.3.11 into a file named `SQL5311.sql` (again, reformatted).
Each file also includes comments indicating the parameter values sent.

The SQL produced by these two versions of NHibernate differs; the difference is shown in a screenshot from KDiff3 in the file `SQLDiff.png`.
As you will see, the 5.3.11 SQL omits a comma in a multi-table select subquery (I believe that this is what makes the SQL syntactically invalid).

The 5.3.11 SQL also omits the many-to-one formula from the criteria of that same subquery.
This would likely cause incorrect results if not also fixed.
