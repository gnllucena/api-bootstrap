  
CREATE SCHEMA `api` DEFAULT CHARACTER SET utf8 ;

use api;

create table user
(
	id int auto_increment,
	name varchar(100) null,
	email varchar(100) not null,
	password varchar(255) not null,
	active bit default true not null,
    confirmed bit default false not null,
	created datetime not null,
	createdby varchar(100) not null,
	updated datetime default NULL null,
	updatedby varchar(100) null,
	constraint user_pk
		primary key (id)
);

create unique index user_email_uindex
	on user (email);