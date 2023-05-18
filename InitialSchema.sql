BEGIN TRAN;

-- I have been lazy and omitted FK constraints but they don't affect the repro case

CREATE TABLE u_entity (
    p_id BIGINT NOT NULL,
    id BIGINT NOT NULL,
    PRIMARY KEY (p_id, id)
);

CREATE TABLE p_entity (
	id BIGINT NOT NULL,
	p_status VARCHAR(100) NOT NULL,
	PRIMARY KEY (id)
);

CREATE TABLE d_entity (
	id BIGINT NOT NULL,
	linked_entity_id BIGINT NOT NULL,
	linked_entity_type VARCHAR(100) NOT NULL,
	PRIMARY KEY (id)
);

CREATE TABLE l_entity (
	id BIGINT NOT NULL,
	t_status VARCHAR(100) NOT NULL,
	t_id BIGINT NOT NULL,
	p_id BIGINT NOT NULL,
	u_id BIGINT NOT NULL,
	PRIMARY KEY (id)
);

CREATE TABLE t_entity (
	id BIGINT NOT NULL,
	PRIMARY KEY (id)
);

COMMIT;

BEGIN TRAN;

-- Here's some sample data which would allow you to get a result from the query

INSERT INTO t_entity (id) VALUES (1), (2), (3);
INSERT INTO p_entity (id, p_status) VALUES (1, 'Good PStatus'), (2, 'Bad PStatus');
INSERT INTO u_entity (p_id, id) VALUES (1, 1), (1, 2), (1, 3), (2, 1);
INSERT INTO l_entity (id, t_status, t_id, p_id, u_id) VALUES
    (1, 'Good TStatus', 1, 1, 1),
    (2, 'Bad TStatus', 1, 3, 2),
    (3, 'Good TStatus', 1, 2, 2),
    (4, 'Good TStatus', 2, 2, 3);
INSERT INTO d_entity (id, linked_entity_id, linked_entity_type) VALUES
    (1, 1, 'T'),
    (2, 2, 'T'),
    (3, 3, 'T'),
    (4, 0, 'Z');

COMMIT;
