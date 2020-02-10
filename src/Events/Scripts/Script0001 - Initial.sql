DROP TABLE IF EXISTS public.networks;

CREATE TABLE public.networks (
  id int primary key not null,
  name text NOT NULL
);

insert into public.networks 
select 10, 'Yammer';
