select
  uentity0_.p_id as p1_0_,
  uentity0_.id as id2_0_
from u_entity uentity0_
where
  (
    exists (
      select pentity1_.id
      from p_entity pentity1_
      where
        pentity1_.p_status = @p0
        and uentity0_.p_id = pentity1_.id
    )
  )
  and (
    exists (
      select dentity2_.id
      from d_entity dentity2_
      where
        denti ty2_.id = @p1
        and (
          exists (
            select lentities3_.id
            from
              l_entity lentities3_,
              t_entity tentity4_
            where
              uentity0_.p_id = lentities3_.p_id
              and uentity0_.id = lentities3_.u_id
              and CASE
                WHEN dentity2_.linked_entity_type = 'T' THEN dentity2_.linked_entity_id
                ELSE NULL
              END = tentity4_.i d
              and (
                lentities3_.t_id = tentity4_.id
                or (lentities3_.t_id is null)
                and (tentity4_.id is null)
              )
          )
        )
    )
  )
  and (
    not (
      exists (
        select lentities6_.id
        from l_entity lentities6_
        where
          uentity0_.p_id = lentities6_.p_id
          and uentity0_.id = lentities6_.u_id
      )
    )
    or not (
      exists (
        select lentiti es7_.id
        from l_entity lentities7_
        where
          uentity0_.p_id = lentities7_.p_id
          and uentity0_.id = lentities7_.u_id
          and not (
            lentities7_.t_status <> @p2
            or lentities7_.t_status is null
          )
      )
    )
  )
  
/*

Parameter values sent by NHibernate:

@p0 = 'Good PStatus'
@p1 = 1
@p2 = 'Bad TStatus'

*/