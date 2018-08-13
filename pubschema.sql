-- fileappdb schema
CREATE TABLE users (
    id serial PRIMARY KEY,
    email varchar(255) NOT NULL UNIQUE,
    password bytea NOT NULL,
    salt bytea NOT NULL
);

CREATE TABLE uploads (
    id      serial PRIMARY KEY,
    name    varchar(255) NOT NULL,
    sha256  bytea NOT NULL, 
    blob    bytea,
    date    date NOT NULL,
    owner   integer NOT NULL references users(id) 
);
