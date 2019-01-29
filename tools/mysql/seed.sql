CREATE SCHEMA `bootstrap` DEFAULT CHARACTER SET utf8 ;

CREATE TABLE `bootstrap`.`User` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `IdProfile` INT NOT NULL,
  `IdCountry` INT NOT NULL,
  `CreatedBy` VARCHAR(80),
  `Name` VARCHAR(80) NOT NULL,
  `Email` VARCHAR(80) NOT NULL,
  `Document` VARCHAR(11) NOT NULL,
  `Birthdate` DATE NOT NULL,
  `Active` BOOLEAN NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Email_UNIQUE` (`Email` ASC) VISIBLE,
  UNIQUE INDEX `Document_UNIQUE` (`Document` ASC) VISIBLE);

CREATE TABLE `bootstrap`.`Profile` (
  `Id` INT NOT NULL,
  `Description` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Description_UNIQUE` (`Description` ASC) VISIBLE);

CREATE TABLE `bootstrap`.`Country` (
  `Id` INT NOT NULL,
  `Description` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Description_UNIQUE` (`Description` ASC) VISIBLE);

ALTER TABLE `bootstrap`.`User` 
ADD INDEX `CreatedBy_idx` (`CreatedBy` ASC) VISIBLE;

ALTER TABLE `bootstrap`.`User` 
ADD INDEX `IdCountry_idx` (`IdCountry` ASC) VISIBLE;
;
ALTER TABLE `bootstrap`.`User` 
ADD CONSTRAINT `IdCountry`
  FOREIGN KEY (`IdCountry`)
  REFERENCES `bootstrap`.`Country` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `bootstrap`.`User` 
ADD INDEX `IdProfile_idx` (`IdProfile` ASC) VISIBLE;
;
ALTER TABLE `bootstrap`.`User` 
ADD CONSTRAINT `IdProfile`
  FOREIGN KEY (`IdProfile`)
  REFERENCES `bootstrap`.`Profile` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

INSERT INTO `bootstrap`.`Country` (`Id`, `Description`) VALUES 
  ('1', 'Kanto'),
  ('2', 'Johto'),
  ('3', 'Hoenn'),
  ('4', 'Sinnoh'),
  ('5', 'Unova'),
  ('6', 'Kalos'),
  ('7', 'Alola');

INSERT INTO `bootstrap`.`Profile` (`Id`, `Description`) VALUES 
  ('1', 'Administrator'),
  ('2', 'Regular');

INSERT INTO `bootstrap`.`User` (`IdProfile`, `IdCountry`, `CreatedBy`, `Name`, `Email`, `Document`, `Birthdate`, `Active`) VALUES 
  ('1', '1', 'gnllucena@gmail.com', 'Pikachu', 'pikachu@gmail.com', '02343767327', '1991-04-28', '1'),
  ('2', '1', 'gnllucena@gmail.com', 'Charmander', 'charmander@gmail.com', '72232072053', '1991-04-28', '1'),
  ('2', '1', 'gnllucena@gmail.com', 'Squirtle', 'squirtle@gmail.com', '57151429037', '1991-04-28', '1'),
  ('2', '1', 'gnllucena@gmail.com', 'Bulbasaur', 'bulbasaur@gmail.com', '78286413032', '1991-04-28', '0'),
  ('1', '1', 'gnllucena@gmail.com', 'Mew', 'mew@gmail.com', '80895608065', '1991-04-28', '1'),
  ('2', '1', 'guhanda@yahoo.com.br', 'Chansey', 'chansey@gmail.com', '32616635020', '1991-04-28', '1'),
  ('1', '3', 'guhanda@yahoo.com.br', 'Blaziken', 'blaziken@gmail.com', '04617643001', '1991-04-28', '0'),
  ('2', '2', 'guhanda@yahoo.com.br', 'Feraligart', 'feraligart@gmail.com', '03143927003', '1991-04-28', '1'),
  ('2', '2', 'guhanda@yahoo.com.br', 'Chikorita', 'chikorita@gmail.com', '04298179087', '1991-04-28', '1'),
  ('2', '3', 'gnllucena@gmail.com', 'Torchic', 'torchic@gmail.com', '33017452026', '1991-04-28', '0');