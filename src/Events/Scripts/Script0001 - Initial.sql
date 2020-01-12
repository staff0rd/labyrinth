DROP TABLE IF EXISTS staff0rd.events;
DROP TABLE IF EXISTS staff0rd.users;
DROP TABLE IF EXISTS public.networks;

CREATE TABLE public.networks (
  id int primary key not null,
  name text NOT NULL
);

insert into public.networks 
select 10, 'Yammer';

CREATE TABLE staff0rd.users (
  id uuid primary key NOT NULL,
  network int NOT NULL references public.networks(id),
  external_id text NOT NULL,
  unique(network, external_id),
  avatar_url text NULL,
  name text NOT NULL,
  known_since timestamp(6) NOT NULL,
  description text NULL
);

CREATE TABLE staff0rd.events (
  id int GENERATED ALWAYS AS IDENTITY primary key not null,
  entity_id uuid NOT NULL,
  network int NOT NULL references public.networks(id),
  event_name text NOT NULL,
  body jsonb NOT NULL,
  inserted_at timestamp(6) NOT NULL DEFAULT statement_timestamp()
);