create schema if not exists extensions;

-- make sure everybody can use everything in the extensions schema
grant usage on schema extensions to public;
grant execute on all functions in schema extensions to public;

-- include future extensions
alter default privileges in schema extensions
   grant execute on functions to public;

alter default privileges in schema extensions
   grant usage on types to public;

create extension if not exists pgcrypto schema extensions;

ALTER USER postgres SET search_path to public, extensions;

DROP TABLE IF EXISTS public.networks;

CREATE TABLE public.networks (
  id int primary key not null,
  name text NOT NULL
);

insert into public.networks 
select 10, 'Yammer';

CREATE TABLE public.keys (
  name text,
  password text,
  key bytea,
  CONSTRAINT pk_keys PRIMARY KEY (name)
)
